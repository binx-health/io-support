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
    /// Searched results form
    /// </summary>
    public partial class SearchedResults : Results
    {
        /// <summary>
        /// The date to search for
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// The user name to search for
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The sample ID to search for
        /// </summary>
        public string SampleId { get; set; }

        /// <summary>
        /// The assay to search for
        /// </summary>
        public string Assay { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchedResults()
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
                // Get the user
                var user = IUsers.Instance.Where(x => x.Name == UserName).FirstOrDefault();

                // Extract the relevant tests
                IResults.Instance.LockingUserId = (UserName == null) ? 0 :
                    ((user == null) ? uint.MaxValue : user.ID);
                IResults.Instance.SampleId = SampleId;
                IResults.Instance.StartDateTime = Date;
                IResults.Instance.AssayName = Assay;
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
        private void buttonNewSearch_Click(object sender, EventArgs e)
        {
            // Issue the back command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Back));
        }

        /// <summary>
        /// Click event handler for the exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            // Issue the back command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
        }
    }
}
