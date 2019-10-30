/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// About IO form
    /// </summary>
    public partial class About : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="storage">Storage available in bytes</param>
        public About()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.AboutTitle;
        }

        /// <summary>
        /// Update the text on the form
        /// </summary>
        protected override void UpdateStatus()
        {
            // Call the base class
            base.UpdateStatus();

            // Format the storage text string
            long storage = IConfiguration.Instance.Storage;
            string storageText;

            if (storage < 1000)
            {
                storageText = storage + "bytes";
            }
            else if (storage < 1000000)
            {
                storageText = (int)(storage / 1000) + "KB";
            }
            else if (storage < 1000000000)
            {
                storageText = (int)(storage / 1000000) + "MB";
            }
            else
            {
                storageText = (int)(storage / 1000000000) + "GB";
            }

            // Set the text in the panel
            panelAbout.Text = string.Format(Properties.Resources.AboutText,
                Application.ProductVersion,
                Comms.Firmware.IManager.Instance.FirmwareVersion,
                Controller.IController.Instance.TotalResults.ToString(),
                Controller.IController.Instance.QueuedResults.ToString(),
                storageText,
                IConfiguration.Instance.SerialNumber);
        }
    }
}
