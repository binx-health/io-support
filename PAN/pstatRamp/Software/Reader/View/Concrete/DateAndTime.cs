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
    public partial class DateAndTime : Form
    {
        /// <summary>
        /// The base year for the form based on the assembly creation date
        /// </summary>
        private static int baseYear = File.GetCreationTime(Assembly.GetExecutingAssembly().Location).Year;

        /// <summary>
        /// Array of localised short month names
        /// </summary>
        private string[] monthNames = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AbbreviatedMonthNames.
            Where(x => string.IsNullOrEmpty(x) == false).ToArray();

        /// <summary>
        /// The date and time value in local time
        /// </summary>
        private DateTime value;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="value">The date and time value</param>
        public DateAndTime()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.SetDateTime;

            // Initialise the value in local time
            value = DateTime.Now;

            // Set the initial options for the spinners
            spinnerMinute.Values = Auxilliary.IntegerSequence(0, 59, 1, "D2");
            spinnerHour.Values = Auxilliary.IntegerSequence(0, 23, 1, "D2");
            spinnerYear.Values = Auxilliary.IntegerSequence(baseYear, baseYear + 100, 1, "D4");
            spinnerMonth.Values = monthNames;
            spinnerDay.Values = Auxilliary.IntegerSequence(1, DateTime.DaysInMonth(value.Year, value.Month), 1, "D2");

            // Set the initial values for the spinners
            spinnerMinute.Text = value.Minute.ToString("D2");
            spinnerHour.Text = value.Hour.ToString("D2");
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
                spinnerDay.Values = Auxilliary.IntegerSequence(1, DateTime.DaysInMonth(value.Year, 
                    spinnerMonth.Index + 1), 1, "D2");

                // Check for an invalid day value
                if (string.IsNullOrEmpty(spinnerDay.Text))
                {
                    // It can only be invalid high so set it to the highest possible value
                    spinnerDay.Text = spinnerDay.Values.Last();
                }
            }

            value = new DateTime(spinnerYear.Index + baseYear, spinnerMonth.Index + 1,
                spinnerDay.Index + 1, spinnerHour.Index, spinnerMinute.Index, 0);
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Issue the next command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next) 
            {
                Parameters = new Dictionary<string,object>() { { "Value", value.ToUniversalTime() } },
            });
        }
    }
}
