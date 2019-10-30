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
    /// <summary>
    /// Login form
    /// </summary>
    public partial class InstrumentName : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InstrumentName()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.InstrumentName;

            // Initialise the text box
            textBox.Text = IConfiguration.Instance.InstrumentName;
        }

        /// <summary>
        /// Click event handler for the Used ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBox.Title, textBox.Text);

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBox.Text = keyboard.Text;
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
            {
                Parameters = new Dictionary<string,object>() { { "InstrumentName", textBox.Text } },
            });
        }
    }
}
