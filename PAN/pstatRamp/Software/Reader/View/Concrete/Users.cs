/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Users screen
    /// </summary>
    public partial class Users : Form
    {
        /// <summary>
        /// The column sort order
        /// </summary>
        private List<Sorter> sortOrder;

        /// <summary>
        /// Cache for remembering existing users
        /// </summary>
        private List<string> existingUsers;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Users()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.Users;

            // Define the initial sort order
            sortOrder = new List<Sorter>() { keyType, keyName, keyEnabled };

            // Load the initial existing users
            existingUsers = new List<string>(IUsers.Instance.Select(x => x.Name));
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="visible">Whether the form is visible</param>
        protected override void ResetForm(bool visible)
        {
            // Call the base calss first
            base.ResetForm(visible);

            if (visible)
            {
                // Set the table users to be a new array
                tableUsers.Values = new List<object[]>();

                // Initialise varaibles to populate the array
                string newUser = null;

                // Loop through the users
                foreach (var user in IUsers.Instance)
                {
                    // Set up string values for the user type and enabled status
                    string userType = (user.Type == UserType.Administrator) ? Properties.Resources.Administrator :
                        Properties.Resources.User;
                    string userEnabled = (tableUsers.Values.Count == 0) ? Properties.Resources.PermanentSystem :
                        (user.PasswordExpired ? Properties.Resources.Disabled : Properties.Resources.Enabled);

                    // Set the data in the array
                    tableUsers.Values.Add(new object[] { userType, user.Name, userEnabled });

                    // Check for a new user or a user with an edited name and remember this user
                    if (existingUsers.Contains(user.Name) == false)
                    {
                        newUser = user.Name;
                    }
                }

                // Set the new existing users cache
                existingUsers = new List<string>(IUsers.Instance.Select(x => x.Name));

                // Set the scrollbar parameters
                scrollBar.Step = tableUsers.ValuesPerPage;
                scrollBar.Maximum = Math.Max(0, tableUsers.Values.Count - tableUsers.ValuesPerPage);

                // Loop through the sort order sorting the list
                foreach (var key in sortOrder.Reverse<Sorter>())
                {
                    // Get the column to sort
                    int column = int.Parse((string)key.Tag);

                    // Sort the data in the relevant direction
                    if (key.ArrowType == ArrowType.Down)
                    {
                        tableUsers.Values = tableUsers.Values.OrderBy(x => x[column]).ToList();
                    }
                    else
                    {
                        tableUsers.Values = tableUsers.Values.OrderByDescending(x => x[column]).ToList();
                    }
                }

                // Check for a new user
                if (newUser != null)
                {
                    // Loop through the sorted data looking for the new user
                    for (int index = 0; index < tableUsers.Values.Count; ++index)
                    {
                        if ((string)tableUsers.Values[index][1] == newUser)
                        {
                            // Set the selected row and scroll bar position
                            tableUsers.SelectedRow = index;
                            scrollBar.Position = (index / tableUsers.ValuesPerPage) * tableUsers.ValuesPerPage;
                            break;
                        }
                    }
                }

                // Limit the scroll bar position and set the top row on the table
                scrollBar.Position = Math.Min(scrollBar.Maximum, scrollBar.Position);
                tableUsers.TopRow = scrollBar.Position;

                // Refresh the table
                tableUsers.Refresh();
            }
        }

        /// <summary>
        /// Click event handler for the view button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonView_Click(object sender, EventArgs e)
        {
            if ((tableUsers.SelectedRow >= 0) && (tableUsers.SelectedRow < tableUsers.Values.Count))
            {
                // Get the users ID
                var userName = (string)tableUsers.Values[tableUsers.SelectedRow][1];

                // Remove it from the existing users as it is going to be edited
                existingUsers.Remove(userName);

                // Issue the next command with parameters for editing
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                {
                    Parameters = new Dictionary<string, object>() { { "UserName", userName } },
                });
            }
        }

        /// <summary>
        /// Click event handler for the add new button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            // Issue the next command with parameters for adding
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
        }

        /// <summary>
        /// Click event handler for the scroll bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollBar_Click(object sender, EventArgs e)
        {
            tableUsers.TopRow = scrollBar.Position;
            tableUsers.Refresh();
        }

        /// <summary>
        /// Click event handler for the sort keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keySort_Click(object sender, EventArgs e)
        {
            // Get the key and the associated column
            Sorter key = (Sorter)sender;
            int column = int.Parse((string)key.Tag);

            // Move this key to the front of the sort order
            sortOrder.Remove(key);
            sortOrder.Add(key);

            // Switch the direction and perform the sort
            if (key.ArrowType == ArrowType.Down)
            {
                tableUsers.Values = tableUsers.Values.OrderByDescending(x => x[column]).ToList();
                tableUsers.Refresh();

                key.ArrowType = ArrowType.Up;
                key.Refresh();
            }
            else
            {
                tableUsers.Values = tableUsers.Values.OrderBy(x => x[column]).ToList();
                tableUsers.Refresh();

                key.ArrowType = ArrowType.Down;
                key.Refresh();
            }
        }
    }
}
