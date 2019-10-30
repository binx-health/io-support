/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using IO.Model.Serializable;
using IO.Model.Volatile;
using IO.Scripting;

namespace IO.View.Concrete
{
    /// <summary>
    /// View\Edit user form
    /// </summary>
    public partial class SearchResults : Form
    {
        /// <summary>
        /// The current date value in UTC
        /// </summary>
        DateTime? date;

        /// <summary>
        /// List of assays
        /// </summary>
        private IEnumerable<IAssay> assays = null;

        /// <summary>
        /// Assays property
        /// </summary>
        public IEnumerable<IAssay> Assays
        {
            get
            {
                return assays;
            }
            set
            {
                assays = value;

                // Set the assays in the list box and refresh it
                listBox.Values = value.Select(x => x.Name);
                listBox.Text = listBox.Values.FirstOrDefault();
                listBox.Refresh();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchResults()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.SearchTestResults;

            // If we are not the administrator then limit the search to the current user
            if (ISessions.Instance.CurrentUser.Type != UserType.Administrator)
            {
                textBoxUserName.Text = ISessions.Instance.CurrentUser.Name;
            }
        }

        /// <summary>
        /// Click event handler for the date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxDate_Click(object sender, EventArgs e)
        {
            // Initialise the date form
            var dateForm = new Date(date.HasValue ? date.Value.Date : DateTime.Now.Date);

            // Reparent the controls
            dateForm.Reparent();

            // Show the form
            dateForm.ShowDialog();

            // Update the text
            date = dateForm.Value.Date;
            textBoxDate.Text = dateForm.Value.ToString("d MMM yyyy");
            textBoxDate.Refresh();
        }

        /// <summary>
        /// Click event handler for the user ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxUserName_Click_1(object sender, EventArgs e)
        {
            // Check for an administrator
            if (ISessions.Instance.CurrentUser.Type == UserType.Administrator)
            {
                // Initialise the keyboard form
                var keyboard = new Keyboard(textBoxUserName.Title, textBoxUserName.Text);

                // Reparent the controls
                keyboard.Reparent();

                // Show the form
                keyboard.ShowDialog();

                // Update the text
                textBoxUserName.Text = keyboard.Text;
                textBoxUserName.Refresh();
            }
        }

        /// <summary>
        /// Click event handler for the specimen ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSpecimenId_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBoxSpecimenId.Title, textBoxSpecimenId.Text);

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxSpecimenId.Text = keyboard.Text;
            textBoxSpecimenId.Refresh();
        }

        /// <summary>
        /// Click event handler for the search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            // Initialise the parameters
            var parameters = new Dictionary<string, object>();

            if (date.HasValue)
            {
                parameters.Add("Date", date.Value);
            }

            if (string.IsNullOrEmpty(textBoxUserName.Text) == false)
            {
                parameters.Add("UserName", textBoxUserName.Text);
            }

            if (string.IsNullOrEmpty(textBoxSpecimenId.Text) == false)
            {
                parameters.Add("SampleId", textBoxSpecimenId.Text);
            }

            if (string.IsNullOrEmpty(comboBox.Text) == false)
            {
                parameters.Add("Assay", comboBox.Text);
            }

            // Issue the save command with a password change
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next) { Parameters = parameters });
        }

        /// <summary>
        /// Click event handler for the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_Click(object sender, EventArgs e)
        {
            // Toggle the list box
            listBox.Visible = listBox.Visible ? false : true;
        }

        /// <summary>
        /// Click event handler for the list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_Click(object sender, EventArgs e)
        {
            comboBox.Text = listBox.Text;
            comboBox.Refresh();

            listBox.Visible = false;
        }
    }
}
