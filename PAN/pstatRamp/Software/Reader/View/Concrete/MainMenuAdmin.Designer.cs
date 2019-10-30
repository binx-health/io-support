namespace IO.View.Concrete
{
    partial class MainMenuAdmin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenuAdmin));
            this.buttonRunTest = new IO.View.Button();
            this.buttonMyResults = new IO.View.Button();
            this.buttonLogout = new IO.View.Button();
            this.buttonSetup = new IO.View.Button();
            this.buttonExportData = new IO.View.Button();
            this.buttonEject = new IO.View.Button();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            resources.ApplyResources(this.panelBottom, "panelBottom");
            // 
            // panelTop
            // 
            resources.ApplyResources(this.panelTop, "panelTop");
            // 
            // labelUser
            // 
            resources.ApplyResources(this.labelUser, "labelUser");
            // 
            // labelDate
            // 
            resources.ApplyResources(this.labelDate, "labelDate");
            // 
            // labelTime
            // 
            resources.ApplyResources(this.labelTime, "labelTime");
            // 
            // buttonRunTest
            // 
            this.buttonRunTest.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonRunTest, "buttonRunTest");
            this.buttonRunTest.Name = "buttonRunTest";
            this.buttonRunTest.Click += new System.EventHandler(this.buttonRunTest_Click);
            // 
            // buttonMyResults
            // 
            this.buttonMyResults.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonMyResults, "buttonMyResults");
            this.buttonMyResults.Name = "buttonMyResults";
            this.buttonMyResults.Click += new System.EventHandler(this.buttonMyResults_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonLogout, "buttonLogout");
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // buttonSetup
            // 
            this.buttonSetup.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonSetup, "buttonSetup");
            this.buttonSetup.Name = "buttonSetup";
            this.buttonSetup.Click += new System.EventHandler(this.buttonSetup_Click);
            // 
            // buttonExportData
            // 
            this.buttonExportData.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonExportData, "buttonExportData");
            this.buttonExportData.Name = "buttonExportData";
            this.buttonExportData.Click += new System.EventHandler(this.buttonExportData_Click);
            // 
            // buttonEject
            // 
            this.buttonEject.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonEject, "buttonEject");
            this.buttonEject.Name = "buttonEject";
            this.buttonEject.Click += new System.EventHandler(this.buttonEject_Click);
            // 
            // MainMenuAdmin
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonEject);
            this.Controls.Add(this.buttonExportData);
            this.Controls.Add(this.buttonRunTest);
            this.Controls.Add(this.buttonSetup);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonMyResults);
            this.HelpText = "This is \'home\' for users with an administrative account.\r\n\r\nChoose from running T" +
                "ests, configuring the reader, or viewing data.\r\n\r\nAdmin users will be able to vi" +
                "ew all recorded data.";
            this.Name = "MainMenuAdmin";
            this.Controls.SetChildIndex(this.pictureBoxBackground, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.buttonMyResults, 0);
            this.Controls.SetChildIndex(this.buttonLogout, 0);
            this.Controls.SetChildIndex(this.buttonSetup, 0);
            this.Controls.SetChildIndex(this.buttonRunTest, 0);
            this.Controls.SetChildIndex(this.buttonExportData, 0);
            this.Controls.SetChildIndex(this.buttonEject, 0);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).EndInit();
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonRunTest;
        private Button buttonMyResults;
        private Button buttonLogout;
        private Button buttonSetup;
        private Button buttonExportData;
        private Button buttonEject;
    }
}