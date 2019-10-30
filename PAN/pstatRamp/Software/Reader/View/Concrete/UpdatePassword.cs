/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using IO.Model.Volatile;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// View\Edit user form
    /// </summary>
    public partial class UpdatePassword : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UpdatePassword()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.EnterNewPassword;

            // Initialise the user ID
            textBoxUserName.Text = ISessions.Instance.CurrentUser.Name;
        }

        /// <summary>
        /// Click event handler for the password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxOldPassword_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBoxOldPassword.Title, string.Empty) { Password = true };

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxOldPassword.Text = keyboard.Text;
            textBoxOldPassword.Refresh();
        }

        /// <summary>
        /// Click event handler for the password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxNewPassword_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBoxNewPassword.Title, string.Empty) { Password = true };

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxNewPassword.Text = keyboard.Text;
            textBoxNewPassword.Refresh();
        }

        /// <summary>
        /// Click event handler for the password verifier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVerify_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBoxVerify.Title, string.Empty) { Password = true };

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxVerify.Text = keyboard.Text;
            textBoxVerify.Refresh();
        }

        /// <summary>
        /// Click event handler for the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Validate the passwords
            if (textBoxNewPassword.Text != textBoxVerify.Text)
            {
                // Show the message window
                var dialog = new Concrete.Message(titleBar.Text, Properties.Resources.NewAndVerifyMustMatch);

                // Reparent controls
                dialog.Reparent();

                // Show the dialog
                dialog.ShowDialog();
            }
            else if ((textBoxNewPassword.Text == textBoxOldPassword.Text) ||
                (IUser.HashPassword(textBoxOldPassword.Text) != ISessions.Instance.CurrentUser.PasswordHash) ||
                (IConfiguration.Instance.PasswordRules.CheckPassword(textBoxNewPassword.Text) == false))
            {
                // Show the invalid credentials dialog
                var dialog = new Concrete.Message(titleBar.Text, Properties.Resources.InvalidCredentials);

                // Reparent controls
                dialog.Reparent();

                // Show the dialog
                dialog.ShowDialog();
            }
            else
            {
                // Issue the save command without a password change
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                {
                    Parameters = new Dictionary<string, object>() 
                    { 
                        { "NewPassword", textBoxNewPassword.Text },
                    }
                });
            }
        }

        /// <summary>
        /// Click event handler for the delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Issue the save command with a password change
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Login));
        }
    }
}
