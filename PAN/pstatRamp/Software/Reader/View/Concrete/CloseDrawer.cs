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
    public partial class CloseDrawer : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CloseDrawer()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.InstrumentDrawerOpen;

            // Intialise the message
            label.Text = Properties.Resources.CloseReaderDrawer;
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
