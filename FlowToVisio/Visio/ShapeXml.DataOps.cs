using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkeD365.FlowToVisio
{
    public class ComposeAction : Action
    {
        public ComposeAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType();
            AddText("Value: " + Property.Value["inputs"].ToString());
        }
    }

    public class TableAction : Action
    {
        public TableAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType();

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"].ToString()).AppendLine();
            sb.AppendLine("Format: " + Property.Value["inputs"]["format"].ToString());

            if (Property.Value["inputs"]["columns"] != null && Property.Value["inputs"]["columns"].HasValues)
            {
                sb.AppendLine("Columns:");
                foreach (var props in Property.Value["inputs"]["columns"].First().Children<JProperty>())
                {
                    sb.AppendLine(props.Name + ": " + props.Value.ToString());
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

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"].ToString()).AppendLine();
            sb.AppendLine("Where: " + Property.Value["inputs"]["where"].ToString());

            AddText(sb.ToString());
        }
    }

    public class JoinAction : Action
    {
        public JoinAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType("Join");

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"].ToString()).AppendLine();
            sb.AppendLine("Join With: " + Property.Value["inputs"]["joinWith"].ToString());

            AddText(sb.ToString());
        }
    }

    public class ParseAction : Action
    {
        public ParseAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType("Parse JSON");

            var sb = new StringBuilder("Content: " + Property.Value["inputs"]["content"].ToString()).AppendLine();
            sb.AppendLine("Schema: " + Property.Value["inputs"]["schema"].ToString());

            AddText(sb.ToString());
        }
    }

    public class SelectAction : Action
    {
        public SelectAction(JProperty property, Action parent, int current, int children) : base(property, parent, current, children, "Compose")
        {
            AddName();
            AddType("Select");

            var sb = new StringBuilder("From: " + Property.Value["inputs"]["from"].ToString()).AppendLine();

            if (Property.Value["inputs"]["select"] != null && Property.Value["inputs"]["select"].HasValues)
            {
                sb.AppendLine("Map:");
                foreach (var props in Property.Value["inputs"]["select"].Children<JProperty>())
                {
                    sb.AppendLine(props.Name + ": " + props.Value.ToString());
                }
            }

            AddText(sb.ToString());
        }
    }
}