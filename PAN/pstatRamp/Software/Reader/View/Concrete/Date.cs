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
    public partial class Date : Form
    {
        /// <summary>
        /// The base year for the form based on 150 years ago
        /// </summary>
        private static int baseYear = DateTime.Now.Year - 150;

        /// <summary>
        /// Array of localised short month names
        /// </summary>
        private string[] monthNames = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AbbreviatedMonthNames.
            Where(x => string.IsNullOrEmpty(x) == false).ToArray();

        /// <summary>
        /// The date and time value
        /// </summary>
        public DateTime Value { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="value">The date and time value</param>
        public Date(DateTime value)
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.SetDate;

            // Initialise the value
            Value = value;

            // Set the initial options for the spinners
            spinnerYear.Values = Auxilliary.IntegerSequence(baseYear, baseYear + 150, 1, "D4");
            spinnerMonth.Values = monthNames;
            spinnerDay.Values = Auxilliary.IntegerSequence(1, DateTime.DaysInMonth(value.Year, value.Month), 1, "D2");

            // Set the initial values for the spinners
            spinnerYear.Text = value.Year.ToString("D4");
            spinnerMonth.Text = monthNames[value.Month - 1];
            spinnerDay.Text = value.Day.ToString("D2");
        }

        /// <summary>
        /// Click event handler for the spinners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spinner_Click(object sender, EventArgs e)
        {
            // Check for the month or year spinner
            if ((sender == spinnerMonth) || (sender == spinnerYear))
            {
                // Set the days to the new value
                spinnerDay.Values = Auxilliary.IntegerSequence(1, DateTime.DaysInMonth(Value.Year, 
                    spinnerMonth.Index + 1), 1, "D2");

                // Check for an invalid day value
                if (string.IsNullOrEmpty(spinnerDay.Text))
                {
                    // It can only be invalid high so set it to the highest possible value
                    spinnerDay.Text = spinnerDay.Values.Last();
                }
            }

            Value = new DateTime(spinnerYear.Index + baseYear, spinnerMonth.Index + 1,
                spinnerDay.Index + 1);
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Close the form
            Close();
        }
    }
}
