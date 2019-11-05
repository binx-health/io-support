﻿namespace IO.View.Concrete
{
    partial class AssaySettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssaySettings));
            this.checkBoxUng = new IO.View.CheckBox();
            this.buttonOk = new IO.View.Button();
            this.checkBoxAutoSampleId = new IO.View.CheckBox();
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
            this.labelDate.Text = "23 Aug 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "15:39:30";
            // 
            // checkBoxUng
            // 
            this.checkBoxUng.Checked = false;
            this.checkBoxUng.Image = global::IO.View.Properties.Resources.Tick;
            this.checkBoxUng.Location = new System.Drawing.Point(25, 120);
            this.checkBoxUng.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.checkBoxUng.Name = "checkBoxUng";
            this.checkBoxUng.Size = new System.Drawing.Size(750, 40);
            this.checkBoxUng.TabIndex = 20;
            this.checkBoxUng.Text = "Disable Contamination Control";
            this.checkBoxUng.Click += new System.EventHandler(this.checkBoxUng_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonOk.Location = new System.Drawing.Point(613, 345);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(160, 40);
            this.buttonOk.TabIndex = 21;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // checkBoxAutoSampleId
            // 
            this.checkBoxAutoSampleId.Checked = false;
            this.checkBoxAutoSampleId.Image = global::IO.View.Properties.Resources.Tick;
            this.checkBoxAutoSampleId.Location = new System.Drawing.Point(25, 170);
            this.checkBoxAutoSampleId.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.checkBoxAutoSampleId.Name = "checkBoxAutoSampleId";
            this.checkBoxAutoSampleId.Size = new System.Drawing.Size(750, 40);
            this.checkBoxAutoSampleId.TabIndex = 22;
            this.checkBoxAutoSampleId.Text = "Use Automatic Specimen ID Method";
            this.checkBoxAutoSampleId.Click += new System.EventHandler(this.checkBoxAutoSampleId_Click);
            // 
            // AssaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.checkBoxAutoSampleId);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxUng);
            this.HelpText = resources.GetString("$this.HelpText");
            this.Name = "AssaySettings";
            this.Text = "AssaySettings";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.checkBoxUng, 0);
            this.Controls.SetChildIndex(this.buttonOk, 0);
            this.Controls.SetChildIndex(this.checkBoxAutoSampleId, 0);
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

        private CheckBox checkBoxUng;
        private Button buttonOk;
        private CheckBox checkBoxAutoSampleId;
    }
}