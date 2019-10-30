/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// Invalid login form
    /// </summary>
    public partial class InvalidLogin : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidLogin()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.InvalidLogin;
            panelMessage.Text = Properties.Resources.InvalidLoginText;
        }

        /// <summary>
        /// Click event handler for the retry button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRetry_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Back));
        }

        /// <summary>
        /// Click event handler for the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
        }
    }
}
