namespace IO.View.Concrete
{
    partial class TimeoutSpin
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
            this.buttonOk = new IO.View.Button();
            this.spinnerMinutes = new IO.View.Spinner();
            this.labelMinutes = new System.Windows.Forms.Label();
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
            this.labelDate.Text = "10 Jun 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "14:10:49";
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(553, 342);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(220, 40);
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "Save Changes";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // spinnerMinutes
            // 
            this.spinnerMinutes.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spinnerMinutes.Location = new System.Drawing.Point(355, 175);
            this.spinnerMinutes.Margin = new System.Windows.Forms.Padding(6, 10, 6, 10);
            this.spinnerMinutes.Name = "spinnerMinutes";
            this.spinnerMinutes.Size = new System.Drawing.Size(90, 140);
            this.spinnerMinutes.TabIndex = 10;
            this.spinnerMinutes.Values = null;
            // 
            // labelMinutes
            // 
            this.labelMinutes.Location = new System.Drawing.Point(362, 145);
            this.labelMinutes.Name = "labelMinutes";
            this.labelMinutes.Size = new System.Drawing.Size(75, 20);
            this.labelMinutes.TabIndex = 14;
            this.labelMinutes.Text = "Minutes";
            this.labelMinutes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimeoutSpin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.labelMinutes);
            this.Controls.Add(this.spinnerMinutes);
            this.Controls.Add(this.buttonOk);
            this.Name = "TimeoutSpin";
            this.Text = "DateAndTime";
            this.Load += new System.EventHandler(this.TimeoutSpin_Load);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.buttonOk, 0);
            this.Controls.SetChildIndex(this.spinnerMinutes, 0);
            this.Controls.SetChildIndex(this.labelMinutes, 0);
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

        private Button buttonOk;
        private Spinner spinnerMinutes;
        private System.Windows.Forms.Label labelMinutes;
    }
}