/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using IO.View;
using IO.Model.Serializable;
using IO.Model.Volatile;
using IO.Scripting;
using IO.FileSystem;

namespace IO.Controller
{
    /// <summary>
    /// The controller object
    /// </summary>
    partial class Controller
    {
        /// <summary>
        /// Handle a view message
        /// </summary>
        /// <param name="message">The message</param>
        private void HandleViewMessage(Message message)
        {
            if (message is CommandMessage)
            {
                HandleCommandMessage(message as CommandMessage);
            }
            else if (message is MenuItemMessage)
            {
                HandleMenuItemMessage(message as MenuItemMessage);
            }
        }

        /// <summary>
        /// Handle a command message
        /// </summary>
        /// <param name="commandMessage">The message</param>
        private void HandleCommandMessage(CommandMessage commandMessage)
        {
            if (commandMessage.Command == FormCommand.Initialise)
            {
                // If we have not already errored then try to intialise the comms
                if (string.IsNullOrEmpty(LastError.Message) == false)
                {
                    // Show the fault screen
                    IViewManager.Instance.ReplaceRoot("Fault").Unwind();
                }
                else
                {
                    try
                    {
                        "Flushing results".Log();

                        // Flush the results file
                        while (IResults.Instance.NextResult() != null)
                        {
                            // Increment the totoal results
                            totalResults++;
                        }

                        "Initialising server thread".Log();

                        // Create a new server thread
                        serverThread = new Thread(ServerThreadProcedure);

                        // Start the server thread
                        serverThread.Start();

                        "Initialising print thread".Log();

                        // Create a new print thread
                        printThread = new Thread(PrintThreadProcedure);

                        // Start the print thread
                        printThread.Start();

                        "Initialising network manager".Log();

                        // Initialise the network manager
                        IO.Comms.Network.IManager.Instance.Initialise();

                        "Initialising firmware manager".Log();

                        // Initialise the firmware manager
                        IO.Comms.Firmware.IManager.Instance.Initialise();

                        "Executing startup script".Log();

                        // Check for a cartridge in the machine
                        if (string.IsNullOrEmpty(IState.Instance.ScannedBarcode) == false)
                        {
                            if (ExecuteScript("StartUpUnload"))
                            {
                                // Show the close drawer form
                                IViewManager.Instance.CurrentForm.ShowFormFromForm("UnexpectedShutdown");

                                // Set the phase to start-up
                                SetPhase("StartUp.Unload");
                            }
                            else
                            {
                                // Show the fault
                                ShowFault("Script sequence error running 'StartUp'");
                            }
                        }
                        // Kick off the start-up script
                        else if (ExecuteScript("StartUp"))
                        {
                            // Set the phase to start-up
                            SetPhase("StartUp.Initialise");
                        }
                        else
                        {
                            // Show the fault
                            ShowFault("Script sequence error running 'StartUp'");
                        }
                    }
                    catch (Exception e)
                    {
                        // Log the fault
                        e.Message.Log();

                        // Show the fault
                        ShowFault(e.Message);
                    }
                }
            }
            else if (commandMessage.Command == FormCommand.Shutdown)
            {
                // Initiate a system shutdown
                ShutdownSystem();
            }
            else if ((commandMessage.Command == FormCommand.Menu) ||
                (commandMessage.Command == FormCommand.Abort) ||
                (commandMessage.Command == FormCommand.Home))
            {
                // Abort any partially initialised tests for the current user
                if (IncompleteTestSetup())
                {
                    // Wait for any scripts to complete
                    if (string.IsNullOrEmpty(runningScript) == false)
                    {
                        // Push this message back onto the queue
                        IViewManager.Instance.CurrentForm.IssueFormCommand(commandMessage);
                    }
                    else
                    {
                        lock (ITests.Instance.CurrentTest)
                        {
                            // Clear the current test
                            ITests.Instance.CurrentTest.Clear();
                        }

                        // Eject the cartridge
                        if (EjectCartridge())
                        {
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
                    }
                }
                else
                {
                    // Clear the current test for this session
                    if (ISessions.Instance.CurrentSession != null)
                    {
                        ISessions.Instance.CurrentSession.CurrentTest = null;
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
                        // Wait for any scripts to complete
                        if (string.IsNullOrEmpty(runningScript) == false)
                        {
                            // Push this message back onto the queue
                            IViewManager.Instance.CurrentForm.IssueFormCommand(commandMessage);
                        }
                        else
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Clear the current test
                                ITests.Instance.CurrentTest.Clear();
                            }

                            // Eject the cartridge
                            if (EjectCartridge())
                            {
                                // Go back to the last menu
                                IViewManager.Instance.CurrentForm.Menu();
                            }
                        }
                    }
                    else
                    {
                        // Go back to the last menu
                        IViewManager.Instance.CurrentForm.Menu();
                    }
                }
                // Check for the cartridge error screen
                else if (commandMessage.FormName == "CartridgeError")
                {
                    // Unload the cartridge but don't unclamp
                    if (UnloadCartridge(false))
                    {
                        // Invoke the back method on the current form
                        IViewManager.Instance.CurrentForm.Back();
                    }
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

                // Check for a running script
                if (incompleteTestSetup && (string.IsNullOrEmpty(runningScript) == false))
                {
                    // Push this message back onto the queue
                    IViewManager.Instance.CurrentForm.IssueFormCommand(commandMessage);
                }
                else
                {
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
                        if (incompleteTestSetup && EjectCartridge())
                        {
                            // We have aborted a test setup so unwind the stack and clear the current form
                            previousSession.CurrentForm.Unwind();
                            previousSession.CurrentForm = null;
                        }
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
                        // Show the invalid login message
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("InvalidLogin");
                    }
                }
                else if (commandMessage.FormName == "InstrumentName")
                {
                    // Get the instrument name
                    var instrumentName = (string)commandMessage.Parameters["InstrumentName"];

                    // Set the configuration
                    IConfiguration.Instance.InstrumentName = instrumentName;
                    IConfiguration.Instance.Modified = true;

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyConfiguration();

                    // Return to the menu
                    IViewManager.Instance.CurrentForm.Back();
                }
                else if (commandMessage.FormName == "Language")
                {
                    // Get the language
                    var language = (string)commandMessage.Parameters["Language"];

                    // Set the locale in the configuration
                    IConfiguration.Instance.Locale = language;
                    IConfiguration.Instance.Modified = true;

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyConfiguration();

                    // Update the current culture and return to the menu
                    IViewManager.Instance.UpdateCurrentCulture(language);
                    IViewManager.Instance.CurrentForm.Back();
                }
                else if (commandMessage.FormName == "QcTestSetup")
                {
                    // Get the parameters
                    var testFrequency = (QcTestFrequency)commandMessage.Parameters["QcTestFrequency"];
                    var quarantineState = (QuarantineState)commandMessage.Parameters["QuarantineState"];

                    // Set the locale in the configuration
                    IConfiguration.Instance.QcTestFrequency = testFrequency;
                    IConfiguration.Instance.QuarantineState = quarantineState;
                    IConfiguration.Instance.Modified = true;

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyConfiguration();

                    // Return to the menu
                    IViewManager.Instance.CurrentForm.Back();
                }
                else if (commandMessage.FormName == "DateAndTime")
                {
                    // Get the new date and time
                    var value = ((DateTime)commandMessage.Parameters["Value"]).ToLocalTime();

                    // Log this event to the audit log
                    IAuditLog.Instance.Log("Date and time changing to " +
                        value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));

                    // Set the machine date and time
                    Microsoft.VisualBasic.DateAndTime.Today = value;
                    Microsoft.VisualBasic.DateAndTime.TimeOfDay = value;

                    // Return to the menu
                    IViewManager.Instance.CurrentForm.Back();
                }
                else if (commandMessage.FormName == "AssaySettings")
                {
                    // Set the configuration parameters
                    IConfiguration.Instance.Ung = (bool)commandMessage.Parameters["Ung"];
                    IConfiguration.Instance.AutoSampleId = (bool)commandMessage.Parameters["AutoSampleId"];
                    IConfiguration.Instance.Modified = true;

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyConfiguration();

                    // Return to the menu
                    IViewManager.Instance.CurrentForm.Back();
                }
                else if (commandMessage.FormName == "Timeout")
                {
                    // Get the timeout value
                    var value = (int)commandMessage.Parameters["Value"];

                    // Set the new value in the configuration
                    IConfiguration.Instance.AutoLogoffPeriodInSeconds = value;
                    IConfiguration.Instance.Modified = true;

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyConfiguration();

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

                            // Log this event to the audit log
                            IAuditLog.Instance.Log("User " + user.ID.ToString() + " (Name='" + user.Name +
                                "', Type='" + user.Type.ToString() + "') name/type modified");

                            // Try to get the password
                            object newPassword;

                            if (commandMessage.Parameters.TryGetValue("NewPassword", out newPassword))
                            {
                                // Set the new password
                                user.Password = (string)newPassword;

                                // Log this event to the audit log
                                IAuditLog.Instance.Log("User " + user.ID.ToString() + " (Name='" + user.Name +
                                    "', Type='" + user.Type.ToString() + "') password modified");
                            }
                        }
                        else
                        {
                            // This is a delete
                            IUsers.Instance.Remove(user);

                            // Log this event to the audit log
                            // Log this event to the audit log
                            IAuditLog.Instance.Log("User " + user.ID.ToString() + " (Name='" + user.Name +
                                "', Type='" + user.Type.ToString() + "') deleted");
                        }
                    }
                    else
                    {
                        // This is a new user
                        IUser user = IUsers.Instance.CreateNewUser(
                            (UserType)commandMessage.Parameters["NewUserType"],
                            (string)commandMessage.Parameters["NewUserName"],
                            (string)commandMessage.Parameters["NewPassword"]);

                        // Log this event to the audit log
                        IAuditLog.Instance.Log("User " + user.ID.ToString() + " (Name='" + user.Name +
                            "', Type='" + user.Type.ToString() + "') created");
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
                    IConfiguration.Instance.PasswordRules.ExpiryTimeInDays =
                        (int)commandMessage.Parameters["ExpiryTimeInDays"];
                    IConfiguration.Instance.PasswordRules.Override =
                        (bool)commandMessage.Parameters["Override"];
                    IConfiguration.Instance.Modified = true;

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyConfiguration();

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
                    IConfiguration.Instance.Modified = true;

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyConfiguration();

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
                        // Check for a running test
                        if (ITests.Instance.CurrentTest.Result.StartDateTime != DateTime.MinValue)
                        {
                            // Abort the test on the firmware
                            AbortCurrentTest();
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

                        // Initialise the number of displayed or recorded fields
                        var fields = (IConfiguration.Instance == null) ? 0 : 
                            IConfiguration.Instance.Fields.Where(
                            x => x.Policy != Model.Serializable.FieldPolicy.Ignore).Count();

                        // Check for a QC test or no fields
                        if (ITests.Instance.CurrentTest.Result.QcTest || (fields == 0))
                        {
                            // Initialise the patient information
                            ITests.Instance.CurrentTest.Result.PatientInformation =
                                new Dictionary<string, object>()
                                {
                                    { "PatientId", ITests.Instance.CurrentTest.Result.SampleId }
                                };

                            // Show the patient information form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("LoadSample");
                        }
                        else
                        {
                            // Show the patient information form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("PatientInformation");
                        }
                    }
                }
                else if (commandMessage.FormName == "PatientInformation")
                {
                    lock (ITests.Instance.CurrentTest)
                    {
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
                    // Wait for any scripts to complete
                    if (string.IsNullOrEmpty(runningScript) == false)
                    {
                        // Push this message back onto the queue
                        IViewManager.Instance.CurrentForm.IssueFormCommand(commandMessage);
                    }
                    // Check to see if we are waiting for the drawer to be closed
                    else if (phase == "Reset.Close")
                    {
                        // Show the close drawer form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("CloseDrawer");

                        // Update the status
                        SetPhase("Reset.Close.Prompt");
                    }
                    else if (phase == "Idle")
                    {
                        // Load the cartridge
                        LoadCartridge();
                    }
                    else
                    {
                        // Something bad has happened
                        ShowFault("Unexpected phase loading cartridge '" + phase + "'");
                    }
                }
                else if (commandMessage.FormName == "EmptyDrawer")
                {
                    // Show the fault
                    ShowFault("Potentiostat check failed");
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
                            string assayCode;
#if (DEBUG)
                            IState.Instance.ScannedBarcode = "94701734019000000000";
#endif
                            var assay = IAssays.Instance.GetAssayForBarcode(IState.Instance.ScannedBarcode,
                                out assayCode, out expiryDate);

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
                                IViewManager.Instance.CurrentForm.ShowFormFromMenu("IncorrectCartridge", false,
                                    new Dictionary<string, object>() { { "AssayCode", assayCode } });
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
                        // Set the QC test flag in the test result
                        ITests.Instance.CurrentTest.Result.QcTest =
                            (bool)commandMessage.Parameters["QcTest"];
                    }

                    // Start the test on the firmware
                    if (ClampCartridge())
                    {
                        // Show the running form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("TestRunning");
                    }
                }
                else if (commandMessage.FormName == "TestRunning")
                {
                    lock (ISessions.Instance.CurrentSession.CurrentTest)
                    {
                        // Check for a QC test
                        if (ISessions.Instance.CurrentSession.CurrentTest.Result.QcTest)
                        {
                            // Show the test result form
                            IViewManager.Instance.CurrentForm.ShowFormFromMenu("TestResult", true,
                                new Dictionary<string, object>() 
                                { 
                                    { "Test", ISessions.Instance.CurrentSession.CurrentTest } 
                                });
                        }
                        else
                        {
                            // Show the test complete form
                            IViewManager.Instance.CurrentForm.ShowFormFromMenu("TestComplete", true);
                        }
                    }
                }
                else if (commandMessage.FormName == "TestComplete")
                {
                    // Show the test result form
                    IViewManager.Instance.CurrentForm.ShowFormFromForm("TestResult",
                        new Dictionary<string, object>() 
                        { 
                            { "Test", ISessions.Instance.CurrentSession.CurrentTest }
                        });
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
                            // Check for a QC test
                            if (QcTestRequired())
                            {
                                // Show the QC test due form
                                IViewManager.Instance.CurrentForm.ShowFormFromMenu("QcTestDue", true,
                                    new Dictionary<string, object>() 
                                    { 
                                        { "User", ISessions.Instance.CurrentUser }
                                    });
                            }
                            else
                            {
                                // Set the locking user
                                ITests.Instance.CurrentTest.LockingUserId = ISessions.Instance.CurrentUser.ID;

                                // Show the sample ID form
                                IViewManager.Instance.CurrentForm.ShowFormFromMenu("SampleId", true);
                            }
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
                    // Get the scanned barcode
                    var assayCode = commandMessage.Parameters["AssayCode"];

                    // Show the add new assay form
                    IViewManager.Instance.CurrentForm.ShowFormFromForm("LocalUsbAssay",
                        new Dictionary<string, object>() { { "AssayCode", assayCode } });
                }
                else if (commandMessage.FormName == "DownloadAssay")
                {
                    try
                    {
                        // Download assays from the cloud
                        var assays = LoadNewAssays(IFtpFileSystem.Instance);

                        // Check for no assays
                        if ((assays == null) || (assays.Count == 0))
                        {
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("NoNewAssays");
                        }
                        else
                        {
                            // Show the select new assay form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("SelectNewAssay",
                                new Dictionary<string, object>() 
                                { 
                                    { "Assays", assays }, 
                                    { "FileSystem", FileSystem.IFtpFileSystem.Instance }
                                });
                        }
                    }
                    catch (ApplicationException)
                    {
                        // Return to the last menu
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("AssayUpdateFailed");
                    }
                }
                else if (commandMessage.FormName == "LocalUsbAssay")
                {
                    // Try to get the assay code
                    object assayCodeObject;
                    string assayCode = null;

                    if ((commandMessage.Parameters != null) &&
                        commandMessage.Parameters.TryGetValue("AssayCode", out assayCodeObject))
                    {
                        assayCode = (string)assayCodeObject;
                    }

                    // get the first removable drive with an assays file
                    var removableDrive = ILocalFileSystem.Instance.GetRemovableDrives().Where(
                        x => x.ReadTextFile("Assays") != null).FirstOrDefault();

                    try
                    {
                        // Download assays from USB
                        var assays = removableDrive == null ? new List<IAssay>() : LoadNewAssays(removableDrive);

                        // Filter by assay code if available
                        if (assayCode != null)
                        {
                            assays = assays.Where(x => x.Code == assayCode).ToList();
                        }

                        // Check for no assays
                        if ((assays == null) || (assays.Count == 0))
                        {
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("NoNewAssays");
                        }
                        else
                        {
                            // Show the select new assay form
                            IViewManager.Instance.CurrentForm.ShowFormFromForm("SelectNewAssay",
                                new Dictionary<string, object>() 
                                { 
                                    { "Assays", assays }, 
                                    { "FileSystem", removableDrive }, 
                                });
                        }
                    }
                    catch (ApplicationException)
                    {
                        // Return to the last menu
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("AssayUpdateFailed");
                    }
                }
                else if (commandMessage.FormName == "SelectNewAssay")
                {
                    // Show the update assay form
                    IViewManager.Instance.CurrentForm.ShowFormFromForm("UpdateAssay",
                        commandMessage.Parameters);
                }
                else if (commandMessage.FormName == "UpdateAssay")
                {
                    try
                    {
                        // Add the new assay to the list
                        InstallAssay((IAssay)commandMessage.Parameters["Assay"],
                            (ISimpleFileSystem)commandMessage.Parameters["FileSystem"]);

                        // Return to the last menu
                        IViewManager.Instance.CurrentForm.Menu();
                    }
                    catch (ApplicationException)
                    {
                        // Return to the last menu
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("AssayUpdateFailed");
                    }
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
                    IViewManager.Instance.CurrentForm.ShowFormFromForm("TestResult",
                        commandMessage.Parameters);
                }
                else if (commandMessage.FormName == "PasswordExpired")
                {
                    IViewManager.Instance.CurrentForm.ShowFormFromForm("UpdatePassword");
                }
                else if (commandMessage.FormName == "UpdatePassword")
                {
                    // Get the user
                    var user = ISessions.Instance.CurrentUser;

                    // Update the password and password date
                    user.Password = (string)commandMessage.Parameters["NewPassword"];
                    user.PasswordDate = DateTime.UtcNow;

                    // Log this event to the audit log
                    IAuditLog.Instance.Log("User " + user.ID.ToString() + " (Name='" + user.Name +
                        "', Type='" + user.Type.ToString() + "') password modified");

                    // Serialise the user data
                    SerialiseUserData();

                    // Clear the current user
                    ISessions.Instance.CurrentUser = null;
                    IViewManager.Instance.UpdateCurrentUser(null);

                    // Show the start form
                    IViewManager.Instance.ReplaceRoot("Start").Unwind();
                }
                else if (commandMessage.FormName == "ReaderQuarantine")
                {
                    // Get the user from the form
                    var user = commandMessage.Parameters["User"] as IUser;

                    // Check for an administrator
                    if (user.Type == UserType.Administrator)
                    {
                        // Set the state to unlocked and reset the last good test date and time
                        IConfiguration.Instance.QuarantineState = QuarantineState.Unlocked;
                        IConfiguration.Instance.QcTestDateTime = DateTime.UtcNow;
                        IConfiguration.Instance.Modified = true;

                        // Notify any developer client
                        Comms.Network.IManager.Instance.NotifyConfiguration();

                        // Return to the main menu
                        IViewManager.Instance.CurrentForm.Back();
                    }
                    else
                    {
                        // Show the login form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("QuarantineLogin");
                    }
                }
                else if (commandMessage.FormName == "QuarantineLogin")
                {
                    // Get the user name and password hash
                    var name = (string)commandMessage.Parameters["UserName"];
                    var passwordHash = IUser.HashPassword((string)commandMessage.Parameters["Password"]);

                    // Search for a valid user
                    IUser validUser = IUsers.Instance.Where(x => (x.Name == name) &&
                        (x.PasswordHash == passwordHash)).FirstOrDefault();

                    // Check for an administrator
                    if ((validUser != null) && (validUser.Type == UserType.Administrator))
                    {
                        IViewManager.Instance.CurrentForm.ShowFormFromMenu("ReaderQuarantine", true,
                            new Dictionary<string, object>() { { "User", validUser } });
                    }
                    else
                    {
                        // Show the invalid login message
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("InvalidLogin");
                    }
                }
                else if (commandMessage.FormName == "QcTestDue")
                {
                    // Set the locking user
                    ITests.Instance.CurrentTest.LockingUserId = ISessions.Instance.CurrentUser.ID;

                    // Set the QC test flag
                    ITests.Instance.CurrentTest.Result.QcTest = true;

                    // Show the sample Id form
                    IViewManager.Instance.CurrentForm.ShowFormFromMenu("SampleId", true);
                }
                else if (commandMessage.FormName == "QcTestDismissal")
                {
                    // Show the QC test login form
                    IViewManager.Instance.CurrentForm.ShowFormFromForm("QcTestLogin");
                }
                else if (commandMessage.FormName == "QcTestLogin")
                {
                    // Get the user name and password hash
                    var name = (string)commandMessage.Parameters["UserName"];
                    var passwordHash = IUser.HashPassword((string)commandMessage.Parameters["Password"]);

                    // Search for a valid user
                    IUser validUser = IUsers.Instance.Where(x => (x.Name == name) &&
                        (x.PasswordHash == passwordHash)).FirstOrDefault();

                    // Check for an administrator
                    if ((validUser != null) && (validUser.Type == UserType.Administrator))
                    {
                        IViewManager.Instance.CurrentForm.ShowFormFromMenu("QcTestDue", true,
                            new Dictionary<string, object>() { { "User", validUser } });
                    }
                    else
                    {
                        // Show the invalid login message
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("InvalidLogin");
                    }
                }
            }
        }

        /// <summary>
        /// Handle a command message
        /// </summary>
        /// <param name="commandMessage">The message</param>
        private void HandleMenuItemMessage(MenuItemMessage menuItemMessage)
        {
            // Check for a run test menu item
            if (menuItemMessage.MenuItem == "RunTest")
            {
                if (IConfiguration.Instance.QuarantineState == QuarantineState.Locked)
                {
                    // Show the test running form
                    IViewManager.Instance.CurrentForm.ShowFormFromMenu("ReaderQuarantine", false,
                        new Dictionary<string, object>() { { "User", ISessions.Instance.CurrentUser } });
                }
                else
                {
                    lock (ITests.Instance.CurrentTest)
                    {
                        // Clear down the current test
                        ISessions.Instance.CurrentSession.CurrentTest = null;

                        // Check if the current test is locked
                        if (ITests.Instance.CurrentTest.LockingUserId == 0)
                        {
                            // Check for a QC test
                            if (QcTestRequired())
                            {
                                // Show the QC test due form
                                IViewManager.Instance.CurrentForm.ShowFormFromMenu("QcTestDue", false,
                                    new Dictionary<string, object>() 
                                    { 
                                        { "User", ISessions.Instance.CurrentUser }
                                    });
                            }
                            else
                            {
                                // Set the locking user
                                ITests.Instance.CurrentTest.LockingUserId = ISessions.Instance.CurrentUser.ID;

                                // Clear the QC test flag
                                ITests.Instance.CurrentTest.Result.QcTest = false;

                                // Show the sample ID form
                                IViewManager.Instance.CurrentForm.ShowFormFromMenu("SampleId");
                            }
                        }
                        else
                        {
                            // Show the test running form
                            IViewManager.Instance.CurrentForm.ShowFormFromMenu("TestCancel");
                        }
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

                // Show the new form from the form
                IViewManager.Instance.CurrentForm.ShowFormFromForm("PatientInformationEdit",
                    new Dictionary<string, object>() {  { "FieldValues", parientInformation } });
            }
            // Check for an export message
            else if (menuItemMessage.MenuItem == "Export")
            {
                // Get the first removable drive
                var fileSystem = ILocalFileSystem.Instance.GetRemovableDrives().FirstOrDefault();

                // Check for a valid drive
                if (fileSystem == null)
                {
                    // Prompt the user
                    IViewManager.Instance.CurrentForm.ShowMessage("NoRemovableStorage");
                }
                else
                {
                    // Show the export menu
                    IViewManager.Instance.CurrentForm.ShowFormFromMenu("Export", false,
                        new Dictionary<string, object>() { { "FileSystem", fileSystem } });
                }
            }
            // Check for an export audit log message
            else if (menuItemMessage.MenuItem == "ExportAuditLog")
            {
                // Export the audit log
                IViewManager.Instance.ExportAuditLog(
                    menuItemMessage.Parameters["FileSystem"] as ISimpleFileSystem);
            }
            // Check for an export results message
            else if (menuItemMessage.MenuItem == "ExportResults")
            {
                // Export the results
                IViewManager.Instance.ExportResults(
                    menuItemMessage.Parameters["FileSystem"] as ISimpleFileSystem);
            }
            // Check for dismissing a QC test
            else if (menuItemMessage.MenuItem == "QcTestDismiss")
            {
                // Set the date and time of the test
                IConfiguration.Instance.QcTestDateTime = DateTime.UtcNow;
                IConfiguration.Instance.Modified = true;

                // Notify any developer client
                Comms.Network.IManager.Instance.NotifyConfiguration();

                // Go back to the main menu
                IViewManager.Instance.CurrentForm.Home();
            }
            // Check for print hard copy
            else if (menuItemMessage.MenuItem == "PrintHardCopy")
            {
                // Get the test
                var test = menuItemMessage.Parameters["Test"] as ITest;

                // Print the result
                IViewManager.Instance.PrintResult(test.Result.SampleId, 
                    test.Result.PatientInformation, test);

                // Return to the menu
                IViewManager.Instance.CurrentForm.Menu();
            }
            // Check for eject
            else if (menuItemMessage.MenuItem == "Eject")
            {
                // Check for the idle phase and a cartridge
                if (phase.StartsWith("Idle") && 
                    (IState.Instance.ScannedBarcode != null))
                {
                    // Execute the unload script
                    if (ExecuteScript("Load") == false)
                    {
                        // Show the fault
                        ShowFault("Script sequence error running 'Load'");
                    }
                    else
                    {
                        // Set the phase
                        SetPhase("Reset.Close");

                        // Reset the current form
                        IViewManager.Instance.CurrentForm.Reset();
                    }
                }
                else
                {
                    // Show a message
                    IViewManager.Instance.CurrentForm.ShowMessage("CannotEject");
                }
            }
            // This is a menu with a menu item, which is the name of the next form
            else
            {
                // Show the new form from the menu
                IViewManager.Instance.CurrentForm.ShowFormFromMenu(menuItemMessage.MenuItem, false,
                    menuItemMessage.Parameters);
            }
        }
    }
}
