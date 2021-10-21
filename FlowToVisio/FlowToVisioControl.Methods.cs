using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase
    {
        private List<FlowDefinition> flows;

        //private FlowConnection flowConnection;
        private LogicAppConn logicAppConn;
        private FlowConn flowConn;
        private APIConns aPIConnections;

        private void LoadFlows()
        {

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Flows",
                Work = (w, e) =>
                {
                    var qe = new QueryExpression("workflow");
                    qe.ColumnSet.AddColumns("ismanaged", "clientdata", "description", "name", "createdon", "modifiedon", "modifiedby", "createdby");
                    qe.Criteria.AddCondition("category", ConditionOperator.Equal, 5);
                    List<FlowDefinition> flowList = new List<FlowDefinition>();

                    var flowRecords = Service.RetrieveMultiple(qe);
                    foreach (var flowRecord in flowRecords.Entities)
                    {
                        flowList.Add(new FlowDefinition
                        {
                            Id = flowRecord["workflowid"].ToString(),
                            Name = flowRecord["name"].ToString(),
                            Definition = flowRecord["clientdata"].ToString(),
                            Description = !flowRecord.Attributes.Contains("description") ? string.Empty : flowRecord["description"].ToString(),
                            Solution = true,
                            Managed = (bool)flowRecord["ismanaged"]
                        });
                    }

                    e.Result = flowList;
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var returnFlows = e.Result as List<FlowDefinition>;
                    if (returnFlows.Any())
                    {
                        flows = returnFlows;
                        grdFlows.DataSource = flows;
                        SortGrid("Name", SortOrder.Ascending);

                    }

                    btnConnectCDS.Visible = !returnFlows.Any();
                    btnConnectLogicApps.Visible = returnFlows.Any();
                    btnConnectFlow.Visible = returnFlows.Any();
                    //flowRecords = (EntityCollection)e.Result;
                    //gridFlows.DataSource = flowRecords;
                },
            });
        }

        private void LoadUnSolutionedFlows()
        {
            Connect(false);
            if (_client == null)
            {
                return;
            }

            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            WorkAsync(
                new WorkAsyncInfo
                {
                    Message = "Loading Flows",
                    Work =
                        (w, args) => args.Result = GetAllFlows(w),
                    //        GetFlows(null, $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConnection.Environment}/flows?$top=20&api-version=2016-11-01",
                    //            w),
                    //_client.GetAsync(// $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConnection.Environment}/flows?$top=20&api-version=2016-11-01"),
                    PostWorkCallBack =
                        args =>
                        {
                            if (args.Error != null)
                            {
                                ShowError(args.Error, "Error retrieving Flows via API");
                                return;
                            }

                            if (args.Result is List<FlowDefinition>)
                            {
                                flows = args.Result as List<FlowDefinition>;

                                grdFlows.DataSource = flows;
                                SortGrid("Name", SortOrder.Ascending);
                            }

                            btnConnectCDS.Visible = flows.Any();
                            btnConnectFlow.Visible = !flows.Any();
                        }
                }
            );
        }

        private List<FlowDefinition> GetAllFlows(BackgroundWorker w)
        {
            var flows = new List<FlowDefinition>();
            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows?$top=50&api-version=2016-11-01";
            flows = GetFlows(flows, url, w);
            url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows?$filter=search(%27team%27)&$top50&api-version=2016-11-01";
            flows = GetFlows(flows, url, w);
            return flows;
        }
        private List<FlowDefinition> GetFlows(List<FlowDefinition> flows, string url, BackgroundWorker w)
        {
            HttpResponseMessage response = _client.GetAsync(url).GetAwaiter()
                   .GetResult();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var flowDefs = JObject.Parse(
                    response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (flowDefs["value"].HasValues)
                {
                    foreach (JToken flowDef in flowDefs["value"].Children())
                    {
                        flows.Add(
                            new FlowDefinition
                            {
                                Id = flowDef["name"].ToString(),
                                Solution = false,
                                Name = flowDef["properties"]["displayName"].ToString(),
                                OwnerType = flowDef["properties"]["userType"].ToString()
                            });
                    }
                }
                if (flowDefs.GetValue("nextLink") != null)
                {
                    flows = GetFlows(flows, flowDefs["nextLink"].ToString(), w);
                }
                return flows;
            }
            else
            {
                LogError("Get Flows via API", response);
                ShowError(
                    $"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.",
                    "Get Flows via API error");
                return null;
            }
        }
        private List<FlowDefinition> GetLogicApps(List<FlowDefinition> flows, string url, BackgroundWorker w)
        {
            if (flows == null)
            {
                flows = new List<FlowDefinition>();
            }

            HttpResponseMessage response = _client.GetAsync(url).GetAwaiter()
                   .GetResult();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var flowDefs = JObject.Parse(
                    response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (flowDefs["value"].HasValues)
                {
                    foreach (JToken flowDef in flowDefs["value"].Children())
                    {
                        flows.Add(
                            new FlowDefinition
                            {
                                Id = flowDef["id"].ToString(),
                                Solution = false,
                                Name = flowDef["name"].ToString(),
                                LogicApp = true
                                //OwnerType = flowDef["properties"]["userType"].ToString()
                            });
                    }
                }
                if (flowDefs.GetValue("nextLink") != null)
                {
                    flows = GetLogicApps(flows, flowDefs["nextLink"].ToString(), w);
                }
                return flows;
            }
            else
            {
                LogError("Get Logic Apps via API", response);
                ShowError(
                    $"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.",
                    "Get Flows via API error");
                return null;
            }
        }

        private JObject LoadFlow(FlowDefinition flowDefinition, string fileName, int flowCount)
        {

            // GetClient();
            string api = flowDefinition.LogicApp
                ? $"https://management.azure.com{flowDefinition.Id}?api-version=2016-06-01"
                : $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{flowDefinition.Id}?&api-version=2016-11-01";
            var result = _client.GetAsync(api).GetAwaiter().GetResult();            


                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //     flowObject = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    flowDefinition.Definition = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    GenerateVisio(fileName, flowDefinition,flowCount, flowDefinition.LogicApp);
                }
                else
                {
                    LogError("Get single Flow via API", result);
                    ShowError($"Status: {result.StatusCode}\r\n{result.ReasonPhrase}\r\nSee XrmToolBox log for details.", "Get Flow Error");
                }

            
                


            return null;
        }
        internal void ShowError(string error, string caption = null)
        {
            ShowError(new Exception(error), caption);
        }
        internal void ShowError(Exception error, string caption = null)
        {
            LogError(error.ToString());
            if (error.InnerException != null)
            {
                ShowError(error.InnerException);
            }
            else
            {
                MessageBox.Show(error.Message, caption ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLogicApps()
        {
            Connect(true);
            if (_client == null)
            {
                return;
            }

            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Logic Apps",
                Work = (w, args) =>
                    args.Result = GetLogicApps(null, $"https://management.azure.com/subscriptions/{logicAppConn.SubscriptionId}/providers/Microsoft.Logic/workflows?$top=20&api-version=2016-06-01", w),
                //_client.GetAsync($"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConnection.Environment}/flows?&api-version=2016-11-01").GetAwaiter().GetResult(),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error retrieving Logic Apps via API");
                        return;
                    }


                    if (args.Result is List<FlowDefinition>)
                    {
                        flows = args.Result as List<FlowDefinition>;

                        grdFlows.DataSource = flows;
                        SortGrid("Name", SortOrder.Ascending);
                    }

                    btnConnectCDS.Visible = flows.Any();
                    btnConnectFlow.Visible = flows.Any();
                    btnConnectLogicApps.Visible = !flows.Any();

                    //   var returnFlows = e.Result as List<FlowDefinition>;

                }
            }); ;

            //  var returnlist = _client.GetAsync($"https://management.azure.com/subscriptions/{flowConnection.SubscriptionId} /providers/Microsoft.Logic/workflows?api-version=2016-06-01");
        }

        private void Connect(bool logicApps)
        {
            ApiConnection apiConnection = new ApiConnection(aPIConnections, logicApps);
            try
            {
                _client = apiConnection.GetClient();
                if (logicApps)
                {
                    logicAppConn = apiConnection.laConn;
                }
                else
                {
                    flowConn = apiConnection.flowConn;
                }
            }
            catch (AdalServiceException adalExec)
            {
                LogError("Adal Error", adalExec.GetBaseException());

                if (adalExec.ErrorCode == "authentication_canceled")
                {
                    return;
                }

                ShowError(adalExec, "Error in connecting, please check details");
            }
            catch (Exception e)
            {
                LogError("Error getting connection", e.Message);
                ShowError(e, "Error in connecting, please check entered details");
                return;
            }
        }
    }

    public class FlowDefinition
    {
        public bool Solution;
        public string Id;
        public string Definition;
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Managed { get; set; }
        public string OwnerType { get; set; }
        public bool LogicApp { get; internal set; }
    }

    class FlowDefComparer : IComparer<FlowDefinition>
    {
        string memberName = string.Empty; // specifies the member name to be sorted
        SortOrder sortOrder = SortOrder.None; // Specifies the SortOrder.


        public FlowDefComparer(string strMemberName, SortOrder sortingOrder)
        {
            memberName = strMemberName;
            sortOrder = sortingOrder;
        }


        public int Compare(FlowDefinition flow1, FlowDefinition flow2)
        {
            int returnValue = 1;
            switch (memberName)
            {
                case "Name":
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Name.CompareTo(flow2.Name);
                    }
                    else
                    {
                        returnValue = flow2.Name.CompareTo(flow1.Name);
                    }

                    break;

                case "Description":
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Description.CompareTo(flow2.Description);
                    }
                    else
                    {
                        returnValue = flow2.Description.CompareTo(flow1.Description);
                    }

                    break;
                case "Managed":
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Managed.CompareTo(flow2.Managed);
                    }
                    else
                    {
                        returnValue = flow2.Managed.CompareTo(flow1.Managed);
                    }
                    break;
                default:
                    if (sortOrder == SortOrder.Ascending)
                    {
                        returnValue = flow1.Name.CompareTo(flow2.Name);
                    }
                    else
                    {
                        returnValue = flow2.Name.CompareTo(flow1.Name);
                    }
                    break;
            }
            return returnValue;
        }
    }
    public class FlowConnection
    {
        public string AppId;// = string.Empty;
        public string TenantId = string.Empty;
        public string ReturnURL = string.Empty;
        public string Environment = string.Empty;
        public bool UseDev;
        public string SubscriptionId = string.Empty;

        public string LATenantId = string.Empty;
        public string LAAppId = string.Empty;
        public string LAReturnURL = string.Empty;
        public bool LAUseDev;
    }

    public class APIConns
    {
        public List<LogicAppConn> LogicAppConns = new List<LogicAppConn>();
        public List<FlowConn> FlowConns = new List<FlowConn>();

        public Display Display = new Display();
    }

    public class Display
    {
        public bool ShowConCurrency { get; set; }
        public bool ShowSecure { get; set; }
        public bool ShowTrackedProps { get; set; }
        public bool ShowTriggers { get; set; }
        public bool ShowTrackingID { get; set; }
    }

    public class LogicAppConn
    {
        public int Id = 0;
        public string Name = string.Empty;
        public string SubscriptionId = string.Empty;
        public string TenantId = string.Empty;
        public string AppId = string.Empty;
        public string ReturnURL = string.Empty;
        public bool UseDev;

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                LogicAppConn p = (LogicAppConn)obj;
                return (Id == p.Id);
            }
        }
    }

    public class FlowConn
    {
        public int Id = 0;
        public string Name;
        public string AppId;// = string.Empty;
        public string TenantId = string.Empty;
        public string ReturnURL = string.Empty;
        public string Environment = string.Empty;
        public bool UseDev;

        public override string ToString()
        {
            return Name;
        }
    }


}