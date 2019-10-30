/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;

namespace IO.View.Concrete
{
    /// <summary>
    /// Configuration form
    /// </summary>
    public partial class Configuration : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Configuration()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.InstrumentConfiguration;
        }

        /// <summary>
        /// Click event handler for the configuration button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonInstrumentName_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("InstrumentName");
        }

        /// <summary>
        /// Click event handler for the language button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLanguage_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("Language");
        }

        /// <summary>
        /// Click event handler for the set date/time button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDateTime_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("DateAndTime");
        }

        /// <summary>
        /// Click event handler for the about button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAbout_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("About");
        }

        /// <summary>
        /// Click event handler for the assay settings button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAssaySettings_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("AssaySettings");
        }

        /// <summary>
        /// Click event handler for the QC test setup button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonQcTestSetup_Click(object sender, EventArgs e)
        {
            // Issue the menu item
            IssueMenuItem("QcTestSetup");
        }
    }
}
