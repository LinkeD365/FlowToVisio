using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;

using XrmToolBox.Extensibility.Interfaces;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase, IGitHubPlugin
    {
        private Settings mySettings;
        private bool overrideSave = false;

        public string RepositoryName => "ERD Visio Builder";
        public string UserName => "LinkeD365";

        public FlowToVisioControl()
        {
            InitializeComponent();
        }

        private void FlowToVisioControl_Load(object sender, EventArgs e)
        {

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
            ExecuteMethod(LoadFlows);
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowToVisioControl_OnClose(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            LoadFlows();
        }

        private void btnCreateVisio_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Please select a file name prior to generating a Visio", "Select File", MessageBoxButtons.OK);
                return;
            }

            if (File.Exists(txtFileName.Text) && !overrideSave)
            {
                if (MessageBox.Show("Do you want to override the file?", "File already exists", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }
            overrideSave = false;
            var selectFlow = ((FlowDefinition) grdFlows.SelectedRows[0].DataBoundItem);
            if (selectFlow.Solution)
            {
                flowObject = JObject.Parse(selectFlow.Definition);
                GenerateVisio();
            }
            else flowObject = LoadFlow(selectFlow);
           // if (((FlowDefinition) grdFlows.SelectedRows[0].DataBoundItem).Solution) flowObject = JObject.Parse(gridFlows.SelectedRowRecords.First()["clientdata"].ToString());
            // CreateVisio();
           // GenerateVisio();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            if (saveDialog.ShowDialog() == DialogResult.OK) txtFileName.Text = saveDialog.FileName;
            overrideSave = true;
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            //gridFlows.DataSource = null;
            if (string.IsNullOrEmpty(textSearch.Text))
            {
                grdFlows.DataSource = flows.Where(flw => flw.Name.ToLower().Contains(textSearch.Text.ToLower()));//.Entities.Where(ent => ent.Attributes["name"].ToString().ToLower().Contains(textSearch.Text));
            }
            else
            {
                grdFlows.DataSource = flows;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //GetClient();
            LoadUnSolutionedFlows();

            //var response = _client.GetAsync("https://unitedkingdom.api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/15a1fec1-1ccc-4940-9d6a-fd622722f998/flows?&api-version=2016-11-01").GetAwaiter().GetResult();
            //var jsonResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            //var unSolutonJson = JObject.Parse(jsonResponse);

            //var response2 = _client.GetAsync("https://unitedkingdom.api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/15a1fec1-1ccc-4940-9d6a-fd622722f998/flows/2d4ae0c3-6901-4ed1-9a77-256cacffb9f5?&api-version=2016-11-01").GetAwaiter().GetResult();
            //jsonResponse = response2.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        private static string GetInteractiveClientToken(PACClientInfo clientInfo, PromptBehavior behavior)
        {
            // Dummy endpoint just to get unauthorized response
            var client = new HttpClient();
            var query = $"{clientInfo.ServiceUrl}/api/status/4799049A-E623-4B2A-818A-3A674E106DE5";
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(query));

            using (var response = client.SendAsync(request).GetAwaiter().GetResult())
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Method below found here: https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/wiki/Acquiring-tokens-interactively---Public-client-application-flows
                    var authParams = AuthenticationParameters.CreateFromUnauthorizedResponseAsync(response).GetAwaiter().GetResult();
                    var authContext = new AuthenticationContext(authParams.Authority);
                    var authResult = authContext.AcquireTokenAsync(
                        "https://service.flow.microsoft.com",
                        clientInfo.ClientId.ToString(),
                        new Uri("httpsL//localhost"),
                        new PlatformParameters(behavior)).GetAwaiter().GetResult();
                    return authResult.AccessToken;
                }
                else
                {
                    throw new Exception($"Unable to connect to the service for authorization information. {response.ReasonPhrase}");
                }
            }
        }

        private HttpClient _client;
        private string _clientId = "ccabbfe0-fa70-4724-a1cd-a9b598363c92";
        private string _tenantID = "4e95d9b9-8b59-4fb8-8eed-d7904bb2f2e0";
        private string _returnUri = "http://localhost";
        

        private void btnConnectCDS_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadFlows);
        }
    }
    public class PACClientInfo
    {
        public string ServiceUrl = "https://unitedkingdom.api.flow.microsoft.com/";

        public Guid ClientId;
        public Guid TenantId;
        public string ClientSec;
        public string Token;
        public string Language;
    }
}