/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using IO.Scripting;

namespace IO.View.Controls
{
    public partial class AssayEditor : UserControl
    {
        /// <summary>
        /// The associated script
        /// </summary>
        private IAssay assay;

        /// <summary>
        /// The current in-place editor
        /// </summary>
        private TextBox editor = null;

        /// <summary>
        /// The associated script
        /// </summary>
        public IAssay Assay
        {
            get
            {
                return assay;
            }
            set
            {
                // Check for a new assay
                if (assay != value)
                {
                    // Set the new value
                    assay = value;
                }

                // Update the controls
                UpdateControls();
            }
        }

        // Default constructor
        public AssayEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update the controls to reflect the current script
        /// </summary>
        private void UpdateControls()
        {
            // Check for an assay
            if (assay == null)
            {
                Enabled = false;
                textBoxName.Text = string.Empty;
                textBoxVersion.Text = string.Empty;
                textBoxScript.Text = string.Empty;
                textBoxUngScript.Text = string.Empty;
                textBoxVoltammetryScript.Text = string.Empty;
                textBoxMetrics.Text = string.Empty;
                listView.Items.Clear();
            }
            else
            {
                Enabled = true;
                textBoxName.Text = assay.Name;
                textBoxVersion.Text = assay.Version.ToString();
                textBoxScript.Text = assay.Script;
                textBoxUngScript.Text = assay.UngScript;
                textBoxVoltammetryScript.Text = assay.VoltammetryScript;
                textBoxMetrics.Text = assay.MetricsName;
                textBoxShortName.Text = assay.ShortName;
                textBoxCode.Text = assay.Code;
                textBoxEstimatedDuration.Text = assay.EstimatedDuration.ToString();

                // Loop through the diseases adding them
                int index = 0;

                foreach (var disease in assay.Diseases)
                {
                    // Check for an existing item
                    if (index < listView.Items.Count)
                    {
                        // Get the item
                        var item = listView.Items[index];

                        // Check for a match
                        if (item.Tag == disease)
                        {
                            // Update the values
                            if (item.Text != disease.PeakName)
                            {
                                item.Text = disease.PeakName;
                            }

                            if (item.SubItems[1].Text != disease.Loinc)
                            {
                                item.SubItems[1].Text = disease.Loinc;
                            }
                        }
                        // Otherwise insert a new item at this point
                        else
                        {
                            item = listView.Items.Insert(index, disease.PeakName);
                            item.Tag = disease;
                            item.SubItems[0].Tag = item;
                            item.SubItems.Add(disease.Loinc).Tag = item;
                        }
                    }
                    else
                    {
                        // Add a new item
                        var item = listView.Items.Add(disease.PeakName);
                        item.Tag = disease;
                        item.SubItems[0].Tag = item;
                        item.SubItems.Add(disease.Loinc).Tag = item;
                    }

                    // Increment the index
                    ++index;
                }

                // Remove any additional items in the list
                while (index < listView.Items.Count)
                {
                    listView.Items.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Notify that the assay has changed
        /// </summary>
        private void Notify()
        {
            // Set the modified and loaded flags
            assay.Modified = true;
            assay.Loaded = false;

            MessageQueue.Instance.Push(new MenuItemMessage("AssayEdit")
            {
                Parameters = new Dictionary<string, object>() { { Assay.Name, Assay } }
            });
        }

        /// <summary>
        /// Create an editor for the current selection
        /// </summary>
        /// <param name="column">The column clicked</param>
        private void CreateEditor(int column)
        {
            // Check for a selected item
            if ((listView.SelectedItems.Count == 1) && (column < listView.Columns.Count))
            {
                // Get the item
                var listViewItem = listView.SelectedItems[0];

                // Close any current editor
                CloseEditor();

                // Create an editor
                editor = new TextBox() { 
                    Bounds = listViewItem.SubItems[column].Bounds,
                    Tag = listViewItem.SubItems[column],
                    Text = listViewItem.SubItems[column].Text,
                };

                // Correct the width
                editor.SetBounds(0, 0, listView.Columns[column].Width, 0, BoundsSpecified.Width);

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
                // Get the sub-item, item, disease and value
                var listViewSubItem = oldEditor.Tag as ListViewItem.ListViewSubItem;
                var listViewItem = listViewSubItem.Tag as ListViewItem;
                var disease = listViewItem.Tag as IDisease;
                var value = oldEditor.Text.Trim();

                // Check for a LOINC edit
                if (listViewItem.SubItems.IndexOf(listViewSubItem) == 1)
                {
                    if (disease.Loinc != value)
                    {
                        listViewSubItem.Text = value;
                        disease.Loinc = value;

                        Notify();
                    }
                }
                // Check for a value
                else if (string.IsNullOrEmpty(value))
                {
                    // Check for a new row
                    if (disease == null)
                    {
                        // Delete the row
                        listView.Items.Remove(listViewItem);
                    }
                }
                else
                {
                    // Set the value in the list view
                    listViewSubItem.Text = value;

                    // Check for a new row
                    if (disease == null)
                    {
                        // Set the tag to the new disease and notify
                        listViewItem.Tag = assay.AddDisease(value);

                        Notify();
                    }
                    // Check for a name change
                    else if (disease.PeakName != value)
                    {
                        // Set the new name and notify
                        disease.PeakName = value;

                        Notify();
                    }
                }
            }

            // Remove the olde editor and dispose it
            listView.Controls.Remove(oldEditor);
            oldEditor.Dispose();
        }

        /// <summary>
        /// Key press event handler for any text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a carriage return
            if (e.KeyChar == '\r')
            {
                SelectNextControl(sender as Control, true, true, true, true);
            }
            // Check for backspace
            else if (e.KeyChar == '\b')
            {
            }
            // Check for spaces
            else if ((e.KeyChar == ',') || (e.KeyChar == ' ') ||
                Path.GetInvalidFileNameChars().Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Leave event handler for the name text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxName_Leave(object sender, EventArgs e)
        {
            // Check for a value
            if (string.IsNullOrEmpty(textBoxName.Text) || 
                textBoxName.Text.Contains(',') || textBoxName.Text.Contains(' ') ||
                textBoxName.Text.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                textBoxName.Text = assay.Name;
            }
            // Check for a change
            else if (assay.Name != textBoxName.Text)
            {
                // Update the value and notify
                assay.Name = textBoxName.Text;
                Notify();
            }
        }

        /// <summary>
        /// Leave event handler forthe version text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVersion_Leave(object sender, EventArgs e)
        {
            int newVersion;

            if ((int.TryParse(textBoxVersion.Text, out newVersion) == false) ||
                (newVersion < assay.Version))
            {
                textBoxVersion.Text = assay.Version.ToString();
            }
            else if (assay.Version != newVersion)
            {
                assay.Version = newVersion;

                Notify();
            }
        }

        /// <summary>
        /// Leave event handler for the script text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxScript_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxScript.Text) ||
                textBoxScript.Text.Contains(',') || textBoxScript.Text.Contains(' ') ||
                textBoxScript.Text.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                textBoxScript.Text = assay.Script;
            }
            else if (assay.Script != textBoxScript.Text)
            {
                assay.Script = textBoxScript.Text;

                Notify();
            }
        }

        /// <summary>
        /// Leave event handler for the UNG script text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxUngScript_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUngScript.Text) ||
                textBoxUngScript.Text.Contains(',') || textBoxUngScript.Text.Contains(' ') ||
                textBoxUngScript.Text.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                textBoxUngScript.Text = assay.UngScript;
            }
            else if (assay.UngScript != textBoxUngScript.Text)
            {
                assay.UngScript = textBoxUngScript.Text;

                Notify();
            }
        }

