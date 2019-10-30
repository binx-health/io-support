﻿namespace IO.View.Concrete
{
    partial class PatientInformation
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
            this.textBox1 = new IO.View.TextBox();
            this.scrollBar = new IO.View.ScrollBar();
            this.textBox2 = new IO.View.TextBox();
            this.textBox3 = new IO.View.TextBox();
            this.textBox4 = new IO.View.TextBox();
            this.buttonOk = new IO.View.Button();
            this.buttonCancel = new IO.View.Button();
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
            this.labelTime.Text = "14:46:18";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(25, 125);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Password = false;
            this.textBox1.Size = new System.Drawing.Size(700, 40);
            this.textBox1.TabIndex = 11;
            this.textBox1.Tag = "0";
            this.textBox1.Title = "";
            this.textBox1.TitleWidth = 200;
            this.textBox1.Click += new System.EventHandler(this.textBox_Click);
            // 
            // scrollBar
            // 
            this.scrollBar.Location = new System.Drawing.Point(735, 125);
            this.scrollBar.Margin = new System.Windows.Forms.Padding(0);
            this.scrollBar.Maximum = 0;
            this.scrollBar.Name = "scrollBar";
            this.scrollBar.Position = 0;
            this.scrollBar.Size = new System.Drawing.Size(40, 190);
            this.scrollBar.Step = 5;
            this.scrollBar.TabIndex = 12;
            this.scrollBar.Text = "scrollBar1";
            this.scrollBar.Click += new System.EventHandler(this.scrollBar_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(25, 175);
            this.textBox2.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Password = false;
            this.textBox2.Size = new System.Drawing.Size(700, 40);
            this.textBox2.TabIndex = 13;
            this.textBox2.Tag = "1";
            this.textBox2.Title = "";
            this.textBox2.TitleWidth = 200;
            this.textBox2.Click += new System.EventHandler(this.textBox_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(25, 225);
            this.textBox3.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Password = false;
            this.textBox3.Size = new System.Drawing.Size(700, 40);
            this.textBox3.TabIndex = 14;
            this.textBox3.Tag = "2";
            this.textBox3.Title = "";
            this.textBox3.TitleWidth = 200;
            this.textBox3.Click += new System.EventHandler(this.textBox_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(25, 275);
            this.textBox4.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBox4.Name = "textBox4";
            this.textBox4.Password = false;
            this.textBox4.Size = new System.Drawing.Size(700, 40);
            this.textBox4.TabIndex = 15;
            this.textBox4.Tag = "3";
            this.textBox4.Title = "";
            this.textBox4.TitleWidth = 200;
            this.textBox4.Click += new System.EventHandler(this.textBox_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(615, 345);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(160, 40);
            this.buttonOk.TabIndex = 17;
            this.buttonOk.Text = "Ok";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(25, 345);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(160, 40);
            this.buttonCancel.TabIndex = 18;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // PatientInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.scrollBar);
            this.Controls.Add(this.textBox1);
            this.HelpText = "Enter the patient details. The entry details that are editable may be\r\nset for th" +
                "is Reader by the local Administrator.\r\n";
            this.Name = "PatientInformation";
            this.Text = "PatientInformation";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBox1, 0);
            this.Controls.SetChildIndex(this.scrollBar, 0);
            this.Controls.SetChildIndex(this.textBox2, 0);
            this.Controls.SetChildIndex(this.textBox3, 0);
            this.Controls.SetChildIndex(this.textBox4, 0);
            this.Controls.SetChildIndex(this.buttonOk, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
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

        private TextBox textBox1;
        private ScrollBar scrollBar;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Button buttonOk;
        private Button buttonCancel;
    }
}