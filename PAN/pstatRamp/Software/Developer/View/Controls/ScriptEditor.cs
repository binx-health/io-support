/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using IO.Scripting;

namespace IO.View.Controls
{
    /// <summary>
    /// Script editor control
    /// </summary>
    public partial class ScriptEditor : UserControl
    {
        /// <summary>
        /// The associated script
        /// </summary>
        private IScript script;

        /// <summary>
        /// The modified flag
        /// </summary>
        private bool modified = false;

        /// <summary>
        /// The associated script
        /// </summary>
        public IScript Script
        {
            get
            {
                return script;
            }
            set
            {
                // Check for a new script
                if (script != value)
                {
                    // If we have modified this script then nofity the change
                    if (modified)
                    {
                        MessageQueue.Instance.Push(new MenuItemMessage("ScriptEdit")
                        {
                            Parameters = new Dictionary<string, object>() { { Script.Name, textBox.Text } }
                        });

                        modified = false;
                    }

                    // Set the new value
                    script = value;
                }

                // Update the controls
                UpdateControls();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScriptEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get the current command and optionally select it
        /// </summary>
        /// <param name="select">True to select the current command</param>
        /// <returns>The current command</returns>
        public string CurrentCommand(bool select = false)
        {
            // Check for an invalid selection
            if ((textBox.SelectionStart < 0) || (textBox.SelectionStart >= textBox.Text.Length))
            {
                // Optionally select the end of the script
                if (select)
                {
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                    textBox.ScrollToCaret();
                }

                return null;
            }

            // Start searching from the selection start
            int start = textBox.SelectionStart;

            // Search backwards for a line-feed
            while (start > 0)
            {
                if (textBox.Text[start - 1] == '\n')
                {
                    break;
                }

                --start;
            }

            // Start searching from the selection end
            int end = Math.Min(textBox.Text.Length, textBox.SelectionStart + textBox.SelectionLength);

            // Search forwards for a line-feed
            while (end < textBox.Text.Length)
            {
                if (textBox.Text[end] == '\n')
                {
                    break;
                }

                ++end;
            }

            // Optionally select the command
            if (select)
            {
                textBox.SelectionStart = start;
                textBox.SelectionLength = end - start;
                textBox.ScrollToCaret();
            }

            // Return the command text
            return textBox.Text.Substring(start, end - start).Trim();
        }

        /// <summary>
        /// Select and return the next command
        /// </summary>
        /// <returns>The next command</returns>
        public string NextCommand()
        {
            // Initialise the result and the start position
            string result = "";
            int start = Math.Min(textBox.SelectionStart + textBox.SelectionLength, textBox.Text.Length);

            // Loop until we get to the end or we find a non-empty command
            while ((result != null) && (result == ""))
            {
                // Search forwards for a line feed
                while (start < textBox.Text.Length)
                {
                    if (textBox.Text[start++] == '\n')
                    {
                        break;
                    }
                }

                // Check for the end of the script
                if (start == textBox.Text.Length)
                {
                    // Set the result
                    result = null;

                    // Select the end of the script
                    textBox.SelectionStart = start;
                    textBox.SelectionLength = 0;
                    textBox.ScrollToCaret();
                }
                else
                {
                    // Search forward for the next line-feed
                    int end = start;

                    while (end < textBox.Text.Length)
                    {
                        if (textBox.Text[end] == '\n')
                        {
                            break;
                        }

                        ++end;
                    }

                    // Set the result
                    result = textBox.Text.Substring(start, end - start).Trim();

                    // Select the command
                    textBox.SelectionStart = start;
                    textBox.SelectionLength = end - start;
                    textBox.ScrollToCaret();
                }
            }

            return result;
        }

        /// <summary>
        /// Update the controls to reflect the current script
        /// </summary>
        private void UpdateControls()
        {
            // Clear any errors
            listBox.Items.Clear();

            // Check for a null script
            if (script == null)
            {
                // Clear the data and disable the controls
                textBox.Text = null;
                textBox.Enabled = false;
                listBox.Enabled = false;
            }
            else
            {
                // Check for a modified value and set this unless we have already modified the script
                if ((modified == false) && (textBox.Text != script.Value))
                {
                    textBox.Text = script.Value;

                    // Select the current command
                    CurrentCommand(true);
                }

                // Enable editing
                textBox.Enabled = true;
                textBox.ReadOnly = (Main.Instance.EditSupportingScripts == false) &&
                     IAssays.SupportingScripts.Contains(script.Name, 
                     StringComparer.CurrentCultureIgnoreCase);

                // Add any errors to the list
                foreach (var error in script.Errors)
                {
                    listBox.Items.Add(error);
                }

                // Enable error selection
                listBox.Enabled = true;
            }
        }

        /// <summary>
        /// Selected index changed event handler for the list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check for a valid selection
            if (listBox.SelectedIndex == -1)
            {
                textBox.SelectionLength = 0;

                return;
            }

            // Get the error
            var scriptError = listBox.Items[listBox.SelectedIndex] as ScriptError;

            // Check for a line based error
            if (scriptError.Line < 0)
            {
                // Clear any selection
                textBox.SelectionLength = 0;
            }
            else
            {
                // Search for the error line
                int position = 0;
                int line = 0;

                while ((line < scriptError.Line) && (position < textBox.Text.Length))
                {
                    var character = textBox.Text[position++];

                    if (character == '\n')
                    {
                        ++line;
                    }
                }

                // Check if we found the line
                if (line != scriptError.Line)
                {
                    // Clear any selection
                    textBox.SelectionLength = 0;
                }
                else
                {
                    // Set the selection start
                    textBox.SelectionStart = position;

                    // Search for the end position
                    while (position < textBox.Text.Length)
                    {
                        var character = textBox.Text[position++];

                        if (character == '\n')
                        {
                            break;
                        }
                    }

                    // Set the selection length
                    textBox.SelectionLength = position - textBox.SelectionStart;
                    textBox.ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// Text changed event for the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            // Check for a valid script
            if (Script != null)
            {
                // Reset the timer
                timer.Enabled = false;
                timer.Enabled = true;

                // Set the modified flag
                modified = true;
            }
        }

        /// <summary>
        /// Tick event handler for the text changed timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Disable the timer
            timer.Enabled = false;

            // Notify of the change
            MessageQueue.Instance.Push(new MenuItemMessage("ScriptEdit")
            {
                Parameters = new Dictionary<string, object>() { { Script.Name, textBox.Text } }
            });

            // Clear the modified flag
            modified = false;
        }
    }
}
