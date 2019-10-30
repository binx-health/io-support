/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Login form
    /// </summary>
    public partial class QcTestSetup : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public QcTestSetup()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.QcTestSetup;

            // Initialise the text boxes
            comboBoxQuarantineState.Text = Auxilliary.DisplayTextForQuarantineState(
                IConfiguration.Instance.QuarantineState);
            comboBoxQuarantineState.Refresh();

            listBoxQuarantineState.Values = new string[] {
                Properties.Resources.Locked,
                Properties.Resources.Unlocked,
                Properties.Resources.DoNotQuarantine
            };
            listBoxQuarantineState.Text = comboBoxQuarantineState.Text;

            // Update the frequency selection
            UpdateFequency();
        }

        /// <summary>
        /// Update the frequencey depending on the state
        /// </summary>
        private void UpdateFequency()
        {
            // Check for unlocked state
            if (comboBoxQuarantineState.Text == Properties.Resources.Unlocked)
            {
                // Set the frequency
                comboBoxFrequency.Text = Auxilliary.DisplayTextForQcTestFrequency(
                    IConfiguration.Instance.QcTestFrequency);

                // Update the list box
                listBoxFrequency.Values = new string[] {
                    Properties.Resources.Daily,
                    Properties.Resources.Weekly,
                    Properties.Resources.Monthly,
                };
            }
            else
            {
                // Clear the frequency
                comboBoxFrequency.Text = Properties.Resources.Never;

                // Update the list box
                listBoxFrequency.Values = new string[] {
                    Properties.Resources.Never,
                };
            }

            // Refresh the display
            comboBoxFrequency.Refresh();
            listBoxFrequency.Text = comboBoxFrequency.Text;
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Get the QC test frequency
            var testFrequency = IConfiguration.Instance.QcTestFrequency;

            if (comboBoxFrequency.Text == Properties.Resources.Daily)
            {
                testFrequency = QcTestFrequency.Daily;
            }
            else if (comboBoxFrequency.Text == Properties.Resources.Weekly)
            {
                testFrequency = QcTestFrequency.Weekly;
            }
            else if (comboBoxFrequency.Text == Properties.Resources.Monthly)
            {
                testFrequency = QcTestFrequency.Monthly;
            }

            // Get the quarantine state
            var quarantineState = QuarantineState.DoNotQuarantine;

            if (comboBoxQuarantineState.Text == Properties.Resources.Locked)
            {
                quarantineState = QuarantineState.Locked;
            }
            else if (comboBoxQuarantineState.Text == Properties.Resources.Unlocked)
            {
                quarantineState = QuarantineState.Unlocked;
            }

            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next) 
            {
                Parameters = new Dictionary<string, object>()
                { 
                    { "QcTestFrequency", testFrequency },
                    { "QuarantineState", quarantineState },
                },
            });
        }

        /// <summary>
        /// Click event handler for the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFrequency_Click(object sender, EventArgs e)
        {
            // Toggle the list box
            listBoxFrequency.Visible = listBoxFrequency.Visible ? false : true;
            listBoxQuarantineState.Visible = false;
        }

        /// <summary>
        /// Click event handler for the list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxFrequency_Click(object sender, EventArgs e)
        {
            // Set the new value in the combo-box
            comboBoxFrequency.Text = listBoxFrequency.Text;
            comboBoxFrequency.Refresh();

            // Hide the list
            listBoxFrequency.Visible = false;
        }

        /// <summary>
        /// Click event handler for the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxQuarantineState_Click(object sender, EventArgs e)
        {
            // Toggle the list box
            listBoxQuarantineState.Visible = listBoxQuarantineState.Visible ? false : true;
            listBoxFrequency.Visible = false;
        }

        /// <summary>
        /// Click event handler for the list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxQuarantineState_Click(object sender, EventArgs e)
        {
            // Set the new value in the combo-box
            comboBoxQuarantineState.Text = listBoxQuarantineState.Text;
            comboBoxQuarantineState.Refresh();

            // Hide the list
            listBoxQuarantineState.Visible = false;

            // Update the frequency selection
            UpdateFequency();
        }
    }
}
