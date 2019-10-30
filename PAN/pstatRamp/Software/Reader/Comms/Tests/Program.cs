/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using IO.Model.Serializable;
using IO.Scripting;

namespace IO.Comms.Tests
{
    /// <summary>
    /// The test program
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Initialise the configuration object
            IConfiguration.Instance = new Configuration() { DeveloperPort = 8080 };

            // Initialise the assays
            IAssays.Instance = new Assays()
            {
                new Assay() { Name = "C.Trachomatis", ShortName = "CT", ScriptName = "CT", MetricsName = "CT" },
            };

            // Create a firmware manager object
            Firmware.IManager.Instance = new Firmware.Manager();

            try
            {
                // Initialise the firmware manager
                Firmware.IManager.Instance.Initialise();
                Firmware.IManager.Instance.SetScript("Test", "# Test");
                Firmware.IManager.Instance.ExecuteScript("Test");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Create a network manager object
            Network.IManager.Instance = new Network.Manager();

            // Initialise the network manager
            Network.IManager.Instance.Initialise();

            // Initialise the message queue wait handles
            var waitHandles = new WaitHandle[] { Network.MessageQueue.Instance,
                Firmware.MessageQueue.Instance };

            // Create an XML document and namespace manager
            var xmlDocument = new XmlDocument();
            var xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);

            // Add the namespaces
            xmlNamespaceManager.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            xmlNamespaceManager.AddNamespace("m", "http://www.AtlasGenetics.com/IO");

            // Loop forever
            while (true)
            {
                // Wait on the handles
                int index = WaitHandle.WaitAny(waitHandles, 10000);

                // Wait for a message in the queue
                if (index == WaitHandle.WaitTimeout)
                {
                    Network.Manager.Instance.NotifyPhaseChange("Idle");
                }
                else if (index == 0)
                {
                    // Load the message into the XML document
                    xmlDocument.LoadXml(Network.MessageQueue.Instance.Pop());

                    // Get the method node
                    var methodNode = xmlDocument.SelectSingleNode("soap:Envelope/soap:Body/*[1]", xmlNamespaceManager);

                    // Pop the message
                    Console.WriteLine(methodNode.LocalName);

                    // Check the message method
                    if (methodNode.LocalName == "GetAssays")
                    {
                        // Creare a serialiser
                        var xmlSerialiser = new XmlSerializer(IAssays.Instance.GetType(),
                            IAssays.Instance.Types);

                        // Create a string builder
                        var stringBuilder = new StringBuilder();

                        // Create an XML writer to serialise the assays
                        using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
                            {
                                OmitXmlDeclaration = true,
                                Indent = false,
                            }))
                        {
                            // Serialise the assays
                            xmlSerialiser.Serialize(xmlWriter, IAssays.Instance);

                            // Send the response message
                            Network.Manager.Instance.RespondGetAssays(stringBuilder.ToString());
                        }
                    }
                }
                else if (index == 1)
                {
                    Console.WriteLine(Firmware.MessageQueue.Instance.Pop());
                }
            }
        }
    }
}
