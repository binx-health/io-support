/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// Patient information form
    /// </summary>
    public partial class TestResult : Form
    {
        /// <summary>
        /// Test object
        /// </summary>
        private ITest test = null;

        /// <summary>
        /// Test property
        /// </summary>
        public ITest Test
        {
            get
            {
                return test;
            }
            set
            {
                test = value;

                // Update the values
                UpdateValues();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestResult()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.TestResults;
        }

        /// <summary>
        /// Update the values
        /// </summary>
        private void UpdateValues()
        {
            lock (test)
            {
                // Update the inherited controls
                titleBar.Text = test.Result.QcTest ? Properties.Resources.QcTestResults : 
                    Properties.Resources.TestResults;

                // Set the sample ID and test type
                textBoxSampleId.Text = test.Result.SampleId;
                textBoxTestType.Text = (test.Result.Assay == null) ? string.Empty : test.Result.Assay.Name;

                // Set the result text box
                textBoxResult.Text = Auxilliary.DisplayTextForOutcome(test.Result);

                // Show the run new test button conditionally
                buttonRunNewTest.Visible = (ISessions.Instance.CurrentSession.CurrentTest == test) &&
                    (test.Result.QcTest == false);
            }
        }

        /// <summary>
        /// Update the date and time on the form
        /// </summary>
        protected override void UpdateStatus()
        {
            // Call the base class first
 	        base.UpdateStatus();

        }

        /// <summary>
        /// Click event handler for the exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
        }

        /// <summary>
        /// Click event handler for the run new test button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRunNewTest_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }

        /// <summary>
        /// Click event handler for the print button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Issue the print menu
            IssueMenuItem("PrintOptions", new Dictionary<string, object>() { { "Test", test } });
        }
    }
}
