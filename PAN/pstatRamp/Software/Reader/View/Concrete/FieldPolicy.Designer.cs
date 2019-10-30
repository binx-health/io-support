﻿namespace IO.View.Concrete
{
    partial class FieldPolicy
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
            this.checkBoxDisplay = new IO.View.CheckBox();
            this.checkBoxRecord = new IO.View.CheckBox();
            this.checkBoxIgnore = new IO.View.CheckBox();
            this.buttonOk = new IO.View.Button();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBoxDisplay
            // 
            this.checkBoxDisplay.Checked = false;
            this.checkBoxDisplay.Image = global::IO.View.Properties.Resources.Tick;
            this.checkBoxDisplay.Location = new System.Drawing.Point(306, 151);
            this.checkBoxDisplay.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.checkBoxDisplay.Name = "checkBoxDisplay";
            this.checkBoxDisplay.Size = new System.Drawing.Size(200, 40);
            this.checkBoxDisplay.TabIndex = 7;
            this.checkBoxDisplay.Text = "Display";
            this.checkBoxDisplay.Click += new System.EventHandler(this.checkBox_Click);
            // 
            // checkBoxRecord
            // 
            this.checkBoxRecord.Checked = false;
            this.checkBoxRecord.Image = global::IO.View.Properties.Resources.Tick;
            this.checkBoxRecord.Location = new System.Drawing.Point(306, 201);
            this.checkBoxRecord.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.checkBoxRecord.Name = "checkBoxRecord";
            this.checkBoxRecord.Size = new System.Drawing.Size(200, 40);
            this.checkBoxRecord.TabIndex = 8;
            this.checkBoxRecord.Text = "Record";
            this.checkBoxRecord.Click += new System.EventHandler(this.checkBox_Click);
            // 
            // checkBoxIgnore
            // 
            this.checkBoxIgnore.Checked = false;
            this.checkBoxIgnore.Image = global::IO.View.Properties.Resources.Tick;
            this.checkBoxIgnore.Location = new System.Drawing.Point(306, 251);
            this.checkBoxIgnore.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.checkBoxIgnore.Name = "checkBoxIgnore";
            this.checkBoxIgnore.Size = new System.Drawing.Size(200, 40);
            this.checkBoxIgnore.TabIndex = 9;
            this.checkBoxIgnore.Text = "Ignore";
            this.checkBoxIgnore.Click += new System.EventHandler(this.checkBox_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(613, 345);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(160, 40);
            this.buttonOk.TabIndex = 10;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // FieldPolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxIgnore);
            this.Controls.Add(this.checkBoxRecord);
            this.Controls.Add(this.checkBoxDisplay);
            this.Name = "FieldPolicy";
            this.Text = "FieldPolicy";
            this.Controls.SetChildIndex(this.checkBoxDisplay, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.checkBoxRecord, 0);
            this.Controls.SetChildIndex(this.checkBoxIgnore, 0);
            this.Controls.SetChildIndex(this.buttonOk, 0);
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

        private CheckBox checkBoxDisplay;
        private CheckBox checkBoxRecord;
        private CheckBox checkBoxIgnore;
        private Button buttonOk;
    }
}