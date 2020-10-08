﻿using System;
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
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;

using XrmToolBox.Extensibility.Interfaces;

namespace LinkeD365.FlowToVisio
{
    public partial class FlowToVisioControl : PluginControlBase, IGitHubPlugin
    {
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
            if (!SettingsManager.Instance.TryLoad(GetType(), out flowConnection))
            {
                flowConnection = new FlowConnection();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
            ExecuteMethod(LoadFlows);
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
            SettingsManager.Instance.Save(GetType(), flowConnection);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (flowConnection != null && detail != null)
            {
              //  mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
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
            var selectFlow = ((FlowDefinition) grdFlows.SelectedRows[0].DataBoundItem);
            if (selectFlow.Solution)
            {
                flowObject = JObject.Parse(selectFlow.Definition);
                GenerateVisio();
            }
            else flowObject = LoadFlow(selectFlow);
           // if (((FlowDefinition) grdFlows.SelectedRows[0].DataBoundItem).Solution) flowObject = JObject.Parse(gridFlows.SelectedRowRecords.First()["clientdata"].ToString());
            // CreateVisio();
           // GenerateVisio();
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
                grdFlows.DataSource = flows.Where(flw => flw.Name.ToLower().Contains(textSearch.Text.ToLower()));//.Entities.Where(ent => ent.Attributes["name"].ToString().ToLower().Contains(textSearch.Text));
            }
            else
            {
                grdFlows.DataSource = flows;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //GetClient();
            LoadUnSolutionedFlows();
        }


        private HttpClient _client;

        private void btnConnectCDS_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadFlows);
        }
    }
}