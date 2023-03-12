using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

namespace LinkeD365.FlowToVisio
{
    public class ComposeAction : Action
    {
        public ComposeAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType();
            AddText("Value: " + Property.Value["inputs"]);
        }
    }

    public class TableAction : Action
    {
        public TableAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType();

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"]).AppendLine();
            sb.AppendLine("Format: " + Property.Value["inputs"]["format"]);

            if (Property.Value["inputs"]["columns"] != null && Property.Value["inputs"]["columns"].HasValues)
            {
                sb.AppendLine("Columns:");
                foreach (var props in Property.Value["inputs"]["columns"].First().Children<JProperty>())
                {
                    sb.AppendLine(props.Name + ": " + props.Value);
                }
            }

            AddText(sb.ToString());
        }
    }

    public class FilterAction : Action
    {
        public FilterAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType("Filter Array");

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"]).AppendLine();
            sb.AppendLine("Where: " + Property.Value["inputs"]["where"]);

            AddText(sb.ToString());
        }
    }

    public class JoinAction : Action
    {
        public JoinAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType("Join");

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"]).AppendLine();
            sb.AppendLine("Join With: " + Property.Value["inputs"]["joinWith"]);

            AddText(sb.ToString());
        }
    }

    public class ParseAction : Action
    {
        public ParseAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType("Parse JSON");

            var sb = new StringBuilder("Content: " + Property.Value["inputs"]["content"]).AppendLine();
            sb.AppendLine("Schema: " + Property.Value["inputs"]["schema"]);

            AddText(sb.ToString());
        }
    }

    public class SelectAction : Action
    {
        public SelectAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType("Select");

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"]).AppendLine();

            if (Property.Value["inputs"]["select"] != null && Property.Value["inputs"]["select"].HasValues)
            {
                sb.AppendLine("Map:");
                foreach (var props in Property.Value["inputs"]["select"].Children<JProperty>())
                {
                    sb.AppendLine(props.Name + ": " + props.Value);
                }
            }

            AddText(sb.ToString());
        }
    }
}