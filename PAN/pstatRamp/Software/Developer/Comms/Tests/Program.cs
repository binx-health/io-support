/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Reflection;
using IO.Scripting;
using IO.FileSystem;

namespace IO.Comms.Tests
{
    /// <summary>
    /// The test program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Write the header message
            Console.WriteLine("IO Developer Application " + Assembly.GetExecutingAssembly().GetName().Version);

            // Create the default file system
            ILocalFileSystem.Instance = new DefaultFileSystem("Data\\");

            // Intialise the model
            IO.Model.Model.Initialise();

            // Initialise the scripting from the files
            IO.Scripting.Scripting.Initialise();

            using (var client = new Network.Client())
            {
                try
                {
                    // Create an event handle for a console line
                    var consoleMessageQueue = new MessageQueue.MessageQueue<string>();
                    var consoleLineReader = new Thread(ConsoleLineReader);

                    consoleLineReader.Start(consoleMessageQueue);

                    // Initialise the message queue wait handles
                    var waitHandles = new WaitHandle[] { Network.MessageQueue.Instance, consoleMessageQueue };

                    // Create an XML document and namespace manager
                    var xmlDocument = new XmlDocument();
                    var xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);

                    // Add the namespaces
                    xmlNamespaceManager.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
                    xmlNamespaceManager.AddNamespace("m", "http://www.AtlasGenetics.com/IO");

                    // Define the chars for tokenising
                    var tokenisingChars = new char[] { ' ', '\t' };

                    // Loop until exit is typed
                    while (true)
                    {
                        try
                        {
                            client.Discover();
                            client.Connect(client.Devices.First(), 443);

                            Console.WriteLine("Connected to " + client.Devices.First());

                            // Loop while the client is connected
                            while (client.Connected)
                            {
                                try
                                {
                                    // Wait for a message in the queue
                                    if (WaitHandle.WaitAny(waitHandles) == 0)
                                    {
                                        // Load the message into the XML document
                                        xmlDocument.LoadXml(Network.MessageQueue.Instance.Pop());

                                        // Get the method node
                                        var methodNode = xmlDocument.SelectSingleNode(
                                            "soap:Envelope/soap:Body/*[1]", xmlNamespaceManager);

                                        if (methodNode.LocalName == "NotifyLogLines")
                                        {
                                            if ((methodNode.FirstChild != null) &&
                                                (methodNode.FirstChild.LocalName == "log"))
                                            {
                                                // Write the log
                                                Console.Write(methodNode.FirstChild.InnerText);
                                            }
                                        }
                                        else if ((methodNode.LocalName == "NotifyDeviceValues") &&
                                                (methodNode.FirstChild.LocalName == "values"))
                                        {
                                            // Write the log
                                            Console.WriteLine(methodNode.FirstChild.InnerXml);
                                        }
                                        else
                                        {
                                            Console.WriteLine(methodNode.LocalName);
                                        }
                                    }
                                    else if (client.HandleConsoleMessage(consoleMessageQueue.Pop()) == false)
                                    {
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }

                            if (client.Connected)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Disconnected");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Handle a console message
        /// </summary>
        /// <param name="client">The client object</param>
        /// <param name="message">The message</param>
        /// <returns>False if the message was exit, true otherwise</returns>
        private static bool HandleConsoleMessage(this Network.Client client, string message)
        {
            // Tokenise the string
            string error;
            string[] tokens = message.Tokenise(out error).ToArray();

            if (tokens == null)
            {
                Console.WriteLine(error);
            }
            else if (tokens.Length > 0)
            {
                var command = tokens[0].ToLower();

                if (command == "exit")
                {
                    return false;
                }
                else if ((command == "get") && (tokens.Length == 2))
                {
                    var parameter = tokens[1].ToLower();

                    if (parameter == "files")
                    {
                        client.GetData();
                    }
                    else if (parameter == "values")
                    {
                        client.GetValues();
                    }
                    else
                    {
                        Console.WriteLine("Invalid command");
                        Console.WriteLine("Options are: get files, get values");
                    }
                }
                else if ((command == "set") && (tokens.Length == 1))
                {
                    client.SetData();
                }
                else if ((command == "write") && (tokens.Length == 1))
                {
                    WriteData();
                }
                else if ((command == "read") && (tokens.Length == 1))
                {
                    IO.Scripting.Scripting.Initialise();

                    Console.WriteLine("Data read");
                }
                else if ((command == "exec") && (tokens.Length == 2))
                {
                    client.ExecuteScript(tokens[1]);
                }
                else if ((command == "reset") && (tokens.Length == 2))
                {
                    var device = tokens[1].ToLower();

                    if (device == "reader")
                    {
                        client.ResetReader();
                        client.Disconnect();

                        Console.WriteLine("Reader reset");

                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid command");
                        Console.WriteLine("Options are: reset reader");
                    }
                }
                else if ((command == "add") && (tokens.Length == 2))
                {
                    if (Path.GetExtension(tokens[1]) == ".cer")
                    {
                        client.AddCertificate(tokens[1]);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid command");
                    Console.WriteLine(
                        "Options are: exit, get files, get values, set, write, read, exec {script}, reset reader, add {certificate}");
                }
            }

            return true;
        }

        /// <summary>
        /// Serialise an object into XML
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The XML as a string</returns>
        private static string Serialise<T>(this T obj, Type[] types = null)
        {
            // Create a string builder
            var stringBuilder = new StringBuilder();

            // Create the serialiser from the type
            var xmlSerializer = new XmlSerializer(obj.GetType(), (types != null) ? types : new Type[0]);

            // Create an XML writer to serialise the object as a fragment
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
        /// Deserialise an object from an XML fragment
        /// </summary>
        /// <typeparam name="T">The type of object required</typeparam>
        /// <param name="obj">Any existing object instance</param>
        /// <param name="fragment">The XML fragment</param>
        /// <param name="types">The additional types required for serialisation</param>
        /// <returns>The new object</returns>
        public static T DeserialiseFragment<T>(this T obj, string fragment, Type[] types = null)
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
        /// Get device values
        /// </summary>
        /// <param name="client"></param>
        private static void GetValues(this Network.Client client)
        {
            // Get the new values
            Model.Volatile.IDeviceValues.Instance = 
                Model.Volatile.IDeviceValues.Instance.DeserialiseFragment(client.GetAllDeviceValues(), null);

            // Write them to the console
            foreach (var value in Model.Volatile.IDeviceValues.Instance)
            {
                Console.WriteLine(value.Key + "=" + value.Value);
            }
        }

        /// <summary>
        /// Serialise all stored data, deleting anything already there
        /// </summary>
        private static void WriteData()
        {
            // Delete any existing files
            ILocalFileSystem.Instance.Clean();

            // Write the assays
            ILocalFileSystem.Instance.WriteTextFile("Assays", IAssays.Instance.Serialise(IAssays.Instance.Types));

            // Write the default metrics
            ILocalFileSystem.Instance.WriteTextFile("Metrics\\DefaultMetrics",
                IDefaultMetrics.Instance.Value);

            // Loop through all metrics
            foreach (var metric in IAssays.Instance.Select(x => x.Metrics).Distinct().ToArray())
            {
                // Write the script to file
                ILocalFileSystem.Instance.WriteTextFile("Metrics\\" + metric.Name, metric.Value);
            }

            // Loop through all scripts
            foreach (var script in IScripts.Instance)
            {
                // Write the script to file
                ILocalFileSystem.Instance.WriteTextFile("Scripts\\" + script.Name, script.Value);
            }

            Console.WriteLine("Data written");
        }

        /// <summary>
        /// Method to get the data from the reader
        /// </summary>
        /// <param name="client"></param>
        private static void SetData(this Network.Client client)
        {
            try
            {
                Console.WriteLine("Setting default metrics");

                // Set the default metrics
                client.SetDefaultMetrics(IDefaultMetrics.Instance.Value);

                Console.WriteLine("Setting assays");

                // Create a string builder
                var stringBuilder = new StringBuilder();

                // Create a serialiaser
                var xmlSerializer = new XmlSerializer(typeof(Assays),
                    new Type[] { typeof(Assay) });

                // Create an xml writer
                using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    Indent = false,
                }))
                {
                    xmlSerializer.Serialize(xmlWriter, IAssays.Instance);
                }

                // Set the serialised assays
                client.SetAssays(stringBuilder.ToString());

                // Get the metrics for the assays
                foreach (var metrics in IAssays.Instance.Select(x => x.Metrics).Distinct())
                {
                    Console.WriteLine("Setting metrics '" + metrics.Name + "'");

                    client.SetMetrics(metrics.Name, metrics.Value);
                }

                // Set all scripts to unloaded
                foreach (var script in IScripts.Instance)
                {
                    script.Loaded = false;
                }

                // Loop through the assays
                foreach (var assay in IAssays.Instance)
                {
                    client.SetChildScripts(new List<string>()
                    {
                        assay.Script, assay.UngScript, assay.VoltammetryScript
                    });
                }

                // Set the supporting scripts
                client.SetChildScripts(IAssays.SupportingScripts);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Set child scripts
        /// </summary>
        /// <param name="client">The network client</param>
        /// <param name="script">The parent script</param>
        private static void SetChildScripts(this Network.Client client, IEnumerable<string> scripts)
        {
            // Loop through the children
            foreach (var name in scripts)
            {
                var childScript = IScripts.Instance.Where(x => x.Name == name).First();

                if (childScript.Loaded == false)
                {
                    Console.WriteLine("Setting script '" + name + "'");

                    client.SetScript(childScript.Name, childScript.Value);

                    childScript.Loaded = true;

                    client.SetChildScripts(childScript.Children);
                }
            }
        }

        /// <summary>
        /// Method to get the data from the reader
        /// </summary>
        /// <param name="client"></param>
        private static void GetData(this Network.Client client)
        {
            try
            {
                // Get the default metrics
                IDefaultMetrics.Instance.Value = client.GetDefaultMetrics();

                Console.WriteLine("Got default metrics");

                // Get the assays
                var response = client.GetAssays();
                var xmlSerializer = new XmlSerializer(typeof(Assays),
                    new Type[] { typeof(Assay) });

                IAssays.Instance = (IAssays)xmlSerializer.Deserialize(new StringReader(response));

                Console.WriteLine("Got assays");

                // Clear the scripts
                IScripts.Instance.Clear();

                // Get the metrics for the assays
                foreach (var metrics in IAssays.Instance.Select(x => x.Metrics).Distinct())
                {
                    metrics.Value = client.GetMetrics(metrics.Name);

                    Console.WriteLine("Got metrics '" + metrics.Name + "'");
                }

                // Loop through the assays
                foreach (var assay in IAssays.Instance)
                {
                    client.GetChildScripts(new List<string>()
                    {
                        assay.Script, assay.UngScript, assay.VoltammetryScript
                    });
                }

                // Get the fixed scripts
                client.GetChildScripts(IAssays.SupportingScripts);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Get child scripts
        /// </summary>
        /// <param name="client">The network client</param>
        /// <param name="script">The parent script</param>
        private static void GetChildScripts(this Network.Client client, IEnumerable<string> scripts)
        {
            // Loop through the children
            foreach (var name in scripts)
            {
                if (IScripts.Instance.Exists(x => x.Name == name) == false)
                {
                    var response = client.GetScript(name);

                    Console.WriteLine("Got script '" + name + "'");

                    client.GetChildScripts(IScripts.Instance.SetScript(name, response).Children);
                }
            }
        }

        /// <summary>
        /// Console line reading thread procedure
        /// </summary>
        /// <param name="param">The console message queue</param>
        private static void ConsoleLineReader(object param)
        {
            // Get the message queue
            var consoleMessageQueue = param as MessageQueue.MessageQueue<string>;
            string line = "";

            // Loop forever
            while (line.ToLower().Trim() != "exit")
            {
                // Push messages onto the queue
                consoleMessageQueue.Push(line = Console.ReadLine());
            }
        }
    }
}
