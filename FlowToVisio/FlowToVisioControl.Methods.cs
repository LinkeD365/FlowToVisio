using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        private List<FlowRun> flowRuns;

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
                    qe.ColumnSet.AddColumns("ismanaged", "clientdata", "description", "name", "createdon", "modifiedon", "modifiedby", "createdby", "workflowidunique");
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
                            Managed = (bool)flowRecord["ismanaged"],
                            UniqueId = flowRecord["workflowidunique"].ToString()
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

        private void LoadSolutions()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Solutions",
                Work = (w, e) =>
                {
                    QueryExpression solQry = new QueryExpression("solution");
                    solQry.Distinct = true;
                    solQry.ColumnSet = new ColumnSet("friendlyname", "version", "publisherid", "solutionid");
                    solQry.AddOrder("friendlyname", OrderType.Ascending);
                    solQry.Criteria = new FilterExpression();
                    solQry.Criteria.AddCondition(new ConditionExpression("isvisible", ConditionOperator.Equal, true));
                    solQry.Criteria.AddCondition(new ConditionExpression("uniquename", ConditionOperator.NotEqual, "Default"));
                    List<Solution> solList = new List<Solution>();

                    var solutionRows = Service.RetrieveMultiple(solQry);
                    foreach (var solution in solutionRows.Entities)
                    {
                        solList.Add(new Solution
                        {
                            Id = solution["solutionid"].ToString(),
                            Name = solution["friendlyname"].ToString(),
                            Publisher = ((EntityReference)solution["publisherid"]).Name
                        });
                    }

                    e.Result = solList;
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var solList = e.Result as List<Solution>;
                    splitTop.Panel2Collapsed = !solList.Any();

                    solList.Insert(0, new Solution() { Name = "Filter on Solution" });
                    ddlSolutions.DataSource = solList;
                    ddlSolutions.DisplayMember = "Name";
                },
            });
        }

        private void GetFlowsForSolution()
        {
            if (ddlSolutions.SelectedIndex == 0) { LoadFlows(); return; }
            string solId = ((Solution)ddlSolutions.SelectedItem).Id;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieiving the Flows for Solution",
                Work = (w, e) =>
                {
                    var qe = new QueryExpression("workflow");
                    qe.ColumnSet.AddColumns("ismanaged", "clientdata", "description", "name", "createdon", "modifiedon", "modifiedby", "createdby");
                    qe.AddOrder("name", OrderType.Ascending);
                    qe.Criteria.AddCondition("category", ConditionOperator.Equal, 5);
                    var solComp = qe.AddLink("solutioncomponent", "workflowid", "objectid", JoinOperator.Inner);
                    solComp.EntityAlias = "solComp";
                    var sol = solComp.AddLink("solution", "solutionid", "solutionid");
                    sol.EntityAlias = "sol";
                    sol.LinkCriteria.AddCondition("solutionid", ConditionOperator.Equal, solId);

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
                    flows = returnFlows;
                    grdFlows.DataSource = flows;
                    if (returnFlows.Any())
                    {
                        SortGrid("Name", SortOrder.Ascending);
                    }

                    btnConnectCDS.Visible = !returnFlows.Any();
                    btnConnectLogicApps.Visible = returnFlows.Any();
                    btnConnectFlow.Visible = returnFlows.Any();
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
                            splitTop.Panel2Collapsed = true;
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
                                UniqueId = flowDef["name"].ToString(),
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
                // flowObject
                // = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                flowDefinition.Definition = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                GenerateVisio(fileName, flowDefinition, flowCount, flowDefinition.LogicApp);
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

        private void ShowError(Exception error, string caption = null)
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
                    splitTop.Panel2Collapsed = true;
                }
            });
        }

        private void Connect(bool logicApps)
        {
            if (_client != null) return;
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

        private void GetAllFlowRuns(FlowDefinition flow)
        {
            Connect(false);
            if (_client == null) return;
            SettingsManager.Instance.Save(typeof(APIConns), aPIConnections);

            string url = $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConn.Environment}/flows/{flow.UniqueId}/runs?&api-version=2016-11-01";
            flowRuns = new List<FlowRun>();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Runs",
                Work = (w, args) => args.Result = GetFlowRuns(flowRuns, url, w),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error retrieving Flow Runs via API");
                        return;
                    }

                    if (args.Result is List<FlowRun>)
                    {
                        FlowRunForm flowRunForm = new FlowRunForm(args.Result as List<FlowRun>, flow, flowConn, _client, this);
                        flowRunForm.ShowDialog();

                        if (flowRunForm.DialogResult == DialogResult.Yes) GetAllFlowRuns(flow);
                    }
                }
            });
            // GetFlowRuns(url);
        }

        private List<FlowRun> GetFlowRuns(List<FlowRun> flows, string url, BackgroundWorker w)
        {
            HttpResponseMessage response = _client.GetAsync(url).GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var flowRunsJO = JObject.Parse(
                                       response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (flowRunsJO["value"].HasValues)
                {
                    foreach (JToken flowRunJO in flowRunsJO["value"].Children())
                    {
                        flows.Add(
                            new FlowRun
                            {
                                Id = flowRunJO["name"].ToString(),// 2023-03-06T16:43:39.6247123Z
                                Start = (DateTime)flowRunJO["properties"]["startTime"],//  DateTime.ParseExact(flowRunJO["properties"]["startTime"].ToString(), "yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                End = (DateTime?)flowRunJO["properties"]["endTime"],
                                Status = flowRunJO["properties"]["status"].ToString()
                            }); ; ;
                    }
                }

                if (flowRunsJO.GetValue("nextLink") != null)
                {
                    GetFlowRuns(flows, flowRunsJO["nextLink"].ToString(), w);
                }
                return flowRuns;
            }
            else
            {
                LogError("Get Flow Runs via API", response);
                ShowError(
                    $"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.",
                    "Get Flow Runs via API error");
                return null;
            }
        }
    }
}