namespace IO.View.Concrete
{
    partial class TestComplete
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
            this.buttonViewResults = new IO.View.Button();
            this.textBoxPatientId = new IO.View.TextBox();
            this.textBoxSampleId = new IO.View.TextBox();
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
            this.labelTime.Text = "14:58:01";
            // 
            // buttonViewResults
            // 
            this.buttonViewResults.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonViewResults.Location = new System.Drawing.Point(575, 345);
            this.buttonViewResults.Name = "buttonViewResults";
            this.buttonViewResults.Size = new System.Drawing.Size(200, 40);
            this.buttonViewResults.TabIndex = 23;
            this.buttonViewResults.Text = "View Results";
            this.buttonViewResults.Click += new System.EventHandler(this.buttonViewResults_Click);
            // 
            // textBoxPatientId
            // 
            this.textBoxPatientId.Location = new System.Drawing.Point(25, 175);
            this.textBoxPatientId.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxPatientId.Name = "textBoxPatientId";
            this.textBoxPatientId.Password = false;
            this.textBoxPatientId.Size = new System.Drawing.Size(750, 40);
            this.textBoxPatientId.TabIndex = 22;
            this.textBoxPatientId.Tag = "1";
            this.textBoxPatientId.Title = "Patient ID";
            this.textBoxPatientId.TitleWidth = 165;
            // 
            // textBoxSampleId
            // 
            this.textBoxSampleId.Location = new System.Drawing.Point(25, 125);
            this.textBoxSampleId.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxSampleId.Name = "textBoxSampleId";
            this.textBoxSampleId.Password = false;
            this.textBoxSampleId.Size = new System.Drawing.Size(750, 40);
            this.textBoxSampleId.TabIndex = 21;
            this.textBoxSampleId.Tag = "0";
            this.textBoxSampleId.Title = "Specimen ID";
            this.textBoxSampleId.TitleWidth = 165;
            // 
            // TestComplete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonViewResults);
            this.Controls.Add(this.textBoxPatientId);
            this.Controls.Add(this.textBoxSampleId);
            this.HelpText = "The Test is complete.\r\n\r\nSelect options to View Results, Print, Run a New Test or" +
                " Exit.";
            this.Name = "TestComplete";
            this.Text = "TestComplete";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxSampleId, 0);
            this.Controls.SetChildIndex(this.textBoxPatientId, 0);
            this.Controls.SetChildIndex(this.buttonViewResults, 0);
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

        private Button buttonViewResults;
        private TextBox textBoxPatientId;
        private TextBox textBoxSampleId;
    }
}