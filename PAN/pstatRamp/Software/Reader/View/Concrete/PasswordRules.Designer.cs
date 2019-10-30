namespace IO.View.Concrete
{
    partial class PasswordRules
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
            this.textBoxMinimumLength = new IO.View.TextBox();
            this.buttonSave = new IO.View.Button();
            this.textBoxMinimumAlphabetical = new IO.View.TextBox();
            this.textBoxExpiryTimeInDays = new IO.View.TextBox();
            this.checkBoxOverride = new IO.View.CheckBox();
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
            this.labelTime.Text = "15:40:44";
            // 
            // textBoxMinimumLength
            // 
            this.textBoxMinimumLength.Location = new System.Drawing.Point(25, 125);
            this.textBoxMinimumLength.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxMinimumLength.Name = "textBoxMinimumLength";
            this.textBoxMinimumLength.Password = false;
            this.textBoxMinimumLength.Size = new System.Drawing.Size(750, 40);
            this.textBoxMinimumLength.TabIndex = 10;
            this.textBoxMinimumLength.Title = "Minimum Password Length";
            this.textBoxMinimumLength.TitleWidth = 400;
            this.textBoxMinimumLength.Click += new System.EventHandler(this.textBox_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSave.Location = new System.Drawing.Point(553, 345);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(220, 40);
            this.buttonSave.TabIndex = 14;
            this.buttonSave.Text = "Save Changes";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxMinimumAlphabetical
            // 
            this.textBoxMinimumAlphabetical.Location = new System.Drawing.Point(25, 175);
            this.textBoxMinimumAlphabetical.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxMinimumAlphabetical.Name = "textBoxMinimumAlphabetical";
            this.textBoxMinimumAlphabetical.Password = false;
            this.textBoxMinimumAlphabetical.Size = new System.Drawing.Size(750, 40);
            this.textBoxMinimumAlphabetical.TabIndex = 15;
            this.textBoxMinimumAlphabetical.Title = "Minimum Number Of Letters";
            this.textBoxMinimumAlphabetical.TitleWidth = 400;
            this.textBoxMinimumAlphabetical.Click += new System.EventHandler(this.textBox_Click);
            // 
            // textBoxExpiryTimeInDays
            // 
            this.textBoxExpiryTimeInDays.Location = new System.Drawing.Point(25, 225);
            this.textBoxExpiryTimeInDays.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxExpiryTimeInDays.Name = "textBoxExpiryTimeInDays";
            this.textBoxExpiryTimeInDays.Password = false;
            this.textBoxExpiryTimeInDays.Size = new System.Drawing.Size(750, 40);
            this.textBoxExpiryTimeInDays.TabIndex = 17;
            this.textBoxExpiryTimeInDays.Title = "Password Expiry In Days";
            this.textBoxExpiryTimeInDays.TitleWidth = 400;
            this.textBoxExpiryTimeInDays.Click += new System.EventHandler(this.textBox_Click);
            // 
            // checkBoxOverride
            // 
            this.checkBoxOverride.Checked = false;
            this.checkBoxOverride.Image = global::IO.View.Properties.Resources.Tick;
            this.checkBoxOverride.Location = new System.Drawing.Point(25, 275);
            this.checkBoxOverride.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.checkBoxOverride.Name = "checkBoxOverride";
            this.checkBoxOverride.Size = new System.Drawing.Size(300, 40);
            this.checkBoxOverride.TabIndex = 18;
            this.checkBoxOverride.Text = "Disable Password Rules?";
            this.checkBoxOverride.Click += new System.EventHandler(this.checkBoxOverride_Click);
            // 
            // PasswordRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.checkBoxOverride);
            this.Controls.Add(this.textBoxExpiryTimeInDays);
            this.Controls.Add(this.textBoxMinimumAlphabetical);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxMinimumLength);
            this.HelpText = "Choose rules that meet your organisation\'s security policy or needs.\r\n\r\nIf necess" +
                "ary, the need for a password can be disabled.";
            this.Name = "PasswordRules";
            this.Text = "PasswordRules";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxMinimumLength, 0);
            this.Controls.SetChildIndex(this.buttonSave, 0);
            this.Controls.SetChildIndex(this.textBoxMinimumAlphabetical, 0);
            this.Controls.SetChildIndex(this.textBoxExpiryTimeInDays, 0);
            this.Controls.SetChildIndex(this.checkBoxOverride, 0);
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

        private TextBox textBoxMinimumLength;
        private Button buttonSave;
        private TextBox textBoxMinimumAlphabetical;
        private TextBox textBoxExpiryTimeInDays;
        private CheckBox checkBoxOverride;
    }
}