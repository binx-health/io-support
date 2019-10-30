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
    /// Yes\No dialog form
    /// </summary>
    public partial class UnexpectedShutdown : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public UnexpectedShutdown()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.UnexpectedShutdown;

            // Set the message text
            panelMessage.Text = Properties.Resources.UnexpectedShutdownText;

            // Initialise the inherited controls
            pictureBoxBack.Visible = false;
            pictureBoxHelp.Visible = false;
            pictureBoxLogin.Visible = false;
            pictureBoxHome.Visible = false;
        }

        /// <summary>
        /// Logout the current user
        /// </summary>
        public override void Logout()
        {
            // Do not logout the current user while the drawer is open
        }
    }
}
