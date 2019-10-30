namespace IO.View.Concrete
{
    partial class UserManagement
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
            this.buttonTimeout = new IO.View.Button();
            this.buttonPasswordRules = new IO.View.Button();
            this.buttonUserAccount = new IO.View.Button();
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
            this.labelTime.Text = "15:36:33";
            // 
            // buttonTimeout
            // 
            this.buttonTimeout.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTimeout.Location = new System.Drawing.Point(275, 270);
            this.buttonTimeout.Margin = new System.Windows.Forms.Padding(15);
            this.buttonTimeout.Name = "buttonTimeout";
            this.buttonTimeout.Size = new System.Drawing.Size(250, 40);
            this.buttonTimeout.TabIndex = 11;
            this.buttonTimeout.Text = "Timeout";
            this.buttonTimeout.Click += new System.EventHandler(this.buttonTimeout_Click);
            // 
            // buttonPasswordRules
            // 
            this.buttonPasswordRules.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPasswordRules.Location = new System.Drawing.Point(275, 200);
            this.buttonPasswordRules.Margin = new System.Windows.Forms.Padding(15);
            this.buttonPasswordRules.Name = "buttonPasswordRules";
            this.buttonPasswordRules.Size = new System.Drawing.Size(250, 40);
            this.buttonPasswordRules.TabIndex = 10;
            this.buttonPasswordRules.Text = "Password Rules";
            this.buttonPasswordRules.Click += new System.EventHandler(this.buttonPasswordRules_Click);
            // 
            // buttonUserAccount
            // 
            this.buttonUserAccount.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUserAccount.Location = new System.Drawing.Point(275, 130);
            this.buttonUserAccount.Margin = new System.Windows.Forms.Padding(15);
            this.buttonUserAccount.Name = "buttonUserAccount";
            this.buttonUserAccount.Size = new System.Drawing.Size(250, 40);
            this.buttonUserAccount.TabIndex = 9;
            this.buttonUserAccount.Text = "User Account";
            this.buttonUserAccount.Click += new System.EventHandler(this.buttonUserAccount_Click);
            // 
            // UserManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonTimeout);
            this.Controls.Add(this.buttonPasswordRules);
            this.Controls.Add(this.buttonUserAccount);
            this.HelpText = "User account, password security and Reader timeout settings.";
            this.Name = "UserManagement";
            this.Text = "UserManagement";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.buttonUserAccount, 0);
            this.Controls.SetChildIndex(this.buttonPasswordRules, 0);
            this.Controls.SetChildIndex(this.buttonTimeout, 0);
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

        private Button buttonTimeout;
        private Button buttonPasswordRules;
        private Button buttonUserAccount;
    }
}