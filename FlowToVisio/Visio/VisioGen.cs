using LinkeD365.FlowToVisio.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using XrmToolBox.Extensibility;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase
    {
        //private JObject flowObject;

        #region xmlVisio bits

        private Package package;

        private PackagePart document;

        private PackagePart pages;
        private PackagePart page;
        // private XDocument xmlPage;

        #endregion xmlVisio bits

        public void GenerateVisio(string fileName, FlowDefinition flow, int flowCount, bool logicApp = false)
        {
            CreateVisio(fileName);
            JObject flowObject = JObject.Parse(flow.Definition);
            //CreateConnections();
            Utils.Root = flowObject;
            Connection.SetAPIs(flowObject);
            var triggerProperty = flowObject["properties"]["definition"]["triggers"].First() as JProperty;

            var triggerShape = Utils.AddAction(triggerProperty, null, 0, 1);
            Utils.AddComment(triggerShape);
            if (flowObject["properties"]["definition"]["actions"].Children<JProperty>().Where(a => !a.Value["runAfter"].HasValues).Any())
                Utils.AddActions(flowObject["properties"]["definition"]["actions"].Children<JProperty>().Where(a => !a.Value["runAfter"].HasValues), triggerShape);

            foreach (var shapeName in Utils.VisioTemplates)
                triggerShape.GetTemplateShape(shapeName).Remove();

            //SaveXDocumentToPart(page, Utils.XMLPage);
            CreateNewPage(package, pages, Utils.XMLPage, //new Uri( Uri.EscapeUriString($"/visio/pages/{flow.Name.Replace(' ','_')}.xml"),UriKind.Relative),
                new Uri(Uri.EscapeUriString($"/visio/pages/flowPage{flowCount}.xml"), UriKind.Relative), page.ContentType, "http://schemas.microsoft.com/visio/2010/relationships/page", flow.Name);
            Utils.Ai.WriteEvent(logicApp ? "Logic App Actions" : "Flow Actions", Utils.actionCount);
            Utils.totalVisio += 1;
            Utils.totalActions += Utils.actionCount;
            Utils.actionCount = 0;

            return;
        }

        public void CompleteVisio(string fileName)
        {
            RemoveTemplate();
            RecalcDocument(package);
            package.Close();
            if (MessageBox.Show($@"{Utils.totalVisio} Visio{(Utils.totalVisio > 1 ? "s" : "")} generated with {Utils.totalActions} actions.{Environment.NewLine}Do you want to open the file?", "Visio Created Succesfully",
                MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                Process.Start(fileName);
            }
            package = null;
            Utils.totalActions = 0;
            Utils.totalVisio = 0;
        }

        public void CreateVisio(string fileName)
        {
            // create a copy of the resource template
            if (package == null)
            {
                File.WriteAllBytes(fileName, Resources.VisioTemplate);
                // var template = Package.Open(new MemoryStream(Properties.Resources.VisioTemplate),
                // FileMode.Open); template.

                #region get to the xml of the page

                package = Package.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            document = GetPackagePart(package, "http://schemas.microsoft.com/visio/2010/relationships/document");

            pages = GetPackagePart(package, document, "http://schemas.microsoft.com/visio/2010/relationships/pages");
            page = GetPackagePart(package, pages, "http://schemas.microsoft.com/visio/2010/relationships/page");
            ;
            Utils.XMLPage = GetXMLFromPart(page);

            #endregion get to the xml of the page

            return;
        }
    }
}