/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using IO.Model.Serializable;

namespace IO.View
{
    /// <summary>
    /// Auxilliary extension methods
    /// </summary>
    internal static class Auxilliary
    {
        /// <summary>
        /// Get the display name for a field
        /// </summary>
        /// <param name="name">The field name</param>
        /// <returns>The display name</returns>
        public static string DisplayNameForField(string name)
        {
            if (name == "Name")
            {
                return Properties.Resources.Name;
            }
            else if (name == "Surname")
            {
                return Properties.Resources.Surname;
            }
            else if (name == "ContactNo")
            {
                return Properties.Resources.ContactNo;
            }
            else if (name == "Address")
            {
                return Properties.Resources.Address;
            }
            else if (name == "PatientId")
            {
                return Properties.Resources.PatientId;
            }
            else if (name == "DateOfBirth")
            {
                return Properties.Resources.DateOfBirth;
            }

            return name;
        }

        /// <summary>
        /// Get the display text for an outcome
        /// </summary>
        /// <param name="result">The result</param>
        /// <returns>The display text</returns>
        public static string DisplayTextForOutcome(ITestResult result)
        {
            if (result.Outcome == TestOutcome.UserAborted)
            {
                return Properties.Resources.Aborted;
            }
            else if (result.Outcome == TestOutcome.Error)
            {
                return Properties.Resources.Error;
            }

            // Create a new string builder
            var stringBuilder = new StringBuilder();

            // Check for and append detected peaks
            if (string.IsNullOrEmpty(result.PositivePeaks) == false)
            {
                stringBuilder.Append(string.Format(Properties.Resources.Detected, result.PositivePeaks));
            }

            // Check for and append undetected peaks
            if (string.IsNullOrEmpty(result.NegativePeaks) == false)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(" ");
                }

                stringBuilder.Append(string.Format(Properties.Resources.NotDetected, result.NegativePeaks));
            }

