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

namespace IO.View.Concrete
{
    /// <summary>
    /// Login form
    /// </summary>
    public partial class Language : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Language()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.Language;

            // Initialise the text box
            comboBox.Text = IConfiguration.Instance.Locale;
            comboBox.Refresh();

            // Update the list box
            listBox.Values = ViewManager.Instance.Locales.Keys;
            listBox.Text = IConfiguration.Instance.Locale;
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
                Parameters = new Dictionary<string,object>() { { "Language", comboBox.Text } },
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
