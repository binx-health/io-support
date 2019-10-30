/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Net.Security;
using System.Net.NetworkInformation;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IO.Scripting;

namespace IO.Comms.Network
{
    /// <summary>
    /// Network client class
    /// </summary>
    public class Client : IClient, IDisposable
    {
        /// <summary>
        /// The read buffer size
        /// </summary>
        private static readonly int READ_BUFFER_SIZE = 4096;

        /// <summary>
        /// The synchronous message timeout
        /// </summary>
        private static readonly int SYNCHRONOUS_MESSAGE_TIMEOUT = 5000;

        /// <summary>
        /// The client certificate
        /// </summary>
        private static X509Certificate clientCertificate = null;

        /// <summary>
        /// Connected flag
        /// </summary>
        private bool connected;

        /// <summary>
        /// The TCP client object
        /// </summary>
        private TcpClient tcpClient = null;

        /// <summary>
        /// The SSL stream object
        /// </summary>
        private SslStream sslStream = null;

        /// <summary>
        /// The TCP listener thread
        /// </summary>
        private Thread tcpListenerThread = null;

        /// <summary>
        /// The discovery listener thread
        /// </summary>
        private Thread discoveryThread = null;

        /// <summary>
        /// The method requiring a synchronous response
        /// </summary>
        private string synchronousMethod = string.Empty;

        /// <summary>
        /// The synchronous response message queue
        /// </summary>
        private IO.MessageQueue.MessageQueue<string> synchronousResponseMessageQueue =
            new IO.MessageQueue.MessageQueue<string>();

        /// <summary>
        /// List of dicovered devices
        /// </summary>
        private Dictionary<string, string> devices = new Dictionary<string, string>();

        /// <summary>
        /// List of discovered devices
        /// </summary>
        public override Dictionary<string, string> Devices
        {
            get
            {
                lock (devices)
                {
                    return new Dictionary<string, string>(devices);
                }
            }
        }

        /// <summary>
        /// Connected flag
        /// </summary>
        public override bool Connected
        {
            get
            {
                return connected;
            }
        }

        /// <summary>
        /// Initialise the client for use
        /// </summary>
        public override void Initialise()
        {
            // Check for a null client certificate
            if (clientCertificate == null)
            {
                // Open the root certificates
                var serverRootCertificate = new X509Certificate2("IoServerRoot.pfx", "App1ause",
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                var clientRootCertificate = new X509Certificate2("IoClientRoot.pfx", "App1ause",
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                // Open the trusted root certificates store
                var store = new X509Store(StoreName.AuthRoot, StoreLocation.LocalMachine);

                // Open the store for read only
                store.Open(OpenFlags.ReadWrite);

                if (store.Certificates.Contains(clientRootCertificate) == false)
                {
                    // Add the certificate to the store and close it
                    store.Add(clientRootCertificate);

                    // Delete the existing client certificate file
                    File.Delete("IoClient.cer");
                }

                if (store.Certificates.Contains(serverRootCertificate) == false)
                {
                    // Add the certificate to the store and close it
                    store.Add(serverRootCertificate);
                }

                store.Close();

                try
                {
                    // Try to load the client certificate
                    clientCertificate = new X509Certificate("IoClient.cer");

                    // Opent the personal certificates store
                    store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

                    // Open the store for read only
                    store.Open(OpenFlags.ReadOnly);

                    // Check for a certificate
                    if (store.Certificates.Contains(clientCertificate) == false)
                    {
                        // Close the store
                        store.Close();

                        // Delete the existing certificate file
                        File.Delete("IoClient.cer");

                        // The certificate is not present so recreate it
                        throw new CryptographicException();
                    }

                    // Close the store
                    store.Close();
                }
                catch (CryptographicException)
                {
                    // Create a new process to generate the client identity certificate
                    var process = new System.Diagnostics.Process();

                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "IoClientMake.bat";
                    process.Start();
                    process.WaitForExit();

                    // Reload the certificate from the file system
                    clientCertificate = new X509Certificate("IoClient.cer");
                }
            }

            // Create a new UDP listener
            var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 11000));

            // Start listening
            udpClient.BeginReceive(new AsyncCallback(UdpCallback), udpClient);
        }

