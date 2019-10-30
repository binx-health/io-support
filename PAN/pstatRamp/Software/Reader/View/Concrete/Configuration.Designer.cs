namespace IO.View.Concrete
{
    partial class Configuration
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
            this.buttonDateTime = new IO.View.Button();
            this.buttonLanguage = new IO.View.Button();
            this.buttonAbout = new IO.View.Button();
            this.buttonAssaySettings = new IO.View.Button();
            this.buttonQcTestSetup = new IO.View.Button();
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
            this.labelTime.Text = "15:30:45";
            // 
            // buttonInstrumentName
            // 
            this.buttonInstrumentName.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInstrumentName.Location = new System.Drawing.Point(100, 130);
            this.buttonInstrumentName.Margin = new System.Windows.Forms.Padding(15);
            this.buttonInstrumentName.Name = "buttonInstrumentName";
            this.buttonInstrumentName.Size = new System.Drawing.Size(250, 40);
            this.buttonInstrumentName.TabIndex = 6;
            this.buttonInstrumentName.Text = "Instrument Name";
            this.buttonInstrumentName.Click += new System.EventHandler(this.buttonInstrumentName_Click);
            // 
            // buttonDateTime
            // 
            this.buttonDateTime.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonDateTime.Location = new System.Drawing.Point(450, 130);
            this.buttonDateTime.Margin = new System.Windows.Forms.Padding(15);
            this.buttonDateTime.Name = "buttonDateTime";
            this.buttonDateTime.Size = new System.Drawing.Size(250, 40);
            this.buttonDateTime.TabIndex = 7;
            this.buttonDateTime.Text = "Set Date/Time";
            this.buttonDateTime.Click += new System.EventHandler(this.buttonDateTime_Click);
            // 
            // buttonLanguage
            // 
            this.buttonLanguage.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonLanguage.Location = new System.Drawing.Point(100, 200);
            this.buttonLanguage.Margin = new System.Windows.Forms.Padding(15);
            this.buttonLanguage.Name = "buttonLanguage";
            this.buttonLanguage.Size = new System.Drawing.Size(250, 40);
            this.buttonLanguage.TabIndex = 9;
            this.buttonLanguage.Text = "Set Language";
            this.buttonLanguage.Click += new System.EventHandler(this.buttonLanguage_Click);
            // 
            // buttonAbout
            // 
            this.buttonAbout.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonAbout.Location = new System.Drawing.Point(450, 200);
            this.buttonAbout.Margin = new System.Windows.Forms.Padding(15);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(250, 40);
            this.buttonAbout.TabIndex = 10;
            this.buttonAbout.Text = "About";
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // buttonAssaySettings
            // 
            this.buttonAssaySettings.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonAssaySettings.Location = new System.Drawing.Point(100, 270);
            this.buttonAssaySettings.Margin = new System.Windows.Forms.Padding(15);
            this.buttonAssaySettings.Name = "buttonAssaySettings";
            this.buttonAssaySettings.Size = new System.Drawing.Size(250, 40);
            this.buttonAssaySettings.TabIndex = 11;
            this.buttonAssaySettings.Text = "Assay Settings";
            this.buttonAssaySettings.Click += new System.EventHandler(this.buttonAssaySettings_Click);
            // 
            // buttonQcTestSetup
            // 
            this.buttonQcTestSetup.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonQcTestSetup.Location = new System.Drawing.Point(450, 270);
            this.buttonQcTestSetup.Margin = new System.Windows.Forms.Padding(15);
            this.buttonQcTestSetup.Name = "buttonQcTestSetup";
            this.buttonQcTestSetup.Size = new System.Drawing.Size(250, 40);
            this.buttonQcTestSetup.TabIndex = 12;
            this.buttonQcTestSetup.Text = "QC Test Setup";
            this.buttonQcTestSetup.Click += new System.EventHandler(this.buttonQcTestSetup_Click);
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonQcTestSetup);
            this.Controls.Add(this.buttonAssaySettings);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.buttonInstrumentName);
            this.Controls.Add(this.buttonLanguage);
            this.Controls.Add(this.buttonDateTime);
            this.HelpText = "Reader settings menu\r\nThis screen allows an Administrator to edit settings for th" +
                "e Reader.\r\nClick to select or BACK to return to Reader Menu";
            this.Name = "Configuration";
            this.Text = "MainMenu";
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.buttonDateTime, 0);
            this.Controls.SetChildIndex(this.buttonLanguage, 0);
            this.Controls.SetChildIndex(this.buttonInstrumentName, 0);
            this.Controls.SetChildIndex(this.buttonAbout, 0);
            this.Controls.SetChildIndex(this.buttonAssaySettings, 0);
            this.Controls.SetChildIndex(this.buttonQcTestSetup, 0);
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
        private Button buttonDateTime;
        private Button buttonLanguage;
        private Button buttonAbout;
        private Button buttonAssaySettings;
        private Button buttonQcTestSetup;
    }
}