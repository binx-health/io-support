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
    /// Main menu form
    /// </summary>
    public partial class Setup : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Setup()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.Setup;
        }

        /// <summary>
        /// Click event handler for the configuration button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConfiguration_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("Configuration");
        }

        /// <summary>
        /// Click event handler for the configuration button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUserManagement_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("UserManagement");
        }

        /// <summary>
        /// Click event handler for the data policy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDataPolicy_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("DataPolicy");
        }

        /// <summary>
        /// Click event handler for the add assays button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddAssays_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("LocalUsbAssay");
        }

        /// <summary>
        /// Click event handler for the server settings button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonServerSettings_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("ServerSettings");
        }
    }
}
