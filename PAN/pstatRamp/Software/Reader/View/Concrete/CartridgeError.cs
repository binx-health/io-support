/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View.Concrete
{
    public partial class CartridgeError : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CartridgeError()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.CartridgeError;

            // Set the message text
            panelMessage.Text = Properties.Resources.CartridgeErrorConfirm;
        }

        /// <summary>
        /// Click event handler for the yes button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEject_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Back));
        }

        /// <summary>
        /// Click event handler for the yes button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
        }
    }
}
