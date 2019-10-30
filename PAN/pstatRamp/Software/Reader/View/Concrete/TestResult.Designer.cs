namespace IO.View.Concrete
{
    partial class TestResult
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
            this.textBoxSampleId = new IO.View.TextBox();
            this.textBoxTestType = new IO.View.TextBox();
            this.textBoxResult = new IO.View.TextBox();
            this.buttonExit = new IO.View.Button();
            this.buttonPrint = new IO.View.Button();
            this.buttonRunNewTest = new IO.View.Button();
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
            this.labelTime.Text = "14:58:17";
            // 
            // textBoxSampleId
            // 
            this.textBoxSampleId.Location = new System.Drawing.Point(25, 125);
            this.textBoxSampleId.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxSampleId.Name = "textBoxSampleId";
            this.textBoxSampleId.Password = false;
            this.textBoxSampleId.Size = new System.Drawing.Size(750, 40);
            this.textBoxSampleId.TabIndex = 11;
            this.textBoxSampleId.Tag = "0";
            this.textBoxSampleId.Title = "Specimen ID";
            this.textBoxSampleId.TitleWidth = 165;
            // 
            // textBoxTestType
            // 
            this.textBoxTestType.Location = new System.Drawing.Point(25, 175);
            this.textBoxTestType.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxTestType.Name = "textBoxTestType";
            this.textBoxTestType.Password = false;
            this.textBoxTestType.Size = new System.Drawing.Size(750, 40);
            this.textBoxTestType.TabIndex = 13;
            this.textBoxTestType.Tag = "1";
            this.textBoxTestType.Title = "Test Type";
            this.textBoxTestType.TitleWidth = 165;
            // 
            // textBoxResult
            // 
            this.textBoxResult.Font = new System.Drawing.Font("Arial Black", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxResult.Location = new System.Drawing.Point(25, 225);
            this.textBoxResult.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.Password = false;
            this.textBoxResult.Size = new System.Drawing.Size(750, 80);
            this.textBoxResult.TabIndex = 16;
            this.textBoxResult.Tag = "4";
            this.textBoxResult.Title = "Test Result";
            this.textBoxResult.TitleWidth = 165;
            // 
            // buttonExit
            // 
            this.buttonExit.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.Location = new System.Drawing.Point(615, 345);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(160, 40);
            this.buttonExit.TabIndex = 17;
            this.buttonExit.Text = "Exit";
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPrint.Location = new System.Drawing.Point(25, 345);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(160, 40);
            this.buttonPrint.TabIndex = 18;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // buttonRunNewTest
            // 
            this.buttonRunNewTest.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRunNewTest.Location = new System.Drawing.Point(296, 345);
            this.buttonRunNewTest.Name = "buttonRunNewTest";
            this.buttonRunNewTest.Size = new System.Drawing.Size(220, 40);
            this.buttonRunNewTest.TabIndex = 19;
            this.buttonRunNewTest.Text = "Run New Test";
            this.buttonRunNewTest.Visible = false;
            this.buttonRunNewTest.Click += new System.EventHandler(this.buttonRunNewTest_Click);
            // 
            // TestResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonRunNewTest);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.textBoxTestType);
            this.Controls.Add(this.textBoxSampleId);
            this.HelpText = "The Test is complete.\r\n\r\nSelect options to View Results, Print, Run a New Test or" +
                " Exit.\r\n";
            this.Name = "TestResult";
            this.Text = "PatientInformation";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxSampleId, 0);
            this.Controls.SetChildIndex(this.textBoxTestType, 0);
            this.Controls.SetChildIndex(this.textBoxResult, 0);
            this.Controls.SetChildIndex(this.buttonExit, 0);
            this.Controls.SetChildIndex(this.buttonPrint, 0);
            this.Controls.SetChildIndex(this.buttonRunNewTest, 0);
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

        private TextBox textBoxSampleId;
        private TextBox textBoxTestType;
        private TextBox textBoxResult;
        private Button buttonExit;
        private Button buttonPrint;
        private Button buttonRunNewTest;
    }
}