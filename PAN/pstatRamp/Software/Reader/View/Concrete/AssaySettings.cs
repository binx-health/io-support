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
    /// Assay settings form
    /// </summary>
    public partial class AssaySettings : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AssaySettings()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.AssaySettings;

            // Initialise the check boxes
            checkBoxUng.Checked = IConfiguration.Instance.Ung == false;
            checkBoxAutoSampleId.Checked = IConfiguration.Instance.AutoSampleId;
        }

        /// <summary>
        /// Click event handler for the UNG checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUng_Click(object sender, EventArgs e)
        {
            // Toggle the checkbox
            checkBoxUng.Checked = checkBoxUng.Checked ? false : true;
            checkBoxUng.Refresh();
        }

        /// <summary>
        /// Click event handler for the auto sample ID checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxAutoSampleId_Click(object sender, EventArgs e)
        {
            // Toggle the checkbox
            checkBoxAutoSampleId.Checked = checkBoxAutoSampleId.Checked ? false : true;
            checkBoxAutoSampleId.Refresh();
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
                Parameters = new Dictionary<string, object>()
                { 
                    { "Ung", checkBoxUng.Checked == false },
                    { "AutoSampleId", checkBoxAutoSampleId.Checked == true },
                },
            });
        }
    }
}
