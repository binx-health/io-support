﻿namespace IO.View.Concrete
{
    partial class Fault
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
            this.panelMessage = new IO.View.Panel();
            this.buttonShutdown = new IO.View.Button();
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
            this.labelDate.Text = "15 Apr 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "16:10:51";
            // 
            // panelMessage
            // 
            this.panelMessage.ForeColor = System.Drawing.Color.Black;
            this.panelMessage.Location = new System.Drawing.Point(25, 125);
            this.panelMessage.Margin = new System.Windows.Forms.Padding(5);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Size = new System.Drawing.Size(750, 200);
            this.panelMessage.TabIndex = 9;
            // 
            // buttonShutdown
            // 
            this.buttonShutdown.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonShutdown.Location = new System.Drawing.Point(595, 345);
            this.buttonShutdown.Name = "buttonShutdown";
            this.buttonShutdown.Size = new System.Drawing.Size(180, 40);
            this.buttonShutdown.TabIndex = 12;
            this.buttonShutdown.Text = "Shut Down";
            this.buttonShutdown.Click += new System.EventHandler(this.buttonShutdown_Click);
            // 
            // Fault
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonShutdown);
            this.Controls.Add(this.panelMessage);
            this.Name = "Fault";
            this.Text = "Fault";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.panelMessage, 0);
            this.Controls.SetChildIndex(this.buttonShutdown, 0);
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

        private Panel panelMessage;
        private Button buttonShutdown;
    }
}