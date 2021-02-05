using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace LinkeD365.FlowToVisio
{
    public partial class ApiConnection : Form
    {
        private FlowConnection _flowConnection;

        private bool LogicApp;
        // Azure Active Directory registered app clientid for Microsoft samples
        private const string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        // Azure Active Directory registered app Redirect URI for Microsoft samples
        private Uri redirectUri = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");

        public ApiConnection(FlowConnection flowConnecton, bool logicApp)
        {
            InitializeComponent();
            _flowConnection = flowConnecton;
            if (_flowConnection == null) return;
            txtTenant.Text = _flowConnection.TenantId;
            txtEnvironment.Text = _flowConnection.Environment;
            txtAppId.Text = _flowConnection.AppId;
            txtReturnURL.Text = _flowConnection.ReturnURL;
            chkUseDevApp.Checked = _flowConnection.UseDev;
            txtSubscriptionId.Text = _flowConnection.SubscriptionId;
            txtLATenant.Text = _flowConnection.LATenantId;
            txtLAApp.Text = _flowConnection.LAAppId;
            txtLAReturnURL.Text = _flowConnection.LAReturnURL;
            chkLADev.Checked = _flowConnection.LAUseDev;
            LogicApp = logicApp;
            panelFlow.Visible = !LogicApp;
            panelLogicApp.Visible = LogicApp;
            chkLADev.Visible = LogicApp;
            chkUseDevApp.Visible = !LogicApp;

            txtAppId.Enabled = !chkUseDevApp.Checked;
            txtReturnURL.Enabled = !chkUseDevApp.Checked;
            txtLAApp.Enabled = !chkLADev.Checked;
            txtLAReturnURL.Enabled = !chkLADev.Checked;

            chkUseDevApp.CheckedChanged += ChkUseDevApp_CheckedChanged;
            chkLADev.CheckedChanged += ChkLADev_CheckedChanged;

        }

        public HttpClient GetClient()
        {
            if (ShowDialog() == DialogResult.OK)
            {
                if (LogicApp)
                {
                    _flowConnection.SubscriptionId = txtSubscriptionId.Text;
                    _flowConnection.LATenantId = txtLATenant.Text;
                    _flowConnection.LAReturnURL = txtLAReturnURL.Text;
                    _flowConnection.LAAppId = txtLAApp.Text;
                    _flowConnection.LAUseDev = chkLADev.Checked;
                }
                else
                {
                    _flowConnection.TenantId = txtTenant.Text;
                    _flowConnection.AppId = txtAppId.Text;
                    _flowConnection.ReturnURL = txtReturnURL.Text;
                    _flowConnection.Environment = txtEnvironment.Text;
                    _flowConnection.UseDev = chkUseDevApp.Checked;
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
            AuthenticationContext ac = new AuthenticationContext($"https://login.microsoftonline.com/{ (LogicApp ? _flowConnection.LATenantId : _flowConnection.TenantId)}");
            string serviceURL = LogicApp ? "https://management.azure.com/" : "https://service.flow.microsoft.com/";
            string appId = LogicApp ? _flowConnection.LAAppId : _flowConnection.AppId;

            try
            {
                return ac.AcquireTokenSilentAsync(serviceURL, appId).GetAwaiter().GetResult().AccessToken;
            }
            catch (AdalException adalException)
            {
                if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently
                    || adalException.ErrorCode == AdalError.InteractionRequired)

                    return ac.AcquireTokenAsync(serviceURL, appId, new Uri(LogicApp ? _flowConnection.LAReturnURL : _flowConnection.ReturnURL),
                        new PlatformParameters(PromptBehavior.SelectAccount)).GetAwaiter().GetResult().AccessToken;
            }

            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (LogicApp && (txtAppId.Text == string.Empty || txtTenant.Text == string.Empty || txtEnvironment.Text == string.Empty || txtReturnURL.Text == string.Empty))
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
    }
}
