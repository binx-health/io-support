/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace IO.View
{
    /// <summary>
    /// View form
    /// </summary>
    public partial class View : Form, IXmlSerializable
    {
        /// <summary>
        /// Flag to indicate that we are dragging a button
        /// </summary>
        private bool buttonDragCapture = false;

        /// <summary>
        /// The name of the view
        /// </summary>
        private string name;

        /// <summary>
        /// The current widget
        /// </summary>
        private Controls.Widget currentWidget = null;

        /// <summary>
        /// The current mouse position for moving controls
        /// </summary>
        private Point currentPosition;

        /// <summary>
        /// Flag to indicate the current action is a move
        /// </summary>
        private bool currentActionMove;

        /// <summary>
        /// The current widget tools
        /// </summary>
        private IEnumerable<ToolStripItem> currentWidgetTools = null;

        /// <summary>
        /// Modified flag
        /// </summary>
        public bool Modified { get; set; }

        /// <summary>
        /// The name of the view
        /// </summary>
        public new string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = toolStripTextBoxName.Text = value;
            }
        }

        /// <summary>
        /// The editing flag
        /// </summary>
        public bool Editing
        {
            get
            {
                return toolStripButtonEdit.Checked;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public View()
        {
            InitializeComponent();

            // Initialse the modified flag
            Modified = false;
        }

        /// <summary>
        /// Get schema laways returns null
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public void ReadXml(XmlReader reader)
        {
            // Read the window position
            SetBounds(int.Parse(reader.GetAttribute("Bounds.X")),
                int.Parse(reader.GetAttribute("Bounds.Y")),
                int.Parse(reader.GetAttribute("Bounds.Width")),
                int.Parse(reader.GetAttribute("Bounds.Height")));
            WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), reader.GetAttribute("WindowState"));

            // Read the XML to the end
            while (true)
            {
                // Check for an element
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Depth == 1))
                {
                    // Create a widget holder for the type
                    var widgetHolder = new WidgetHolder(reader.Name);

                    // Read the widget specific XML
                    widgetHolder.Widget.ReadXml(reader);

                    // Add the widget
                    AddWidget(widgetHolder, widgetHolder.Widget.Location);
                }
                
                if (reader.Read() == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public void WriteXml(XmlWriter writer)
        {
            // Write the window position
            writer.WriteAttributeString("Bounds.X", Bounds.X.ToString());
            writer.WriteAttributeString("Bounds.Y", Bounds.Y.ToString());
            writer.WriteAttributeString("Bounds.Width", Bounds.Width.ToString());
            writer.WriteAttributeString("Bounds.Height", Bounds.Height.ToString());
            writer.WriteAttributeString("WindowState", WindowState.ToString());

            // Loop through the values writing the elements
            foreach (var control in Controls)
            {
                if (control is Controls.Widget)
                {
                    // Write an element for the type
                    writer.WriteStartElement(control.GetType().ToString());

                    // Write the widget specific XML
                    (control as Controls.Widget).WriteXml(writer);

                    // End the element
                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Initiate the moving of a widget
        /// </summary>
        /// <param name="widget">The widget</param>
        /// <param name="position">The mouse position</param>
        public void MoveWidget(Controls.Widget widget, Point position)
        {
            // Set capture and the state variables for moving
            Capture = true;
            currentWidget = widget;
            currentPosition = position;
            currentActionMove = true;
        }

        /// <summary>
        /// Initiate the resizing of a widget
        /// </summary>
        /// <param name="widget">The widget</param>
        /// <param name="position">The mouse position</param>
        public void ResizeWidget(Controls.Widget widget, Point position)
        {
            // Set capture and the state variables for resizing
            Capture = true;
            currentWidget = widget;
            currentPosition = position;
            currentActionMove = false;
        }

        /// <summary>
        /// Called by a widget when it is entered
        /// </summary>
        /// <param name="widget">The widget</param>
        public void WidgetEntered(Controls.Widget widget)
        {
            // Set the current widget
            currentWidget = widget;

            // Update the widget tools
            UpdateWidgetTools();

            // Update the buttons
            UpdateButtons();
        }

        /// <summary>
        /// Called by a widget when it is left
        /// </summary>
        /// <param name="widget">The widget</param>
        public void WidgetLeft(Controls.Widget widget)
        {
            // Clear the current widget
            currentWidget = null;

            // Update the buttons
            UpdateButtons();
        }

        /// <summary>
        /// Delete the current widget
        /// </summary>
        public void DeleteCurrentWidget()
        {
            // Check for a current widget
            if (currentWidget != null)
            {
                // Remember the value
                var widget = currentWidget;

                // Clear the value
                currentWidget = null;

                // Select the next control
                SelectNextControl(widget, true, true, true, true);

                // Remove the widget
                Controls.Remove(widget);

                // Set the modified flag
                Modified = true;

                // Update the widget tools
                UpdateWidgetTools();
    
                // Update the buttons
                UpdateButtons();
            }
        }

        /// <summary>
        /// Update the widget tools for the current widget
        /// </summary>
        private void UpdateWidgetTools()
        {
            // Check for widget tools
            if (currentWidgetTools != null)
            {
                // Remove them to the tool bar
                foreach (var tool in currentWidgetTools)
                {
                    toolStrip.Items.Remove(tool);
                }
            }

            // Check for widget tools
            if (Editing && (currentWidget != null) && ((currentWidgetTools = currentWidget.Tools) != null))
            {
                // Add them to the tool bar
                foreach (var tool in currentWidgetTools)
                {
                    toolStrip.Items.Add(tool);
                }
            }
        }

        /// <summary>
        /// Refresh the view and update the buttons
        /// </summary>
        private void UpdateButtons()
        {
            Refresh();

            toolStripButtonDelete.Enabled = Editing && (currentWidget != null);
            toolStripButtonSwitch.Enabled = Editing;
            toolStripButtonValue.Enabled = Editing;
            toolStripButtonStepper.Enabled = Editing;
            toolStripButtonReservior.Enabled = Editing;
            toolStripButtonTherm.Enabled = Editing;
            toolStripButtonGraph.Enabled = Editing;
            toolStripButtonBox.Enabled = Editing;
        }

        /// <summary>
        /// Add a widget in the specificed location
        /// </summary>
        /// <param name="widgetHolder">The widget holder</param>
        /// <param name="position">The location</param>
        private void AddWidget(WidgetHolder widgetHolder, Point position)
        {
            // Set the bounds on the control
            widgetHolder.Widget.SetBounds(position.X, position.Y, 0, 0, BoundsSpecified.Location);

            // Add it to the form
            Controls.Add(widgetHolder.Widget);

            // Set the modified flag
            Modified = true;

            // Select it
            widgetHolder.Widget.Select();
        }

        /// <summary>
        /// Load event handler for the view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_Load(object sender, EventArgs e)
        {
            // If there are no widgets then start up in edit mode
            toolStripButtonEdit.Checked = Controls.Count <= 1;

            UpdateButtons();
        }

        /// <summary>
        /// Form closed event handler for the view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Ensure that any name change is made
            toolStripTextBoxName_Leave(sender, e);

            // Inform the main window that the view has closed
            Main.Instance.ViewClosed(Name);
        }

        /// <summary>
        /// Key press event handler for the name tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                toolStripTextBoxName_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the name tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBoxName_Leave(object sender, EventArgs e)
        {
            // Check for a change
            if (name != toolStripTextBoxName.Text)
            {
                // try to rename the view
                if (Main.Instance.RenameView(name, toolStripTextBoxName.Text))
                {
                    // Rename succeeded
                    name = toolStripTextBoxName.Text;
                }
                else
                {
                    // Rename failed
                    toolStripTextBoxName.Text = name;
                }
            }
        }

        /// <summary>
        /// Mouse down event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MouseDown(object sender, MouseEventArgs e)
        {
            // Clear the current widget and active control and update
            currentWidget = null;
            ActiveControl = null;
            UpdateWidgetTools();
            UpdateButtons();
        }

        /// <summary>
        /// Mouse move event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MouseMove(object sender, MouseEventArgs e)
        {
            // If there is a current widget and we have capture then we are moving a control
            if (Capture && (currentWidget != null))
            {
                if (currentActionMove)
                {
                    // Move the control
                    currentWidget.SetBounds(currentWidget.Bounds.X + (e.Location.X - currentPosition.X),
                        currentWidget.Bounds.Y + (e.Location.Y - currentPosition.Y), 0, 0,
                        BoundsSpecified.Location);
                }
                else
                {
                    // Resize the control
                    currentWidget.SetBounds(0, 0, currentWidget.Width + (e.Location.X - currentPosition.X),
                        currentWidget.Height + (e.Location.Y - currentPosition.Y),
                        BoundsSpecified.Size);
                }

                // Set the modified flag
                Modified = true;

                currentWidget.Refresh();

                // Update the current mouse position
                currentPosition = e.Location;
            }
        }

        /// <summary>
        /// Click event handler for the edit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            // Toggle the state and update
            toolStripButtonEdit.Checked = toolStripButtonEdit.Checked == false;
            UpdateWidgetTools();
            UpdateButtons();
        }

        /// <summary>
        /// Clicke event handler for the delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            // Delete the current widget
            DeleteCurrentWidget();
        }

        /// <summary>
        /// Mouse move event handler for a draggable button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_MouseMove(object sender, MouseEventArgs e)
        {
            // Check to see if we clicked and are dragging
            if (buttonDragCapture && (sender is ToolStripButton))
            {
                DoDragDrop(new WidgetHolder((sender as ToolStripButton).Tag as string), 
                    DragDropEffects.Copy);

                buttonDragCapture = false;
            }
        }

        /// <summary>
        /// Mouse down event handler for a draggable button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_MouseDown(object sender, MouseEventArgs e)
        {
            // Set the capture flag
            buttonDragCapture = true;
        }

        /// <summary>
        /// Mouse up event handler for a draggable button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_MouseUp(object sender, MouseEventArgs e)
        {
            // Clear the capture flag
            buttonDragCapture = false;
        }

        /// <summary>
        /// Mouse leave event handler for a draggable button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_MouseLeave(object sender, EventArgs e)
        {
            // Clear the capture flag
            buttonDragCapture = false;
        }

        /// <summary>
        /// Click event handler for a draggable button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_Click(object sender, EventArgs e)
        {
            // Add the widget in the center of the form
            AddWidget(new WidgetHolder((sender as ToolStripButton).Tag as string),
                new Point(ClientSize.Width / 2, ClientSize.Height / 2));
        }

        /// <summary>
        /// Drag enter event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_DragEnter(object sender, DragEventArgs e)
        {
            // Check for a widget holder
            if (e.Data.GetDataPresent(typeof(WidgetHolder)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Drag drop event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_DragDrop(object sender, DragEventArgs e)
        {
            // Add the passed widget
            AddWidget(e.Data.GetData(typeof(WidgetHolder)) as WidgetHolder,
                PointToClient(new Point(e.X, e.Y)));
        }

        /// <summary>
        /// Move event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_Move(object sender, EventArgs e)
        {
            // Set the modified flag
            Modified = true;
        }

        /// <summary>
        /// Resize event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_Resize(object sender, EventArgs e)
        {
            // Set the modified flag
            Modified = true;
        }
    }
}
