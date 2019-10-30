/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml;

namespace Manufacturing
{
    /// <summary>
    /// Sample client software for manufacturing
    /// </summary>
    class Program
    {
        /// <summary>
        /// Console application main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Create a new network client
            IO.Comms.Network.IClient.Instance = new IO.Comms.Network.Client();

            // Initialise the client
            // This will attempt to install root certificates and create a client identity certificate
            // if they do not already exist
            IO.Comms.Network.IClient.Instance.Initialise();

            // Kick off the discovery process
            IO.Comms.Network.IClient.Instance.Discover();

            // Create an XML document and namespace manager
            var xmlDocument = new XmlDocument();
            var xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);

            // Add the namespaces
            xmlNamespaceManager.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            xmlNamespaceManager.AddNamespace("m", "http://www.AtlasGenetics.com/IO");

            // Wait on the network message queue
            while (IO.Comms.Network.MessageQueue.Instance.WaitOne())
            {
                // Keep reading messages from the queue
                string message;

                while ((message = IO.Comms.Network.MessageQueue.Instance.Pop()) != null)
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
                            if (ConnectToDevice())
                            {
                                ConfigureDevice();
                            }
                        }
                        // Check for log lines
                        else if ((methodNode.LocalName == "NotifyLogLines") &&
                                (methodNode.FirstChild.LocalName == "log"))
                        {
                            // Log lines passed in methodNode.FirstChild.InnerText
                        }
                        // Check for device values
                        else if ((methodNode.LocalName == "NotifyDeviceValues") &&
                                (methodNode.FirstChild.LocalName == "values"))
                        {
                            // Device values passed in methodNode.FirstChild.InnerText
                        }
                        // Check for phase change
                        else if ((methodNode.LocalName == "NotifyPhaseChange") &&
                                (methodNode.FirstChild.LocalName == "phase"))
                        {
                            // Phase passed in methodNode.FirstChild.InnerText
                        }
                        // Check for script completion
                        else if ((methodNode.LocalName == "NotifyScriptComplete") &&
                                (methodNode.FirstChild.LocalName == "name"))
                        {
                            // Completed script name passed in methodNode.FirstChild.InnerText
                        }
                        // Check for test data
                        else if (methodNode.LocalName == "NotifyTestData")
                        {
                            // New test data is available
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to connect to discovered devices
        /// </summary>
        /// <returns>True if a connection is made</returns>
        private static bool ConnectToDevice()
        {
            // Check that we are not already connected
            if (IO.Comms.Network.IClient.Instance.Connected)
            {
                return true;
            }

            // Loop through the discovered devices
            foreach (var device in IO.Comms.Network.IClient.Instance.Devices)
            {
                // Try to connect
                // Devices is a dictionary of device names keyed by IP address
                try
                {
                    IO.Comms.Network.IClient.Instance.Connect(device.Key, 443);

                    // The client is now connected to a device
                    Console.WriteLine("Connected to " + device.Value + " at " + device.Key);

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return false;
        }

        /// <summary>
        /// Configure the connected device
        /// </summary>
        private static void ConfigureDevice()
        {
            // Check that we are connected
            if (IO.Comms.Network.IClient.Instance.Connected == false)
            {
                throw new ApplicationException("No connection");
            }

            // Get the configuration
            // This is an XML document containing the configuration parameters defined in the specification
            // The serial number for the device is stored in the optional SerialNumber element (string)
            // An additional optional element called 'Manufacturing' can be added containing any arbitrary
            // manufacturing data
            var configuration = IO.Comms.Network.IClient.Instance.GetConfiguration();

            // Set the configuration
            IO.Comms.Network.IClient.Instance.SetConfiguration(configuration);
        }
    }
}
