namespace IO.View.Concrete
{
    partial class EmptyDrawer
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
            this.illustration = new IO.View.Illustration();
            this.label = new System.Windows.Forms.Label();
            this.buttonContinue = new IO.View.Button();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
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
            this.labelDate.Text = "13 Sep 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "15:20:52";
            // 
            // illustration
            // 
            this.illustration.BackColor = System.Drawing.Color.Transparent;
            this.illustration.Image = global::IO.View.Properties.Resources.Instrument_drawer_open;
            this.illustration.Location = new System.Drawing.Point(25, 125);
            this.illustration.Name = "illustration";
            this.illustration.Size = new System.Drawing.Size(750, 200);
            this.illustration.TabIndex = 15;
            this.illustration.TabStop = false;
            this.illustration.Text = "illustration";
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(410, 135);
            this.label.Margin = new System.Windows.Forms.Padding(5);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(300, 180);
            this.label.TabIndex = 16;
            this.label.Text = "Text";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonContinue
            // 
            this.buttonContinue.BackColor = System.Drawing.Color.Transparent;
            this.buttonContinue.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold);
            this.buttonContinue.Location = new System.Drawing.Point(25, 342);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(160, 40);
            this.buttonContinue.TabIndex = 17;
            this.buttonContinue.Text = "Continue";
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // EmptyDrawer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.label);
            this.Controls.Add(this.illustration);
            this.HelpText = "The Cartridge drawer must be closed to allow the Reader to reset after running a " +
                "Test.\r\nClosing the drawer allows the internal components to move back to the cor" +
                "rect position\r\nto start a new Test.";
            this.Name = "EmptyDrawer";
            this.Text = "CloseDrawer";
            this.Controls.SetChildIndex(this.pictureBoxBackground, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.illustration, 0);
            this.Controls.SetChildIndex(this.label, 0);
            this.Controls.SetChildIndex(this.buttonContinue, 0);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).EndInit();
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Illustration illustration;
        private System.Windows.Forms.Label label;
        private Button buttonContinue;

    }
}