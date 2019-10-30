/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Concrete
{
    /// <summary>
    /// View\Edit user form
    /// </summary>
    public partial class EditUser : Form
    {
        /// <summary>
        /// The original user ID before it was changed
        /// </summary>
        private string originalUserName = null;

        /// <summary>
        /// The original user type
        /// </summary>
        private UserType originalUserType;

        /// <summary>
        /// Flag to indicate that the password has changed
        /// </summary>
        private bool passwordChanged = false;

        /// <summary>
        /// The user ID
        /// </summary>
        public string UserName
        {
            get
            {
                return textBoxUserName.Text;
            }
            set
            {
                // Check for an original user
                if (originalUserName == null)
                {
                    // Set the original user name
                    originalUserName = value;

                    // Get the user object
                    var user = IUsers.Instance.Where(x => x.Name == originalUserName).First();

                    // Set the original user type and initialise the UI
                    originalUserType = UserType = user.Type;
                    PermanentSystem = IUsers.Instance[0].Name == value;
                    buttonDelete.Visible = (PermanentSystem == false) &&
                        (originalUserName != ISessions.Instance.CurrentUser.Name);
                    buttonDelete.Refresh();
                    titleBar.Text = Properties.Resources.EditUser;
                    titleBar.Refresh();
                    textBoxPassword.Text = "********";
                    textBoxPassword.Refresh();
                    textBoxVerify.Text = "********";
                    textBoxVerify.Refresh();
                }

                textBoxUserName.Text = value;
            }
        }

        /// <summary>
        /// The user type
        /// </summary>
        public UserType UserType
        {
            get
            {
                return radioAdministrator.Checked ? UserType.Administrator : UserType.User;
            }
            set
            {
                radioAdministrator.Checked = value == UserType.Administrator;
                radioAdministrator.Refresh();
                radioUser.Checked = value == UserType.User;
                radioUser.Refresh();
            }
        }

        /// <summary>
        /// Permanent system flag
        /// </summary>
        public bool PermanentSystem { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EditUser()
        {
            InitializeComponent();

            // Initialise the inherited controls
            titleBar.Text = Properties.Resources.EnterUser;
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
            var keyboard = new Keyboard(textBoxPassword.Title, string.Empty) { Password = true };

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxPassword.Text = keyboard.Text;
            textBoxPassword.Refresh();

            // Flag that the password has changed
            passwordChanged = true;
        }

        /// <summary>
        /// Click event handler for the password verifier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVerify_Click(object sender, EventArgs e)
        {
            // Initialise the keyboard form
            var keyboard = new Keyboard(textBoxVerify.Title, string.Empty) { Password = true };

            // Reparent the controls
            keyboard.Reparent();

            // Show the form
            keyboard.ShowDialog();

            // Update the text
            textBoxVerify.Text = keyboard.Text;
            textBoxVerify.Refresh();

            // Flag that the password has changed
            passwordChanged = true;
        }

        /// <summary>
        /// Click event handler for the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Count the number of administrators
            int administrators = IUsers.Instance.Count(x => x.Type == UserType.Administrator);

            // Check for a new user
            if (originalUserName == null)
            {
                if ((UserType == UserType.Administrator) &&
                    (administrators >= IUsers.MAXIMUM_ADMINISTRATORS))
                {
                    // Create the dialog
                    var dialog = new Message(Properties.Resources.EnterUser,
                        string.Format(Properties.Resources.MaximumAdministrators,
                        IUsers.MAXIMUM_ADMINISTRATORS));

                    // Reparent the controls
                    dialog.Reparent();

                    // Display the message for the required field
                    dialog.ShowDialog();
                }
                else if ((string.IsNullOrEmpty(UserName) == false) &&
                    (IUsers.Instance.Exists(x => x.Name == UserName) == false) &&
                    (textBoxPassword.Text == textBoxVerify.Text) &&
                    IConfiguration.Instance.PasswordRules.CheckPassword(textBoxPassword.Text))
                {
                    // Issue the save command without a password change
                    IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                    {
                        Parameters = new Dictionary<string, object>() 
                        { 
                            { "NewUserName", UserName },
                            { "NewUserType", UserType },
                            { "NewPassword", textBoxPassword.Text },
                        }
                    });
                }
                else
                {
                    // Show the invalid credentials dialog
                    var dialog = new Concrete.Message(titleBar.Text, Properties.Resources.InvalidCredentials);

                    // Reparent controls
                    dialog.Reparent();

                    // Show the dialog
                    dialog.ShowDialog();
                }
            }
            // Check for a valid user ID
            else if ((string.IsNullOrEmpty(UserName) == false) &&
                ((UserName == originalUserName) || (IUsers.Instance.Exists(x => x.Name == UserName) == false)))
            {
                // Check for a new administrator
                if ((originalUserType != UserType.Administrator) &&
                    (UserType == UserType.Administrator) &&
                    (administrators >= IUsers.MAXIMUM_ADMINISTRATORS))
                {
                    // Display the message for the required field
                    var dialog = new Message(Properties.Resources.EnterUser,
                        string.Format(Properties.Resources.MaximumAdministrators,
                        IUsers.MAXIMUM_ADMINISTRATORS));

                    // Reparent the controls
                    dialog.Reparent();

                    // Show the dialog
                    dialog.ShowDialog();
                }
                // Check for a password change
                else if (passwordChanged == false)
                {
                    // Issue the save command without a password change
                    IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                    {
                        Parameters = new Dictionary<string, object>() 
                        { 
                            { "UserName", originalUserName }, 
                            { "NewUserName", UserName },
                            { "NewUserType", UserType },
                        }
                    });
                }
                else if ((textBoxPassword.Text == textBoxVerify.Text) &&
                    IConfiguration.Instance.PasswordRules.CheckPassword(textBoxPassword.Text))
                {
                    // Issue the save command with a password change
                    IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                    {
                        Parameters = new Dictionary<string, object>() 
                        { 
                            { "UserName", originalUserName }, 
                            { "NewUserName", UserName }, 
                            { "NewUserType", UserType },
                            { "NewPassword", textBoxPassword.Text },
                        },
                    });
                }
                else
                {
                    // Show the invalid credentials dialog
                    var dialog = new Concrete.Message(titleBar.Text, Properties.Resources.InvalidCredentials);

                    // Reparent controls
                    dialog.Reparent();

                    // Show the dialog
                    dialog.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Click event handler for the delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Show the logout confirmation dialog and check the result
            if (new YesNo(Properties.Resources.DeleteUser, Properties.Resources.DeleteUserConfirm).
                ShowDialog() == DialogResult.Yes)
            {
                // Issue the save command with a password change
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Next)
                {
                    Parameters = new Dictionary<string, object>() 
                    { 
                        { "UserName", originalUserName },
                    }
                });
            }
        }

        /// <summary>
        /// Click event for administrator radio button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioAdministrator_Click(object sender, EventArgs e)
        {
            UserType = UserType.Administrator;
        }

        /// <summary>
        /// Click event for user radio button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioUser_Click(object sender, EventArgs e)
        {
            if (PermanentSystem == false)
            {
                UserType = UserType.User;
            }
        }
    }
}
