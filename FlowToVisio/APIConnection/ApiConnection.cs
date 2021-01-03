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


        // Azure Active Directory registered app clientid for Microsoft samples
        private const string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
        // Azure Active Directory registered app Redirect URI for Microsoft samples
        private Uri redirectUri = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");

        public ApiConnection(FlowConnection flowConnecton)
        {
            InitializeComponent();
            _flowConnection = flowConnecton;
            if (_flowConnection == null) return;
            txtTenant.Text = _flowConnection.TenantId;
            txtEnvironment.Text = _flowConnection.Environment;
            txtAppId.Text = _flowConnection.AppId;
            txtReturnURL.Text = _flowConnection.ReturnURL;
            chkUseDevApp.Checked = _flowConnection.UseDev;
        }

        public HttpClient GetClient()
        {
            if (ShowDialog() == DialogResult.OK)
            {
                _flowConnection.TenantId = txtTenant.Text;
                _flowConnection.AppId = txtAppId.Text;
                _flowConnection.ReturnURL = txtReturnURL.Text;
                _flowConnection.Environment = txtEnvironment.Text;
                _flowConnection.UseDev = chkUseDevApp.Checked;
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
            AuthenticationContext ac = new AuthenticationContext($"https://login.microsoftonline.com/{_flowConnection.TenantId}");
            try
            {
                return ac.AcquireTokenSilentAsync("https://service.flow.microsoft.com/", _flowConnection.AppId).GetAwaiter().GetResult().AccessToken;
            }
            catch (AdalException adalException)
            {
                if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently
                    || adalException.ErrorCode == AdalError.InteractionRequired)

                    return ac.AcquireTokenAsync("https://service.flow.microsoft.com/", _flowConnection.AppId, new Uri(_flowConnection.ReturnURL),
                        new PlatformParameters(PromptBehavior.SelectAccount)).GetAwaiter().GetResult().AccessToken;
            }

            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtAppId.Text == string.Empty || txtTenant.Text == string.Empty || txtEnvironment.Text == string.Empty || txtReturnURL.Text == string.Empty)
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
    }
}
