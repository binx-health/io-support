namespace IO.View.Controls
{
    partial class AssayEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelDetails = new System.Windows.Forms.Panel();
            this.labelSeconds = new System.Windows.Forms.Label();
            this.labelEstimatedDuration = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.labelShortName = new System.Windows.Forms.Label();
            this.textBoxEstimatedDuration = new System.Windows.Forms.TextBox();
            this.textBoxCode = new System.Windows.Forms.TextBox();
            this.textBoxShortName = new System.Windows.Forms.TextBox();
            this.textBoxMetrics = new System.Windows.Forms.TextBox();
            this.labelMetrics = new System.Windows.Forms.Label();
            this.labelVoltammetryScript = new System.Windows.Forms.Label();
            this.textBoxVoltammetryScript = new System.Windows.Forms.TextBox();
            this.labelUngScript = new System.Windows.Forms.Label();
            this.textBoxUngScript = new System.Windows.Forms.TextBox();
            this.labelScript = new System.Windows.Forms.Label();
            this.textBoxScript = new System.Windows.Forms.TextBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.textBoxVersion = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listView = new IO.View.Controls.ListView();
            this.columnHeaderDisease = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLoinc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.panelDetails.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.labelSeconds);
            this.panelDetails.Controls.Add(this.labelEstimatedDuration);
            this.panelDetails.Controls.Add(this.labelCode);
            this.panelDetails.Controls.Add(this.labelShortName);
            this.panelDetails.Controls.Add(this.textBoxEstimatedDuration);
            this.panelDetails.Controls.Add(this.textBoxCode);
            this.panelDetails.Controls.Add(this.textBoxShortName);
            this.panelDetails.Controls.Add(this.textBoxMetrics);
            this.panelDetails.Controls.Add(this.labelMetrics);
            this.panelDetails.Controls.Add(this.labelVoltammetryScript);
            this.panelDetails.Controls.Add(this.textBoxVoltammetryScript);
            this.panelDetails.Controls.Add(this.labelUngScript);
            this.panelDetails.Controls.Add(this.textBoxUngScript);
            this.panelDetails.Controls.Add(this.labelScript);
            this.panelDetails.Controls.Add(this.textBoxScript);
            this.panelDetails.Controls.Add(this.labelVersion);
            this.panelDetails.Controls.Add(this.textBoxVersion);
            this.panelDetails.Controls.Add(this.labelName);
            this.panelDetails.Controls.Add(this.textBoxName);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(0, 0);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(420, 253);
            this.panelDetails.TabIndex = 0;
            // 
            // labelSeconds
            // 
            this.labelSeconds.AutoSize = true;
            this.labelSeconds.Location = new System.Drawing.Point(187, 224);
            this.labelSeconds.Name = "labelSeconds";
            this.labelSeconds.Size = new System.Drawing.Size(47, 13);
            this.labelSeconds.TabIndex = 17;
            this.labelSeconds.Text = "seconds";
            // 
            // labelEstimatedDuration
            // 
            this.labelEstimatedDuration.AutoSize = true;
            this.labelEstimatedDuration.Location = new System.Drawing.Point(17, 224);
            this.labelEstimatedDuration.Name = "labelEstimatedDuration";
            this.labelEstimatedDuration.Size = new System.Drawing.Size(96, 13);
            this.labelEstimatedDuration.TabIndex = 16;
            this.labelEstimatedDuration.Text = "Estimated Duration";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Location = new System.Drawing.Point(81, 198);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(32, 13);
            this.labelCode.TabIndex = 15;
            this.labelCode.Text = "Code";
            // 
            // labelShortName
            // 
            this.labelShortName.AutoSize = true;
            this.labelShortName.Location = new System.Drawing.Point(50, 172);
            this.labelShortName.Name = "labelShortName";
            this.labelShortName.Size = new System.Drawing.Size(63, 13);
            this.labelShortName.TabIndex = 14;
            this.labelShortName.Text = "Short Name";
            // 
            // textBoxEstimatedDuration
            // 
            this.textBoxEstimatedDuration.Location = new System.Drawing.Point(119, 221);
            this.textBoxEstimatedDuration.Name = "textBoxEstimatedDuration";
            this.textBoxEstimatedDuration.Size = new System.Drawing.Size(62, 20);
            this.textBoxEstimatedDuration.TabIndex = 13;
            this.textBoxEstimatedDuration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxEstimatedDuration.Leave += new System.EventHandler(this.textBoxEstimatedDuration_Leave);
            // 
            // textBoxCode
            // 
            this.textBoxCode.Location = new System.Drawing.Point(119, 195);
            this.textBoxCode.Name = "textBoxCode";
            this.textBoxCode.Size = new System.Drawing.Size(62, 20);
            this.textBoxCode.TabIndex = 12;
            this.textBoxCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxCode.Leave += new System.EventHandler(this.textBoxCode_Leave);
            // 
            // textBoxShortName
            // 
            this.textBoxShortName.Location = new System.Drawing.Point(119, 169);
            this.textBoxShortName.Name = "textBoxShortName";
            this.textBoxShortName.Size = new System.Drawing.Size(62, 20);
            this.textBoxShortName.TabIndex = 11;
            this.textBoxShortName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxShortName.Leave += new System.EventHandler(this.textBoxShortName_Leave);
            // 
            // textBoxMetrics
            // 
            this.textBoxMetrics.Location = new System.Drawing.Point(119, 143);
            this.textBoxMetrics.Name = "textBoxMetrics";
            this.textBoxMetrics.Size = new System.Drawing.Size(126, 20);
            this.textBoxMetrics.TabIndex = 5;
            this.textBoxMetrics.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxMetrics.Leave += new System.EventHandler(this.textBoxMetrics_Leave);
            // 
            // labelMetrics
            // 
            this.labelMetrics.AutoSize = true;
            this.labelMetrics.Location = new System.Drawing.Point(71, 146);
            this.labelMetrics.Name = "labelMetrics";
            this.labelMetrics.Size = new System.Drawing.Size(41, 13);
            this.labelMetrics.TabIndex = 10;
            this.labelMetrics.Text = "Metrics";
            // 
            // labelVoltammetryScript
            // 
            this.labelVoltammetryScript.AutoSize = true;
            this.labelVoltammetryScript.Location = new System.Drawing.Point(19, 120);
            this.labelVoltammetryScript.Name = "labelVoltammetryScript";
            this.labelVoltammetryScript.Size = new System.Drawing.Size(94, 13);
            this.labelVoltammetryScript.TabIndex = 9;
            this.labelVoltammetryScript.Text = "Voltammetry Script";
            // 
            // textBoxVoltammetryScript
            // 
            this.textBoxVoltammetryScript.Location = new System.Drawing.Point(119, 117);
            this.textBoxVoltammetryScript.Name = "textBoxVoltammetryScript";
            this.textBoxVoltammetryScript.Size = new System.Drawing.Size(126, 20);
            this.textBoxVoltammetryScript.TabIndex = 4;
            this.textBoxVoltammetryScript.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxVoltammetryScript.Leave += new System.EventHandler(this.textBoxVoltammetryScript_Leave);
            // 
            // labelUngScript
            // 
            this.labelUngScript.AutoSize = true;
            this.labelUngScript.Location = new System.Drawing.Point(52, 94);
            this.labelUngScript.Name = "labelUngScript";
            this.labelUngScript.Size = new System.Drawing.Size(61, 13);
            this.labelUngScript.TabIndex = 7;
            this.labelUngScript.Text = "UNG Script";
            // 
            // textBoxUngScript
            // 
            this.textBoxUngScript.Location = new System.Drawing.Point(119, 91);
            this.textBoxUngScript.Name = "textBoxUngScript";
            this.textBoxUngScript.Size = new System.Drawing.Size(126, 20);
            this.textBoxUngScript.TabIndex = 3;
            this.textBoxUngScript.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxUngScript.Leave += new System.EventHandler(this.textBoxUngScript_Leave);
            // 
            // labelScript
            // 
            this.labelScript.AutoSize = true;
            this.labelScript.Location = new System.Drawing.Point(79, 68);
            this.labelScript.Name = "labelScript";
            this.labelScript.Size = new System.Drawing.Size(34, 13);
            this.labelScript.TabIndex = 5;
            this.labelScript.Text = "Script";
            // 
            // textBoxScript
            // 
            this.textBoxScript.Location = new System.Drawing.Point(119, 65);
            this.textBoxScript.Name = "textBoxScript";
            this.textBoxScript.Size = new System.Drawing.Size(126, 20);
            this.textBoxScript.TabIndex = 2;
            this.textBoxScript.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxScript.Leave += new System.EventHandler(this.textBoxScript_Leave);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(71, 42);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(42, 13);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "Version";
            // 
            // textBoxVersion
            // 
            this.textBoxVersion.Location = new System.Drawing.Point(119, 39);
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.Size = new System.Drawing.Size(62, 20);
            this.textBoxVersion.TabIndex = 1;
            this.textBoxVersion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxVersion.Leave += new System.EventHandler(this.textBoxVersion_Leave);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(78, 16);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(119, 13);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(254, 20);
            this.textBoxName.TabIndex = 0;
            this.textBoxName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBoxName.Leave += new System.EventHandler(this.textBoxName_Leave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.listView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 253);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 143);
            this.panel1.TabIndex = 1;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderDisease,
            this.columnHeaderLoinc});
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 28);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.OwnerDraw = true;
            this.listView.Size = new System.Drawing.Size(420, 115);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.listView_Scroll);
            this.listView.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView_ColumnWidthChanging);
            this.listView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_DrawColumnHeader);
            this.listView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_DrawSubItem);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listView_KeyPress);
            this.listView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseClick);
            // 
            // columnHeaderDisease
            // 
            this.columnHeaderDisease.Text = "Disease";
            this.columnHeaderDisease.Width = 100;
            // 
            // columnHeaderLoinc
            // 
            this.columnHeaderLoinc.Text = "LOINC";
            this.columnHeaderLoinc.Width = 200;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAdd,
            this.toolStripButtonDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(420, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = global::IO.View.Properties.Resources.Add_16x16;
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "Add";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelete.Enabled = false;
            this.toolStripButtonDelete.Image = global::IO.View.Properties.Resources.Delete_16x16;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelete.Text = "Delete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // AssayEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelDetails);
            this.Name = "AssayEditor";
            this.Size = new System.Drawing.Size(420, 396);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.TextBox textBoxVersion;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TextBox textBoxMetrics;
        private System.Windows.Forms.Label labelMetrics;
        private System.Windows.Forms.Label labelVoltammetryScript;
        private System.Windows.Forms.TextBox textBoxVoltammetryScript;
        private System.Windows.Forms.Label labelUngScript;
        private System.Windows.Forms.TextBox textBoxUngScript;
        private System.Windows.Forms.Label labelScript;
        private System.Windows.Forms.TextBox textBoxScript;
        private System.Windows.Forms.Label labelSeconds;
        private System.Windows.Forms.Label labelEstimatedDuration;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.Label labelShortName;
        private System.Windows.Forms.TextBox textBoxEstimatedDuration;
        private System.Windows.Forms.TextBox textBoxCode;
        private System.Windows.Forms.TextBox textBoxShortName;
        private System.Windows.Forms.Panel panel1;
        private ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeaderDisease;
        private System.Windows.Forms.ColumnHeader columnHeaderLoinc;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;

    }
}
