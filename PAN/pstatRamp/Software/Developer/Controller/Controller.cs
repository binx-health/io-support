/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IO.View;
using IO.Comms.Network;
using IO.Scripting;
using IO.FileSystem;
using IO.Model.Volatile;
using IO.Model.Serializable;

namespace IO.Controller
{
    /// <summary>
    /// The developer controller object
    /// </summary>
    public class Controller: IDisposable
    {
        /// <summary>
        /// The controller thread
        /// </summary>
        private Thread controllerThread = null;

        /// <summary>
        /// The client root certificate that contains the private key for digital signatures
        /// </summary>
        private RSACryptoServiceProvider rsaCryptoServiceProvider = null;

        /// <summary>
        /// Initialise the controller
        /// </summary>
        public void Initialise()
        {
            // Load the client root certificate
            rsaCryptoServiceProvider = new X509Certificate2("IoClientRoot.pfx", "App1ause").PrivateKey as
                RSACryptoServiceProvider;

            // Abort the controller thread
            if (controllerThread != null)
            {
                controllerThread.Abort();
            }

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
            // Initialise the list of wait handles
            var waitHandles = new WaitHandle[]
            { 
                IO.Comms.Network.MessageQueue.Instance,
                IO.View.MessageQueue.Instance,
            };

            // Create an XML document and namespace manager
            var xmlDocument = new XmlDocument() { XmlResolver = null };
            var xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);

            // Add the namespaces
            xmlNamespaceManager.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            xmlNamespaceManager.AddNamespace("m", "http://www.AtlasGenetics.com/IO");

