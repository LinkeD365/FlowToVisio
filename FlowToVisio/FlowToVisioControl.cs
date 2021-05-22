using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase, IGitHubPlugin, INoConnectionRequired, IPayPalPlugin
    {
        private bool overrideSave;

        public string RepositoryName => "FlowToVisio";
        public string UserName => "LinkeD365";

        public string DonationDescription => "Flow to Visio Fans";

        public string EmailAccount => "carl.cookson@gmail.com";

        public FlowToVisioControl()
        {
            InitializeComponent();
        }

        private void FlowToVisioControl_Load(object sender, EventArgs e)
        {
            try
            {
                if (SettingsManager.Instance.TryLoad(GetType(), out FlowConnection flowConnection))
                {
                    LogWarning("Old settings file found, converting");
                    aPIConnections = new APIConns();
                    if (!string.IsNullOrEmpty(flowConnection.TenantId))
                    {
                        aPIConnections.FlowConns
                            .Add(
                                new FlowConn
                                {
                                    AppId = flowConnection.AppId,
                                    TenantId = flowConnection.TenantId,
                                    Environment = flowConnection.Environment,
                                    ReturnURL = flowConnection.ReturnURL,
                                    UseDev = flowConnection.UseDev,
                                    Name = "Flow Connection"
                                });
                    }
                    if (!string.IsNullOrEmpty(flowConnection.SubscriptionId))
                    {
                        aPIConnections.LogicAppConns
                           .Add(
                               new LogicAppConn
                               {
                                   AppId = flowConnection.LAAppId,
                                   TenantId = flowConnection.LATenantId,
                                   ReturnURL = flowConnection.LAReturnURL,
                                   SubscriptionId = flowConnection.SubscriptionId,
                                   UseDev = flowConnection.UseDev,
                                   Name = "LA Connection"
                               });
                    }

                    return;

                }

            }
            catch (Exception)
            {

            }
            if (!SettingsManager.Instance.TryLoad(GetType(), out aPIConnections))
            {
                aPIConnections = new APIConns();

                LogWarning("Settings not found => a new settings file has been created!");
            }

            // Loads or creates the settings for the plugin
            //if (!SettingsManager.Instance.TryLoad(GetType(), out aPIConnections))
            //{
            //    FlowConnection flowConnection;
            //    if (!SettingsManager.Instance.TryLoad(GetType(), out flowConnection))
            //    {
            //        aPIConnections = new APIConns();

            //        LogWarning("Settings not found => a new settings file has been created!");
            //    }
            //    else
            //    {


            //    }
            //}
            //else
            //{
            //    LogInfo("Settings found and loaded");
            //}
            // ExecuteMethod(LoadFlows);
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
            SettingsManager.Instance.Save(GetType(), aPIConnections);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (flowConn != null && detail != null)
            {
                //  mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            LoadFlows();
        }

        private void btnCreateVisio_Click(object sender, EventArgs e)
        {
            var selectFlow = ((FlowDefinition)grdFlows.SelectedRows[0].DataBoundItem);


            saveDialog.FileName = selectFlow.Name + ".vsdx";
            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (selectFlow.Solution)
            {
                flowObject = JObject.Parse(selectFlow.Definition);
                GenerateVisio(saveDialog.FileName);
            }
            else
            {
                flowObject = LoadFlow(selectFlow, saveDialog.FileName);
            }
        }

        public List<dynamic> Sort<T>(List<dynamic> input, string property)
        {
            var type = typeof(T);
            var sortProperty = type.GetProperty(property);
            return input.OrderBy(p => sortProperty.GetValue(p, null)).ToList();
        }
        private SortOrder getSortOrder(int columnIndex)
        {
            if (grdFlows.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None ||
                grdFlows.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending)
            {
                grdFlows.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                return SortOrder.Ascending;
            }
            else
            {
                grdFlows.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                return SortOrder.Descending;
            }
        }
        private void grdFlows_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SortOrder sortOrder = getSortOrder(e.ColumnIndex);

            SortGrid(grdFlows.Columns[e.ColumnIndex].Name, sortOrder);
            // string strColumnName = grdFlows.Columns[e.ColumnIndex].Name;



        }

        private void SortGrid(string name, SortOrder sortOrder)
        {
            List<FlowDefinition> sortingFlows = (List<FlowDefinition>)grdFlows.DataSource;
            sortingFlows.Sort(new FlowDefComparer(name, sortOrder));
            grdFlows.DataSource = null;
            grdFlows.DataSource = sortingFlows;
            InitGrid();
            grdFlows.Columns[name].HeaderCell.SortGlyphDirection = sortOrder;
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            //gridFlows.DataSource = null;
            if (!string.IsNullOrEmpty(textSearch.Text))
            {
                grdFlows.DataSource = flows.Where(flw => flw.Name.ToLower().Contains(textSearch.Text.ToLower())).ToList();//.Entities.Where(ent => ent.Attributes["name"].ToString().ToLower().Contains(textSearch.Text));
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
        }


        private HttpClient _client;

        private void btnConnectCDS_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadFlows);
        }

        private void InitGrid()
        {
            grdFlows.AutoResizeColumns();
            grdFlows.Columns["Name"].SortMode = DataGridViewColumnSortMode.Automatic;

            grdFlows.Columns["Managed"].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        private void btnConnectLogicApps_Click(object sender, EventArgs e)
        {
            LoadLogicApps();
        }


    }
}