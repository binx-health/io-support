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
    public partial class QcTestDismissal : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public QcTestDismissal()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.QcTestDismissal;

            // Set the message text
            panelMessage.Text = Properties.Resources.QcTestDismissalText;
        }

        /// <summary>
        /// Click event handler for the unlock button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }

        /// <summary>
        /// Click event handler for the back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBack_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Back));
        }
    }
}
