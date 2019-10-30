namespace IO.View.Concrete
{
    partial class Date
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
            this.spinnerDay = new IO.View.Spinner();
            this.labelDay = new System.Windows.Forms.Label();
            this.buttonOk = new IO.View.Button();
            this.spinnerMonth = new IO.View.Spinner();
            this.spinnerYear = new IO.View.Spinner();
            this.labelMonth = new System.Windows.Forms.Label();
            this.labelYear = new System.Windows.Forms.Label();
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
            this.labelDate.Text = "27 Feb 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "14:03:09";
            // 
            // spinnerDay
            // 
            this.spinnerDay.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spinnerDay.Location = new System.Drawing.Point(253, 175);
            this.spinnerDay.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerDay.Name = "spinnerDay";
            this.spinnerDay.Size = new System.Drawing.Size(90, 140);
            this.spinnerDay.TabIndex = 7;
            this.spinnerDay.Values = null;
            this.spinnerDay.Click += new System.EventHandler(this.spinner_Click);
            // 
            // labelDay
            // 
            this.labelDay.Location = new System.Drawing.Point(265, 145);
            this.labelDay.Name = "labelDay";
            this.labelDay.Size = new System.Drawing.Size(65, 20);
            this.labelDay.TabIndex = 8;
            this.labelDay.Text = "Day";
            this.labelDay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(613, 345);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(160, 40);
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // spinnerMonth
            // 
            this.spinnerMonth.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spinnerMonth.Location = new System.Drawing.Point(355, 175);
            this.spinnerMonth.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerMonth.Name = "spinnerMonth";
            this.spinnerMonth.Size = new System.Drawing.Size(90, 140);
            this.spinnerMonth.TabIndex = 10;
            this.spinnerMonth.Values = null;
            this.spinnerMonth.Click += new System.EventHandler(this.spinner_Click);
            // 
            // spinnerYear
            // 
            this.spinnerYear.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spinnerYear.Location = new System.Drawing.Point(457, 175);
            this.spinnerYear.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerYear.Name = "spinnerYear";
            this.spinnerYear.Size = new System.Drawing.Size(90, 140);
            this.spinnerYear.TabIndex = 11;
            this.spinnerYear.Values = null;
            this.spinnerYear.Click += new System.EventHandler(this.spinner_Click);
            // 
            // labelMonth
            // 
            this.labelMonth.Location = new System.Drawing.Point(367, 145);
            this.labelMonth.Name = "labelMonth";
            this.labelMonth.Size = new System.Drawing.Size(65, 20);
            this.labelMonth.TabIndex = 14;
            this.labelMonth.Text = "Month";
            this.labelMonth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelYear
            // 
            this.labelYear.Location = new System.Drawing.Point(469, 145);
            this.labelYear.Name = "labelYear";
            this.labelYear.Size = new System.Drawing.Size(65, 20);
            this.labelYear.TabIndex = 15;
            this.labelYear.Text = "Year";
            this.labelYear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Date
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.labelYear);
            this.Controls.Add(this.labelMonth);
            this.Controls.Add(this.spinnerYear);
            this.Controls.Add(this.spinnerMonth);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.spinnerDay);
            this.Controls.Add(this.labelDay);
            this.Name = "Date";
            this.Text = "DateAndTime";
            this.Controls.SetChildIndex(this.labelDay, 0);
            this.Controls.SetChildIndex(this.spinnerDay, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.buttonOk, 0);
            this.Controls.SetChildIndex(this.spinnerMonth, 0);
            this.Controls.SetChildIndex(this.spinnerYear, 0);
            this.Controls.SetChildIndex(this.labelMonth, 0);
            this.Controls.SetChildIndex(this.labelYear, 0);
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

        private Spinner spinnerDay;
        private System.Windows.Forms.Label labelDay;
        private Button buttonOk;
        private Spinner spinnerMonth;
        private Spinner spinnerYear;
        private System.Windows.Forms.Label labelMonth;
        private System.Windows.Forms.Label labelYear;
    }
}