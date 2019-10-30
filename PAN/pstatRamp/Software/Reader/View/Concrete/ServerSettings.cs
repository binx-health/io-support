/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Password rules form
    /// </summary>
    public partial class ServerSettings : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ServerSettings()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.ServerSettings;

            // Initialise the values on the form
            textBoxServerAddress.Text = IConfiguration.Instance.Poct1ServerUri;
            textBoxServerPort.Text = IConfiguration.Instance.Poct1ServerPort.ToString();
        }

        /// <summary>
        /// Click event handler for the server address text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxServerAddress_Click(object sender, EventArgs e)
        {
            // Get the text box
            var textBox = (TextBox)sender;

            // Initialise the keyboard form
            var keyboard = new Keyboard(textBox.Title, textBox.Text);

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBox.Text = keyboard.Text;
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the server port text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxServerPort_Click(object sender, EventArgs e)
        {
            // Get the text box
            var textBox = (TextBox)sender;

            // Initialise the keypad form
            var keypad = new Keypad(textBox.Title, textBox.Text);

            // Reparent the controls
            keypad.Reparent();

            // Show the form
            keypad.ShowDialog();

            // Update the text
            textBox.Text = keypad.Text;
            textBox.Refresh();
        }

        /// <summary>
        /// Click event handler for the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
            {
                Parameters = new Dictionary<string, object>() 
                {
                    { "Poct1ServerUri", textBoxServerAddress.Text },
                    { "Poct1ServerPort", int.Parse(textBoxServerPort.Text) },
                },
            });
        }
    }
}
