using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkeD365.FlowToVisio
{
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
}