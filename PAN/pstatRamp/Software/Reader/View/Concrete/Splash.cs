/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using IO.Controller;

namespace IO.View.Concrete
{
    /// <summary>
    /// Splash screen
    /// </summary>
    public partial class Splash : Form
    {
        /// <summary>
        /// Default constructor intialises the inherited controls
        /// </summary>
        public Splash()
        {
            InitializeComponent();

            // Hide the title panel
            titleBar.Visible = false;

            // Initialise the inherited controls
            pictureBoxBack.Visible = false;
            pictureBoxHelp.Visible = false;
            pictureBoxLogin.Visible = false;
            pictureBoxHome.Visible = false;
            pictureBoxAnimation.Left = 531;
            pictureBoxAnimation.Top = 132;
        }

        /// <summary>
        /// Update the date and time on the form
        /// </summary>
        protected override void UpdateStatus()
        {
            // Set the status text to the product version
            labelStatus.Text = string.Format(Properties.Resources.FirmwareVersion,
                Application.ProductVersion);
        }

        /// <summary>
        /// Load event handler sends an initialise message to the controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSplash_Load(object sender, EventArgs e)
        {
            // Send an initialise message to the controller
            MessageQueue.Instance.Push(new CommandMessage(FormName, FormCommand.Initialise));

            // Kick off the wait timer
            timerWait.Enabled = true;
        }
    }
}
