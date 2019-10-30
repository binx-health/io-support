/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Yes\No dialog form
    /// </summary>
    public partial class QcTestDue : Form
    {
        /// <summary>
        /// The logged in user
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public QcTestDue()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.QcTestDue;

            // Set the message text
            panelMessage.Text = Properties.Resources.QcTestDueText;
        }

        /// <summary>
        /// Click event handler for the yes button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonYes_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }

        /// <summary>
        /// Click event handler for the no button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNo_Click(object sender, EventArgs e)
        {
            // Check for an administrator
            if (User.Type == UserType.Administrator)
            {
                // Dismiss the QC test
                IssueMenuItem("QcTestDismiss");
            }
            else
            {
                // Show the QC test dismissal form
                IssueMenuItem("QcTestDismissal");
            }
        }
    }
}
