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
    public partial class YesNo : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public YesNo(string title, string message)
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = title;

            // Set the message text
            panelMessage.Text = message;

            // Initialise the result to no
            DialogResult = DialogResult.No;
        }

        /// <summary>
        /// Click event handler for the yes button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonYes_Click(object sender, EventArgs e)
        {
            // Set the result to yes
            DialogResult = DialogResult.Yes;

            // Close the form
            Close();
        }

        /// <summary>
        /// Click event handler for the no button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNo_Click(object sender, EventArgs e)
        {
            // Just close the form
            Close();
        }
    }
}
