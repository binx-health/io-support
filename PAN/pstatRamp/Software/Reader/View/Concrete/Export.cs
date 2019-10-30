/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.FileSystem;

namespace IO.View.Concrete
{
    /// <summary>
    /// Add new assay type form
    /// </summary>
    public partial class Export : Form
    {
        /// <summary>
        /// The file system to export to
        /// </summary>
        public ISimpleFileSystem FileSystem { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Export()
        {
            InitializeComponent();

            // Set the title text
            titleBar.Text = Properties.Resources.Export;
        }

        /// <summary>
        /// Click event handler for the cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Issue the home command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Menu));
        }

        /// <summary>
        /// Click event handler for the audit log button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAuditLog_Click(object sender, EventArgs e)
        {
            // Create the dialog
            var dialog = new ContinueBack(titleBar.Text, Properties.Resources.ExportOverwrite);

            // Reparent the controls
            dialog.Reparent();

            if ((FileSystem.ReadTextFile("AuditLog") == null) ||
                (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Yes))
            {
                IssueMenuItem("ExportAuditLog", 
                    new Dictionary<string,object>() { { "FileSystem", FileSystem } });
            }
        }

        /// <summary>
        /// Click event handler for the results button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResults_Click(object sender, EventArgs e)
        {
            // Create the dialog
            var dialog = new ContinueBack(titleBar.Text, Properties.Resources.ExportOverwrite);

            // Reparent the controls
            dialog.Reparent();

            if ((FileSystem.ReadTextFile("Results") == null) ||
                (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Yes))
            {
                IssueMenuItem("ExportResults",
                    new Dictionary<string, object>() { { "FileSystem", FileSystem } });
            }
        }
    }
}