        /// <summary>
        /// Discover at least one device on the network
        /// </summary>
        public override void Discover()
        {
            // Clear the list of devices
            lock (devices)
            {
                devices.Clear();
            }

            // Abort any current discovery thread
            if (discoveryThread != null)
            {
                discoveryThread.Abort();
            }

            // Creat the discovery thread
            discoveryThread = new Thread(DiscoveryThreadProcedure);
            discoveryThread.Start();
        }

        /// <summary>
        /// Connect to the reader
        /// </summary>
        /// <param name="uri">The uri for the reader</param>
        /// <param name="port">The port</param>
        public override void Connect(string uri, int port)
        {
            // Disconnect first
            Disconnect();

            try
            {
                // Create a tcp client
                tcpClient = new TcpClient(uri, port);

                // Create am SSL stream
                sslStream = new System.Net.Security.SslStream(tcpClient.GetStream(), false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    new LocalCertificateSelectionCallback(LocalCertificateSelection));

                // Authenticate with the server
                sslStream.AuthenticateAsClient("IO Server");

                // Create a new TCP listener thread
                tcpListenerThread = new Thread(TcpListenerThreadProcedure);

                // Set the connected flag
                connected = true;

                // Start the thread
                tcpListenerThread.Start();
            }
            catch (Exception e)
            {
                // Disconnect
                Disconnect();

                // Rethrow
                throw e;
            }
        }

        /// <summary>
        /// Disconnect from the reader
        /// </summary>
        /// <param name="undiscover">Undiscover any readers</param>
        public override void Disconnect(bool undiscover = false)
        {
            // Abort any current discovery thread
            if (discoveryThread != null)
            {
                discoveryThread.Abort();
                discoveryThread = null;
            }

            if (undiscover)
            {
                // Clear the list of devices
                lock (devices)
                {
                    devices.Clear();
                }
            }

            // Stop any listener thread
            if (tcpListenerThread != null)
            {
                tcpListenerThread.Abort();
                tcpListenerThread = null;
            }

            // If we have an SSL stream then dispose it
            if (sslStream != null)
            {
                sslStream.Dispose();
                sslStream = null;
            }

            // If we have an SSL stream then dispose it
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }

            // Clear the connected flag
            connected = false;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            // Disconnect
            Disconnect();
        }

        /// <summary>
        /// Call GetPhase on the reader
        /// </summary>
        /// <returns>The phase</returns>
        public override string GetPhase()
        {
            return HttpUtility.HtmlDecode(InvokeReaderMethod("GetPhase"));
        }

        /// <summary>
        /// Call GetConfiguration on the reader
        /// </summary>
        /// <returns>The serialised configuration</returns>
        public override string GetConfiguration()
        {
            return InvokeReaderMethod("GetConfiguration");
        }

        /// <summary>
        /// Call GetAssays on the reader
        /// </summary>
        /// <returns>The serialised assays</returns>
        public override string GetAssays()
        {
            return InvokeReaderMethod("GetAssays");
        }

        /// <summary>
        /// Call GetScript on the reader
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <returns>The script in plain text</returns>
        public override string GetScript(string name)
        {
            return HttpUtility.HtmlDecode(InvokeReaderMethod("GetScript", new Dictionary<string, string>() 
            { 
                { "name", SecurityElement.Escape(name) } 
            }));
        }

        /// <summary>
        /// Call GetDefaultMetrics on the reader
        /// </summary>
        /// <returns>The default metrics in plain text</returns>
        public override string GetDefaultMetrics()
        {
            return HttpUtility.HtmlDecode(InvokeReaderMethod("GetDefaultMetrics"));
        }

