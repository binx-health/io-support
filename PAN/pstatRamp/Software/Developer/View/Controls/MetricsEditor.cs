/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using IO.Scripting;

namespace IO.View.Controls
{
    /// <summary>
    /// Metrics editor control
    /// </summary>
    public partial class MetricsEditor : UserControl
    {
        /// <summary>
        /// The associated script
        /// </summary>
        private IMetrics metrics;

        /// <summary>
        /// The current in-place editor
        /// </summary>
        private TextBox editor = null;

        /// <summary>
        /// The bold font for base values
        /// </summary>
        private Font boldFont;

        /// <summary>
        /// The associated script
        /// </summary>
        public IMetrics Metrics
        {
            get
            {
                return metrics;
            }
            set
            {
                // Check for a new script
                if (metrics != value)
                {
                    // Set the new value
                    metrics = value;
                }

                // Update the controls
                UpdateControls();
            }
        }

        /// <summary>
        /// Read only flag
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetricsEditor()
        {
            InitializeComponent();

            // Initialise the bold font
            boldFont = new Font(Font, FontStyle.Bold);
        }

        /// <summary>
        /// Update the controls to reflect the current script
        /// </summary>
        private void UpdateControls()
        {
            // Clear any errors
            listView.Items.Clear();

            // Check for a null script
            if (metrics != null)
            {
                // Initialise the select flag
                bool select = true;

                // Loop through the metrics
                foreach (var name in metrics.Names)
                {
                    // Add the list item
                    var listViewItem = listView.Items.Add(new ListViewItem()
                    {
                        Name = name,
                        Text = name,
                        ImageIndex = 0,
                        Selected = select,
                    });
                    listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem());

                    // Clear the select flag
                    select = false;
                }
            }
        }

        /// <summary>
        /// Create an editor for the current selection
        /// </summary>
        private void CreateEditor()
        {
            // Check for a selected item
            if (listView.SelectedItems.Count == 1)
            {
                // Get the item
                var listViewItem = listView.SelectedItems[0];

                // Close any current editor
                CloseEditor();

                // Create an editor
                editor = new TextBox() { Bounds = listViewItem.SubItems[1].Bounds, Tag = listViewItem.Name };
                bool inherited = false;
                int intValue;

                // Check for an integer metric
                if (metrics.TryGetIntegerMetric(listViewItem.Name, out intValue, out inherited))
                {
                    // Setup for integer
                    editor.Text = intValue.ToString();
                    editor.TextAlign = HorizontalAlignment.Right;
                }
                else
                {
                    // Setup for text
                    editor.Text = metrics.GetMetric(listViewItem.Name, out inherited);
                    editor.TextAlign = HorizontalAlignment.Left;
                }

                // Set the font to bold for a base value
                if (inherited == false)
                {
                    editor.Font = boldFont;
                }

                // Add the editor
                listView.Controls.Add(editor);

                // Set the focus
                editor.Focus();
                editor.LostFocus += editor_LostFocus;
                editor.KeyPress += editor_KeyPress;
                editor.KeyDown += editor_KeyDown;
            }
        }

        /// <summary>
        /// Close the current editor, saving any changes
        /// </summary>
        /// <param name="save">Flag to save the value</param>
        private void CloseEditor(bool save = true)
        {
            // Check for no editor
            if (editor == null)
            {
                return;
            }

            // Remember the editor
            var oldEditor = editor;

            // Dereference the editor to avoid re-entrant code
            editor = null;

            // Check the save flag
            if (save)
            {
                // Intialise the name, value and metric variables
                string name = oldEditor.Tag as string;
                string value = oldEditor.Text.Trim();
                bool inherited = false;
                int intValue;

                // Check for an empty string
                if (string.IsNullOrEmpty(value))
                {
                    // Clear down this value
                    metrics.SetMetric(name, "");
                }
                // Check for an integer metric
                else if (metrics.TryGetIntegerMetric(name, out intValue, out inherited))
                {
                    // Get the new value and compare
                    int newIntValue;

                    if (int.TryParse(value, out newIntValue) && (newIntValue != intValue))
                    {
                        // Set the new value and notify
                        metrics.SetIntegerMetric(name, newIntValue);
                        metrics.Modified = true;
                        metrics.Loaded = false;

                        MessageQueue.Instance.Push(new MenuItemMessage("MetricsEdit")
                        {
                            Parameters = new Dictionary<string, object>() { { Metrics.Name, Metrics } }
                        });
                    }
                }
                // Check for a new string value
                else if (value != metrics.GetMetric(name, out inherited))
                {
                    // Set the new value and notify
                    metrics.SetMetric(name, value);
                    metrics.Modified = true;
                    metrics.Loaded = false;

                    MessageQueue.Instance.Push(new MenuItemMessage("MetricsEdit")
                    {
                        Parameters = new Dictionary<string, object>() { { Metrics.Name, Metrics } }
                    });
                }
            }

            // Remove the olde editor and dispose it
            listView.Controls.Remove(oldEditor);
            oldEditor.Dispose();
        }

