using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace LinkeD365.FlowToVisio
{
    public partial class ApiConnection : Form
    {
        public APIConns apiConns;
        public FlowConn flowConn;
        public LogicAppConn laConn;

        private bool LogicApp;

        // Azure Active Directory registered app clientid for Microsoft samples
        private const string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";

        // Azure Active Directory registered app Redirect URI for Microsoft samples
        private Uri redirectUri = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");

        private const string subscriptionText = "Azure Subscription Id";
        private const string appText = "Client Id for Configured App Registration";
        private const string tenantText = "Azure Tenant Id";
        private const string returnText = "Return URL for Configured App Regisration";
        private const string labelText = "Label for Connection";
        private const string environmentText = "Power Automate Environment Id";

        public ApiConnection(APIConns apiConnections, bool logicApp)
        {
            InitializeComponent();
            apiConns = apiConnections;

            LogicApp = logicApp;

            if (!LogicApp)
            {
                if (apiConns.FlowConns.Any())
                {
                    cboFlowConns.Items.AddRange(apiConns.FlowConns.ToArray());
                    cboFlowConns.SelectedIndex = 0;
                }
                else
                {
                    EnableControls();
                    cboFlowConns.Enabled = false;

                    chkUseDevApp.CheckedChanged += ChkUseDevApp_CheckedChanged;
                }
            }
            else if (apiConns.LogicAppConns.Any())
            {
                cboLAConns.Items.AddRange(apiConns.LogicAppConns.ToArray());
                cboLAConns.SelectedIndex = 0;
            }
            else
            {
                EnableControls();
                cboLAConns.Enabled = false;
                chkLADev.CheckedChanged += ChkLADev_CheckedChanged;
            }

            panelFlow.Visible = !LogicApp;
            panelLogicApp.Visible = LogicApp;
        }

        private void EnableControls()
        {
            if (!LogicApp)
            {
                panelFlow.Controls.OfType<TextBox>().ToList().ForEach(ctl => ctl.Enabled = false);
                chkUseDevApp.Enabled = false;
                cboFlowConns.Enabled = false;

                //txtTenant.Enabled = false;
                //txtAppId.Enabled = false;
                //txtEnvironment.Enabled = false;
                //txtName.Enabled = false;
                //txtReturnURL.Enabled = false;
            }
            else
            {
                panelLogicApp.Controls.OfType<TextBox>().ToList().ForEach(ctl => ctl.Enabled = false);
                chkLADev.Enabled = false;
                cboLAConns.Enabled = false;

                //txtLATenant.Enabled = false;
                //txtLAApp.Enabled = false;
                //txtSubscriptionId.Enabled = false;
                //txtLAReturnURL.Enabled = false;
                //txtLAName.Enabled = false;
            }
        }

        public HttpClient GetClient()
        {
            if (ShowDialog() == DialogResult.OK)
            {
                if (LogicApp)
                {
                    laConn = apiConns.LogicAppConns.FirstOrDefault(la => la.Id == (int)lblLAName.Tag);
                    if (laConn == null)
                    {
                        laConn = new LogicAppConn();
                    }

                    laConn.Id = (int)lblLAName.Tag;
                    laConn.SubscriptionId = txtSubscriptionId.Text;
                    laConn.TenantId = txtLATenant.Text;
                    laConn.ReturnURL = txtLAReturnURL.Text;
                    laConn.AppId = txtLAApp.Text;
                    laConn.UseDev = chkLADev.Checked;
                    laConn.Name = txtLAName.Text;
                    var laIndex = apiConns.LogicAppConns.IndexOf(laConn);
                    if (laIndex == -1)
                    {
                        apiConns.LogicAppConns.Add(laConn);
                    }
                    else
                    {
                        apiConns.LogicAppConns[laIndex] = laConn;
                    }
                }
                else
                {
                    flowConn = apiConns.FlowConns.FirstOrDefault(flw => flw.Id == (int)lblName.Tag);
                    if (flowConn == null)
                    {
                        flowConn = new FlowConn();
                    }

                    flowConn.Id = (int)lblName.Tag;
                    flowConn.Environment = txtEnvironment.Text;
                    flowConn.TenantId = txtTenant.Text;
                    flowConn.ReturnURL = txtReturnURL.Text;
                    flowConn.AppId = txtAppId.Text;
                    flowConn.UseDev = chkUseDevApp.Checked;
                    flowConn.Name = txtName.Text;
                    var flwIndex = apiConns.FlowConns.IndexOf(flowConn);
                    if (flwIndex == -1)
                    {
                        apiConns.FlowConns.Add(flowConn);
                    }
                    else
                    {
                        apiConns.FlowConns[flwIndex] = flowConn;
                    }
                }
                return Connect();
            }

            return null;
        }

        private HttpClient Connect()
        {
            var token = GetInteractiveClientToken();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("accept", "application/json");
            return client;
        }

        private string GetInteractiveClientToken()
        {
            AuthenticationContext ac = new AuthenticationContext($"https://login.microsoftonline.com/{(LogicApp ? laConn.TenantId : flowConn.TenantId)}");
            string serviceURL = LogicApp ? "https://management.azure.com/" : "https://service.flow.microsoft.com/";
            string appId = LogicApp ? laConn.AppId : flowConn.AppId;

            try
            {
                return ac.AcquireTokenSilentAsync(serviceURL, appId).GetAwaiter().GetResult().AccessToken;
            }
            catch (AdalException adalException)
            {
                if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently
                    || adalException.ErrorCode == AdalError.InteractionRequired)
                {
                    return ac.AcquireTokenAsync(serviceURL, appId, new Uri(LogicApp ? laConn.ReturnURL : flowConn.ReturnURL),
                        new PlatformParameters(PromptBehavior.SelectAccount)).GetAwaiter().GetResult().AccessToken;
                }
            }

            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!LogicApp && (txtAppId.Text == string.Empty || txtAppId.Text == appText
                || txtTenant.Text == string.Empty || txtTenant.Text == tenantText
                || txtEnvironment.Text == string.Empty || txtEnvironment.Text == environmentText
                || txtReturnURL.Text == string.Empty || txtReturnURL.Text == returnText
                || txtName.Text == string.Empty || txtName.Text == labelText))
            {
                MessageBox.Show("Please ensure all fields have a value", "Required properties missing", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None;
                return;
            }
            if (LogicApp && (txtSubscriptionId.Text == string.Empty || txtSubscriptionId.Text == subscriptionText
                || txtLAApp.Text == string.Empty || txtLAApp.Text == appText
                || txtLAName.Text == string.Empty || txtLAName.Text == labelText
                || txtLAReturnURL.Text == string.Empty || txtLAReturnURL.Text == returnText
                || txtLATenant.Text == string.Empty || txtLATenant.Text == tenantText))
            {
                MessageBox.Show("Please ensure all fields have a value", "Required properties missing", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None;
            }
        }

        private void ChkUseDevApp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseDevApp.Checked)
            {
                MessageBox.Show(
                    "For development and prototyping purposes, Microsoft has provided AppId and Redirect URI for use in OAuth situations. " +
                        Environment.NewLine +
                        "For production use, create an AppId that is specific to your tenant in the Azure Management Portal", "Use only for Dev", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtAppId.Text = clientId;
                txtReturnURL.Text = redirectUri.ToString();
            }
            else
            {
                txtAppId.Text = "Client Id for Configured App Registration";
                txtReturnURL.Text = "Return URL for Configured App Regisration";

                txtAppId.ForeColor = Color.Silver;
                txtReturnURL.ForeColor = Color.Silver;
            }

            txtAppId.Enabled = !chkUseDevApp.Checked;
            txtReturnURL.Enabled = !chkUseDevApp.Checked;
        }

        private void ChkLADev_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLADev.Checked)
            {
                MessageBox.Show(
                    "For development and prototyping purposes, Microsoft has provided AppId and Redirect URI for use in OAuth situations. " +
                        Environment.NewLine +
                        "For production use, create an AppId that is specific to your tenant in the Azure Management Portal", "Use only for Dev", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLAApp.Text = clientId;
                txtLAReturnURL.Text = redirectUri.ToString();
            }
            else
            {
                txtLAApp.Text = "Client Id for Configured App Registration";
                txtLAReturnURL.Text = "Return URL for Configured App Regisration";
                txtLAApp.ForeColor = Color.Silver;
                txtLAReturnURL.ForeColor = Color.Silver;
            }
            txtLAApp.Enabled = !chkLADev.Checked;
            txtLAReturnURL.Enabled = !chkLADev.Checked;
        }

        private void cboLAConns_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkLADev.CheckedChanged -= ChkLADev_CheckedChanged;
            var selectedLAConn = cboLAConns.SelectedItem as LogicAppConn;
            lblLAName.Tag = selectedLAConn.Id;
            txtLATenant.Text = selectedLAConn.TenantId;
            txtLAApp.Text = selectedLAConn.AppId;
            txtLAReturnURL.Text = selectedLAConn.ReturnURL;
            chkLADev.Checked = selectedLAConn.UseDev;
            txtSubscriptionId.Text = selectedLAConn.SubscriptionId;
            txtLAName.Text = selectedLAConn.Name;
            panelLogicApp.Controls.OfType<TextBox>().ToList().ForEach(txt => txt.ForeColor = Color.Black);

            txtLAApp.Enabled = !chkLADev.Checked;
            txtLAReturnURL.Enabled = !chkLADev.Checked;
            chkLADev.CheckedChanged += ChkLADev_CheckedChanged;
        }

        private void cboFlowConns_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkUseDevApp.CheckedChanged -= ChkUseDevApp_CheckedChanged;
            var selectedConn = cboFlowConns.SelectedItem as FlowConn;
            lblName.Tag = selectedConn.Id;

            txtTenant.Text = selectedConn.TenantId;
            txtAppId.Text = selectedConn.AppId;
            txtReturnURL.Text = selectedConn.ReturnURL;
            chkUseDevApp.Checked = selectedConn.UseDev;
            txtEnvironment.Text = selectedConn.Environment;
            txtName.Text = selectedConn.Name;
            chkUseDevApp.CheckedChanged += ChkUseDevApp_CheckedChanged;
            panelFlow.Controls.OfType<TextBox>().ToList().ForEach(txt => txt.ForeColor = Color.Black);

            txtAppId.Enabled = !chkUseDevApp.Checked;
            txtReturnURL.Enabled = !chkUseDevApp.Checked;
        }

        private void btnAddLA_Click(object sender, EventArgs e)
        {
            lblLAName.Tag = apiConns.LogicAppConns.Any() ? apiConns.LogicAppConns.Max(la => la.Id) + 1 : 0;
            txtSubscriptionId.Text = subscriptionText;
            txtLATenant.Text = tenantText;
            txtLAApp.Text = appText;
            txtLAReturnURL.Text = returnText;
            chkLADev.Checked = false;
            txtLAName.Text = labelText;
            txtLAApp.Enabled = true;
            txtLAReturnURL.Enabled = true;
            txtLAName.Enabled = true;
            txtLATenant.Enabled = true;
            txtLAName.Enabled = true;
            txtSubscriptionId.Enabled = true;
            chkLADev.Enabled = true;
        }

        private void btnFlowAdd_Click(object sender, EventArgs e)
        {
            lblName.Tag = apiConns.FlowConns.Any() ? apiConns.FlowConns.Max(flw => flw.Id) + 1 : 0;
            txtEnvironment.Text = environmentText;
            txtTenant.Text = tenantText;
            txtAppId.Text = appText;
            txtReturnURL.Text = returnText;
            chkUseDevApp.Checked = false;
            txtName.Text = labelText;
            txtAppId.Enabled = true;
            txtReturnURL.Enabled = true;
            txtName.Enabled = true;
            txtTenant.Enabled = true;
            txtName.Enabled = true;
            txtEnvironment.Enabled = true;
            chkUseDevApp.Enabled = true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (cboFlowConns.SelectedItem == null)
            {
                return;
            }

            FlowConn removeFlow = (FlowConn)cboFlowConns.SelectedItem;
            if (MessageBox.Show("Remove the Power Automate Connection '" + removeFlow.Name + "' ?",
                    "Remove Flow Connection", MessageBoxButtons.YesNo) !=
                DialogResult.Yes)
            {
                return;
            }
            cboFlowConns.Items.Clear();

            apiConns.FlowConns.Remove(removeFlow);
            if (apiConns.FlowConns.Any())
            {
                cboFlowConns.Items.AddRange(apiConns.FlowConns.ToArray());
                cboFlowConns.SelectedIndex = 0;
            }
            else
            {
                cboFlowConns.ResetText();
                txtEnvironment.Text = environmentText;
                txtTenant.Text = tenantText;
                txtAppId.Text = appText;
                txtReturnURL.Text = returnText;
                chkUseDevApp.Checked = false;
                txtName.Text = labelText;
                cboFlowConns.Enabled = false;
                panelFlow.Controls.OfType<TextBox>().ToList().ForEach(txt => txt.Enabled = false);
                chkUseDevApp.Enabled = false;
            }
        }

        private void btnRemoveLA_Click(object sender, EventArgs e)
        {
            if (cboLAConns.SelectedItem == null)
            {
                return;
            }

            LogicAppConn removeLA = (LogicAppConn)cboLAConns.SelectedItem;
            if (MessageBox.Show("Remove the Logic App Connection '" + removeLA.Name + "' ?",
                    "Remove LA Connection", MessageBoxButtons.YesNo) !=
                DialogResult.Yes)
            {
                return;
            }
            cboLAConns.Items.Clear();

            apiConns.LogicAppConns.Remove(removeLA);
            if (apiConns.LogicAppConns.Any())
            {
                cboLAConns.Items.AddRange(apiConns.LogicAppConns.ToArray());
                cboLAConns.SelectedIndex = 0;
            }
            else
            {
                cboLAConns.ResetText();
                txtSubscriptionId.Text = subscriptionText;
                txtLATenant.Text = tenantText;
                txtLAApp.Text = appText;
                txtLAReturnURL.Text = returnText;
                chkLADev.Checked = false;
                txtLAName.Text = labelText;
                cboLAConns.Enabled = false;
                panelLogicApp.Controls.OfType<TextBox>().ToList().ForEach(txt => txt.Enabled = false);

                chkLADev.Enabled = false;
            }
        }

        private void configValueEnter(object sender, EventArgs e)
        {
            TextBox inputTextBox = sender as TextBox;
            if (inputTextBox.Text == GetDefaultText(inputTextBox))
            {
                inputTextBox.Text = string.Empty;
                inputTextBox.ForeColor = Color.Black;
            }
        }

        private string GetDefaultText(TextBox inputBox)
        {
            switch (inputBox.Name)
            {
                case "txtLAName":
                case "txtName":
                    return labelText;

                case "txtSubscriptionId":
                    return subscriptionText;

                case "txtLAApp":
                case "txtAppId":
                    return appText;

                case "txtLAReturnURL":
                case "txtReturnURL":
                    return returnText;

                case "txtLATenant":
                case "txtTenant":
                    return tenantText;

                case "txtEnvironment":
                    return environmentText;

                default:
                    return string.Empty;
            }
        }

        private void configValueLeave(object sender, EventArgs e)
        {
            TextBox inputTextBox = sender as TextBox;

            if (inputTextBox.Text == string.Empty)
            {
                inputTextBox.Text = GetDefaultText(inputTextBox);
                inputTextBox.ForeColor = Color.Silver;
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string url = "https://linked365.blog/2020/10/14/flow-to-visio-xrmtoolbox-addon/#" + (LogicApp ? "LogicApps" : "PowerAutomateAPI");
            Process.Start(url);
        }
    }
}