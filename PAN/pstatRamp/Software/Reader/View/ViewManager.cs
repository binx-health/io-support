/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Forms;
using IO.Model.Serializable;
using IO.Model.Volatile;
using IO.FileSystem;

namespace IO.View
{
    /// <summary>
    /// Singleton view manager that initialises and maintains the view state
    /// </summary>
    public class ViewManager : IViewManager, IMessageFilter
    {
        /// <summary>
        /// Printing constants
        /// </summary>
        private static readonly int PRINTER_TOTAL_WIDTH = 42;
        private static readonly int PRINTER_FIRST_COLUMN_WIDTH = 14;
        private static readonly string PRINTER_SEPARATOR = new string('*', PRINTER_TOTAL_WIDTH);

        /// <summary>
        /// The timer to log out the current user
        /// </summary>
        private System.Timers.Timer timerLogout = null;

        /// <summary>
        /// The background form
        /// </summary>
        private Background background = null;

        /// <summary>
        /// The current form
        /// </summary>
        public override IForm CurrentForm { get; set; }

        /// <summary>
        /// The list of locales
        /// </summary>
        public override Dictionary<string, CultureInfo> Locales { get; set; }

        /// <summary>
        /// Flag to indicate that thei vie manager is running
        /// </summary>
        public override bool IsRunning
        {
            get
            {
                return (background != null) && background.IsHandleCreated;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewManager()
        {
            // Initialise the locales for this assembly
            Locales = new Dictionary<string, CultureInfo>() 
            {
                { "English", System.Globalization.CultureInfo.GetCultureInfo("en-US") }, 
                { "Deutsch", System.Globalization.CultureInfo.GetCultureInfo("de-DE") } 
            };
        }

        /// <summary>
        /// Run the application
        /// </summary>
        public override void Run()
        {
            // Change the culture for the UI thread
            Thread.CurrentThread.CurrentUICulture = Locales[IConfiguration.Instance.Locale];

            // Initialise the windows forms environment
            Application.SetCompatibleTextRenderingDefault(false);

            // Add the message filter
            Application.AddMessageFilter(this);

            // Create the background form
            background = new Background();

            // Run the application
            Application.Run(background);
        }

        /// <summary>
        /// Stop the application
        /// </summary>
        public override void Stop()
        {
            background.BeginInvoke((MethodInvoker)delegate
            {
                // Close the background window
                background.Close();
            });
        }

        /// <summary>
        /// Create a new root form and return the previous stack
        /// </summary>
        /// <param name="name">The name of the new root form to show</param>
        /// <returns>The previous current form</returns>
        public override IForm ReplaceRoot(string name)
        {
            // Set the previous current form
            IForm previousCurrentForm = CurrentForm;

            background.BeginInvoke((MethodInvoker)delegate
            {
                // Get the type for the form
                var type = Type.GetType("IO.View.Concrete." + name);

                // Create a new instance of the form
                var form = (Form)Activator.CreateInstance(type);

                // Reparent any controls on the background image
                form.Reparent();

                // Show the form
                form.Show();

                // Set the current form
                CurrentForm = form;
            });

            // Hide all of the forms in the previous stack
            if (previousCurrentForm != null)
            {
                previousCurrentForm.HideAll();
            }

            return previousCurrentForm;
        }

        /// <summary>
        /// Replace the current stack with a new stack
        /// </summary>
        /// <param name="form">The form at the top of the new stack</param>
        /// <returns>The previous current form</returns>
        public override IForm ReplaceRoot(IForm form)
        {
            // Set the previous current form
            IForm previousCurrentForm = CurrentForm;

            // Show the form
            form.ShowAll();

            // Hide all of the forms in the previous stack
            if (previousCurrentForm != null)
            {
                CurrentForm.HideAll();
            }

            // Set the current form
            CurrentForm = form;

            return previousCurrentForm;
        }

        /// <summary>
        /// Update the current user
        /// </summary>
        /// <param name="value">The new value</param>
        public override void UpdateCurrentUser(IUser value)
        {
            // Dispose of any current login timer
            if (timerLogout != null)
            {
                timerLogout.Dispose();
                timerLogout = null;
            }

            // Check for a login
            if ((value != null) && (IConfiguration.Instance.AutoLogoffPeriodInSeconds > 0))
            {
                // Start a new logout timer
                timerLogout = new System.Timers.Timer()
                {
                    Interval = IConfiguration.Instance.AutoLogoffPeriodInSeconds * 1000,
                    Enabled = true,
                    AutoReset = false,
                };

                timerLogout.Elapsed += timerLogout_Elapsed;
            }
        }

        /// <summary>
        /// Update the timeout for logging out a user in seconds
        /// </summary>
        /// <param name="value">The new value</param>
        public override void UpdateAutoLogoffPeriodInSeconds(int value)
        {
            background.BeginInvoke((MethodInvoker)delegate
            {
                // Change the timer value if there is a logout timer running
                if (timerLogout != null)
                {
                    timerLogout.Enabled = false;
                    timerLogout.Interval = value * 1000;
                    timerLogout.Enabled = IConfiguration.Instance.AutoLogoffPeriodInSeconds > 0;
                }
            });
        }

        /// <summary>
        /// Update the current culture
        /// </summary>
        /// <param name="value">The new value</param>
        public override void UpdateCurrentCulture(string value)
        {
            background.Invoke((MethodInvoker)delegate
            {
                // Change the culture for the UI thread
                Thread.CurrentThread.CurrentUICulture = Locales[value];

                // Reapply the resources for the current stack
                CurrentForm.ReapplyResources(Thread.CurrentThread.CurrentUICulture);

                // Reapply the resources for each saved session stack
                foreach (var sessionForm in ISessions.Instance.Select(
                    x => x.Value.CurrentForm).Where(x => x != null))
                {
                    sessionForm.ReapplyResources(Thread.CurrentThread.CurrentUICulture);
                }
            });
        }

        /// <summary>
        /// Print a result
        /// </summary>
        /// <param name="sampleId">The sample ID</param>
        /// <param name="patientInformation">The patient information</param>
        /// <param name="test">The test</param>
        public override void PrintResult(string sampleId, Dictionary<string, object> patientInformation, 
            ITest test)
        {
            // Create a text writer
            using (var textWriter = new StringWriter())
            {
                // Print a separator
                textWriter.WriteLine(PRINTER_SEPARATOR);

                // Print the sample ID
                PrintLine(textWriter, Properties.Resources.SampleId, sampleId);

                // Loop through the other fields writing them
                foreach (var field in IConfiguration.Instance.Fields)
                {
                    // Check that it is a record field and that it has a value
                    object value;

                    if ((field.Policy == FieldPolicy.Record) &&
                        patientInformation.TryGetValue(field.Name, out value))
                    {
                        string stringValue;

                        // Format the value based on type
                        if (field.FieldType == FieldType.Date)
                        {
                            stringValue = ((DateTime)value).ToLocalTime().ToString("dd MMM yyyy");
                        }
                        else
                        {
                            stringValue = (string)value;
                        }

                        // Print the value
                        PrintLine(textWriter, Auxilliary.DisplayNameForField(field.Name), stringValue);
                    }
                }

                // Check for a test
                if (test != null)
                {
                    // Print the test type and outcome
                    if (test.Result.Assay != null)
                    {
                        PrintLine(textWriter, Properties.Resources.TestType, test.Result.Assay.Name);
                    }
                    PrintLine(textWriter, Properties.Resources.Positive, test.Result.PositivePeaks);
                    PrintLine(textWriter, Properties.Resources.Negative, test.Result.NegativePeaks);
                    PrintLine(textWriter, Properties.Resources.Invalid, test.Result.InvalidPeaks);

                    // Check for investigation use only assays
                    if ((test.Result.Assay != null) && (test.Result.Assay.InvestigationUseOnly))
                    {
                        textWriter.WriteLine(Properties.Resources.ForInvestigationUseOnly);
                    }

                    // Print a separator
                    textWriter.WriteLine();
                    textWriter.WriteLine(PRINTER_SEPARATOR);

                    // Determine the locking user
                    var lockingUser = IUsers.Instance.Where(x => x.ID == test.LockingUserId).FirstOrDefault();
                    string lockingUserName = (lockingUser != null) ? lockingUser.Name : string.Empty;

                    // Print the operator ID, cartridge code, assay version and timestamp
                    PrintLine(textWriter, Properties.Resources.OperatorId, lockingUserName);
                    PrintLine(textWriter, Properties.Resources.CartridgeCode, test.Result.CartridgeData);
                    PrintLine(textWriter, Properties.Resources.AssayVersion,test.Result.AssayVersion.ToString());
                    PrintLine(textWriter, Properties.Resources.DateAndTime, test.Result.EndDateTime.ToLocalTime().
                        ToString("dd MMM yyy HH:mmtt"));
                }

                // Print a separator
                textWriter.WriteLine(PRINTER_SEPARATOR);

                // Feed the paper to the cutter
                textWriter.Write(new string('\n', 7));

                // Push the job onto the queue
                PrintQueue.Instance.Push(textWriter.ToString());
            }
        }

        /// <summary>
        /// Export the audit log to file
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        public override void ExportAuditLog(ISimpleFileSystem fileSystem)
        {
            try
            {
                // Write the audit log
                fileSystem.WriteTextFile("AuditLog", string.Format(Properties.Resources.ExportHeader,
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), 
                    IConfiguration.Instance.InstrumentName) + 
                    ILocalFileSystem.Instance.ReadTextFile("AuditLog"));

                // Show the message window
                IViewManager.Instance.CurrentForm.ShowMessage("ExportSuccess", "AUDITLOG");
            }
            catch (Exception)
            {
                // Show the error
                IViewManager.Instance.CurrentForm.ShowMessage("ExportError");
            }
        }

        /// <summary>
        /// Export the results to file
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        public override void ExportResults(ISimpleFileSystem fileSystem)
        {
            try
            {
                // Write the results
                fileSystem.WriteTextFile("Results", string.Format(Properties.Resources.ExportHeader,
                    DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), 
                    IConfiguration.Instance.InstrumentName) + 
                    ILocalFileSystem.Instance.ReadTextFile("Results"));

                // Show the message window
                IViewManager.Instance.CurrentForm.ShowMessage("ExportSuccess", "RESULTS");
            }
            catch (Exception)
            {
                // Show the error
                IViewManager.Instance.CurrentForm.ShowMessage("ExportError");
            }
        }

