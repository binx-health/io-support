/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IO.View;
using IO.Scripting;
using IO.FileSystem;
using IO.Model.Serializable;
using IO.Model.Volatile;
using IO.Analysis;

namespace IO.Controller
{
    /// <summary>
    /// The controller object
    /// </summary>
    public partial class Controller : IController, IDisposable
    {
        /// <summary>
        /// The name for a command script
        /// </summary>
        public static readonly string COMMAND_SCRIPT_NAME = "?";

        /// <summary>
        /// The scheduled idle period in milliseconds
        /// </summary>
        private static readonly int SCHEDULED_IDLE_PERIOD = 60000;

        /// <summary>
        /// Xml document for loading messages
        /// </summary>
        private XmlDocument xmlDocument = new XmlDocument() { XmlResolver = null };

        /// <summary>
        /// Xml namespace manager for loading messages
        /// </summary>
        private XmlNamespaceManager xmlNamespaceManager = null;

        /// <summary>
        /// The controller thread
        /// </summary>
        private Thread controllerThread = null;

        /// <summary>
        /// The server thread
        /// </summary>
        private Thread serverThread = null;

        /// <summary>
        /// The print thread
        /// </summary>
        private Thread printThread = null;

        /// <summary>
        /// The current running script
        /// </summary>
        private string runningScript = "";

        /// <summary>
        /// Flag to indicate the develper is running the current script
        /// </summary>
        private bool developerOverride = false;

        /// <summary>
        /// The phase
        /// </summary>
        private string phase = string.Empty;

        /// <summary>
        /// The current analysis object
        /// </summary>
        private IAnalysis analysis = null;

        /// <summary>
        /// The current peaks
        /// </summary>
        private List<PeakData> peaks = null;

        /// <summary>
        /// The analysis results by token
        /// </summary>
        private Dictionary<int, AnalysisResult> analysisResults = null;

        /// <summary>
        /// Cache of log lines from the firmware
        /// </summary>
        private StringBuilder logLinesCache = new StringBuilder();

        /// <summary>
        /// Cache of values from the firmware
        /// </summary>
        private IDeviceValues valuesCache = null;

        /// <summary>
        /// The client root certificate that contains the public key for digital signatures
        /// </summary>
        private RSACryptoServiceProvider rsaCryptoServiceProvider = null;

        /// <summary>
        /// The total stored results
        /// </summary>
        private uint totalResults = 0;

        /// <summary>
        /// Value notify event
        /// </summary>
        private AutoResetEvent valueNotifyEvent = new AutoResetEvent(false);

