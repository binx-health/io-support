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
    /// Help dialog form
    /// </summary>
    public partial class Help : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="text">The help text to display</param>
        public Help(string text)
        {
            InitializeComponent();

            // Disable the help button
            pictureBoxHelp.Image = Properties.Resources.Help_icon_selected;

            // Set the title text
            titleBar.Text = Properties.Resources.Help;

            // Set the message text
            if (string.IsNullOrEmpty(text))
            {
                panelMessage.Text = Properties.Resources.NoHelp + Properties.Resources.PressBackToReturn;
            }
            else
            {
                panelMessage.Text = text + Properties.Resources.PressBackToReturn;
            }
        }

        /// <summary>
        /// Show the help screen
        /// </summary>
        protected override void ShowHelp()
        {
        }
    }
}
