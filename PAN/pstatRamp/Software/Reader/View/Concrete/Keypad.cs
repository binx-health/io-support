/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;

namespace IO.View.Concrete
{
    /// <summary>
    /// Keyboard form for editing a text value
    /// </summary>
    public partial class Keypad : Form
    {
        /// <summary>
        /// String of valid text characters for text entry
        /// </summary>
        private static readonly string VALID_NUMERIC_CHARACTERS = "0123456789 ";

        /// <summary>
        /// The text being edited on the form which is set in the constructor
        /// </summary>
        public new string Text { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Keypad(string title, string text)
        {
            InitializeComponent();

            // Hide the title panel
            titleBar.Visible = false;

            // Set the text property and the text box
            textBox.Title = title;
            textBox.Text = Text = text;
        }

        /// <summary>
        /// Click event handler for non-special keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void key_Click(object sender, EventArgs e)
        {
            // Get the key object from the sender 
            var key = (Key)sender;

            // Append the character in the correct case and refresh the text box
            textBox.Text += key.Text;
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the space key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keySpace_Click(object sender, EventArgs e)
        {
            // Append a space character refresh the text box
            textBox.Text += " ";
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the clear key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyClear_Click(object sender, EventArgs e)
        {
            // Clear the text and refresh the control
            textBox.Text = string.Empty;
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the backspace key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyBackspace_Click(object sender, EventArgs e)
        {
            // Check for characters to delete
            if (textBox.Text.Length > 0)
            {
                // Delete the last character and refresh the text box
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                textBox.Refresh();
            }
        }

        /// <summary>
        /// Click event handler for the exit key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyExit_Click(object sender, EventArgs e)
        {
            // Close the form without modifying the text
            Close();
        }

        /// <summary>
        /// Click event handler for the enter key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyEnter_Click(object sender, EventArgs e)
        {
            // Modify the text property
            Text = textBox.Text;

            // Close the form
            Close();
        }

        /// <summary>
        /// Key press event handler for the form so that a physical keyboard can be used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormKeyboard_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Check for the backspace key
            if (e.KeyChar == '\b')
            {
                keyBackspace_Click(sender, e);
            }
            // Check for the enter key
            else if (e.KeyChar == '\r')
            {
                keyEnter_Click(sender, e);
            }
            // Check for a valid character
            else if (VALID_NUMERIC_CHARACTERS.Contains(e.KeyChar))
            {
                // Append the character to the text box and refresh it
                textBox.Text += e.KeyChar;
                textBox.Refresh();
            }
        }
    }
}
