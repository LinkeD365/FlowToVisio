using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.IO;
using Newtonsoft.Json.Linq;
using XrmToolBox.Extensibility.Interfaces;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase, IGitHubPlugin
    {
        private Settings mySettings;
        private bool overrideSave = false;

        public string RepositoryName => "ERD Visio Builder";
        public string UserName => "LinkeD365";

        public FlowToVisioControl()
        {
            InitializeComponent();
        }

        private void FlowToVisioControl_Load(object sender, EventArgs e)
        {
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlowToVisioControl_OnClose(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            LoadFlows();
        }

        private void btnCreateVisio_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                MessageBox.Show("Please select a file name prior to generating a Visio", "Select File", MessageBoxButtons.OK);
                return;
            }

            if (File.Exists(txtFileName.Text) && !overrideSave)
            {
                if (MessageBox.Show("Do you want to override the file?", "File already exists", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }
            overrideSave = false;

            flowObject = JObject.Parse(gridFlows.SelectedRowRecords.First()["clientdata"].ToString());
            // CreateVisio();
            GenerateVisio();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            if (saveDialog.ShowDialog() == DialogResult.OK) txtFileName.Text = saveDialog.FileName;
            overrideSave = true;
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            //gridFlows.DataSource = null;
            if (string.IsNullOrEmpty(textSearch.Text))
            {
                gridFlows.DataSource = flowRecords.Entities.Where(ent => ent.Attributes["name"].ToString().ToLower().Contains(textSearch.Text));
            }
            else
            {
                gridFlows.DataSource = flowRecords;
            }
        }
    }
}