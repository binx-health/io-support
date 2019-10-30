namespace IO.View.Concrete
{
    partial class MainMenu
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
            this.buttonRunTest = new IO.View.Button();
            this.buttonMyResults = new IO.View.Button();
            this.buttonLogout = new IO.View.Button();
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
            this.labelDate.Text = "12 Sep 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "14:33:20";
            // 
            // buttonRunTest
            // 
            this.buttonRunTest.BackColor = System.Drawing.Color.White;
            this.buttonRunTest.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRunTest.Location = new System.Drawing.Point(100, 130);
            this.buttonRunTest.Margin = new System.Windows.Forms.Padding(15);
            this.buttonRunTest.Name = "buttonRunTest";
            this.buttonRunTest.Size = new System.Drawing.Size(240, 40);
            this.buttonRunTest.TabIndex = 6;
            this.buttonRunTest.Text = "Run/Cancel Test";
            this.buttonRunTest.Click += new System.EventHandler(this.buttonRunTest_Click);
            // 
            // buttonMyResults
            // 
            this.buttonMyResults.BackColor = System.Drawing.Color.Transparent;
            this.buttonMyResults.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMyResults.Location = new System.Drawing.Point(450, 130);
            this.buttonMyResults.Margin = new System.Windows.Forms.Padding(15);
            this.buttonMyResults.Name = "buttonMyResults";
            this.buttonMyResults.Size = new System.Drawing.Size(240, 40);
            this.buttonMyResults.TabIndex = 7;
            this.buttonMyResults.Text = "My Results";
            this.buttonMyResults.Click += new System.EventHandler(this.buttonMyResults_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.BackColor = System.Drawing.Color.Transparent;
            this.buttonLogout.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLogout.Location = new System.Drawing.Point(100, 200);
            this.buttonLogout.Margin = new System.Windows.Forms.Padding(15);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(240, 40);
            this.buttonLogout.TabIndex = 8;
            this.buttonLogout.Text = "Logout";
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // buttonEject
            // 
            this.buttonEject.BackColor = System.Drawing.Color.Transparent;
            this.buttonEject.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEject.Location = new System.Drawing.Point(450, 200);
            this.buttonEject.Margin = new System.Windows.Forms.Padding(15);
            this.buttonEject.Name = "buttonEject";
            this.buttonEject.Size = new System.Drawing.Size(240, 40);
            this.buttonEject.TabIndex = 9;
            this.buttonEject.Text = "Eject";
            this.buttonEject.Click += new System.EventHandler(this.buttonEject_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonEject);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonMyResults);
            this.Controls.Add(this.buttonRunTest);
            this.HelpText = "This is \'home\' if users have a basic user account.\r\n\r\nChoose from running Tests o" +
                "r viewing data.\r\n\r\nUsers will only be able to view data from Tests they have run" +
                ".";
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.Controls.SetChildIndex(this.pictureBoxBackground, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.buttonRunTest, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.buttonMyResults, 0);
            this.Controls.SetChildIndex(this.buttonLogout, 0);
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
        private Button buttonEject;
    }
}