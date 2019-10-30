/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View.Concrete
{
    public partial class PasswordExpired : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public PasswordExpired()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.PasswordExpired;

            // Set the message text
            panelMessage.Text = Properties.Resources.PasswordExpiredConfirm;
        }

        /// <summary>
        /// Click event handler for the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Login));
        }

        /// <summary>
        /// Click event handler for the renew button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRenew_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }
    }
}
