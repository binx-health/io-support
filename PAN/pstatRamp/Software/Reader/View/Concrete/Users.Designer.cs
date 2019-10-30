namespace IO.View.Concrete
{
    partial class Users
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Users));
            this.tableUsers = new IO.View.Table();
            this.buttonView = new IO.View.Button();
            this.scrollBar = new IO.View.ScrollBar();
            this.keyType = new IO.View.Sorter();
            this.keyName = new IO.View.Sorter();
            this.keyEnabled = new IO.View.Sorter();
            this.buttonAddNew = new IO.View.Button();
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
            this.labelTime.Text = "15:39:42";
            // 
            // tableUsers
            // 
            this.tableUsers.ColumnWidths = new int[] {
        250,
        250};
            this.tableUsers.Location = new System.Drawing.Point(25, 163);
            this.tableUsers.Margin = new System.Windows.Forms.Padding(10);
            this.tableUsers.Name = "tableUsers";
            this.tableUsers.SelectedRow = 0;
            this.tableUsers.Size = new System.Drawing.Size(700, 162);
            this.tableUsers.TabIndex = 8;
            this.tableUsers.TopRow = 0;
            this.tableUsers.Values = null;
            // 
            // buttonView
            // 
            this.buttonView.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonView.Location = new System.Drawing.Point(25, 345);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(160, 40);
            this.buttonView.TabIndex = 9;
            this.buttonView.Text = "View";
            this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
            // 
            // scrollBar
            // 
            this.scrollBar.Location = new System.Drawing.Point(733, 125);
            this.scrollBar.Margin = new System.Windows.Forms.Padding(0);
            this.scrollBar.Maximum = 100;
            this.scrollBar.Name = "scrollBar";
            this.scrollBar.Position = 0;
            this.scrollBar.Size = new System.Drawing.Size(40, 200);
            this.scrollBar.Step = 1;
            this.scrollBar.TabIndex = 10;
            this.scrollBar.Text = "scrollBar1";
            this.scrollBar.Click += new System.EventHandler(this.scrollBar_Click);
            // 
            // keyType
            // 
            this.keyType.ArrowType = IO.View.ArrowType.Down;
            this.keyType.Location = new System.Drawing.Point(25, 125);
            this.keyType.Margin = new System.Windows.Forms.Padding(0);
            this.keyType.Name = "keyType";
            this.keyType.Size = new System.Drawing.Size(252, 40);
            this.keyType.TabIndex = 11;
            this.keyType.Tag = "0";
            this.keyType.Text = "Type";
            this.keyType.Toggled = false;
            this.keyType.Click += new System.EventHandler(this.keySort_Click);
            // 
            // keyName
            // 
            this.keyName.ArrowType = IO.View.ArrowType.Down;
            this.keyName.Location = new System.Drawing.Point(275, 125);
            this.keyName.Margin = new System.Windows.Forms.Padding(0);
            this.keyName.Name = "keyName";
            this.keyName.Size = new System.Drawing.Size(252, 40);
            this.keyName.TabIndex = 12;
            this.keyName.Tag = "1";
            this.keyName.Text = "Name";
            this.keyName.Toggled = false;
            this.keyName.Click += new System.EventHandler(this.keySort_Click);
            // 
            // keyEnabled
            // 
            this.keyEnabled.ArrowType = IO.View.ArrowType.Down;
            this.keyEnabled.Location = new System.Drawing.Point(525, 125);
            this.keyEnabled.Margin = new System.Windows.Forms.Padding(0);
            this.keyEnabled.Name = "keyEnabled";
            this.keyEnabled.Size = new System.Drawing.Size(200, 40);
            this.keyEnabled.TabIndex = 13;
            this.keyEnabled.Tag = "2";
            this.keyEnabled.Text = "Enabled/disabled";
            this.keyEnabled.Toggled = false;
            this.keyEnabled.Click += new System.EventHandler(this.keySort_Click);
            // 
            // buttonAddNew
            // 
            this.buttonAddNew.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddNew.Location = new System.Drawing.Point(613, 345);
            this.buttonAddNew.Name = "buttonAddNew";
            this.buttonAddNew.Size = new System.Drawing.Size(160, 40);
            this.buttonAddNew.TabIndex = 14;
            this.buttonAddNew.Text = "Add New";
            this.buttonAddNew.Click += new System.EventHandler(this.buttonAddNew_Click);
            // 
            // Users
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.buttonAddNew);
            this.Controls.Add(this.keyEnabled);
            this.Controls.Add(this.keyName);
            this.Controls.Add(this.keyType);
            this.Controls.Add(this.scrollBar);
            this.Controls.Add(this.tableUsers);
            this.Controls.Add(this.buttonView);
            this.HelpText = resources.GetString("$this.HelpText");
            this.Name = "Users";
            this.Text = "Users";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.buttonView, 0);
            this.Controls.SetChildIndex(this.tableUsers, 0);
            this.Controls.SetChildIndex(this.scrollBar, 0);
            this.Controls.SetChildIndex(this.keyType, 0);
            this.Controls.SetChildIndex(this.keyName, 0);
            this.Controls.SetChildIndex(this.keyEnabled, 0);
            this.Controls.SetChildIndex(this.buttonAddNew, 0);
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

        private Table tableUsers;
        private Button buttonView;
        private ScrollBar scrollBar;
        private Sorter keyType;
        private Sorter keyName;
        private Sorter keyEnabled;
        private Button buttonAddNew;

    }
}