namespace IO.View.Concrete
{
    partial class DateAndTime
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
            this.spinnerMinute = new IO.View.Spinner();
            this.spinnerHour = new IO.View.Spinner();
            this.labelMonth = new System.Windows.Forms.Label();
            this.labelYear = new System.Windows.Forms.Label();
            this.labelHour = new System.Windows.Forms.Label();
            this.labelMinute = new System.Windows.Forms.Label();
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
            this.labelTime.Text = "15:32:24";
            // 
            // spinnerDay
            // 
            this.spinnerDay.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.spinnerDay.Location = new System.Drawing.Point(121, 180);
            this.spinnerDay.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerDay.Name = "spinnerDay";
            this.spinnerDay.Size = new System.Drawing.Size(90, 140);
            this.spinnerDay.TabIndex = 7;
            this.spinnerDay.Values = null;
            this.spinnerDay.Click += new System.EventHandler(this.spinner_Click);
            // 
            // labelDay
            // 
            this.labelDay.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDay.Location = new System.Drawing.Point(132, 150);
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
            this.spinnerMonth.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.spinnerMonth.Location = new System.Drawing.Point(223, 180);
            this.spinnerMonth.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerMonth.Name = "spinnerMonth";
            this.spinnerMonth.Size = new System.Drawing.Size(90, 140);
            this.spinnerMonth.TabIndex = 10;
            this.spinnerMonth.Values = null;
            this.spinnerMonth.Click += new System.EventHandler(this.spinner_Click);
            // 
            // spinnerYear
            // 
            this.spinnerYear.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.spinnerYear.Location = new System.Drawing.Point(325, 180);
            this.spinnerYear.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerYear.Name = "spinnerYear";
            this.spinnerYear.Size = new System.Drawing.Size(90, 140);
            this.spinnerYear.TabIndex = 11;
            this.spinnerYear.Values = null;
            this.spinnerYear.Click += new System.EventHandler(this.spinner_Click);
            // 
            // spinnerMinute
            // 
            this.spinnerMinute.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.spinnerMinute.Location = new System.Drawing.Point(589, 180);
            this.spinnerMinute.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerMinute.Name = "spinnerMinute";
            this.spinnerMinute.Size = new System.Drawing.Size(90, 140);
            this.spinnerMinute.TabIndex = 12;
            this.spinnerMinute.Values = null;
            this.spinnerMinute.Click += new System.EventHandler(this.spinner_Click);
            // 
            // spinnerHour
            // 
            this.spinnerHour.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.spinnerHour.Location = new System.Drawing.Point(487, 180);
            this.spinnerHour.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerHour.Name = "spinnerHour";
            this.spinnerHour.Size = new System.Drawing.Size(90, 140);
            this.spinnerHour.TabIndex = 13;
            this.spinnerHour.Values = null;
            this.spinnerHour.Click += new System.EventHandler(this.spinner_Click);
            // 
            // labelMonth
            // 
            this.labelMonth.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMonth.Location = new System.Drawing.Point(235, 150);
            this.labelMonth.Name = "labelMonth";
            this.labelMonth.Size = new System.Drawing.Size(65, 20);
            this.labelMonth.TabIndex = 14;
            this.labelMonth.Text = "Month";
            this.labelMonth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelYear
            // 
            this.labelYear.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYear.Location = new System.Drawing.Point(338, 150);
            this.labelYear.Name = "labelYear";
            this.labelYear.Size = new System.Drawing.Size(65, 20);
            this.labelYear.TabIndex = 15;
            this.labelYear.Text = "Year";
            this.labelYear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelHour
            // 
            this.labelHour.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHour.Location = new System.Drawing.Point(499, 150);
            this.labelHour.Name = "labelHour";
            this.labelHour.Size = new System.Drawing.Size(65, 20);
            this.labelHour.TabIndex = 16;
            this.labelHour.Text = "Hour";
            this.labelHour.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMinute
            // 
            this.labelMinute.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinute.Location = new System.Drawing.Point(600, 150);
            this.labelMinute.Name = "labelMinute";
            this.labelMinute.Size = new System.Drawing.Size(65, 20);
            this.labelMinute.TabIndex = 17;
            this.labelMinute.Text = "Minute";
            this.labelMinute.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DateAndTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.labelMinute);
            this.Controls.Add(this.labelHour);
            this.Controls.Add(this.labelYear);
            this.Controls.Add(this.labelMonth);
            this.Controls.Add(this.spinnerHour);
            this.Controls.Add(this.spinnerMinute);
            this.Controls.Add(this.spinnerYear);
            this.Controls.Add(this.spinnerMonth);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.spinnerDay);
            this.Controls.Add(this.labelDay);
            this.HelpText = "Use the up and down buttons to adjust the time/date fields\r\nto the right date and" +
                " time.";
            this.Name = "DateAndTime";
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
            this.Controls.SetChildIndex(this.spinnerMinute, 0);
            this.Controls.SetChildIndex(this.spinnerHour, 0);
            this.Controls.SetChildIndex(this.labelMonth, 0);
            this.Controls.SetChildIndex(this.labelYear, 0);
            this.Controls.SetChildIndex(this.labelHour, 0);
            this.Controls.SetChildIndex(this.labelMinute, 0);
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
        private Spinner spinnerMinute;
        private Spinner spinnerHour;
        private System.Windows.Forms.Label labelMonth;
        private System.Windows.Forms.Label labelYear;
        private System.Windows.Forms.Label labelHour;
        private System.Windows.Forms.Label labelMinute;
    }
}