namespace IO.View.Concrete
{
    partial class UpdatePassword
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
            this.textBoxUserName = new IO.View.TextBox();
            this.buttonSave = new IO.View.Button();
            this.textBoxNewPassword = new IO.View.TextBox();
            this.textBoxVerify = new IO.View.TextBox();
            this.buttonCancel = new IO.View.Button();
            this.textBoxOldPassword = new IO.View.TextBox();
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
            this.labelTime.Text = "16:39:25";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(25, 125);
            this.textBoxUserName.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Password = false;
            this.textBoxUserName.Size = new System.Drawing.Size(750, 40);
            this.textBoxUserName.TabIndex = 7;
            this.textBoxUserName.Title = "User ID";
            this.textBoxUserName.TitleWidth = 165;
            // 
            // buttonSave
            // 
            this.buttonSave.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(533, 345);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(240, 40);
            this.buttonSave.TabIndex = 9;
            this.buttonSave.Text = "Save Changes";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxNewPassword
            // 
            this.textBoxNewPassword.Location = new System.Drawing.Point(25, 225);
            this.textBoxNewPassword.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxNewPassword.Name = "textBoxNewPassword";
            this.textBoxNewPassword.Password = true;
            this.textBoxNewPassword.Size = new System.Drawing.Size(750, 40);
            this.textBoxNewPassword.TabIndex = 10;
            this.textBoxNewPassword.Title = "New Password";
            this.textBoxNewPassword.TitleWidth = 165;
            this.textBoxNewPassword.Click += new System.EventHandler(this.textBoxNewPassword_Click);
            // 
            // textBoxVerify
            // 
            this.textBoxVerify.Location = new System.Drawing.Point(25, 275);
            this.textBoxVerify.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxVerify.Name = "textBoxVerify";
            this.textBoxVerify.Password = true;
            this.textBoxVerify.Size = new System.Drawing.Size(750, 40);
            this.textBoxVerify.TabIndex = 11;
            this.textBoxVerify.Title = "Verify Password";
            this.textBoxVerify.TitleWidth = 165;
            this.textBoxVerify.Click += new System.EventHandler(this.textBoxVerify_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(25, 345);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(160, 40);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Visible = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxOldPassword
            // 
            this.textBoxOldPassword.Location = new System.Drawing.Point(25, 175);
            this.textBoxOldPassword.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxOldPassword.Name = "textBoxOldPassword";
            this.textBoxOldPassword.Password = true;
            this.textBoxOldPassword.Size = new System.Drawing.Size(750, 40);
            this.textBoxOldPassword.TabIndex = 13;
            this.textBoxOldPassword.Title = "Old Password";
            this.textBoxOldPassword.TitleWidth = 165;
            this.textBoxOldPassword.Click += new System.EventHandler(this.textBoxOldPassword_Click);
            // 
            // UpdatePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.textBoxOldPassword);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxVerify);
            this.Controls.Add(this.textBoxNewPassword);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxUserName);
            this.HelpText = "Please create a new password using the data entry boxes.";
            this.Name = "UpdatePassword";
            this.Text = "EditUser";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxUserName, 0);
            this.Controls.SetChildIndex(this.buttonSave, 0);
            this.Controls.SetChildIndex(this.textBoxNewPassword, 0);
            this.Controls.SetChildIndex(this.textBoxVerify, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.textBoxOldPassword, 0);
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

        private TextBox textBoxUserName;
        private Button buttonSave;
        private TextBox textBoxNewPassword;
        private TextBox textBoxVerify;
        private Button buttonCancel;
        private TextBox textBoxOldPassword;
    }
}