namespace IO.View.Concrete
{
    partial class Keypad
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
            this.textBox = new IO.View.TextBox();
            this.keyClear = new IO.View.Key();
            this.keyBackspace = new IO.View.Key();
            this.keyExit = new IO.View.Key();
            this.keyEnter = new IO.View.Key();
            this.keySpace = new IO.View.Key();
            this.key1 = new IO.View.Key();
            this.key2 = new IO.View.Key();
            this.key3 = new IO.View.Key();
            this.key4 = new IO.View.Key();
            this.key5 = new IO.View.Key();
            this.key6 = new IO.View.Key();
            this.key7 = new IO.View.Key();
            this.key8 = new IO.View.Key();
            this.key9 = new IO.View.Key();
            this.key0 = new IO.View.Key();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHome)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(27, 78);
            this.textBox.Margin = new System.Windows.Forms.Padding(4);
            this.textBox.Name = "textBox";
            this.textBox.Password = false;
            this.textBox.Size = new System.Drawing.Size(600, 40);
            this.textBox.TabIndex = 3;
            this.textBox.TabStop = false;
            this.textBox.Title = null;
            this.textBox.TitleWidth = 165;
            // 
            // keyClear
            // 
            this.keyClear.Location = new System.Drawing.Point(635, 78);
            this.keyClear.Margin = new System.Windows.Forms.Padding(4);
            this.keyClear.Name = "keyClear";
            this.keyClear.Size = new System.Drawing.Size(130, 40);
            this.keyClear.TabIndex = 4;
            this.keyClear.TabStop = false;
            this.keyClear.Text = "Clear";
            this.keyClear.Toggled = false;
            this.keyClear.Click += new System.EventHandler(this.keyClear_Click);
            // 
            // keyBackspace
            // 
            this.keyBackspace.Location = new System.Drawing.Point(693, 138);
            this.keyBackspace.Margin = new System.Windows.Forms.Padding(4);
            this.keyBackspace.Name = "keyBackspace";
            this.keyBackspace.Size = new System.Drawing.Size(94, 40);
            this.keyBackspace.TabIndex = 5;
            this.keyBackspace.TabStop = false;
            this.keyBackspace.Text = "<";
            this.keyBackspace.Toggled = false;
            this.keyBackspace.Click += new System.EventHandler(this.keyBackspace_Click);
            // 
            // keyExit
            // 
            this.keyExit.Location = new System.Drawing.Point(31, 330);
            this.keyExit.Margin = new System.Windows.Forms.Padding(4);
            this.keyExit.Name = "keyExit";
            this.keyExit.Size = new System.Drawing.Size(124, 40);
            this.keyExit.TabIndex = 6;
            this.keyExit.TabStop = false;
            this.keyExit.Text = "Exit";
            this.keyExit.Toggled = false;
            this.keyExit.Click += new System.EventHandler(this.keyExit_Click);
            // 
            // keyEnter
            // 
            this.keyEnter.Location = new System.Drawing.Point(641, 330);
            this.keyEnter.Margin = new System.Windows.Forms.Padding(4);
            this.keyEnter.Name = "keyEnter";
            this.keyEnter.Size = new System.Drawing.Size(124, 40);
            this.keyEnter.TabIndex = 7;
            this.keyEnter.TabStop = false;
            this.keyEnter.Text = "Enter";
            this.keyEnter.Toggled = false;
            this.keyEnter.Click += new System.EventHandler(this.keyEnter_Click);
            // 
            // keySpace
            // 
            this.keySpace.Location = new System.Drawing.Point(163, 330);
            this.keySpace.Margin = new System.Windows.Forms.Padding(4);
            this.keySpace.Name = "keySpace";
            this.keySpace.Size = new System.Drawing.Size(470, 40);
            this.keySpace.TabIndex = 8;
            this.keySpace.TabStop = false;
            this.keySpace.Text = "Space";
            this.keySpace.Toggled = false;
            this.keySpace.Click += new System.EventHandler(this.keySpace_Click);
            // 
            // key1
            // 
            this.key1.Location = new System.Drawing.Point(299, 138);
            this.key1.Margin = new System.Windows.Forms.Padding(4);
            this.key1.Name = "key1";
            this.key1.Size = new System.Drawing.Size(60, 40);
            this.key1.TabIndex = 22;
            this.key1.TabStop = false;
            this.key1.Text = "1";
            this.key1.Toggled = false;
            this.key1.Click += new System.EventHandler(this.key_Click);
            // 
            // key2
            // 
            this.key2.Location = new System.Drawing.Point(367, 138);
            this.key2.Margin = new System.Windows.Forms.Padding(4);
            this.key2.Name = "key2";
            this.key2.Size = new System.Drawing.Size(60, 40);
            this.key2.TabIndex = 23;
            this.key2.TabStop = false;
            this.key2.Text = "2";
            this.key2.Toggled = false;
            this.key2.Click += new System.EventHandler(this.key_Click);
            // 
            // key3
            // 
            this.key3.Location = new System.Drawing.Point(435, 138);
            this.key3.Margin = new System.Windows.Forms.Padding(4);
            this.key3.Name = "key3";
            this.key3.Size = new System.Drawing.Size(60, 40);
            this.key3.TabIndex = 24;
            this.key3.TabStop = false;
            this.key3.Text = "3";
            this.key3.Toggled = false;
            this.key3.Click += new System.EventHandler(this.key_Click);
            // 
            // key4
            // 
            this.key4.Location = new System.Drawing.Point(299, 186);
            this.key4.Margin = new System.Windows.Forms.Padding(4);
            this.key4.Name = "key4";
            this.key4.Size = new System.Drawing.Size(60, 40);
            this.key4.TabIndex = 25;
            this.key4.TabStop = false;
            this.key4.Text = "4";
            this.key4.Toggled = false;
            this.key4.Click += new System.EventHandler(this.key_Click);
            // 
            // key5
            // 
            this.key5.Location = new System.Drawing.Point(367, 186);
            this.key5.Margin = new System.Windows.Forms.Padding(4);
            this.key5.Name = "key5";
            this.key5.Size = new System.Drawing.Size(60, 40);
            this.key5.TabIndex = 26;
            this.key5.TabStop = false;
            this.key5.Text = "5";
            this.key5.Toggled = false;
            this.key5.Click += new System.EventHandler(this.key_Click);
            // 
            // key6
            // 
            this.key6.Location = new System.Drawing.Point(435, 186);
            this.key6.Margin = new System.Windows.Forms.Padding(4);
            this.key6.Name = "key6";
            this.key6.Size = new System.Drawing.Size(60, 40);
            this.key6.TabIndex = 27;
            this.key6.TabStop = false;
            this.key6.Text = "6";
            this.key6.Toggled = false;
            this.key6.Click += new System.EventHandler(this.key_Click);
            // 
            // key7
            // 
            this.key7.Location = new System.Drawing.Point(299, 234);
            this.key7.Margin = new System.Windows.Forms.Padding(4);
            this.key7.Name = "key7";
            this.key7.Size = new System.Drawing.Size(60, 40);
            this.key7.TabIndex = 28;
            this.key7.TabStop = false;
            this.key7.Text = "7";
            this.key7.Toggled = false;
            this.key7.Click += new System.EventHandler(this.key_Click);
            // 
            // key8
            // 
            this.key8.Location = new System.Drawing.Point(367, 234);
            this.key8.Margin = new System.Windows.Forms.Padding(4);
            this.key8.Name = "key8";
            this.key8.Size = new System.Drawing.Size(60, 40);
            this.key8.TabIndex = 29;
            this.key8.TabStop = false;
            this.key8.Text = "8";
            this.key8.Toggled = false;
            this.key8.Click += new System.EventHandler(this.key_Click);
            // 
            // key9
            // 
            this.key9.Location = new System.Drawing.Point(435, 234);
            this.key9.Margin = new System.Windows.Forms.Padding(4);
            this.key9.Name = "key9";
            this.key9.Size = new System.Drawing.Size(60, 40);
            this.key9.TabIndex = 30;
            this.key9.TabStop = false;
            this.key9.Text = "9";
            this.key9.Toggled = false;
            this.key9.Click += new System.EventHandler(this.key_Click);
            // 
            // key0
            // 
            this.key0.Location = new System.Drawing.Point(367, 282);
            this.key0.Margin = new System.Windows.Forms.Padding(4);
            this.key0.Name = "key0";
            this.key0.Size = new System.Drawing.Size(60, 40);
            this.key0.TabIndex = 31;
            this.key0.TabStop = false;
            this.key0.Text = "0";
            this.key0.Toggled = false;
            this.key0.Click += new System.EventHandler(this.key_Click);
            // 
            // Keypad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.key0);
            this.Controls.Add(this.key9);
            this.Controls.Add(this.key8);
            this.Controls.Add(this.key7);
            this.Controls.Add(this.key6);
            this.Controls.Add(this.key5);
            this.Controls.Add(this.key4);
            this.Controls.Add(this.key3);
            this.Controls.Add(this.key2);
            this.Controls.Add(this.key1);
            this.Controls.Add(this.keySpace);
            this.Controls.Add(this.keyEnter);
            this.Controls.Add(this.keyExit);
            this.Controls.Add(this.keyBackspace);
            this.Controls.Add(this.keyClear);
            this.Controls.Add(this.textBox);
            this.Name = "Keypad";
            this.Text = "FormKeyboard";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormKeyboard_KeyPress);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBox, 0);
            this.Controls.SetChildIndex(this.keyClear, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.keyBackspace, 0);
            this.Controls.SetChildIndex(this.keyExit, 0);
            this.Controls.SetChildIndex(this.keyEnter, 0);
            this.Controls.SetChildIndex(this.keySpace, 0);
            this.Controls.SetChildIndex(this.key1, 0);
            this.Controls.SetChildIndex(this.key2, 0);
            this.Controls.SetChildIndex(this.key3, 0);
            this.Controls.SetChildIndex(this.key4, 0);
            this.Controls.SetChildIndex(this.key5, 0);
            this.Controls.SetChildIndex(this.key6, 0);
            this.Controls.SetChildIndex(this.key7, 0);
            this.Controls.SetChildIndex(this.key8, 0);
            this.Controls.SetChildIndex(this.key9, 0);
            this.Controls.SetChildIndex(this.key0, 0);
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

        private TextBox textBox;
        private Key keyClear;
        private Key keyBackspace;
        private Key keyExit;
        private Key keyEnter;
        private Key keySpace;
        private Key key1;
        private Key key2;
        private Key key3;
        private Key key4;
        private Key key5;
        private Key key6;
        private Key key7;
        private Key key8;
        private Key key9;
        private Key key0;
    }
}