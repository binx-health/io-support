/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View.Concrete
{
    /// <summary>
    /// Field policy form
    /// </summary>
    public partial class FieldPolicy : Form
    {
        /// <summary>
        /// Field policy value
        /// </summary>
        public IO.Model.Serializable.FieldPolicy Value { get; private set; }

        /// <summary>
        /// Default constrictor
        /// </summary>
        public FieldPolicy(string name, IO.Model.Serializable.FieldPolicy value)
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = name;

            // Set the value
            Value = value;

            // Initialise the form
            checkBoxDisplay.Checked = Value == IO.Model.Serializable.FieldPolicy.Display;
            checkBoxDisplay.Refresh();
            checkBoxRecord.Checked = Value == IO.Model.Serializable.FieldPolicy.Record;
            checkBoxRecord.Refresh();
            checkBoxIgnore.Checked = Value == IO.Model.Serializable.FieldPolicy.Ignore;
            checkBoxIgnore.Refresh();
        }
        
        /// <summary>
        /// Click event handler for the ok button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (checkBoxDisplay.Checked)
            {
                Value = IO.Model.Serializable.FieldPolicy.Display;
            }
            else if (checkBoxRecord.Checked)
            {
                Value = IO.Model.Serializable.FieldPolicy.Record;
            }
            else
            {
                Value = IO.Model.Serializable.FieldPolicy.Ignore;
            }

            // Close the form
            Close();
        }

        /// <summary>
        /// Click event handler for the check boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Click(object sender, EventArgs e)
        {
            // Update the check boxes based on the clicked item
            checkBoxDisplay.Checked = (sender == checkBoxDisplay);
            checkBoxDisplay.Refresh();
            checkBoxRecord.Checked = (sender == checkBoxRecord);
            checkBoxRecord.Refresh();
            checkBoxIgnore.Checked = (sender == checkBoxIgnore);
            checkBoxIgnore.Refresh();
        }
    }
}
