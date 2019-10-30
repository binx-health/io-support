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
    public partial class ReaderQuarantine : Form
    {
        /// <summary>
        /// The logged in user
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ReaderQuarantine()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.ReaderQuarantine;
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="visible">Whether the form is visible</param>
        protected override void ResetForm(bool visible)
        {
            // Set the message text
            if (User.Type == UserType.Administrator)
            {
                panelMessage.Text = Properties.Resources.ReaderQuarantineTextAdmin;
            }
            else
            {
                panelMessage.Text = Properties.Resources.ReaderQuarantineTextUser;
                buttonUnlock.Text = Properties.Resources.Login;
            }
        }

        /// <summary>
        /// Click event handler for the unlock button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUnlock_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next) 
                { Parameters = new Dictionary<string, object>() { { "User", User } } });
        }

        /// <summary>
        /// Click event handler for the back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBack_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
        }
    }
}
