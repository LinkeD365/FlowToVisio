namespace LinkeD365.FlowToVisio
{
    partial class ApiConnection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApiConnection));
            this.lblClientId = new System.Windows.Forms.Label();
            this.txtAppId = new System.Windows.Forms.TextBox();
            this.txtEnvironment = new System.Windows.Forms.TextBox();
            this.lblEnvironment = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtTenant = new System.Windows.Forms.TextBox();
            this.lblTenant = new System.Windows.Forms.Label();
            this.txtReturnURL = new System.Windows.Forms.TextBox();
            this.lblReturn = new System.Windows.Forms.Label();
            this.chkUseDevApp = new System.Windows.Forms.CheckBox();
            this.flowMain = new System.Windows.Forms.FlowLayoutPanel();
            this.panelFlow = new System.Windows.Forms.Panel();
            this.panelLogicApp = new System.Windows.Forms.Panel();
            this.txtLAReturnURL = new System.Windows.Forms.TextBox();
            this.lblLAApp = new System.Windows.Forms.Label();
            this.txtLAApp = new System.Windows.Forms.TextBox();
            this.lblLAReturn = new System.Windows.Forms.Label();
            this.txtLATenant = new System.Windows.Forms.TextBox();
            this.lblLATenant = new System.Windows.Forms.Label();
            this.lblSubscription = new System.Windows.Forms.Label();
            this.txtSubscriptionId = new System.Windows.Forms.TextBox();
            this.chkLADev = new System.Windows.Forms.CheckBox();
            this.flowMain.SuspendLayout();
            this.panelFlow.SuspendLayout();
            this.panelLogicApp.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(17, 8);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(78, 13);
            this.lblClientId.TabIndex = 0;
            this.lblClientId.Text = "Client / App Id:";
            // 
            // txtAppId
            // 
            this.txtAppId.Location = new System.Drawing.Point(101, 5);
            this.txtAppId.Name = "txtAppId";
            this.txtAppId.Size = new System.Drawing.Size(273, 20);
            this.txtAppId.TabIndex = 1;
            this.txtAppId.Tag = "0";
            // 
            // txtEnvironment
            // 
            this.txtEnvironment.Location = new System.Drawing.Point(101, 53);
            this.txtEnvironment.Name = "txtEnvironment";
            this.txtEnvironment.Size = new System.Drawing.Size(273, 20);
            this.txtEnvironment.TabIndex = 5;
            // 
            // lblEnvironment
            // 
            this.lblEnvironment.AutoSize = true;
            this.lblEnvironment.Location = new System.Drawing.Point(14, 56);
            this.lblEnvironment.Name = "lblEnvironment";
            this.lblEnvironment.Size = new System.Drawing.Size(81, 13);
            this.lblEnvironment.TabIndex = 4;
            this.lblEnvironment.Text = "Environment Id:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(90, 120);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(171, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtTenant
            // 
            this.txtTenant.Location = new System.Drawing.Point(101, 29);
            this.txtTenant.Name = "txtTenant";
            this.txtTenant.Size = new System.Drawing.Size(273, 20);
            this.txtTenant.TabIndex = 3;
            // 
            // lblTenant
            // 
            this.lblTenant.AutoSize = true;
            this.lblTenant.Location = new System.Drawing.Point(39, 34);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(56, 13);
            this.lblTenant.TabIndex = 2;
            this.lblTenant.Text = "Tenant Id:";
            // 
            // txtReturnURL
            // 
            this.txtReturnURL.Location = new System.Drawing.Point(101, 77);
            this.txtReturnURL.Name = "txtReturnURL";
            this.txtReturnURL.Size = new System.Drawing.Size(273, 20);
            this.txtReturnURL.TabIndex = 7;
            // 
            // lblReturn
            // 
            this.lblReturn.AutoSize = true;
            this.lblReturn.Location = new System.Drawing.Point(28, 80);
            this.lblReturn.Name = "lblReturn";
            this.lblReturn.Size = new System.Drawing.Size(67, 13);
            this.lblReturn.TabIndex = 6;
            this.lblReturn.Text = "Return URL:";
            // 
            // chkUseDevApp
            // 
            this.chkUseDevApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkUseDevApp.AutoSize = true;
            this.chkUseDevApp.Location = new System.Drawing.Point(269, 124);
            this.chkUseDevApp.Name = "chkUseDevApp";
            this.chkUseDevApp.Size = new System.Drawing.Size(123, 17);
            this.chkUseDevApp.TabIndex = 10;
            this.chkUseDevApp.Text = "Use Dev App Config";
            this.chkUseDevApp.UseVisualStyleBackColor = true;
            // 
            // flowMain
            // 
            this.flowMain.Controls.Add(this.panelFlow);
            this.flowMain.Controls.Add(this.panelLogicApp);
            this.flowMain.Location = new System.Drawing.Point(-1, 8);
            this.flowMain.Name = "flowMain";
            this.flowMain.Size = new System.Drawing.Size(401, 107);
            this.flowMain.TabIndex = 11;
            // 
            // panelFlow
            // 
            this.panelFlow.Controls.Add(this.txtReturnURL);
            this.panelFlow.Controls.Add(this.lblClientId);
            this.panelFlow.Controls.Add(this.txtAppId);
            this.panelFlow.Controls.Add(this.lblEnvironment);
            this.panelFlow.Controls.Add(this.lblReturn);
            this.panelFlow.Controls.Add(this.txtEnvironment);
            this.panelFlow.Controls.Add(this.txtTenant);
            this.panelFlow.Controls.Add(this.lblTenant);
            this.panelFlow.Location = new System.Drawing.Point(3, 3);
            this.panelFlow.Name = "panelFlow";
            this.panelFlow.Size = new System.Drawing.Size(387, 100);
            this.panelFlow.TabIndex = 12;
            // 
            // panelLogicApp
            // 
            this.panelLogicApp.Controls.Add(this.txtLAReturnURL);
            this.panelLogicApp.Controls.Add(this.lblLAApp);
            this.panelLogicApp.Controls.Add(this.txtLAApp);
            this.panelLogicApp.Controls.Add(this.lblLAReturn);
            this.panelLogicApp.Controls.Add(this.txtLATenant);
            this.panelLogicApp.Controls.Add(this.lblLATenant);
            this.panelLogicApp.Controls.Add(this.lblSubscription);
            this.panelLogicApp.Controls.Add(this.txtSubscriptionId);
            this.panelLogicApp.Location = new System.Drawing.Point(3, 109);
            this.panelLogicApp.Name = "panelLogicApp";
            this.panelLogicApp.Size = new System.Drawing.Size(387, 100);
            this.panelLogicApp.TabIndex = 12;
            // 
            // txtLAReturnURL
            // 
            this.txtLAReturnURL.Location = new System.Drawing.Point(101, 73);
            this.txtLAReturnURL.Name = "txtLAReturnURL";
            this.txtLAReturnURL.Size = new System.Drawing.Size(273, 20);
            this.txtLAReturnURL.TabIndex = 11;
            // 
            // lblLAApp
            // 
            this.lblLAApp.AutoSize = true;
            this.lblLAApp.Location = new System.Drawing.Point(15, 32);
            this.lblLAApp.Name = "lblLAApp";
            this.lblLAApp.Size = new System.Drawing.Size(78, 13);
            this.lblLAApp.TabIndex = 8;
            this.lblLAApp.Text = "Client / App Id:";
            // 
            // txtLAApp
            // 
            this.txtLAApp.Location = new System.Drawing.Point(101, 29);
            this.txtLAApp.Name = "txtLAApp";
            this.txtLAApp.Size = new System.Drawing.Size(273, 20);
            this.txtLAApp.TabIndex = 9;
            this.txtLAApp.Tag = "0";
            // 
            // lblLAReturn
            // 
            this.lblLAReturn.AutoSize = true;
            this.lblLAReturn.Location = new System.Drawing.Point(26, 76);
            this.lblLAReturn.Name = "lblLAReturn";
            this.lblLAReturn.Size = new System.Drawing.Size(67, 13);
            this.lblLAReturn.TabIndex = 10;
            this.lblLAReturn.Text = "Return URL:";
            // 
            // txtLATenant
            // 
            this.txtLATenant.Location = new System.Drawing.Point(101, 51);
            this.txtLATenant.Name = "txtLATenant";
            this.txtLATenant.Size = new System.Drawing.Size(273, 20);
            this.txtLATenant.TabIndex = 5;
            // 
            // lblLATenant
            // 
            this.lblLATenant.AutoSize = true;
            this.lblLATenant.Location = new System.Drawing.Point(37, 54);
            this.lblLATenant.Name = "lblLATenant";
            this.lblLATenant.Size = new System.Drawing.Size(56, 13);
            this.lblLATenant.TabIndex = 4;
            this.lblLATenant.Text = "Tenant Id:";
            // 
            // lblSubscription
            // 
            this.lblSubscription.AutoSize = true;
            this.lblSubscription.Location = new System.Drawing.Point(13, 10);
            this.lblSubscription.Name = "lblSubscription";
            this.lblSubscription.Size = new System.Drawing.Size(80, 13);
            this.lblSubscription.TabIndex = 2;
            this.lblSubscription.Text = "Subscription Id:";
            // 
            // txtSubscriptionId
            // 
            this.txtSubscriptionId.Location = new System.Drawing.Point(101, 7);
            this.txtSubscriptionId.Name = "txtSubscriptionId";
            this.txtSubscriptionId.Size = new System.Drawing.Size(273, 20);
            this.txtSubscriptionId.TabIndex = 3;
            this.txtSubscriptionId.Tag = "0";
            // 
            // chkLADev
            // 
            this.chkLADev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLADev.AutoSize = true;
            this.chkLADev.Location = new System.Drawing.Point(269, 124);
            this.chkLADev.Name = "chkLADev";
            this.chkLADev.Size = new System.Drawing.Size(123, 17);
            this.chkLADev.TabIndex = 12;
            this.chkLADev.Text = "Use Dev App Config";
            this.chkLADev.UseVisualStyleBackColor = true;
            // 
            // ApiConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 153);
            this.Controls.Add(this.chkLADev);
            this.Controls.Add(this.flowMain);
            this.Controls.Add(this.chkUseDevApp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ApiConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure API Connection";
            this.flowMain.ResumeLayout(false);
            this.panelFlow.ResumeLayout(false);
            this.panelFlow.PerformLayout();
            this.panelLogicApp.ResumeLayout(false);
            this.panelLogicApp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblClientId;
        private System.Windows.Forms.TextBox txtAppId;
        private System.Windows.Forms.TextBox txtEnvironment;
        private System.Windows.Forms.Label lblEnvironment;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtTenant;
        private System.Windows.Forms.Label lblTenant;
        private System.Windows.Forms.TextBox txtReturnURL;
        private System.Windows.Forms.Label lblReturn;
        private System.Windows.Forms.CheckBox chkUseDevApp;
        private System.Windows.Forms.FlowLayoutPanel flowMain;
        private System.Windows.Forms.Panel panelFlow;
        private System.Windows.Forms.Panel panelLogicApp;
        private System.Windows.Forms.Label lblSubscription;
        private System.Windows.Forms.TextBox txtSubscriptionId;
        private System.Windows.Forms.TextBox txtLATenant;
        private System.Windows.Forms.Label lblLATenant;
        private System.Windows.Forms.TextBox txtLAReturnURL;
        private System.Windows.Forms.Label lblLAApp;
        private System.Windows.Forms.TextBox txtLAApp;
        private System.Windows.Forms.Label lblLAReturn;
        private System.Windows.Forms.CheckBox chkLADev;
    }
}