            // Run until aborted
            while (true)
            {
                // Wait for a message
                int handleIndex = WaitHandle.WaitAny(waitHandles);

                // Check for a network message
                if (handleIndex != WaitHandle.WaitTimeout)
                {
                    // Pop and process all of the messages
                    string message;
                    int messages = IO.Comms.Network.MessageQueue.Instance.Length;

                    while ((messages-- > 0) &&
                        (message = IO.Comms.Network.MessageQueue.Instance.Pop()) != null)
                    {
                        try
                        {
                            // Load the message into the XML document
                            xmlDocument.LoadXml(message);

                            // Get the method node
                            var methodNode = xmlDocument.SelectSingleNode(
                                "soap:Envelope/soap:Body/*[1]", xmlNamespaceManager);

                            // Check for discovery of devices
                            if (methodNode.LocalName == "NotifyDiscovery")
                            {
                                // Update the list of available devices
                                Main.Instance.UpdateDevices(IClient.Instance.Devices.Values.Distinct());
                            }
                            // Check for log lines
                            else if ((methodNode.LocalName == "NotifyLogLines") &&
                                    (methodNode.FirstChild.LocalName == "log"))
                            {
                                // Write the log
                                Main.Instance.Log(methodNode.FirstChild.InnerText);
                            }
                            // Check for device values
                            else if ((methodNode.LocalName == "NotifyDeviceValues") &&
                                    (methodNode.FirstChild.LocalName == "values"))
                            {
                                // Clone the device values object
                                IDeviceValues deviceValues = 
                                    (IDeviceValues)IDeviceValues.Instance.Clone(false);

                                // Clear the values and reload from the XML
                                deviceValues.ReadXml(XmlReader.Create(new StringReader(
                                    methodNode.FirstChild.InnerXml)));

                                // Update the device values
                                Main.Instance.UpdateDeviceValues(deviceValues);
                            }
                            // Check for phase change
                            else if ((methodNode.LocalName == "NotifyPhaseChange") &&
                                    (methodNode.FirstChild.LocalName == "phase"))
                            {
                                // Set the new phase
                                Main.Instance.Phase = methodNode.FirstChild.InnerText;
                            }
                            // Check for script completion
                            else if ((methodNode.LocalName == "NotifyScriptComplete") &&
                                    (methodNode.FirstChild.LocalName == "name"))
                            {
                                // Check for the command script name
                                if (methodNode.FirstChild.InnerText != "?")
                                {
                                    // Clear the running script
                                    Main.Instance.RunningScript = null;
                                }
                            }
                            // Check for test data
                            else if (methodNode.LocalName == "NotifyTestData")
                            {
                                // Read the test data from the device
                                GetTestData();

                                // Update the assays
                                Main.Instance.UpdateTestData();
                            }
                            // Check for test data
                            else if (methodNode.LocalName == "NotifyConfiguration")
                            {
                                // Get the new configuration
                                Main.Instance.Configuration = IClient.Instance.GetConfiguration();

                                // Update the assays
                                Main.Instance.UpdateConfiguration();
                            }
                        }
                        catch (Exception e)
                        {
                            // Set the last error
                            LastError.Message = e.Message;

                            // Check for a valid handle first as the window may have been destroyed
                            if (Main.Instance.IsHandleCreated)
                            {
                                // Update the status
                                Main.Instance.UpdateStatus();
                            }
                        }
                    }

                    // Flush the queue
                    Message viewMessage;

                    while ((viewMessage = IO.View.MessageQueue.Instance.Pop()) != null)
                    {
                        try
                        {
                            // Handle the view message
                            HandleViewMessage(viewMessage);
                        }
                        catch (Exception e)
                        {
                            // Set the last error
                            LastError.Message = e.Message;

                            // Update the status
                            Main.Instance.UpdateStatus();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle a view message
        /// </summary>
        /// <param name="message">The message</param>
        private void HandleViewMessage(Message message)
        {
            // Check for a command message
            if (message is CommandMessage)
            {
                var formCommand = (CommandMessage)message;

                // Check for an intialise message
                if (formCommand.Command == FormCommand.Initialise)
                {
                    // Update the status screen
                    Main.Instance.UpdateStatus();

                    // Update the assays
                    Main.Instance.UpdateAssays();

                    // Attempt to connect
                    IClient.Instance.Discover();

                    // Update the status
                    Main.Instance.UpdateStatus();
                }
            }
            // Check for a menu item message
            else if (message is MenuItemMessage)
            {
                var menuItemMessage = (MenuItemMessage)message;

                // Check for a reconnect message
                if (menuItemMessage.MenuItem == "Reconnect")
                {
                    // Disconnect
                    IClient.Instance.Disconnect();

                    // Clear any errors
                    LastError.Message = null;

                    // Loop through the addresses for the preferred device
                    foreach (var address in IClient.Instance.Devices.Where(
                        x => x.Value == Main.Instance.PreferredDevice).Select(x => x.Key))
                    {
                        try
                        {
                            // Update the status
                            Main.Instance.UpdateStatus();

                            // Try to connect to the device
                            IClient.Instance.Connect(address, Main.Instance.ReaderPort);

                            // Update the status
                            Main.Instance.UpdateStatus();

                            try
                            {
                                // Get the phase
                                Main.Instance.Phase = IClient.Instance.GetPhase();

                                // Get the configuration
                                Main.Instance.Configuration = IClient.Instance.GetConfiguration();

                                // Get the device values
                                var deviceValues = Deserialise(IDeviceValues.Instance,
                                    IClient.Instance.GetDeviceValues());

                                // Update the device values
                                Main.Instance.UpdateDeviceValues(deviceValues);

                                // Read the test data from the device
                                GetTestData();

                                // Update the assays
                                Main.Instance.UpdateTestData();

                                break;
                            }
                            catch (Exception e)
                            {
                                // Disconnect again
                                IClient.Instance.Disconnect();

                                throw e;
                            }
                        }
                        catch (Exception e)
                        {
                            // Report the error
                            LastError.Message = e.Message;

                            // Clear the configuration
                            Main.Instance.Configuration = null;
                        }
                    }

                    // Update the configuration
                    Main.Instance.UpdateConfiguration();

                    if (IClient.Instance.Connected)
                    {
                        // Clear the loaded flags
                        IDefaultMetrics.Instance.Loaded = false;

                        // Loop through the assays
                        foreach (var assay in IAssays.Instance)
                        {
                            // Clear the loaded flag for the assay
                            assay.Loaded = false;

                            // Clear the loaded flag for the metrics
                            if (assay.Metrics != null)
                            {
                                assay.Metrics.Loaded = false;
                            }
                        }

                        // Loop through the scripts
                        foreach (var script in IScripts.Instance)
                        {
                            script.Loaded = false;
                        }

                        // Update the assays
                        Main.Instance.UpdateAssays();
                    }
                    else
                    {
                        // Clear the list of available devices
                        Main.Instance.UpdateDevices();

                        // Discover again
                        IClient.Instance.Discover();
                    }
                }
                // Check for a read message
                else if (menuItemMessage.MenuItem == "Read")
                {
                    // Read the assays from the device
                    GetData();

                    // Update the status and assays
                    Main.Instance.UpdateStatus();
                    Main.Instance.UpdateAssays();
                    Main.Instance.UpdateControl();
                }
                // Check for a write message
                else if (menuItemMessage.MenuItem == "Write")
                {
                    // Write the assays to the device
                    SetData();

                    // Update the status and assays
                    Main.Instance.UpdateStatus();
                    Main.Instance.UpdateAssays();
                }
                // Check for a reset message
                else if (menuItemMessage.MenuItem == "Reset")
                {
                    // Reset  the reader and disconnect
                    IClient.Instance.ResetReader();
                    IClient.Instance.Disconnect(true);

                    // Clear any errors
                    LastError.Message = null;

                    // Update the status
                    Main.Instance.UpdateStatus();

                    // Clear the list of available devices
                    Main.Instance.UpdateDevices();

                    // Wait for the reader to shutdown
                    Thread.Sleep(5000);

                    // Attempt to connect
                    IClient.Instance.Discover();

                    // Update the status
                    Main.Instance.UpdateStatus();
                }
                // Check for a reset message
                else if (menuItemMessage.MenuItem == "CommandPrompt")
                {
                    // Reset  the reader and disconnect
                    IClient.Instance.LaunchCommandPrompt();
                    IClient.Instance.Disconnect(true);

                    // Clear any errors
                    LastError.Message = null;

                    // Update the status
                    Main.Instance.UpdateStatus();

                    // Clear the list of available devices
                    Main.Instance.UpdateDevices();

                    // Wait for the reader to shutdown
                    Thread.Sleep(5000);

                    // Attempt to connect
                    IClient.Instance.Discover();

                    // Update the status
                    Main.Instance.UpdateStatus();
                }
                // Check for a load message
                else if (menuItemMessage.MenuItem == "Load")
                {
                    // Load the assays
                    Load();

                    // Update the assays
                    Main.Instance.UpdateAssays();

                    // Update the current control
                    Main.Instance.UpdateControl();
                }
                // Check for a save message
                else if (menuItemMessage.MenuItem == "Save")
                {
                    // Save the assays
                    Save();

                    // Update the assays
                    Main.Instance.UpdateAssays();
                }
                // Check for a save message
                else if (menuItemMessage.MenuItem == "Export")
                {
                    // Save the assays
                    Export(menuItemMessage.Parameters["Folder"] as string);
                }
                // Check for an execute message
                else if (menuItemMessage.MenuItem == "Execute")
                {
                    // Try to get the script name
                    object value;

                    if (menuItemMessage.Parameters.TryGetValue("Name", out value))
                    {
                        // Execute the script
                        Execute((string)value);

                        // Set the running script
                        Main.Instance.RunningScript = (string)value;
                    }
                }
                // Check for an abort message
                else if (menuItemMessage.MenuItem == "Abort")
                {
                    try
                    {
                        // Try to abort the running script
                        IClient.Instance.AbortScript();

                        // Clear the running script
                        Main.Instance.RunningScript = null;
                    }
                    catch (Exception)
                    {
                        // This can legitimately fail
                    }
                }
                // Check for a command message
                else if (menuItemMessage.MenuItem == "Command")
                {
                    // Try to get the command
                    object value;

                    if (menuItemMessage.Parameters.TryGetValue("Value", out value))
                    {
                        // Execute the command and the delay command to stop the machine resetting
                        IClient.Instance.ExecuteCommand((string)value);

                        // Set the running script
                        Main.Instance.RunningScript = "?";
                    }
                }
                // Check for a script edit notification
                else if (menuItemMessage.MenuItem == "ScriptEdit")
                {
                    // Set each notified script
                    foreach (var parameter in menuItemMessage.Parameters)
                    {
                        IScripts.Instance.SetScript(parameter.Key, parameter.Value as string);
                    }

                    // Update the assays
                    Main.Instance.UpdateAssays();
                }
                // Check for a configuration edit notification
                else if (menuItemMessage.MenuItem == "ConfigurationEdit")
                {
                    // Update the assays
                    Main.Instance.Configuration = menuItemMessage.Parameters["Value"] as string;

                    // Set the new configuration on the reader
                    IClient.Instance.SetConfiguration(Main.Instance.Configuration);
                }
                // Check for a script edit notification
                else if (menuItemMessage.MenuItem == "MetricsEdit")
                {
                    // Update the assays
                    Main.Instance.UpdateAssays();
                }
                // Check for a script edit notification
                else if (menuItemMessage.MenuItem == "AssayEdit")
                {
                    // Update the assays
                    Main.Instance.UpdateAssays();
                }
            }
        }

        /// <summary>
        /// Serialise an object into XML
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The XML as a string</returns>
        private static string Serialise<T>(T obj, Type[] types = null)
        {
            // Create a string builder
            var stringBuilder = new StringBuilder();

            // Create the serialiser from the type
            var xmlSerializer = new XmlSerializer(obj.GetType(), (types != null) ? types : new Type[0]);

            // Create an XML writer to serialise the object as a file
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
            {
                Indent = true,
            }))
            {
                // Serialise the object
                xmlSerializer.Serialize(xmlWriter, obj);
            }

            // Return the XML fragment as a string
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Serialise an object into an XML fragment
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The XML as a string</returns>
        private static string SerialiseFragment<T>(T obj, Type[] types = null)
        {
            // Create a string builder
            var stringBuilder = new StringBuilder();

            // Create the serialiser from the type
            var xmlSerializer = new XmlSerializer(obj.GetType(), (types != null) ? types : new Type[0]);

            // Create an XML writer to serialise the object as a file
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = false,
            }))
            {
                // Serialise the object
                xmlSerializer.Serialize(xmlWriter, obj);
            }

            // Return the XML fragment as a string
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deserialise an object from XML
        /// </summary>
        /// <typeparam name="T">The type of object required</typeparam>
        /// <param name="obj">Any existing object instance</param>
        /// <param name="fragment">The XML</param>
        /// <param name="types">The additional types required for serialisation</param>
        /// <returns>The new object</returns>
        public static T Deserialise<T>(T obj, string fragment, Type[] types = null)
        {
            // Create the serialiser from the type
            var xmlSerializer = new XmlSerializer(obj.GetType(), (types != null) ? types : new Type[0]);

            // Create a string reader
            using (var stringReader = new StringReader(fragment))
            {
                // Deserialise the object and return it
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }

        /// <summary>
        /// Execute the passed script
        /// </summary>
        /// <param name="name"></param>
        private void Execute(string name)
        {
            // Set this and all child scripts on the reader
            SetChildScripts(new string[] { name });

            // Execute the script
            IClient.Instance.ExecuteScript(name);
        }

        /// <summary>
        /// Method to get the data from the reader
        /// </summary>
        private void GetData()
        {
            // Get the default metrics
            var value = IClient.Instance.GetDefaultMetrics();

            if (value != IDefaultMetrics.Instance.Value)
            {
                IDefaultMetrics.Instance.Value = value;
                IDefaultMetrics.Instance.Modified = true;
            }

            // Set the loaded flag
            IDefaultMetrics.Instance.Loaded = true;

            // Get the new assays
            var xmlSerializer = new XmlSerializer(IAssays.Instance.GetType(), IAssays.Instance.Types);
            var response = IClient.Instance.GetAssays();
            var newAssays = (IAssays)xmlSerializer.Deserialize(new StringReader(response));

            // Loop through the existing assays
            foreach (var existingAssay in IAssays.Instance.ToArray())
            {
                // Check for a new instance
                var newAssay = newAssays.Where(x => x.Name == existingAssay.Name).FirstOrDefault();

                if (newAssay == null)
                {
                    // Remove this assay
                    IAssays.Instance.Remove(existingAssay);
                }
                else 
                {
                    // Remove the new assay
                    newAssays.Remove(newAssay);

                    // Check for an assay cahnge
                    if (newAssay.Equals(existingAssay) == false)
                    {
                        // Replace the existing assay
                        IAssays.Instance.Remove(existingAssay);
                        IAssays.Instance.Add(newAssay);

                        // Set the flags on the new assay
                        newAssay.Modified = true;
                        newAssay.Loaded = true;
                    }
                    else
                    {
                        // Set the loaded flag on the existing assay
                        existingAssay.Loaded = true;
                    }
                }
            }

            // Loop through the assays that we didn't have existing copies of
            foreach (var newAssay in newAssays)
            {
                // Add the assay
                IAssays.Instance.Add(newAssay);

                // Set the flags on the new assay
                newAssay.Modified = true;
                newAssay.Loaded = true;
            }

            // Get the metrics for the assays
            foreach (var metrics in IAssays.Instance.Select(x => x.Metrics).Distinct())
            {
                // Get the value
                value = IClient.Instance.GetMetrics(metrics.Name);

                // Check for a change
                if (value != metrics.Value)
                {
                    metrics.Value = value;
                    metrics.Modified = true;
                }

                metrics.Loaded = true;
            }

            // Initialise a set of added scripts
            var scriptsAdded = new HashSet<string>();

            // Loop through the assays getting the child scripts
            foreach (var assay in IAssays.Instance)
            {
                GetChildScripts(new List<string>()
                {
                    assay.Script, assay.UngScript, assay.VoltammetryScript
                }, ref scriptsAdded);
            }

            // Get the fixed scripts
            GetChildScripts(IAssays.SupportingScripts, ref scriptsAdded);

            // Remove any unreferenced scripts
            IScripts.Instance.RemoveAll(x => scriptsAdded.Contains(x.Name, 
                StringComparer.CurrentCultureIgnoreCase) == false);
        }

        /// <summary>
        /// Method to get the test data from the reader
        /// </summary>
        private void GetTestData()
        {
            // Get the tests from the reader
            var xmlSerializer = new XmlSerializer(ITests.Instance.GetType(), ITests.Instance.Types);
            var response = IClient.Instance.GetTestData();
            ITests.Instance = (ITests)xmlSerializer.Deserialize(new StringReader(response));
        }

        /// <summary>
        /// Get child scripts
        /// </summary>
        /// <param name="script">The parent script</param>
        private void GetChildScripts(IEnumerable<string> scripts, ref HashSet<string> scriptsAdded)
        {
            // Loop through the children
            foreach (var name in scripts)
            {
                // Check if we have already added this script
                if (scriptsAdded.Contains(name, StringComparer.CurrentCultureIgnoreCase) == false)
                {
                    // Add this script to the set
                    scriptsAdded.Add(name);

                    // Get the script
                    var response = IClient.Instance.GetScript(name);

                    // Set the script value
                    var script = IScripts.Instance.SetScript(name, response);

                    // Set the script to loaded
                    script.Loaded = true;

                    // Reccursively get the child scripts
                    GetChildScripts(script.Children, ref scriptsAdded);
                }
            }
        }

        /// <summary>
        /// Method to set the data on the reader
        /// </summary>
        private void SetData()
        {
            // Only write the default metrics if supporting scripts are editable
            if (Main.Instance.EditSupportingScripts &&
                (IDefaultMetrics.Instance.Loaded == false))
            {
                // Set the default metrics
                IClient.Instance.SetDefaultMetrics(IDefaultMetrics.Instance.Value);
                IDefaultMetrics.Instance.Loaded = true;
            }

            if ((IAssays.Instance.Loaded == false) || 
                (IAssays.Instance.Exists(x => x.Loaded == false)))
            {
                // Set the serialised assays
                IClient.Instance.SetAssays(SerialiseFragment(IAssays.Instance, IAssays.Instance.Types));

                foreach (var assay in IAssays.Instance)
                {
                    assay.Loaded = true;
                }

                IAssays.Instance.Loaded = true;
            }

            // Get the metrics for the assays
            foreach (var metrics in IAssays.Instance.Select(x => x.Metrics).Distinct().Where(
                x => x.Loaded == false))
            {
                IClient.Instance.SetMetrics(metrics.Name, metrics.Value);
                metrics.Loaded = true;
            }

            // Loop through the assays
            foreach (var assay in IAssays.Instance)
            {
                SetChildScripts(new List<string>()
                    {
                        assay.Script, assay.UngScript, assay.VoltammetryScript
                    });
            }

            // Set the supporting scripts
            SetChildScripts(IAssays.SupportingScripts);
        }

        /// <summary>
        /// Set child scripts
        /// </summary>
        /// <param name="client">The network client</param>
        /// <param name="script">The parent script</param>
        private static void SetChildScripts(IEnumerable<string> scripts)
        {
            // Loop through the children
            foreach (var name in scripts)
            {
                // Get the script object
                var childScript = IScripts.Instance.Where(
                    x => string.Compare(x.Name, name, true) == 0).First();

                // Check the loaded flag
                if (childScript.Loaded == false)
                {
                    // Do not write supporting scritps unless the flag is set
                    if (Main.Instance.EditSupportingScripts ||
                        (IAssays.SupportingScripts.Contains(childScript.Name,
                        StringComparer.CurrentCultureIgnoreCase) == false))
                    {
                        // Set the script on the reader
                        IClient.Instance.SetScript(childScript.Name, childScript.Value);
                    }

                    childScript.Loaded = true;
                }

                // Reccursively set the child scripts for this script
                SetChildScripts(childScript.Children);
            }
        }

        /// <summary>
        /// Load data from storage
        /// </summary>
        private void Load()
        {
            // Reload the assays
            IAssays.Instance = Deserialise(IAssays.Instance, 
                ILocalFileSystem.Instance.ReadTextFile("Assays"), IAssays.Instance.Types);

            // Reload the scripts
            IScripts.Instance.Reload();

            // Reload the default metrics
            IDefaultMetrics.Instance.Value = ILocalFileSystem.Instance.ReadTextFile(
                "Metrics\\DefaultMetrics");
            IDefaultMetrics.Instance.Loaded = false;
            IDefaultMetrics.Instance.Modified = false;

            // Reload the metrics
            foreach (var assay in IAssays.Instance)
            {
                assay.Metrics.Value = ILocalFileSystem.Instance.ReadTextFile("Metrics\\" + 
                    assay.Metrics.Name);
                assay.Metrics.Loaded = false;
                assay.Metrics.Modified = false;
            }
        }

        /// <summary>
        /// Save any modified data
        /// </summary>
        private void Save()
        {
            // Loop through the assays
            foreach (var assay in IAssays.Instance)
            {
                // Clear the modified flag
                assay.Modified = false;

                // Create a string builder to store all of the data
                var subStringBuilder = new StringBuilder();

                // Append the metrics
                subStringBuilder.Append(assay.Metrics.Value);

                // Initialise a hash of processed scripts
                var processedScripts = new HashSet<string>();

                // Concatenate the assay scripts and their children
                ConcatenateScripts(assay.Script, subStringBuilder, processedScripts);
                ConcatenateScripts(assay.UngScript, subStringBuilder, processedScripts);
                ConcatenateScripts(assay.VoltammetryScript, subStringBuilder, processedScripts);

                // Concatenate the supporting scripts and their children
                foreach (var scriptName in IAssays.SupportingScripts)
                {
                    // Install the script
                    ConcatenateScripts(scriptName, subStringBuilder, processedScripts);
                }

                // Create a hasher
                using (var hasher = SHA1Managed.Create())
                {
                    // Hash the data
                    byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(subStringBuilder.ToString()));

                    // Set the assay signature using the client's private key
                    assay.Signature = Convert.ToBase64String(
                        rsaCryptoServiceProvider.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
                }
            }

            // Get the assays and append to the string builder
            var assayXml = Serialise(IAssays.Instance, IAssays.Instance.Types);

            // Create a string builder to store all of the data
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(assayXml);

            // Write the assays to file
            ILocalFileSystem.Instance.WriteTextFile("Assays", assayXml);

            // Get the default metrics and append to the string builder
            var defaultMetrics = IDefaultMetrics.Instance.Value;

            stringBuilder.Append(defaultMetrics);

            // Write the default metrics
            ILocalFileSystem.Instance.WriteTextFile("Metrics\\DefaultMetrics", defaultMetrics);
            IDefaultMetrics.Instance.Modified = false;

            // Create a hasher
            using (var hasher = SHA1Managed.Create())
            {
                // Hash the data
                byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));

                // Write the signature using the client's private key
                ILocalFileSystem.Instance.WriteTextFile("Signature", Convert.ToBase64String(
                    rsaCryptoServiceProvider.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"))));
            }

            // Loop through all metrics
            foreach (var metric in IAssays.Instance.Select(x => x.Metrics).Distinct())
            {
                // Get the metrics and append to the string builder
                var metricData = metric.Value;

                // Write the metric to file
                ILocalFileSystem.Instance.WriteTextFile("Metrics\\" + metric.Name, metricData);
                metric.Modified = false;
            }

            // Loop through all scripts
            foreach (var script in IScripts.Instance)
            {
                // Write the script to file
                ILocalFileSystem.Instance.WriteTextFile("Scripts\\" + script.Name, script.Value);
                script.Modified = false;
            }

            Main.Instance.Modified = false;
        }

        /// <summary>
        /// Save any modified data
        /// </summary>
        /// <param name="folder">The folder to export into</param>
        private void Export(string folder)
        {
            // Get the file system for the folder
            var fileSystem = ILocalFileSystem.Instance.GetSimpleFileSystem(folder +
                Path.DirectorySeparatorChar);

            // All exported scripts
            var allExportedScripts = new HashSet<string>();

            // Loop through the assays
            foreach (var assay in IAssays.Instance)
            {
                // Create a string builder to store all of the data
                var subStringBuilder = new StringBuilder();

                // Append the metrics
                subStringBuilder.Append(assay.Metrics.Value);

                // Initialise a hash of processed scripts
                var processedScripts = new HashSet<string>();

                // Concatenate the assay scripts and their children
                ConcatenateScripts(assay.Script, subStringBuilder, processedScripts);
                ConcatenateScripts(assay.UngScript, subStringBuilder, processedScripts);
                ConcatenateScripts(assay.VoltammetryScript, subStringBuilder, processedScripts);

                // Concatenate the supporting scripts and their children
                foreach (var scriptName in IAssays.SupportingScripts)
                {
                    // Concatenate the supporting scripts
                    ConcatenateScripts(scriptName, subStringBuilder, processedScripts);
                }

                // Add the processed scripts into the exported scripts
                allExportedScripts.UnionWith(processedScripts);

                // Create a hasher
                using (var hasher = SHA1Managed.Create())
                {
                    // Hash the data
                    byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(subStringBuilder.ToString()));

                    // Set the assay signature using the client's private key
                    assay.Signature = Convert.ToBase64String(
                        rsaCryptoServiceProvider.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
                }
            }

            // Create a string builder to store all of the data
            var stringBuilder = new StringBuilder();

            // Get the assays and append to the string builder
            var assayXml = Serialise(IAssays.Instance, IAssays.Instance.Types);

            stringBuilder.Append(assayXml);

            // Write the assays to file
            fileSystem.WriteTextFile("Assays", assayXml);

            // Get the default metrics and append to the string builder
            var defaultMetrics = IDefaultMetrics.Instance.Value;

            stringBuilder.Append(defaultMetrics);

            // Write the default metrics
            fileSystem.WriteTextFile("Metrics\\DefaultMetrics", defaultMetrics);

            // Create a hasher
            using (var hasher = SHA1Managed.Create())
            {
                // Hash the data
                byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));

                // Write the signature using the client's private key
                fileSystem.WriteTextFile("Signature", Convert.ToBase64String(
                    rsaCryptoServiceProvider.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"))));
            }

            // Loop through all metrics
            foreach (var metric in IAssays.Instance.Select(x => x.Metrics).Distinct())
            {
                // Get the metrics
                var metricData = metric.Value;

                // Write the metric to file
                fileSystem.WriteTextFile("Metrics\\" + metric.Name, metricData);
            }

            // Loop through all exported scripts
            foreach (var script in IScripts.Instance.Where(x => allExportedScripts.Contains(x.Name)))
            {
                // Write the script to file
                fileSystem.WriteTextFile("Scripts\\" + script.Name, script.Value);
            }
        }

        /// <summary>
        /// Concatenate a script and its children reccursively
        /// </summary>
        /// <param name="scriptName">The script name</param>
        /// <param name="stringBuilder">The string builder</param>
        /// <param name="processedScripts">Hash of scripts already processed</param>
        private void ConcatenateScripts(string scriptName, StringBuilder stringBuilder, 
            HashSet<string> processedScripts)
        {
            // Check for loaded scripts
            if (processedScripts.Contains(scriptName, StringComparer.CurrentCultureIgnoreCase) == false)
            {
                // Get the named script
                var script = IScripts.Instance.Where(
                    x => string.Compare(x.Name, scriptName, true) == 0).FirstOrDefault();

                if (script != null)
                {
                    // Add this script to the loaded scripts
                    processedScripts.Add(scriptName);

                    // Append the script to the string builder
                    stringBuilder.Append(script.Value);

                    // Loop through the child scripts
                    foreach (var childScriptName in script.Children)
                    {
                        ConcatenateScripts(childScriptName, stringBuilder, processedScripts);
                    }
                }
            }
        }
    }
}