        /// <summary>
        /// Leave event handler for the voltammetry script text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVoltammetryScript_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxVoltammetryScript.Text) ||
                textBoxVoltammetryScript.Text.Contains(',') || textBoxVoltammetryScript.Text.Contains(' ') ||
                textBoxVoltammetryScript.Text.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                textBoxVoltammetryScript.Text = assay.VoltammetryScript;
            }
            else if (assay.VoltammetryScript != textBoxVoltammetryScript.Text)
            {
                assay.VoltammetryScript = textBoxVoltammetryScript.Text;

                Notify();
            }
        }

        /// <summary>
        /// Leave event handler for the metrics text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxMetrics_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxMetrics.Text) ||
                textBoxMetrics.Text.Contains(',') || textBoxMetrics.Text.Contains(' ') ||
                textBoxMetrics.Text.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                textBoxMetrics.Text = assay.MetricsName;
            }
            else if (assay.MetricsName != textBoxMetrics.Text)
            {
                assay.MetricsName = textBoxMetrics.Text;

                Notify();
            }
        }

        /// <summary>
        /// Leave event handler for the short name text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxShortName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxShortName.Text) ||
                textBoxShortName.Text.Contains(',') || textBoxShortName.Text.Contains(' ') ||
                textBoxShortName.Text.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                textBoxShortName.Text = assay.ShortName;
            }
            else if (assay.ShortName != textBoxShortName.Text)
            {
                assay.ShortName = textBoxShortName.Text;

                Notify();
            }
        }

        /// <summary>
        /// Leave event handler for the code text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCode_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCode.Text) ||
                textBoxCode.Text.Contains(',') || textBoxCode.Text.Contains(' ') ||
                textBoxCode.Text.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                textBoxCode.Text = assay.Code;
            }
            else if (assay.Code != textBoxCode.Text)
            {
                assay.Code = textBoxCode.Text;

                Notify();
            }
        }

        /// <summary>
        /// Leave event handler for the esitmated duration text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxEstimatedDuration_Leave(object sender, EventArgs e)
        {
            int newEstimatedDuration;

            if (int.TryParse(textBoxEstimatedDuration.Text, out newEstimatedDuration) == false)
            {
                textBoxVersion.Text = assay.Version.ToString();
            }
            else if (assay.Version != newEstimatedDuration)
            {
                assay.EstimatedDuration = newEstimatedDuration;

                Notify();
            }
        }

        /// <summary>
        /// Click event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X < listView.Columns[0].Width)
            {
                // Create an editor
                CreateEditor(0);
            }
            else
            {
                // Create an editor
                CreateEditor(1);
            }

            // If an editor exists then select the text
            if (editor != null)
            {
                editor.SelectAll();
            }
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
        /// Draw sub item event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // Initialise the text properties
            Color textColor;

            // Initialise the metrics variables
            string value = e.SubItem.Text;

            // Check for the name column
            if (e.ColumnIndex == 0)
            {
            }
            else
            {
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
                textColor = ForeColor;
            }

            // Draw the text
            TextRenderer.DrawText(e.Graphics, value, Font, e.Bounds, textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.Left);
        }

        /// <summary>
        /// Key press event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for a non-control character
            if (char.IsControl(e.KeyChar) == false)
            {
                // Create an editor
                CreateEditor(0);

                // If an editor exists then append the character
                if (editor != null)
                {
                    editor.Text = string.Empty;
                    editor.AppendText(e.KeyChar.ToString());
                }
            }
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
        /// Click event handler for the add button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            // Close any open editor
            CloseEditor(false);

            // Create a new item
            var item = listView.Items.Add("");
            item.SubItems[0].Tag = item;
            item.SubItems.Add("").Tag = item;

            // Select it
            item.Selected = true;

            // Start editing the name
            CreateEditor(0);
        }

        /// <summary>
        /// Click event handler for the delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            // Close any open editor
            CloseEditor(false);

            // Check for a selected item
            if (listView.SelectedItems.Count == 1)
            {
                // Get the item
                var listViewItem = listView.SelectedItems[0];
                var disease = listViewItem.Tag as IDisease;

                // Remove it
                Assay.Diseases.Remove(disease);
                listView.Items.Remove(listViewItem);
            }
        }

        /// <summary>
        /// Selected index changed event handler for the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the delete button
            toolStripButtonDelete.Enabled = listView.SelectedIndices.Count == 1;
        }
    }
}
