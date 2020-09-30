﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LinkeD365.FlowToVisio
{
    public class Utils
    {
        public static int actionCount = 0;
        public static JObject Root { get; set; }

        public static XDocument XMLPage { get; set; }

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
                        Ai.WriteEvent("No Action: " + actionProperty.Value["type"].ToString());

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
                Ai.WriteEvent("No Request Trigger: " + actionProperty.Value["kind"].ToString());
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
                Ai.WriteEvent("No API Trigger: " + actionProperty.Value["inputs"]?["operationId"].ToString());
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
                }
                Ai.WriteEvent("No Open API Action: " + Connection.APIConnections.First(con => con.Name == connectName).Api);
            }

            return new Action(actionProperty, parent, curCount, childCount);
        }
    }

    public abstract class BaseShape
    {
        public JProperty Property { get; private set; }
        //protected XDocument xmlPage;

        protected XElement shapes;

        private XElement line;
        public double PinX { get; protected set; }
        public double PinY { get; protected set; }
        public int Id { get; protected set; }

        public XElement GetTemplateShape(string name)
        {
            IEnumerable<XElement> selectedElements =
                      from el in Shapes.Elements()
                      where el.Attribute("NameU")?.Value == name
                      select el;
            return selectedElements.DefaultIfEmpty(null).FirstOrDefault();
        }

        public XElement Line
        {
            get
            {
                if (line == null)
                {
                    IEnumerable<XElement> selectedElements =
                       from el in Shapes.Elements()
                       where el.Attribute("ID").Value == "1000"
                       select el;
                    line = selectedElements.DefaultIfEmpty(null).FirstOrDefault();
                }
                return line;
            }
        }

        public XElement Shapes
        {
            get
            {
                if (shapes == null)
                {
                    IEnumerable<XElement> elements =
                        from element in Utils.XMLPage.Descendants()
                        where element.Name.LocalName == "Shapes"
                        select element;
                    // Return the selected elements to the calling code.
                    shapes = elements.FirstOrDefault();
                }
                return shapes;
            }
        }

        public string Name
        {
            get
            {
                if (Property == null) return "Line." + Id.ToString();
                else return Property.Name;
            }
        }

        public BaseShape(JProperty property)
        {
            Property = property;
        }

        public BaseShape()
        {
        }

        private XElement shape;

        public XElement Shape
        {
            get => shape;
            set
            {
                shape = value;
                Shapes.Add(shape);
                SetId();
            }
        }

        public string PropertyName
        {
            get
            {
                if (Property == null) return string.Empty;
                var regext = new Regex("/(_{2,})|_/g, '$1'");
                return regext.Replace(Property.Name, " ");
            }
        }

        private void SetId()
        {
            if (Shape.Attribute("ID") == null) return;
            Id = Shapes.Descendants().Where(el => el.Attribute("ID") != null).Max(x => int.Parse(x.Attribute("ID").Value)) + 1;

            Shape.SetAttributeValue("ID", Id);

            foreach (var stencil in Shape.Descendants().Where(el => el.Attribute("ID") != null))
            {
                stencil.SetAttributeValue("ID", Shapes.Descendants().Where(el => el.Attribute("ID") != null).Max(x => int.Parse(x.Attribute("ID").Value)) + 1);
            }
            //if (Shape.Elements().Any(el => el.Name.LocalName == "Shapes"))
            //{
            //    foreach(var stencilShape in Shape.Elements().Where(el => el.Name.LocalName == Shapes).)
            //}
        }
    }

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
                {
                    connectStart = new XElement("Connect",
                       new XAttribute("FromSheet", Id),
                       new XAttribute("FromCell", "BeginX"),
                       new XAttribute("FromPart", "9"),
                       new XAttribute("ToSheet", ParentAction.Id),
                       new XAttribute("ToCell", "Connections.X" + (connectorNo + 1)),
                       new XAttribute("ToPart", 101 + connectorNo));
                }
                return connectStart;
            }
        }

        public XElement ConnectEnd
        {
            get
            {
                if (connectEnd == null)
                {
                    connectEnd = new XElement("Connect",
                       new XAttribute("FromSheet", Id),
                       new XAttribute("FromCell", "EndX"),
                       new XAttribute("FromPart", "12"),
                       new XAttribute("ToSheet", ChildAction.Id),
                       new XAttribute("ToCell", "Connections.X1"),
                       new XAttribute("ToPart", 100));
                }
                return connectEnd;
            }
        }

        public void Connect(Action parent, Action child, int current, int children)
        {
            Shape.Elements().Where(el => el.Attribute("N").Value == "BegTrigger").First().
                SetAttributeValue("F", "_XFTRIGGER(Sheet." + parent.EndAction.Id + "!EventXFMod)");
            Shape.Elements().Where(el => el.Attribute("N").Value == "EndTrigger").First().
                SetAttributeValue("F", "_XFTRIGGER(Sheet." + child.Id + "!EventXFMod)");
            var connection = XElement.Parse("<Row T='Connection' IX='" + (current + 6).ToString() + "'>" +
                   //  "<Cell N='X' V='0' U = 'MM' F = 'Width*1/5' />" +
                   "<Cell N='X' F = 'Width*" + current.ToString() + '/' + (children + 1).ToString() + "'/>" +
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

    public class Action : BaseShape
    {
        private Action endAction;
        public List<Action> FinalActions = new List<Action>();
        private XElement sections;
        protected double offsetX = 3; // inches
        protected double offsetY = 1.1;

        protected double CalcX
        {
            get
            {
                if (children == 1) return 0;
                //    if (children == 2)
                double width = (children + 1);
                return (-(width / 2) + ((double)current / (children + 1) * width)) * offsetX;
                //    return (Math.Ceiling((double)children / 2) - current + (children % 2 == 0 ? 1 : 0)) * offsetX;
            }
        }

        protected void CalcPosition()
        {
            PinY = Parent.EndAction.PinY - offsetY;// Double.Parse(Shape.Elements().Where(el => el.Attribute("N").Value == "PinY").First().Attribute("V").Value) - offsetY);
            PinX = Parent.EndAction.PinX + CalcX;// Double.Parse(parent.Shape.Elements().Where(el => el.Attribute("N").Value == "PinX").First().Attribute("V").Value) - CalcX;
            SetPosition();
        }

        protected void SetPosition()
        {
            Shape.Elements().Where(el => el.Attribute("N").Value == "PinY").First().SetAttributeValue("V", PinY);
            Shape.Elements().Where(el => el.Attribute("N").Value == "PinX").First().SetAttributeValue("V", PinX);
        }

        protected int current = 0;
        protected int children = 0;

        public XElement Connections
        {
            get
            {
                if (sections == null)
                {
                    IEnumerable<XElement> elements =
                     from element in Shape.Descendants()
                     where element.Name.LocalName == "Section" && element.Attribute("N").Value == "Connection"
                     select element;
                    if (!elements.Any())
                    {
                        sections = new XElement("Section");
                        sections.SetAttributeValue("N", "Connection");
                        Shape.Add(sections);
                    }
                    else sections = elements.FirstOrDefault();
                }
                return sections;
            }
        }

        private XElement props;

        public XElement Props
        {
            get
            {
                if (props == null)
                {
                    IEnumerable<XElement> elements =
                     from element in Shape.Descendants()
                     where element.Name.LocalName == "Section" && element.Attribute("N").Value == "Property"
                     select element;
                    if (!elements.Any())
                    {
                        props = new XElement("Section");
                        props.SetAttributeValue("N", "Property");
                        Shape.Add(props);
                    }
                    else props = elements.FirstOrDefault();
                }

                return props;
            }
        }

        protected void AddProp(string name, string value)
        {
            Props.Add(XElement.Parse("<Row N='" + name + "'> <Cell N='Value' V='" + value + "' U='STR'/></Row>"));
        }

        protected void AddType(string value)
        {
            AddProp("ActionType", value);
        }

        protected void AddType()
        {
            AddType(Property.Value["type"].ToString());
        }

        protected void AddName(string value)
        {
            AddProp("ActionName", value);
        }

        protected void AddName()
        {
            AddProp("ActionName", Property.Name);
        }

        protected void AddText(string text)
        {
            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            textElement.ReplaceWith(XElement.Parse("<Text><![CDATA[" + text + "]]></Text>"));
        }

        protected void AddText(StringBuilder sb)
        {
            AddText(sb.ToString());
        }

        public Action EndAction
        {
            get
            {
                if (endAction == null) return this;
                return endAction;
            }
            protected set { endAction = value; }
        }

        public Action Parent;

        public Action(Action parent) : base()
        {
            Utils.actionCount++;
            Parent = parent;
        }

        public Action(JProperty property) : this(property, null, 0, 0, "Default")
        {
            AddBaseText();
        }

        public Action(JProperty property, Action parent, int current, int children) : this(property, parent, current, children, "Default")
        {
            AddBaseText();
        }

        public Action(JProperty property, Action parent, int current, int children, string templateName) : base(property)
        {
            Parent = parent;
            Shape = new XElement(GetTemplateShape(templateName));
            Utils.actionCount++;

            this.current = current;
            this.children = children;
            Shape.SetAttributeValue("NameU", property.Name);
            if (parent != null)
            {
                CalcPosition();
                //Shapes.Add(shape);

                Line line = new Line();
                line.Connect(Parent.EndAction, this, current, children);
            }
            else
            {
                PinX = double.Parse(Shape.Elements().Where(el => el.Attribute("N").Value == "PinX").First().Attribute("V").Value);
                PinY = double.Parse(Shape.Elements().Where(el => el.Attribute("N").Value == "PinY").First().Attribute("V").Value);
            }
            // if (this is Action) AddBaseText();
        }

        public Action() : base()
        {
            Utils.actionCount++;
        }

        private void AddBaseText()
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + PropertyName + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='" + Property.Value["type"].ToString() + "' U='STR'/></Row>"));
            // var sb = "<Text><cp IX = '0' /><pp IX = '0' />" + PropertyName + "\n";
            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder("<Text><![CDATA[Properties: ");
            sb.AppendLine();
            if (((JObject)Property.Value).Properties().Count() > 0)
            {
                foreach (var item in ((JObject)Property.Value).Properties().Where(p => p.Name != "runAfter"))
                {
                    sb.AppendLine(item.Name + " : " + @item.Value.ToString());
                }
            }
            textElement.ReplaceWith(XElement.Parse(sb.ToString() + "]]></Text>"));
        }

        public void AddFillColour(string colour)
        {
            Shape.Add(XElement.Parse("<Cell N = 'FillForegnd' V = '' F = 'THEMEGUARD(RGB(" + colour + "))' />"));
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
            Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='" + PropertyName + " | Terminate | " + Property.Value["inputs"]["runStatus"].ToString() + "' U='STR'/></Row>"));
            AddFillColour("255,51,0");
        }
    }

    public class InitVariable : Action
    {
        public InitVariable(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "VariableAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + property.Name + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='Initialize Variable' U='STR'/></Row>"));

            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder("<Text><![CDATA[Variable Name: ");
            sb.AppendLine((property.Value["inputs"] as JObject)["variables"][0]["name"].ToString());
            sb.AppendLine("Type: " + (property.Value["inputs"] as JObject)["variables"][0]["type"].ToString() + "]]></Text>");

            textElement.ReplaceWith(XElement.Parse(sb.ToString()));
        }
    }

    public class SetVariable : Action
    {
        public SetVariable(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "VariableAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + property.Name + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='Set Variable' U='STR'/></Row>"));
            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder("<Text><![CDATA[Variable Name: ");
            sb.AppendLine(property.Value["inputs"]["name"].ToString());
            sb.AppendLine("Value: " + property.Value["inputs"]["value"].ToString() + "]]></Text>");

            textElement.ReplaceWith(XElement.Parse(sb.ToString()));
        }
    }

    public class HttpAction : Action
    {
        public HttpAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "HttpAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + property.Name + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='HTTP Action' U='STR'/></Row>"));
            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder("<Text><![CDATA[Method: ");
            sb.AppendLine((property.Value["inputs"] as JObject)["method"].ToString());
            sb.AppendLine("URI: " + (property.Value["inputs"] as JObject)["uri"].ToString());

            if (property.Value["inputs"]["headers"] != null)
            {
                sb.AppendLine("Headers:");
                foreach (var header in property.Value["inputs"]["headers"] as JObject)
                {
                    sb.AppendLine(header.Key + " : " + header.Value.ToString());
                }
            }
            if (property.Value["inputs"]["body"] != null)
            {
                sb.AppendLine("Body:");
                foreach (var header in property.Value["inputs"]["body"] as JObject)
                {
                    sb.AppendLine(header.Key + " : " + header.Value.ToString());
                }
            }

            //sb.AppendLine("Type: " + (property.Value["inputs"] as JObject)["variables"][0]["type"].ToString() + "]]></Text>");

            textElement.ReplaceWith(XElement.Parse(sb.ToString() + "]]></Text>"));
        }
    }

    public class HttpResponse : Action
    {
        public HttpResponse(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "HttpAction")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + property.Name + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='HTTP Response' U='STR'/></Row>"));
            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder("<Text><![CDATA[Status Code: ");
            sb.AppendLine((property.Value["inputs"] as JObject)["statusCode"].ToString());
            // sb.AppendLine("URI: " + (property.Value["inputs"] as JObject)["uri"].ToString());

            if (property.Value["inputs"]["headers"] != null)
            {
                sb.AppendLine("Headers:");
                foreach (var header in property.Value["inputs"]["headers"] as JObject)
                {
                    sb.AppendLine(header.Key + " : " + header.Value.ToString());
                }
            }
            if (property.Value["inputs"]["body"] != null)
            {
                sb.AppendLine("Body:");
                foreach (var header in property.Value["inputs"]["body"] as JObject)
                {
                    sb.AppendLine(header.Key + " : " + header.Value.ToString());
                }
            }

            //sb.AppendLine("Type: " + (property.Value["inputs"] as JObject)["variables"][0]["type"].ToString() + "]]></Text>");

            textElement.ReplaceWith(XElement.Parse(sb.ToString() + "]]></Text>"));
        }
    }

    public class HttpRequest : Action
    {
        public HttpRequest(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "HttpAction")
        {
            AddName();
            AddType("HTTP Request");

            AddText("Schema: " + Property.Value["inputs"]["schema"].ToString());
        }
    }

    public class CDSAction : Action
    {
        private const string encode = "encodeURIComponent(";
        private string text = "";
        private string type;
        private string path { get { return Property.Value["inputs"]["path"].ToString(); } }
        private string method { get { return Property.Value["inputs"]["method"].ToString(); } }

        private List<string> pathParts { get { return path.Split('/').ToList(); } }

        private string environment
        {
            get
            {
                string name = pathParts[3].Substring(pathParts[3].IndexOf("('") + 2, pathParts[3].IndexOf("')") - pathParts[3].IndexOf("('") - 2);
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
            {
                switch (Property.Value["inputs"]["host"]["operationId"].ToString())
                {
                    case "CreateRecord":
                    case "PostItem_V2":
                        type = "Create Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null) sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"].ToString());

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null ?
                            Property.Value["inputs"]["parameters"]["entityName"].ToString() : Property.Value["inputs"]["parameters"]["table"].ToString()));

                        sb.AppendLine("Fields: ");
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        {
                            if (param.Name != "entityName" && param.Name != "table" && param.Name != "dataset") sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value.ToString());
                        }
                        break;

                    case "DeleteRecord":
                    case "DeleteItem":
                        type = "Delete Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null) sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"].ToString());

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null ?
                                             Property.Value["inputs"]["parameters"]["entityName"].ToString() : Property.Value["inputs"]["parameters"]["table"].ToString()));
                        sb.AppendLine("Record Id: " + (Property.Value["inputs"]?["parameters"]?["recordId"] != null ?
                             Property.Value["inputs"]["parameters"]["recordId"].ToString() : Property.Value["inputs"]["parameters"]["id"].ToString()));
                        break;

                    case "GetItem":
                    case "GetItem_V2":
                        type = "Get Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null) sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"].ToString());

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null ?
                                             Property.Value["inputs"]["parameters"]["entityName"].ToString() : Property.Value["inputs"]["parameters"]["table"].ToString()));
                        sb.AppendLine("Record Id: " + (Property.Value["inputs"]?["parameters"]?["recordId"] != null ?
                             Property.Value["inputs"]["parameters"]["recordId"].ToString() : Property.Value["inputs"]["parameters"]["id"].ToString()));
                        // sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());

                        omitList = new List<string> { "entityName", "recordId", "dataset", "table", "id" };
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(el => !omitList.Any(o => o == el.Name)))
                        {
                            sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value.ToString());
                        }
                        break;

                    case "GetEntityFileImageFieldContent":
                        type = "Get File or Image Content";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"].ToString());
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());
                        sb.AppendLine("Field: " + Property.Value["inputs"]["parameters"]["fileImageFieldName"].ToString());

                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        {
                            if (param.Name != "entityName" && param.Name != "recordId" && param.Name != "fileImageFieldName") sb.AppendLine(param.Name + " : " + param.Value.ToString());
                        }
                        break;

                    case "ListRecords":
                    case "GetItems_V2":
                        type = "Get Records";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null) sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"].ToString());

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null ?
                                             Property.Value["inputs"]["parameters"]["entityName"].ToString() : Property.Value["inputs"]["parameters"]["table"].ToString()));

                        omitList = new List<string> { "entityName", "dataset", "table" };
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(el => !omitList.Any(o => o == el.Name)))
                        {
                            if (param.Name != "entityName") sb.AppendLine(param.Name.Substring(1, param.Name.Length - 1) + " : " + param.Value.ToString());
                        }
                        break;

                    case "PerformBoundAction":
                        type = "Preform Bound Action";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"].ToString());
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());
                        sb.AppendLine("Action: " + Property.Value["inputs"]["parameters"]["actionName"].ToString());
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        {
                            if (param.Name != "entityName" && param.Name != "recordId" && param.Name != "actionName") sb.AppendLine(param.Name + " : " + param.Value.ToString());
                        }
                        break;

                    case "PerformUnboundAction":
                        type = "Perform Unbound Action";
                        sb.AppendLine("Action: " + Property.Value["inputs"]["parameters"]["actionName"].ToString());
                        sb.AppendLine("Parameters:");
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>())
                        {
                            if (param.Name != "actionName") sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value.ToString());
                        }
                        break;

                    case "AssociateEntities":
                        type = "Relate Records";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"].ToString());
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());
                        sb.AppendLine("Relationship: " + Property.Value["inputs"]["parameters"]["associationEntityRelationship"].ToString());
                        sb.AppendLine("URL: " + Property.Value["inputs"]["parameters"]["item/@odata.id"].ToString());
                        break;

                    case "DisassociateEntities":
                        type = "UnRelate Records";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"].ToString());
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());
                        sb.AppendLine("Relationship: " + Property.Value["inputs"]["parameters"]["associationEntityRelationship"].ToString());
                        sb.AppendLine("URL: " + Property.Value["inputs"]["parameters"]["$id"].ToString());
                        break;

                    case "UpdateRecord":
                    case "PatchItem_V2":
                        type = "Update Record";
                        if (Property.Value["inputs"]?["parameters"]?["dataset"] != null) sb.AppendLine("Environment: " + Property.Value["inputs"]["parameters"]["dataset"].ToString());

                        sb.AppendLine("Entity: " + (Property.Value["inputs"]?["parameters"]?["entityName"] != null ?
                                             Property.Value["inputs"]["parameters"]["entityName"].ToString() : Property.Value["inputs"]["parameters"]["table"].ToString()));
                        sb.AppendLine("Record Id: " + (Property.Value["inputs"]?["parameters"]?["recordId"] != null ?
                             Property.Value["inputs"]["parameters"]["recordId"].ToString() : Property.Value["inputs"]["parameters"]["id"].ToString()));
                        // sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());

                        omitList = new List<string> { "entityName", "recordId", "dataset", "table", "id" };
                        foreach (var param in Property.Value["inputs"]["parameters"].Children<JProperty>().Where(el => !omitList.Any(o => o == el.Name)))
                        {
                            sb.AppendLine(param.Name.Substring(5, param.Name.Length - 5) + " : " + param.Value.ToString());
                        }
                        break;

                    case "UpdateEntityFileImageFieldContent":
                        type = "Update File or Image Content";
                        sb.AppendLine("Entity: " + Property.Value["inputs"]["parameters"]["entityName"].ToString());
                        sb.AppendLine("Record Id: " + Property.Value["inputs"]["parameters"]["recordId"].ToString());
                        sb.AppendLine("Field: " + Property.Value["inputs"]["parameters"]["fileImageFieldName"].ToString());
                        sb.AppendLine("File Name:" + Property.Value["inputs"]["parameters"]["x-ms-file-name"].ToString());

                        break;

                    default:
                        type = Property.Value["inputs"]["host"]["operationId"].ToString();
                        break;
                }
            }
            else
            {
                switch (method)
                {
                    case "get":
                        if (pathParts.Last() == "items")
                        {
                            type = "List Records - " + environment;
                            sb.AppendLine("Entity: " + pathParts[5].Substring(pathParts[5].IndexOf("('") + 2, pathParts[5].IndexOf("')") - pathParts[5].IndexOf("('") - 2));

                            foreach (var query in ((JObject)Property.Value["inputs"]["queries"]).Children<JProperty>())
                            {
                                sb.AppendLine(query.Name.Substring(1, query.Name.Length - 1) + ": " + query.Value.ToString());
                            }
                        }
                        else
                        {
                            sb.AppendLine("Entity: " + pathParts[5].Substring(pathParts[5].IndexOf("('") + 2, pathParts[5].IndexOf("')") - pathParts[5].IndexOf("('") - 2));
                            sb.AppendLine("Id: " + pathParts[7].Substring(pathParts[7].LastIndexOf(encode) + encode.Length, pathParts[7].IndexOf("))}") - (pathParts[7].LastIndexOf(encode) + encode.Length)));

                            type = "Get Record - " + environment;
                        }
                        break;

                    case "patch":
                        type = "Update Record - " + environment;
                        sb.AppendLine("Entity: " + pathParts[5].Substring(pathParts[5].IndexOf("('") + 2, pathParts[5].IndexOf("')") - pathParts[5].IndexOf("('") - 2));
                        sb.AppendLine("Id: " + pathParts[7].Substring(pathParts[7].LastIndexOf(encode) + encode.Length, pathParts[7].IndexOf("))}") - (pathParts[7].LastIndexOf(encode) + encode.Length)));
                        sb.AppendLine("Fields: ");
                        foreach (var query in ((JObject)Property.Value["inputs"]["body"]).Children<JProperty>())
                        {
                            sb.AppendLine(query.Name + ": " + query.Value.ToString());
                        }

                        break;

                    case "delete":

                        type = "Delete Record - " + pathParts[2].Substring(pathParts[2].IndexOf("('") + 2, pathParts[2].IndexOf("')") - pathParts[2].IndexOf("('") - 2);
                        sb.AppendLine("Entity: " + pathParts[4].Substring(pathParts[4].IndexOf("('") + 2, pathParts[4].IndexOf("')") - pathParts[4].IndexOf("('") - 2));
                        sb.AppendLine("Id: " + pathParts[6].Substring(pathParts[6].LastIndexOf(encode) + encode.Length, pathParts[6].IndexOf("))}") - (pathParts[6].LastIndexOf(encode) + encode.Length)));

                        break;

                    default:
                        type = "Unknown";
                        break;
                }
            }
            text = sb.ToString();
        }
    }

    public class CDSTriggerAction : Action
    {
        private Dictionary<string, string> conditions = new Dictionary<string, string> { { "1", "Create" }, { "2", "Delete" }, { "3", "Update" },
            { "4", "Create or Update" }, { "5", "Create or Delete" }, { "6", "Update or Delete" }, { "7", "Create or Update or Delete" } };

        private Dictionary<string, string> scope = new Dictionary<string, string> { { "1", "User" }, { "2", "Business Unit" }, { "3", "Parent: Child business unit" },
            { "4", "Organization" } };

        private Dictionary<string, string> runAs = new Dictionary<string, string> { { "1", "Triggering User" }, { "2", "Record Owner" }, { "3", "Process Owner" } };

        public CDSTriggerAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "CDS")
        {
            AddName();
            AddType("CDS Trigger");
            var sb = new StringBuilder("Trigger Condition: ");
            sb.Append(conditions.First(kvp => kvp.Key == Property.Value["inputs"]["parameters"]["subscriptionRequest/message"].ToString()).Value);
            sb.AppendLine(" Entity: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/entityname"].ToString());
            sb.AppendLine("Scope: " + conditions.First(kvp => kvp.Key == Property.Value["inputs"]["parameters"]["subscriptionRequest/scope"].ToString()).Value);

            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/runas"] != null) sb.AppendLine("Run As: " +
               conditions.First(kvp => kvp.Key == Property.Value["inputs"]["parameters"]["subscriptionRequest/runas"].ToString()).Value);
            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/filteringattributes"] != null) sb.AppendLine("Only These attributes: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/filteringattributes"].ToString());
            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/filterexpression"] != null) sb.AppendLine("Filter Expression: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/filterexpression"].ToString());
            if (Property.Value["inputs"]?["parameters"]?["subscriptionRequest/postponeuntil"] != null) sb.AppendLine("Postpone Until: " + Property.Value["inputs"]["parameters"]["subscriptionRequest/postponeuntil"].ToString());

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
                List<Meta> metaList = new List<Meta>();

                if (Property.Value["inputs"]?["schema"]?["properties"]?["rows"]?["items"]?["required"] != null && Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["required"].HasValues)
                {
                    foreach (var props in Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["required"].Children<JToken>())
                    {
                        metaList.Add(new Meta() { Label = props.ToString() });
                    }
                }
                if (Property.Value["inputs"]?["schema"]?["properties"]?["rows"]?["items"]?["properties"] != null && Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["properties"].HasValues)
                {
                    var sb = new StringBuilder("Fields: ").AppendLine();
                    foreach (var props in Property.Value["inputs"]["schema"]["properties"]["rows"]["items"]["properties"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Value["title"] + " : " + (metaList.Any(m => m.Label == props.Name) ? "(rqd) " : "") + "(" + props.Value["type"].ToString() + ") " + props.Value["description"].ToString());
                    }
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
                {
                    sb.AppendLine(props.Name + ": " + props.Value.ToString());
                }
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
                {
                    foreach (var props in Property.Value["inputs"]["queries"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Name + " : " + props.Value.ToString());
                    }
                }
            }
            else
            {
                AddType("Excel " + Property.Value["inputs"]["host"]["operationId"].ToString());
                var metaList = new List<Meta>();
                if (Property.Value["metadata"] != null && Property.Value["metadata"].HasValues)
                {
                    foreach (var props in Property.Value["metadata"].Children<JProperty>())
                    {
                        metaList.Add(new Meta() { Id = props.Name, Label = props.Value.ToString() });
                    }
                }

                if (Property.Value["inputs"]["parameters"] != null && Property.Value["inputs"]["parameters"].HasValues)
                {
                    foreach (var props in Property.Value["inputs"]["parameters"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Name + " : " + (metaList.Any(m => m.Id == props.Value.ToString()) ? metaList.First(m => m.Id == props.Value.ToString()).Label : props.Value.ToString()));
                    }
                }
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
                {
                    switch (Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString())
                    {
                        case "SendEmailV2":
                            AddType("Send an Email (V2)");
                            break;

                        default:
                            AddType(Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString());
                            break;
                    }
                }
                else AddType("");
                sb.AppendLine("Method: " + Property.Value["inputs"]["method"].ToString());
                sb.AppendLine("Path: " + Property.Value["inputs"]["path"].ToString());

                if (Property.Value["inputs"]?["body"] != null && Property.Value["inputs"]["body"].HasValues)
                {
                    foreach (var props in Property.Value["inputs"]["body"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Name + " : " + props.Value.ToString());
                    }
                }
            }
            else
            {
                if (Property.Value["inputs"]?["host"]?["operationId"] != null)
                {
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
                }
                else AddType("");

                if (Property.Value["inputs"]["parameters"] != null && Property.Value["inputs"]["parameters"].HasValues)
                {
                    foreach (var props in Property.Value["inputs"]["parameters"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Name + " : " + props.Value.ToString());
                    }
                }
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
                {
                    switch (Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString())
                    {
                        case "ListApis":
                            AddType("List Connectors");
                            break;

                        default:
                            AddType(Property.Value["metadata"]["flowSystemMetadata"]["swaggerOperationId"].ToString());
                            break;
                    }
                }
                else AddType("");
                sb.AppendLine("Method: " + Property.Value["inputs"]["method"].ToString());
                sb.AppendLine("Path: " + Property.Value["inputs"]["path"].ToString());

                if (Property.Value["inputs"]?["body"] != null && Property.Value["inputs"]["body"].HasValues)
                {
                    foreach (var props in Property.Value["inputs"]["body"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Name + " : " + props.Value.ToString());
                    }
                }
            }
            else
            {
                if (Property.Value["inputs"]?["host"]?["operationId"] != null)
                {
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
                }
                else AddType("");

                if (Property.Value["inputs"]["parameters"] != null && Property.Value["inputs"]["parameters"].HasValues)
                {
                    foreach (var props in Property.Value["inputs"]["parameters"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Name + " : " + props.Value.ToString());
                    }
                }
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
            AddText("Schema: " + Property.Value["inputs"]["schema"].ToString());
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
                var sb = new StringBuilder("Frequency: " + Property.Value["recurrence"]["frequency"].ToString()).AppendLine();
                sb.AppendLine("Interval: " + Property.Value["recurrence"]["interval"].ToString());
                sb.AppendLine("Start: " + Property.Value["recurrence"]["startTime"].ToString());
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
                {
                    foreach (var props in Property.Value["inputs"]["schema"]["required"].Children<JToken>())
                    {
                        metaList.Add(new Meta() { Label = props.ToString() });
                    }
                }
                if (Property.Value["inputs"]?["schema"]?["properties"] != null && Property.Value["inputs"]["schema"]["properties"].HasValues)
                {
                    foreach (var props in Property.Value["inputs"]["schema"]["properties"].Children<JProperty>())
                    {
                        sb.AppendLine(props.Name + " : " + (metaList.Any(m => m.Label == props.Name) ? "(rqd) " : "") + props.Value["description"].ToString());
                    }
                }
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