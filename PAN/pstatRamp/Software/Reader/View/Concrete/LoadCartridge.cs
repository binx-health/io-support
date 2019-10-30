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
    public partial class LoadCartridge : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LoadCartridge()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.LoadCartridge;
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="visible">Whether the form is visible</param>
        protected override void ResetForm(bool visible)
        {
            base.ResetForm(visible);

            // Check for a scanned barcode
            if (visible && (string.IsNullOrEmpty(IState.Instance.ScannedBarcode) == false))
            {
                // Issue the abort command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
            }
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
