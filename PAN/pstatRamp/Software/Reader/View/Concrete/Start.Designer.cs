namespace IO.View.Concrete
{
    partial class Start
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Start));
            this.buttonShutdown = new IO.View.Button();
            this.buttonLogin = new IO.View.Button();
            this.pictureBoxAtlas = new System.Windows.Forms.PictureBox();
            this.pictureBoxIo = new System.Windows.Forms.PictureBox();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAtlas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIo)).BeginInit();
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
            this.labelDate.Text = "12 Sep 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "10:21:14";
            // 
            // buttonShutdown
            // 
            this.buttonShutdown.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonShutdown.Location = new System.Drawing.Point(595, 266);
            this.buttonShutdown.Name = "buttonShutdown";
            this.buttonShutdown.Size = new System.Drawing.Size(180, 40);
            this.buttonShutdown.TabIndex = 11;
            this.buttonShutdown.Text = "Shut Down";
            this.buttonShutdown.Click += new System.EventHandler(this.buttonShutdown_Click);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLogin.Location = new System.Drawing.Point(595, 142);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(180, 40);
            this.buttonLogin.TabIndex = 12;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // pictureBoxAtlas
            // 
            this.pictureBoxAtlas.Image = global::IO.View.Properties.Resources.Atlas;
            this.pictureBoxAtlas.Location = new System.Drawing.Point(20, 345);
            this.pictureBoxAtlas.Name = "pictureBoxAtlas";
            this.pictureBoxAtlas.Size = new System.Drawing.Size(110, 50);
            this.pictureBoxAtlas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxAtlas.TabIndex = 14;
            this.pictureBoxAtlas.TabStop = false;
            // 
            // pictureBoxIo
            // 
            this.pictureBoxIo.Image = global::IO.View.Properties.Resources.IO;
            this.pictureBoxIo.Location = new System.Drawing.Point(55, 70);
            this.pictureBoxIo.Name = "pictureBoxIo";
            this.pictureBoxIo.Size = new System.Drawing.Size(310, 260);
            this.pictureBoxIo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxIo.TabIndex = 13;
            this.pictureBoxIo.TabStop = false;
            // 
            // Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.pictureBoxAtlas);
            this.Controls.Add(this.pictureBoxIo);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.buttonShutdown);
            this.HelpText = resources.GetString("$this.HelpText");
            this.Name = "Start";
            this.Text = "FormSplash";
            this.Controls.SetChildIndex(this.pictureBoxBackground, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.buttonShutdown, 0);
            this.Controls.SetChildIndex(this.buttonLogin, 0);
            this.Controls.SetChildIndex(this.pictureBoxIo, 0);
            this.Controls.SetChildIndex(this.pictureBoxAtlas, 0);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).EndInit();
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAtlas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonShutdown;
        private Button buttonLogin;
        private System.Windows.Forms.PictureBox pictureBoxAtlas;
        private System.Windows.Forms.PictureBox pictureBoxIo;
    }
}