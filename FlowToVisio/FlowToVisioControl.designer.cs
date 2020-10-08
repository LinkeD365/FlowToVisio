namespace LinkeD365.FlowToVisio
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
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCreateVisio = new System.Windows.Forms.ToolStripButton();
            this.btnConnectFlow = new System.Windows.Forms.ToolStripButton();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitTop = new System.Windows.Forms.SplitContainer();
            this.splitSearch = new System.Windows.Forms.SplitContainer();
            this.lblSearch = new System.Windows.Forms.Label();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.splitFile = new System.Windows.Forms.SplitContainer();
            this.btnFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.grdFlows = new System.Windows.Forms.DataGridView();
            this.saveDialog = new System.Windows.Forms.SaveFileDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnConnectCDS = new System.Windows.Forms.ToolStripButton();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitFile)).BeginInit();
            this.splitFile.Panel1.SuspendLayout();
            this.splitFile.Panel2.SuspendLayout();
            this.splitFile.SuspendLayout();
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
            this.btnConnectCDS});
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
            this.tsbClose.Size = new System.Drawing.Size(86, 28);
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
            this.btnCreateVisio.Size = new System.Drawing.Size(97, 28);
            this.btnCreateVisio.Text = "Create Visio";
            this.btnCreateVisio.Click += new System.EventHandler(this.btnCreateVisio_Click);
            // 
            // btnConnectFlow
            // 
            this.btnConnectFlow.Image = global::LinkeD365.FlowToVisio.Properties.Resources.powerautomate;
            this.btnConnectFlow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnectFlow.Name = "btnConnectFlow";
            this.btnConnectFlow.Size = new System.Drawing.Size(143, 28);
            this.btnConnectFlow.Text = "Connect to Flow API";
            this.btnConnectFlow.Click += new System.EventHandler(this.toolStripButton1_Click);
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
            this.splitTop.Panel2.Controls.Add(this.splitFile);
            this.splitTop.Size = new System.Drawing.Size(908, 25);
            this.splitTop.SplitterDistance = 540;
            this.splitTop.TabIndex = 6;
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
            this.splitSearch.Size = new System.Drawing.Size(540, 25);
            this.splitSearch.SplitterDistance = 40;
            this.splitSearch.TabIndex = 6;
            // 
            // lblSearch
            // 
            this.lblSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(3, 2);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(41, 13);
            this.lblSearch.TabIndex = 5;
            this.lblSearch.Text = "Search";
            // 
            // textSearch
            // 
            this.textSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textSearch.Location = new System.Drawing.Point(0, 0);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(496, 20);
            this.textSearch.TabIndex = 4;
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            // 
            // splitFile
            // 
            this.splitFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitFile.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitFile.Location = new System.Drawing.Point(0, 0);
            this.splitFile.Name = "splitFile";
            // 
            // splitFile.Panel1
            // 
            this.splitFile.Panel1.Controls.Add(this.btnFile);
            // 
            // splitFile.Panel2
            // 
            this.splitFile.Panel2.Controls.Add(this.txtFileName);
            this.splitFile.Size = new System.Drawing.Size(364, 25);
            this.splitFile.TabIndex = 4;
            // 
            // btnFile
            // 
            this.btnFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnFile.Location = new System.Drawing.Point(0, 0);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(50, 20);
            this.btnFile.TabIndex = 3;
            this.btnFile.Text = "File";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFileName.Enabled = false;
            this.txtFileName.Location = new System.Drawing.Point(0, 0);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(310, 20);
            this.txtFileName.TabIndex = 2;
            // 
            // grdFlows
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.grdFlows.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grdFlows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdFlows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdFlows.Location = new System.Drawing.Point(0, 0);
            this.grdFlows.Name = "grdFlows";
            this.grdFlows.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdFlows.Size = new System.Drawing.Size(908, 694);
            this.grdFlows.TabIndex = 6;
            // 
            // saveDialog
            // 
            this.saveDialog.DefaultExt = "vsdx";
            this.saveDialog.Filter = "Visio Files(*.vsdx)|*.vsdx";
            // 
            // btnConnectCDS
            // 
            this.btnConnectCDS.Image = global::LinkeD365.FlowToVisio.Properties.Resources.CDSbtton;
            this.btnConnectCDS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnectCDS.Name = "btnConnectCDS";
            this.btnConnectCDS.Size = new System.Drawing.Size(119, 28);
            this.btnConnectCDS.Text = "Connect to CDS";
            this.btnConnectCDS.Click += new System.EventHandler(this.btnConnectCDS_Click);
            // 
            // FlowToVisioControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "FlowToVisioControl";
            this.Size = new System.Drawing.Size(908, 754);
            this.OnCloseTool += new System.EventHandler(this.FlowToVisioControl_OnClose);
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
            this.splitFile.Panel1.ResumeLayout(false);
            this.splitFile.Panel2.ResumeLayout(false);
            this.splitFile.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitFile)).EndInit();
            this.splitFile.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.SaveFileDialog saveDialog;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.SplitContainer splitTop;
        private System.Windows.Forms.SplitContainer splitSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox textSearch;
        private System.Windows.Forms.SplitContainer splitFile;
        private System.Windows.Forms.ToolStripButton btnConnectFlow;
        private System.Windows.Forms.DataGridView grdFlows;
        private System.Windows.Forms.ToolStripButton btnConnectCDS;
    }
}
