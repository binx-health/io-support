/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;

namespace IO.View.Concrete
{
    /// <summary>
    /// Fault form
    /// </summary>
    public partial class Fault : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Fault()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Visible = false;
            panelMessage.Text = Properties.Resources.Fault;
            pictureBoxHome.Visible = false;
            pictureBoxLogin.Visible = false;
            pictureBoxBack.Visible = false;
            pictureBoxHelp.Visible = false;
        }

        /// <summary>
        /// Update the status on the form
        /// </summary>
        protected override void UpdateStatus()
        {
            labelStatus.Text = LastError.Message;
        }

        /// <summary>
        /// Logout the current user
        /// </summary>
        public override void Logout()
        {
            // Do not logout the current user where a fault has occurred
        }

        /// <summary>
        /// Click event handler for the shutdown button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonShutdown_Click(object sender, EventArgs e)
        {
            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Shutdown));
        }
    }
}
