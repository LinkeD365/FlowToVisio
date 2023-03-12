namespace LinkeD365.FlowToVisio
{
    partial class FlowRunForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlowRunForm));
            this.dgvFlowRuns = new System.Windows.Forms.DataGridView();
            this.tscMain = new System.Windows.Forms.ToolStripContainer();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.tools = new System.Windows.Forms.ToolStrip();
            this.btnStopFlows = new System.Windows.Forms.ToolStripButton();
            this.ddlFilter = new System.Windows.Forms.ToolStripComboBox();
            this.lblCancel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFlowRuns)).BeginInit();
            this.tscMain.ContentPanel.SuspendLayout();
            this.tscMain.TopToolStripPanel.SuspendLayout();
            this.tscMain.SuspendLayout();
            this.tools.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvFlowRuns
            // 
            this.dgvFlowRuns.AllowUserToAddRows = false;
            this.dgvFlowRuns.AllowUserToDeleteRows = false;
            this.dgvFlowRuns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFlowRuns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFlowRuns.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgvFlowRuns.Location = new System.Drawing.Point(0, 0);
            this.dgvFlowRuns.MultiSelect = false;
            this.dgvFlowRuns.Name = "dgvFlowRuns";
            this.dgvFlowRuns.RowHeadersVisible = false;
            this.dgvFlowRuns.Size = new System.Drawing.Size(545, 425);
            this.dgvFlowRuns.TabIndex = 0;
            this.dgvFlowRuns.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFlowRuns_CellValueChanged);
            this.dgvFlowRuns.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFlowRuns_ColumnHeaderMouseClick);
            this.dgvFlowRuns.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvFlowRuns_CurrentCellDirtyStateChanged);
            // 
            // tscMain
            // 
            this.tscMain.BottomToolStripPanelVisible = false;
            // 
            // tscMain.ContentPanel
            // 
            this.tscMain.ContentPanel.Controls.Add(this.lblCancel);
            this.tscMain.ContentPanel.Controls.Add(this.chkAll);
            this.tscMain.ContentPanel.Controls.Add(this.dgvFlowRuns);
            this.tscMain.ContentPanel.Size = new System.Drawing.Size(545, 425);
            this.tscMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tscMain.LeftToolStripPanelVisible = false;
            this.tscMain.Location = new System.Drawing.Point(0, 0);
            this.tscMain.Name = "tscMain";
            this.tscMain.RightToolStripPanelVisible = false;
            this.tscMain.Size = new System.Drawing.Size(545, 450);
            this.tscMain.TabIndex = 2;
            this.tscMain.Text = "toolStripContainer1";
            // 
            // tscMain.TopToolStripPanel
            // 
            this.tscMain.TopToolStripPanel.Controls.Add(this.tools);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(3, 3);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(15, 14);
            this.chkAll.TabIndex = 1;
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // tools
            // 
            this.tools.Dock = System.Windows.Forms.DockStyle.None;
            this.tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStopFlows,
            this.ddlFilter});
            this.tools.Location = new System.Drawing.Point(3, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(278, 25);
            this.tools.TabIndex = 0;
            // 
            // btnStopFlows
            // 
            this.btnStopFlows.Image = global::LinkeD365.FlowToVisio.Properties.Resources.Cancel_16;
            this.btnStopFlows.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStopFlows.Name = "btnStopFlows";
            this.btnStopFlows.Size = new System.Drawing.Size(143, 22);
            this.btnStopFlows.Text = "Cancel Selected Flows";
            this.btnStopFlows.Click += new System.EventHandler(this.btnStopFlows_Click);
            // 
            // ddlFilter
            // 
            this.ddlFilter.Name = "ddlFilter";
            this.ddlFilter.Size = new System.Drawing.Size(121, 25);
            this.ddlFilter.SelectedIndexChanged += new System.EventHandler(this.ddlFilter_SelectedIndexChanged);
            // 
            // lblCancel
            // 
            this.lblCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCancel.Location = new System.Drawing.Point(0, 0);
            this.lblCancel.Name = "lblCancel";
            this.lblCancel.Size = new System.Drawing.Size(545, 425);
            this.lblCancel.TabIndex = 2;
            this.lblCancel.Text = "Cancelling Flow Runs";
            this.lblCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FlowRunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 450);
            this.Controls.Add(this.tscMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FlowRunForm";
            this.Text = "FlowRuns";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFlowRuns)).EndInit();
            this.tscMain.ContentPanel.ResumeLayout(false);
            this.tscMain.ContentPanel.PerformLayout();
            this.tscMain.TopToolStripPanel.ResumeLayout(false);
            this.tscMain.TopToolStripPanel.PerformLayout();
            this.tscMain.ResumeLayout(false);
            this.tscMain.PerformLayout();
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFlowRuns;
        private System.Windows.Forms.ToolStripContainer tscMain;
        private System.Windows.Forms.ToolStrip tools;
        private System.Windows.Forms.ToolStripButton btnStopFlows;
        private System.Windows.Forms.ToolStripComboBox ddlFilter;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Label lblCancel;
    }
}