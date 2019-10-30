/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IO.Model.Serializable;
using IO.Scripting;
using IO.FileSystem;

namespace IO.View.Concrete
{
    /// <summary>
    /// Login form
    /// </summary>
    public partial class SelectNewAssay : Form
    {
        /// <summary>
        /// List of assays
        /// </summary>
        private IEnumerable<IAssay> assays = null;

        /// <summary>
        /// Assays property
        /// </summary>
        public IEnumerable<IAssay> Assays
        {
            get
            {
                return assays;
            }
            set
            {
                assays = value;

                // Set the assays in the list box and refresh it
                listBox.Values = value.Select(x => x.Name);
                listBox.Text = listBox.Values.FirstOrDefault();
                listBox.Refresh();

                // Select the first assay
                comboBox.Text = listBox.Values.FirstOrDefault();
                comboBox.Refresh();
            }
        }

        /// <summary>
        /// The file system
        /// </summary>
        public ISimpleFileSystem FileSystem { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SelectNewAssay()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.SelectNewAssay;
        }

        /// <summary>
        /// Click event handler for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next) 
            {
                Parameters = new Dictionary<string,object>() 
                { 
                    { "Assay", assays.Where(x => x.Name == comboBox.Text).FirstOrDefault() },
                    { "FileSystem", FileSystem },
                },
            });
        }

        /// <summary>
        /// Click event handler for the combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_Click(object sender, EventArgs e)
        {
            // Toggle the list box
            listBox.Visible = listBox.Visible ? false : true;
        }

        /// <summary>
        /// Click event handler for the list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_Click(object sender, EventArgs e)
        {
            comboBox.Text = listBox.Text;
            comboBox.Refresh();

            listBox.Visible = false;
        }
    }
}
