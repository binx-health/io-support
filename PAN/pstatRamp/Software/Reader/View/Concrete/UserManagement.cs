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
    /// User management menu
    /// </summary>
    public partial class UserManagement : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UserManagement()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.UserManagement;
        }

        /// <summary>
        /// Click event handler for the user account button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUserAccount_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("Users");
        }

        /// <summary>
        /// Click event handler for the password rules button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPasswordRules_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("PasswordRules");
        }

        /// <summary>
        /// Click event handler for the timeout button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTimeout_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("Timeout");
        }
    }
}
