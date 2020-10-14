using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private FlowConnection flowConnection;

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
                        InitGrid();
                    }

                    btnConnectCDS.Visible = !returnFlows.Any();
                    btnConnectFlow.Visible = returnFlows.Any();
                    //flowRecords = (EntityCollection)e.Result;
                    //gridFlows.DataSource = flowRecords;
                },
            });
        }

        private void LoadUnSolutionedFlows()
        {
            ApiConnection apiConnection = new ApiConnection(flowConnection);
            try
            {
                _client = apiConnection.GetClient();
            }
            catch (Exception e)
            {
                LogError("Error getting connection", e.Message);
                ShowError(e, "Error in connecting, please check entered details");
                return;
            } 
            if (_client == null) return;
            //GetClient();
            SettingsManager.Instance.Save(typeof(FlowConnection), flowConnection);

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Flows",
                Work = (w, args) =>
                {

                    args.Result = _client.GetAsync($"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConnection.Environment}/flows?&api-version=2016-11-01").GetAwaiter().GetResult();
                    //    var response = _client.GetAsync("https://unitedkingdom.api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/32180c50-6a4d-42cd-bcf1-75f1cfc5bc77/flows?&api-version=2016-11-01").GetAwaiter().GetResult();



                    //   e.Result = flowList;
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error retrieving Flows via API");
                        return;
                    }
                    List<FlowDefinition> flowList = new List<FlowDefinition>();
                    if (args.Result is HttpResponseMessage)
                    {
                        var response = args.Result as HttpResponseMessage;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {

                            var flowDefs = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                            if (flowDefs["value"].HasValues)
                            {
                                foreach (JToken flowDef in flowDefs["value"].Children())
                                    flowList.Add(new FlowDefinition
                                    {
                                        Id = flowDef["name"].ToString(),
                                        Solution = false,
                                        Name = flowDef["properties"]["displayName"].ToString(),
                                        OwnerType = flowDef["properties"]["userType"].ToString()
                                    });
                            }
                        }
                        else
                        {
                            LogError("Get Flows via API", response);
                            ShowError($"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.", "Get Flows via API error");
                        }
                    }

                    //   var returnFlows = e.Result as List<FlowDefinition>;
                    if (flowList.Any())
                    {
                        flows = flowList;
                        grdFlows.DataSource = flows;
                        InitGrid();
                    }

                    btnConnectCDS.Visible = flowList.Any();
                    btnConnectFlow.Visible = !flowList.Any();


                }
            });
        }

        private JObject LoadFlow(FlowDefinition flowDefinition)
        {
            // GetClient();


            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Flow",
                Work = (w, args) =>
                {
                   args.Result = _client.GetAsync($"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{flowConnection.Environment}/flows/{flowDefinition.Id}?&api-version=2016-11-01").GetAwaiter().GetResult();

                    //jsonResponse = response2.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error retrieving Flow via API");
                        return;
                    }

                    if (args.Result is HttpResponseMessage)
                    {
                        var response = args.Result as HttpResponseMessage;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            flowObject = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                            GenerateVisio();
                        }
                        else
                        {
                            LogError("Get single Flow via API", response);
                            ShowError($"Status: {response.StatusCode}\r\n{response.ReasonPhrase}\r\nSee XrmToolBox log for details.", "Get Flow Error");
                        }

                    }


                },
            });

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

    }

    public class FlowConnection
    {
        public string AppId;// = string.Empty;
        public string TenantId = string.Empty;
        public string ReturnURL = string.Empty;
        public string Environment = string.Empty;
    }
}