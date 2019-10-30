namespace IO.View.Concrete
{
    partial class Setup
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
            this.buttonInstrumentName = new IO.View.Button();
            this.buttonUserManagement = new IO.View.Button();
            this.buttonDataPolicy = new IO.View.Button();
            this.buttonAddAssays = new IO.View.Button();
            this.buttonServerSettings = new IO.View.Button();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Location = new System.Drawing.Point(0, 410);
            this.panelBottom.Size = new System.Drawing.Size(800, 70);
            // 
            // panelTop
            // 
            this.panelTop.Size = new System.Drawing.Size(800, 50);
            // 
            // labelUser
            // 
            this.labelUser.Text = "";
            // 
            // labelDate
            // 
            this.labelDate.Text = "19 Jun 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "15:29:38";
            // 
            // buttonInstrumentName
            // 
            this.buttonInstrumentName.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInstrumentName.Location = new System.Drawing.Point(100, 130);
            this.buttonInstrumentName.Margin = new System.Windows.Forms.Padding(15);
            this.buttonInstrumentName.Name = "buttonInstrumentName";
            this.buttonInstrumentName.Size = new System.Drawing.Size(250, 40);
            this.buttonInstrumentName.TabIndex = 6;
            this.buttonInstrumentName.Text = "Configuration";
            this.buttonInstrumentName.Click += new System.EventHandler(this.buttonConfiguration_Click);
            // 
            // buttonUserManagement
            // 
            this.buttonUserManagement.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUserManagement.Location = new System.Drawing.Point(100, 200);
            this.buttonUserManagement.Margin = new System.Windows.Forms.Padding(15);
            this.buttonUserManagement.Name = "buttonUserManagement";
            this.buttonUserManagement.Size = new System.Drawing.Size(250, 40);
            this.buttonUserManagement.TabIndex = 7;
            this.buttonUserManagement.Text = "User Mgmt";
            this.buttonUserManagement.Click += new System.EventHandler(this.buttonUserManagement_Click);
            // 
            // buttonDataPolicy
            // 
            this.buttonDataPolicy.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDataPolicy.Location = new System.Drawing.Point(450, 130);
            this.buttonDataPolicy.Margin = new System.Windows.Forms.Padding(15);
            this.buttonDataPolicy.Name = "buttonDataPolicy";
            this.buttonDataPolicy.Size = new System.Drawing.Size(250, 40);
            this.buttonDataPolicy.TabIndex = 9;
            this.buttonDataPolicy.Text = "Data Policy";
            this.buttonDataPolicy.Click += new System.EventHandler(this.buttonDataPolicy_Click);
            // 
            // buttonAddAssays
            // 
            this.buttonAddAssays.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddAssays.Location = new System.Drawing.Point(100, 270);
            this.buttonAddAssays.Margin = new System.Windows.Forms.Padding(15);
            this.buttonAddAssays.Name = "buttonAddAssays";
            this.buttonAddAssays.Size = new System.Drawing.Size(250, 40);
            this.buttonAddAssays.TabIndex = 10;
            this.buttonAddAssays.Text = "Add Tests";
            this.buttonAddAssays.Click += new System.EventHandler(this.buttonAddAssays_Click);
            // 
            // buttonServerSettings
            // 
            this.buttonServerSettings.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonServerSettings.Location = new System.Drawing.Point(450, 200);
            this.buttonServerSettings.Margin = new System.Windows.Forms.Padding(15);
            this.buttonServerSettings.Name = "buttonServerSettings";
            this.buttonServerSettings.Size = new System.Drawing.Size(250, 40);
            this.buttonServerSettings.TabIndex = 11;
            this.buttonServerSettings.Text = "Server Settings";
            this.buttonServerSettings.Click += new System.EventHandler(this.buttonServerSettings_Click);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonServerSettings);
            this.Controls.Add(this.buttonAddAssays);
            this.Controls.Add(this.buttonInstrumentName);
            this.Controls.Add(this.buttonDataPolicy);
            this.Controls.Add(this.buttonUserManagement);
            this.HelpText = "Reader setup menu\r\nThis screen offers a number of Reader setup options. Click to " +
                "select.";
            this.Name = "Setup";
            this.Text = "MainMenu";
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.buttonUserManagement, 0);
            this.Controls.SetChildIndex(this.buttonDataPolicy, 0);
            this.Controls.SetChildIndex(this.buttonInstrumentName, 0);
            this.Controls.SetChildIndex(this.buttonAddAssays, 0);
            this.Controls.SetChildIndex(this.buttonServerSettings, 0);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).EndInit();
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonInstrumentName;
        private Button buttonUserManagement;
        private Button buttonDataPolicy;
        private Button buttonAddAssays;
        private Button buttonServerSettings;
    }
}