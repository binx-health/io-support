/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using IO.Comms;
using IO.Model.Serializable;
using IO.Scripting;
using IO.FileSystem;
using IO.Model.Volatile;
using IO.View;
using IO.Analysis;

namespace IO.Controller
{
    /// <summary>
    /// The controller object
    /// </summary>
    partial class Controller
    {
        /// <summary>
        /// Firmware throw message constants
        /// </summary>
        private static readonly int INFORMATION_BASE = 10000;
        private static readonly int INFORMATION_PHASE_CHANGE = INFORMATION_BASE;
        private static readonly int INFORMATION_DRAWER_CLOSED = INFORMATION_BASE + 1;
        private static readonly int INFORMATION_PERCENT_COMPLETE = INFORMATION_BASE + 2;
        private static readonly int INFORMATION_DRAWER_OPENING = INFORMATION_BASE + 3;
        private static readonly int WARNING_BASE = 20000;
        private static readonly int WARNING_METRIC_VIOLATED = WARNING_BASE;
        private static readonly int WARNING_DRAWER_CLOSED_NO_BARCODE = WARNING_BASE + 1;
        private static readonly int WARNING_DRAWER_NOT_OPENED = WARNING_BASE + 2;
        private static readonly int WARNING_POTENTIOSTAT_CHECK_FAILED = WARNING_BASE + 4;
        private static readonly int WARNING_AMBIENT_PRESSURE_CHECK_FAILED = WARNING_BASE + 7;
        private static readonly int WARNING_PRESSURE_SENSOR_CHECK_FAILED = WARNING_BASE + 8;
        private static readonly int ERROR_BASE = 30000;
        private static readonly int ERROR_METRIC_VIOLATED = ERROR_BASE;
        private static readonly int ERROR_SCRIPT_SYNTAX = ERROR_BASE + 1;
        private static readonly int ERROR_DRAWER_NOT_CLOSED = ERROR_BASE + 2;
        private static readonly int ERROR_ISOLATION_VALVE_RELEASED = ERROR_BASE + 3;

        /// <summary>
        /// Constant to convert from potentiostat ADC to nA
        /// (Adc_Value * 5000 * 1000000) / (32768 * 400000)
        /// </summary>
        private static readonly double POTENTIOSTAT_ADC_TO_NANO_AMPS = 0.3814697265625;

        /// <summary>
        /// Whether or not to record PCR temperatures
        /// </summary>
        private bool recordPcr = false;

        /// <summary>
        /// Hash set of warnings raised while running a script
        /// </summary>
        private HashSet<int> hashSetWarnings = new HashSet<int>();

