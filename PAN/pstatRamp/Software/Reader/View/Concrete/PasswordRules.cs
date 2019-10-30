/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Password rules form
    /// </summary>
    public partial class PasswordRules : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public PasswordRules()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.PasswordRules;

            // Initialise the values on the form
            textBoxMinimumLength.Text = IConfiguration.Instance.PasswordRules.MinimumLength.ToString();
            textBoxMinimumAlphabetical.Text = 
                IConfiguration.Instance.PasswordRules.MinimumAlphabetical.ToString();
            textBoxExpiryTimeInDays.Text = IConfiguration.Instance.PasswordRules.ExpiryTimeInDays.ToString();
            checkBoxOverride.Checked = IConfiguration.Instance.PasswordRules.Override;
        }

        /// <summary>
        /// Click event handler for text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Click(object sender, EventArgs e)
        {
            // Get the text box
            var textBox = (TextBox)sender;

            // Initialise the keyboard form
            var keypad = new Keypad(textBox.Title, textBox.Text);

            // Reparent the controls
            keypad.Reparent();

            // Show the form
            keypad.ShowDialog();

            // Update the text
            textBox.Text = keypad.Text;
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
            {
                Parameters = new Dictionary<string, object>() 
                {
                    { "MinimumLength", int.Parse(textBoxMinimumLength.Text) },
                    { "MinimumAlphabetical", int.Parse(textBoxMinimumAlphabetical.Text) },
                    { "ExpiryTimeInDays", int.Parse(textBoxExpiryTimeInDays.Text) },
                    { "Override", checkBoxOverride.Checked },
                },
            });
        }

        /// <summary>
        /// Override button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxOverride_Click(object sender, EventArgs e)
        {
            checkBoxOverride.Checked = checkBoxOverride.Checked == false;
            checkBoxOverride.Refresh();
        }
    }
}
