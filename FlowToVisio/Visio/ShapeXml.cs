using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LinkeD365.FlowToVisio
{
    public class Line : BaseShape
    {
        public Line() : base()
        {
            Shape = new XElement(Line);
            Shape.SetAttributeValue("NameU", "Line." + Id);
        }

        public Action ParentAction { get; protected set; }
        public Action ChildAction { get; protected set; }

        private XElement connectStart;
        private XElement connectEnd;
        private int connectorNo;

        public XElement ConnectStart
        {
            get
            {
                if (connectStart == null)
                    connectStart = new XElement("Connect",
                        new XAttribute("FromSheet", Id),
                        new XAttribute("FromCell", "BeginX"),
                        new XAttribute("FromPart", "9"),
                        new XAttribute("ToSheet", ParentAction.Id),
                        new XAttribute("ToCell", "Connections.X" + (connectorNo + 1)),
                        new XAttribute("ToPart", 101 + connectorNo));
                return connectStart;
            }
        }

        public XElement ConnectEnd
        {
            get
            {
                if (connectEnd == null)
                    connectEnd = new XElement("Connect",
                        new XAttribute("FromSheet", Id),
                        new XAttribute("FromCell", "EndX"),
                        new XAttribute("FromPart", "12"),
                        new XAttribute("ToSheet", ChildAction.Id),
                        new XAttribute("ToCell", "Connections.X1"),
                        new XAttribute("ToPart", 100));
                return connectEnd;
            }
        }

        public void Connect(Action parent, Action child, int current, int children)
        {
            Shape.Elements().Where(el => el.Attribute("N").Value == "BegTrigger").First()
                .SetAttributeValue("F", "_XFTRIGGER(Sheet." + parent.EndAction.Id + "!EventXFMod)");
            Shape.Elements().Where(el => el.Attribute("N").Value == "EndTrigger").First()
                .SetAttributeValue("F", "_XFTRIGGER(Sheet." + child.Id + "!EventXFMod)");
            var connection = XElement.Parse("<Row T='Connection' IX='" + (current + 6) + "'>" +
                                            //  "<Cell N='X' V='0' U = 'MM' F = 'Width*1/5' />" +
                                            "<Cell N='X' F = 'Width*" + current + '/' + (children + 1) + "'/>" +
                                            "<Cell N = 'Y' V = '0' U = 'MM' F = 'Height*0' />" +
                                            "<Cell N = 'DirX' V = '0' />" +
                                            "<Cell N = 'DirY' V = '1' />" +
                                            "<Cell N = 'Type' V = '0' />" +
                                            "<Cell N = 'AutoGen' V = '0' />" +
                                            "<Cell N = 'Prompt' V = '' F = 'No Formula' />" +
                                            "</Row>");
            ParentAction = parent.EndAction;
            ChildAction = child;
            connectorNo = current;

            ParentAction.Connections.Add(connection);

            Utils.Connects.Add(ConnectStart);
            Utils.Connects.Add(ConnectEnd);
        }
    }
    public class Trigger : Action
    {
        public Trigger(JProperty triggerProperty) : base(triggerProperty)
        {
        }
    }

    public class Terminate : Action
    {
        public Terminate(JProperty property, Action parent, int curCount, int children) : base(property, parent, curCount, children, "case")
        {
            Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='" + PropertyName + " | Terminate | " +
                                     Property.Value["inputs"]["runStatus"] + "' U='STR'/></Row>"));
            AddFillColour("255,51,0");
        }
    }

    public class InitVariable : Action
    {
        public InitVariable(JProperty property, Action parent, int current, int children) : base(property, parent, current, children,
            "VariableAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + PropertyName + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='Initialize Variable' U='STR'/></Row>"));

            // var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder("Variable Name: ");
            sb.AppendLine((property.Value["inputs"] as JObject)["variables"][0]["name"].ToString());
            sb.AppendLine("Type: " + (property.Value["inputs"] as JObject)["variables"][0]["type"]);
            AddText(sb);

        }
    }

    public class SetVariable : Action
    {
        public SetVariable(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "VariableAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + PropertyName + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='Set Variable' U='STR'/></Row>"));
            var sb = new StringBuilder("Variable Name: ");
            sb.AppendLine(property.Value["inputs"]["name"].ToString());
            sb.AppendLine("Value: " + property.Value["inputs"]["value"]);
            AddText(sb);
        }
    }

    public class HttpAction : Action
    {
        public HttpAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "HttpAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + PropertyName + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='HTTP Action' U='STR'/></Row>"));
            var sb = new StringBuilder("Method: ");
            sb.AppendLine((property.Value["inputs"] as JObject)["method"].ToString());
            sb.AppendLine("URI: " + (property.Value["inputs"] as JObject)["uri"]);

            if (property.Value["inputs"]["headers"] != null)
            {
                sb.AppendLine("Headers:");
                foreach (var header in property.Value["inputs"]["headers"] as JObject) sb.AppendLine(header.Key + " : " + header.Value);
            }

            if (property.Value["inputs"]["body"] != null)
                sb.AppendLine("Body:" + property.Value["inputs"]["body"]);
            //  foreach (var header in property.Value["inputs"]["body"] as JObject) sb.AppendLine(header.Key + " : " + header.Value);

            AddText(sb);
        }
    }

    public class HttpResponse : Action
    {
        public HttpResponse(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "HttpAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + PropertyName + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='HTTP Response' U='STR'/></Row>"));
            var sb = new StringBuilder("Status Code: ");
            sb.AppendLine((property.Value["inputs"] as JObject)["statusCode"].ToString());
            // sb.AppendLine("URI: " + (property.Value["inputs"] as JObject)["uri"].ToString());

            if (property.Value["inputs"]["headers"] != null)
            {
                sb.AppendLine("Headers:");
                foreach (var header in property.Value["inputs"]["headers"] as JObject) sb.AppendLine(header.Key + " : " + header.Value);
            }

            if (property.Value["inputs"]["body"] != null)
            {
                sb.AppendLine("Body:");
                foreach (var header in property.Value["inputs"]["body"] as JObject) sb.AppendLine(header.Key + " : " + header.Value);
            }

            AddText(sb);
        }
    }

    public class HttpRequest : Action
    {
        public HttpRequest(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "HttpAction")
        {
            AddName();
            AddType("HTTP Request");

            AddText("Schema: " + Property.Value["inputs"]["schema"]);

        }
    }

    public class CDSAction : Action
    {
        private const string encode = "encodeURIComponent(";
        private string text = string.Empty;
        private string type;
        private string path => Property.Value["inputs"]["path"].ToString();
        private string method => Property.Value["inputs"]["method"].ToString();

        private List<string> pathParts => path.Split('/').ToList();

        private string environment
        {
            get
            {
                var name = pathParts[3].Substring(pathParts[3].IndexOf("('") + 2, pathParts[3].IndexOf("')") - pathParts[3].IndexOf("('") - 2);
                return name == "default.cds" ? "Current" : name;
            }
        }

        public CDSAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "CDS")
        {
            AddName();
            SetText();
            AddType(type);
            AddText(text);
        }

        private void SetText()
        {
            var sb = new StringBuilder();
            List<string> omitList;
            if (Property.Value["type"].ToString() == "OpenApiConnection")
                switch (Property.Value["inputs"]["host"]["operationId"].ToString())
                {
                    case "CreateRecord":
                    case "PostItem_V2":
                        type = "Create Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null)
                            sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"]);

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null
                            ? Property.Value["inputs"]["parameters"]["entityName"].ToString()
                            : Property.Value["inputs"]["parameters"]["table"].ToString()));

                        sb.AppendLine("Fields: ");
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                            if (param.Name != "entityName" && param.Name != "table" && param.Name != "dataset")
                                sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value);
                        break;

                    case "DeleteRecord":
                    case "DeleteItem":
                        type = "Delete Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null)
                            sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"]);

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null
                            ? Property.Value["inputs"]["parameters"]["entityName"].ToString()
                            : Property.Value["inputs"]["parameters"]["table"].ToString()));
                        sb.AppendLine("Record Id: " + (Property.Value["inputs"]?["parameters"]?["recordId"] != null
                            ? Property.Value["inputs"]["parameters"]["recordId"].ToString()
                            : Property.Value["inputs"]["parameters"]["id"].ToString()));
                        break;

                    case "GetItem":
                    case "GetItem_V2":
                        type = "Get Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null)
                            sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"]);

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null
                            ? Property.Value["inputs"]["parameters"]["entityName"].ToString()
                            : Property.Value["inputs"]["parameters"]["table"].ToString()));
                        sb.AppendLine("Record Id: " + (Property.Value["inputs"]?["parameters"]?["recordId"] != null
                            ? Property.Value["inputs"]["parameters"]["recordId"].ToString()
                            : Property.Value["inputs"]["parameters"]["id"].ToString()));
                        // sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());

                        omitList = new List<string> { "entityName", "recordId", "dataset", "table", "id" };
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>()
                            .Where(el => !omitList.Any(o => o == el.Name)))
                            sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value);
                        break;

                    case "GetEntityFileImageFieldContent":
                        type = "Get File or Image Content";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"]);
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"]);
                        sb.AppendLine("Field: " + Property.Value["inputs"]["parameters"]["fileImageFieldName"]);

                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                            if (param.Name != "entityName" && param.Name != "recordId" && param.Name != "fileImageFieldName")
                                sb.AppendLine(param.Name + " : " + param.Value);
                        break;

                    case "ListRecords":
                    case "GetItems_V2":
                        type = "Get Records";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null)
                            sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"]);

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null
                            ? Property.Value["inputs"]["parameters"]["entityName"].ToString()
                            : Property.Value["inputs"]["parameters"]["table"].ToString()));

                        omitList = new List<string> { "entityName", "dataset", "table" };
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>()
                            .Where(el => !omitList.Any(o => o == el.Name)))
                            if (param.Name != "entityName")
                                sb.AppendLine(param.Name.Substring(1, param.Name.Length - 1) + " : " + param.Value);
                        break;

                    case "PerformBoundAction":
                        type = "Preform Bound Action";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"]);
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"]);
                        sb.AppendLine("Action: " + Property.Value["inputs"]["parameters"]["actionName"]);
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                            if (param.Name != "entityName" && param.Name != "recordId" && param.Name != "actionName")
                                sb.AppendLine(param.Name + " : " + param.Value);
                        break;

                    case "PerformUnboundAction":
                        type = "Perform Unbound Action";
                        sb.AppendLine("Action: " + Property.Value["inputs"]["parameters"]["actionName"]);
                        sb.AppendLine("Parameters:");
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                            if (param.Name != "actionName")
                                sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value);
                        break;

                    case "AssociateEntities":
                        type = "Relate Records";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"]);
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"]);
                        sb.AppendLine("Relationship: " + Property.Value["inputs"]["parameters"]["associationEntityRelationship"]);
                        sb.AppendLine("URL: " + Property.Value["inputs"]["parameters"]["item/@odata.id"]);
                        break;

                    case "DisassociateEntities":
                        type = "UnRelate Records";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"]);
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"]);
                        sb.AppendLine("Relationship: " + Property.Value["inputs"]["parameters"]["associationEntityRelationship"]);
                        sb.AppendLine("URL: " + Property.Value["inputs"]["parameters"]["$id"]);
                        break;

                    case "UpdateRecord":
                    case "PatchItem_V2":
                        type = "Update Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null)
                            sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"]);

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null
                            ? Property.Value["inputs"]["parameters"]["entityName"].ToString()
                            : Property.Value["inputs"]["parameters"]["table"].ToString()));
                        sb.AppendLine("Record Id: " + (Property.Value["inputs"]?["parameters"]?["recordId"] != null
                            ? Property.Value["inputs"]["parameters"]["recordId"].ToString()
                            : Property.Value["inputs"]["parameters"]["id"].ToString()));
                        // sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());

                        omitList = new List<string> { "entityName", "recordId", "dataset", "table", "id" };
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>()
                            .Where(el => !omitList.Any(o => o == el.Name)))
                            sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value);
                        break;

                    case "UpdateEntityFileImageFieldContent":
                        type = "Update File or Image Content";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"]);
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"]);
                        sb.AppendLine("Field: " + Property.Value["inputs"]["parameters"]["fileImageFieldName"]);
                        sb.AppendLine("File Name:" + Property.Value["inputs"]["parameters"]["x-ms-file-name"]);

                        break;

                    default:
                        type = Property.Value["inputs"]["host"]["operationId"].ToString();
                        break;
                }
            else
                switch (method)
                {
                    case "get":
                        if (pathParts.Last() == "items")
                        {
                            type = "List Records - " + environment;
                            sb.AppendLine("Entity: " + pathParts[5].Substring(pathParts[5].IndexOf("('") + 2,
                                pathParts[5].IndexOf("')") - pathParts[5].IndexOf("('") - 2));

                            foreach (var query in ((JObject)Property.Value["inputs"]["queries"]).Children<JProperty>())
                                sb.AppendLine(query.Name.Substring(1, query.Name.Length - 1) + ": " + query.Value);
                        }
                        else
                        {
                            sb.AppendLine("Entity: " + pathParts[5].Substring(pathParts[5].IndexOf("('") + 2,
                                pathParts[5].IndexOf("')") - pathParts[5].IndexOf("('") - 2));
                            sb.AppendLine("Id: " + pathParts[7].Substring(pathParts[7].LastIndexOf(encode) + encode.Length,
                                pathParts[7].IndexOf("))}") - (pathParts[7].LastIndexOf(encode) + encode.Length)));

                            type = "Get Record - " + environment;
                        }

                        break;

                    case "patch":
                        type = "Update Record - " + environment;
                        sb.AppendLine("Entity: " + pathParts[5].Substring(pathParts[5].IndexOf("('") + 2,
                            pathParts[5].IndexOf("')") - pathParts[5].IndexOf("('") - 2));
                        sb.AppendLine("Id: " + pathParts[7].Substring(pathParts[7].LastIndexOf(encode) + encode.Length,
                            pathParts[7].IndexOf("))}") - (pathParts[7].LastIndexOf(encode) + encode.Length)));
                        sb.AppendLine("Fields: ");
                        foreach (var query in ((JObject)Property.Value["inputs"]["body"]).Children<JProperty>())
                            sb.AppendLine(query.Name + ": " + query.Value);

                        break;

                    case "delete":

                        type = "Delete Record - " + pathParts[2].Substring(pathParts[2].IndexOf("('") + 2,
                            pathParts[2].IndexOf("')") - pathParts[2].IndexOf("('") - 2);
                        sb.AppendLine("Entity: " + pathParts[4].Substring(pathParts[4].IndexOf("('") + 2,
                            pathParts[4].IndexOf("')") - pathParts[4].IndexOf("('") - 2));
                        sb.AppendLine("Id: " + pathParts[6].Substring(pathParts[6].LastIndexOf(encode) + encode.Length,
                            pathParts[6].IndexOf("))}") - (pathParts[6].LastIndexOf(encode) + encode.Length)));

                        break;

                    default:
                        type = "Unknown";
                        break;
                }

            text = sb.ToString();
        }
    }

    public class CDSTriggerAction : Action
    {
        private Dictionary<string, string> conditions = new Dictionary<string, string>
        {
            {"1", "Create"}, {"2", "Delete"}, {"3", "Update"},
            {"4", "Create or Update"}, {"5", "Create or Delete"}, {"6", "Update or Delete"}, {"7", "Create or Update or Delete"}
        };

        private Dictionary<string, string> scope = new Dictionary<string, string>
        {
            {"1", "User"}, {"2", "Business Unit"}, {"3", "Parent: Child business unit"},
            {"4", "Organization"}
        };

        private Dictionary<string, string> runAs = new Dictionary<string, string>
            {{"1", "Triggering User"}, {"2", "Record Owner"}, {"3", "Process Owner"}};

        public CDSTriggerAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "CDS")
        {
            AddName();
            AddType("CDS Trigger");
            var sb = new StringBuilder("Trigger Condition: ");
            sb.Append(conditions.First(kvp => kvp.Key == Property.Value["inputs"]["parameters"]["subscriptionRequest/message"].ToString()).Value);
            sb.AppendLine(" Entity: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/entityname"]);
            sb.AppendLine("Scope: " +
                          conditions.First(kvp => kvp.Key == Property.Value["inputs"]["parameters"]["subscriptionRequest/scope"].ToString()).Value);

            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/runas"] != null)
                sb.AppendLine("Run As: " +
                              conditions.First(kvp => kvp.Key == Property.Value["inputs"]["parameters"]["subscriptionRequest/runas"].ToString())
                                  .Value);
            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/filteringattributes"] != null)
                sb.AppendLine("Only These attributes: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/filteringattributes"]);
            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/filterexpression"] != null)
                sb.AppendLine("Filter Expression: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/filterexpression"]);
            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/postponeuntil"] != null)
                sb.AppendLine("Postpone Until: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/postponeuntil"]);

            AddText(sb);
        }
    }

    public class CDSStepAction : Action
    {
        public CDSStepAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "CDS")
        {
            AddName();
            AddType("Flow Step Executed");
            if (Property.Value["inputs"]?["schema"]?["properties"] != null && Property.Value["inputs"]["schema"]["properties"].HasValues)
            {
                var metaList = new List<Meta>();

                if (Property.Value["inputs"]?["schema"]?["properties"]?["rows"]?["items"]?["required"] != null &&
                    Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["required"].HasValues)
                    foreach (var props in Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["required"].Children<JToken>())
                        metaList.Add(new Meta { Label = props.ToString() });
                if (Property.Value["inputs"]?["schema"]?["properties"]?["rows"]?["items"]?["properties"] != null &&
                    Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["properties"].HasValues)
                {
                    var sb = new StringBuilder("Fields: ").AppendLine();
                    foreach (var props in Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["properties"].Children<JProperty>())
                        sb.AppendLine(props.Value["title"] + " : " + (metaList.Any(m => m.Label == props.Name) ? "(rqd) " : string.Empty) + "(" +
                                      props.Value["type"] + ") " + props.Value["description"]);
                    AddText(sb);
                }
            }
        }
    }

    public class TeamsAction : Action
    {
        public TeamsAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Teams")
        {
            AddName();
            var sb = new StringBuilder();
            switch (Property.Value["inputs"]["method"].ToString())
            {
                case "get":
                    AddType("Get");
                    break;

                case "post":
                    AddType("Add");
                    break;

                case "patch":
                    AddType("Update");
                    break;

                case "delete":
                    AddText("Delete");
                    break;
            }

            if (Property.Value["inputs"]["body"] != null && Property.Value["inputs"]["body"].HasValues)
            {
                sb.AppendLine("Parameters:");
                foreach (var props in ((JObject)Property.Value["inputs"]["body"]).Children<JProperty>())
                    sb.AppendLine(props.Name + ": " + props.Value);
            }

            AddText(sb.ToString());
        }
    }

    public class ExcelAction : Action
    {
        public ExcelAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Excel")

        {
            AddName();
            var sb = new StringBuilder("Parameters").AppendLine();

            if (Property.Value["type"].ToString() == "ApiConnection")
            {
                switch (Property.Value["inputs"]["method"].ToString())
                {
                    case "get":
                        AddType("Get");
                        break;

                    case "post":
                        AddType("Add");
                        break;

                    case "patch":
                        AddType("Update");
                        break;

                    case "delete":
                        AddType("Delete");
                        break;
                }

                sb.AppendLine(Property.Value["inputs"]["path"].ToString());
                if (Property.Value["inputs"]["queries"] != null && Property.Value["inputs"]["queries"].HasValues)
                    foreach (var props in Property.Value["inputs"]["queries"].Children<JProperty>())
                        sb.AppendLine(props.Name + " : " + props.Value);
            }
            else
            {
                AddType("Excel " + Property.Value["inputs"]["host"]["operationId"]);
                var metaList = new List<Meta>();
                if (Property.Value["metadata"] != null && Property.Value["metadata"].HasValues)
                    foreach (var props in Property.Value["metadata"].Children<JProperty>())
                        metaList.Add(new Meta { Id = props.Name, Label = props.Value.ToString() });

                if (Property.Value["inputs"]["parameters"] != null && Property.Value["inputs"]["parameters"].HasValues)
                    foreach (var props in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        sb.AppendLine(props.Name + " : " + (metaList.Any(m => m.Id == props.Value.ToString())
                            ? metaList.First(m => m.Id == props.Value.ToString()).Label
                            : props.Value.ToString()));
            }

            AddText(sb.ToString());
        }
    }

    public class OfficeAction : Action
    {
        public OfficeAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Outlook")
        {
            AddName();
            var sb = new StringBuilder("Parameters").AppendLine();
            if (Property.Value["type"].ToString() == "ApiConnection")
            {
                if (Property.Value["metadata"]?["flowSystemMetadata"]?["swaggerOperationId"] != null)
                    switch (Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString())
                    {
                        case "SendEmailV2":
                            AddType("Send an Email (V2)");
                            break;

                        default:
                            AddType(Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString());
                            break;
                    }
                else AddType(string.Empty);

                sb.AppendLine("Method: " + Property.Value["inputs"]["method"]);
                sb.AppendLine("Path: " + Property.Value["inputs"]["path"]);

                if (Property.Value["inputs"]?["body"] != null && Property.Value["inputs"]["body"].HasValues)
                    foreach (var props in Property.Value["inputs"]["body"].Children<JProperty>())
                        sb.AppendLine(props.Name + " : " + props.Value);
            }
            else
            {
                if (Property.Value["inputs"]?["host"]?["operationId"] != null)
                    switch (Property.Value["inputs"]?["host"]?["operationId"].ToString())
                    {
                        case "SendEmailV2":
                            AddType("Send an Email (V2)");
                            break;

                        case "V4CalendarPostItem":
                            AddType("Create an Event (V4)");
                            break;

                        default:
                            AddType(Property.Value["inputs"]?["host"]?["operationId"].ToString());
                            break;
                    }
                else AddType(string.Empty);

                if (Property.Value["inputs"]["parameters"] != null && Property.Value["inputs"]["parameters"].HasValues)
                    foreach (var props in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        sb.AppendLine(props.Name + " : " + props.Value);
            }

            AddText(sb.ToString());
        }
    }

    public class FlowAction : Action
    {
        public FlowAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Flow")
        {
            AddName();
            var sb = new StringBuilder("Parameters").AppendLine();
            if (Property.Value["type"].ToString() == "ApiConnection")
            {
                if (Property.Value["metadata"]?["flowSystemMetadata"]?["swaggerOperationId"] != null)
                    switch (Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString())
                    {
                        case "ListApis":
                            AddType("List Connectors");
                            break;

                        default:
                            AddType(Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString());
                            break;
                    }
                else AddType(string.Empty);

                sb.AppendLine("Method: " + Property.Value["inputs"]["method"]);
                sb.AppendLine("Path: " + Property.Value["inputs"]["path"]);

                if (Property.Value["inputs"]?["body"] != null && Property.Value["inputs"]["body"].HasValues)
                    foreach (var props in Property.Value["inputs"]["body"].Children<JProperty>())
                        sb.AppendLine(props.Name + " : " + props.Value);
            }
            else
            {
                if (Property.Value["inputs"]?["host"]?["operationId"] != null)
                    switch (Property.Value["inputs"]?["host"]?["operationId"].ToString())
                    {
                        case "ListApis":
                            AddType("List Connectors");
                            break;

                        case "V4CalendarPostItem":
                            AddType("Create an Event (V4)");
                            break;

                        default:
                            AddType(Property.Value["inputs"]?["host"]?["operationId"].ToString());
                            break;
                    }
                else AddType(string.Empty);

                if (Property.Value["inputs"]["parameters"] != null && Property.Value["inputs"]["parameters"].HasValues)
                    foreach (var props in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        sb.AppendLine(props.Name + " : " + props.Value);
            }

            AddText(sb.ToString());
        }
    }

    internal class FlowButtonAction : Action
    {
        public FlowButtonAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Flow")
        {
            AddName("Flow Button Trigger");
            AddType("Trigger");
            AddText("Schema: " + Property.Value["inputs"]["schema"]);
        }
    }

    internal class RecurrenceAction : Action
    {
        public RecurrenceAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Flow")
        {
            AddName();
            AddType("Reoccuring Flow");

            if (Property.Value["recurrence"] != null)
            {
                var sb = new StringBuilder("Frequency: " + Property.Value["recurrence"]["frequency"]).AppendLine();
                sb.AppendLine("Interval: " + Property.Value["recurrence"]["interval"]);
                sb.AppendLine("Start: " + Property.Value["recurrence"]["startTime"]);
                AddText(sb);
            }
        }
    }

    internal class PAButtonAction : Action
    {
        public PAButtonAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "PowerApp")
        {
            AddName("Power Apps Button Trigger");
            AddType("Trigger");

            if (Property.Value["inputs"]?["schema"] != null)
            {
                var sb = new StringBuilder("Properties: ").AppendLine();
                var metaList = new List<Meta>();
                if (Property.Value["inputs"]?["schema"]?["required"] != null && Property.Value["inputs"]["schema"]["required"].HasValues)
                    foreach (var props in Property.Value["inputs"]["schema"]["required"].Children<JToken>())
                        metaList.Add(new Meta { Label = props.ToString() });
                if (Property.Value["inputs"]?["schema"]?["properties"] != null && Property.Value["inputs"]["schema"]["properties"].HasValues)
                    foreach (var props in Property.Value["inputs"]["schema"]["properties"].Children<JProperty>())
                        sb.AppendLine(props.Name + " : " + (metaList.Any(m => m.Label == props.Name) ? "(rqd) " : string.Empty) + props.Value["description"]);
                AddText(sb);
            }
        }
    }

    public class Meta
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }
}