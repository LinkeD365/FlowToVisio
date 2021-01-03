using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Xml.Linq;

namespace LinkeD365.FlowToVisio
{
    public class CaseAction : Action
    {
        public CaseAction(Action parent, int current, int children, string Name) : base(parent)
        {
            Shape = new XElement(GetTemplateShape("case"));
            this.current = current;
            this.children = children;
            Shape.SetAttributeValue("NameU", Name);
            CalcPosition();

            Line line = new Line();
            line.Connect(Parent, this, current, children);
        }

        public CaseAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "case")
        {
        }

        public CaseAction(ConditionAction conditionAction, string type) : base()
        {
            Shape = new XElement(GetTemplateShape("case"));
            current = 1;
            children = 1;
            Shape.SetAttributeValue("NameU", conditionAction.PropertyName + ".End");
            Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='End " + type + " : " + conditionAction.PropertyName + "' U='STR'/></Row>"));
            PinY = conditionAction.FinalActions.Min(el => el.PinY) - offsetY; //Double.Parse(Parent.Shape.Elements().Where(el => el.Attribute("N").Value == "PinY").First().Attribute("V").Value) - offsetY);
            PinX = conditionAction.PinX;// Double.Parse(Parent.Shape.Elements().Where(el => el.Attribute("N").Value == "PinX").First().Attribute("V").Value) - CalcX;
            SetPosition();

            foreach (var parentAction in conditionAction.FinalActions)
            {
                Line line = new Line();
                line.Connect(parentAction, this, current, children);
            }
        }
    }
}