            // Check for and append invalid peaks
            if (string.IsNullOrEmpty(result.InvalidPeaks) == false)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(" ");
                }

                stringBuilder.Append(string.Format(Properties.Resources.TestInvalid, result.InvalidPeaks));
            }

            // Check for investigation use only assays
            if ((result.Assay != null) && (result.Assay.InvestigationUseOnly))
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(" ");
                }

                stringBuilder.Append(Properties.Resources.ForInvestigationUseOnly);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get the display text for a field policy
        /// </summary>
        /// <param name="policy">The field policy</param>
        /// <returns>The display text</returns>
        public static string DisplayTextForFieldPolicy(FieldPolicy policy)
        {
            if (policy == FieldPolicy.Display)
            {
                return Properties.Resources.Display;
            }
            else if (policy == FieldPolicy.Record)
            {
                return Properties.Resources.Record;
            }

            return Properties.Resources.Ignore;
        }

        /// <summary>
        /// Get the display text for a QC test frequency
        /// </summary>
        /// <param name="policy">The QC test frequency</param>
        /// <returns>The display text</returns>
        public static string DisplayTextForQcTestFrequency(QcTestFrequency value)
        {
            if (value == QcTestFrequency.Daily)
            {
                return Properties.Resources.Daily;
            }
            else if (value == QcTestFrequency.Weekly)
            {
                return Properties.Resources.Weekly;
            }

            return Properties.Resources.Monthly;
        }

        /// <summary>
        /// Get the display text for a quarantine state
        /// </summary>
        /// <param name="policy">The quarantine state</param>
        /// <returns>The display text</returns>
        public static string DisplayTextForQuarantineState(QuarantineState value)
        {
            if (value == QuarantineState.Locked)
            {
                return Properties.Resources.Locked;
            }
            else if (value == QuarantineState.Unlocked)
            {
                return Properties.Resources.Unlocked;
            }

            return Properties.Resources.DoNotQuarantine;
        }

        /// <summary>
        /// Fill a rounded rectangle
        /// </summary>
        /// <param name="graphics">The graphics object</param>
        /// <param name="brush">The brush</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="radius">The corner radius</param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle, 
            int radius)
        {
            radius = Math.Min(rectangle.Height / 2, Math.Min(rectangle.Width / 2, radius));

            if (radius < 2)
            {
                graphics.FillRectangle(brush, rectangle);
            }
            else
            {
                var diameter = radius + radius;
                var originalSmoothingMode = graphics.SmoothingMode;

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.FillEllipse(brush, rectangle.X, rectangle.Y, diameter, diameter);
                graphics.FillEllipse(brush, rectangle.X + (rectangle.Width - diameter) - 1, rectangle.Y,
                    diameter, diameter);
                graphics.FillEllipse(brush, rectangle.X, rectangle.Y + (rectangle.Height - diameter) - 1,
                    diameter, diameter);
                graphics.FillEllipse(brush, rectangle.X + (rectangle.Width - diameter) - 1,
                    rectangle.Y + (rectangle.Height - diameter) - 1, diameter, diameter);

                var top = new Rectangle(rectangle.X + radius, rectangle.Y, rectangle.Width - diameter, 
                    radius);
                var middle = new Rectangle(rectangle.X, rectangle.Y + radius, rectangle.Width, 
                    rectangle.Height - diameter);
                var bottom = new Rectangle(rectangle.X + radius, rectangle.Y + (rectangle.Height - radius), 
                    rectangle.Width - diameter, radius);

                graphics.SmoothingMode = originalSmoothingMode;
                graphics.FillRectangle(brush, top);
                graphics.FillRectangle(brush, middle);
                graphics.FillRectangle(brush, bottom);
            }
        }

        /// <summary>
        /// Fill a rectangle with righ hand rounded corners
        /// </summary>
        /// <param name="graphics">The graphics object</param>
        /// <param name="brush">The brush</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="radius">The corner radius</param>
        public static void FillRightRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle,
            int radius)
        {
            radius = Math.Min(rectangle.Height / 2, Math.Min(rectangle.Width / 2, radius));

            if (radius < 2)
            {
                graphics.FillRectangle(brush, rectangle);
            }
            else
            {
                var diameter = radius + radius;
                var originalSmoothingMode = graphics.SmoothingMode;

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.FillEllipse(brush, rectangle.X + (rectangle.Width - diameter) - 1, rectangle.Y,
                    diameter, diameter);
                graphics.FillEllipse(brush, rectangle.X + (rectangle.Width - diameter) - 1,
                    rectangle.Y + (rectangle.Height - diameter) - 1, diameter, diameter);

                var left = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - radius,
                    rectangle.Height);
                var right = new Rectangle(rectangle.X + (rectangle.Width - radius), 
                    rectangle.Y + radius, radius, rectangle.Height - diameter);

                graphics.SmoothingMode = originalSmoothingMode;
                graphics.FillRectangle(brush, left);
                graphics.FillRectangle(brush, right);
            }
        }

        /// <summary>
        /// Method to create a formatted integer sequence as an array of strings
        /// </summary>
        /// <param name="start">The start value</param>
        /// <param name="end">The end value</param>
        /// <param name="step">The step value</param>
        /// <param name="format">The format for the string</param>
        /// <returns>The array of formatted values</returns>
        public static string[] IntegerSequence(int start, int end, int step, string format)
        {
            var result = new string[((end - start) + 1) / step];

            for (int value = start, index = 0; value <= end; value += step, ++index)
            {
                result[index] = value.ToString(format);
            }

            return result;
        }

        /// <summary>
        /// Show the abandon test setup confirmation dialog and return the result
        /// </summary>
        /// <param name="test">The test</param>
        /// <returns>True if confirmed</returns>
        public static bool AbandonTestSetup(this ITest test)
        {
            // Use the correct test for a QC test
            var dialog = test.Result.QcTest ? new Concrete.YesNo(Properties.Resources.AbandonQcTestSetup, 
                Properties.Resources.AbandonQcSetupConfirm) : new Concrete.YesNo(
                Properties.Resources.AbandonTestSetup, Properties.Resources.AbandonSetupConfirm);

            // Reparent controls
            dialog.Reparent();

            return dialog.ShowDialog() == DialogResult.Yes;
        }
    }
}
