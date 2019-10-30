namespace IO.View.Concrete
{
    partial class PrintOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintOptions));
            this.buttonPrintHardCopy = new IO.View.Button();
            this.buttonPrintToFile = new IO.View.Button();
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
            this.labelTime.Text = "16:35:19";
            // 
            // buttonPrintHardCopy
            // 
            this.buttonPrintHardCopy.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonPrintHardCopy.Location = new System.Drawing.Point(289, 182);
            this.buttonPrintHardCopy.Name = "buttonPrintHardCopy";
            this.buttonPrintHardCopy.Size = new System.Drawing.Size(250, 40);
            this.buttonPrintHardCopy.TabIndex = 9;
            this.buttonPrintHardCopy.Text = "Print Hard Copy";
            this.buttonPrintHardCopy.Click += new System.EventHandler(this.buttonPrintHardCopy_Click);
            // 
            // buttonPrintToFile
            // 
            this.buttonPrintToFile.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonPrintToFile.Location = new System.Drawing.Point(289, 250);
            this.buttonPrintToFile.Name = "buttonPrintToFile";
            this.buttonPrintToFile.Size = new System.Drawing.Size(250, 40);
            this.buttonPrintToFile.TabIndex = 10;
            this.buttonPrintToFile.Text = "Print To File";
            this.buttonPrintToFile.Click += new System.EventHandler(this.buttonPrintToFile_Click);
            // 
            // PrintOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonPrintToFile);
            this.Controls.Add(this.buttonPrintHardCopy);
            this.HelpText = resources.GetString("$this.HelpText");
            this.Name = "PrintOptions";
            this.Text = "AddNewAssay";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.buttonPrintHardCopy, 0);
            this.Controls.SetChildIndex(this.buttonPrintToFile, 0);
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

        private Button buttonPrintHardCopy;
        private Button buttonPrintToFile;
    }
}