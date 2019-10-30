/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View.Concrete
{
    public partial class AssayUpdateFailed : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AssayUpdateFailed()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.AddNewAssay;

            // Set the message text
            panelMessage.Text = Properties.Resources.AssayUpdateFailed;
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Home));
        }
    }
}