        /// <summary>
        /// The total stored results
        /// </summary>
        public override uint TotalResults
        {
            get
            {
                return totalResults;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Controller()
        {
            // Create a new namespace manager
            xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);

            // Add the namespaces
            xmlNamespaceManager.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            xmlNamespaceManager.AddNamespace("m", "http://www.AtlasGenetics.com/IO");
        }

        /// <summary>
        /// Initialise the controller
        /// </summary>
        public override void Initialise()
        {
            // Abort the server thread
            if (serverThread != null)
            {
                serverThread.Abort();
            }

            // Abort the print thread
            if (printThread != null)
            {
                printThread.Abort();
            }

            // Abort the controller thread
            if (controllerThread != null)
            {
                controllerThread.Abort();
            }

            // Load the client root certificate
            rsaCryptoServiceProvider = new X509Certificate2("IoClientRoot.pfx", "App1ause").PrivateKey as
                RSACryptoServiceProvider;

            // Create a cache based on the device values
            valuesCache = (IDeviceValues)IDeviceValues.Instance.Clone();

            // Create a new controller thread
            controllerThread = new Thread(ControllerThreadProcedure);

            // Start the controller thread
            controllerThread.Start();
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            // Abort the server thread
            if (serverThread != null)
            {
                serverThread.Abort();
                serverThread = null;
            }

            // Abort the print+ thread
            if (printThread != null)
            {
                printThread.Abort();
                printThread = null;
            }

            // Abort the controller thread
            if (controllerThread != null)
            {
                controllerThread.Abort();
                controllerThread = null;
            }
        }

        /// <summary>
        /// Controller thread procedure
        /// </summary>
        private void ControllerThreadProcedure()
        {
            "Waiting for view manager".Log();

            // Wait for the view manager to start
            while ((IViewManager.Instance == null) || (IViewManager.Instance.IsRunning == false))
            {
                Thread.Sleep(1);
            }

            "Creating splash screen".Log();

            // Create the splash screen
            IViewManager.Instance.ReplaceRoot("Splash");

            "Creating value notify timer".Log();

            // Initialise the list of wait handles
            var waitHandles = new WaitHandle[]
            { 
                valueNotifyEvent,
                Comms.Firmware.MessageQueue.Instance,
                Comms.Network.MessageQueue.Instance,
                View.MessageQueue.Instance,
            };

            // Create and start a stopwatch for periodic serialisation
            var periodicSerialisationStopwatch = new Stopwatch();

            periodicSerialisationStopwatch.Start();

            // Run until aborted
            while (true)
            {
                try
                {
                    // Check for a periodic serialisation event
                    if (periodicSerialisationStopwatch.ElapsedMilliseconds > SCHEDULED_IDLE_PERIOD)
                    {
                        // Check for a modified configuration                        
                        if (IConfiguration.Instance.Modified)
                        {
                            SerialiseConfiguration();
                        }

                        // Reset the stopwatch
                        periodicSerialisationStopwatch.Reset();
                        periodicSerialisationStopwatch.Start();
                    }

                    // Wait for a message
                    int handleIndex = WaitHandle.WaitAny(waitHandles, SCHEDULED_IDLE_PERIOD);

                    if (handleIndex == 0)
                    {
                        // Flush the current values cache
                        try
                        {
                            // Create a string builder
                            var stringBuilder = new StringBuilder();

                            // Create an XML writer to serialise the object as a fragment
                            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
                            {
                                OmitXmlDeclaration = true,
                                Indent = false,
                            }))
                            {
                                // Write the document element
                                xmlWriter.WriteStartElement("DeviceValues");

                                // Serialise the object
                                valuesCache.WriteXml(xmlWriter);
                            }

                            // Notify the device values
                            Comms.Network.IManager.Instance.NotifyDeviceValues(
                                stringBuilder.ToString());
                        }
                        finally
                        {
                            // Update the model device values from the cache
                            IDeviceValues.Instance.Update(valuesCache);

                            // Clear the cache
                            valuesCache.Clear();
                        }

                        // Flush the log
                        FlushLog();
                    }

                    if (handleIndex != WaitHandle.WaitTimeout)
                    {
                        // Flush the firmware message queue
                        string message;

                        while ((message = Comms.Firmware.MessageQueue.Instance.Pop()) != null)
                        {
                            // Handle the firmware message
                            HandleFirmwareMessage(message);
                        }

                        // Flush the network message queue
                        while ((message = Comms.Network.MessageQueue.Instance.Pop()) != null)
                        {
                            try
                            {
                                // Handle the network message
                                HandleNetworkMessage(message);
                            }
                            catch (Exception e)
                            {
                                // Report the error
                                Comms.Network.IManager.Instance.RespondError("m:Server", e.Message);

                                throw e;
                            }
                        }

                        // Check for view messages
                        if (View.MessageQueue.Instance.Length > 0)
                        {
                            // Reset the periodic serialisation stopwatch
                            periodicSerialisationStopwatch.Reset();
                            periodicSerialisationStopwatch.Start();

                            // Flush the view message queue
                            Message viewMessage;

                            while ((viewMessage = View.MessageQueue.Instance.Pop()) != null)
                            {
                                // Handle the view message
                                HandleViewMessage(viewMessage);
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // Occurs when reader shuts down
                }
                catch (Exception e)
                {
                    // Show the fault
                    ShowFault(e.Message);
                }
            }
        }

        /// <summary>
        /// Flush the log
        /// </summary>
        private void FlushLog()
        {
            // Check for log in the cache
            if (logLinesCache.Length > 0)
            {
                // If we are in the testing phase then append the log lines
                if (phase.StartsWith("Testing"))
                {
                    ITests.Instance.CurrentTest.Data.Log += logLinesCache.ToString();
                }

                // Notify the log
                Comms.Network.IManager.Instance.NotifyLogLines(logLinesCache.ToString());

                // Create a new cache
                logLinesCache = new StringBuilder();
            }
        }

        /// <summary>
        /// Determine if a QC test is required
        /// </summary>
        /// <returns></returns>
        private bool QcTestRequired()
        {
            // Check for the unlocked state
            if (IConfiguration.Instance.QuarantineState == QuarantineState.Unlocked)
            {
                // Evaluate the time of the next QC test in local time
                DateTime timeOfLastQcTest = IConfiguration.Instance.QcTestDateTime.ToLocalTime();
                DateTime timeOfNextQcTest;

                if (IConfiguration.Instance.QcTestFrequency == QcTestFrequency.Monthly)
                {
                    // Add a month to this month
                    timeOfNextQcTest = new DateTime(
                        timeOfLastQcTest.Year, timeOfLastQcTest.Month, 1).AddMonths(1);
                }
                else if (IConfiguration.Instance.QcTestFrequency == QcTestFrequency.Weekly)
                {
                    // Calculate the diff to Sunday
                    int diff = DayOfWeek.Sunday - timeOfLastQcTest.DayOfWeek;
                    
                    if (diff <= 0)
                    {
                        diff += 7;
                    }

                    // Add this to the last QC test date
                    timeOfNextQcTest = timeOfLastQcTest.AddDays(diff).Date;
                }
                else
                {
                    // Add a day to today's date
                    timeOfNextQcTest = timeOfLastQcTest.Date.AddDays(1);
                }

                // Check to see if this time has expired
                return timeOfNextQcTest < DateTime.Now;
            }

            return false;
        }
                

        /// <summary>
        /// Set the phase
        /// </summary>
        /// <param name="value">The phase value</param>
        private void SetPhase(string value)
        {
            // Flush the log
            FlushLog();

            // Log the phase change
            logLinesCache.AppendLine("Phase changed to '" + value + "'");

            // Set the phase
            phase = value;

            // Notify any developer client
            Comms.Network.IManager.Instance.NotifyPhaseChange(phase);
        }

        /// <summary>
        /// Called when analysis is complete on a random thread
        /// </summary>
        /// <param name="token">The token for the result</param>
        /// <param name="analysisResult">The result of the analysis</param>
        private void AnalysisComplete(int token, AnalysisResult analysisResult)
        {
            // Check for a valid results object
            if (analysisResults == null)
            {
                if (string.IsNullOrEmpty(LastError.Message))
                {
                    // Set the last error message
                    LastError.Message = "Unexpected analysis results";

                    // Show the fault screen
                    IViewManager.Instance.ReplaceRoot("Fault").Unwind();
                }

                return;
            }

            // Lock the results object as we are not on the controller thread
            lock (analysisResults)
            {
                // Add the analysis result 
                analysisResults[token] = analysisResult;

                // Check for a complete set of results
                if (analysisResults.ContainsValue(null) == false)
                {
                    // Post a message to the firmware queue to indicate completion
                    Comms.Firmware.MessageQueue.Instance.Push("*F=Analysis");
                }
            }
        }

        /// <summary>
        /// Called when analysis is complete on the controller thread
        /// </summary>
        private void AnalysisComplete()
        {
            // Initialise a disctionary of outcomes for peaks
            var outcomeForPeaks = new Dictionary<string, PeakOutcome>();

            // Initialise the rescan flag
            bool rescan = false;

            // Initialise the control outcome
            PeakOutcome controlOutcome = PeakOutcome.Positive;

            // Loop round the analysis results appending the logs
            foreach (var analysisResult in analysisResults.Values)
            {
                // Append the result of the analysis to the log
                logLinesCache.Append(analysisResult.Log);
            }

            // Loop round the analysis results calculating the peak outcomes
            foreach (var analysisResult in analysisResults.Values)
            {
                // Loop through the peaks
                for (int index = 0; index < analysisResult.Data.Peaks.Length; ++index)
                {
                    // Get the peak outcome
                    PeakOutcome peakOutcome = ((analysisResult.PeakOutcomes != null) &&
                        (analysisResult.PeakOutcomes.Length > index)) ? analysisResult.PeakOutcomes[index] : 
                        PeakOutcome.Invalid;

                    // Get the peak name
                    string peakName = analysisResult.Data.Peaks[index].Name;

                    logLinesCache.AppendLine("Peak outcome for '" + peakName + "' is " + 
                        peakOutcome.ToString());

                    // Check for an ignore peak
                    if (peakOutcome == PeakOutcome.Ignore)
                    {
                        continue;
                    }
                    
                    // Check for a rescan peak
                    if (peakOutcome == PeakOutcome.Rescan)
                    {
                        rescan = true;
                        continue;
                    }

                    // Check for the conrol peak
                    if (peakName == "Control")
                    {
                        // If all control peaks are not positive then they are invalid
                        if ((controlOutcome == PeakOutcome.Positive) &&
                            (peakOutcome != PeakOutcome.Positive))
                        {
                            controlOutcome = PeakOutcome.Invalid;
                        }
                    }
                    else
                    {
                        // Try to get the existing peak outcome
                        PeakOutcome existingPeakOutcome;

                        if (outcomeForPeaks.TryGetValue(peakName, out existingPeakOutcome))
                        {
                            // Check for a positive outcome
                            if (peakOutcome == PeakOutcome.Positive)
                            {
                                // Any positive outcome is recorded
                                outcomeForPeaks[peakName] = peakOutcome;
                            }
                            // Check for a negative outcome
                            else if (peakOutcome == PeakOutcome.Negative)
                            {
                                // Check for an exisiting Positive outcome
                                if (existingPeakOutcome != PeakOutcome.Positive)
                                {
                                    // A negative is recorded is no positive was found
                                    outcomeForPeaks[peakName] = peakOutcome;
                                }
                            }
                        }
                        else
                        {
                            // Set the peak outcome
                            outcomeForPeaks[peakName] = peakOutcome;
                        }
                    }
                }
            }

            // Check the rescan flag
            if (rescan)
            {
                if (phase == "Testing.Main")
                {
                    // Set the phase
                    SetPhase("Testing.Rescan");

                    // Execute the voltammerty script
                    if (ExecuteScript(ITests.Instance.CurrentTest.Result.Assay.VoltammetryScript) == false)
                    {
                        // Show the fault
                        ShowFault("Script sequence error running '" +
                            ITests.Instance.CurrentTest.Result.Assay.VoltammetryScript + "'");
                    }
                }
                else
                {
                    logLinesCache.AppendLine("Rescan flagged in phase '" + phase + "'");

                    // Terminate the current test
                    TerminateCurrentTest(TestOutcome.Error);
                }
            }
            else
            {
                logLinesCache.AppendLine("Control outcome is " + controlOutcome.ToString());

                // Compile a list of positive peaks
                string positivePeaks = null;

                foreach (var peakOutcome in outcomeForPeaks.Where(x => x.Value == PeakOutcome.Positive))
                {
                    positivePeaks += string.IsNullOrEmpty(positivePeaks) ? peakOutcome.Key :
                        ("," + peakOutcome.Key);
                }

                logLinesCache.AppendLine("Positive peaks: " + positivePeaks);

                // Compile a list of negative peaks
                string negativePeaks = null;

                foreach (var peakOutcome in outcomeForPeaks.Where(x => x.Value == PeakOutcome.Negative))
                {
                    negativePeaks += string.IsNullOrEmpty(negativePeaks) ? peakOutcome.Key :
                        ("," + peakOutcome.Key);
                }

                // Compile a list of invalid peaks
                string invalidPeaks = null;

                // Check for an invalid control
                if (controlOutcome != PeakOutcome.Positive)
                {
                    // All negative peaks are invalid in this case
                    invalidPeaks = negativePeaks;
                    negativePeaks = null;
                }

                logLinesCache.AppendLine("Negative peaks: " + negativePeaks);

                foreach (var peakOutcome in outcomeForPeaks.Where(x => x.Value == PeakOutcome.Invalid))
                {
                    invalidPeaks += string.IsNullOrEmpty(invalidPeaks) ? peakOutcome.Key :
                        ("," + peakOutcome.Key);
                }

                logLinesCache.AppendLine("Invalid peaks: " + invalidPeaks);

                // Terminate the current test
                TerminateCurrentTest(TestOutcome.Valid, positivePeaks, negativePeaks, invalidPeaks);

                // Dereference the results
                analysisResults = null;
            }
        }

        /// <summary>
        /// Terminate the current test if it is running
        /// </summary>
        /// <param name="outcome">The test outcome</param>
        /// <param name="positivePeaks">The comma separated list of positive peak names</param>
        /// <param name="negativePeaks">The comma separated list of negative peak names</param>
        /// <param name="invalidPeaks">The comma separated list of invalid peak names</param>
        private void TerminateCurrentTest(TestOutcome outcome,
            string positivePeaks = null, string negativePeaks = null, string invalidPeaks = null)
        {
            // Execute the unclamp script
            UnclampCartridge();

            lock (ITests.Instance.CurrentTest)
            {
                // Check that the current test is running
                if (ITests.Instance.CurrentTest.Result.StartDateTime != DateTime.MinValue)
                {
                    // Set the end date for the current test
                    ITests.Instance.CurrentTest.Result.EndDateTime = DateTime.UtcNow;

                    // Set the outcome and peaks for the current test
                    ITests.Instance.CurrentTest.Result.Outcome = outcome;
                    ITests.Instance.CurrentTest.Result.PositivePeaks = positivePeaks;
                    ITests.Instance.CurrentTest.Result.NegativePeaks = negativePeaks;
                    ITests.Instance.CurrentTest.Result.InvalidPeaks = invalidPeaks;

                    // Check for a valid QC test
                    if (ITests.Instance.CurrentTest.Result.QcTest && (outcome == TestOutcome.Valid))
                    {
                        // Check for quarantine tests
                        if (IConfiguration.Instance.QuarantineState != QuarantineState.DoNotQuarantine)
                        {
                            // Check for any invalid peaks
                            if (string.IsNullOrEmpty(invalidPeaks) == false)
                            {
                                // Test must be redone
                            }
                            // Else the QC test was valid
                            else
                            {
                                // Set the test time
                                IConfiguration.Instance.QcTestDateTime = 
                                    ITests.Instance.CurrentTest.Result.StartDateTime;
                                IConfiguration.Instance.Modified = true;

                                // Notify any developer client
                                Comms.Network.IManager.Instance.NotifyConfiguration();

                                // Check for the existence of positive peaks
                                if (string.IsNullOrEmpty(positivePeaks) == false)
                                {
                                    // Set the locked state
                                    IConfiguration.Instance.QuarantineState = QuarantineState.Locked;
                                }
                            }
                        }
                    }

                    // Set the users current test to this one
                    ISessions.Instance[ITests.Instance.CurrentTest.LockingUserId].CurrentTest =
                        ITests.Instance.CurrentTest;

                    // Add the result to the results file
                    IResults.Instance.AddResult(ITests.Instance.CurrentTest);

                    // Increment the total results
                    totalResults++;

                    // Message the server controller
                    Comms.Server.MessageQueue.Instance.Push(ITests.Instance.CurrentTest.Result.SampleId);

                    // Create a new current test
                    ITests.Instance.CreateNewTest();

                    // Serialise the tests data
                    SerialiseTestData();

                    // Notify any developer client
                    Comms.Network.IManager.Instance.NotifyTestData();
                }
            }
        }

        /// <summary>
        /// Serialise the configuration
        /// </summary>
        private void SerialiseConfiguration()
        {
            // Write the tests data to disk
            ILocalFileSystem.Instance.WriteTextFile("Configuration", IConfiguration.Instance.Serialise(
                IConfiguration.Instance.Types));

            // Clear the modified flag
            IConfiguration.Instance.Modified = false;
        }

        /// <summary>
        /// Serialise the test data
        /// </summary>
        private void SerialiseTestData()
        {
            // Write the tests data to disk
            ILocalFileSystem.Instance.WriteTextFile("Tests", ITests.Instance.Serialise(
                ITests.Instance.Types));
        }

        /// <summary>
        /// Serialise the user data
        /// </summary>
        private void SerialiseUserData()
        {
            // Write the users data to disk
            ILocalFileSystem.Instance.WriteTextFile("Users", IUsers.Instance.Serialise(
                IUsers.Instance.Types));
        }

        /// <summary>
        /// Serialise any assay data
        /// </summary>
        private void SerialiseAssayData()
        {
            // Check for modified assays
            if (IAssays.Instance.Exists(x => x.Modified))
            {
                // Serialise the assays
                ILocalFileSystem.Instance.WriteTextFile("Assays", 
                    IAssays.Instance.Serialise(IAssays.Instance.Types));

                // Loop through the modified assays clearing the flag
                foreach (var assay in IAssays.Instance.Where(x => x.Modified).ToArray())
                {
                    assay.Modified = false;
                }
            }

            // Check for modified metrics
            if (IDefaultMetrics.Instance.Modified)
            {
                // Write the script to file
                ILocalFileSystem.Instance.WriteTextFile("Metrics\\DefaultMetrics", 
                    IDefaultMetrics.Instance.Value);

                // Clear the modified flag
                IDefaultMetrics.Instance.Modified = false;
            }

            // Loop through all modified metrics
            foreach (var metric in IAssays.Instance.Select(x => x.Metrics).Where(
                x => x.Modified).Distinct().ToArray())
            {
                // Write the script to file
                ILocalFileSystem.Instance.WriteTextFile("Metrics\\" + metric.Name, metric.Value);

                // Clear the modified flag
                metric.Modified = false;
            }

            // Loop through all modified scripts
            foreach (var script in IScripts.Instance.Where(x => x.Modified).ToArray())
            {
                // Don't serialise the command script
                if (script.Name != COMMAND_SCRIPT_NAME)
                {
                    // Write the script to file
                    ILocalFileSystem.Instance.WriteTextFile("Scripts\\" + script.Name, script.Value);
                }

                // Clear the modified flag
                script.Modified = false;
            }
        }

        /// <summary>
        /// Shutdown the system
        /// </summary>
        private void ShutdownSystem(bool restart = false)
        {
            // Serialise the configuration if modified
            if (IConfiguration.Instance.Modified)
            {
                SerialiseConfiguration();
            }

            // Serialise any assay data
            SerialiseAssayData();

            try
            {
                // Commence the instrument shutdown sequence
                Comms.Firmware.IManager.Instance.Shutdown();
            }
            catch (Exception)
            {
                // If this fails then shutdown anyway
            }

#if (DEBUG)
            // Close the main window
            IViewManager.Instance.Stop();
#else
            // Initiate the shutdown sequence
            System.Diagnostics.Process.Start("shutdown", restart ? "/r /t 0" : "/s /t 0");
#endif
        }

        /// <summary>
        /// Clamp the cartridge
        /// </summary>
        /// <returns>True if the script can be executed, false otherwise</returns>
        private bool ClampCartridge()
        {
            // Execute the unload script
            if (ExecuteScript("Clamp") == false)
            {
                // Show the fault
                ShowFault("Script sequence error running 'Clamp'");

                return false;
            }

            // Set the phase
            SetPhase("Testing.Clamping");

            return true;
        }

        /// <summary>
        /// Start the current test on the firmware
        /// </summary>
        /// <returns>True on success, false otherwise</returns>
        private bool StartCurrentTest()
        {
            lock (ITests.Instance.CurrentTest)
            {
                // Determine which script to run
                var script = IConfiguration.Instance.Ung ?
                    ITests.Instance.CurrentTest.Result.Assay.UngScript :
                    ITests.Instance.CurrentTest.Result.Assay.Script;

                // Set the test state to started
                ITests.Instance.CurrentTest.Result.StartDateTime = DateTime.UtcNow;
                ITests.Instance.CurrentTest.Result.CartridgeData = IState.Instance.ScannedBarcode;

                // Get and log the ambient temperature
                int ambientTemperature;

                if (IDeviceValues.Instance.TryGetValue("therm4", out ambientTemperature))
                {
                    logLinesCache.AppendLine("Ambient temperature " + ambientTemperature / 10.0 + "°C");
                }

                // Get and log the ambient pressure
                int ambientPressure;

                if (IDeviceValues.Instance.TryGetValue("ps5", out ambientPressure))
                {
                    logLinesCache.AppendLine("Ambient pressure " + ambientPressure + "mb");
                }

                // Execute the test script
                if (ExecuteScript(script) == false)
                {
                    // Show the fault
                    ShowFault("Script sequence error running '" + script + "'");

                    return false;
                }
            }
            
            // Set the phase
            SetPhase("Testing.Main");

            return true;
        }

        /// <summary>
        /// Unclamp the cartridge
        /// </summary>
        /// <returns>True if the script can be executed, false otherwise</returns>
        private bool UnclampCartridge()
        {
            // Execute the unload script
            if (ExecuteScript("Unclamp") == false)
            {
                // Show the fault
                ShowFault("Script sequence error running 'Unclamp'");

                return false;
            }

            // Set the phase
            SetPhase("Testing.Unclamping");

            return true;
        }

        /// <summary>
        /// Reset the clamp
        /// </summary>
        /// <returns>True if the script can be executed, false otherwise</returns>
        private bool ResetClamp()
        {
            // Execute the unload script
            if (ExecuteScript("Reset") == false)
            {
                // Show the fault
                ShowFault("Script sequence error running 'Reset'");

                return false;
            }

            // Set the phase
            SetPhase("Reset.Datum");

            return true;
        }

        /// <summary>
        /// Load the cartridge
        /// </summary>
        /// <returns>True on success, false otherwise</returns>
        private bool LoadCartridge()
        {
            // Execute the unload script
            if (ExecuteScript("Load") == false)
            {
                // Show the fault
                ShowFault("Script sequence error running 'Load'");

                return false;
            }

            // Set the phase
            SetPhase("Loading.Open");

            return true;
        }

        /// <summary>
        /// Unload the cartridge
        /// </summary>
        /// <param name="reset">Whether to reset the clamp after unloading</param>
        /// <returns>True on success, false otherwise</returns>
        private bool UnloadCartridge(bool reset = true)
        {
            // Execute the unload script
            if (ExecuteScript("Unload") == false)
            {
                // Show the fault
                ShowFault("Script sequence error running 'Unload'");

                return false;
            }

            // Check for the reset flag
            if (reset)
            {
                // Set the phase
                SetPhase("Reset.Close");
            }

            return true;
        }

        /// <summary>
        /// Eject any loaded cartridge
        /// </summary>
        private bool EjectCartridge()
        {
            // Check for idle
            if (phase == "Idle")
            {
                return true;
            }
            // Check for a loaded cartridge
            else if (string.IsNullOrEmpty(IState.Instance.ScannedBarcode) == false)
            {
                // Unload the cartridge and reset the clamp
                return UnloadCartridge();
            }
            // Check for a closed drawer
            else if (IState.Instance.ScannedBarcode == "")
            {
                // Reset the clamp to the datum
                return ResetClamp();
            }
            // The drawer is open
            else
            {
                // Set the phase to reset closing
                SetPhase("Reset.Close");

                return true;
            }
        }

        /// <summary>
        /// Check for an incomplete test setup
        /// </summary>
        /// <returns>True for an incomplete test setup, false otherwise</returns>
        private bool IncompleteTestSetup()
        {
            lock (ITests.Instance.CurrentTest)
            {
                return (ISessions.Instance.CurrentUser != null) &&
                    (ITests.Instance.CurrentTest.LockingUserId == ISessions.Instance.CurrentUser.ID) &&
                    (ITests.Instance.CurrentTest.Result.StartDateTime == DateTime.MinValue);
            }
        }

        /// <summary>
        /// Abort any test that is currently running on the firmware
        /// </summary>
        private void AbortCurrentTest()
        {
            // Abort the currently running script
            AbortScript();

            // Terminate the current test
            TerminateCurrentTest(TestOutcome.UserAborted);
        }

        /// <summary>
        /// Abort any scripts, save any data and show the fault screen
        /// </summary>
        /// <param name="message">The error message</param>
        private void ShowFault(string message)
        {
            // Check for a change to a valid message
            if (string.IsNullOrEmpty(LastError.Message) && (string.IsNullOrEmpty(message) == false))
            {
                // Log the activity
                logLinesCache.AppendLine("Fault occurred: " + message);

                if (string.IsNullOrEmpty(runningScript) == false)
                {
                    try
                    {
                        // Reset the firmware
                        AbortScript();
                    }
                    catch (Exception)
                    {
                        // Firmware may have crashed
                    }
                }

                try
                {
                    // Execute the alarm
                    ExecuteScript("Error");
                }
                catch (Exception)
                {
                    // Firmware may have crashed
                }

                try
                {
                    // Set the last error message
                    LastError.Message = message;

                    // Show the fault screen
                    IViewManager.Instance.ReplaceRoot("Fault").Unwind();
                }
                catch (Exception)
                {
                    // We cannot do much here if this fails
                }
            }
        }

        /// <summary>
        /// Execute a script on the firmware
        /// </summary>
        /// <param name="name">The script name</param>
        /// <returns>True if the script is executed, false if another script is running</returns>
        private bool ExecuteScript(string name)
        {
            // Get the named script
            var script = IScripts.Instance.Where(
                x => string.Compare(x.Name, name, true) == 0).FirstOrDefault();

            // Check for a value
            if (script == null)
            {
                throw new ApplicationException("Invalid script name '" + name + "'");
            }

            // Check for another script running
            if (string.IsNullOrEmpty(runningScript) == false)
            {
                return false;
            }

            // Delete all scripts on the firmware
            Comms.Firmware.IManager.Instance.DeleteScripts();

            // Set the default metrics
            if (IDefaultMetrics.Instance.Loaded == false)
            {
                Comms.Firmware.IManager.Instance.SetDefaultMetrics(IDefaultMetrics.Instance.Value);
                IDefaultMetrics.Instance.Loaded = true;
            }

            // Set the test metrics if we are running a test
            if ((ITests.Instance.CurrentTest.Result.StartDateTime != DateTime.MinValue) &&
                (ITests.Instance.CurrentTest.Result.Assay != null) &&
                (ITests.Instance.CurrentTest.Result.Assay.Metrics != null))
            {
                Comms.Firmware.IManager.Instance.SetTestMetrics(ITests.Instance.CurrentTest.Result.Assay.Metrics.Value);
            }

            // Load the script and all of its children
            string error;

            if (LoadScript(script, out error) == false)
            {
                throw new ApplicationException(error);
            }

            // Clear down the warnings
            hashSetWarnings.Clear();

            // Execute the script on the firmware
            Comms.Firmware.IManager.Instance.ExecuteScript(name);

            // Set the running script
            runningScript = name;

            // Log the script execution
            logLinesCache.AppendLine("Script '" + name + "' executed");

            return true;
        }

        /// <summary>
        /// Load the script and all of its children
        /// </summary>
        /// <param name="script">The script to load</param>
        /// <param name="error">The error text</param>
        /// <returns>True if the script was loaded, false otherwise</returns>
        private bool LoadScript(IScript script, out string error)
        {
            // Check for errors
            if (script.Errors.Count() > 0)
            {
                // Only report the first error
                var scriptError = script.Errors.First();

                // set the error
                error = scriptError.Description + " in script " + script.Name + 
                    " at line " + (scriptError.Line + 1);

                // We should not load bad scripts onto the firmware
                return false;
            }

            // Load the script
            Comms.Firmware.IManager.Instance.SetScript(script.Name, script.Value);

            // Load all child scripts
            foreach (var scriptName in script.Children)
            {
                if (LoadScript(IScripts.Instance.Where(x => string.Compare(
                    x.Name, scriptName, true) == 0).FirstOrDefault(), out error) == false)
                {
                    return false;
                }
            }

            // Set the error to null
            error = null;

            return true;
        }

        /// <summary>
        /// Execute a command on the firmware
        /// </summary>
        /// <param name="command"></param>
        private void ExecuteCommand(string command)
        {
            // Get the named script
            var script = IScripts.Instance.SetScript(COMMAND_SCRIPT_NAME, command);

            try
            {
                // Delete all scripts on the firmware
                Comms.Firmware.IManager.Instance.DeleteScripts();

                // Set the default metrics
                if (IDefaultMetrics.Instance.Loaded == false)
                {
                    Comms.Firmware.IManager.Instance.SetDefaultMetrics(IDefaultMetrics.Instance.Value);
                    IDefaultMetrics.Instance.Loaded = true;
                }

                // Load the script and all of its children
                string error;

                if (LoadScript(script, out error) == false)
                {
                    throw new ApplicationException(error);
                }
                else
                {
                    // Clear down the warnings
                    hashSetWarnings.Clear();

                    // Execute the script on the firmware
                    Comms.Firmware.IManager.Instance.ExecuteScript(COMMAND_SCRIPT_NAME);

                    // Set the running script
                    runningScript = COMMAND_SCRIPT_NAME;
                }
            }
            finally
            {
                // Remove the command script
                IScripts.Instance.SetScript(COMMAND_SCRIPT_NAME, null);
            }
        }

        /// <summary>
        /// Abort the current script
        /// </summary>
        private void AbortScript()
        {
            // Log the activity
            logLinesCache.AppendLine("Aborting current script");

            // Abort the script on the firmware
            Comms.Firmware.IManager.Instance.AbortScript();

            // Clear the running script and developer override
            runningScript = null;
            developerOverride = false;
        }

        /// <summary>
        /// Load new assays from a file system
        /// </summary>
        /// <param name="fileSystem">The file system to load the assays from</param>
        /// <returns>A list of new assays</returns>
        private List<IAssay> LoadNewAssays(ISimpleFileSystem fileSystem)
        {
            // Create a string builder to store all of the data
            var stringBuilder = new StringBuilder();

            // Get the assays from the server and append to the string builder
            var assayXml = fileSystem.ReadTextFile("Assays");

            stringBuilder.Append(assayXml);

            // Load the assays object
            var assays = IAssays.Instance.Deserialise(assayXml, IAssays.Instance.Types);

            // Get the default metrics and append to the string builder
            stringBuilder.Append(fileSystem.ReadTextFile("Metrics\\DefaultMetrics"));

            // Create a hasher
            using (var hasher = SHA1Managed.Create())
            {
                // Hash the data
                byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                byte[] signature = Convert.FromBase64String(fileSystem.ReadTextFile("Signature"));

                // Check the signature using the client's public key
                if (rsaCryptoServiceProvider.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), 
                    signature) == false)
                {
                    throw new ApplicationException("Invalid signature for assays");
                }
            }

            // Return any new or upversioned assays
            return assays.Where(x => IAssays.Instance.Exists(y => (y.Name == x.Name) &&
                (y.Version == x.Version)) == false).ToList();
        }