        /// <summary>
        /// Call GetMetrics on the reader
        /// </summary>
        /// <param name="name">The name of the metrics</param>
        /// <returns>The metrics in plain text</returns>
        public override string GetMetrics(string name)
        {
            return HttpUtility.HtmlDecode(InvokeReaderMethod("GetMetrics", new Dictionary<string, string>() 
            { 
                { "name", SecurityElement.Escape(name) } 
            }));
        }

        /// <summary>
        /// Call GetDeviceValues on the reader
        /// </summary>
        /// <returns>The serialised device values</returns>
        public override string GetDeviceValues()
        {
            return HttpUtility.HtmlDecode(InvokeReaderMethod("GetDeviceValues"));
        }

        /// <summary>
        /// Call GetAssays on the reader
        /// </summary>
        /// <returns>The serialised test data</returns>
        public override string GetTestData()
        {
            return InvokeReaderMethod("GetTestData");
        }

        /// <summary>
        /// Call SetConfiguration on the reader
        /// </summary>
        /// <param name="value">The serialised configuration</param>
        public override void SetConfiguration(string value)
        {
            InvokeReaderMethod("SetConfiguration", new Dictionary<string, string>() { { "value", value } });
        }

        /// <summary>
        /// Call SetAssays on the reader
        /// </summary>
        /// <param name="value">The serialised assays</param>
        public override void SetAssays(string value)
        {
            InvokeReaderMethod("SetAssays", new Dictionary<string, string>() { { "value", value } });
        }

        /// <summary>
        /// Call SetScript on the reader
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <param name="value">The script in plain text</param>
        public override void SetScript(string name, string value)
        {
            InvokeReaderMethod("SetScript", new Dictionary<string, string>() 
            { 
                { "name", SecurityElement.Escape(name) },
                { "value", SecurityElement.Escape(value) },
            });
        }

        /// <summary>
        /// Call SetDefaultMetrics on the reader
        /// </summary>
        /// <param name="value">The default metrics in plain text</param>
        public override void SetDefaultMetrics(string value)
        {
            InvokeReaderMethod("SetDefaultMetrics", new Dictionary<string, string>() 
            { 
                { "value", SecurityElement.Escape(value) } 
            });
        }

        /// <summary>
        /// Call SetMetrics on the reader
        /// </summary>
        /// <param name="name">The name of the metrics</param>
        /// <param name="value">The metrics in plain text</param>
        public override void SetMetrics(string name, string value)
        {
            InvokeReaderMethod("SetMetrics", new Dictionary<string, string>() 
            { 
                { "name", SecurityElement.Escape(name) },
                { "value", SecurityElement.Escape(value) },
            });
        }

        /// <summary>
        /// Call ExecuteScript on the reader
        /// </summary>
        /// <param name="name">The name of the script</param>
        public override void ExecuteScript(string name)
        {
            InvokeReaderMethod("ExecuteScript", new Dictionary<string, string>() 
            { 
                { "name", SecurityElement.Escape(name) } 
            });
        }

        /// <summary>
        /// Call ExecuteCommand on the reader
        /// </summary>
        /// <param name="command">The command in plain text</param>
        public override void ExecuteCommand(string command)
        {
            InvokeReaderMethod("ExecuteCommand", new Dictionary<string, string>() 
            { 
                { "command", SecurityElement.Escape(command) } 
            });
        }

        /// <summary>
        /// Call AbortScript on the reader
        /// </summary>
        public override void AbortScript()
        {
            InvokeReaderMethod("AbortScript");
        }

        /// <summary>
        /// Call ResetFirmware on the reader
        /// </summary>
        public void ResetFirmware()
        {
            InvokeReaderMethod("ResetFirmware");
        }

        /// <summary>
        /// Call ResetReader on the reader
        /// </summary>
        public override void ResetReader()
        {
            InvokeReaderMethod("ResetReader");
        }

        /// <summary>
        /// Call LaunchCommandPrompt on the reader
        /// </summary>
        public override void LaunchCommandPrompt()
        {
            InvokeReaderMethod("LaunchCommandPrompt");
        }

