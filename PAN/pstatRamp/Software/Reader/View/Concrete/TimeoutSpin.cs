/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IO.View.Concrete
{
    /// <summary>
    /// Date and time form
    /// </summary>
    public partial class TimeoutSpin : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeoutSpin()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.Timeout;

            // Set the initial values for the spinner
            spinnerMinutes.Values = new string[] { "0", "1", "5", "10", "30", "60", "120" };
        }

        /// <summary>
        /// Load event for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeoutSpin_Load(object sender, EventArgs e)
        {
            spinnerMinutes.Text = Text;
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Update the value
            Text = spinnerMinutes.Text;

            // Close the form
            Close();
        }
    }
}
