namespace IO.View
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelPhase = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelX = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRead = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonWrite = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonReload = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonExecute = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAbort = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCommand = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButtonViews = new System.Windows.Forms.ToolStripDropDownButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPower = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCommandPrompt = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBoxDevices = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonReset = new System.Windows.Forms.ToolStripButton();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerAssays = new System.Windows.Forms.SplitContainer();
            this.splitContainerExplorer = new System.Windows.Forms.SplitContainer();
            this.treeViewExplorer = new System.Windows.Forms.TreeView();
            this.textBoxTerminal = new System.Windows.Forms.TextBox();
            this.listViewDeviceValues = new IO.View.Controls.DoubleBufferedListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainerAssays.Panel1.SuspendLayout();
            this.splitContainerAssays.Panel2.SuspendLayout();
            this.splitContainerAssays.SuspendLayout();
            this.splitContainerExplorer.Panel1.SuspendLayout();
            this.splitContainerExplorer.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripStatusLabelPhase,
            this.toolStripStatusLabelX});
            this.statusStrip.Location = new System.Drawing.Point(0, 658);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(821, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel.Image = global::IO.View.Properties.Resources.Ok;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabel.Text = "Idle";
            // 
            // toolStripStatusLabelPhase
            // 
            this.toolStripStatusLabelPhase.Name = "toolStripStatusLabelPhase";
            this.toolStripStatusLabelPhase.Size = new System.Drawing.Size(38, 17);
            this.toolStripStatusLabelPhase.Text = "Phase";
            // 
            // toolStripStatusLabelX
            // 
            this.toolStripStatusLabelX.Name = "toolStripStatusLabelX";
            this.toolStripStatusLabelX.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRead,
            this.toolStripButtonWrite,
            this.toolStripSeparator2,
            this.toolStripButtonReload,
            this.toolStripButtonSave,
            this.toolStripButtonExport,
            this.toolStripSeparator1,
            this.toolStripButtonDelete,
            this.toolStripButtonAdd,
            this.toolStripSeparator4,
            this.toolStripButtonExecute,
            this.toolStripButtonAbort,
            this.toolStripButtonCommand,
            this.toolStripSeparator5,
            this.toolStripDropDownButtonViews,
            this.toolStripSeparator3,
            this.toolStripButtonPower,
            this.toolStripButtonCommandPrompt,
            this.toolStripComboBoxDevices,
            this.toolStripButtonReset});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(821, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButtonRead
            // 
            this.toolStripButtonRead.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRead.Enabled = false;
            this.toolStripButtonRead.Image = global::IO.View.Properties.Resources.Download_16x16;
            this.toolStripButtonRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRead.Name = "toolStripButtonRead";
            this.toolStripButtonRead.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRead.Text = "Read From Instrument";
            this.toolStripButtonRead.Click += new System.EventHandler(this.toolStripButtonRead_Click);
            // 
            // toolStripButtonWrite
            // 
            this.toolStripButtonWrite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonWrite.Enabled = false;
            this.toolStripButtonWrite.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonWrite.Image")));
            this.toolStripButtonWrite.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonWrite.Name = "toolStripButtonWrite";
            this.toolStripButtonWrite.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonWrite.Text = "Write To Instrument";
            this.toolStripButtonWrite.Click += new System.EventHandler(this.toolStripButtonWrite_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonReload
            // 
            this.toolStripButtonReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReload.Image = global::IO.View.Properties.Resources.Download_16x16;
            this.toolStripButtonReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReload.Name = "toolStripButtonReload";
            this.toolStripButtonReload.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReload.Text = "Reload From Disk";
            this.toolStripButtonReload.Click += new System.EventHandler(this.toolStripButtonReload_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Enabled = false;
            this.toolStripButtonSave.Image = global::IO.View.Properties.Resources.Save_16x16;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Save To Disk";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonExport.Image = global::IO.View.Properties.Resources.Upload_16x16;
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonExport.Text = "Export";
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelete.Enabled = false;
            this.toolStripButtonDelete.Image = global::IO.View.Properties.Resources.Delete_16x16;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelete.Text = "Delete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Enabled = false;
            this.toolStripButtonAdd.Image = global::IO.View.Properties.Resources.Add_16x16;
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "Add";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonExecute
            // 
            this.toolStripButtonExecute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonExecute.Enabled = false;
            this.toolStripButtonExecute.Image = global::IO.View.Properties.Resources.Play_16x16;
            this.toolStripButtonExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExecute.Name = "toolStripButtonExecute";
            this.toolStripButtonExecute.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonExecute.Text = "Execute Script";
            this.toolStripButtonExecute.Click += new System.EventHandler(this.toolStripButtonExecute_Click);
            // 
            // toolStripButtonAbort
            // 
            this.toolStripButtonAbort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAbort.Enabled = false;
            this.toolStripButtonAbort.Image = global::IO.View.Properties.Resources.Stop_16x16;
            this.toolStripButtonAbort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbort.Name = "toolStripButtonAbort";
            this.toolStripButtonAbort.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAbort.Text = "Stop Executing Script";
            this.toolStripButtonAbort.Click += new System.EventHandler(this.toolStripButtonAbort_Click);
            // 
            // toolStripButtonCommand
            // 
            this.toolStripButtonCommand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCommand.Enabled = false;
            this.toolStripButtonCommand.Image = global::IO.View.Properties.Resources.Redo_16x16;
            this.toolStripButtonCommand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCommand.Name = "toolStripButtonCommand";
            this.toolStripButtonCommand.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCommand.Text = "Execute Selected Command";
            this.toolStripButtonCommand.Click += new System.EventHandler(this.toolStripButtonCommand_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButtonViews
            // 
            this.toolStripDropDownButtonViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonViews.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripSeparator6});
            this.toolStripDropDownButtonViews.Image = global::IO.View.Properties.Resources.Window;
            this.toolStripDropDownButtonViews.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonViews.Name = "toolStripDropDownButtonViews";
            this.toolStripDropDownButtonViews.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButtonViews.Text = "Views";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(95, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPower
            // 
            this.toolStripButtonPower.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPower.Enabled = false;
            this.toolStripButtonPower.Image = global::IO.View.Properties.Resources.power;
            this.toolStripButtonPower.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPower.Name = "toolStripButtonPower";
            this.toolStripButtonPower.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPower.Text = "Power Off";
            this.toolStripButtonPower.Click += new System.EventHandler(this.toolStripButtonPower_Click);
            // 
            // toolStripButtonCommandPrompt
            // 
            this.toolStripButtonCommandPrompt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCommandPrompt.Enabled = false;
            this.toolStripButtonCommandPrompt.Image = global::IO.View.Properties.Resources.CommandPrompt;
            this.toolStripButtonCommandPrompt.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonCommandPrompt.Name = "toolStripButtonCommandPrompt";
            this.toolStripButtonCommandPrompt.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCommandPrompt.Text = "Launch Command Prompt";
            this.toolStripButtonCommandPrompt.Click += new System.EventHandler(this.toolStripButtonCommandPrompt_Click);
            // 
            // toolStripComboBoxDevices
            // 
            this.toolStripComboBoxDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxDevices.Enabled = false;
            this.toolStripComboBoxDevices.Name = "toolStripComboBoxDevices";
            this.toolStripComboBoxDevices.Size = new System.Drawing.Size(121, 25);
            this.toolStripComboBoxDevices.Sorted = true;
            this.toolStripComboBoxDevices.ToolTipText = "Devices";
            this.toolStripComboBoxDevices.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxDevices_SelectedIndexChanged);
            // 
            // toolStripButtonReset
            // 
            this.toolStripButtonReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReset.Enabled = false;
            this.toolStripButtonReset.Image = global::IO.View.Properties.Resources.Refresh_16x16;
            this.toolStripButtonReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReset.Name = "toolStripButtonReset";
            this.toolStripButtonReset.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReset.Text = "Reconnect";
            this.toolStripButtonReset.Click += new System.EventHandler(this.toolStripButtonReset_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Doc.ico");
            this.imageList.Images.SetKeyName(1, "Doc-Warn.ico");
            this.imageList.Images.SetKeyName(2, "Icon2.ico");
            this.imageList.Images.SetKeyName(3, "Icon3.ico");
            this.imageList.Images.SetKeyName(4, "Icon4.ico");
            this.imageList.Images.SetKeyName(5, "Icon5.ico");
            this.imageList.Images.SetKeyName(6, "Icon6.ico");
            this.imageList.Images.SetKeyName(7, "Icon7.ico");
            this.imageList.Images.SetKeyName(8, "Text_Document.ico");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainerAssays);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewDeviceValues);
            this.splitContainer1.Size = new System.Drawing.Size(821, 633);
            this.splitContainer1.SplitterDistance = 707;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainerAssays
            // 
            this.splitContainerAssays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAssays.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAssays.Name = "splitContainerAssays";
            this.splitContainerAssays.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerAssays.Panel1
            // 
            this.splitContainerAssays.Panel1.Controls.Add(this.splitContainerExplorer);
            // 
            // splitContainerAssays.Panel2
            // 
            this.splitContainerAssays.Panel2.Controls.Add(this.textBoxTerminal);
            this.splitContainerAssays.Size = new System.Drawing.Size(707, 633);
            this.splitContainerAssays.SplitterDistance = 442;
            this.splitContainerAssays.TabIndex = 3;
            this.splitContainerAssays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTerminal_KeyPress);
            // 
            // splitContainerExplorer
            // 
            this.splitContainerExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerExplorer.Location = new System.Drawing.Point(0, 0);
            this.splitContainerExplorer.Name = "splitContainerExplorer";
            // 
            // splitContainerExplorer.Panel1
            // 
            this.splitContainerExplorer.Panel1.Controls.Add(this.treeViewExplorer);
            this.splitContainerExplorer.Size = new System.Drawing.Size(707, 442);
            this.splitContainerExplorer.SplitterDistance = 147;
            this.splitContainerExplorer.TabIndex = 3;
            this.splitContainerExplorer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTerminal_KeyPress);
            // 
            // treeViewExplorer
            // 
            this.treeViewExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewExplorer.HideSelection = false;
            this.treeViewExplorer.ImageIndex = 0;
            this.treeViewExplorer.ImageList = this.imageList;
            this.treeViewExplorer.ItemHeight = 18;
            this.treeViewExplorer.Location = new System.Drawing.Point(0, 0);
            this.treeViewExplorer.Name = "treeViewExplorer";
            this.treeViewExplorer.SelectedImageIndex = 0;
            this.treeViewExplorer.Size = new System.Drawing.Size(147, 442);
            this.treeViewExplorer.TabIndex = 0;
            this.treeViewExplorer.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewExplorer_AfterLabelEdit);
            this.treeViewExplorer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewExplorer_AfterSelect);
            this.treeViewExplorer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTerminal_KeyPress);
            // 
            // textBoxTerminal
            // 
            this.textBoxTerminal.BackColor = System.Drawing.Color.White;
            this.textBoxTerminal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTerminal.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTerminal.HideSelection = false;
            this.textBoxTerminal.Location = new System.Drawing.Point(0, 0);
            this.textBoxTerminal.Multiline = true;
            this.textBoxTerminal.Name = "textBoxTerminal";
            this.textBoxTerminal.ReadOnly = true;
            this.textBoxTerminal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxTerminal.Size = new System.Drawing.Size(707, 187);
            this.textBoxTerminal.TabIndex = 0;
            this.textBoxTerminal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxTerminal_KeyDown);
            this.textBoxTerminal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTerminal_KeyPress);
            // 
            // listViewDeviceValues
            // 
            this.listViewDeviceValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderValue});
            this.listViewDeviceValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewDeviceValues.Location = new System.Drawing.Point(0, 0);
            this.listViewDeviceValues.Name = "listViewDeviceValues";
            this.listViewDeviceValues.Size = new System.Drawing.Size(110, 633);
            this.listViewDeviceValues.TabIndex = 0;
            this.listViewDeviceValues.UseCompatibleStateImageBehavior = false;
            this.listViewDeviceValues.View = System.Windows.Forms.View.Details;
            this.listViewDeviceValues.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewDeviceValues_ItemDrag);
            this.listViewDeviceValues.Resize += new System.EventHandler(this.listViewDeviceValues_Resize);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            // 
            // columnHeaderValue
            // 
            this.columnHeaderValue.Text = "Value";
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Export Assays";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(821, 680);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IO Developer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerAssays.Panel1.ResumeLayout(false);
            this.splitContainerAssays.Panel2.ResumeLayout(false);
            this.splitContainerAssays.Panel2.PerformLayout();
            this.splitContainerAssays.ResumeLayout(false);
            this.splitContainerExplorer.Panel1.ResumeLayout(false);
            this.splitContainerExplorer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton toolStripButtonReset;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonReload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRead;
        private System.Windows.Forms.ToolStripButton toolStripButtonWrite;
        private System.Windows.Forms.ToolStripButton toolStripButtonExecute;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbort;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonCommand;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainerAssays;
        private System.Windows.Forms.SplitContainer splitContainerExplorer;
        private System.Windows.Forms.TreeView treeViewExplorer;
        private System.Windows.Forms.TextBox textBoxTerminal;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonViews;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton toolStripButtonCommandPrompt;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelPhase;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelX;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxDevices;
        private System.Windows.Forms.ToolStripButton toolStripButtonPower;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
        private Controls.DoubleBufferedListView listViewDeviceValues;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}