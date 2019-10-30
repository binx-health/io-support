/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View.Concrete
{
    public partial class NoNewAssays : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NoNewAssays()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.NoNewAssays;

            // Set the message text
            panelMessage.Text = Properties.Resources.NoNewAssaysHaveBeenFound;
        }

        /// <summary>
        /// Click event handler for the return button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReturn_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Home));
        }
    }
}
