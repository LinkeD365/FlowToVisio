using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LinkeD365.FlowToVisio
{
    public class TemplateAction : Action
    {
        public TemplateAction(JProperty template, JProperty property, Action parent, int current, int children, string templateName) : base(property, parent, current, children, templateName)
        {
            AddName();
            AddType(template.Name);
            var sb = new StringBuilder();
            if (template.Value["display"] != null)
            {
                foreach (var display in template.Value["display"].Children<JProperty>())
                {
                    var splitList = display.Value["value"].ToString().Split('|').ToList();
                    string value = GetPropValue(property, splitList, display.Value["options"]);
                    if (value != string.Empty) sb.AppendLine(display.Name + " : " + value);
                }
            }

            if (template.Value["repeat"] != null)
            {
                foreach (var display in template.Value["repeat"].Children<JObject>())
                {
                    var repeat = CreateRepeat(property, display["path"].ToString().Split('|').ToList(),
                        display["filter"]?.ToString().Split('|').ToList() ?? new List<string>(), int.Parse(display["left"]?.ToString() ?? "0"), display["class"] != null && bool.Parse(display["class"].ToString()));
                    if (repeat != String.Empty)
                    {
                        if (display["title"] != null) sb.AppendLine(display["title"] + " :");
                        sb.AppendLine(repeat);
                    }
                }

            }

            AddText(sb);
        }

        private string CreateRepeat(JToken property, List<string> splitList, List<string> filter, int left, bool isClass)
        {
            string name = splitList[0];
            if (property.Type == JTokenType.Object)
            {

                var childObject = ((JObject)property).Children<JProperty>().FirstOrDefault(prop => prop.Name == name);
                if (childObject == null) return string.Empty;
                if (splitList.Count == 1)
                {
                    var sb = new StringBuilder();
                    if (isClass)
                    {
                        foreach (var classProp in childObject.Value.Children<JProperty>())
                        {
                            sb.AppendLine(classProp.Name + CreateClassString(classProp, filter, left));
                        }
                    }
                    else
                    {
                        foreach (var valueProp in childObject.Value.Children<JProperty>().Where(vo => !filter.Contains(vo.Name)))
                        {
                            sb.AppendLine(valueProp.Name.Substring(left, valueProp.Name.Length - left) + " : " + valueProp.Value);
                        }
                    }

                    return sb.ToString();

                }
                return CreateRepeat(childObject, splitList.GetRange(1, splitList.Count - 1), filter, left, isClass);
                //if (((JProperty)property).Value[name] == null) return string.Empty;
            }
            else if (property.Type == JTokenType.Property)
            {
                if (((JProperty)property).Value[name] == null) return string.Empty;
                if (splitList.Count == 1)
                {
                    var sb = new StringBuilder();
                    if (((JProperty)property).Value[name] is JObject)
                    {

                        var valueObject = ((JProperty)property).Value[name] as JObject;
                        foreach (var valueProp in valueObject.Children<JProperty>().Where(vo => !filter.Contains(vo.Name)))
                        {
                            sb.AppendLine(valueProp.Name.Substring(left, valueProp.Name.Length - left) + " : " + valueProp.Value);
                        }

                        return sb.ToString();
                    }

                    if (((JProperty)property).Value[name] is JArray)
                    {
                        foreach (var tokenValue in ((JProperty)property).Value[name] as JArray)
                        {
                            if (tokenValue is JObject)
                            {
                                var objectValue = tokenValue as JObject;
                                foreach (var childValue in objectValue.Children<JProperty>())
                                {
                                    sb.AppendLine(childValue.Name.Substring(left, childValue.Name.Length - left) + " : " + childValue.Value);
                                }
                            }
                            else sb.AppendLine(tokenValue.ToString());
                        }

                        return sb.ToString();
                    }

                    return string.Empty;
                }

                return CreateRepeat(((JProperty)property).Value[name], splitList.GetRange(1, splitList.Count - 1), filter, left, isClass);
            }

            return string.Empty;
        }

        private string CreateClassString(JProperty property, List<string> filter, int left)
        {
            var sb = new StringBuilder(property.Name + " : ").AppendLine();

            foreach (var propValue in property.Children().Values<JProperty>().Where(vo => !filter.Contains(vo.Name)))
            {
                sb.AppendLine(propValue.Name.Substring(left, propValue.Name.Length - left) + " : " + propValue.Value);
            }

            // property.Values<JProperty>().Where(vo => !filter.Contains(vo.Name))
            return sb.ToString();
        }

        private string GetPropValue(JToken property, List<string> splitList, JToken options)
        {
            string name = splitList[0];
            if (property.Type == JTokenType.Object)
            {
                var childObject = ((JObject)property).Children<JProperty>().FirstOrDefault(prop => prop.Name == name);
                if (childObject == null) return string.Empty;
                if (splitList.Count == 1) return GetOptionValue(childObject.Value.ToString(), options);

                return GetPropValue(childObject, splitList.GetRange(1, splitList.Count - 1), options);
            }
            else if (property.Type == JTokenType.Property)
            {
                if (((JProperty)property).Value[name] == null) return string.Empty;
                if (splitList.Count == 1) return GetOptionValue(((JProperty)property).Value[name].ToString(), options);
                return GetPropValue(((JProperty)property).Value[name], splitList.GetRange(1, splitList.Count - 1), options);
            }

            return string.Empty;
            //   if (property.Value[name] == null) return string.Empty;
            // if (splitList.Count == 0) return property.Value[name].ToString();

            //return GetPropValue((JProperty)property.Value[name], splitList.GetRange(1, splitList.Count));
        }

        private string GetOptionValue(string value, JToken options)
        {
            if (options == null) return value;

            var option = options.Children<JProperty>().FirstOrDefault(opt => opt.Name == value);
            if (option == null) return value;
            return option.Value.ToString();
        }
    }
}
