using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //Shapes.Add(shape);

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

    public class ConditionAction : Action
    {
        //public List<Action> FinalActions = new List<Action>();

        public ConditionAction(JProperty property, Action parent, int current, int children, string type) : base(property, parent, current, children, type)
        {
        }

        protected void AddChildActions(IEnumerable<JProperty> childActions, Action parent, int finalNo)
        {
            int childCount = childActions.Count();
            int curCount = 0;
            foreach (var actionProperty in childActions)
            {
                var childAction = Utils.AddAction(actionProperty, parent, ++curCount, childCount);
                FinalActions[finalNo] = childAction.EndAction;
                if (actionProperty.Parent != null && actionProperty.Parent.Children<JProperty>().Any(el => el.Value["runAfter"].HasValues && ((JProperty)el.Value["runAfter"].First()).Name == childAction.PropertyName))
                {
                    AddChildActions(actionProperty.Parent.Children<JProperty>().Where(el => el.Value["runAfter"].HasValues && ((JProperty)el.Value["runAfter"].First()).Name == childAction.PropertyName), childAction, finalNo);
                }
            }
        }

    }

    public class IfAction : ConditionAction
    {

        public CaseAction Yes { get; private set; }
        public CaseAction No { get; private set; }

        private void CreateYesNo()
        {
            Yes = new CaseAction(this, 1, 2, Property.Name + ".Yes");
            Yes.Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='If yes' U='STR'/></Row>"));
            Yes.AddFillColour("136, 218, 141");

            No = new CaseAction(this, 2, 2, Property.Name + ".No");
            No.Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='If no' U='STR'/></Row>"));
            No.AddFillColour("251, 137, 129");
            FinalActions.Add(Yes);
            if (Property.Value["actions"] != null && Property.Value["actions"].Count() > 0)
            {
                AddChildActions(Property.Value["actions"].Children<JProperty>().Where(a => !a.Value["runAfter"].HasValues), Yes, 0);
            }

            FinalActions.Add(No);
            if (Property.Value["else"] != null && ((JObject)Property.Value["else"])["actions"] != null)
            {
                AddChildActions((Property.Value["else"] as JObject)["actions"].Children<JProperty>().Where(el => !el.Value["runAfter"].HasValues), No, 1);
            }

            EndAction = new CaseAction(this, "If");
            EndAction.AddFillColour("221, 223, 224");
        }

        public IfAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "condition")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + property.Name + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='If' U='STR'/></Row>"));
            var sb = new StringBuilder("Expression: ");
            var condition = ((JObject)property.Value["expression"]).Children<JProperty>().First();
            sb.Append(condition.Value.First() + " " + condition.Name + " " + condition.Value.Last());
            AddText(sb);
            CreateYesNo();
        }
    }

    public class SwitchAction : ConditionAction
    {
        public SwitchAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "condition")
        {
            AddFillColour("234,237,239");
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + property.Name + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='Switch' U='STR'/></Row>"));
            var sb = new StringBuilder("Expression: ");
            // var condition = ((JObject)property.Value["expression"]).Children<JProperty>().First();
            sb.Append(property.Value["expression"]);
            AddText(sb);

            CreateCases();
        }

        private void CreateCases()
        {
            int curCount = 0;
            int childCount = ((JObject)Property.Value["cases"]).Children<JProperty>().Count();
            if (Property.Value["default"].HasValues) childCount++;
            foreach (var caseProperty in ((JObject)Property.Value["cases"]).Children<JProperty>())
            {
                var caseAction = new CaseAction(caseProperty, this, ++curCount, childCount);
                caseAction.Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='" + caseProperty.Name + " | Value = " + caseProperty.Value["case"] + "' U='STR'/></Row>"));
                FinalActions.Add(caseAction);
                if (caseProperty.Value["actions"] != null && ((JObject)caseProperty.Value["actions"]).Children<JProperty>().Count() > 0)
                {
                    AddChildActions(((JObject)caseProperty.Value["actions"]).Children<JProperty>().Where(el => !el.Value["runAfter"].HasValues), caseAction, FinalActions.Count() - 1);
                }
            }

            if (Property.Value["default"]["actions"].HasValues)
            {
                var defaultAction = new CaseAction(this, ++curCount, childCount, "Default");
                defaultAction.Props.Add(XElement.Parse("<Row N='ActionCase'> <Cell N='Value' V='Default:' U='STR'/></Row>"));
                FinalActions.Add(defaultAction);
                AddChildActions(((JObject)Property.Value["default"]["actions"]).Children<JProperty>().Where(el => !el.Value["runAfter"].HasValues), defaultAction, FinalActions.Count() - 1);
            }

            EndAction = new CaseAction(this, "Switch");
            EndAction.AddFillColour("234,237,239");
        }
    }

    public class ForEachAction : ConditionAction
    {
        public ForEachAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "condition")
        {
            Props.Add(XElement.Parse("<Row N='ActionName'> <Cell N='Value' V='" + property.Name + "' U='STR'/></Row>"));
            Props.Add(XElement.Parse("<Row N='ActionType'> <Cell N='Value' V='For Each' U='STR'/></Row>"));
            var sb = new StringBuilder("On: ");
            // var condition = ((JObject)property.Value["expression"]).Children<JProperty>().First();
            sb.Append(property.Value["foreach"]);
            AddText(sb);
            FinalActions.Add(this);
            AddChildActions(((JObject)Property.Value["actions"]).Children<JProperty>().Where(el => !el.Value["runAfter"].HasValues), this, 0);

            EndAction = new CaseAction(this, "For Each");
            EndAction.AddFillColour("234,237,239");
        }
    }

    public class ScopeAction : ConditionAction
    {
        public ScopeAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Scope")
        {
            AddName();
            AddType("Scope");

            FinalActions.Add(this);

            AddChildActions(((JObject)Property.Value["actions"]).Children<JProperty>().Where(el => !el.Value["runAfter"].HasValues), this, 0);

            EndAction = new CaseAction(this, "Scope");
            EndAction.AddFillColour("238,225,217");
        }
    }

    public class UntilAction : ConditionAction
    {
        public UntilAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "condition")
        {
            AddName();
            AddType("Do Until");

            var sb = new StringBuilder("Expression: " + Property.Value["expression"]).AppendLine();
            if (Property.Value["limit"] != null && Property.Value["limit"].HasValues)
            {
                sb.AppendLine("Limit:");
                foreach (var props in Property.Value["limit"].Children<JProperty>())
                {
                    sb.AppendLine(props.Name + ": " + props.Value);
                }
            }
            AddText(sb.ToString());

            FinalActions.Add(this);

            AddChildActions(((JObject)Property.Value["actions"]).Children<JProperty>().Where(el => !el.Value["runAfter"].HasValues), this, 0);

            EndAction = new CaseAction(this, "Do Until");
            EndAction.AddFillColour("234,237,239");
        }
    }

    public class ChangeSetAction : ConditionAction
    {
        public ChangeSetAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "CDS")
        {
            AddName();
            AddType("Change Set");
            FinalActions.Add(this);

            foreach (var childProperty in ((JObject)Property.Value["actions"]).Children<JProperty>())
            {
                FinalActions[0] = Utils.AddAction(childProperty, FinalActions[0], 1, 1);
            }

            // AddChildActions(((JObject)Property.Value["actions"]).Children<JProperty>(), this, 0);

            EndAction = new CaseAction(this, "Change Set");
            EndAction.AddFillColour("234,223,234");
        }
    }
}