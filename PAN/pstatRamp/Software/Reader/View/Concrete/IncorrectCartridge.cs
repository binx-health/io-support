/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.View.Concrete
{
    public partial class IncorrectCartridge : Form
    {
        /// <summary>
        /// The assay code
        /// </summary>
        public string AssayCode { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public IncorrectCartridge()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.IncorrectCartridge;

            // Set the message text
            panelMessage.Text = Properties.Resources.IncorrectCartridgeConfirm;
        }

        /// <summary>
        /// Click event handler for the add test button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddTest_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
            {
                Parameters = new Dictionary<string, object>() { { "AssayCode", AssayCode } }
            });
        }

        /// <summary>
        /// Click event handler for the yes button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Home));
        }
    }
}
