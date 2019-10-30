/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Load sample form
    /// </summary>
    public partial class ConfirmSample : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConfirmSample()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.ConfirmSample;
        }

        /// <summary>
        /// Click event handler for the next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }

        /// <summary>
        /// Click event handler for the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Show the logout confirmation dialog and check the result
            if (ITests.Instance.CurrentTest.AbandonTestSetup())
            {
                // Issue the abort command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Abort));
            }
        }
    }
}
