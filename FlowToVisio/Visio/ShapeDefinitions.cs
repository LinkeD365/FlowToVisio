using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using VisioAutomation.VDX.Elements;
using VisioAutomation.VDX.Enums;
using VisioAutomation.VDX.Sections;

namespace LinkeD365.FlowToVisio
{
    public abstract class BaseAction : Shape
    {
        public int FontId { get; set; }
        private VisioAutomation.VDX.Sections.Char bold = new VisioAutomation.VDX.Sections.Char();
        private VisioAutomation.VDX.Sections.Char attribute = new VisioAutomation.VDX.Sections.Char();
        public string ShapeName { get; set; }

        private ParagraphFormat _left;

        private ParagraphFormat Left
        {
            get
            {
                if (_left == null)
                {
                    _left = new ParagraphFormat();
                    _left.HorzAlign.Result = ParaHorizontalAlignment.Left;
                }
                return _left;
            }
        }

        public BaseAction(string actionName, double pinx, double piny, int fontId) : base(0, pinx, piny, 1.2, 0.8)
        {
            Name = actionName;
            ShapeName = actionName;
            FontId = fontId;
            attribute.Style.Result = CharStyle.None;
            attribute.Font.Result = FontId;
            attribute.Size.Result = 6;
            bold.Style.Result = CharStyle.Bold;
            bold.Font.Result = FontId;
            bold.Size.Result = 8;

            CharFormats = new List<VisioAutomation.VDX.Sections.Char>();
            ParaFormats = new List<ParagraphFormat>();

            ParaFormats.Add(Left);
            CharFormats.Add(bold);
            CharFormats.Add(attribute);
            XForm.Height.Formula = "GUARD(TEXTHEIGHT(TheText,Width))";
            XForm.Width.Formula = "GUARD(TEXTWIDTH(TheText))";
            Text.Add(ShapeName, 0, 0, null);
        }

        //public string Name { get; set; }

        public string Description { get; set; }

        public abstract void GenerateDefault();
    }

    public class TriggerOld : BaseAction
    {
        public JProperty TriggerProperty { get; set; }

        public TriggerOld(JProperty trigger, double pinx, double piny, int fontId) : base(trigger.Name, pinx, piny, fontId)
        {
            this.TriggerProperty = trigger;
            Type = trigger.Value["type"].ToString();
            Schema = trigger.Value["inputs"]["schema"].ToString();
            Kind = trigger.Value["kind"].ToString();
            GenerateDefault();
        }

        public TriggerOld(string actionName, string type, string schema, double pinx, double piny, int fontId) : base(actionName, pinx, piny, fontId)
        {
            Type = type;
            Schema = schema;
            GenerateDefault();
        }

        public string Type { get; set; }
        public string Schema { get; set; } = string.Empty;
        public string Kind { get; set; }

        public override void GenerateDefault()
        {
            Text.Add("\nType: " + Type);
            Text.Add("\nKind: " + Kind);
            if (!string.IsNullOrEmpty(Schema) && CustomProps == null)
            {
                CustomProps = new CustomProps();
                CustomProps.Add(new CustomProp("Schema") { Value = Schema });
            }
            // if (!string.IsNullOrEmpty(Schema)) Text.Add("\nSchema: " + Schema);
        }
    }

    public class ActionOld : BaseAction
    {
        private List<ActionOld> childActions;

        public string Type { get; set; }
        public JToken ActionToken { get; set; }
        public JProperty ActionProperty { get { return (JProperty)ActionToken; } }

        public List<ActionOld> ChildActions
        {
            get
            {
                if (childActions == null)
                {
                    childActions = new List<ActionOld>();

                    if (ActionToken.Children().Any(t => t["actions"] != null))
                    {
                        foreach (var token in ActionToken.Children().Where(t => t["actions"] != null && t["actions"].HasValues).Select(t => t["actions"]).Children())
                        {
                            ChildActions.Add(new ActionOld(token, 0, 0, FontId));
                        }
                    }
                    else if (ActionToken.Children().Any(t => t["cases"] != null))
                    {
                        foreach (var token in ActionToken.Children().Where(t => t["cases"] != null && t["cases"].HasValues).Select(t => t["cases"]).Children())
                        {
                            ChildActions.Add(new ActionOld(token, 0, 0, FontId));
                        }
                    }
                }
                return childActions;
            }
            set => childActions = value;
        }

        public string RunAfter { get; set; }

        public ActionOld(JToken action, double pinx, double piny, int fontId) : base(((JProperty)action).Name, pinx, piny, fontId)
        {
            this.ActionToken = action;

            Type = ActionProperty.Value["type"] != null ? ActionProperty.Value["type"].ToString() : string.Empty;
            RunAfter = ActionProperty.Value["runAfter"] != null ? ActionProperty.Value["runAfter"].HasValues ? ((JProperty)ActionProperty.Value["runAfter"].First()).Name : string.Empty : string.Empty;
            // RunAfter = action.Value["runAfter"].ToString();
            GenerateDefault();
        }

        public override void GenerateDefault()
        {
            Text.Add("\nType: " + Type);
        }
    }
}