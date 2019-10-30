/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IO.View.Concrete
{
    /// <summary>
    /// Login form
    /// </summary>
    public partial class Login : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Login()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.Login;
            pictureBoxLogin.Image = Properties.Resources.Login_icon_selected;
        }

        /// <summary>
        /// Click event handler for the Used ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxUserName_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBoxUserName.Title, textBoxUserName.Text);

            // Reparent the controls
            keyboard.Reparent();

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxUserName.Text = keyboard.Text;
            textBoxUserName.Refresh();
        }

        /// <summary>
        /// Click event handler for the password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPassword_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBoxPassword.Title, textBoxPassword.Text) { Password = true };

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxPassword.Text = keyboard.Text;
            textBoxPassword.Refresh();
        }

        /// <summary>
        /// Click event handler for the login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Issue the login command
            IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
            {
                Parameters = new Dictionary<string,object>() 
                    { { "UserName" , textBoxUserName.Text }, { "Password", textBoxPassword.Text } },
            });
        }
    }
}
