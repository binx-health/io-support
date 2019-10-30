/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using IO.Model.Serializable;
using IO.Model.Volatile;
using IO.Scripting;

namespace IO.View.Tests
{
    /// <summary>
    /// The test program
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // Create the view manager
            IViewManager.Instance = new ViewManager();

            // Initialise the model
            IState.Instance = new State();
            IConfiguration.Instance = new Configuration();
            IConfiguration.Instance.Fields = new List<IField>()
            {
                new Field() { Name = "Name", FieldType = FieldType.Text, Policy = FieldPolicy.Display },
                new Field() { Name = "Surname", FieldType = FieldType.Text, Policy = FieldPolicy.Display },
                new Field() { Name = "ContactNo", FieldType = FieldType.Text, Policy = FieldPolicy.Ignore },
                new Field() { Name = "Address", FieldType = FieldType.Text, Policy = FieldPolicy.Display },
                new Field() { Name = "PatientId", FieldType = FieldType.Text, Policy = FieldPolicy.Record },
                new Field() { Name = "DateOfBirth", FieldType = FieldType.Date, Policy = FieldPolicy.Display },
            };
            IUsers.Instance = new Users();
            IUsers.Instance.Add(new User()
            {
                ID = 1,
                Name = "admin",
                Type = UserType.Administrator,
                Password = "",
                PasswordDate = DateTime.UtcNow,
            });
            ISessions.Instance = new Sessions();
            ITests.Instance = new Tests()
            {
                CurrentTest = new Test(),
            };
            IAssays.Instance = new Assays();
            IResults.Instance = new Results();

            // Launch the controller thread
            new Thread(ControllerThreadProcedure).Start();

