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
    public partial class DataPolicy : Form
    {
        /// <summary>
        /// Dictionary of field policies
        /// </summary>
        private Dictionary<string, object> fieldPolicies = new Dictionary<string, object>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataPolicy()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.DataPolicy;

            // Update the values
            UpdateValues();
        }

        /// <summary>
        /// Update the list of values
        /// </summary>
        private void UpdateValues()
        {
            // Get the field count
            var fieldCount = IConfiguration.Instance.Fields.Count();
            var textBoxArray = new TextBox[] { textBox1, textBox2, textBox3, textBox4 };

            // Set the scroll bar maximum
            scrollBar.Maximum = Math.Max(0, fieldCount - textBoxArray.Length);
            scrollBar.Position = Math.Min(scrollBar.Position, scrollBar.Maximum);

            // Initialise arrays of fields and text boxes
            var fieldsArray = IConfiguration.Instance.Fields.ToArray();

            // Loop through the visible text boxes
            for (int textBox = 0, field = scrollBar.Position; textBox < textBoxArray.Length; ++textBox, ++field)
            {
                // Check for an associated field
                if (field < fieldCount)
                {
                    // Check for an existing value
                    object value;

                    if (fieldPolicies.TryGetValue(fieldsArray[field].Name, out value) == false)
                    {
                        // Set the field policy
                        value = fieldsArray[field].Policy;
                    }

                    // Make the text box visible and set the title
                    textBoxArray[textBox].Visible = true;
                    textBoxArray[textBox].Title = Auxilliary.DisplayNameForField(
                        fieldsArray[field].Name);
                    textBoxArray[textBox].Text = Auxilliary.DisplayTextForFieldPolicy(
                        (IO.Model.Serializable.FieldPolicy)value);
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
            var field = IConfiguration.Instance.Fields.ElementAt(int.Parse((string)textBox.Tag) + scrollBar.Position);

            // Check for an existing value
            object value;

            if (fieldPolicies.TryGetValue(field.Name, out value) == false)
            {
                // Set the field policy
                value = field.Policy;
            }

            // Initialise the field policy form
            var fieldPolicy = new FieldPolicy(textBox.Title, (IO.Model.Serializable.FieldPolicy)value);

            // Reparent the controls
            fieldPolicy.Reparent();

            // Show the form
            fieldPolicy.ShowDialog();

            // Update the text
            textBox.Text = Auxilliary.DisplayTextForFieldPolicy(fieldPolicy.Value);
            textBox.Refresh();

            // Set the value in the field values
            fieldPolicies[field.Name] = fieldPolicy.Value;
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
                Parameters = fieldPolicies,
            });
        }
    }
}
