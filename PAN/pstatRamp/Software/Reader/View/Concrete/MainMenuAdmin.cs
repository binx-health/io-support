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
    public partial class MainMenuAdmin : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainMenuAdmin()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.MainMenuAdmin;
            pictureBoxHome.Image = Properties.Resources.Home_icon_selected;
        }

        /// <summary>
        /// Click event handler for the run test button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRunTest_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("RunTest");
        }

        /// <summary>
        /// Click event handler for the results button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMyResults_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("AllResults");
        }

        /// <summary>
        /// Click event handler for the logout button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            // Create the dialog
            var dialog = new Concrete.YesNo(Properties.Resources.Login, Properties.Resources.LogoutConfirm);

            // Reparent controls
            dialog.Reparent();

            // Show the logout confirmation dialog and check the result
            if (dialog.ShowDialog() == DialogResult.Yes)
            {
                // Issue the login form command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Login));
            }
        }

        /// <summary>
        /// Click event handler for the setup button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetup_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("Setup");
        }

        /// <summary>
        /// Click event handler for the export button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportData_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("Export");
        }

        /// <summary>
        /// Click event handler for the eject button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEject_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("Eject");
        }
    }
}