            // Run the application
            IViewManager.Instance.Run();
        }

        /// <summary>
        /// The controller thread procedure
        /// </summary>
        private static void ControllerThreadProcedure()
        {
            // Wait for the view manager to start
            while (IViewManager.Instance.IsRunning == false)
            {
                Thread.Sleep(1);
            }

            // Create the splash screen
            IViewManager.Instance.ReplaceRoot("Splash");

            while (true)
            {
                var waitHandles = new WaitHandle[] { MessageQueue.Instance };

                WaitHandle.WaitAny(waitHandles);

                var message = MessageQueue.Instance.Pop();

                if (message is CommandMessage)
                {
                    var commandMessage = message as CommandMessage;

                    if (commandMessage.Command == FormCommand.Initialise)
                    {
                        // Simulate the initialisation by sleeping
                        Thread.Sleep(1000);
                        
                        // Invoke the close method on the current form
                        IViewManager.Instance.ReplaceRoot("Start").Unwind();
                    }
                    else if ((commandMessage.Command == FormCommand.Menu) ||
                        (commandMessage.Command == FormCommand.Abort) ||
                        (commandMessage.Command == FormCommand.Home))
                    {
                        // Abort any partially initialised tests for the current user
                        if (IncompleteTestSetup())
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Clear the current test
                                ITests.Instance.CurrentTest.Clear();
                            }
                        }

                        // Invoke the relevant method on the current form
                        if (commandMessage.Command == FormCommand.Home)
                        {
                            IViewManager.Instance.CurrentForm.Home();
                        }
                        else
                        {
                            IViewManager.Instance.CurrentForm.Menu();
                        }
                    }
                    else if (commandMessage.Command == FormCommand.Back)
                    {
                        // Check for the test running screen
                        if (commandMessage.FormName == "TestRunning")
                        {
                            // Reset the current form
                            IViewManager.Instance.CurrentForm.Reset();
                        }
                        else if (commandMessage.FormName == "SampleId")
                        {
                            // Abort any partially initialised tests for the current user
                            if (IncompleteTestSetup())
                            {
                                lock (ITests.Instance.CurrentTest)
                                {
                                    // Clear the current test
                                    ITests.Instance.CurrentTest.Clear();
                                }
                            }

                            // Go back to the last menu
                            IViewManager.Instance.CurrentForm.Menu();
                        }
                        else
                        {
                            // Invoke the back method on the current form
                            IViewManager.Instance.CurrentForm.Back();
                        }
                    }
                    else if (commandMessage.Command == FormCommand.Login)
                    {
                        // Abort any partially initialised tests for the current user
                        var incompleteTestSetup = IncompleteTestSetup();

                        // Abort any incomplete test
                        if (incompleteTestSetup)
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Clear the current test
                                ITests.Instance.CurrentTest.Clear();
                            }
                        }

                        // Remember the current session
                        ISession previousSession = ISessions.Instance.CurrentSession;

                        // Clear the current user
                        ISessions.Instance.CurrentUser = null;
                        IViewManager.Instance.UpdateCurrentUser(null);

                        // Check for the login form
                        if (commandMessage.FormName == "Login")
                        {
                            IViewManager.Instance.CurrentForm.Reset();
                        }
                        // Check for the start form
                        else if (commandMessage.FormName == "Start")
                        {
                            IViewManager.Instance.CurrentForm.ShowFormFromMenu("Login");
                        }
                        // Check to see if we are currently logged in
                        else if (previousSession == null)
                        {
                            IViewManager.Instance.ReplaceRoot("Start").Unwind();
                        }
                        // Otherwise we are logging out
                        else
                        {
                            // Save the current form and replace with the start screen
                            previousSession.CurrentForm = IViewManager.Instance.ReplaceRoot("Start");

                            // Check if we aborted a test setup and eject the cartridge
                            if (incompleteTestSetup)
                            {
                                // We have aborted a test setup so unwind the stack and clear the current form
                                previousSession.CurrentForm.Unwind();
                                previousSession.CurrentForm = null;
                            }
                        }
                    }
                    else if (commandMessage.Command == FormCommand.Next)
                    {
                        if (commandMessage.FormName == "Start")
                        {
                            // Show the login screen
                            IViewManager.Instance.CurrentForm.ShowFormFromMenu("Login");
                        }
                        else if (commandMessage.FormName == "Login")
                        {
                            // Get the user name and password hash
                            var name = (string)commandMessage.Parameters["UserName"];
                            var passwordHash = IUser.HashPassword((string)commandMessage.Parameters["Password"]);

                            // Search for a valid user
                            IUser validUser = IUsers.Instance.Where(x => (x.Name == name) &&
                                (x.PasswordHash == passwordHash)).FirstOrDefault();

                            if (validUser != null)
                            {
                                // Set the current user
                                ISessions.Instance.CurrentUser = validUser;

                                // Update the current user in the view
                                IViewManager.Instance.UpdateCurrentUser(validUser);

                                // Get the current session
                                ISession currentSession = ISessions.Instance.CurrentSession;

                                // Check for an expired password
                                if (ISessions.Instance.CurrentUser.PasswordExpired)
                                {
                                    IViewManager.Instance.ReplaceRoot("PasswordExpired").Unwind();
                                }
                                // Check for a current form and show this
                                else if (currentSession.CurrentForm != null)
                                {
                                    IViewManager.Instance.ReplaceRoot(currentSession.CurrentForm).Unwind();
                                }
                                // Otherwise check for an administrator user and show the administrator menu
                                else if (validUser.Type == UserType.Administrator)
                                {
                                    IViewManager.Instance.ReplaceRoot("MainMenuAdmin").Unwind();
                                }
                                // Otherwise show the user menu
                                else
                                {
                                    IViewManager.Instance.ReplaceRoot("MainMenu").Unwind();
                                }
                            }
                            else
                            {
                                // Show the invalid credentials message
                                IViewManager.Instance.CurrentForm.ShowMessage("InvalidCredentials");

                                // The login failed so reset the login form
                                IViewManager.Instance.CurrentForm.Reset();
                            }
                        }
                        else if (commandMessage.FormName == "InstrumentName")
                        {
                            // Get the instrument name
                            var instrumentName = (string)commandMessage.Parameters["InstrumentName"];

                            // Set the configuration
                            IConfiguration.Instance.InstrumentName = instrumentName;

                            // Save the configuration
                            SerialiseConfiguration();

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "Language")
                        {
                            // Get the language
                            var language = (string)commandMessage.Parameters["Language"];

                            // Set the locale in the configuration
                            IConfiguration.Instance.Locale = language;

                            // Save the configuration
                            SerialiseConfiguration();

                            // Update the current culture and return to the menu
                            IViewManager.Instance.UpdateCurrentCulture(language);
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "DateAndTime")
                        {
                            // Get the new date and time
                            var value = ((DateTime)commandMessage.Parameters["Value"]).ToLocalTime();

                            // Set the machine date and time
                            Microsoft.VisualBasic.DateAndTime.Today = value;
                            Microsoft.VisualBasic.DateAndTime.TimeOfDay = value;

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "AssaySettings")
                        {
                            // Get the new UNG flag
                            var value = (bool)commandMessage.Parameters["Value"];

                            // Set the machine date and time
                            IConfiguration.Instance.Ung = value;

                            // Save the configuration
                            SerialiseConfiguration();

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "Timeout")
                        {
                            // Get the timeout value
                            var value = (int)commandMessage.Parameters["Value"];

                            // Set the new value in the configuration
                            IConfiguration.Instance.AutoLogoffPeriodInSeconds = value;

                            // Save the configuration
                            SerialiseConfiguration();

                            // Update the view and return to the menu
                            IViewManager.Instance.UpdateAutoLogoffPeriodInSeconds(value);
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "Users")
                        {
                            // Show the edit user form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("EditUser", commandMessage.Parameters);
                        }
                        else if (commandMessage.FormName == "EditUser")
                        {
                            // Try to get the current user ID
                            object userName;

                            if (commandMessage.Parameters.TryGetValue("UserName", out userName))
                            {
                                // This is an existing user
                                IUser user = IUsers.Instance.Where(x => x.Name == (string)userName).First();

                                // Try to get the new user ID
                                object newUserName;

                                if (commandMessage.Parameters.TryGetValue("NewUserName", out newUserName))
                                {
                                    // This is an edit
                                    user.Name = (string)newUserName;
                                    user.Type = (UserType)commandMessage.Parameters["NewUserType"];

                                    // Try to get the password
                                    object newPassword;

                                    if (commandMessage.Parameters.TryGetValue("NewPassword", out newPassword))
                                    {
                                        user.Password = (string)newPassword;
                                    }
                                }
                                else
                                {
                                    // This is a delete
                                    IUsers.Instance.Remove(user);
                                }
                            }
                            else
                            {
                                // This is a new user
                                IUsers.Instance.CreateNewUser(
                                    (UserType)commandMessage.Parameters["NewUserType"],
                                    (string)commandMessage.Parameters["NewUserName"],
                                    (string)commandMessage.Parameters["NewPassword"]);
                            }

                            // Serialise the user data
                            SerialiseUserData();

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "PasswordRules")
                        {
                            // Set the configuration from the parameters
                            IConfiguration.Instance.PasswordRules.MinimumLength =
                                (int)commandMessage.Parameters["MinimumLength"];
                            IConfiguration.Instance.PasswordRules.MinimumAlphabetical =
                                (int)commandMessage.Parameters["MinimumAlphabetical"];
                            IConfiguration.Instance.PasswordRules.MinimumAlphanumeric =
                                (int)commandMessage.Parameters["MinimumAlphanumeric"];
                            IConfiguration.Instance.PasswordRules.ExpiryTimeInDays =
                                (int)commandMessage.Parameters["ExpiryTimeInDays"];
                            IConfiguration.Instance.PasswordRules.Override =
                                (bool)commandMessage.Parameters["Override"];

                            // Save the configuration
                            SerialiseConfiguration();

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "ServerSettings")
                        {
                            // Set the configuration from the parameters
                            IConfiguration.Instance.Poct1ServerUri =
                                (string)commandMessage.Parameters["Poct1ServerUri"];
                            IConfiguration.Instance.Poct1ServerPort =
                                (int)commandMessage.Parameters["Poct1ServerPort"];

                            // Save the configuration
                            SerialiseConfiguration();

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "DataPolicy")
                        {
                            // Loop through the field policies updating them
                            foreach (var fieldPolicy in commandMessage.Parameters)
                            {
                                // Find the associated field
                                var field = IConfiguration.Instance.Fields.Where(
                                    x => x.Name == fieldPolicy.Key).FirstOrDefault();

                                // Set the new policy
                                field.Policy = (FieldPolicy)fieldPolicy.Value;
                            }

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "TestCancel")
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Check that the curren test is running
                                if (ITests.Instance.CurrentTest.Result.StartDateTime != null)
                                {
                                    // Set the end date for the current test
                                    ITests.Instance.CurrentTest.Result.EndDateTime = DateTime.UtcNow;

                                    // Set the outcome and peaks for the current test
                                    ITests.Instance.CurrentTest.Result.Outcome = TestOutcome.UserAborted;

                                    // Set the users current test to this one
                                    ISessions.Instance[ITests.Instance.CurrentTest.LockingUserId].CurrentTest =
                                        ITests.Instance.CurrentTest;

                                    // Create a new current test
                                    ITests.Instance.CreateNewTest();
                                }
                            }

                            // Return to the menu
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "SampleId")
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Set the sample ID in the test
                                ITests.Instance.CurrentTest.Result.SampleId =
                                    (string)commandMessage.Parameters["SampleId"];
                            }

                            // Show the patient information form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("PatientInformation");
                        }
                        else if (commandMessage.FormName == "PatientInformation")
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Set the QC test flag in the test result
                                ITests.Instance.CurrentTest.Result.QcTest =
                                        (bool)commandMessage.Parameters["QcTest"];

                                // Remove the QcTest value from the parameters
                                commandMessage.Parameters.Remove("QcTest");

                                // Set the patient information to the remaining parameters
                                ITests.Instance.CurrentTest.Result.PatientInformation =
                                    commandMessage.Parameters;
                            }

                            // Show the load sample form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("LoadSample");
                        }
                        else if (commandMessage.FormName == "PatientInformationEdit")
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Set the QC test flag in the test result
                                ITests.Instance.CurrentTest.Result.QcTest =
                                        (bool)commandMessage.Parameters["QcTest"];

                                // Remove the QcTest value from the parameters
                                commandMessage.Parameters.Remove("QcTest");

                                // Set the patient information to the remaining parameters
                                ITests.Instance.CurrentTest.Result.PatientInformation =
                                    commandMessage.Parameters;
                            }

                            // Go back to the review form
                            IViewManager.Instance.CurrentForm.Back();
                        }
                        else if (commandMessage.FormName == "LoadSample")
                        {
                            // Show the confirm sample form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("ConfirmSample");
                        }
                        else if (commandMessage.FormName == "ConfirmSample")
                        {
                            // Set the scanned barcode
                            IState.Instance.ScannedBarcode = "132090870000000000";

                            // Show the cartridge error form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("LoadCartridge");
                        }
                        else if (commandMessage.FormName == "LoadCartridge")
                        {
                            // Check for a barcode scan from the firmware
                            if (string.IsNullOrEmpty(IState.Instance.ScannedBarcode))
                            {
                                // Show the cartridge error form
                                IViewManager.Instance.CurrentForm.ShowFormFromForm("CartridgeError");
                            }
                            else
                            {
                                try
                                {
                                    // Get the assay for the barcode
                                    DateTime expiryDate;
                                    var assay = IAssays.Instance.GetAssayForBarcode(IState.Instance.ScannedBarcode,
                                        out expiryDate);

                                    // Check for an expired cartridge
                                    if (expiryDate < DateTime.UtcNow)
                                    {
                                        IViewManager.Instance.CurrentForm.ShowFormFromForm("ExpiredCartridge");
                                    }
                                    // Check for a known assay
                                    else if (assay != null)
                                    {
                                        lock (ITests.Instance.CurrentTest)
                                        {
                                            ITests.Instance.CurrentTest.Result.Assay = assay;
                                            ITests.Instance.CurrentTest.Result.AssayVersion = assay.Version;
                                        }

                                        IViewManager.Instance.CurrentForm.ShowFormFromForm("ReviewTest");
                                    }
                                    // Unknown assay
                                    else
                                    {
                                        IViewManager.Instance.CurrentForm.ShowFormFromMenu("IncorrectCartridge");
                                    }
                                }
                                catch (Exception)
                                {
                                    // Show the cartridge error form
                                    IViewManager.Instance.CurrentForm.ShowFormFromForm("CartridgeError");
                                }
                            }
                        }
                        else if (commandMessage.FormName == "ReviewTest")
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Set the test state to started
                                ITests.Instance.CurrentTest.Result.StartDateTime = DateTime.UtcNow;
                                ITests.Instance.CurrentTest.Result.CartridgeData = IState.Instance.ScannedBarcode;
                            }

                            // Start the test thread
                            new Thread(TestThreadProcedure).Start();

                            // Show the running form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("TestRunning");
                        }
                        else if (commandMessage.FormName == "TestRunning")
                        {
                            // Show the test complete form
                            IViewManager.Instance.CurrentForm.ShowFormFromMenu("TestComplete", true);
                        }
                        else if (commandMessage.FormName == "TestComplete")
                        {
                            // Show the test result form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("TestResult");
                        }
                        else if (commandMessage.FormName == "TestResult")
                        {
                            // Clear the current test for this session
                            ISessions.Instance.CurrentSession.CurrentTest = null;

                            lock (ITests.Instance.CurrentTest)
                            {
                                // Check if the current test is locked
                                if (ITests.Instance.CurrentTest.LockingUserId == 0)
                                {
                                    // Set the locking user
                                    ITests.Instance.CurrentTest.LockingUserId = ISessions.Instance.CurrentUser.ID;

                                    // Show the sample ID form
                                    IViewManager.Instance.CurrentForm.ShowFormFromMenu("SampleId", true);
                                }
                                else
                                {
                                    // Reset the current form
                                    IViewManager.Instance.CurrentForm.Reset();
                                }
                            }
                        }
                        else if (commandMessage.FormName == "IncorrectCartridge")
                        {
                            // Show the add new assay form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("AddNewAssay");
                        }
                        else if (commandMessage.FormName == "DownloadAssay")
                        {
                            // Simulate by sleeping
                            Thread.Sleep(2000);

                            // Download assays from the cloud
                            var assays = new List<IAssay>();

                            assays.Add(new Assay()
                            {
                                Name = "C.Trachomatis",
                                Version = 0,
                                ShortName = "Ct",
                                Code = "700",
                                EstimatedDuration = 20
                            });

                            // Show the select new assay form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("SelectNewAssay",
                                new Dictionary<string, object>() 
                            { 
                                { "Assays", assays }, 
                                { "FileSystem", null }
                            });
                        }
                        else if (commandMessage.FormName == "LocalUsbAssay")
                        {
                            // Simulate by sleeping
                            Thread.Sleep(2000);

                            // Download assays from the cloud
                            var assays = new List<IAssay>();

                            // Show the select new assay form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("SelectNewAssay",
                                new Dictionary<string, object>() 
                            { 
                                { "Assays", assays }, 
                                { "FileSystem", null }, 
                            });
                        }
                        else if (commandMessage.FormName == "SelectNewAssay")
                        {
                            // Show the result form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("UpdateAssay",
                                commandMessage.Parameters);
                        }
                        else if (commandMessage.FormName == "UpdateAssay")
                        {
                            // Simulate the by sleeping
                            Thread.Sleep(1000);

                            // Add the assay
                            IAssays.Instance.Add((IAssay)commandMessage.Parameters["Assay"]);

                            // Return to the last menu
                            IViewManager.Instance.CurrentForm.Menu();
                        }
                        else if (commandMessage.FormName == "SearchResults")
                        {
                            // Show the searched results form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("SearchedResults",
                                commandMessage.Parameters);
                        }
                        else if ((commandMessage.FormName == "AllResults") ||
                            (commandMessage.FormName == "SearchedResults"))
                        {
                            // Show the result form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("Result",
                                commandMessage.Parameters);
                        }
                        else if (commandMessage.FormName == "PasswordExpired")
                        {
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("UpdatePassword");
                        }
                        else if (commandMessage.FormName == "UpdatePassword")
                        {
                            // Update the password and password date
                            ISessions.Instance.CurrentUser.Password =
                                (string)commandMessage.Parameters["NewPassword"];
                            ISessions.Instance.CurrentUser.PasswordDate = DateTime.UtcNow;

                            // Serialise the user data
                            SerialiseUserData();

                            // Clear the current user
                            ISessions.Instance.CurrentUser = null;
                            IViewManager.Instance.UpdateCurrentUser(null);

                            // Show the start form
                            IViewManager.Instance.ReplaceRoot("Start").Unwind();
                        }
                    }
                    else if (commandMessage.Command == FormCommand.Shutdown)
                    {
                        // Simulate the shutdown by sleeping
                        Thread.Sleep(1000);

                        break;
                    }
                    else if (commandMessage.Command == FormCommand.Print)
                    {
                        // Show the printer not found form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("PrinterNotFound");
                    }
                }
                else if (message is MenuItemMessage)
                {
                    var menuItemMessage = message as MenuItemMessage;

                    // Check for a run test menu item
                    if (menuItemMessage.MenuItem == "RunTest")
                    {
                        lock (ITests.Instance.CurrentTest)
                        {
                            // Check if the current test is locked
                            if (ITests.Instance.CurrentTest.LockingUserId == 0)
                            {
                                // Set the locking user
                                ITests.Instance.CurrentTest.LockingUserId = ISessions.Instance.CurrentUser.ID;

                                // Show the sample Id form
                                IViewManager.Instance.CurrentForm.ShowFormFromMenu("SampleId");
                            }
                            else
                            {
                                // Show the test running form
                                IViewManager.Instance.CurrentForm.ShowFormFromMenu("TestCancel");
                            }
                        }
                    }
                    // Check for forms that have menu items, which are name of the next form
                    else if ((menuItemMessage.MenuItem == "DownloadAssay") ||
                        (menuItemMessage.MenuItem == "LocalUsbAssay"))
                    {
                        // Show the new form from the form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm(menuItemMessage.MenuItem);
                    }
                    // Check for a search results menu item
                    else if (menuItemMessage.MenuItem == "SearchResults")
                    {
                        // Show the new form from the form
                        IViewManager.Instance.CurrentForm.ShowFormFromMenu("SearchResults", false,
                            new Dictionary<string, object>() 
                                { 
                                    { "Assays", IAssays.Instance }, 
                                });
                    }
                    // Check for a patient information edit menu item
                    else if (menuItemMessage.MenuItem == "PatientInformationEdit")
                    {
                        // Initialise a copy of the patient information
                        var parientInformation = new Dictionary<string, object>();

                        foreach (var value in ITests.Instance.CurrentTest.Result.PatientInformation)
                        {
                            parientInformation.Add(value.Key, value.Value);
                        }

                        // Set the QcTest value
                        parientInformation["QcTest"] = ITests.Instance.CurrentTest.Result.QcTest;

                        // Show the new form from the form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("PatientInformationEdit",
                            new Dictionary<string, object>() { { "FieldValues", parientInformation } });
                    }
                    // This is a menu with a menu item, which is the name of the next form
                    else
                    {
                        // Show the new form from the menu
                        IViewManager.Instance.CurrentForm.ShowFormFromMenu(menuItemMessage.MenuItem);
                    }
                }
            }

            Application.Exit();
        }

        /// <summary>
        /// The controller thread procedure
        /// </summary>
        private static void TestThreadProcedure()
        {
            while (true)
            {
                lock (ITests.Instance.CurrentTest)
                {
                    if ((ITests.Instance.CurrentTest.Result.StartDateTime != DateTime.MinValue) &&
                        (ITests.Instance.CurrentTest.Result.Assay != null) &&
                        (ITests.Instance.CurrentTest.Result.Assay.EstimatedDuration > 0))
                    {
                        ITests.Instance.CurrentTest.PercentComplete = Math.Min(100, ((int)(DateTime.UtcNow -
                            ITests.Instance.CurrentTest.Result.StartDateTime).TotalSeconds * 100) /
                            ITests.Instance.CurrentTest.Result.Assay.EstimatedDuration);

                        if (ITests.Instance.CurrentTest.PercentComplete == 100)
                        {
                            // Set the end date for the current test
                            ITests.Instance.CurrentTest.Result.EndDateTime = DateTime.UtcNow;
                            ITests.Instance.CurrentTest.Result.PositivePeaks = "CT";

                            // Set the outcome and peaks for the current test
                            ITests.Instance.CurrentTest.Result.Outcome = TestOutcome.Valid;

                            // Set the users current test to this one
                            ISessions.Instance[ITests.Instance.CurrentTest.LockingUserId].CurrentTest =
                                ITests.Instance.CurrentTest;

                            // Create a new current test
                            ITests.Instance.CreateNewTest();

                            break;
                        }
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Serialise the configuration
        /// </summary>
        private static void SerialiseConfiguration()
        {
        }

        /// <summary>
        /// Serialise user data
        /// </summary>
        private static void SerialiseUserData()
        {
        }

        /// <summary>
        /// Check for an incomplete test setup
        /// </summary>
        /// <returns>True for an incomplete test setup, false otherwise</returns>
        private static bool IncompleteTestSetup()
        {
            lock (ITests.Instance.CurrentTest)
            {
                return (ISessions.Instance.CurrentUser != null) &&
                    (ITests.Instance.CurrentTest.LockingUserId == ISessions.Instance.CurrentUser.ID) &&
                    (ITests.Instance.CurrentTest.Result.StartDateTime == DateTime.MinValue);
            }
        }
    }
}
