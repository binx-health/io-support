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
    /// Close drawer form
    /// </summary>
    public partial class EmptyDrawer : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EmptyDrawer()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.InstrumentDrawerOpen;

            // Intialise the message
            label.Text = Properties.Resources.EmptyReaderDrawer;
        }

        /// <summary>
        /// Logout the current user
        /// </summary>
        public override void Logout()
        {
            // Do not logout the current user while the drawer is open
        }

        /// <summary>
        /// Click event handler for the continue button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContinue_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }
    }
}
