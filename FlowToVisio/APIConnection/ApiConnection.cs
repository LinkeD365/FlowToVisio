using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
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

        public ApiConnection(APIConns apiConnections, bool logicApp)
        {
            InitializeComponent();
            apiConns = apiConnections;

            //if (_flowConnection == null)
            //{
            //    return;
            //}
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
                chkLADev.CheckedChanged += ChkLADev_CheckedChanged;

            }

            panelFlow.Visible = !LogicApp;
            panelLogicApp.Visible = LogicApp;


        }

        private void EnableControls()
        {
            if (LogicApp)
            {
                txtTenant.Enabled = false;
                txtAppId.Enabled = false;
                txtEnvironment.Enabled = false;
                txtName.Enabled = false;
                txtReturnURL.Enabled = false;
            }

            else
            {
                txtLATenant.Enabled = false;
                txtLAApp.Enabled = false;
                txtSubscriptionId.Enabled = false;
                txtLAReturnURL.Enabled = false;
                txtLAName.Enabled = false;
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
            AuthenticationContext ac = new AuthenticationContext($"https://login.microsoftonline.com/{ (LogicApp ? laConn.TenantId : flowConn.TenantId)}");
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
            if (!LogicApp && (txtAppId.Text == string.Empty
                || txtTenant.Text == string.Empty || txtEnvironment.Text == string.Empty
                || txtReturnURL.Text == string.Empty || txtName.Text == string.Empty))
            {
                MessageBox.Show("Please ensure all fields have a value", "Required properties missing", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None;
                return;
            }
            if (LogicApp && (txtSubscriptionId.Text == string.Empty
                || txtLAApp.Text == string.Empty || txtLAName.Text == string.Empty
                || txtLAReturnURL.Text == string.Empty || txtLATenant.Text == string.Empty))
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

            txtLAApp.Enabled = !chkLADev.Checked;
            txtLAReturnURL.Enabled = !chkLADev.Checked;
            chkLADev.CheckedChanged += ChkLADev_CheckedChanged;

            //panelFlow.Visible = !LogicApp;
            //panelLogicApp.Visible = LogicApp;
            //chkLADev.Visible = LogicApp;
            //chkUseDevApp.Visible = !LogicApp;
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
            txtAppId.Enabled = !chkUseDevApp.Checked;
            txtReturnURL.Enabled = !chkUseDevApp.Checked;
        }

        private void btnAddLA_Click(object sender, EventArgs e)
        {
            lblLAName.Tag = apiConns.LogicAppConns.Any() ? apiConns.LogicAppConns.Max(la => la.Id) + 1 : 0;
            txtSubscriptionId.Text = string.Empty;
            txtLATenant.Text = string.Empty;
            txtLAApp.Text = string.Empty;
            txtLAReturnURL.Text = string.Empty;
            chkLADev.Checked = false;
            txtLAName.Text = string.Empty;
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
            txtEnvironment.Text = string.Empty;
            txtTenant.Text = string.Empty;
            txtAppId.Text = string.Empty;
            txtReturnURL.Text = string.Empty;
            chkUseDevApp.Checked = false;
            txtName.Text = string.Empty;
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
                txtEnvironment.Text = string.Empty;
                txtTenant.Text = string.Empty;
                txtAppId.Text = string.Empty;
                txtReturnURL.Text = string.Empty;
                chkUseDevApp.Checked = false;
                txtName.Text = string.Empty;
                txtAppId.Enabled = false;
                txtReturnURL.Enabled = false;
                txtName.Enabled = false;
                txtTenant.Enabled = false;
                txtName.Enabled = false;
                txtEnvironment.Enabled = false;
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
                txtSubscriptionId.Text = string.Empty;
                txtLATenant.Text = string.Empty;
                txtLAApp.Text = string.Empty;
                txtLAReturnURL.Text = string.Empty;
                chkLADev.Checked = false;
                txtLAName.Text = string.Empty;
                txtLAApp.Enabled = false;
                txtLAReturnURL.Enabled = false;
                txtLAName.Enabled = false;
                txtLATenant.Enabled = false;
                txtLAName.Enabled = false;
                txtSubscriptionId.Enabled = false;
                chkLADev.Enabled = false;
            }
        }
    }
}
