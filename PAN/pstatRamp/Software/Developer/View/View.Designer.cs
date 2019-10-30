namespace IO.View
{
    partial class View
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(View));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripTextBoxName = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSwitch = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonValue = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonStepper = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReservior = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTherm = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGraph = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBox = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxName,
            this.toolStripButtonEdit,
            this.toolStripButtonDelete,
            this.toolStripSeparator1,
            this.toolStripButtonSwitch,
            this.toolStripButtonValue,
            this.toolStripButtonStepper,
            this.toolStripButtonReservior,
            this.toolStripButtonTherm,
            this.toolStripButtonGraph,
            this.toolStripButtonBox,
            this.toolStripSeparator2});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(739, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip";
            // 
            // toolStripTextBoxName
            // 
            this.toolStripTextBoxName.Name = "toolStripTextBoxName";
            this.toolStripTextBoxName.Size = new System.Drawing.Size(150, 25);
            this.toolStripTextBoxName.Leave += new System.EventHandler(this.toolStripTextBoxName_Leave);
            this.toolStripTextBoxName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripTextBoxName_KeyPress);
            // 
            // toolStripButtonEdit
            // 
            this.toolStripButtonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEdit.Image = global::IO.View.Properties.Resources.Edit;
            this.toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEdit.Text = "Edit";
            this.toolStripButtonEdit.Click += new System.EventHandler(this.toolStripButtonEdit_Click);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSwitch
            // 
            this.toolStripButtonSwitch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSwitch.Enabled = false;
            this.toolStripButtonSwitch.Image = global::IO.View.Properties.Resources.Switch;
            this.toolStripButtonSwitch.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonSwitch.Name = "toolStripButtonSwitch";
            this.toolStripButtonSwitch.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSwitch.Tag = "IO.View.Controls.SwitchWidget";
            this.toolStripButtonSwitch.Text = "Switch";
            this.toolStripButtonSwitch.Click += new System.EventHandler(this.toolStripButton_Click);
            this.toolStripButtonSwitch.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseDown);
            this.toolStripButtonSwitch.MouseLeave += new System.EventHandler(this.toolStripButton_MouseLeave);
            this.toolStripButtonSwitch.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseMove);
            this.toolStripButtonSwitch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseUp);
            // 
            // toolStripButtonValue
            // 
            this.toolStripButtonValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonValue.Enabled = false;
            this.toolStripButtonValue.Image = global::IO.View.Properties.Resources.Value;
            this.toolStripButtonValue.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonValue.Name = "toolStripButtonValue";
            this.toolStripButtonValue.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonValue.Tag = "IO.View.Controls.ValueWidget";
            this.toolStripButtonValue.Text = "Value";
            this.toolStripButtonValue.Click += new System.EventHandler(this.toolStripButton_Click);
            this.toolStripButtonValue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseDown);
            this.toolStripButtonValue.MouseLeave += new System.EventHandler(this.toolStripButton_MouseLeave);
            this.toolStripButtonValue.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseMove);
            this.toolStripButtonValue.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseUp);
            // 
            // toolStripButtonStepper
            // 
            this.toolStripButtonStepper.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStepper.Enabled = false;
            this.toolStripButtonStepper.Image = global::IO.View.Properties.Resources.Stepper;
            this.toolStripButtonStepper.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonStepper.Name = "toolStripButtonStepper";
            this.toolStripButtonStepper.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonStepper.Tag = "IO.View.Controls.StepperWidget";
            this.toolStripButtonStepper.Text = "Stepper";
            this.toolStripButtonStepper.Click += new System.EventHandler(this.toolStripButton_Click);
            this.toolStripButtonStepper.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseDown);
            this.toolStripButtonStepper.MouseLeave += new System.EventHandler(this.toolStripButton_MouseLeave);
            this.toolStripButtonStepper.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseMove);
            this.toolStripButtonStepper.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseUp);
            // 
            // toolStripButtonReservior
            // 
            this.toolStripButtonReservior.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReservior.Enabled = false;
            this.toolStripButtonReservior.Image = global::IO.View.Properties.Resources.Reservior;
            this.toolStripButtonReservior.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonReservior.Name = "toolStripButtonReservior";
            this.toolStripButtonReservior.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonReservior.Tag = "IO.View.Controls.ReserviorWidget";
            this.toolStripButtonReservior.Text = "Reservior";
            this.toolStripButtonReservior.Click += new System.EventHandler(this.toolStripButton_Click);
            this.toolStripButtonReservior.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseDown);
            this.toolStripButtonReservior.MouseLeave += new System.EventHandler(this.toolStripButton_MouseLeave);
            this.toolStripButtonReservior.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseMove);
            this.toolStripButtonReservior.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseUp);
            // 
            // toolStripButtonTherm
            // 
            this.toolStripButtonTherm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTherm.Enabled = false;
            this.toolStripButtonTherm.Image = global::IO.View.Properties.Resources.Therm;
            this.toolStripButtonTherm.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonTherm.Name = "toolStripButtonTherm";
            this.toolStripButtonTherm.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTherm.Tag = "IO.View.Controls.ThermWidget";
            this.toolStripButtonTherm.Text = "Peltier";
            this.toolStripButtonTherm.Click += new System.EventHandler(this.toolStripButton_Click);
            this.toolStripButtonTherm.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseDown);
            this.toolStripButtonTherm.MouseLeave += new System.EventHandler(this.toolStripButton_MouseLeave);
            this.toolStripButtonTherm.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseMove);
            this.toolStripButtonTherm.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseUp);
            // 
            // toolStripButtonGraph
            // 
            this.toolStripButtonGraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGraph.Enabled = false;
            this.toolStripButtonGraph.Image = global::IO.View.Properties.Resources.graph;
            this.toolStripButtonGraph.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonGraph.Name = "toolStripButtonGraph";
            this.toolStripButtonGraph.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGraph.Tag = "IO.View.Controls.GraphWidget";
            this.toolStripButtonGraph.Text = "Graph";
            this.toolStripButtonGraph.Click += new System.EventHandler(this.toolStripButton_Click);
            this.toolStripButtonGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseDown);
            this.toolStripButtonGraph.MouseLeave += new System.EventHandler(this.toolStripButton_MouseLeave);
            this.toolStripButtonGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseMove);
            this.toolStripButtonGraph.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseUp);
            // 
            // toolStripButtonBox
            // 
            this.toolStripButtonBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBox.Enabled = false;
            this.toolStripButtonBox.Image = global::IO.View.Properties.Resources.Box;
            this.toolStripButtonBox.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonBox.Name = "toolStripButtonBox";
            this.toolStripButtonBox.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBox.Tag = "IO.View.Controls.BoxWidget";
            this.toolStripButtonBox.Text = "Box";
            this.toolStripButtonBox.ToolTipText = "Box";
            this.toolStripButtonBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseDown);
            this.toolStripButtonBox.MouseLeave += new System.EventHandler(this.toolStripButton_MouseLeave);
            this.toolStripButtonBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseMove);
            this.toolStripButtonBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.toolStripButton_MouseUp);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // View
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(739, 664);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "View";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.View_FormClosed);
            this.Load += new System.EventHandler(this.View_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.View_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.View_DragEnter);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.View_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.View_MouseMove);
            this.Move += new System.EventHandler(this.View_Move);
            this.Resize += new System.EventHandler(this.View_Resize);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxName;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.ToolStripButton toolStripButtonSwitch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonStepper;
        private System.Windows.Forms.ToolStripButton toolStripButtonReservior;
        private System.Windows.Forms.ToolStripButton toolStripButtonTherm;
        private System.Windows.Forms.ToolStripButton toolStripButtonBox;
        private System.Windows.Forms.ToolStripButton toolStripButtonGraph;
    }
}