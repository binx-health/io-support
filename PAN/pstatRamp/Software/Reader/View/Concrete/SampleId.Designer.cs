namespace IO.View.Concrete
{
    partial class SampleId
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
            this.textBox = new IO.View.TextBox();
            this.buttonNext = new IO.View.Button();
            this.buttonCancel = new IO.View.Button();
            this.illustration1 = new IO.View.Illustration();
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
            this.labelTime.Text = "14:07:56";
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.Transparent;
            this.textBox.Location = new System.Drawing.Point(25, 125);
            this.textBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBox.Name = "textBox";
            this.textBox.Password = false;
            this.textBox.Size = new System.Drawing.Size(750, 40);
            this.textBox.TabIndex = 7;
            this.textBox.TabStop = false;
            this.textBox.Title = "Specimen ID";
            this.textBox.TitleWidth = 165;
            this.textBox.Click += new System.EventHandler(this.textBoxSampleId_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.Transparent;
            this.buttonNext.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNext.Location = new System.Drawing.Point(613, 345);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(160, 40);
            this.buttonNext.TabIndex = 9;
            this.buttonNext.TabStop = false;
            this.buttonNext.Text = "Next";
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.Transparent;
            this.buttonCancel.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(25, 345);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(160, 40);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.TabStop = false;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // illustration1
            // 
            this.illustration1.BackColor = System.Drawing.Color.Transparent;
            this.illustration1.Image = global::IO.View.Properties.Resources.Barcode_scan;
            this.illustration1.Location = new System.Drawing.Point(25, 173);
            this.illustration1.Name = "illustration1";
            this.illustration1.Size = new System.Drawing.Size(750, 146);
            this.illustration1.TabIndex = 14;
            this.illustration1.TabStop = false;
            this.illustration1.Text = "illustration1";
            // 
            // SampleId
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.illustration1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.textBox);
            this.HelpText = "You have started to run a test.\r\nUse a hand-held barcode scanner (if fitted)\r\nor " +
                "click on the Specimen ID box to enter manually.";
            this.Name = "SampleId";
            this.Text = "RunTest";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SampleId_KeyPress);
            this.Controls.SetChildIndex(this.pictureBoxBackground, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBox, 0);
            this.Controls.SetChildIndex(this.buttonNext, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.illustration1, 0);
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

        private TextBox textBox;
        private Button buttonNext;
        private Button buttonCancel;
        private Illustration illustration1;
    }
}