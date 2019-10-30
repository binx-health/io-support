/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// Test complete form
    /// </summary>
    public partial class TestComplete : Form
    {
        public TestComplete()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.TestComplete;

            // Display the test data on the form
            textBoxSampleId.Text = ISessions.Instance.CurrentSession.CurrentTest.Result.SampleId;

            // Get the patient ID field
            var patientIdField = IConfiguration.Instance.Fields.Where(x => x.Name == "PatientId").FirstOrDefault();

            // Check the policy on the patient ID field
            if ((patientIdField != null) && (patientIdField.Policy != IO.Model.Serializable.FieldPolicy.Ignore))
            {
                textBoxPatientId.Title = Auxilliary.DisplayNameForField("PatientId");

                // Check for a patient ID and show it in the box
                object name;

                if (ISessions.Instance.CurrentSession.CurrentTest.Result.PatientInformation.TryGetValue(
                    "PatientId", out name))
                {
                    textBoxPatientId.Text = name.ToString();
                }
            }
            else
            {
                // Hide the name text box
                textBoxPatientId.Visible = false;
            }
        }

        /// <summary>
        /// Click event handler for the view results button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonViewResults_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }
    }
}
