namespace IO.View.Concrete
{
    partial class EditUser
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
            this.textBoxPassword = new IO.View.TextBox();
            this.textBoxVerify = new IO.View.TextBox();
            this.buttonDelete = new IO.View.Button();
            this.radioAdministrator = new IO.View.Radio();
            this.radioUser = new IO.View.Radio();
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
            this.labelDate.Text = "27 Feb 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "14:44:52";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(25, 177);
            this.textBoxUserName.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Password = false;
            this.textBoxUserName.Size = new System.Drawing.Size(750, 40);
            this.textBoxUserName.TabIndex = 7;
            this.textBoxUserName.Title = "User ID";
            this.textBoxUserName.TitleWidth = 165;
            this.textBoxUserName.Click += new System.EventHandler(this.textBoxUserName_Click);
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
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(25, 227);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Password = true;
            this.textBoxPassword.Size = new System.Drawing.Size(750, 40);
            this.textBoxPassword.TabIndex = 10;
            this.textBoxPassword.Title = "Password";
            this.textBoxPassword.TitleWidth = 165;
            this.textBoxPassword.Click += new System.EventHandler(this.textBoxPassword_Click);
            // 
            // textBoxVerify
            // 
            this.textBoxVerify.Location = new System.Drawing.Point(25, 277);
            this.textBoxVerify.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxVerify.Name = "textBoxVerify";
            this.textBoxVerify.Password = true;
            this.textBoxVerify.Size = new System.Drawing.Size(750, 40);
            this.textBoxVerify.TabIndex = 11;
            this.textBoxVerify.Title = "Verify Password";
            this.textBoxVerify.TitleWidth = 165;
            this.textBoxVerify.Click += new System.EventHandler(this.textBoxVerify_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDelete.Location = new System.Drawing.Point(25, 345);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(240, 40);
            this.buttonDelete.TabIndex = 12;
            this.buttonDelete.Text = "Delete Account";
            this.buttonDelete.Visible = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // radioAdministrator
            // 
            this.radioAdministrator.Checked = false;
            this.radioAdministrator.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioAdministrator.Location = new System.Drawing.Point(212, 128);
            this.radioAdministrator.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.radioAdministrator.Name = "radioAdministrator";
            this.radioAdministrator.Size = new System.Drawing.Size(200, 35);
            this.radioAdministrator.TabIndex = 13;
            this.radioAdministrator.Text = "Administrator";
            this.radioAdministrator.Click += new System.EventHandler(this.radioAdministrator_Click);
            // 
            // radioUser
            // 
            this.radioUser.Checked = true;
            this.radioUser.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioUser.Location = new System.Drawing.Point(482, 128);
            this.radioUser.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.radioUser.Name = "radioUser";
            this.radioUser.Size = new System.Drawing.Size(100, 35);
            this.radioUser.TabIndex = 14;
            this.radioUser.Text = "User";
            this.radioUser.Click += new System.EventHandler(this.radioUser_Click);
            // 
            // EditUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.radioUser);
            this.Controls.Add(this.radioAdministrator);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.textBoxVerify);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxUserName);
            this.Name = "EditUser";
            this.Text = "EditUser";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxUserName, 0);
            this.Controls.SetChildIndex(this.buttonSave, 0);
            this.Controls.SetChildIndex(this.textBoxPassword, 0);
            this.Controls.SetChildIndex(this.textBoxVerify, 0);
            this.Controls.SetChildIndex(this.buttonDelete, 0);
            this.Controls.SetChildIndex(this.radioAdministrator, 0);
            this.Controls.SetChildIndex(this.radioUser, 0);
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
        private TextBox textBoxPassword;
        private TextBox textBoxVerify;
        private Button buttonDelete;
        private Radio radioAdministrator;
        private Radio radioUser;
    }
}