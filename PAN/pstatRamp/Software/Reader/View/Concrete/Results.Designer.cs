namespace IO.View.Concrete
{
    partial class Results
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Results));
            this.tableTests = new IO.View.Table();
            this.buttonView = new IO.View.Button();
            this.scrollBar = new IO.View.ScrollBar();
            this.keyDateTime = new IO.View.Sorter();
            this.keyUser = new IO.View.Sorter();
            this.keySpecimenId = new IO.View.Sorter();
            this.keyTest = new IO.View.Sorter();
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
            this.labelDate.Text = "12 Sep 2013";
            // 
            // labelTime
            // 
            this.labelTime.Text = "11:06:11";
            // 
            // tableTests
            // 
            this.tableTests.ColumnWidths = new int[] {
        200,
        150,
        200};
            this.tableTests.Location = new System.Drawing.Point(25, 163);
            this.tableTests.Margin = new System.Windows.Forms.Padding(10);
            this.tableTests.Name = "tableTests";
            this.tableTests.SelectedRow = 0;
            this.tableTests.Size = new System.Drawing.Size(700, 162);
            this.tableTests.TabIndex = 8;
            this.tableTests.TopRow = 0;
            this.tableTests.Values = null;
            // 
            // buttonView
            // 
            this.buttonView.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonView.Location = new System.Drawing.Point(615, 345);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(160, 40);
            this.buttonView.TabIndex = 9;
            this.buttonView.Text = "View";
            this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
            // 
            // scrollBar
            // 
            this.scrollBar.Location = new System.Drawing.Point(735, 125);
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
            // keyDateTime
            // 
            this.keyDateTime.ArrowType = IO.View.ArrowType.None;
            this.keyDateTime.Location = new System.Drawing.Point(25, 125);
            this.keyDateTime.Margin = new System.Windows.Forms.Padding(0);
            this.keyDateTime.Name = "keyDateTime";
            this.keyDateTime.Size = new System.Drawing.Size(202, 40);
            this.keyDateTime.TabIndex = 11;
            this.keyDateTime.Tag = "0";
            this.keyDateTime.Text = "Date/Time";
            this.keyDateTime.Toggled = false;
            this.keyDateTime.Click += new System.EventHandler(this.keySort_Click);
            // 
            // keyUser
            // 
            this.keyUser.ArrowType = IO.View.ArrowType.None;
            this.keyUser.Location = new System.Drawing.Point(225, 125);
            this.keyUser.Margin = new System.Windows.Forms.Padding(0);
            this.keyUser.Name = "keyUser";
            this.keyUser.Size = new System.Drawing.Size(152, 40);
            this.keyUser.TabIndex = 12;
            this.keyUser.Tag = "1";
            this.keyUser.Text = "User";
            this.keyUser.Toggled = false;
            this.keyUser.Click += new System.EventHandler(this.keySort_Click);
            // 
            // keySpecimenId
            // 
            this.keySpecimenId.ArrowType = IO.View.ArrowType.None;
            this.keySpecimenId.Location = new System.Drawing.Point(375, 125);
            this.keySpecimenId.Margin = new System.Windows.Forms.Padding(0);
            this.keySpecimenId.Name = "keySpecimenId";
            this.keySpecimenId.Size = new System.Drawing.Size(202, 40);
            this.keySpecimenId.TabIndex = 13;
            this.keySpecimenId.Tag = "2";
            this.keySpecimenId.Text = "Specimen ID";
            this.keySpecimenId.Toggled = false;
            this.keySpecimenId.Click += new System.EventHandler(this.keySort_Click);
            // 
            // keyTest
            // 
            this.keyTest.ArrowType = IO.View.ArrowType.None;
            this.keyTest.Location = new System.Drawing.Point(575, 125);
            this.keyTest.Margin = new System.Windows.Forms.Padding(0);
            this.keyTest.Name = "keyTest";
            this.keyTest.Size = new System.Drawing.Size(150, 40);
            this.keyTest.TabIndex = 15;
            this.keyTest.Tag = "1";
            this.keyTest.Text = "Test";
            this.keyTest.Toggled = false;
            this.keyTest.Click += new System.EventHandler(this.keySort_Click);
            // 
            // Results
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.keyTest);
            this.Controls.Add(this.keySpecimenId);
            this.Controls.Add(this.keyUser);
            this.Controls.Add(this.keyDateTime);
            this.Controls.Add(this.scrollBar);
            this.Controls.Add(this.tableTests);
            this.Controls.Add(this.buttonView);
            this.HelpText = resources.GetString("$this.HelpText");
            this.Name = "Results";
            this.Text = "Users";
            this.Controls.SetChildIndex(this.pictureBoxBackground, 0);
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.buttonView, 0);
            this.Controls.SetChildIndex(this.tableTests, 0);
            this.Controls.SetChildIndex(this.scrollBar, 0);
            this.Controls.SetChildIndex(this.keyDateTime, 0);
            this.Controls.SetChildIndex(this.keyUser, 0);
            this.Controls.SetChildIndex(this.keySpecimenId, 0);
            this.Controls.SetChildIndex(this.keyTest, 0);
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

        protected Table tableTests;
        protected Button buttonView;
        protected ScrollBar scrollBar;
        protected Sorter keyDateTime;
        protected Sorter keyUser;
        protected Sorter keySpecimenId;
        protected Sorter keyTest;

    }
}