        /// <summary>
        /// Draw sub item event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // Initialise the text properties
            TextFormatFlags format = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            Color textColor;
            Font font = Font;

            // Initialise the metrics variables
            string value = e.SubItem.Text;
            bool inherited = false;

            // Check for the name column
            if (e.ColumnIndex == 0)
            {
                format |= TextFormatFlags.Left;
            }
            else
            {
                // try to read an integer value
                int intValue;

                if (metrics.TryGetIntegerMetric(e.Item.Name, out intValue, out inherited))
                {
                    // Set the integer value and right align
                    value = intValue.ToString();
                    format |= TextFormatFlags.Right;
                }
                else
                {
                    // Set the string value and left align
                    value = metrics.GetMetric(e.Item.Name, out inherited);
                    format |= TextFormatFlags.Left;
                }

                // Switch to bold font for base values
                if (inherited == false)
                {
                    font = boldFont;
                }
            }

            // Check for a selected item, draw the appropriate background and set the tect colour
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                textColor = SystemColors.HighlightText;
            }
            else
            {
                e.DrawBackground();
                textColor = inherited ? SystemColors.GrayText : ForeColor;
            }

            // Draw the text
            TextRenderer.DrawText(e.Graphics, value, font, e.Bounds, textColor, format);
        }

        /// <summary>
        /// Draw column header event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        /// <summary>
        /// Lost focus event handler for the edit box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editor_LostFocus(object sender, EventArgs e)
        {
            // Close the editor once focus is lost
            CloseEditor();
        }

        /// <summary>
        /// Key press event handler for the edit box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editor_KeyPress(Object sender, KeyPressEventArgs e)
        {
            // Close the edit box if the return jey is pressed
            if (e.KeyChar == '\r')
            {
                CloseEditor();
            }
            else if (e.KeyChar == 27)
            {
                CloseEditor(false);
            }
        }

        /// <summary>
        /// Key down event handler for the edit box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editor_KeyDown(Object sender, KeyEventArgs e)
        {
            // Close the edit box if the return jey is pressed
            if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Up))
            {
                CloseEditor(true);
            }
        }

        /// <summary>
        /// Column width changing event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            // Close the editor if the column widths are being changed
            CloseEditor();
        }

        /// <summary>
        /// Scroll event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_Scroll(object sender, ScrollEventArgs e)
        {
            // Close the editor if the list view is scrolled
            CloseEditor();
        }

        /// <summary>
        /// Click event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            // Check for a read-only editor
            if (ReadOnly == false)
            {
                // Create an editor
                CreateEditor();

                // If an editor exists then select the text
                if (editor != null)
                {
                    editor.SelectAll();
                }
            }
        }

        /// <summary>
        /// Key press event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a read-only editor
            if (ReadOnly == false)
            {
                // Check for a non-control character
                if (char.IsControl(e.KeyChar) == false)
                {
                    // Create an editor
                    CreateEditor();

                    // If an editor exists then append the character
                    if (editor != null)
                    {
                        editor.Text = string.Empty;
                        editor.AppendText(e.KeyChar.ToString());
                    }
                }
            }
        }
    }
}
