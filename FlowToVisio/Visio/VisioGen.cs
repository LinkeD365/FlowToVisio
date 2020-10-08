using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase
    {


        private List<BaseShape> shapesList = new List<BaseShape>();
        private List<Connection> connections = new List<Connection>();
        private JObject flowObject;

        #region xmlVisio bits

        private Package package;

        private PackagePart document;

        private PackagePart pages;
        private PackagePart page;
        // private XDocument xmlPage;

        #endregion xmlVisio bits

        public void GenerateVisio()
        {
            CreateVisio();

            //CreateConnections();
            Utils.Root = flowObject;
            Connection.SetAPIs(flowObject);
            var triggerProperty = flowObject["properties"]["definition"]["triggers"].First() as JProperty;

            var triggerShape = Utils.AddAction(triggerProperty, null, 0, 1);

            if (flowObject["properties"]["definition"]["actions"].Children<JProperty>().Where(a => !a.Value["runAfter"].HasValues).Any())
            {
                Utils.AddActions(flowObject["properties"]["definition"]["actions"].Children<JProperty>().Where(a => !a.Value["runAfter"].HasValues), triggerShape);
            }

            // triggerShape.Default.Remove();
            //  triggerShape.Line.Remove();

            foreach (var shapeName in Utils.VisioTemplates)
            {
                triggerShape.GetTemplateShape(shapeName).Remove();
            }

            RecalcDocument(package);
            SaveXDocumentToPart(page, Utils.XMLPage);

            Utils.Ai.WriteEvent("Flow Actions", Utils.actionCount);

            MessageBox.Show("Visio generated with " + Utils.actionCount + " actions");
            Utils.actionCount = 0;

            package.Close();
            return;
        }

        public void CreateVisio()
        {
            // create a copy of the resource template
            package = null;

            File.WriteAllBytes(txtFileName.Text, Properties.Resources.VisioTemplate);
            // var template = Package.Open(new MemoryStream(Properties.Resources.VisioTemplate), FileMode.Open);
            //  template.

            #region get to the xml of the page

            package = Package.Open(txtFileName.Text, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            document = GetPackagePart(package, "http://schemas.microsoft.com/visio/2010/relationships/document");

            pages = GetPackagePart(package, document, "http://schemas.microsoft.com/visio/2010/relationships/pages");
            page = GetPackagePart(package, pages, "http://schemas.microsoft.com/visio/2010/relationships/page");

            Utils.XMLPage = GetXMLFromPart(page);

            #endregion get to the xml of the page

            return;
        }

        private static int CheckForRecalc(XDocument customPropsXDoc)
        {
            // Set the inital pidValue to -1, which is not an allowed value.
            // The calling code tests to see whether the pidValue is
            // greater than -1.
            int pidValue = -1;
            // Get all of the property elements from the document.
            IEnumerable<XElement> props = GetXElementsByName(
                customPropsXDoc, "property");
            // Get the RecalcDocument property from the document if it exists already.
            XElement recalcProp = GetXElementByAttribute(props,
                "name", "RecalcDocument");
            // If there is already a RecalcDocument instruction in the
            // Custom File Properties part, then we don't need to add another one.
            // Otherwise, we need to create a unique pid value.
            if (recalcProp != null)
            {
                return pidValue;
            }
            else
            {
                // Get all of the pid values of the property elements and then
                // convert the IEnumerable object into an array.
                IEnumerable<string> propIDs =
                    from prop in props
                    where prop.Name.LocalName == "property"
                    select prop.Attribute("pid").Value;
                string[] propIDArray = propIDs.ToArray();
                // Increment this id value until a unique value is found.
                // This starts at 2, because 0 and 1 are not valid pid values.
                int id = 2;
                while (pidValue == -1)
                {
                    if (propIDArray.Contains(id.ToString()))
                    {
                        id++;
                    }
                    else
                    {
                        pidValue = id;
                    }
                }
            }
            return pidValue;
        }

        private static void RecalcDocument(Package filePackage)
        {
            // Get the Custom File Properties part from the package and
            // and then extract the XML from it.
            PackagePart customPart = GetPackagePart(filePackage,
                "http://schemas.openxmlformats.org/officeDocument/2006/relationships/" +
                "custom-properties");
            XDocument customPartXML = GetXMLFromPart(customPart);
            // Check to see whether document recalculation has already been
            // set for this document. If it hasn't, use the integer
            // value returned by CheckForRecalc as the property ID.
            int pidValue = CheckForRecalc(customPartXML);
            if (pidValue > -1)
            {
                XElement customPartRoot = customPartXML.Elements().ElementAt(0);
                // Two XML namespaces are needed to add XML data to this
                // document. Here, we're using the GetNamespaceOfPrefix and
                // GetDefaultNamespace methods to get the namespaces that
                // we need. You can specify the exact strings for the
                // namespaces, but that is not recommended.
                XNamespace customVTypesNS = customPartRoot.GetNamespaceOfPrefix("vt");
                XNamespace customPropsSchemaNS = customPartRoot.GetDefaultNamespace();
                // Construct the XML for the new property in the XDocument.Add method.
                // This ensures that the XNamespace objects will resolve properly,
                // apply the correct prefix, and will not default to an empty namespace.
                customPartRoot.Add(
                    new XElement(customPropsSchemaNS + "property",
                        new XAttribute("pid", pidValue.ToString()),
                        new XAttribute("name", "RecalcDocument"),
                        new XAttribute("fmtid",
                            "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}"),
                        new XElement(customVTypesNS + "bool", "true")
                    ));
            }
            // Save the Custom Properties package part back to the package.
            SaveXDocumentToPart(customPart, customPartXML);
        }

        private static void SaveXDocumentToPart(PackagePart packagePart,
    XDocument partXML)
        {
            // Create a new XmlWriterSettings object to
            // define the characteristics for the XmlWriter
            XmlWriterSettings partWriterSettings = new XmlWriterSettings();
            partWriterSettings.Encoding = Encoding.UTF8;
            // Create a new XmlWriter and then write the XML
            // back to the document part.
            XmlWriter partWriter = XmlWriter.Create(packagePart.GetStream(),
                partWriterSettings);

            partXML.WriteTo(partWriter);
            // Flush and close the XmlWriter.
            partWriter.Flush();
            partWriter.Close();
        }

        private static PackagePart GetPackagePart(Package filePackage,
    string relationship)
        {
            // Use the namespace that describes the relationship
            // to get the relationship.
            PackageRelationship packageRel =
                filePackage.GetRelationshipsByType(relationship).FirstOrDefault();
            PackagePart part = null;
            // If the Visio file package contains this type of relationship with
            // one of its parts, return that part.
            if (packageRel != null)
            {
                // Clean up the URI using a helper class and then get the part.
                Uri docUri = PackUriHelper.ResolvePartUri(
                    new Uri("/", UriKind.Relative), packageRel.TargetUri);
                part = filePackage.GetPart(docUri);
            }
            return part;
        }

        private static PackagePart GetPackagePart(Package filePackage,
    PackagePart sourcePart, string relationship)
        {
            // This gets only the first PackagePart that shares the relationship
            // with the PackagePart passed in as an argument. You can modify the code
            // here to return a different PackageRelationship from the collection.
            PackageRelationship packageRel =
                sourcePart.GetRelationshipsByType(relationship).FirstOrDefault();
            PackagePart relatedPart = null;
            if (packageRel != null)
            {
                // Use the PackUriHelper class to determine the URI of PackagePart
                // that has the specified relationship to the PackagePart passed in
                // as an argument.
                Uri partUri = PackUriHelper.ResolvePartUri(
                    sourcePart.Uri, packageRel.TargetUri);
                relatedPart = filePackage.GetPart(partUri);
            }
            return relatedPart;
        }

        private static XDocument GetXMLFromPart(PackagePart packagePart)
        {
            XDocument partXml = null;
            // Open the packagePart as a stream and then
            // open the stream in an XDocument object.
            Stream partStream = packagePart.GetStream();
            partXml = XDocument.Load(partStream, LoadOptions.PreserveWhitespace);
            return partXml;
        }

        private static IEnumerable<XElement> GetXElementsByName(
    XDocument packagePart, string elementType)
        {
            // Construct a LINQ query that selects elements by their element type.
            IEnumerable<XElement> elements =
                from element in packagePart.Descendants()
                where element.Name.LocalName == elementType
                select element;
            // Return the selected elements to the calling code.
            return elements.DefaultIfEmpty(null);
        }

        private static XElement GetXElementByAttribute(IEnumerable<XElement> elements,
    string attributeName, string attributeValue)
        {
            // Construct a LINQ query that selects elements from a group
            // of elements by the value of a specific attribute.
            IEnumerable<XElement> selectedElements =
                from el in elements
                where el.Attribute(attributeName).Value == attributeValue
                select el;
            // If there aren't any elements of the specified type
            // with the specified attribute value in the document,
            // return null to the calling code.
            return selectedElements.DefaultIfEmpty(null).FirstOrDefault();
        }
    }
}