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

        public ApiConnection( FlowConnection flowConnecton)
        {
            InitializeComponent();
            _flowConnection = flowConnecton;
            if (_flowConnection == null) return;
            txtTenant.Text = _flowConnection.TenantId;
            txtEnvironment.Text = _flowConnection.Environment;
            txtAppId.Text = _flowConnection.AppId;
            txtReturnURL.Text = _flowConnection.ReturnURL;

        }

        public HttpClient GetClient()
        {
            if (ShowDialog() == DialogResult.OK)
            {
                _flowConnection.TenantId = txtTenant.Text;
                _flowConnection.AppId = txtAppId.Text;
                _flowConnection.ReturnURL = txtReturnURL.Text;
                _flowConnection.Environment = txtEnvironment.Text;
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
                {

                    return ac.AcquireTokenAsync("https://service.flow.microsoft.com/", _flowConnection.AppId, new Uri(_flowConnection.ReturnURL),
                        new PlatformParameters(PromptBehavior.SelectAccount)).GetAwaiter().GetResult().AccessToken;
                }
            }

            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtAppId.Text == string.Empty || txtTenant.Text == string.Empty || txtEnvironment.Text == string.Empty || txtReturnURL.Text == string.Empty )
            {
                MessageBox.Show("Please ensure all fields have a value", "Required properties missing", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None;
            }
        }

        private void txtReturnURL_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
