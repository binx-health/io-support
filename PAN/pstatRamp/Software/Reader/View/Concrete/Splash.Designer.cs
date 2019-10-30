namespace IO.View.Concrete
{
    partial class Splash
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
            this.labelLoading = new System.Windows.Forms.Label();
            this.pictureBoxIo = new System.Windows.Forms.PictureBox();
            this.pictureBoxAtlas = new System.Windows.Forms.PictureBox();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAtlas)).BeginInit();
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
            this.labelTime.Text = "10:50:50";
            // 
            // labelLoading
            // 
            this.labelLoading.AutoSize = true;
            this.labelLoading.BackColor = System.Drawing.Color.Transparent;
            this.labelLoading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(35)))), ((int)(((byte)(106)))));
            this.labelLoading.Location = new System.Drawing.Point(577, 311);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(84, 19);
            this.labelLoading.TabIndex = 4;
            this.labelLoading.Text = "Loading...";
            // 
            // pictureBoxIo
            // 
            this.pictureBoxIo.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxIo.Image = global::IO.View.Properties.Resources.IO;
            this.pictureBoxIo.Location = new System.Drawing.Point(55, 70);
            this.pictureBoxIo.Name = "pictureBoxIo";
            this.pictureBoxIo.Size = new System.Drawing.Size(310, 260);
            this.pictureBoxIo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxIo.TabIndex = 10;
            this.pictureBoxIo.TabStop = false;
            // 
            // pictureBoxAtlas
            // 
            this.pictureBoxAtlas.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxAtlas.Image = global::IO.View.Properties.Resources.Atlas;
            this.pictureBoxAtlas.Location = new System.Drawing.Point(20, 345);
            this.pictureBoxAtlas.Name = "pictureBoxAtlas";
            this.pictureBoxAtlas.Size = new System.Drawing.Size(110, 50);
            this.pictureBoxAtlas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxAtlas.TabIndex = 11;
            this.pictureBoxAtlas.TabStop = false;
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.pictureBoxAtlas);
            this.Controls.Add(this.pictureBoxIo);
            this.Controls.Add(this.labelLoading);
            this.Name = "Splash";
            this.Text = "FormSplash";
            this.Load += new System.EventHandler(this.FormSplash_Load);
            this.Controls.SetChildIndex(this.pictureBoxBackground, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.labelLoading, 0);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAtlas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelLoading;
        private System.Windows.Forms.PictureBox pictureBoxIo;
        private System.Windows.Forms.PictureBox pictureBoxAtlas;
    }
}