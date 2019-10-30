/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace IO.View.Concrete
{
    /// <summary>
    /// Splash screen
    /// </summary>
    public partial class Start : Form
    {
        /// <summary>
        /// Default constructor intialises the inherited controls
        /// </summary>
        public Start()
        {
            InitializeComponent();

            // Hide the title panel
            titleBar.Visible = false;

            // Initialise the inherited controls
            labelStatus.Text = string.Format(Properties.Resources.FirmwareVersion,
                Application.ProductVersion);
        }

        /// <summary>
        /// Click event handler for the login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }

        /// <summary>
        /// Click event handler for the shutdown button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonShutdown_Click(object sender, EventArgs e)
        {
            // Create the dialog
            var dialog = new YesNo(Properties.Resources.Shutdown, Properties.Resources.ShutdownConfirm);

            // Reparent the controls
            dialog.Reparent();

            // Show the shutdown dialog and check the result
            if (dialog.ShowDialog() == DialogResult.Yes)
            {
                // Issue the shutdown command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Shutdown));
            }
        }
    }
}