        /// <summary>
        /// Handle a firmware message
        /// </summary>
        /// <param name="message">The message</param>
        void HandleFirmwareMessage(string message)
        {
            // Check for script completion
            if ((message.StartsWith("*F=")) && (message.Length > 3))
            {
                // Extract the script from the message
                var script = message.Substring(3);

                // Clear the running script
                runningScript = null;

                // Check for a developer override
                if (developerOverride)
                {
                    // Clear the override flag
                    developerOverride = false;

                    // Notify the developer client
                    Comms.Network.IManager.Instance.NotifyScriptComplete(script);
                }
                else if (HandleFinishMessage(script))
                {
                    // Log the event
                    logLinesCache.AppendLine("Script '" + script + "' finished");
                }
                else
                {
                    // Log the unexpected firmware message
                    logLinesCache.AppendLine("Unexpected script finish: " + script);
                }
            }
            // Check for a peak message
            else if ((message.StartsWith("*P=")) && (message.Length > 3))
            {
                // Extract the payload from the message
                var payload = message.Substring(3);

                // Tokenise the payload
                string error;
                var tokens = payload.Tokenise(out error);

                // Check for a valid command
                if ((tokens == null) || (HandlePeakMessage(tokens) == false))
                {
                    // Log the unexpected firmware message
                    logLinesCache.AppendLine("Unexpected firmware peak data: " + payload);
                }
                else
                {
                    logLinesCache.AppendLine("Peak data received: " + payload);
                }
            }
            // Check for a potentiostat message
            else if ((message.StartsWith("*U=")) && (message.Length > 3))
            {
                // Extract the payload from the message
                var payload = message.Substring(3);

                // Split the payload on commas and read the code from the first token
                var tokens = payload.Split(new char[] { ',' });

                if ((tokens.Length > 2) && ((tokens.Length - 2) % 4 == 0))
                {
                    // Calculate the number of values
                    int values = (tokens.Length - 2) / 4;

                    // Log the firmware potentiostat data
                    logLinesCache.AppendLine("Firmware potentiostat data: " + payload);

                    try
                    {
                        lock (ITests.Instance.CurrentTest)
                        {
                            // Initialise the current test data
                            ITests.Instance.CurrentTest.Data.StartPotential = int.Parse(tokens[0].Trim());
                            ITests.Instance.CurrentTest.Data.IncrementalPotential = 
                                int.Parse(tokens[1].Trim());

                            // Initialis the arrays fo values
                            for (int potentiostat = 0; potentiostat < 4; ++potentiostat)
                            {
                                ITests.Instance.CurrentTest.Data.CellValues[potentiostat] = new double[values];
                            }

                            // Read the cell values
                            for (int tokenIndex = 1, index = 0; index < values; ++index)
                            {
                                for (int potentiostat = 0; potentiostat < 4; ++potentiostat)
                                {
                                    ITests.Instance.CurrentTest.Data.CellValues[potentiostat][index] =
                                        int.Parse(tokens[++tokenIndex].Trim()) * POTENTIOSTAT_ADC_TO_NANO_AMPS;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Log the unexpected firmware potentiostat data
                        logLinesCache.AppendLine("Unexpected firmware potentiostat data: " + payload);
                    }
                }
                else
                {
                    // Log the unexpected firmware potentiostat data
                    logLinesCache.AppendLine("Unexpected firmware potentiostat data: " + payload);
                }
            }
            // Check for a log message
            else if ((message.StartsWith("*L=")) && (message.Length > 3))
            {
                // This is a log message so append it to the log cache
                logLinesCache.AppendLine(message.Substring(3));
            }
            // Check for a throw message
            else if ((message.StartsWith("*T=")) && (message.Length > 3))
            {
                // Extract the payload from the message
                var payload = message.Substring(3);

                // Split the payload on commas and read the code from the first token
                var tokens = payload.Split(new char[] { ',' });
                int code;

                // Append it to the log cache
                logLinesCache.AppendLine("Firmware throw: " + payload);

                if ((int.TryParse(tokens[0].Trim(), out code) == false) ||
                    (HandleFirmwareThrow(code, tokens) == false))
                {
                    // Log the unexpected firmware message
                    logLinesCache.AppendLine("Unexpected firmware throw: " + payload);
                }
            }
            // Check for a hardware message
            else if ((message.StartsWith("*H=")) && (message.Length > 3))
            {
                // Parse the comma separated parameters
                var tokens = message.Substring(3).Split(new char[] { ',' });
                int value;

                if ((tokens.Length == 2) && int.TryParse(tokens[1].Trim(), out value))
                {
                    // Get the device name
                    string name = tokens[0].Trim();

                    // Check for the PCR peltier target temperature
                    if (name == "therm2")
                    {
                        // Check for a change in state
                        if (recordPcr != (value != 0))
                        {
                            // Check for off
                            if (value == 0)
                            {
                                recordPcr = false;
                            }
                            else
                            {
                                // The peltier is on so check for a current test
                                lock (ITests.Instance.CurrentTest)
                                {
                                    // Check the test start time and get the reporting period for PCR data
                                    int fastReportingPeriod = 0;
                                    bool inherited;

                                    recordPcr = (ITests.Instance.CurrentTest.Result.StartDateTime != 
                                        DateTime.MinValue) &&
                                        (ITests.Instance.CurrentTest.Result.Assay != null) &&
                                        (ITests.Instance.CurrentTest.Result.Assay.Metrics != null) &&
                                        ITests.Instance.CurrentTest.Result.Assay.Metrics.TryGetIntegerMetric(
                                        "General.Log.FastReporting", out fastReportingPeriod, out inherited);

                                    // If we are recording then check for the initial values
                                    if (recordPcr && (ITests.Instance.CurrentTest.Data.PcrValues == null))
                                    {
                                        // Instantiate the array to store data and set the reporting period
                                        ITests.Instance.CurrentTest.Data.PcrValues = new List<double>();
                                        ITests.Instance.CurrentTest.Data.FastReportingPeriod =
                                            fastReportingPeriod;
                                    }
                                }
                            }
                        }
                    }
                    // Check for the PCR peltier top plate temperature
                    else if (name == "therm2.top")
                    {
                        // Check to see if we are recording
                        if (recordPcr)
                        {
                            lock (ITests.Instance.CurrentTest)
                            {
                                // Check for the end of the test
                                if (ITests.Instance.CurrentTest.Result.StartDateTime != DateTime.MinValue)
                                {
                                    // Record the temperature value
                                    ITests.Instance.CurrentTest.Data.PcrValues.Add((double)value / 10.0);
                                }
                                else
                                {
                                    // Turn off recording
                                    recordPcr = false;
                                }
                            }
                        }
                    }
                    // Check fof PCR peltier cycles
                    else if ((name == "therm2.cycles") && (value > 0))
                    {
                        logLinesCache.AppendLine(value + " PCR cycles run");

                        // Update the configuration
                        IConfiguration.Instance.PcrCycles += value;
                        IConfiguration.Instance.Modified = true;

                        // Notify any developer client
                        Comms.Network.IManager.Instance.NotifyConfiguration();
                    }
                    // Check for pump 1 time
                    else if ((name == "p1.time") && (value > 0))
                    {
                        logLinesCache.AppendLine("Pump 1 ran for " + value + "s");
                    }
                    // Check for pump 2 time
                    else if ((name == "p2.time") && (value > 0))
                    {
                        logLinesCache.AppendLine("Pump 2 ran for " + value + "s");
                    }


                    // This is a device value so set it in the value cache
                    valuesCache[name] = value;
                }
                else
                {
                    // Log the unexpected firmware message
                    logLinesCache.AppendLine("Unexpected firmware message: " + message);
                }
            }
            // Check for a tick message
            else if ((message.StartsWith("*I=")) && (message.Length > 3))
            {
                int value;

                if (int.TryParse(message.Substring(3), out value))
                {
                    if (valuesCache.Count > 0)
                    {
                        valueNotifyEvent.Set();
                    }

                    // This is a device value so set it in the value cache
                    valuesCache["ticks"] = value;
                }
                else
                {
                    // Log the unexpected firmware message
                    logLinesCache.AppendLine("Unexpected firmware message: " + message);
                }
            }
            else
            {
                // Log the unexpected firmware message
                logLinesCache.AppendLine("Unexpected firmware message: " + message);
            }
        }

        /// <summary>
        /// Handle a finish message from the firmware
        /// </summary>
        /// <param name="script">The script name</param>
        /// <returns>True if handled, false otherwise</returns>
        private bool HandleFinishMessage(string script)
        {
            if (script == "Analysis")
            {
                // Call the analysis complete method for the controller thread
                AnalysisComplete();
            }
            else if (script == "StartUp")
            {
                // Check for the startup initialise phase
                if (phase.StartsWith("StartUp.Initialise"))
                {
                    // Set the phase to idle
                    SetPhase("Idle");

                    // Close the splash screen and show the start form
                    IViewManager.Instance.ReplaceRoot("Start").Unwind();
                }
                else
                {
                    return false;
                }
            }
            else if ((script == "Load") || (script == "StartUpUnload") || (script == "Unload"))
            {
                // Check for failure to open drawer
                if (hashSetWarnings.Contains(WARNING_DRAWER_NOT_OPENED))
                {
                    // Remember the current warnings
                    var currentWarnings = new HashSet<int>(hashSetWarnings);

                    // Remove the failure to open drawer warning
                    currentWarnings.Remove(WARNING_DRAWER_NOT_OPENED);

                    // Don't try to unload if an error has occurred
                    if (string.IsNullOrEmpty(LastError.Message))
                    {
                        // Unload the cartridge
                        if (UnloadCartridge(false))
                        {
                            // Add the current warnings to the list
                            hashSetWarnings.UnionWith(currentWarnings);
                        }
                    }
                }
                // Check for the loading open phase
                else if (phase.StartsWith("Loading.Open"))
                {
                    // Check for warnings
                    if (hashSetWarnings.Contains(WARNING_AMBIENT_PRESSURE_CHECK_FAILED))
                    {
                        ShowFault("Ambient pressure check failed");
                    }
                    else if (hashSetWarnings.Contains(WARNING_PRESSURE_SENSOR_CHECK_FAILED))
                    {
                        ShowFault("Pressure sensor check failed");
                    }
                    else if (hashSetWarnings.Contains(WARNING_POTENTIOSTAT_CHECK_FAILED))
                    {
                        // Update the phase
                        SetPhase("Reset.Close.Prompt");

                        // Show the close drawer form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("EmptyDrawer");
                    }
                    else
                    {
                        // Set the phase to loading closing the drawer
                        SetPhase("Loading.Close");

                        // Show the load cartridge form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("LoadCartridge");
                    }
                }
                // Check for the startup unload phase
                else if (phase.StartsWith("StartUp.Unload"))
                {
                    // Set the phase to startup close
                    SetPhase("StartUp.Close");
                }
                else
                {
                    return false;
                }
            }
            else if (script == "Clamp")
            {
                if (phase.StartsWith("Testing.Clamping"))
                {
                    // Start the test
                    StartCurrentTest();
                }
                else
                {
                    return false;
                }
            }
            else if (script == "Unclamp")
            {
                // Check for the unclamping phase
                if (phase.StartsWith("Testing.Unclamping"))
                {
                    // Unload the cartridge
                    UnloadCartridge();
                }
                else
                {
                    return false;
                }
            }
            else if (script == "Reset")
            {
                // Check for the reset datum phase
                if (phase.StartsWith("Reset.Datum"))
                {
                    // Set the phase to idle
                    SetPhase("Idle");
                }
                else
                {
                    return false;
                }
            }
            else
            {
                lock (ITests.Instance.CurrentTest)
                {
                    // Check for a valid assay
                    if (ITests.Instance.CurrentTest.Result.Assay == null)
                    {
                        return false;
                    }

                    // Check for the right phase
                    if ((phase.StartsWith("Testing.Main") == false) &&
                        (phase.StartsWith("Testing.Rescan") == false))
                    {
                        return false;
                    }

                    // Check for the main testing phase
                    if (phase.StartsWith("Testing.Main") &&
                        (script != ITests.Instance.CurrentTest.Result.Assay.Script) &&
                        (script != ITests.Instance.CurrentTest.Result.Assay.UngScript))
                    {
                        return false;
                    }

                    // Check for the rescan testing phase
                    if (phase.StartsWith("Testing.Rescan") &&
                        (script != ITests.Instance.CurrentTest.Result.Assay.VoltammetryScript))
                    {
                        return false;
                    }

                    // Check for the demo assays
                    if ((ITests.Instance.CurrentTest.Result.Assay.Code == "947") ||
                        (ITests.Instance.CurrentTest.Result.Assay.Code == "948") ||
                        (ITests.Instance.CurrentTest.Result.Assay.Code == "949"))
                    {
                        ITests.Instance.CurrentTest.Data.StartPotential = 0.0;
                        ITests.Instance.CurrentTest.Data.IncrementalPotential = 3.0;
                        ITests.Instance.CurrentTest.Data.CellValues[0] = new double[] { 12.5, 12.5, 12.5, 12.5, 15.0, 12.5, 15, 12.5, 12.5, 15.0, 12.5, 15.0, 15.0, 15.0, 12.5, 12.5, 15.0, 15.0, 17.5, 17.5, 15.0, 15.0, 17.5, 17.5, 17.5, 17.5, 15.0, 20.0, 20.0, 20.0, 20.0, 17.5, 20.0, 22.5, 22.5, 25.0, 22.5, 25.0, 25.0, 27.5, 22.5, 27.5, 30.0, 30.0, 35.0, 37.5, 40.0, 40.0, 42.5, 45.0, 47.5, 50.0, 50.0, 57.5, 60.0, 62.5, 65.0, 70.0, 70.0, 72.5, 75.0, 82.5, 82.5, 87.5, 92.5, 95.0, 95.0, 95.0, 100, 95.0, 100, 100, 97.5, 97.5, 97.5, 95.0, 90.0, 90.0, 87.5, 85.0, 82.5, 77.5, 75.0, 72.5, 67.5, 62.5, 60.0, 60.0, 57.5, 52.5, 52.5, 47.5, 45.0, 45.0, 37.5, 37.5, 37.5, 37.5, 32.5, 35.0, 32.5, 32.5, 30.0, 27.5, 25.0, 30.0, 25.0, 27.5, 27.5, 27.5, 25.0, 22.5, 25.0, 22.5, 22.5, 22.5, 25.0, 22.5, 20.0, 22.5, 22.5, 20.0, 22.5, 22.5, 20.0, 22.5, 20.0, 22.5, 20.0, 22.5, 20.0, 20.0, 22.5, 22.5, 22.5, 22.5, 22.5, 22.5, 22.5, 25.0, 25.0, 27.5, 22.5, 25.0, 22.5, 25.0, 25.0, 22.5, 22.5, 27.5, 22.5, 22.5, 25.0, 25.0, 27.5, 27.5, 25.0, 27.5, 27.5, 25.0, 25.0, 30.0, 27.5, 25.0, 27.5, 30, 27.5, 27.5, 27.5, 25.0, 30.0, 27.5, 32.5, 32.5, 32.5, 35.0, 37.5, 35.0, 32.5, 35.0, 37.5, 37.5, 40.0, 40.0, 42.5, 40.0, 42.5, 45.0, 45.0, 45.0, 47.5, 50.0, 52.5, 50.0, 52.5, 55.0, 57.5, 62.5, 60.0, 65.0, 65.0, 67.5, 69.5, 72.5, 77.5, 82.5, 80.0, 82.5, 90.0, 92.5, 95.0, 97.5, 103, 108, 110, 115 };
                        ITests.Instance.CurrentTest.Data.CellValues[1] = new double[] { 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 15, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5 };
                        ITests.Instance.CurrentTest.Data.CellValues[2] = new double[] { 12.5, 12.5, 12.5, 12.5, 15.0, 12.5, 15, 12.5, 12.5, 15.0, 12.5, 15.0, 15.0, 15.0, 12.5, 12.5, 15.0, 15.0, 17.5, 17.5, 15.0, 15.0, 17.5, 17.5, 17.5, 17.5, 15.0, 20.0, 20.0, 20.0, 20.0, 17.5, 20.0, 22.5, 22.5, 25.0, 22.5, 25.0, 25.0, 27.5, 22.5, 27.5, 30.0, 30.0, 35.0, 37.5, 40.0, 40.0, 42.5, 45.0, 47.5, 50.0, 50.0, 57.5, 60.0, 62.5, 65.0, 70.0, 70.0, 72.5, 75.0, 82.5, 82.5, 87.5, 92.5, 95.0, 95.0, 95.0, 100, 95.0, 100, 100, 97.5, 97.5, 97.5, 95.0, 90.0, 90.0, 87.5, 85.0, 82.5, 77.5, 75.0, 72.5, 67.5, 62.5, 60.0, 60.0, 57.5, 52.5, 52.5, 47.5, 45.0, 45.0, 37.5, 37.5, 37.5, 37.5, 32.5, 35.0, 32.5, 32.5, 30.0, 27.5, 25.0, 30.0, 25.0, 27.5, 27.5, 27.5, 25.0, 22.5, 25.0, 22.5, 22.5, 22.5, 25.0, 22.5, 20.0, 22.5, 22.5, 20.0, 22.5, 22.5, 20.0, 22.5, 20.0, 22.5, 20.0, 22.5, 20.0, 20.0, 22.5, 22.5, 22.5, 22.5, 22.5, 22.5, 22.5, 25.0, 25.0, 27.5, 22.5, 25.0, 22.5, 25.0, 25.0, 22.5, 22.5, 27.5, 22.5, 22.5, 25.0, 25.0, 27.5, 27.5, 25.0, 27.5, 27.5, 25.0, 25.0, 30.0, 27.5, 25.0, 27.5, 30, 27.5, 27.5, 27.5, 25.0, 30.0, 27.5, 32.5, 32.5, 32.5, 35.0, 37.5, 35.0, 32.5, 35.0, 37.5, 37.5, 40.0, 40.0, 42.5, 40.0, 42.5, 45.0, 45.0, 45.0, 47.5, 50.0, 52.5, 50.0, 52.5, 55.0, 57.5, 62.5, 60.0, 65.0, 65.0, 67.5, 69.5, 72.5, 77.5, 82.5, 80.0, 82.5, 90.0, 92.5, 95.0, 97.5, 103, 108, 110, 115 };
                        ITests.Instance.CurrentTest.Data.CellValues[3] = new double[] { 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 15, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5, 12.5 };
                    }

                    // Check for null analysis or peaks
                    if ((analysis == null) || (peaks == null))
                    {
                        // Show the fault
                        ShowFault("Invalid peak data for assay " +
                            ITests.Instance.CurrentTest.Result.Assay.Name);
                    }
                    else if ((ITests.Instance.CurrentTest.Data.CellValues[0] == null) ||
                        (ITests.Instance.CurrentTest.Data.CellValues[1] == null) ||
                        (ITests.Instance.CurrentTest.Data.CellValues[2] == null) ||
                        (ITests.Instance.CurrentTest.Data.CellValues[3] == null))
                    {
                        // Show the fault
                        ShowFault("Invalid potentiostat data for assay " +
                            ITests.Instance.CurrentTest.Result.Assay.Name);
                    }
                    else
                    {
                        // Initialise the results object
                        analysisResults = new Dictionary<int, AnalysisResult>();

                        // Initialise the list of potentiostats
                        var potentiostats = peaks.Select(x => x.Potentiostat).Distinct().ToArray();

                        logLinesCache.AppendLine("Analysing potentiostat(s) " + 
                            string.Join(", ", potentiostats.Select(x => x.ToString()).ToArray()));

                        lock (analysisResults)
                        {
                            // Kick off the analysis for the potentiostats with defined peaks
                            foreach (int potentiostat in potentiostats)
                            {
                                // Get the token for the analysis
                                int token = analysis.Analyse(new AnalysisData()
                                {
                                    Name = ITests.Instance.CurrentTest.Result.SampleId + "-" + potentiostat,
                                    Potentiostat = potentiostat,
                                    StartPotential = ITests.Instance.CurrentTest.Data.StartPotential,
                                    IncrementalPotential = 
                                        ITests.Instance.CurrentTest.Data.IncrementalPotential,
                                    CurrentDifferences = 
                                        ITests.Instance.CurrentTest.Data.CellValues[potentiostat - 1],
                                    Peaks = peaks.Where(x => x.Potentiostat == potentiostat).ToArray(),
                                }, AnalysisComplete);

                                // Initialise the placeholder for the results
                                analysisResults[token] = null;
                            }
                        }

                        // Dereference the analysis and peaks objects
                        analysis = null;
                        peaks = null;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Handle a peak message from the firmware
        /// </summary>
        /// <param name="tokens">The message tokens</param>
        /// <returns>True if handled, false otherwise</returns>
        private bool HandlePeakMessage(List<string> tokens)
        {
            if (tokens.Count < 2)
            {
                // Report an invalid peak statement
                return false;
            }
            
            if (tokens[0].ToLower() == "algorithm")
            {
                if (tokens.Count < 2)
                {
                    // Report an invalid peak statement
                    return false;
                }

                // Create the analysis object
                analysis = IAnalysis.Create(tokens[1].StringValue(), 
                    tokens.GetRange(2, tokens.Count - 2).ToArray());

                // Check for valid parameters
                if (analysis == null)
                {
                    return false;
                }

                // Set the analysis type value in the data
                ITests.Instance.CurrentTest.Data.AnalysisType = tokens[1].StringValue();

                // Create the new peaks object
                peaks = new List<PeakData>();

                return true;
            }
            else if (tokens[0].ToLower() == "define")
            {
                // Check for null peak data
                if (peaks == null)
                {
                    return false;
                }

                // Read and validate the peak parameters
                int potentiostat;
                double minPotential, maxPotential, mean, tolerance, lowerLimit, upperLimit,
                    maxVarianceForCurveFit, topPercentageForCurveFit;

                if ((tokens.Count < 12) ||
                    (int.TryParse(tokens[3], out potentiostat) == false) ||
                    (potentiostat < 1) || (potentiostat > 4) ||
                    (double.TryParse(tokens[4], out minPotential) == false) ||
                    (double.TryParse(tokens[5], out maxPotential) == false) ||
                    (maxPotential < minPotential) ||
                    (double.TryParse(tokens[6], out mean) == false) ||
                    (mean < minPotential) || (mean > maxPotential) ||
                    (double.TryParse(tokens[7], out tolerance) == false) ||
                    (tolerance < 0) ||
                    (double.TryParse(tokens[8], out lowerLimit) == false) ||
                    (lowerLimit < 0) ||
                    (double.TryParse(tokens[9], out upperLimit) == false) ||
                    (upperLimit < lowerLimit) ||
                    (double.TryParse(tokens[10], out maxVarianceForCurveFit) == false) ||
                    (maxVarianceForCurveFit < 0) ||
                    (double.TryParse(tokens[11], out topPercentageForCurveFit) == false) ||
                    (topPercentageForCurveFit < 0) || (topPercentageForCurveFit > 100))
                {
                    // Dereference the peaks
                    peaks = null;

                    // Report an invalid peak statement
                    return false;
                }

                // Create the new peak data
                var peak = new PeakData()
                {
                    Name = tokens[1].StringValue(),
                    Potentiostat = potentiostat,
                    MinPotential = minPotential,
                    MaxPotential = maxPotential,
                    Mean = mean,
                    Tolerance = tolerance,
                    LowerLimit = lowerLimit,
                    UpperLimit = upperLimit,
                    MaxVarianceForCurveFit = maxVarianceForCurveFit,
                    TopFractionForCurveFit = topPercentageForCurveFit / 100.0,
                };

                // Get the peak type
                var peakType = tokens[2].ToLower();

                if (peakType == "ignore")
                {
                    peak.Type = PeakType.Ignore;
                }
                else if (peakType == "rescan")
                {
                    peak.Type = PeakType.Rescan;
                }
                else if (peakType == "positive")
                {
                    peak.Type = PeakType.Positive;
                }
                else
                {
                    // Dereference the peaks
                    peaks = null;

                    // Report an invalid peak statement
                    return false;
                }

                // Add the peak to the current peaks
                peaks.Add(peak);

                return true;
            }
    
            // Report an invalid peak statement
            return false;
        }

        /// <summary>
        /// Handle a throw from the firmware
        /// </summary>
        /// <param name="code">The code</param>
        /// <param name="tokens">The message tokens</param>
        /// <returns>False for unexpected parameters, true otherwise</returns>
        private bool HandleFirmwareThrow(int code, string[] tokens)
        {
            // Check for an exception
            if (code >= ERROR_BASE)
            {
                // Remember and clear the running script
                string script = runningScript;

                runningScript = null;

                // The name of the script
                string scriptName = (script == null) ? "Unknown" : ("'" + script + "'");

                // Check for a developer override
                if (developerOverride)
                {
                    // Clear the override flag
                    developerOverride = false;

                    // Notify the developer client
                    Comms.Network.IManager.Instance.NotifyScriptComplete(script);
                }
                else if (code == ERROR_DRAWER_NOT_CLOSED)
                {
                    // Check for the startup initialise phase
                    if (phase.StartsWith("StartUp.Initialise"))
                    {
                        // Show the close drawer form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("UnexpectedShutdown");

                        // Set the phase to start-up close
                        SetPhase("StartUp.Close");
                    }
                    // Check for the startup initialise phase
                    else if (phase.StartsWith("StartUp.Unload"))
                    {
                        // Set the phase to start-up close
                        SetPhase("StartUp.Close");
                    }
                    else
                    {
                        // Show the fault
                        ShowFault(scriptName + " script threw error " + code + " in phase " + phase);
                    }
                }
                else
                {
                    // Show the fault
                    ShowFault(scriptName + " script threw error " + code + " in phase " + phase);
                }
            }
            else if ((code == INFORMATION_PHASE_CHANGE) && (tokens.Length == 2))
            {
                // Set the phase in the current test
                SetPhase(tokens[1].Trim());
            }
            else if ((code == INFORMATION_DRAWER_CLOSED) && (tokens.Length == 2))
            {
                // Set the scanned barcode
                IState.Instance.ScannedBarcode = tokens[1].Trim();

                // Check for the startup closing phase
                if (phase.StartsWith("StartUp.Close"))
                {
                    // Check for a script running
                    if (string.IsNullOrEmpty(runningScript))
                    {
                        // Unload the cartridge
                        UnloadCartridge(false);
                    }
                }
                // Check for the reset closing phase
                else if (phase.StartsWith("Reset.Close"))
                {
                    // Check for a script running
                    if (string.IsNullOrEmpty(runningScript))
                    {
                        // Unload the cartridge
                        UnloadCartridge(false);
                    }
                }
                // Check for the loading closing phase
                else if (phase.StartsWith("Loading.Close"))
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
                                new Dictionary<string,object>() { { "AssayCode", assayCode } } );
                        }
                    }
                    catch (Exception)
                    {
                        // Show the cartridge error form
                        IViewManager.Instance.CurrentForm.ShowFormFromForm("CartridgeError");
                    }
                }
            }
            else if ((code == INFORMATION_PERCENT_COMPLETE) && (tokens.Length == 2))
            {
                // Parse the percent complete value
                int percentComplete;

                if (int.TryParse(tokens[1], out percentComplete) &&
                    (percentComplete >= 0) && (percentComplete <= 100))
                {
                    lock (ITests.Instance.CurrentTest)
                    {
                        ITests.Instance.CurrentTest.PercentComplete = percentComplete;
                    }
                }
            }
            else if (code == INFORMATION_DRAWER_OPENING)
            {
                // Set the scanned barcode to null
                IState.Instance.ScannedBarcode = null;
            }
            else if ((code == WARNING_METRIC_VIOLATED) && (tokens.Length == 3))
            {
            }
            else if ((code == WARNING_DRAWER_CLOSED_NO_BARCODE) && (tokens.Length == 1))
            {
                // Set the scanned barcode to an empty string
                IState.Instance.ScannedBarcode = "";

                // Check for the startup closing phase
                if (phase.StartsWith("StartUp.Close"))
                {
                    // Close the close drawer form
                    IViewManager.Instance.CurrentForm.Back();

                    // Execute the startup script
                    if (ExecuteScript("StartUp"))
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
                // Check for the reset closing phase
                else if (phase.StartsWith("Reset.Close"))
                {
                    // Check for a close drawer prompt
                    if (phase.StartsWith("Reset.Close.Prompt"))
                    {
                        // Close the close drawer form
                        IViewManager.Instance.CurrentForm.Back();
                    }

                    // Check for a script running
                    if (string.IsNullOrEmpty(runningScript))
                    {
                        // Reset the clamp
                        ResetClamp();
                    }
                }
                // Check for the loading closing phase
                else if (phase.StartsWith("Loading.Close"))
                {
                    // Show the cartridge error form
                    IViewManager.Instance.CurrentForm.ShowFormFromForm("CartridgeError");
                }
            }
            else if (code == WARNING_DRAWER_NOT_OPENED)
            {
                // Check for a script running
                if (string.IsNullOrEmpty(runningScript))
                {
                    // Don't try to unload if an error has occurred
                    if (string.IsNullOrEmpty(LastError.Message))
                    {
                        // Unload the cartridge
                        UnloadCartridge(false);
                    }
                }
                else
                {
                    // Add this to the set of warnings for this script
                    hashSetWarnings.Add(code);
                }
            }
            else if ((code == WARNING_POTENTIOSTAT_CHECK_FAILED) ||
                (code == WARNING_AMBIENT_PRESSURE_CHECK_FAILED) ||
                (code == WARNING_PRESSURE_SENSOR_CHECK_FAILED))
            {
                // Add this to the set of warnings for this script
                hashSetWarnings.Add(code);
            }
            else
            {
                // Unrecognised throw command
                return false;
            }

            return true;
        }
    }
}