        /// <summary>
        /// Install a new assay from a file system
        /// </summary>
        /// <param name="assay">The assay to install</param>
        /// <param name="fileSystem">The file system to load the assay from</param>
        private void InstallAssay(IAssay assay, ISimpleFileSystem fileSystem)
        {
            // Serialise the data
            SerialiseAssayData();

            // Remember the exsiting metrics
            var existingDefaultMetricsValue = IDefaultMetrics.Instance.Value;
            var existingAssayMetricsValue = assay.Metrics.Value;

            // Make a copy of the existing assays
            var existingAssays = IAssays.Instance.Deserialise(
                ILocalFileSystem.Instance.ReadTextFile("Assays"), IAssays.Instance.Types);

            try
            {
                // Load the default metrics
                var defaultMetricsValue = fileSystem.ReadTextFile("Metrics/DefaultMetrics");

                if (defaultMetricsValue != null)
                {
                    IDefaultMetrics.Instance.Value = defaultMetricsValue;
                    IDefaultMetrics.Instance.Modified = true;
                }

                // Create a string builder to store all of the data
                var stringBuilder = new StringBuilder();

                // Load the assay metrics
                var assayMetricsValue = fileSystem.ReadTextFile("Metrics/" + assay.MetricsName);

                if (assayMetricsValue == null)
                {
                    throw new ApplicationException("Missing assay metrics '" + assay.MetricsName + 
                        "' for assay '" + assay.Name + "'");
                }
                else
                {
                    // Append the metrics
                    stringBuilder.Append(assayMetricsValue);

                    // Set the value
                    assay.Metrics.Value = assayMetricsValue;
                    assay.Metrics.Modified = true;
                }

                // Initialise the loaded scripts
                var loadedScripts = new HashSet<string>();

                // Install the main UNG and voltammetry scripts for the assay
                InstallScript(assay.Script, fileSystem, stringBuilder, loadedScripts);
                InstallScript(assay.UngScript, fileSystem, stringBuilder, loadedScripts);
                InstallScript(assay.VoltammetryScript, fileSystem, stringBuilder, loadedScripts);

                // Load the supporting scripts
                foreach (var scriptName in IAssays.SupportingScripts)
                {
                    // Install the script
                    InstallScript(scriptName, fileSystem, stringBuilder, loadedScripts);
                }

                // Create a hasher
                using (var hasher = SHA1Managed.Create())
                {
                    // Hash the data
                    byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                    byte[] signature = Convert.FromBase64String(assay.Signature);

                    // Check the signature using the client's public key
                    if (rsaCryptoServiceProvider.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"),
                        signature) == false)
                    {
                        throw new ApplicationException("Invalid assay signature for assay '" + assay.Name + 
                            "'");
                    }
                }

                // Get an existing assay of the same name
                var existingAssay = IAssays.Instance.Where(x => x.Name == assay.Name).FirstOrDefault();

                // Remove any existing assay
                if (existingAssay != null)
                {
                    IAssays.Instance.Remove(existingAssay);
                }

                // Set the new assay to modified
                assay.Modified = true;

                // Remove the signature from the assay
                assay.Signature = null;

                // Add it to the list
                IAssays.Instance.Add(assay);

                // Serialise the data
                SerialiseAssayData();
            }
            catch (Exception e)
            {
                // Reload the scripts, assays and metrics from storage
                IScripts.Instance.Reload();
                IAssays.Instance = existingAssays;
                IDefaultMetrics.Instance.Value = existingDefaultMetricsValue;
                IDefaultMetrics.Instance.Modified = false;

                // Check for an existing metrics value
                if (existingAssayMetricsValue != null)
                {
                    assay.Metrics.Value = existingAssayMetricsValue;
                    assay.Metrics.Modified = false;
                }

                throw e;
            }
        }

