namespace IO.View.Concrete
{
    partial class SearchResults
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchResults));
            this.textBoxDate = new IO.View.TextBox();
            this.buttonSearch = new IO.View.Button();
            this.textBoxUserName = new IO.View.TextBox();
            this.textBoxSpecimenId = new IO.View.TextBox();
            this.listBox = new IO.View.ListBox();
            this.comboBox = new IO.View.ComboBox();
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
            this.labelTime.Text = "16:37:46";
            // 
            // textBoxDate
            // 
            this.textBoxDate.Location = new System.Drawing.Point(25, 125);
            this.textBoxDate.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxDate.Name = "textBoxDate";
            this.textBoxDate.Password = false;
            this.textBoxDate.Size = new System.Drawing.Size(750, 40);
            this.textBoxDate.TabIndex = 7;
            this.textBoxDate.Title = "Date";
            this.textBoxDate.TitleWidth = 165;
            this.textBoxDate.Click += new System.EventHandler(this.textBoxDate_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSearch.Location = new System.Drawing.Point(613, 345);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(160, 40);
            this.buttonSearch.TabIndex = 9;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(25, 175);
            this.textBoxUserName.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Password = false;
            this.textBoxUserName.Size = new System.Drawing.Size(750, 40);
            this.textBoxUserName.TabIndex = 10;
            this.textBoxUserName.Title = "User";
            this.textBoxUserName.TitleWidth = 165;
            this.textBoxUserName.Click += new System.EventHandler(this.textBoxUserName_Click_1);
            // 
            // textBoxSpecimenId
            // 
            this.textBoxSpecimenId.Location = new System.Drawing.Point(25, 225);
            this.textBoxSpecimenId.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.textBoxSpecimenId.Name = "textBoxSpecimenId";
            this.textBoxSpecimenId.Password = false;
            this.textBoxSpecimenId.Size = new System.Drawing.Size(750, 40);
            this.textBoxSpecimenId.TabIndex = 11;
            this.textBoxSpecimenId.Title = "Specimen ID";
            this.textBoxSpecimenId.TitleWidth = 165;
            this.textBoxSpecimenId.Click += new System.EventHandler(this.textBoxSpecimenId_Click);
            // 
            // listBox
            // 
            this.listBox.Location = new System.Drawing.Point(189, 313);
            this.listBox.Margin = new System.Windows.Forms.Padding(0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(585, 150);
            this.listBox.TabIndex = 13;
            this.listBox.Values = ((System.Collections.Generic.IEnumerable<string>)(resources.GetObject("listBox.Values")));
            this.listBox.Visible = false;
            this.listBox.Click += new System.EventHandler(this.listBox_Click);
            // 
            // comboBox
            // 
            this.comboBox.Location = new System.Drawing.Point(25, 275);
            this.comboBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(750, 40);
            this.comboBox.TabIndex = 12;
            this.comboBox.Title = "Test Type";
            this.comboBox.Click += new System.EventHandler(this.comboBox_Click);
            // 
            // SearchResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.textBoxSpecimenId);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.textBoxDate);
            this.HelpText = "This screen allows stored results to be searched.\r\nSelect from Test Date, User ID" +
                ", Specimen ID and Test Type then click on Search.\r\nSelect more than one option t" +
                "o refine the search results.";
            this.Name = "SearchResults";
            this.Text = "EditUser";
            this.Controls.SetChildIndex(this.panelBottom, 0);
            this.Controls.SetChildIndex(this.panelTop, 0);
            this.Controls.SetChildIndex(this.pictureBoxAnimation, 0);
            this.Controls.SetChildIndex(this.titleBar, 0);
            this.Controls.SetChildIndex(this.textBoxDate, 0);
            this.Controls.SetChildIndex(this.buttonSearch, 0);
            this.Controls.SetChildIndex(this.textBoxUserName, 0);
            this.Controls.SetChildIndex(this.textBoxSpecimenId, 0);
            this.Controls.SetChildIndex(this.comboBox, 0);
            this.Controls.SetChildIndex(this.listBox, 0);
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

        private TextBox textBoxDate;
        private Button buttonSearch;
        private TextBox textBoxUserName;
        private TextBox textBoxSpecimenId;
        private ListBox listBox;
        private ComboBox comboBox;
    }
}