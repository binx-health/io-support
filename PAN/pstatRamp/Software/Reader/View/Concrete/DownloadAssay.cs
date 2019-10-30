/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View.Concrete
{
    public partial class DownloadAssay : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DownloadAssay()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.CheckingForAssays;
            pictureBoxAnimation.Left = 531;
            pictureBoxAnimation.Top = 132;
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="visible">Whether the form is visible</param>
        protected override void ResetForm(bool visible)
        {
            // Call the base class
            base.ResetForm(visible);

            if (visible)
            {
                // Issue the next command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
            }
        }
    }
}
