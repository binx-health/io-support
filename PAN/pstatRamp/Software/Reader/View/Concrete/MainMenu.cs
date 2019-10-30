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
    public partial class MainMenu : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.MainMenu;
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
