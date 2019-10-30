/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using IO.Model.Serializable;

namespace IO.View.Controls
{
    /// <summary>
    /// Test viewer control
    /// </summary>
    public partial class TestViewer : UserControl
    {
        /// <summary>
        /// The test
        /// </summary>
        private ITest test = null;

        /// <summary>
        /// Test accessor
        /// </summary>
        public ITest Test
        {
            get
            {
                return test;
            }
            set
            {
                // Check for a new assay
                if (test != value)
                {
                    // Set the new value
                    test = value;
                }

                // Update the controls
                UpdateControls();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update the controls to reflect the current script
        /// </summary>
        private void UpdateControls()
        {
            // Check for an assay
            if (test == null)
            {
                Enabled = false;
                textBoxPercentComplete.Clear();
                textBoxSampleId.Clear();
                checkBoxQcTest.Checked = false;
                textBoxStartDateTime.Clear();
                textBoxEndDateTime.Clear();
                textBoxAssay.Clear();
                textBoxAssayVersion.Clear();
                textBoxCartridgeData.Clear();
                checkBoxCalibrationOutsideTolerance.Checked = false;
                textBoxOutcome.Clear();
                textBoxPositive.Clear();
                textBoxNegative.Clear();
                textBoxInvalid.Clear();
                textBoxAnalysisType.Clear();
                textBoxLog.Clear();
                graphWidget.Start = 0.0;
                graphWidget.Increment = 1.0;
                graphWidget.SetData(0, null);
                graphWidget.SetData(1, null);
                graphWidget.SetData(2, null);
                graphWidget.SetData(3, null);
                graphWidget.Refresh();
                graphWidgetPcr.Start = 0.0;
                graphWidgetPcr.Increment = 1.0;
                graphWidgetPcr.SetData(0, null);
                graphWidgetPcr.SetData(1, null);
                graphWidgetPcr.SetData(2, null);
                graphWidgetPcr.SetData(3, null);
                graphWidgetPcr.Refresh();
            }
            else
            {
                Enabled = true;
                textBoxPercentComplete.Text = test.PercentComplete.ToString();
                textBoxSampleId.Text = test.Result.SampleId.ToString();
                checkBoxQcTest.Checked = test.Result.QcTest;
                textBoxStartDateTime.Text = test.Result.StartDateTime.ToLocalTime().ToString();
                textBoxEndDateTime.Text = (test.Result.EndDateTime == DateTime.MinValue) ? null : 
                    test.Result.EndDateTime.ToLocalTime().ToString();
                textBoxAssay.Text = test.Result.AssayName;
                textBoxAssayVersion.Text = test.Result.AssayVersion.ToString();
                textBoxCartridgeData.Text = test.Result.CartridgeData;
                checkBoxCalibrationOutsideTolerance.Checked = test.Result.CalibrationOutsideTolerance;
                textBoxOutcome.Text = test.Result.Outcome.ToString();
                textBoxPositive.Text = test.Result.PositivePeaks;
                textBoxNegative.Text = test.Result.NegativePeaks;
                textBoxInvalid.Text = test.Result.InvalidPeaks;
                textBoxAnalysisType.Text = test.Data.AnalysisType;
                textBoxLog.Lines = test.Data.Log.Split('\n');
                graphWidget.Start = test.Data.StartPotential;
                graphWidget.Increment = test.Data.IncrementalPotential;
                graphWidget.SetData(0, test.Data.CellValues[0]);
                graphWidget.SetData(1, test.Data.CellValues[1]);
                graphWidget.SetData(2, test.Data.CellValues[2]);
                graphWidget.SetData(3, test.Data.CellValues[3]);
                graphWidget.Refresh();
                graphWidgetPcr.Start = 0;
                graphWidgetPcr.Increment = test.Data.FastReportingPeriod / 1000.0;
                graphWidgetPcr.SetData(0, (test.Data.PcrValues == null) ? null : 
                    test.Data.PcrValues.ToArray());
                graphWidgetPcr.Refresh();
            }
        }

        /// <summary>
        /// Click event handler for the copy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            // Initialise the starting voltage, data length and string builder
            double voltage = test.Data.StartPotential;
            int length = test.Data.CellValues.Select(x => x.Length).Min();
            var stringBuilder = new StringBuilder();

            // Loop through the data compiling a table suitable for pasting into MS Excel
            for (int index = 0; index < length; ++index)
            {
                stringBuilder.Append(voltage.ToString("F1"));
                stringBuilder.Append("\t");
                stringBuilder.Append(test.Data.CellValues[0][index].ToString("F1"));
                stringBuilder.Append("\t");
                stringBuilder.Append(test.Data.CellValues[1][index].ToString("F1"));
                stringBuilder.Append("\t");
                stringBuilder.Append(test.Data.CellValues[2][index].ToString("F1"));
                stringBuilder.Append("\t");
                stringBuilder.Append(test.Data.CellValues[3][index].ToString("F1"));
                stringBuilder.Append("\r\n");

                voltage += test.Data.IncrementalPotential;
            }

            // Put the data on the clipboard
            Clipboard.SetText(stringBuilder.ToString());
        }

        /// <summary>
        /// Click event handler for the PCR copy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPcrCopy_Click(object sender, EventArgs e)
        {
            // Initialise the time, increment and string builder
            double time = 0;
            double increment = test.Data.FastReportingPeriod / 1000.0;
            var stringBuilder = new StringBuilder();

            // Loop through the data compiling a table suitable for pasting into MS Excel
            foreach (var temperature in test.Data.PcrValues)
            {
                stringBuilder.Append(time.ToString("F1"));
                stringBuilder.Append("\t");
                stringBuilder.Append(temperature.ToString("F1"));
                stringBuilder.Append("\r\n");

                time += increment;
            }

            // Put the data on the clipboard
            Clipboard.SetText(stringBuilder.ToString());
        }
    }
}
