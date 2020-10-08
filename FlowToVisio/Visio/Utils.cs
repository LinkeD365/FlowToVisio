using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace LinkeD365.FlowToVisio
{

    public class Utils
    {
        public static int actionCount = 0;
        public static JObject Root { get; set; }

        public static XDocument XMLPage
        {
            get => _xmlPage;
            set
            {
                _xmlPage = value;
                connects = null;
            }
        }
        private static XDocument _xmlPage;

        private static XElement connects;

        public static XElement Connects
        {
            get
            {
                if (connects == null)
                {
                    IEnumerable<XElement> elements =
                      from element in XMLPage.Descendants()
                      where element.Name.LocalName == "Connects"
                      select element;
                    if (!elements.Any())
                    {
                        IEnumerable<XElement> pageContents =
                      from element in XMLPage.Descendants()
                      where element.Name.LocalName == "PageContents"
                      select element;
                        connects = new XElement("Connects");
                        pageContents.FirstOrDefault().Add(connects);
                    }
                    else connects = elements.FirstOrDefault();
                }
                return connects;
            }
        }

        private static AppInsights ai;

        public static AppInsights Ai
        {
            get
            {
                if (ai == null)
                {
                    ai = new AppInsights(aiEndpoint, aiKey, Assembly.GetExecutingAssembly());
                    ai.WriteEvent("Control Loaded");
                }
                return ai;
            }
        }

        private static JObject _actionTemplate;

        private static JObject ActionTemplate
        {
            get
            {
                if (_actionTemplate == null)
                {
#if DEBUG
                    _actionTemplate = JObject.Parse(File.ReadAllText(@"E:\Live\FlowToVisio\actions.json"));
#else
                    string jsonString =
                        new WebClient().DownloadString("https://raw.githubusercontent.com/LinkeD365/F/master/LinkeD65OrgSettings.xml");
                    _actionTemplate = JObject.Parse(jsonString);
#endif

                }

                return _actionTemplate;
            }
        }

        public static List<string> VisioTemplates
        {
            get
            {
                return (ActionTemplate["visioShapes"] as JArray).Select(jt => jt.ToString()).ToList();
            }
        }

        private static List<FlowRegion> _flowRegions;
        public static List<FlowRegion> FlowRegions
        {
            get
            {
                if (_flowRegions == null)
                {
                    _flowRegions = new List<FlowRegion>();
                    foreach (JProperty regionToken in ActionTemplate["regions"])
                    {
                        _flowRegions.Add(new FlowRegion
                        {
                           Name = regionToken.Name,
                           crmPart = regionToken.Value["crmpart"].ToString(),
                           flowPrefix = regionToken.Value["url"].ToString()

                            // Name = regionToken.
                        });
                    }
                }

                return _flowRegions;
            }
        }

        private static List<JProperty> OpenApiTemplates
        {
            get
            {
                return ActionTemplate["actions"].Children<JProperty>()
                    .Where(prop => prop.Value["type"] != null && prop.Value["type"].ToString() == "OpenApiConnection").ToList();
            }
        }

        private static List<JProperty> OtherTemplates
        {
            get
            {
                return ActionTemplate["actions"].Children<JProperty>()
                    .Where(prop => prop.Value["type"] != null && prop.Value["type"].ToString() != "OpenApiConnection").ToList();
            }
        }

        private const string aiEndpoint = "https://dc.services.visualstudio.com/v2/track";

        private const string aiKey = "cc383234-dfdb-429a-a970-d17847361df3";

        public static void AddActions(IEnumerable<JProperty> childActions, Action parent)
        {
            int childCount = childActions.Count();
            int curCount = 0;
            foreach (var actionProperty in childActions)
            {
                var childAction = AddAction(actionProperty, parent, ++curCount, childCount);
                AddActions(Root["properties"]["definition"]["actions"].Children<JProperty>().Where(a => a.Value["runAfter"].HasValues && ((JProperty)a.Value["runAfter"].First()).Name == childAction.Name), childAction);
            }
        }

        public static Action AddAction(JProperty actionProperty, Action parent, int curCount, int childCount)
        {
            if (actionProperty.Value["type"] == null)
            {
                return new Action(actionProperty, parent, curCount, childCount);
            }
            else
            {
                var templateAction = CreateTemplateAction(actionProperty, parent, curCount, childCount);
                if (templateAction != null) return templateAction;
                switch (actionProperty.Value["type"].ToString())
                {
                    case "InitializeVariable":
                        return new InitVariable(actionProperty, parent, curCount, childCount);

                    case "SetVariable":
                        return new SetVariable(actionProperty, parent, curCount, childCount);

                    case "Http":
                        return new HttpAction(actionProperty, parent, curCount, childCount);

                    case "Response":
                        return new HttpResponse(actionProperty, parent, curCount, childCount);

                    case "Request":
                        return CreateRequestAction(actionProperty, parent, curCount, childCount);

                    case "If":
                        return new IfAction(actionProperty, parent, curCount, childCount);

                    case "Switch":
                        return new SwitchAction(actionProperty, parent, curCount, childCount);

                    case "Foreach":
                        return new ForEachAction(actionProperty, parent, curCount, childCount);

                    case "Terminate":
                        return new Terminate(actionProperty, parent, curCount, childCount);

                    case "ApiConnection":
                    case "OpenApiConnection":
                        return CreateAPIAction(actionProperty, parent, curCount, childCount);

                    case "Scope":
                        return new ScopeAction(actionProperty, parent, curCount, childCount);

                    case "Compose":
                        return new ComposeAction(actionProperty, parent, curCount, childCount);

                    case "Table":
                        return new TableAction(actionProperty, parent, curCount, childCount);

                    case "Query":
                        return new FilterAction(actionProperty, parent, curCount, childCount);

                    case "Join":
                        return new JoinAction(actionProperty, parent, curCount, childCount);

                    case "ParseJson":
                        return new ParseAction(actionProperty, parent, curCount, childCount);

                    case "Select":
                        return new SelectAction(actionProperty, parent, curCount, childCount);

                    case "Until":
                        return new UntilAction(actionProperty, parent, curCount, childCount);

                    case "OpenApiConnectionWebhook":
                        return CreateWebhook(actionProperty, parent, curCount, childCount);

                    case "Recurrence":
                        return new RecurrenceAction(actionProperty, parent, curCount, childCount);

                    case "Changeset":
                        return new ChangeSetAction(actionProperty, parent, curCount, childCount);

                    default:
                        Ai.WriteEvent("No Action: " + actionProperty.Value["type"]);

                        return new Action(actionProperty, parent, curCount, childCount);
                }
            }
        }

        private static Action CreateWebhook(JProperty actionProperty, Action parent, int curCount, int childCount)
        {
            if (actionProperty.Value["inputs"]?["host"] != null)
            {
                switch (Connection.APIConnections.First(con => con.Name == actionProperty.Value["inputs"]["host"]["connectionName"].ToString()).Api)
                {
                    case "shared_commondataserviceforapps":
                        return new CDSTriggerAction(actionProperty, parent, curCount, childCount);
                }
                Ai.WriteEvent("No Webhook: " + Connection.APIConnections.First(con => con.Name == actionProperty.Value["inputs"]["host"]["connectionName"].ToString()).Api);
            }
            return new Action(actionProperty, parent, curCount, childCount);
        }

        private static Action CreateRequestAction(JProperty actionProperty, Action parent, int curCount, int childCount)
        {
            if (actionProperty.Value["kind"] != null)
            {
                switch (actionProperty.Value["kind"].ToString())
                {
                    case "Http":
                        return new HttpRequest(actionProperty, parent, curCount, childCount);

                    case "Button":
                        return new FlowButtonAction(actionProperty, parent, curCount, childCount);

                    case "PowerApp":
                        return new PAButtonAction(actionProperty, parent, curCount, childCount);

                    case "ApiConnection":
                        return CreateAPITrigger(actionProperty, parent, curCount, childCount);
                }
                Ai.WriteEvent("No Request Trigger: " + actionProperty.Value["kind"]);
            }
            return new Action(actionProperty, parent, curCount, childCount);
        }

        private static Action CreateAPITrigger(JProperty actionProperty, Action parent, int curCount, int childCount)
        {
            if (actionProperty.Value["inputs"]?["operationId"] != null)
            {
                switch (actionProperty.Value["inputs"]["operationId"].ToString())
                {
                    case "FlowStepRun":
                        return new CDSStepAction(actionProperty, parent, curCount, childCount);
                }
                Ai.WriteEvent("No API Trigger: " + actionProperty.Value["inputs"]?["operationId"]);
            }

            return new Action(actionProperty, parent, curCount, childCount);
        }

        private static Action CreateAPIAction(JProperty actionProperty, Action parent, int curCount, int childCount)
        {
            if (actionProperty.Value["type"].ToString() == "ApiConnection")
            {
                var connectionName = actionProperty.Value["inputs"]["host"]["connection"]["name"].ToString();

                int pFrom = connectionName.IndexOf("['") + 2;
                int pTo = connectionName.IndexOf("']");
                connectionName = connectionName.Substring(pFrom, pTo - pFrom);
                switch (Connection.APIConnections.First(con => con.Name == connectionName).Api)
                {
                    case "shared_commondataservice":
                        return new CDSAction(actionProperty, parent, curCount, childCount);

                    case "shared_teams":
                        return new TeamsAction(actionProperty, parent, curCount, childCount);

                    case "shared_excelonlinebusiness":
                        return new ExcelAction(actionProperty, parent, curCount, childCount);

                    case "shared_office365":
                        return new OfficeAction(actionProperty, parent, curCount, childCount);

                    case "shared_flowmanagement":
                        return new FlowAction(actionProperty, parent, curCount, childCount);
                    case "shared_sharepointonline":
                        return new SharePointAction(actionProperty, parent, curCount, childCount);
                }
                Ai.WriteEvent("No API Action: " + Connection.APIConnections.First(con => con.Name == connectionName).Api);
            }
            else if (actionProperty.Value["type"].ToString() == "OpenApiConnection")
            {
                var connectName = actionProperty.Value["inputs"]["host"]["connectionName"].ToString();
                switch (Connection.APIConnections.First(con => con.Name == connectName).Api)
                {
                    case "shared_excelonlinebusiness":
                        return new ExcelAction(actionProperty, parent, curCount, childCount);

                    case "shared_office365":
                        return new OfficeAction(actionProperty, parent, curCount, childCount);

                    case "shared_flowmanagement":
                        return new FlowAction(actionProperty, parent, curCount, childCount);

                    case "shared_commondataserviceforapps":
                    case "shared_commondataservice":
                        return new CDSAction(actionProperty, parent, curCount, childCount);
                    case "shared_sharepointonline":
                        return new SharePointAction(actionProperty, parent, curCount, childCount);
                }
                Ai.WriteEvent("No Open API Action: " + Connection.APIConnections.First(con => con.Name == connectName).Api);
            }

            return new Action(actionProperty, parent, curCount, childCount);
        }

        private static Action CreateTemplateAction(JProperty actionProperty, Action parent, int curCount, int childCount)
        {
            //   var templates = ActionTemplate.
            if (actionProperty.Value["type"].ToString() == "OpenApiConnection")
            {
                var template = OpenApiTemplates.FirstOrDefault(prop =>
                    prop.Value["connectionName"].ToString() == actionProperty.Value["inputs"]["host"]["connectionName"].ToString() &&
                    prop.Value["operationId"].ToString() == actionProperty.Value["inputs"]["host"]["operationId"].ToString());
                if (template == null) return null;

                return new TemplateAction(template, actionProperty, parent, curCount, childCount, template.Value["visioShape"].ToString());
            }
            
            if(actionProperty.Value["kind"] != null)
            {
                var template = OtherTemplates.FirstOrDefault(prop =>
                    prop.Value["type"].ToString() == actionProperty.Value["type"].ToString() && prop.Value["kind"].ToString() == actionProperty.Value["kind"].ToString() );
                if (template == null) return null;

                return new TemplateAction(template, actionProperty, parent, curCount, childCount, template.Value["visioShape"].ToString());
            }

            if (actionProperty.Value["kind"] == null)
            {
                var template = OtherTemplates.FirstOrDefault(prop =>
                    prop.Value["type"].ToString() == actionProperty.Value["type"].ToString() && prop.Value["kind"] == null);
                if (template == null) return null;

                return new TemplateAction(template, actionProperty, parent, curCount, childCount, template.Value["visioShape"].ToString());
            }



            return null;
        }
    }

    public class FlowRegion
    {
        public string Name { get; set; }
        public string flowPrefix;
        public string crmPart;
    }
}
