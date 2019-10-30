/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Review test input form
    /// </summary>
    public partial class ReviewTest : Form
    {
        public ReviewTest()
        {
            InitializeComponent();

            lock (ITests.Instance.CurrentTest)
            {
                // Initialise the inherited controls
                titleBar.Text = ITests.Instance.CurrentTest.Result.QcTest ?
                    Properties.Resources.ReviewQcTest : Properties.Resources.ReviewTest;

                // Get the name field
                var patientIdField = IConfiguration.Instance.Fields.Where(x => x.Name == "PatientId").FirstOrDefault();

                // Check the policy on the name field
                if ((patientIdField != null) && (patientIdField.Policy != IO.Model.Serializable.FieldPolicy.Ignore))
                {
                    textBoxPatientId.Title = Auxilliary.DisplayNameForField("PatientId");

                    // Check for a patient name and show it in the box
                    object name;

                    if (ITests.Instance.CurrentTest.Result.PatientInformation.TryGetValue("PatientId", out name))
                    {
                        textBoxPatientId.Text = name.ToString();
                    }
                }
                else
                {
                    textBoxPatientId.Visible = false;
                }

                // Display the test data on the form
                textBoxSampleId.Text = ITests.Instance.CurrentTest.Result.SampleId;

                // Set the assay name
                textBoxAssay.Text = ITests.Instance.CurrentTest.Result.Assay.Name;

                // Set the QC test flag
                checkBoxQcTest.Checked = ITests.Instance.CurrentTest.Result.QcTest;
                checkBoxQcTest.Enabled = checkBoxQcTest.Checked == false;
            }
        }

        /// <summary>
        /// Click event handler for the run test button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRunTest_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next) 
                { Parameters = new Dictionary<string, object>() { { "QcTest", checkBoxQcTest.Checked } } });
        }

        /// <summary>
        /// Click event handler for the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Show the logout confirmation dialog and check the result
            if (ITests.Instance.CurrentTest.AbandonTestSetup())
            {
                // Issue the abort command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Abort));
            }
        }

        /// <summary>
        /// Click event handler for the edit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            IssueMenuItem("PatientInformationEdit");
        }

        /// <summary>
        /// Click event handler for the QC test check-box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxQcTest_Click(object sender, EventArgs e)
        {
            // Toggle the check-box
            checkBoxQcTest.Checked = checkBoxQcTest.Checked == false;
            checkBoxQcTest.Refresh();
        }
    }
}
