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
    /// Yes\No dialog form
    /// </summary>
    public partial class Message : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Message(string title, string message)
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = title;

            // Set the message text
            panelMessage.Text = message;

            // Initialise the result to OK
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Click event handler for the no button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            // Just close the form
            Close();
        }
    }
}
