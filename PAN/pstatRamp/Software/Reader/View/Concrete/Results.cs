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
    /// Generic results form
    /// </summary>
    public partial class Results : Form
    {
        /// <summary>
        /// The column sort order
        /// </summary>
        private List<Sorter> sortOrder;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Results()
        {
            InitializeComponent();

            // Define the initial sort order
            sortOrder = new List<Sorter>() { keyDateTime };
            keyDateTime.ArrowType = ArrowType.Down;
            keyDateTime.Toggled = true;
        }

        /// <summary>
        /// Apply the existing sort
        /// </summary>
        protected void ApplySort()
        {
            // Check if we have clicked sort
            if (sortOrder.Count > 0)
            {
                // Loop through the sort order sorting the list
                foreach (var key in sortOrder.Reverse<Sorter>())
                {
                    // Get the column to sort
                    int column = int.Parse((string)key.Tag);

                    // Sort the data in the relevant direction
                    if (key.ArrowType == ArrowType.Down)
                    {
                        tableTests.Values = tableTests.Values.OrderBy(x => x[column]).ToList();
                    }
                    else
                    {
                        tableTests.Values = tableTests.Values.OrderByDescending(x => x[column]).ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Flush the results file to the number of values required
        /// </summary>
        /// <param name="count">The number of values</param>
        protected void FlushValues(int count)
        {
            // Initialise variables to populate the array
            ITest test = null;

            // Flush the file to the end of the current page
            while ((tableTests.Values.Count < count) &&
                ((test = IResults.Instance.NextResult()) != null))
            {
                // Get the patient ID or the name
                var lockingUser = IUsers.Instance.Where(x => x.ID == test.LockingUserId).FirstOrDefault();
                string lockingUserName = (lockingUser != null) ? lockingUser.Name : string.Empty;

                // Set the data in the array
                tableTests.Values.Add(new object[]
                    {
                        test.Result.StartDateTime,
                        lockingUserName,
                        test.Result.SampleId,
                        (test.Result.Assay == null) ? string.Empty : test.Result.Assay.ShortName,
                        test.Result.SampleId,
                    });
            }

            // Set the scroll bar maximum
            scrollBar.Maximum = (test != null) ? int.MaxValue :
                Math.Max(0, tableTests.Values.Count - tableTests.ValuesPerPage);
        }

        /// <summary>
        /// Click event handler for the view button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonView_Click(object sender, EventArgs e)
        {
            if ((tableTests.SelectedRow >= 0) && (tableTests.SelectedRow < tableTests.Values.Count))
            {
                // Get the test from the sample ID
                var test = IResults.Instance[(string)tableTests.Values[tableTests.SelectedRow][4]];

                // Issue the next command with parameters for editing
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                {
                    // Extract the test from the list by sample id
                    Parameters = new Dictionary<string, object>() { { "Test", test }, },
                });
            }
        }

        /// <summary>
        /// Click event handler for the scroll bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollBar_Click(object sender, EventArgs e)
        {
            // Check for a scroll within the current data
            if ((scrollBar.Position <= tableTests.TopRow) ||
                (scrollBar.Position < tableTests.Values.Count - tableTests.ValuesPerPage))
            {
                tableTests.TopRow = scrollBar.Position;
            }
            // Otherwise try to flush the next page
            else 
            {
                // Flush more values from the file
                FlushValues(scrollBar.Position + tableTests.ValuesPerPage);

                tableTests.TopRow = tableTests.Values.Count - tableTests.ValuesPerPage;
            }

            tableTests.Refresh();
        }

        /// <summary>
        /// Click event handler for the sort keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keySort_Click(object sender, EventArgs e)
        {
            // We are sorting so flush to the end
            FlushValues(int.MaxValue);

            // Get the key and the associated column
            Sorter key = (Sorter)sender;
            int column = int.Parse((string)key.Tag);

            // Move this key to the front of the sort order
            sortOrder.Remove(key);
            sortOrder.Add(key);

            // Switch the direction and perform the sort
            if (key.ArrowType == ArrowType.Down)
            {
                tableTests.Values = tableTests.Values.OrderByDescending(x => x[column]).ToList();

                key.ArrowType = ArrowType.Up;
            }
            else
            {
                tableTests.Values = tableTests.Values.OrderBy(x => x[column]).ToList();

                key.ArrowType = ArrowType.Down;
            }

            // Refresh the table
            tableTests.Refresh();

            // Toggle the key
            foreach (var sortKey in sortOrder)
            {
                sortKey.Toggled = sortKey == key;
                sortKey.Refresh();
            }
        }
    }
}
