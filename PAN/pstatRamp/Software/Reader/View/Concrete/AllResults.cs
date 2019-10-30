/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// All results form
    /// </summary>
    public partial class AllResults : Results
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AllResults()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.AllResults;
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="visible">Whether the form is visible</param>
        protected override void ResetForm(bool visible)
        {
            // Call the base class first
            base.ResetForm(visible);

            if (visible)
            {
                // Extract the relevant tests
                IResults.Instance.LockingUserId = (ISessions.Instance.CurrentUser.Type == 
                    UserType.Administrator) ? 0 : ISessions.Instance.CurrentUser.ID;
                IResults.Instance.SampleId = null;
                IResults.Instance.StartDateTime = null;
                IResults.Instance.AssayName = null;
                IResults.Instance.Reset();

                // Set the table users to be a new array
                tableTests.Values = new List<object[]>();

                // Set the scrollbar parameters
                scrollBar.Step = tableTests.ValuesPerPage;

                // Flush values to the current page
                FlushValues(tableTests.TopRow + tableTests.ValuesPerPage);

                // Apply the existing sort
                ApplySort();

                // Limit the scroll bar position and set the top row on the table
                scrollBar.Position = Math.Min(scrollBar.Maximum, scrollBar.Position);
                tableTests.TopRow = scrollBar.Position;

                // Refresh the table
                tableTests.Refresh();
            }
        }

        /// <summary>
        /// Click event handler for the search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            // Issue the back command
            IssueMenuItem("SearchResults");
        }

        /// <summary>
        /// Click event handler for the exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            // Issue the back command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Back));
        }
    }
}
