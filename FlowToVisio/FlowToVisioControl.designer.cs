﻿namespace LinkeD365.FlowToVisio
{
    partial class FlowToVisioControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowToVisioControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCreateVisio = new System.Windows.Forms.ToolStripButton();
            this.btnConnectFlow = new System.Windows.Forms.ToolStripButton();
            this.btnConnectCDS = new System.Windows.Forms.ToolStripButton();
            this.btnConnectLogicApps = new System.Windows.Forms.ToolStripButton();
            this.dropOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.chkShowCustomTracking = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowSecure = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowConCurrency = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowTriggerConditions = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowTrackedProps = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowComments = new System.Windows.Forms.ToolStripMenuItem();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitTop = new System.Windows.Forms.SplitContainer();
            this.splitSearch = new System.Windows.Forms.SplitContainer();
            this.lblSearch = new System.Windows.Forms.Label();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.splitSolution = new System.Windows.Forms.SplitContainer();
            this.lblSolution = new System.Windows.Forms.Label();
            this.ddlSolutions = new System.Windows.Forms.ComboBox();
            this.grdFlows = new System.Windows.Forms.DataGridView();
            this.saveDialog = new System.Windows.Forms.SaveFileDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.toolStripMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTop)).BeginInit();
            this.splitTop.Panel1.SuspendLayout();
            this.splitTop.Panel2.SuspendLayout();
            this.splitTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitSearch)).BeginInit();
            this.splitSearch.Panel1.SuspendLayout();
            this.splitSearch.Panel2.SuspendLayout();
            this.splitSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitSolution)).BeginInit();
            this.splitSolution.Panel1.SuspendLayout();
            this.splitSolution.Panel2.SuspendLayout();
            this.splitSolution.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdFlows)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.btnCreateVisio,
            this.btnConnectFlow,
            this.btnConnectCDS,
            this.btnConnectLogicApps,
            this.dropOptions});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(908, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(107, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // btnCreateVisio
            // 
            this.btnCreateVisio.Image = ((System.Drawing.Image)(resources.GetObject("btnCreateVisio.Image")));
            this.btnCreateVisio.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreateVisio.Name = "btnCreateVisio";
            this.btnCreateVisio.Size = new System.Drawing.Size(116, 28);
            this.btnCreateVisio.Text = "Create Visio";
            this.btnCreateVisio.Click += new System.EventHandler(this.btnCreateVisio_Click);
            // 
            // btnConnectFlow
            // 
            this.btnConnectFlow.Image = global::LinkeD365.FlowToVisio.Properties.Resources.powerautomate;
            this.btnConnectFlow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnectFlow.Name = "btnConnectFlow";
            this.btnConnectFlow.Size = new System.Drawing.Size(170, 28);
            this.btnConnectFlow.Text = "Connect to Flow API";
            this.btnConnectFlow.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // btnConnectCDS
            // 
            this.btnConnectCDS.Image = global::LinkeD365.FlowToVisio.Properties.Resources.Dataverse_32x32;
            this.btnConnectCDS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnectCDS.Name = "btnConnectCDS";
            this.btnConnectCDS.Size = new System.Drawing.Size(179, 28);
            this.btnConnectCDS.Text = "Connect to Dataverse";
            this.btnConnectCDS.Click += new System.EventHandler(this.btnConnectCDS_Click);
            // 
            // btnConnectLogicApps
            // 
            this.btnConnectLogicApps.Image = ((System.Drawing.Image)(resources.GetObject("btnConnectLogicApps.Image")));
            this.btnConnectLogicApps.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnectLogicApps.Name = "btnConnectLogicApps";
            this.btnConnectLogicApps.Size = new System.Drawing.Size(187, 28);
            this.btnConnectLogicApps.Text = "Connect to Logic Apps";
            this.btnConnectLogicApps.Click += new System.EventHandler(this.btnConnectLogicApps_Click);
            // 
            // dropOptions
            // 
            this.dropOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkShowCustomTracking,
            this.chkShowSecure,
            this.chkShowConCurrency,
            this.chkShowTriggerConditions,
            this.chkShowTrackedProps,
            this.chkShowComments});
            this.dropOptions.Image = global::LinkeD365.FlowToVisio.Properties.Resources.Options;
            this.dropOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dropOptions.Name = "dropOptions";
            this.dropOptions.Size = new System.Drawing.Size(99, 28);
            this.dropOptions.Text = "Options";
            // 
            // chkShowCustomTracking
            // 
            this.chkShowCustomTracking.CheckOnClick = true;
            this.chkShowCustomTracking.Name = "chkShowCustomTracking";
            this.chkShowCustomTracking.Size = new System.Drawing.Size(254, 26);
            this.chkShowCustomTracking.Text = "Show Custom Tracking";
            this.chkShowCustomTracking.CheckedChanged += new System.EventHandler(this.chkShowCustomTracking_CheckedChanged);
            // 
            // chkShowSecure
            // 
            this.chkShowSecure.CheckOnClick = true;
            this.chkShowSecure.Name = "chkShowSecure";
            this.chkShowSecure.Size = new System.Drawing.Size(254, 26);
            this.chkShowSecure.Text = "Show Secure Params";
            this.chkShowSecure.CheckedChanged += new System.EventHandler(this.chkShowSecure_CheckedChanged);
            // 
            // chkShowConCurrency
            // 
            this.chkShowConCurrency.CheckOnClick = true;
            this.chkShowConCurrency.Name = "chkShowConCurrency";
            this.chkShowConCurrency.Size = new System.Drawing.Size(254, 26);
            this.chkShowConCurrency.Text = "Show Concurrency";
            this.chkShowConCurrency.CheckedChanged += new System.EventHandler(this.chkShowConCurrency_CheckedChanged);
            this.chkShowConCurrency.Click += new System.EventHandler(this.chkShowConCurrency_Click);
            // 
            // chkShowTriggerConditions
            // 
            this.chkShowTriggerConditions.CheckOnClick = true;
            this.chkShowTriggerConditions.Name = "chkShowTriggerConditions";
            this.chkShowTriggerConditions.Size = new System.Drawing.Size(254, 26);
            this.chkShowTriggerConditions.Text = "Show Trigger Conditions";
            this.chkShowTriggerConditions.CheckedChanged += new System.EventHandler(this.chkShowTriggerConditions_CheckedChanged);
            // 
            // chkShowTrackedProps
            // 
            this.chkShowTrackedProps.CheckOnClick = true;
            this.chkShowTrackedProps.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.chkShowTrackedProps.Name = "chkShowTrackedProps";
            this.chkShowTrackedProps.Size = new System.Drawing.Size(254, 26);
            this.chkShowTrackedProps.Text = "Show Tracked Properties";
            this.chkShowTrackedProps.CheckedChanged += new System.EventHandler(this.chkShowTrackedProps_CheckedChanged);
            // 
            // chkShowComments
            // 
            this.chkShowComments.CheckOnClick = true;
            this.chkShowComments.Name = "chkShowComments";
            this.chkShowComments.Size = new System.Drawing.Size(254, 26);
            this.chkShowComments.Text = "Show Comments";
            this.chkShowComments.CheckedChanged += new System.EventHandler(this.chkShowComments_CheckedChanged);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(0, 31);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.splitTop);
            this.splitMain.Panel1MinSize = 20;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.grdFlows);
            this.splitMain.Size = new System.Drawing.Size(908, 723);
            this.splitMain.SplitterDistance = 25;
            this.splitMain.TabIndex = 6;
            // 
            // splitTop
            // 
            this.splitTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTop.Location = new System.Drawing.Point(0, 0);
            this.splitTop.Name = "splitTop";
            // 
            // splitTop.Panel1
            // 
            this.splitTop.Panel1.Controls.Add(this.splitSearch);
            // 
            // splitTop.Panel2
            // 
            this.splitTop.Panel2.Controls.Add(this.splitSolution);
            this.splitTop.Size = new System.Drawing.Size(908, 25);
            this.splitTop.SplitterDistance = 478;
            this.splitTop.TabIndex = 8;
            // 
            // splitSearch
            // 
            this.splitSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitSearch.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitSearch.Location = new System.Drawing.Point(0, 0);
            this.splitSearch.Name = "splitSearch";
            // 
            // splitSearch.Panel1
            // 
            this.splitSearch.Panel1.Controls.Add(this.lblSearch);
            // 
            // splitSearch.Panel2
            // 
            this.splitSearch.Panel2.Controls.Add(this.textSearch);
            this.splitSearch.Size = new System.Drawing.Size(478, 25);
            this.splitSearch.SplitterDistance = 44;
            this.splitSearch.TabIndex = 6;
            // 
            // lblSearch
            // 
            this.lblSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(3, 2);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(46, 15);
            this.lblSearch.TabIndex = 5;
            this.lblSearch.Text = "Search";
            // 
            // textSearch
            // 
            this.textSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSearch.Location = new System.Drawing.Point(0, 0);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(430, 20);
            this.textSearch.TabIndex = 4;
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            // 
            // splitSolution
            // 
            this.splitSolution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitSolution.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitSolution.Location = new System.Drawing.Point(0, 0);
            this.splitSolution.Name = "splitSolution";
            // 
            // splitSolution.Panel1
            // 
            this.splitSolution.Panel1.Controls.Add(this.lblSolution);
            // 
            // splitSolution.Panel2
            // 
            this.splitSolution.Panel2.Controls.Add(this.ddlSolutions);
            this.splitSolution.Size = new System.Drawing.Size(426, 25);
            this.splitSolution.SplitterDistance = 51;
            this.splitSolution.TabIndex = 7;
            // 
            // lblSolution
            // 
            this.lblSolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSolution.AutoSize = true;
            this.lblSolution.Location = new System.Drawing.Point(0, 6);
            this.lblSolution.Name = "lblSolution";
            this.lblSolution.Size = new System.Drawing.Size(52, 15);
            this.lblSolution.TabIndex = 5;
            this.lblSolution.Text = "Solution";
            // 
            // ddlSolutions
            // 
            this.ddlSolutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ddlSolutions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlSolutions.FormattingEnabled = true;
            this.ddlSolutions.Location = new System.Drawing.Point(0, 0);
            this.ddlSolutions.Name = "ddlSolutions";
            this.ddlSolutions.Size = new System.Drawing.Size(371, 21);
            this.ddlSolutions.TabIndex = 0;
            this.ddlSolutions.SelectedIndexChanged += new System.EventHandler(this.ddlSolutions_SelectedIndexChanged);
            // 
            // grdFlows
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.grdFlows.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdFlows.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.grdFlows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdFlows.DefaultCellStyle = dataGridViewCellStyle3;
            this.grdFlows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdFlows.Location = new System.Drawing.Point(0, 0);
            this.grdFlows.Name = "grdFlows";
            this.grdFlows.ReadOnly = true;
            this.grdFlows.RowHeadersWidth = 51;
            this.grdFlows.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdFlows.Size = new System.Drawing.Size(908, 694);
            this.grdFlows.TabIndex = 6;
            this.grdFlows.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdFlows_CellClick);
            this.grdFlows.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdFlows_CellMouseEnter);
            this.grdFlows.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdFlows_ColumnHeaderMouseClick);
            // 
            // saveDialog
            // 
            this.saveDialog.DefaultExt = "vsdx";
            this.saveDialog.Filter = "Visio Files(*.vsdx)|*.vsdx";
            // 
            // FlowToVisioControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "FlowToVisioControl";
            this.PluginIcon = ((System.Drawing.Icon)(resources.GetObject("$this.PluginIcon")));
            this.Size = new System.Drawing.Size(908, 754);
            this.TabIcon = ((System.Drawing.Image)(resources.GetObject("$this.TabIcon")));
            this.Load += new System.EventHandler(this.FlowToVisioControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitTop.Panel1.ResumeLayout(false);
            this.splitTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTop)).EndInit();
            this.splitTop.ResumeLayout(false);
            this.splitSearch.Panel1.ResumeLayout(false);
            this.splitSearch.Panel1.PerformLayout();
            this.splitSearch.Panel2.ResumeLayout(false);
            this.splitSearch.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitSearch)).EndInit();
            this.splitSearch.ResumeLayout(false);
            this.splitSolution.Panel1.ResumeLayout(false);
            this.splitSolution.Panel1.PerformLayout();
            this.splitSolution.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitSolution)).EndInit();
            this.splitSolution.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdFlows)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.ToolStripButton btnCreateVisio;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SaveFileDialog saveDialog;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.SplitContainer splitSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox textSearch;
        private System.Windows.Forms.ToolStripButton btnConnectFlow;
        private System.Windows.Forms.DataGridView grdFlows;
        private System.Windows.Forms.ToolStripButton btnConnectCDS;
        private System.Windows.Forms.ToolStripButton btnConnectLogicApps;
        private System.Windows.Forms.ToolStripDropDownButton dropOptions;
        private System.Windows.Forms.ToolStripMenuItem chkShowConCurrency;
        private System.Windows.Forms.ToolStripMenuItem chkShowSecure;
        private System.Windows.Forms.ToolStripMenuItem chkShowCustomTracking;
        private System.Windows.Forms.ToolStripMenuItem chkShowTriggerConditions;
        private System.Windows.Forms.ToolStripMenuItem chkShowTrackedProps;
        private System.Windows.Forms.ToolStripMenuItem chkShowComments;
        private System.Windows.Forms.SplitContainer splitTop;
        private System.Windows.Forms.SplitContainer splitSolution;
        private System.Windows.Forms.Label lblSolution;
        private System.Windows.Forms.ComboBox ddlSolutions;
    }
}