        /// <summary>
        /// Call GetAllDeviceValues on the reader
        /// </summary>
        /// <returns>The serialised device values</returns>
        public string GetAllDeviceValues()
        {
            return InvokeReaderMethod("GetAllDeviceValues");
        }

        /// <summary>
        /// Call GetDeviceValue on the reader
        /// </summary>
        /// <param name="name">The name of the device</param>
        /// <returns>The device value in plain text</returns>
        public int GetDeviceValue(string name)
        {
            return int.Parse(HttpUtility.HtmlDecode(InvokeReaderMethod("GetDeviceValue", 
                new Dictionary<string, string>() 
            { 
                { "name", SecurityElement.Escape(name) } 
            })));
        }

        /// <summary>
        /// Call AddCertificate on the reader
        /// </summary>
        /// <param name="path"></param>
        public void AddCertificate(string path)
        {
            // Initialise the encoded certificate value
            string encodedCertificate;

            // Open the certificate file
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                // Alloacate and read the buffer
                var buffer = new byte[fileStream.Length];

                fileStream.Read(buffer, 0, (int)fileStream.Length);

                // Encode the buffer
                encodedCertificate = Convert.ToBase64String(buffer);
            }

            // Invoke the method on the server
            InvokeReaderMethod("AddCertificate", new Dictionary<string, string>() 
            { 
                { "value", SecurityElement.Escape(encodedCertificate) } 
            });
        }

        /// <summary>
        /// Broadcast a UDP message on a specific port
        /// </summary>
        /// <param name="buffer">The message buffer</param>
        /// <param name="port">The port</param>
        private void Broadcast(byte[] buffer, int port)
        {
            // Initialis a list of broadcast addresses
            var broadcastAddresses = new HashSet<string>();

            // Loop through the network interfaces
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces().Where(
                x => x.OperationalStatus == OperationalStatus.Up))
            {
                var ipProperties = networkInterface.GetIPProperties();

                // Loop through the IPv4 unicast IP addresses for this interface
                foreach (var o in ipProperties.UnicastAddresses.Where(
                    x => (x.Address.AddressFamily == AddressFamily.InterNetwork) && 
                        (x.IPv4Mask != null)))
                {
                    // Calculate the broadcast address
                    byte[] address = o.Address.GetAddressBytes();
                    byte[] mask = o.IPv4Mask.GetAddressBytes();
                    string broadcast = string.Format("{0}.{1}.{2}.{3}",
                        (byte)(address[0] | ~mask[0]),
                        (byte)(address[1] | ~mask[1]),
                        (byte)(address[2] | ~mask[2]),
                        (byte)(address[3] | ~mask[3]));

                    // Add the broadcast address to the list
                    broadcastAddresses.Add(broadcast);
                }
            }

            // Broadcast the message to each unique address
            foreach (var broadcast in broadcastAddresses)
            {
                using (var oUdpClient = new UdpClient() { EnableBroadcast = true })
                {
                    oUdpClient.Send(buffer, buffer.Length, broadcast, port);
                    oUdpClient.Close();
                }
            }
        }

        /// <summary>
        /// Static callback function for a UDP message
        /// </summary>
        /// <param name="oAsyncResult"></param>
        private void UdpCallback(IAsyncResult oAsyncResult)
        {
            // Initialise the client and an IP end point
            var udpClient = oAsyncResult.AsyncState as UdpClient;
            IPEndPoint ipEndPoint = null;

            try
            {
                // Get the message and reset the listener
                var buffer = udpClient.EndReceive(oAsyncResult, ref ipEndPoint);
                udpClient.BeginReceive(new AsyncCallback(UdpCallback), udpClient);

                string deviceName = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                byte[] address = ipEndPoint.Address.GetAddressBytes();
                string uri = string.Format("{0}.{1}.{2}.{3}", address[0], address[1], address[2], 
                    address[3]);

                // Add this to the list of devices
                devices.Add(uri, deviceName);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Invoke a reader method
        /// </summary>
        /// <param name="name">The method name</param>
        /// <param name="parameters">The method parameters as dictionary</param>
        /// <returns></returns>
        private string InvokeReaderMethod(string name, Dictionary<string, string> parameters = null)
        {
            // Check for a reader connection
            if ((sslStream == null) || (Connected == false))
            {
                // The server never responded
                throw new ApplicationException("No server connection");
            }

            // Set the synchronous response name
            lock (synchronousMethod)
            {
                synchronousMethod = name;
            }

            // Create an XML writer for the soap envelope
            using (var xmlWriter = XmlWriter.Create(sslStream, new XmlWriterSettings()
            {
                CloseOutput = false,
                Encoding = new UTF8Encoding(false),
            }))
            {
                // Write the SOAP message
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("soap", "Envelope", "http://www.w3.org/2003/05/soap-envelope");
                xmlWriter.WriteElementString("Header", "http://www.w3.org/2003/05/soap-envelope", null);
                xmlWriter.WriteStartElement("Body", "http://www.w3.org/2003/05/soap-envelope");
                xmlWriter.WriteStartElement("m", name, "http://www.AtlasGenetics.com/IO");

                // If we have parameters then write them
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        xmlWriter.WriteStartElement(parameter.Key,
                            "http://www.AtlasGenetics.com/IO");
                        xmlWriter.WriteRaw(parameter.Value);
                        xmlWriter.WriteEndElement();
                    }
                }
            }

            // Wait for the response form the server
            if (WaitHandle.WaitAny(new WaitHandle[] { synchronousResponseMessageQueue },
                SYNCHRONOUS_MESSAGE_TIMEOUT) != WaitHandle.WaitTimeout)
            {
                if (synchronousMethod == string.Empty)
                {
                    // A valid response was received
                    return synchronousResponseMessageQueue.Pop();
                }
                else
                {
                    // An error was received
                    throw new ApplicationException("Server response error " + 
                        synchronousResponseMessageQueue.Pop());
                }
            }

            // Clear the synchronous response name
            lock (synchronousMethod)
            {
                synchronousMethod = string.Empty;
            }

            // The server never responded
            throw new ApplicationException("Server response timeout");
        }

        /// <summary>
        /// Discovery thread procedure
        /// </summary>
        private void DiscoveryThreadProcedure()
        {
            // The number of devices discovered
            int devicesFound = 0;

            // Run until devices are discovered
            while (true)
            {
                // Broadcast
                Broadcast(Encoding.UTF8.GetBytes("IO"), 11001);

                // Wait for responses
                Thread.Sleep(1000);

                // Check for devices
                lock (devices)
                {
                    // Check for new devices
                    if (devices.Count > devicesFound)
                    {
                        devicesFound = devices.Count;

                        // Create an XML writer for the soap envelope
                        var stringBuilder = new StringBuilder();

                        using (var xmlWriter = XmlWriter.Create(stringBuilder))
                        {
                            // Write the SOAP message
                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("soap", "Envelope", 
                                "http://www.w3.org/2003/05/soap-envelope");
                            xmlWriter.WriteElementString("Header", 
                                "http://www.w3.org/2003/05/soap-envelope", null);
                            xmlWriter.WriteStartElement("Body", "http://www.w3.org/2003/05/soap-envelope");
                            xmlWriter.WriteStartElement("m", "NotifyDiscovery", 
                                "http://www.AtlasGenetics.com/IO");
                        }

                        // Push the message onto the queue
                        IO.Comms.Network.MessageQueue.Instance.Push(stringBuilder.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// TCP listener thread procedure
        /// </summary>
        private void TcpListenerThreadProcedure()
        {
            // Initialise a buffer and bytes read
            var buffer = new byte[READ_BUFFER_SIZE];
            int read;

            // Initialise a message string builder
            StringBuilder messageBuilder = null;

            // Create an XML document and namespace manager
            var xmlDocument = new XmlDocument() { XmlResolver = null };
            var xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);

            // Add the namespaces
            xmlNamespaceManager.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            xmlNamespaceManager.AddNamespace("m", "http://www.AtlasGenetics.com/IO");

            // Loop until the thread is aborted
            while (true)
            {
                try
                {
                    // Read from the stream
                    if ((read = sslStream.Read(buffer, 0, READ_BUFFER_SIZE)) > 0)
                    {
                        // Get the message fragment
                        var fragment = Encoding.UTF8.GetString(buffer, 0, read);

                        // Check for an XML declaration
                        if (fragment.StartsWith("<?xml"))
                        {
                            // Create a new message builder for this message
                            messageBuilder = new StringBuilder();
                        }

                        if (messageBuilder == null)
                        {
                            // Bad data
                            break;
                        }
                        else
                        {
                            // Append this fragment to the message
                            messageBuilder.Append(fragment);

                            try
                            {
                                // Get the message as a string
                                string message = messageBuilder.ToString();

                                // Check for valid XML
                                xmlDocument.LoadXml(message);

                                // The XML is valid so dereference the message builder
                                messageBuilder = null;

                                // Get the method node
                                var methodNode = xmlDocument.SelectSingleNode("soap:Envelope/soap:Body/*[1]",
                                    xmlNamespaceManager);

                                lock (synchronousMethod)
                                {
                                    if (methodNode.LocalName == synchronousMethod)
                                    {
                                        // This is a synchronous method response so clear the method and push
                                        // the value onto the queue
                                        synchronousMethod = string.Empty;

                                        // Check for a returned value and push this or null onto the queue
                                        if ((methodNode.FirstChild != null) &&
                                            (methodNode.FirstChild.LocalName == "value"))
                                        {
                                            synchronousResponseMessageQueue.Push(
                                                methodNode.FirstChild.InnerXml);
                                        }
                                        else
                                        {
                                            synchronousResponseMessageQueue.Push(null);
                                        }
                                    }
                                    else if ((string.IsNullOrEmpty(synchronousMethod) == false) &&
                                        (methodNode.LocalName == "Fault") &&
                                        (methodNode.FirstChild != null) &&
                                        (methodNode.FirstChild.LocalName == "faultcode") &&
                                        (methodNode.FirstChild.NextSibling != null) &&
                                        (methodNode.FirstChild.NextSibling.LocalName == "faultstring"))
                                    {
                                        // This is a fault code so push it onto the queue
                                        synchronousResponseMessageQueue.Push(
                                            methodNode.FirstChild.InnerText + " " +
                                            methodNode.FirstChild.NextSibling.InnerText);
                                    }
                                    else
                                    {
                                        // Push the message onto the queue
                                        IO.Comms.Network.MessageQueue.Instance.Push(message);
                                    }
                                }
                            }
                            catch (XmlException)
                            {
                                // The XML is not valid
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Something bad happened so start listening again
                }
            }

            // Clear the connected flag
            connected = false;
        }

        /// <summary>
        /// Static server certificate authentication
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="certificate">The server certificate</param>
        /// <param name="chain">The certificate chain</param>
        /// <param name="sslPolicyErrors">The policy errors</param>
        /// <returns>True if the server certificate is valid, false otherwise</returns>
        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // Check for a valid certificate
            return true;
//            return (sslPolicyErrors == SslPolicyErrors.None);
        }

        /// <summary>
        /// Static local certificate selection
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="targetHost">The target host</param>
        /// <param name="localCertificates">List of local certificates</param>
        /// <param name="remoteCertificate">The server certificate</param>
        /// <param name="acceptableIssuers">The array of acceptable issuers</param>
        /// <returns>The one and only client certificate</returns>
        private static X509Certificate LocalCertificateSelection(
            object sender,
            string targetHost,
            X509CertificateCollection localCertificates,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers)
        {
            // Check for a valid host
            if (targetHost == "IO Server")
            {
                return clientCertificate;
            }

            return null;
        }
    }
}
