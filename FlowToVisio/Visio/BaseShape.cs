using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LinkeD365.FlowToVisio
{
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
            var selectedElements =
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
                    var selectedElements =
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
                    var elements =
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
            get { if (Property == null) return "Line." + Id; else return Property.Name; }
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
                string returnstring = Property.Name.Replace("__", "LiNkEd365").Replace("_", " ").Replace("LiNkEd365", "_").Replace("'", "&apos;");
                //var regext = new Regex("/(_{2,})|_/g, '$1'");
                //    string returnstring = regext.Replace(Property.Name, " ").Replace("'", "&apos;");
                return returnstring;
            }
        }

        private void SetId()
        {
            if (Shape.Attribute("ID") == null) return;
            Id = Shapes.Descendants().Where(el => el.Attribute("ID") != null).Max(x => int.Parse(x.Attribute("ID").Value)) + 1;

            Shape.SetAttributeValue("ID", Id);

            foreach (var stencil in Shape.Descendants().Where(el => el.Attribute("ID") != null))
                stencil.SetAttributeValue("ID",
                    Shapes.Descendants().Where(el => el.Attribute("ID") != null).Max(x => int.Parse(x.Attribute("ID").Value)) + 1);
            //if (Shape.Elements().Any(el => el.Name.LocalName == "Shapes"))
            //{
            //    foreach(var stencilShape in Shape.Elements().Where(el => el.Name.LocalName == Shapes).)
            //}
        }
    }
}