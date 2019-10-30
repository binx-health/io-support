/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.View.Concrete
{
    public partial class LocalUsbAssay : Form
    {
        /// <summary>
        /// The assay code
        /// </summary>
        public string AssayCode { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LocalUsbAssay()
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
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                {
                    Parameters = (AssayCode == null) ? null :
                        new Dictionary<string, object>() { { "AssayCode", AssayCode } }
                });
            }
        }
    }
}
