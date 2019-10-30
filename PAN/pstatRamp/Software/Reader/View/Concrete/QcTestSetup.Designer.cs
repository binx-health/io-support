namespace IO.View.Concrete
{
    partial class QcTestSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QcTestSetup));
            this.buttonOk = new IO.View.Button();
            this.comboBoxFrequency = new IO.View.ComboBox();
            this.listBoxFrequency = new IO.View.ListBox();
            this.comboBoxQuarantineState = new IO.View.ComboBox();
            this.listBoxQuarantineState = new IO.View.ListBox();
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
            this.labelTime.Text = "17:03:26";
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(613, 345);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(160, 40);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxFrequency
            // 
            this.comboBoxFrequency.Location = new System.Drawing.Point(25, 125);
            this.comboBoxFrequency.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.comboBoxFrequency.Name = "comboBoxFrequency";
            this.comboBoxFrequency.Size = new System.Drawing.Size(750, 40);
            this.comboBoxFrequency.TabIndex = 9;
            this.comboBoxFrequency.Text = "One";
            this.comboBoxFrequency.Title = "Test Frequency";
            this.comboBoxFrequency.Click += new System.EventHandler(this.comboBoxFrequency_Click);
            // 
            // listBoxFrequency
            // 
            this.listBoxFrequency.Location = new System.Drawing.Point(190, 163);
            this.listBoxFrequency.Margin = new System.Windows.Forms.Padding(0);
            this.listBoxFrequency.Name = "listBoxFrequency";
            this.listBoxFrequency.Size = new System.Drawing.Size(585, 150);
            this.listBoxFrequency.TabIndex = 10;
            this.listBoxFrequency.Values = ((System.Collections.Generic.IEnumerable<string>)(resources.GetObject("listBoxFrequency.Values")));
            this.listBoxFrequency.Visible = false;
            this.listBoxFrequency.Click += new System.EventHandler(this.listBoxFrequency_Click);
            // 
            // comboBoxQuarantineState
            // 
            this.comboBoxQuarantineState.Location = new System.Drawing.Point(25, 170);
            this.comboBoxQuarantineState.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.comboBoxQuarantineState.Name = "comboBoxQuarantineState";
            this.comboBoxQuarantineState.Size = new System.Drawing.Size(750, 40);
            this.comboBoxQuarantineState.TabIndex = 11;
            this.comboBoxQuarantineState.Text = "One";
            this.comboBoxQuarantineState.Title = "Quarantine State";
            this.comboBoxQuarantineState.Click += new System.EventHandler(this.comboBoxQuarantineState_Click);
            // 
            // listBoxQuarantineState
            // 
            this.listBoxQuarantineState.Location = new System.Drawing.Point(190, 208);
            this.listBoxQuarantineState.Margin = new System.Windows.Forms.Padding(0);
            this.listBoxQuarantineState.Name = "listBoxQuarantineState";
            this.listBoxQuarantineState.Size = new System.Drawing.Size(585, 150);
            this.listBoxQuarantineState.TabIndex = 12;
            this.listBoxQuarantineState.Values = ((System.Collections.Generic.IEnumerable<string>)(resources.GetObject("listBoxQuarantineState.Values")));
            this.listBoxQuarantineState.Visible = false;
            this.listBoxQuarantineState.Click += new System.EventHandler(this.listBoxQuarantineState_Click);
            // 
            // QcTestSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.listBoxQuarantineState);
            this.Controls.Add(this.listBoxFrequency);
            this.Controls.Add(this.comboBoxQuarantineState);
            this.Controls.Add(this.comboBoxFrequency);
            this.Controls.Add(this.buttonOk);
            this.HelpText = resources.GetString("$this.HelpText");
            this.Name = "QcTestSetup";
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.buttonOk, 0);
            this.Controls.SetChildIndex(this.comboBoxFrequency, 0);
            this.Controls.SetChildIndex(this.comboBoxQuarantineState, 0);
            this.Controls.SetChildIndex(this.listBoxFrequency, 0);
            this.Controls.SetChildIndex(this.listBoxQuarantineState, 0);
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
        private ComboBox comboBoxFrequency;
        private ListBox listBoxFrequency;
        private ComboBox comboBoxQuarantineState;
        private ListBox listBoxQuarantineState;

    }
}