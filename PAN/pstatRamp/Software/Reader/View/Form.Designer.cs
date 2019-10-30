namespace IO.View
{
    partial class Form
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            this.panelBottom = new System.Windows.Forms.Panel();
            this.pictureBoxHome = new System.Windows.Forms.PictureBox();
            this.pictureBoxLogin = new System.Windows.Forms.PictureBox();
            this.pictureBoxHelp = new System.Windows.Forms.PictureBox();
            this.pictureBoxBack = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelDate = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelUser = new System.Windows.Forms.Label();
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxAnimation = new System.Windows.Forms.PictureBox();
            this.timerWait = new System.Windows.Forms.Timer(this.components);
            this.titleBar = new IO.View.Title();
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(32)))), ((int)(((byte)(128)))));
            this.panelBottom.Controls.Add(this.pictureBoxHome);
            this.panelBottom.Controls.Add(this.pictureBoxLogin);
            this.panelBottom.Controls.Add(this.pictureBoxHelp);
            this.panelBottom.Controls.Add(this.pictureBoxBack);
            this.panelBottom.Controls.Add(this.labelStatus);
            resources.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            // 
            // pictureBoxHome
            // 
            resources.ApplyResources(this.pictureBoxHome, "pictureBoxHome");
            this.pictureBoxHome.Name = "pictureBoxHome";
            this.pictureBoxHome.TabStop = false;
            this.pictureBoxHome.Click += new System.EventHandler(this.pictureBoxHome_Click);
            this.pictureBoxHome.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBoxHome.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // pictureBoxLogin
            // 
            resources.ApplyResources(this.pictureBoxLogin, "pictureBoxLogin");
            this.pictureBoxLogin.Name = "pictureBoxLogin";
            this.pictureBoxLogin.TabStop = false;
            this.pictureBoxLogin.Click += new System.EventHandler(this.pictureBoxLogin_Click);
            this.pictureBoxLogin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBoxLogin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // pictureBoxHelp
            // 
            resources.ApplyResources(this.pictureBoxHelp, "pictureBoxHelp");
            this.pictureBoxHelp.Name = "pictureBoxHelp";
            this.pictureBoxHelp.TabStop = false;
            this.pictureBoxHelp.Click += new System.EventHandler(this.pictureBoxHelp_Click);
            this.pictureBoxHelp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBoxHelp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // pictureBoxBack
            // 
            this.pictureBoxBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(32)))), ((int)(((byte)(128)))));
            resources.ApplyResources(this.pictureBoxBack, "pictureBoxBack");
            this.pictureBoxBack.Name = "pictureBoxBack";
            this.pictureBoxBack.TabStop = false;
            this.pictureBoxBack.Click += new System.EventHandler(this.pictureBoxBack_Click);
            this.pictureBoxBack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBoxBack.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // labelStatus
            // 
            this.labelStatus.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelStatus, "labelStatus");
            this.labelStatus.ForeColor = System.Drawing.Color.White;
            this.labelStatus.Name = "labelStatus";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(69)))), ((int)(((byte)(67)))));
            this.panelTop.Controls.Add(this.labelDate);
            this.panelTop.Controls.Add(this.labelTime);
            this.panelTop.Controls.Add(this.labelUser);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            // 
            // labelDate
            // 
            this.labelDate.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelDate, "labelDate");
            this.labelDate.ForeColor = System.Drawing.Color.White;
            this.labelDate.Name = "labelDate";
            // 
            // labelTime
            // 
            this.labelTime.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelTime, "labelTime");
            this.labelTime.ForeColor = System.Drawing.Color.White;
            this.labelTime.Name = "labelTime";
            // 
            // labelUser
            // 
            this.labelUser.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelUser, "labelUser");
            this.labelUser.ForeColor = System.Drawing.Color.White;
            this.labelUser.Name = "labelUser";
            // 
            // timerClock
            // 
            this.timerClock.Enabled = true;
            this.timerClock.Interval = 1000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // pictureBoxAnimation
            // 
            this.pictureBoxAnimation.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBoxAnimation, "pictureBoxAnimation");
            this.pictureBoxAnimation.Image = global::IO.View.Properties.Resources.io_processing;
            this.pictureBoxAnimation.Name = "pictureBoxAnimation";
            this.pictureBoxAnimation.TabStop = false;
            // 
            // timerWait
            // 
            this.timerWait.Interval = 500;
            this.timerWait.Tick += new System.EventHandler(this.timerWait_Tick);
            // 
            // titleBar
            // 
            resources.ApplyResources(this.titleBar, "titleBar");
            this.titleBar.Name = "titleBar";
            this.titleBar.TabStop = false;
            // 
            // pictureBoxBackground
            // 
            resources.ApplyResources(this.pictureBoxBackground, "pictureBoxBackground");
            this.pictureBoxBackground.Name = "pictureBoxBackground";
            this.pictureBoxBackground.TabStop = false;
            // 
            // Form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.titleBar);
            this.Controls.Add(this.pictureBoxAnimation);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.pictureBoxBackground);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form_Load);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).EndInit();
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Panel panelBottom;
        protected System.Windows.Forms.PictureBox pictureBoxBack;
        protected System.Windows.Forms.Label labelStatus;
        protected System.Windows.Forms.PictureBox pictureBoxLogin;
        protected System.Windows.Forms.PictureBox pictureBoxHelp;
        protected System.Windows.Forms.PictureBox pictureBoxHome;
        protected System.Windows.Forms.Panel panelTop;
        protected System.Windows.Forms.Label labelUser;
        protected System.Windows.Forms.Timer timerClock;
        protected System.Windows.Forms.Label labelDate;
        protected System.Windows.Forms.Label labelTime;
        protected System.Windows.Forms.PictureBox pictureBoxAnimation;
        protected System.Windows.Forms.Timer timerWait;
        protected Title titleBar;
        protected System.Windows.Forms.PictureBox pictureBoxBackground;
    }
}