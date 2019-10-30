/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using IO.Model.Serializable;
using IO.Model.Volatile;
using IO.Controller;

namespace IO.View
{
    /// <summary>
    /// Base class for all forms
    /// </summary>
    public partial class Form : System.Windows.Forms.Form, IForm
    {
        /// <summary>
        /// Key debounce timer period
        /// </summary>
        public static readonly int KEY_DEBOUNCE_TIMER_PERIOD_MILLISECONDS = 100;

        /// <summary>
        /// The border width in pixels
        /// </summary>
        public static readonly int BORDER_WIDTH = 2;

        /// <summary>
        /// The corner radius in pixels
        /// </summary>
        public static readonly int CORNER_RADIUS = 6;

        /// <summary>
        /// Double the border width in pixels
        /// </summary>
        public static readonly int DOUBLE_BORDER_WIDTH = BORDER_WIDTH + BORDER_WIDTH;

        /// <summary>
        /// The scroll bar width
        /// </summary>
        public static readonly int SCROLL_BAR_WIDTH = 40;

        /// <summary>
        /// The main color
        /// </summary>
        public static readonly Color MAIN_COLOR = Color.FromArgb(192, 32, 128);

        /// <summary>
        /// The brush for the main color
        /// </summary>
        public static readonly SolidBrush MAIN_COLOR_BRUSH = new SolidBrush(MAIN_COLOR);

        /// <summary>
        /// The background color
        /// </summary>
        public static readonly Color BACKGROUND_COLOR = Color.White;

        /// <summary>
        /// The brush for the background color
        /// </summary>
        public static readonly SolidBrush BACKGROUND_COLOR_BRUSH = new SolidBrush(BACKGROUND_COLOR);

        /// <summary>
        /// The panel color
        /// </summary>
        public static readonly Color TITLE_COLOR = Color.FromArgb(129, 127, 131);

        /// <summary>
        /// The brush for the panel color
        /// </summary>
        public static readonly SolidBrush TITLE_COLOR_BRUSH = new SolidBrush(TITLE_COLOR);

        /// <summary>
        /// The panel color
        /// </summary>
        public static readonly Color PANEL_COLOR = Color.LightGray;

        /// <summary>
        /// The brush for the panel color
        /// </summary>
        public static readonly SolidBrush PANEL_COLOR_BRUSH = new SolidBrush(PANEL_COLOR);

        /// <summary>
        /// The panel color
        /// </summary>
        public static readonly Color BLUE_COLOR = Color.DodgerBlue;

        /// <summary>
        /// The brush for the panel color
        /// </summary>
        public static readonly SolidBrush BLUE_COLOR_BRUSH = new SolidBrush(BLUE_COLOR);

        /// <summary>
        /// The text color
        /// </summary>
        public static readonly Color TEXT_COLOR = Color.Black;

        /// <summary>
        /// The text color
        /// </summary>
        public static readonly SolidBrush TEXT_COLOR_BRUSH = new SolidBrush(TEXT_COLOR);

        /// <summary>
        /// The previous form in the sequence
        /// </summary>
        private Form PreviousForm { get; set; }

        /// <summary>
        /// The previous menu
        /// </summary>
        private Form PreviousMenu { get; set; }

        /// <summary>
        /// Form name accessor
        /// </summary>
        public string FormName
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// Help text associated with the form
        /// </summary>
        [EditorAttribute(
            "System.ComponentModel.Design.MultilineStringEditor, System.Design",
            "System.Drawing.Design.UITypeEditor")]
        public string HelpText { get; set; }

        /// <summary>
        /// Default constructor sets the initial date and time
        /// </summary>
        public Form()
        {
            InitializeComponent();

#if (DEBUG)
#else
            // Hide the cursor
            Cursor.Hide();
            TopMost = true;
#endif
        }

