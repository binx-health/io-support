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
    public partial class ContinueBack : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ContinueBack(string title, string message)
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
        /// Click event handler for the continue button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonContinue_Click(object sender, EventArgs e)
        {
            // Set the result to yes
            DialogResult = DialogResult.Yes;

            // Close the form
            Close();
        }

        /// <summary>
        /// Click event handler for the back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBack_Click(object sender, EventArgs e)
        {
            // Just close the form
            Close();
        }
    }
}
