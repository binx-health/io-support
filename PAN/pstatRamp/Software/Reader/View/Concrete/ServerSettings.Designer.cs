namespace IO.View.Concrete
{
    partial class ServerSettings
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
            this.textBoxServerAddress = new IO.View.TextBox();
            this.buttonSave = new IO.View.Button();
            this.textBoxServerPort = new IO.View.TextBox();
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
            this.labelTime.Text = "16:49:13";
            // 
            // textBoxServerAddress
            // 
            this.textBoxServerAddress.Location = new System.Drawing.Point(25, 125);
            this.textBoxServerAddress.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxServerAddress.Name = "textBoxServerAddress";
            this.textBoxServerAddress.Password = false;
            this.textBoxServerAddress.Size = new System.Drawing.Size(750, 40);
            this.textBoxServerAddress.TabIndex = 10;
            this.textBoxServerAddress.Title = "Server Address";
            this.textBoxServerAddress.TitleWidth = 400;
            this.textBoxServerAddress.Click += new System.EventHandler(this.textBoxServerAddress_Click);
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
            // textBoxServerPort
            // 
            this.textBoxServerPort.Location = new System.Drawing.Point(25, 175);
            this.textBoxServerPort.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxServerPort.Name = "textBoxServerPort";
            this.textBoxServerPort.Password = false;
            this.textBoxServerPort.Size = new System.Drawing.Size(750, 40);
            this.textBoxServerPort.TabIndex = 15;
            this.textBoxServerPort.Title = "Server Port";
            this.textBoxServerPort.TitleWidth = 400;
            this.textBoxServerPort.Click += new System.EventHandler(this.textBoxServerPort_Click);
            // 
            // ServerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.textBoxServerPort);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxServerAddress);
            this.HelpText = "Enter the Server Address and Server Port details necessary to connect\r\nto your ne" +
                "twork environment.";
            this.Name = "ServerSettings";
            this.Text = "PasswordRules";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxServerAddress, 0);
            this.Controls.SetChildIndex(this.buttonSave, 0);
            this.Controls.SetChildIndex(this.textBoxServerPort, 0);
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

        private TextBox textBoxServerAddress;
        private Button buttonSave;
        private TextBox textBoxServerPort;
    }
}