/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// Sample ID form
    /// </summary>
    public partial class SampleId : Form
    {
        /// <summary>
        /// String of valid text characters for text entry
        /// </summary>
        private static readonly string VALID_TEXT_CHARACTERS =
            "0123456789:<>?abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Default constructor
        /// </summary>
        public SampleId()
        {
            InitializeComponent();
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
                lock (ITests.Instance.CurrentTest)
                {
                    // Check that we are the locking user for the test
                    if (ITests.Instance.CurrentTest.LockingUserId != ISessions.Instance.CurrentUser.ID)
                    {
                        // If not then bail
                        IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
                    }

                    // Initialise the inherited controls
                    titleBar.Text = ITests.Instance.CurrentTest.Result.QcTest ?
                        Properties.Resources.EnterQcSampleId : Properties.Resources.EnterSampleId;
                }
            }
        }

        /// <summary>
        /// Click event handler for the next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            // Get the sample ID
            var sampleId = textBox.Text.Trim();

            // Check for a valid sample ID
            if (string.IsNullOrEmpty(sampleId))
            {
                // Create the dialog
                Form dialog = new YesNo(Properties.Resources.MissingInformation, 
                    Properties.Resources.AutoSampleId);

                // Reparent the controls
                dialog.Reparent();

                // Check for auto sample ID
                if (IConfiguration.Instance.AutoSampleId &&
                    (dialog.ShowDialog() == DialogResult.Yes))
                {
                    // Generate a sample ID based on the elapsed seconds since 1st Jan 2013
                    sampleId = Math.Floor((DateTime.UtcNow - 
                        new DateTime(2013, 1, 1)).TotalSeconds).ToString();

                    // Issue the next command
                    IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                    {
                        Parameters = new Dictionary<string, object>() 
                        { 
                            { "SampleId", sampleId }
                        },
                    });
                }
                else
                {
                    // Create the dialog
                    dialog = new Message(Properties.Resources.MissingInformation,
                        Properties.Resources.MissingInformationText);

                    // Reparent the controls
                    dialog.Reparent();

                    // Show the dialog
                    dialog.ShowDialog();
                }
            }
            // Check for a unique sample ID
            else if (IResults.Instance[sampleId] != null)
            {
                // Create the dialog
                Form dialog = new YesNo(Properties.Resources.MissingInformation, 
                    Properties.Resources.UniqueSmpleId);

                // Reparent the controls
                dialog.Reparent();

                // Check for auto sample ID
                if (dialog.ShowDialog() == DialogResult.Yes)
                {
                    // Generate a unique sample ID
                    int index = 0;
                    string uniqueSampleId;

                    do
                    {
                        uniqueSampleId = sampleId + ":" + (++index).ToString();
                    }
                    while (IResults.Instance[uniqueSampleId] != null);

                    // Issue the next command
                    IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                    {
                        Parameters = new Dictionary<string, object>() 
                        { 
                            { "SampleId", uniqueSampleId }
                        },
                    });
                }
                else
                {
                    // Create the dialog
                    dialog = new Message(titleBar.Text, Properties.Resources.EnterUniqueSampleId);

                    // Reparent the controls
                    dialog.Reparent();

                    // Show the dialog
                    dialog.ShowDialog();
                }
            }
            else
            {
                // Issue the next command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                {
                    Parameters = new Dictionary<string, object>() 
                    { 
                        { "SampleId", sampleId }
                    },
                });
            }
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

        /// <summary>
        /// Click event handler for the sample ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSampleId_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBox.Title, textBox.Text);

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBox.Text = keyboard.Text;
            textBox.Refresh();
        }

        /// <summary>
        /// Key press event handler for the form so that a physical keyboard can be used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SampleId_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the backspace key
            if (e.KeyChar == '\b')
            {
                // Check for characters to delete
                if (textBox.Text.Length > 0)
                {
                    // Delete the last character and refresh the text box
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.Refresh();
                }
            }
            // Check for the enter key
            else if (e.KeyChar == '\r')
            {
            }
            // Check for a valid character
            else if (VALID_TEXT_CHARACTERS.Contains(e.KeyChar))
            {
                // Append the character to the text box and refresh it
                textBox.Text += e.KeyChar;
                textBox.Refresh();
            }
        }
    }
}