        /// <summary>
        /// Close all the windows in the stack
        /// </summary>
        public void Unwind()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Close all of the forms in the whole stack
                foreach (var form in GetFormStack(false))
                {
                    form.Close();
                }
            });
        }

        /// <summary>
        /// Show all the forms in the stack
        /// </summary>
        public void ShowAll()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Reset this form
                Reset();

                // Show all of the forms in the whole stack
                foreach (var form in GetFormStack(false))
                {
                    form.Show();
                }
            });
        }

        /// <summary>
        /// Hide all the forms in the stack
        /// </summary>
        public void HideAll()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Hide all of the forms in the whole stack
                foreach (var form in GetFormStack(false))
                {
                    form.Hide();
                }
            });
        }

        /// <summary>
        /// Unwind the form stack to the last menu
        /// </summary>
        public new void Menu()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check this form is visible
                if (Visible)
                {
                    // Get the form stack to the previous menu
                    var formStack = GetFormStack(true);
                    var previousMenu = formStack.Pop();

                    // Update the current form
                    previousMenu.ResetForm(true);

                    // Enable the menu
                    ViewManager.Instance.CurrentForm = previousMenu;

                    // Close all of the forms in the remaining stack
                    foreach (var form in formStack)
                    {
                        form.Close();
                    }
                }
            });
        }

        /// <summary>
        /// Unwind the form stack to the top
        /// </summary>
        public void Home()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check this form is visible
                if (Visible)
                {
                    // Get the form stack to the previous menu
                    var formStack = GetFormStack(false);
                    var previousMenu = formStack.Pop();

                    // Update the current form
                    previousMenu.ResetForm(true);

                    // Enable the menu
                    ViewManager.Instance.CurrentForm = previousMenu;

                    // Close all of the forms in the remaining stack
                    foreach (var form in formStack)
                    {
                        form.Close();
                    }
                }
            });
        }

        /// <summary>
        /// Unwind the form stack one form
        /// </summary>
        public void Back()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check this form is visible
                if (Visible)
                {
                    // Check for a previous form
                    if ((PreviousForm != null) || (PreviousMenu != null))
                    {
                        // Enable the previous form
                        ((PreviousMenu == null) ? PreviousForm : PreviousMenu).ResetForm(true);

                        // Close this form, displaying the previous form in the sequence
                        Close();

                        ViewManager.Instance.CurrentForm = (PreviousMenu == null) ? PreviousForm : 
                            PreviousMenu;
                    }
                    else
                    {
                        // Enable this form
                        ResetForm(true);

                        // Update the current form
                        ViewManager.Instance.CurrentForm = this;
                    }
                }
            });
        }

        /// <summary>
        /// Show a form from a menu
        /// </summary>
        /// <param name="form">The name of the form to show</param>
        /// <param name="goHomeFirst">Flag to go home before showing the form</param>
        /// <param name="parameters">The parameters for the form</param>
        public void ShowFormFromMenu(string name, bool goHomeFirst, 
            Dictionary<string, object> parameters)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check this form is visible
                if (Visible)
                {
                    // Get the type for the form
                    var type = Type.GetType("IO.View.Concrete." + name);

                    // Create a new instance of the form
                    var newForm = (Form)Activator.CreateInstance(type);

                    // Reparent any controls on the background image
                    newForm.Reparent();

                    // Check for parameters
                    if (parameters != null)
                    {
                        // Set the parameters on the form
                        foreach (var parameter in parameters)
                        {
                            type.InvokeMember(parameter.Key, BindingFlags.Instance | BindingFlags.Public |
                                BindingFlags.SetProperty, Type.DefaultBinder, newForm,
                                new object[] { parameter.Value });
                        }
                    }

                    // Check the go home first flag
                    if (goHomeFirst)
                    {
                        // Get the form stack to the previous menu
                        var formStack = GetFormStack(true);

                        // Set the previous menu to the top of the stack
                        newForm.PreviousMenu = formStack.Pop();

                        // Show the form
                        newForm.Show();

                        // Close all of the forms in the remaining stack
                        foreach (var form in formStack)
                        {
                            form.Close();
                        }
                    }
                    else
                    {
                        // Set the previous menu to this
                        newForm.PreviousMenu = this;

                        // Show the form
                        newForm.Show();
                    }

                    // Set the current form
                    ViewManager.Instance.CurrentForm = newForm;

                    // Reset the previous form
                    newForm.PreviousMenu.ResetForm(false);
                }
            });
        }

        /// <summary>
        /// Show a form from a form
        /// </summary>
        /// <param name="form">The name of the form to show</param>
        public void ShowFormFromForm(string name, Dictionary<string, object> parameters = null)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Get the type for the form
                var type = Type.GetType("IO.View.Concrete." + name);

                // Create a new instance of the form
                var form = (Form)Activator.CreateInstance(type);

                // Check for parameters
                if (parameters != null)
                {
                    // Set the parameters on the form
                    foreach (var parameter in parameters)
                    {
                        type.InvokeMember(parameter.Key, BindingFlags.Instance | BindingFlags.Public |
                            BindingFlags.SetProperty, Type.DefaultBinder, form, 
                            new object[] { parameter.Value });
                    }
                }

                // Show the form
                ShowFormFromForm(form);
            });
        }

        /// <summary>
        /// Show a message to the user
        /// </summary>
        /// <param name="resource">The resource name for the message</param>
        /// <param name="arg">Optional argument for the resource</param>
        public void ShowMessage(string resource, object arg = null)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Create the message window
                var dialog = new Concrete.Message(titleBar.Text, string.Format(
                    Properties.Resources.ResourceManager.GetString(resource), arg));

                // Reparent controls
                dialog.Reparent();

                // Show the dialog
                dialog.ShowDialog();

                // Reset this form
                ResetForm(true);
            });
        }

        /// <summary>
        /// Logout the current user
        /// </summary>
        public virtual void Logout()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check for a modal form
                if (Modal)
                {
                    // Close it
                    Close();
                }

                // Disable the controls
                EnableControls(false);

                // Kick off the wait timer
                timerWait.Enabled = true;

                // Enqueue the message
                MessageQueue.Instance.Push(new CommandMessage(FormName, FormCommand.Login));
            });
        }

        /// <summary>
        /// Disable the form, kick off the wait timer and issue the passed form command
        /// </summary>
        /// <param name="command">The command message</param>
        public void IssueFormCommand(CommandMessage commandMessage)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Disable the controls
                EnableControls(false);

                // Kick off the wait timer
                timerWait.Enabled = true;

                // Enqueue the message
                MessageQueue.Instance.Push(commandMessage);
            });
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        public void Reset()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                ResetForm(true);
            });
        }

        /// <summary>
        /// Reapply the resources to the stack
        /// </summary>
        /// <param name="cultureInfo">The new culture info</param>
        public void ReapplyResources(CultureInfo cultureInfo)
        {
            // Create a component resource manager
            ComponentResourceManager resources = new ComponentResourceManager(GetType());

            // Loop through the controls applying resources
            foreach (Control control in Controls)
            {
                resources.ApplyResources(control, control.Name, cultureInfo);
            }

            // Loop through the controls applying resources
            foreach (Control control in pictureBoxBackground.Controls)
            {
                resources.ApplyResources(control, control.Name, cultureInfo);
            }

            // Check for a previous form
            if ((PreviousForm != null) || (PreviousMenu != null))
            {
                // Reapply resources to the previous form
                ((PreviousMenu == null) ? PreviousForm : PreviousMenu).ReapplyResources(cultureInfo);
            }
        }

        /// <summary>
        /// Reparent any controls on the background image
        /// </summary>
        public virtual void Reparent()
        {
            // Make a list of controls within the backbround image
            var controls = new List<Control>();

            foreach (Control control in Controls)
            {
                // Ignore the background image itself and the animation
                if ((control != pictureBoxBackground) &&
                    (control != pictureBoxAnimation) &&
                    (control != titleBar) &&
                    pictureBoxBackground.Bounds.Contains(control.Bounds))
                {
                    controls.Add(control);
                }
            }

            // Loop through the controls reparenting them
            foreach (var control in controls)
            {
                // Make images transparent
                if (control is PictureBox)
                {
                    Transparent((PictureBox)control, pictureBoxBackground);
                }
                else
                {
                    Reparent(control, pictureBoxBackground);
                }
            }
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="visible">Whether the form is visible</param>
        protected virtual void ResetForm(bool visible)
        {
            // Deactivate the timer
            timerWait.Enabled = false;
            pictureBoxAnimation.Visible = false;

            // Enable the controls
            EnableControls(true);

            // Update the identity
            UpdateIdentity();

            // Update the status
            UpdateStatus();
        }

        /// <summary>
        /// Show a form from a form
        /// </summary>
        /// <param name="form"></param>
        protected void ShowFormFromForm(IForm form)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check this form is visible
                if (Visible)
                {
                    // Set the previous menu for the form
                    ((Form)form).PreviousForm = this;

                    // Reparent any controls on the background image
                    ((Form)form).Reparent();

                    // Show the form
                    ((Form)form).Show();

                    // Set the current form
                    ViewManager.Instance.CurrentForm = form;

                    // Enable this form
                    ResetForm(false);
                }
            });
        }

        /// <summary>
        /// Disable the form, kick off the wait timer and issue the passed menu item
        /// </summary>
        /// <param name="menuItem">The menu tiem</param>
        /// <param name="parameters">The message parameters</param>
        protected void IssueMenuItem(string menuItem, Dictionary<string, object> parameters = null)
        {
            // Disable the controls
            EnableControls(false);

            // Kick off the wait timer
            timerWait.Enabled = true;

            // Enqueue the message
            MessageQueue.Instance.Push(new MenuItemMessage(menuItem) { Parameters = parameters });
        }

        /// <summary>
        /// Enable the controls on the form
        /// </summary>
        /// <param name="enabled">True to enable or false to disable</param>
        protected virtual void EnableControls(bool enabled)
        {
            // Enable the timer
            timerClock.Enabled = enabled;

            // Enable the picture buttons
            pictureBoxBack.Enabled = enabled;
            pictureBoxHelp.Enabled = enabled;
            pictureBoxHome.Enabled = enabled;
            pictureBoxLogin.Enabled = enabled;

            // Enable all custom controls on the form
            foreach (var control in Controls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).Enabled = enabled;
                }
                else if (control is Button)
                {
                    ((Button)control).Enabled = enabled;
                }
                else if (control is Key)
                {
                    ((Key)control).Enabled = enabled;
                }
            }

            // Enable all custom controls on the form
            foreach (var control in pictureBoxBackground.Controls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).Enabled = enabled;
                }
                else if (control is Button)
                {
                    ((Button)control).Enabled = enabled;
                }
                else if (control is Key)
                {
                    ((Key)control).Enabled = enabled;
                }
            }
        }

        /// <summary>
        /// Update the date and time on the form
        /// </summary>
        protected virtual void UpdateStatus()
        {
            // Check for a running test
            if (ITests.Instance != null)
            {
                lock (ITests.Instance.CurrentTest)
                {
                    if ((ITests.Instance.CurrentTest.Result.StartDateTime != DateTime.MinValue) &&
                        (ITests.Instance.CurrentTest.Result.Assay != null) &&
                        (ITests.Instance.CurrentTest.Result.Assay.EstimatedDuration > 0))
                    {
                        // Calculate the remaining time
                        int remainingTimeInSeconds = 
                            (ITests.Instance.CurrentTest.Result.Assay.EstimatedDuration *
                            (100 - ITests.Instance.CurrentTest.PercentComplete)) / 100;

                        // Show the appropriate text
                        if (remainingTimeInSeconds > 119)
                        {
                            labelStatus.Text = string.Format(Properties.Resources.TestCompleteIn,
                                (remainingTimeInSeconds / 60), Properties.Resources.Minutes);
                        }
                        else if (remainingTimeInSeconds > 59)
                        {
                            labelStatus.Text = string.Format(Properties.Resources.TestCompleteIn,
                                1, Properties.Resources.Minute);
                        }
                        else if (remainingTimeInSeconds > 1)
                        {
                            labelStatus.Text = string.Format(Properties.Resources.TestCompleteIn,
                                remainingTimeInSeconds, Properties.Resources.Seconds);
                        }
                        else
                        {
                            labelStatus.Text = string.Format(Properties.Resources.TestCompleteIn,
                                1, Properties.Resources.Second);
                        }
                    }
                    else if (IController.Instance.PrinterNotFound)
                    {
                        // Show the quarantined text
                        labelStatus.Text = Properties.Resources.PrinterNotFound;
                    }
                    else if ((IConfiguration.Instance != null) &&
                        (IConfiguration.Instance.QuarantineState == QuarantineState.Locked))
                    {
                        // Show the quarantined text
                        labelStatus.Text = Properties.Resources.Quarantined;
                    }
                    else if ((IConfiguration.Instance.WarningPcrCycles > 0) &&
                        (IConfiguration.Instance.PcrCycles >= IConfiguration.Instance.WarningPcrCycles))
                    {
                        // Show the replace PCR text
                        labelStatus.Text = Properties.Resources.ReplacePcrStack;
                    }
                    else
                    {
                        // Show the idle text
                        labelStatus.Text = Properties.Resources.SystemIdle;
                    }
                }
            }
            else
            {
                // Show the idle text
                labelStatus.Text = Properties.Resources.SystemIdle;
            }
        }

        /// <summary>
        /// Reparent a control, maintaining screen position
        /// </summary>
        /// <param name="control">The control to reparent</param>
        /// <param name="newParent">The new parent</param>
        protected void Reparent(Control control, Control newParent)
        {
            // Get the bounds in screen coordinates
            var screenRect = control.RectangleToScreen(control.ClientRectangle);

            // Reparent
            control.Parent = newParent;

            // Move back to the same position
            control.Bounds = newParent.RectangleToClient(screenRect);
        }

        /// <summary>
        /// Make a picture box transparent by reparenting it
        /// </summary>
        /// <param name="pictureBox">The picture box to make transparent</param>
        /// <param name="newParent">The new parent</param>
        protected void Transparent(PictureBox pictureBox, Control newParent)
        {
            // Make the picture transparent
            pictureBox.BackColor = Color.Transparent; 
            ((Bitmap)pictureBox.Image).MakeTransparent(Color.White);

            // Reparent control
            Reparent(pictureBox, newParent);
        }

        /// <summary>
        /// Show the help screen
        /// </summary>
        protected virtual void ShowHelp()
        {
            // Create the help dialog
            var dialog = new Concrete.Help(HelpText);

            // Reparent controls
            dialog.Reparent();

            // Show the dialog
            dialog.ShowDialog();
        }

        /// <summary>
        /// Clock tick timer event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event arguments</param>
        protected virtual void timerClock_Tick(object sender, EventArgs e)
        {
            // Update the date and time
            UpdateDateTime();

            // Update the status
            UpdateStatus();
        }

        /// <summary>
        /// Form stack
        /// </summary>
        private Stack<Form> GetFormStack(bool stopAtMenu)
        {
            // Initialise a stack of forms and a form vraiable
            var formStack = new Stack<Form>();
            var form = this;

            // Walk back up the stack pushing the forms
            do
            {
                formStack.Push(form);

                // Check for a previous menu
                if (form.PreviousMenu == null)
                {
                    form = form.PreviousForm;
                }
                // Check to see if we are stopping at menus
                else if (stopAtMenu)
                {
                    formStack.Push(form.PreviousMenu);
                    form = null;
                }
                // Otherise continue
                else
                {
                    form = form.PreviousMenu;
                }
            }
            while (form != null);

            // Return the stack
            return formStack;
        }

        /// <summary>
        /// Update the date and time on the form
        /// </summary>
        private void UpdateDateTime()
        {
            labelTime.Text = DateTime.Now.ToString("HH:mm:ss");
            labelDate.Text = DateTime.Now.ToString("d MMM yyyy");
        }

        /// <summary>
        /// Update the identity
        /// </summary>
        private void UpdateIdentity()
        {
            // Check for a configuration object
            if ((ViewManager.Instance != null) && (IConfiguration.Instance != null))
            {
                // Check for an instrument name
                if (string.IsNullOrEmpty(IConfiguration.Instance.InstrumentName))
                {
                    // Check for an current user
                    if (ISessions.Instance.CurrentUser == null)
                    {
                        labelUser.Text = string.Empty;
                    }
                    else
                    {
                        labelUser.Text = ISessions.Instance.CurrentUser.Name;
                    }
                }
                else
                {
                    // Check for an current user
                    if (ISessions.Instance.CurrentUser == null)
                    {
                        labelUser.Text = IConfiguration.Instance.InstrumentName;
                    }
                    else
                    {
                        labelUser.Text = IConfiguration.Instance.InstrumentName + "/" +
                            ISessions.Instance.CurrentUser.Name;
                    }
                }
            }
            else
            {
                labelUser.Text = string.Empty;
            }
        }

        /// <summary>
        /// Wait timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerWait_Tick(object sender, EventArgs e)
        {
            // Create a bitmap for the background image
            var backgroundImage = new Bitmap(pictureBoxBackground.Width, pictureBoxBackground.Height);

            // Draw the background image
            pictureBoxBackground.DrawToBitmap(backgroundImage, pictureBoxBackground.ClientRectangle);

            // Extract the area of the bitmap that is coverd by the control and set this to the background
            pictureBoxAnimation.BackgroundImage = backgroundImage.Clone(new Rectangle(
                pictureBoxAnimation.Left - pictureBoxBackground.Left, 
                pictureBoxAnimation.Top - pictureBoxBackground.Top, 
                pictureBoxAnimation.Width, pictureBoxAnimation.Height), backgroundImage.PixelFormat);

            // Show the animation
            pictureBoxAnimation.BringToFront();
            pictureBoxAnimation.Visible = true;

            // Disable the timer
            timerWait.Enabled = false;
        }

        /// <summary>
        /// Click event handler for the home image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxHome_Click(object sender, EventArgs e)
        {
            // Check for a modal form
            if (Modal)
            {
                // Just close it
                Close();
            }
            else
            {
                // Issue the home form command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Home));
            }
        }

        /// <summary>
        /// Click event handler for the login image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxLogin_Click(object sender, EventArgs e)
        {
            // Check for a modal form
            if (Modal)
            {
                // Close it
                Close();
            }

            if (ISessions.Instance.CurrentUser != null)
            {
                // Create the dialog
                var dialog = new Concrete.YesNo(Properties.Resources.Login, Properties.Resources.LogoutConfirm);

                // Reparent controls
                dialog.Reparent();

                // Show the logout confirmation dialog and check the result
                if (dialog.ShowDialog() == DialogResult.Yes)
                {
                    // Issue the login form command
                    ViewManager.Instance.CurrentForm.IssueFormCommand(new CommandMessage(FormName, FormCommand.Login));
                }
            }
            else
            {
                // Issue the login form command
                ViewManager.Instance.CurrentForm.IssueFormCommand(new CommandMessage(FormName, FormCommand.Login));
            }
        }

        /// <summary>
        /// Click event handler for the help image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxHelp_Click(object sender, EventArgs e)
        {
            // Show the help screen
            ShowHelp();
        }

        /// <summary>
        /// Click event handler for the back image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            // Check for a modal form
            if (Modal)
            {
                // Just close it
                Close();
            }
            else
            {
                // Issue the back form command
                IssueFormCommand(new CommandMessage(FormName, FormCommand.Back));
            }
        }

        /// <summary>
        /// Load event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Load(object sender, EventArgs e)
        {
            // Update the date and time
            UpdateDateTime();

            // Reset the form
            ResetForm(true);
        }

        /// <summary>
        /// Mouse down event handler for picture boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox)
            {
                // Show the fixed single border
                ((PictureBox)sender).BorderStyle = BorderStyle.FixedSingle;
            }
        }

        /// <summary>
        /// Mouse up event handler for picture boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox)
            {
                // Show no border
                ((PictureBox)sender).BorderStyle = BorderStyle.None;
            }
        }
    }
}
