/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View.Concrete
{
    /// <summary>
    /// Add new assay type form
    /// </summary>
    public partial class AddNewAssay : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AddNewAssay()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.AddNewAssay;
        }

        /// <summary>
        /// Click event handler for the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
        }

        /// <summary>
        /// Click event handler for the Local - USB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUsb_Click(object sender, EventArgs e)
        {
            IssueMenuItem("LocalUsbAssay");
        }

        /// <summary>
        /// Click event handler for the download button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDownload_Click(object sender, EventArgs e)
        {
            IssueMenuItem("DownloadAssay");
        }
    }
}
