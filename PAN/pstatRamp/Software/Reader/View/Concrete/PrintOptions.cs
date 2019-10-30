/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using IO.Model.Serializable;
using IO.FileSystem;

namespace IO.View.Concrete
{
    /// <summary>
    /// Add new assay type form
    /// </summary>
    public partial class PrintOptions : Form
    {
        /// <summary>
        /// The test to be printed
        /// </summary>
        public ITest Test { get; set; } 

        /// <summary>
        /// Default constructor
        /// </summary>
        public PrintOptions()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.PrintOptions;
        }

        /// <summary>
        /// Click event handler for the Print Hard Copy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrintHardCopy_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("PrintHardCopy", new Dictionary<string,object>() { { "Test", Test } });
        }

        /// <summary>
        /// Click event handler for the Print To File button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPrintToFile_Click(object sender, EventArgs e)
        {
            // Get the first removable drive
            var fileSystem = ILocalFileSystem.Instance.GetRemovableDrives().FirstOrDefault();
            Form dialog = new ContinueBack(titleBar.Text, Properties.Resources.ExportOverwrite);

            // Reparent the controls
            dialog.Reparent();

            // Check for a valid drive
            if (fileSystem == null)
            {
                // Create the dialog
                dialog = new Message(Properties.Resources.PrintToFile, Properties.Resources.NoRemovableStorage);

                // Reparent the controls
                dialog.Reparent();

                // Prompt the user
                dialog.ShowDialog();
            }
            else if ((fileSystem.ReadTextFile("Results") == null) ||
                (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Yes))
            {
                // Write the results
                fileSystem.WriteTextFile("Results", string.Format(Properties.Resources.ExportHeader,
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    IConfiguration.Instance.InstrumentName) +
                    IResults.Instance.ResultText(Test));

                // Create the dialog
                dialog = new Message(Properties.Resources.PrintToFile,
                    string.Format(Properties.Resources.ExportSuccess, "RESULTS"));

                // Reparent the controls
                dialog.Reparent();

                // Prompt the user
                dialog.ShowDialog();
            }
        }
    }
}