        /// <summary>
        /// Install a script and all child scripts from a file system
        /// </summary>
        /// <param name="scriptName">The script name</param>
        /// <param name="fileSystem">The file system to load the scripts from</param>
        /// <param name="stringBuilder">The string builder to concatenate the scripts</param>
        /// <param name="loadedScripts">The set of loaded script names</param>
        private void InstallScript(string scriptName, ISimpleFileSystem fileSystem,
            StringBuilder stringBuilder, HashSet<string> loadedScripts)
        {
            // Check for loaded scripts
            if (loadedScripts.Contains(scriptName, StringComparer.CurrentCultureIgnoreCase) == false)
            {
                // Get the script value
                var scriptValue = fileSystem.ReadTextFile("Scripts/" + scriptName);

                // Check the script exists
                if (scriptValue == null)
                {
                    throw new ApplicationException("Missing script '" + scriptName + "' for assay");
                }
                else
                {
                    // Add this script to the loaded scripts
                    loadedScripts.Add(scriptName);

                    // Append the script value
                    stringBuilder.Append(scriptValue);

                    // Install the script reccursively
                    InstallChildScripts(IScripts.Instance.SetScript(scriptName,
                        scriptValue), fileSystem, stringBuilder, loadedScripts);
                }
            }
        }

        /// <summary>
        /// Install child scripts reccursively from a file system
        /// </summary>
        /// <param name="script">The parent script</param>
        /// <param name="fileSystem">The file system to load the scripts from</param>
        /// <param name="loadedScripts">The set of loaded script names</param>
        private void InstallChildScripts(IScript script, ISimpleFileSystem fileSystem,
            StringBuilder stringBuilder, HashSet<string> loadedScripts)
        {
            // Loop through the child scripts
            foreach (var scriptName in script.Children)
            {
                // Check for loaded scripts
                if (loadedScripts.Contains(scriptName, StringComparer.CurrentCultureIgnoreCase) == false)
                {
                    // Get the script value
                    var scriptValue = fileSystem.ReadTextFile("Scripts/" + scriptName);

                    // Check the script exists
                    if (scriptValue == null)
                    {
                        throw new ApplicationException("Missing script '" + scriptName + "' for assay");
                    }
                    else
                    {
                        // Add this script to the loaded scripts
                        loadedScripts.Add(scriptName);

                        // Append the script value
                        stringBuilder.Append(scriptValue);

                        // Install the script reccursively
                        InstallChildScripts(IScripts.Instance.SetScript(scriptName,
                            scriptValue), fileSystem, stringBuilder, loadedScripts);
                    }
                }
            }
        }
    }
}
