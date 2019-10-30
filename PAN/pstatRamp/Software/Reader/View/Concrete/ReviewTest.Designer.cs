namespace IO.View.Concrete
{
    partial class ReviewTest
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
            this.textBoxAssay = new IO.View.TextBox();
            this.textBoxPatientId = new IO.View.TextBox();
            this.buttonCancel = new IO.View.Button();
            this.buttonRunTest = new IO.View.Button();
            this.textBoxSampleId = new IO.View.TextBox();
            this.checkBoxQcTest = new IO.View.CheckBox();
            this.buttonEdit = new IO.View.Button();
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
            this.labelTime.Text = "14:56:04";
            // 
            // textBoxAssay
            // 
            this.textBoxAssay.Location = new System.Drawing.Point(25, 225);
            this.textBoxAssay.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxAssay.Name = "textBoxAssay";
            this.textBoxAssay.Password = false;
            this.textBoxAssay.Size = new System.Drawing.Size(750, 40);
            this.textBoxAssay.TabIndex = 19;
            this.textBoxAssay.Tag = "3";
            this.textBoxAssay.Title = "Selected Test";
            this.textBoxAssay.TitleWidth = 165;
            // 
            // textBoxPatientId
            // 
            this.textBoxPatientId.Location = new System.Drawing.Point(25, 125);
            this.textBoxPatientId.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxPatientId.Name = "textBoxPatientId";
            this.textBoxPatientId.Password = false;
            this.textBoxPatientId.Size = new System.Drawing.Size(750, 40);
            this.textBoxPatientId.TabIndex = 16;
            this.textBoxPatientId.Tag = "0";
            this.textBoxPatientId.Title = "Patient ID";
            this.textBoxPatientId.TitleWidth = 165;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(25, 345);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(160, 40);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonRunTest
            // 
            this.buttonRunTest.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRunTest.Location = new System.Drawing.Point(338, 305);
            this.buttonRunTest.Name = "buttonRunTest";
            this.buttonRunTest.Size = new System.Drawing.Size(120, 80);
            this.buttonRunTest.TabIndex = 20;
            this.buttonRunTest.Text = "Run Test";
            this.buttonRunTest.Click += new System.EventHandler(this.buttonRunTest_Click);
            // 
            // textBoxSampleId
            // 
            this.textBoxSampleId.Location = new System.Drawing.Point(25, 175);
            this.textBoxSampleId.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxSampleId.Name = "textBoxSampleId";
            this.textBoxSampleId.Password = false;
            this.textBoxSampleId.Size = new System.Drawing.Size(750, 40);
            this.textBoxSampleId.TabIndex = 22;
            this.textBoxSampleId.Tag = "0";
            this.textBoxSampleId.Title = "Specimen ID";
            this.textBoxSampleId.TitleWidth = 165;
            // 
            // checkBoxQcTest
            // 
            this.checkBoxQcTest.Checked = false;
            this.checkBoxQcTest.Image = global::IO.View.Properties.Resources.Tick;
            this.checkBoxQcTest.Location = new System.Drawing.Point(25, 275);
            this.checkBoxQcTest.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.checkBoxQcTest.Name = "checkBoxQcTest";
            this.checkBoxQcTest.Size = new System.Drawing.Size(160, 40);
            this.checkBoxQcTest.TabIndex = 23;
            this.checkBoxQcTest.Text = "QC Test?";
            this.checkBoxQcTest.Click += new System.EventHandler(this.checkBoxQcTest_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEdit.Location = new System.Drawing.Point(573, 345);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(200, 40);
            this.buttonEdit.TabIndex = 24;
            this.buttonEdit.Text = "Edit Details";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // ReviewTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.checkBoxQcTest);
            this.Controls.Add(this.textBoxSampleId);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonRunTest);
            this.Controls.Add(this.textBoxAssay);
            this.Controls.Add(this.textBoxPatientId);
            this.HelpText = "This is the final check prior to running a Test.\r\n\r\nCheck that all fields are fil" +
                "led in correctly and that the expected Test is ready.\r\n\r\nClick Run Test.";
            this.Name = "ReviewTest";
            this.Text = "ReviewTest";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxPatientId, 0);
            this.Controls.SetChildIndex(this.textBoxAssay, 0);
            this.Controls.SetChildIndex(this.buttonRunTest, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.textBoxSampleId, 0);
            this.Controls.SetChildIndex(this.checkBoxQcTest, 0);
            this.Controls.SetChildIndex(this.buttonEdit, 0);
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

        private TextBox textBoxAssay;
        private TextBox textBoxPatientId;
        private Button buttonCancel;
        private Button buttonRunTest;
        private TextBox textBoxSampleId;
        private CheckBox checkBoxQcTest;
        private Button buttonEdit;
    }
}