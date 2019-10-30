/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    public partial class Timeout : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Timeout()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.TimeoutSettings;

            // Initialise the text box
            textBox.Text = (IConfiguration.Instance.AutoLogoffPeriodInSeconds / 60).ToString();
        }

        /// <summary>
        /// Click event handler for the timeout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Click(object sender, EventArgs e)
        {
            // Initialise the timeout spin form
            var timeoutSpin = new TimeoutSpin() { Text = textBox.Text };

            // Reparent the controls
            timeoutSpin.Reparent();

            // Show the form
            timeoutSpin.ShowDialog();

            // Update the text
            textBox.Text = timeoutSpin.Text;
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            int result, value = int.TryParse(textBox.Text, out result) ? result : 0;

            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
            {
                Parameters = new Dictionary<string,object>() { { "Value", value * 60 } },
            });
        }
    }
}
