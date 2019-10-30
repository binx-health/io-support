/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Threading;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IO.Comms.Network;
using IO.Model.Serializable;
using IO.Scripting;
using IO.FileSystem;
using IO.Model.Volatile;
using IO.View;

namespace IO.Controller
{
    /// <summary>
    /// The controller object
    /// </summary>
    partial class Controller
    {
        /// <summary>
        /// Handle a network message
        /// </summary>
        /// <param name="message">The message</param>
        void HandleNetworkMessage(string message)
        {
            // Load the message into the XML document
            xmlDocument.LoadXml(message);

            // Get the method node
            var methodNode = xmlDocument.SelectSingleNode("soap:Envelope/soap:Body/*[1]", 
                xmlNamespaceManager);

            // Check the message method
            if (methodNode.LocalName == "GetPhase")
            {
                // Respond with the list of assays
                IManager.Instance.RespondGetPhase(phase);
            }
            else if (methodNode.LocalName == "GetConfiguration")
            {
                // Respond with the configuration
                IManager.Instance.RespondGetConfiguration(IConfiguration.Instance.SerialiseFragment(
                    IConfiguration.Instance.Types));
            }
            else if (methodNode.LocalName == "GetAssays")
            {
                // Respond with the list of assays
                IManager.Instance.RespondGetAssays(IAssays.Instance.SerialiseFragment(
                    IAssays.Instance.Types));
            }
            else if (methodNode.LocalName == "GetScript")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "name"))
                {
                    var script = IScripts.Instance.Where(x => string.Compare(x.Name, 
                        methodNode.FirstChild.InnerText, true) == 0).FirstOrDefault();

                    if (script != null)
                    {
                        // Respond with the script as plain text
                        IManager.Instance.RespondGetScript(script.Value);
                    }
                    else
                    {
                        IManager.Instance.RespondError("m:Client", "Invalid 'name' argument");
                    }
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'name' parameter");
                }
            }
            else  if (methodNode.LocalName == "GetDefaultMetrics")
            {
                // Respond with the default metrics
                IManager.Instance.RespondGetDefaultMetrics(IDefaultMetrics.Instance.Value);
            }
            else if (methodNode.LocalName == "GetMetrics")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "name"))
                {
                    // Read the metrics file
                    string metrics = IAssays.Instance.Where(x => x.MetricsName == 
                        methodNode.FirstChild.InnerText).Select(x => x.Metrics.Value).FirstOrDefault();

                    if (metrics != null)
                    {
                        // Respond with the metrics as plain text
                        IManager.Instance.RespondGetMetrics(metrics);
                    }
                    else
                    {
                        IManager.Instance.RespondError("m:Client", "Invalid 'name' argument");
                    }
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'name' parameter");
                }
            }
            else if (methodNode.LocalName == "GetDeviceValues")
            {
                // Respond with the device values
                IManager.Instance.RespondGetDeviceValues(IDeviceValues.Instance.SerialiseFragment());
            }
            else if (methodNode.LocalName == "GetTestData")
            {
                // Respond with the test data
                IManager.Instance.RespondGetTestData(ITests.Instance.SerialiseFragment(
                    ITests.Instance.Types));
            }
            else if (methodNode.LocalName == "SetConfiguration")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "value"))
                {
                    // Set the configuration
                    IConfiguration.Instance = IConfiguration.Instance.Deserialise(
                        methodNode.FirstChild.InnerXml, IConfiguration.Instance.Types);
                    IConfiguration.Instance.Modified = true;

                    // Respond with an acknowledgement
                    IManager.Instance.RespondSetConfiguration();
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'value' parameter");
                }
            }
            else if (methodNode.LocalName == "SetAssays")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "value"))
                {
                    // Set the assays
                    IAssays.Instance = IAssays.Instance.Deserialise(
                        methodNode.FirstChild.InnerXml, IAssays.Instance.Types);

                    // Set the assays to modified
                    foreach (var assay in IAssays.Instance)
                    {
                        assay.Modified = true;
                    }

                    // Respond with an acknowledgement
                    IManager.Instance.RespondSetAssays();
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'value' parameter");
                }
            }
            else if (methodNode.LocalName == "SetScript")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "name") &&
                    (methodNode.FirstChild.NextSibling != null) &&
                    (methodNode.FirstChild.NextSibling.LocalName == "value"))
                {
                    // Set the new script value
                    IScripts.Instance.SetScript(methodNode.FirstChild.InnerText,
                        methodNode.FirstChild.NextSibling.InnerText);

                    // Respond with an acknowledgement
                    IManager.Instance.RespondSetScript();
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'name' or 'value' parameter");
                }
            }
            else if (methodNode.LocalName == "SetDefaultMetrics")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "value"))
                {
                    // Check for a change in value
                    if (IDefaultMetrics.Instance.Value != methodNode.FirstChild.InnerText)
                    {
                        // Set the default metrics
                        IDefaultMetrics.Instance.Value = methodNode.FirstChild.InnerText;

                        // Set the modified and loaded flags
                        IDefaultMetrics.Instance.Modified = true;
                        IDefaultMetrics.Instance.Loaded = false;
                    }

                    // Respond with an acknowledgement
                    IManager.Instance.RespondSetDefaultMetrics();
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'value' parameter");
                }
            }
            else if (methodNode.LocalName == "SetMetrics")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "name") &&
                    (methodNode.FirstChild.NextSibling != null) &&
                    (methodNode.FirstChild.NextSibling.LocalName == "value"))
                {
                    // Find the metrics
                    var metrics = IAssays.Instance.Select(x => x.Metrics).Where(
                        x => x.Name == methodNode.FirstChild.InnerText).FirstOrDefault();

                    if (metrics != null)
                    {
                        // Check for a change in value
                        if (metrics.Value != methodNode.FirstChild.NextSibling.InnerText)
                        {
                            // Set the new metric value
                            metrics.Value = methodNode.FirstChild.NextSibling.InnerText;

                            // Set the modified and loaded flags
                            metrics.Modified = true;
                            metrics.Loaded = false;
                        }

                        // Respond with an acknowledgement
                        IManager.Instance.RespondSetMetrics();
                    }
                    else
                    {
                        IManager.Instance.RespondError("m:Client", "Invalid 'name' argument");
                    }
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'name' or 'value' parameter");
                }
            }
            else if (methodNode.LocalName == "ExecuteScript")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "name"))
                {
                    // Execute a script on the firmware
                    if (ExecuteScript(methodNode.FirstChild.InnerText))
                    {
                        // Set the flag to indicate a developer override
                        developerOverride = true;

                        // Respond with an acknowledgement
                        IManager.Instance.RespondExecuteScript();
                    }
                    else
                    {
                        IManager.Instance.RespondError("m:Client", "Script sequence error running '" +
                            methodNode.FirstChild.InnerText + "'");
                    }
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'name' parameter");
                }
            }
            else if (methodNode.LocalName == "ExecuteCommand")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "command"))
                {
                    // Execute a command on the firmware
                    ExecuteCommand(methodNode.FirstChild.InnerText);

                    // Set the flag to indicate a developer override
                    developerOverride = true;

                    // Respond with an acknowledgement
                    IManager.Instance.RespondExecuteCommand();
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'command' parameter");
                }
            }
            else if (methodNode.LocalName == "AbortScript")
            {
                // Execute a command on the firmware
                AbortScript();

                // Respond with an acknowledgement
                IManager.Instance.RespondAbortScript();
            }
            else if (methodNode.LocalName == "ResetReader")
            {
                // Respond with an acknowledgement
                IManager.Instance.RespondResetReader();

                // Shutdown
                ShutdownSystem();
            }
            else if (methodNode.LocalName == "LaunchCommandPrompt")
            {
                // Respond with an acknowledgement
                IManager.Instance.RespondLaunchCommandPrompt();

                // Serialise any assay data
                SerialiseAssayData();

                // Start the command prompt
                System.Diagnostics.Process.Start("cmd.exe");

                // Close the main window
                IViewManager.Instance.Stop();
            }
            else if (methodNode.LocalName == "GetAllDeviceValues")
            {
                // Respond with the serialised device values
                IManager.Instance.RespondGetAllDeviceValues(IDeviceValues.Instance.SerialiseFragment());
            }
            else if (methodNode.LocalName == "GetDeviceValue")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "name"))
                {
                    // Try to get the value from the dictionary
                    int value;

                    if (IDeviceValues.Instance.TryGetValue(methodNode.FirstChild.InnerText, out value))
                    {
                        // Respond with the value
                        IManager.Instance.RespondGetDeviceValue(value);
                    }
                    else
                    {
                        IManager.Instance.RespondError("m:Client", "Invalid 'name' argument");
                    }
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'name' parameter");
                }
            }
            else if (methodNode.LocalName == "AddCertificate")
            {
                if ((methodNode.FirstChild != null) &&
                    (methodNode.FirstChild.LocalName == "value"))
                {
                    // Create the certificate from the data and the authorised root certificate store
                    var certificate = new X509Certificate2(Convert.FromBase64String(
                        methodNode.FirstChild.InnerText));
                    var store = new X509Store(StoreName.AuthRoot, StoreLocation.LocalMachine);

                    // Open the store, add the certificate and close it again
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(certificate);
                    store.Close();
                }
                else
                {
                    IManager.Instance.RespondError("m:Client", "Missing 'value' parameter");
                }
            }
            else
            {
                IManager.Instance.RespondError("m:Client", "Unknown function");
            }
        }
    }
}