        /// <summary>
        /// Message filter to handle mouse clicks
        /// </summary>
        /// <param name="m">The message</param>
        /// <returns>True</returns>
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            // Check for a screen press
            if ((m.Msg == 513) ||           // WM_LBUTTONDOWN
                (m.Msg == 514))             // WM_LBUTTONUP
            {
                // If we have a timer running then reset it
                if (timerLogout != null)
                {
                    timerLogout.Stop();
                    timerLogout.Start();
                }
            }

            return false;
        }

        /// <summary>
        /// Tick event handler for the logout timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerLogout_Elapsed(Object sender, EventArgs e)
        {
            // Call logout on the current form
            CurrentForm.Logout();
        }

        /// <summary>
        /// Print a line of information
        /// </summary>
        /// <param name="textWriter">The text writer</param>
        /// <param name="name">The field name</param>
        /// <param name="value">The field value</param>
        private void PrintLine(TextWriter textWriter, string name, string value)
        {
            // Check for a null or empty value
            if (string.IsNullOrEmpty(value) == false)
            {
                // Write the line
                textWriter.WriteLine(name.PadRight(PRINTER_FIRST_COLUMN_WIDTH).
                    Substring(0, PRINTER_FIRST_COLUMN_WIDTH) + " : " + value);
            }
        }
    }
}
