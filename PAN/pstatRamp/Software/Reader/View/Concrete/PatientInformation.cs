/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Patient information form
    /// </summary>
    public partial class PatientInformation : Form
    {
        /// <summary>
        /// Array of displayed or recorded fields
        /// </summary>
        private IField[] fieldsArray = null;

        /// <summary>
        /// Dictionary of field values
        /// </summary>
        protected Dictionary<string, object> fieldValues = new Dictionary<string, object>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public PatientInformation()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.EnterPatientDetails;

            // Update the values
            UpdateValues();
        }

        /// <summary>
        /// Update the list of values
        /// </summary>
        protected void UpdateValues()
        {
            // Initialise the array of displayed or recorded fields
            fieldsArray = (IConfiguration.Instance == null) ? new IField[0] : 
                IConfiguration.Instance.Fields.Where(
                x => x.Policy != Model.Serializable.FieldPolicy.Ignore).ToArray();

            // Get the field count
            var fieldCount = fieldsArray.Length;

            // Initialise the array of text boxes
            var textBoxArray = new TextBox[] { textBox1, textBox2, textBox3, textBox4 };

            // Set the scroll bar maximum
            scrollBar.Maximum = Math.Max(0, fieldCount - textBoxArray.Length);
            scrollBar.Position = Math.Min(scrollBar.Position, scrollBar.Maximum);

            // Initialise a variable to take the field values
            object value;

            // Loop through the visible text boxes
            for (int textBox = 0, field = scrollBar.Position; textBox < textBoxArray.Length; ++textBox, ++field)
            {
                // Check for an associated field
                if (field < fieldCount)
                {
                    // Make the text box visible and set the title
                    textBoxArray[textBox].Visible = true;
                    textBoxArray[textBox].Title = Auxilliary.DisplayNameForField(
                        fieldsArray[field].Name);

                    // Try to get the value from the field values dictionary
                    if (fieldValues.TryGetValue(fieldsArray[field].Name, out value))
                    {
                        // Check for a text field and set the value
                        if (fieldsArray[field].FieldType == FieldType.Text)
                        {
                            textBoxArray[textBox].Text = (string)value;
                        }
                        // Otherwise it is a date
                        else
                        {
                            textBoxArray[textBox].Text = ((DateTime)value).ToLocalTime().ToString("d MMM yyyy");
                        }
                    }
                    else
                    {
                        // There is no value so set it to empty
                        textBoxArray[textBox].Text = string.Empty;
                    }

                    // Refresh the text box
                    textBoxArray[textBox].Refresh();
                }
                else
                {
                    // Hide the unused text box
                    textBoxArray[textBox].Visible = false;
                }
            }
        }

        /// <summary>
        /// Click event handler for the scroll bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollBar_Click(object sender, EventArgs e)
        {
            // Update the values based on the new scroll position
            UpdateValues();
        }

        /// <summary>
        /// Click event handler for a text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Click(object sender, EventArgs e)
        {
            // Get the text box and associated field
            var textBox = (TextBox)sender;
            var field = fieldsArray[int.Parse((string)textBox.Tag) + scrollBar.Position];

            // Check for a text field
            if (field.FieldType == FieldType.Text)
            {
                // Initialise the keyboard form
                var keyboard = new Keyboard(textBox.Title, textBox.Text);

                // Reparent the controls
                keyboard.Reparent();

                // Show the form
                keyboard.ShowDialog();

                // Update the text
                textBox.Text = keyboard.Text.Trim();
                textBox.Refresh();

                // Check for an empty string
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    // Remove the value from the list
                    fieldValues.Remove(field.Name);
                }
                else
                {
                    // Set the value in the field values
                    fieldValues[field.Name] = textBox.Text;
                }
            }
            // Otherwise it is a date
            else
            {
                // Try to get the value from the field values dictionary
                object value;

                if (fieldValues.TryGetValue(field.Name, out value) == false)
                {
                    value = DateTime.UtcNow;
                }

                // Initialise the date form
                var date = new Date(((DateTime)value).Date);

                // Reparent the controls
                date.Reparent();

                // Show the form
                date.ShowDialog();

                // Update the text
                textBox.Text = date.Value.Date.ToString("d MMM yyyy");
                textBox.Refresh();

                // Set the value in the field values
                fieldValues[field.Name] = date.Value.Date;
            }
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Check that all fields have values
            foreach (var field in fieldsArray)
            {
                // Try to get the value and, if is is a string, check for empty
                object value;

                if ((fieldValues.TryGetValue(field.Name, out value) == false) ||
                    ((value is string) && (((string)value) == string.Empty)))
                {
                    // Create the dialog
                    var dialog = new Message(Properties.Resources.MissingInformation,
                        Properties.Resources.MissingInformationText);

                    // Reparent the controls
                    dialog.Reparent();

                    // Show the dialog
                    dialog.ShowDialog();

                    return;
                }
            }

            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
            {
                Parameters = fieldValues,
            });
        }

        /// <summary>
        /// Click event handler for the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Show the abandon test setup confirmation dialog and check the result
            if (ITests.Instance.CurrentTest.AbandonTestSetup())
            {
                // Issue the abort command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Abort));
            }
        }
    }
}
