/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Windows.Forms;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// Test cancel form
    /// </summary>
    public partial class TestCancel : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public TestCancel()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.TestRunning;

            lock (ITests.Instance.CurrentTest)
            {
                // Get the locking user
                var lockingUser = IUsers.Instance.Where(x => x.ID ==
                    ITests.Instance.CurrentTest.LockingUserId).FirstOrDefault();

                panelMessage.Text = string.Format(Properties.Resources.TestInProgress,
                    (lockingUser != null) ? lockingUser.Name : string.Empty);
            }
        }

        /// <summary>
        /// Update the date and time on the form
        /// </summary>
        protected override void UpdateStatus()
        {
            // Call the base class
            base.UpdateStatus();

            lock (ITests.Instance.CurrentTest)
            {
                // Show the cancel button conditionally
                buttonCancel.Visible = (ISessions.Instance.CurrentUser.Type == UserType.Administrator) ||
                    (ITests.Instance.CurrentTest.LockingUserId == ISessions.Instance.CurrentUser.ID);
            }
        }

        /// <summary>
        /// Click event handler for the cancel test button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Create the dialog
            var dialog = new YesNo(Properties.Resources.CancelTest, Properties.Resources.CancelTestConfirm);

            // Reparent the controls
            dialog.Reparent();

            // Show the confirmation dialog and check the result
            if (dialog.ShowDialog() == DialogResult.Yes)
            {
                // Issue the next command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next));
            }
            else
            {
                // Issue the next command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Back));
            }
        }
    }
}
