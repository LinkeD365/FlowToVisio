using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinkeD365.FlowToVisio
{
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
                double width = children + 1;
                return (-(width / 2) + (double)current / (children + 1) * width) * offsetX;
                //    return (Math.Ceiling((double)children / 2) - current + (children % 2 == 0 ? 1 : 0)) * offsetX;
            }
        }

        protected void CalcPosition()
        {
            PinY = Parent.EndAction.PinY -
                   offsetY; // Double.Parse(Shape.Elements().Where(el => el.Attribute("N").Value == "PinY").First().Attribute("V").Value) - offsetY);
            PinX = Parent.EndAction.PinX +
                   CalcX; // Double.Parse(parent.Shape.Elements().Where(el => el.Attribute("N").Value == "PinX").First().Attribute("V").Value) - CalcX;
            SetPosition();
        }

        protected void SetPosition()
        {
            Shape.Elements().First(el => el.Attribute("N").Value == "PinY").SetAttributeValue("V", PinY);
            Shape.Elements().First(el => el.Attribute("N").Value == "PinX").SetAttributeValue("V", PinX);
        }

        protected int current = 0;
        protected int children = 0;

        public XElement Connections
        {
            get
            {
                if (sections == null)
                {
                    var elements =
                        from element in Shape.Descendants()
                        where element.Name.LocalName == "Section" && element.Attribute("N").Value == "Connection"
                        select element;
                    if (!elements.Any())
                    {
                        sections = new XElement("Section");
                        sections.SetAttributeValue("N", "Connection");
                        Shape.Add(sections);
                    }
                    else
                        sections = elements.FirstOrDefault();
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
                    var elements =
                        from element in Shape.Descendants()
                        where element.Name.LocalName == "Section" && element.Attribute("N").Value == "Property"
                        select element;
                    if (!elements.Any())
                    {
                        props = new XElement("Section");
                        props.SetAttributeValue("N", "Property");
                        Shape.Add(props);
                    }
                    else
                        props = elements.FirstOrDefault();
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
            AddProp("ActionName", PropertyName);
        }

        protected void AddText(string text)
        {
            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder();

            if (Utils.Display.ShowTrackingID && Property.Value["correlation"]?["clientTrackingId"] != null)
                sb.AppendLine("Client Tracking ID: " + Property.Value["correlation"]["clientTrackingId"]).ToString();

            if (Utils.Display.ShowConCurrency && Property.Value["runtimeConfiguration"]?["concurrency"]?["runs"] != null)
                sb.AppendLine("ConCurrency: " + Property.Value["runtimeConfiguration"]["concurrency"]["runs"]).ToString();
            if (Utils.Display.ShowSecure && Property.Value["runtimeConfiguration"]?["secureData"]?["properties"] != null)
            {
                if (((JArray)(Property.Value["runtimeConfiguration"]["secureData"]["properties"])).Select(jt => jt.ToString()).ToList().Any(st => st == "inputs")) sb.AppendLine("Secure Inputs: true");
                if (((JArray)(Property.Value["runtimeConfiguration"]["secureData"]["properties"])).Select(jt => jt.ToString()).ToList().Any(st => st == "outputs")) sb.AppendLine("Secure Outputs: true");
            }
            if (Utils.Display.ShowTriggers && Property.Value["conditions"] != null)
            {
                sb.AppendLine("Triggers:");
                ((JArray)Property.Value["conditions"]).Children<JToken>().ToList().ForEach(jt => sb.AppendLine(jt["expression"].ToString()));
            }

            if (Utils.Display.ShowTrackedProps && Property.Value["trackedProperties"] != null)
            {
                sb.AppendLine("Tracked Properties:");
                (Property.Value["trackedProperties"]).Children<JProperty>().ToList().ForEach(jp => sb.AppendLine(jp.Name + " : " + jp.Value.ToString()));
            }
            if (Property.Value["description"] != null) sb.AppendLine("Comment: " + Property.Value["description"]);
            sb.AppendLine(text).ToString();
            textElement.ReplaceWith(XElement.Parse("<Text><![CDATA[" + sb.ToString() + "]]></Text>"));
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
            protected set => endAction = value;
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

                AddRunAfter();
            else
            {
                // Variant to allow for international useage
                PinX = double.TryParse(Shape.Elements().First(el => el.Attribute("N").Value == "PinX").Attribute("V").Value, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var tempPinX) ? tempPinX : 0.0;
                PinY = double.TryParse(Shape.Elements().First(el => el.Attribute("N").Value == "PinY").Attribute("V").Value, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var tempPiny) ? tempPiny : 0.0;
            }

            // if (this is Action) AddBaseText();
        }

        private void AddRunAfter()
        {
            if (Property.Value["runAfter"] != null && Property.Value["runAfter"].HasValues) // #2 Added check for null
            {
                var runAfterString = Property.Value["runAfter"].Children().First().Value<JProperty>().Value.Where(jt => jt.ToString() != "Succeeded").Aggregate(string.Empty, (accumulator, jToken) => accumulator += jToken + " | ");

                if (runAfterString != string.Empty)
                {
                    runAfterString = runAfterString.Substring(0, runAfterString.Length - 3);
                    var header = new CaseAction(Parent, current, children, PropertyName + runAfterString + current);
                    header.AddName(runAfterString);
                    header.Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='" + runAfterString + "' U='STR'/></Row>"));
                    header.AddFillColour("255,242,204");
                    Parent = header;
                    current = 1;
                    children = 1;
                }
            }
            CalcPosition();
            var line = new Line();
            line.Connect(Parent.EndAction, this, current, children);
        }

        public Action() : base()
        {
            Utils.actionCount++;
        }

        private void AddBaseText()
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + PropertyName + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='" + Property.Value["type"] + "' U='STR'/></Row>"));
            // var sb = "<Text><cp IX = '0' /><pp IX = '0' />" + PropertyName + "\n";
            //   var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();
            var sb = new StringBuilder("Properties: ");
            sb.AppendLine();
            if (((JObject)Property.Value).Properties().Count() > 0)
                foreach (var item in ((JObject)Property.Value).Properties().Where(p => p.Name != "runAfter"))
                    sb.AppendLine(item.Name + " : " + @item.Value);
            AddText(sb);
        }

        public void AddFillColour(string colour)
        {
            Shape.Add(XElement.Parse("<Cell N = 'FillForegnd' V = '' F = 'THEMEGUARD(RGB(" + colour + "))' />"));
        }
    }
}