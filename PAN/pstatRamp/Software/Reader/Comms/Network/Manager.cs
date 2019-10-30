/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.Comms.Network
{
    /// <summary>
    /// Network manager class
    /// </summary>
    public class Manager : IManager, IDisposable
    {
        /// <summary>
        /// The read buffer size
        /// </summary>
        private static readonly int READ_BUFFER_SIZE = 4096;

        /// <summary>
        /// TCP listener object
        /// </summary>
        private TcpListener listener = null;

        /// <summary>
        /// The listener thread
        /// </summary>
        private Thread listenerThread = null;

        /// <summary>
        /// The UDP client for discovery
        /// </summary>
        private UdpClient udpClient = null;

        /// <summary>
        /// The SSL stream for the current developer client
        /// </summary>
        private volatile SslStream sslStream = null;

        /// <summary>
        /// The server certificate
        /// </summary>
        private X509Certificate serverCertificate = null;

        /// <summary>
        /// Initialise the manager for use
        /// </summary>
        public override void Initialise()
        {
            "Loading server certificate".Log();

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
            }

            if (store.Certificates.Contains(serverRootCertificate) == false)
            {
                // Add the certificate to the store and close it
                store.Add(serverRootCertificate);

                // Delete the existing server certificate file
                File.Delete("IoServer.cer");
            }

            store.Close();

            try
            {
                // Try to load the server certificate
                serverCertificate = new X509Certificate("IoServer.cer");

                // Opent the personal certificates store
                store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

                // Open the store for read only
                store.Open(OpenFlags.ReadOnly);

                // Check for a certificate
                if (store.Certificates.Contains(serverCertificate) == false)
                {
                    // Close the store
                    store.Close();

                    // Delete the existing certificate file
                    File.Delete("IoServer.cer");

                    // The certificate is not present so recreate it
                    throw new CryptographicException();
                }

                // Close the store
                store.Close();
            }
            catch (CryptographicException)
            {
                // Create a new process to generate the server identity certificate
                var process = new System.Diagnostics.Process();

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "IoServerMake.bat";
                process.Start();
                process.WaitForExit();

                // Reload the certificate from the file system
                serverCertificate = new X509Certificate("IoServer.cer");
            }

            "Disposing".Log();

            // Dispose of the listener and listener thread
            Dispose();

            "Creating UDP discovery".Log();

            // Create a new UDP listener thread
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 11001));
            udpClient.BeginReceive(new AsyncCallback(UdpCallback), udpClient);

            "Creating TCP listner".Log();

            // Create a new TCP/IP listener
            listener = new TcpListener(IPAddress.Any, IConfiguration.Instance.DeveloperPort);

            "Starting to listen".Log();

            // Start listening
            listener.Start();

            "Creating network listener thread".Log();

            // Create a new listener thread
            listenerThread = new Thread(ListenerThreadProcedure);

            "Starting network listener thread".Log();

            // Start the thread
            listenerThread.Start();
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            // Lock this object and dispose of any SSL stream
            lock (this)
            {
                // Check for an SSL stream
                if (sslStream != null)
                {
                    // Close the stream
                    sslStream.Dispose();
                    sslStream = null;
                }
            }

            // Check for a listener
            if (listener != null)
            {
                // Get the socket from the listener
                var socket = listener.Server;

                // Clear the listener variable
                listener = null;

                // Close the socket
                socket.Close();
            }

            // Check for a running listener thread
            if (listenerThread != null)
            {
                // Abort it
                listenerThread.Abort();
                listenerThread = null;
            }

            // Close the UDP client
            if (udpClient != null)
            {
                // Close the client
                udpClient.Close();
                udpClient = null;
            }
        }

        /// <summary>
        /// Notify a developer client of new device values
        /// </summary>
        /// <param name="values">The device values XML</param>
        public override void NotifyDeviceValues(string values)
        {
            InvokeClientMethod("NotifyDeviceValues", new Dictionary<string, string>() 
                { { "values", values } });
        }

        /// <summary>
        /// Notify a developer client of new log lines
        /// </summary>
        /// <param name="log">The new log lines text</param>
        public override void NotifyLogLines(string log)
        {
            InvokeClientMethod("NotifyLogLines", new Dictionary<string, string>() 
                { { "log", SecurityElement.Escape(log) } });
        }

        /// <summary>
        /// Notify a developer client of a phase change
        /// </summary>
        /// <param name="phase">The new phase text</param>
        public override void NotifyPhaseChange(string phase)
        {
            InvokeClientMethod("NotifyPhaseChange", new Dictionary<string, string>() 
                { { "phase", SecurityElement.Escape(phase) } });
        }

        /// <summary>
        /// Notify a developer client of a completed script
        /// </summary>
        /// <param name="name">The script name</param>
        public override void NotifyScriptComplete(string name)
        {
            InvokeClientMethod("NotifyScriptComplete", new Dictionary<string, string>() 
                { { "name", SecurityElement.Escape(name) } });
        }

        /// <summary>
        /// Notify a developer client of new test data
        /// </summary>
        public override void NotifyTestData()
        {
            InvokeClientMethod("NotifyTestData");
        }

        /// <summary>
        /// Notify a developer client of new configuration
        /// </summary>
        public override void NotifyConfiguration()
        {
            InvokeClientMethod("NotifyConfiguration");
        }

        /// <summary>
        /// Respond to a get assays message from a developer client
        /// </summary>
        /// <param name="value">The assays as XML</param>
        public override void RespondError(string faultCode, string faultString)
        {
            // Lock this object to avoid the SSL stream being disposed of during the call
            lock (this)
            {
                // Check we have a valid SSL stream
                if (sslStream != null)
                {
                    // Write the soap message
                    using (var xmlWriter = XmlWriter.Create(sslStream, new XmlWriterSettings()
                    {
                        CloseOutput = false,
                        Encoding = new UTF8Encoding(false),
                    }))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("soap", "Envelope",
                            "http://www.w3.org/2003/05/soap-envelope");
                        xmlWriter.WriteElementString("Header",
                            "http://www.w3.org/2003/05/soap-envelope", null);
                        xmlWriter.WriteStartElement("Body", "http://www.w3.org/2003/05/soap-envelope");
                        xmlWriter.WriteStartElement("Fault", "http://www.w3.org/2003/05/soap-envelope");
                        xmlWriter.WriteElementString("faultcode", faultCode);
                        xmlWriter.WriteElementString("faultstring", faultString);
                    }
                }
            }
        }

        /// <summary>
        /// Respond to a get phase message from a developer client
        /// </summary>
        /// <param name="phase">The phase</param>
        public override void RespondGetPhase(string phase)
        {
            InvokeClientMethod("GetPhase", new Dictionary<string, string>() 
                { { "value", SecurityElement.Escape(phase) } });
        }

        /// <summary>
        /// Respond to a get configuration message from a developer client
        /// </summary>
        /// <param name="value">The configuration as XML</param>
        public override void RespondGetConfiguration(string value)
        {
            InvokeClientMethod("GetConfiguration", new Dictionary<string, string>() { { "value", value } });
        }

        /// <summary>
        /// Respond to a get assays message from a developer client
        /// </summary>
        /// <param name="value">The assays as XML</param>
        public override void RespondGetAssays(string value)
        {
            InvokeClientMethod("GetAssays", new Dictionary<string, string>() { { "value", value } });
        }

        /// <summary>
        /// Respond to a get script message from a developer client
        /// </summary>
        /// <param name="value">The script text</param>
        public override void RespondGetScript(string value)
        {
            InvokeClientMethod("GetScript", new Dictionary<string, string>() 
                { { "value", SecurityElement.Escape(value) } });
        }

        /// <summary>
        /// Respond to a get default metrics message from a developer client
        /// </summary>
        /// <param name="value">The default metrics text</param>
        public override void RespondGetDefaultMetrics(string value)
        {
            InvokeClientMethod("GetDefaultMetrics", new Dictionary<string, string>() 
                { { "value", SecurityElement.Escape(value) } });
        }

        /// <summary>
        /// Respond to a get metrics message from a developer client
        /// </summary>
        /// <param name="value">The metrics text</param>
        public override void RespondGetMetrics(string value)
        {
            InvokeClientMethod("GetMetrics", new Dictionary<string, string>()
                { { "value", SecurityElement.Escape(value) } });
        }

        /// <summary>
        /// Respond to a get device values message from a developer client
        /// </summary>
        /// <param name="value">The device values XML</param>
        public override void RespondGetDeviceValues(string value)
        {
            InvokeClientMethod("GetDeviceValues", new Dictionary<string, string>() { { "value", value } });
        }

        /// <summary>
        /// Respond to a get test data message from a developer client
        /// </summary>
        /// <param name="value">The test data XML</param>
        public override void RespondGetTestData(string value)
        {
            InvokeClientMethod("GetTestData", new Dictionary<string, string>() { { "value", value } });
        }

        /// <summary>
        /// Respond to a set configuration message from a developer client
        /// </summary>
        public override void RespondSetConfiguration()
        {
            InvokeClientMethod("SetConfiguration");
        }

        /// <summary>
        /// Respond to a set assays message from a developer client
        /// </summary>
        public override void RespondSetAssays()
        {
            InvokeClientMethod("SetAssays");
        }

        /// <summary>
        /// Respond to a set script message from a developer client
        /// </summary>
        public override void RespondSetScript()
        {
            InvokeClientMethod("SetScript");
        }

        /// <summary>
        /// Respond to a set default metrics message from a developer client
        /// </summary>
        public override void RespondSetDefaultMetrics()
        {
            InvokeClientMethod("SetDefaultMetrics");
        }

        /// <summary>
        /// Respond to a set metrics message from a developer client
        /// </summary>
        public override void RespondSetMetrics()
        {
            InvokeClientMethod("SetMetrics");
        }

        /// <summary>
        /// Respond to an execute script message from a developer client
        /// </summary>
        public override void RespondExecuteScript()
        {
            InvokeClientMethod("ExecuteScript");
        }

        /// <summary>
        /// Respond to an execute command message from a developer client
        /// </summary>
        public override void RespondExecuteCommand()
        {
            InvokeClientMethod("ExecuteCommand");
        }

        /// <summary>
        /// Respond to an abort script message from a developer client
        /// </summary>
        public override void RespondAbortScript()
        {
            InvokeClientMethod("AbortScript");
        }

        /// <summary>
        /// Respond to a reset reader message from a developer client
        /// </summary>
        public override void RespondLaunchCommandPrompt()
        {
            InvokeClientMethod("LaunchCommandPrompt");
        }

        /// <summary>
        /// Respond to a launch command prompt message from a developer client
        /// </summary>
        public override void RespondResetReader()
        {
            InvokeClientMethod("ResetReader");
        }

        /// <summary>
        /// Respond to a get all device values message from a developer client
        /// </summary>
        /// <param name="value">The device values XML</param>
        public override void RespondGetAllDeviceValues(string value)
        {
            InvokeClientMethod("GetAllDeviceValues", new Dictionary<string, string>() 
                { { "value", value } });
        }

        /// <summary>
        /// Respond to a get device value message from a developer client
        /// </summary>
        /// <param name="value">The device value text</param>
        public override void RespondGetDeviceValue(int value)
        {
            InvokeClientMethod("GetDeviceValue", new Dictionary<string, string>()
                { { "value", SecurityElement.Escape(value.ToString()) } });
        }

        /// <summary>
        /// Static callback function for a UDP message
        /// </summary>
        /// <param name="oAsyncResult"></param>
        private static void UdpCallback(IAsyncResult oAsyncResult)
        {
            // Initialise the client and an IP end point
            var udpClient = oAsyncResult.AsyncState as UdpClient;
            IPEndPoint ipEndPoint = null;

            try
            {
                // Get the message and reset the listener
                var buffer = udpClient.EndReceive(oAsyncResult, ref ipEndPoint);
                udpClient.BeginReceive(new AsyncCallback(UdpCallback), udpClient);

                // Check for an IO message
                if (Encoding.ASCII.GetString(buffer, 0, buffer.Length) == "IO")
                {
                    // Encode the instrument name
                    buffer = Encoding.UTF8.GetBytes(IConfiguration.Instance.InstrumentName);

                    // Bounce this back to the client
                    udpClient.Send(buffer, buffer.Length, new IPEndPoint(ipEndPoint.Address, 11000));
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Invoke a client method
        /// </summary>
        /// <param name="name">The method name</param>
        /// <param name="parameters">The method parameters as an XML fragment</param>
        /// <returns></returns>
        private void InvokeClientMethod(string name, Dictionary<string, string> parameters = null)
        {
            // Lock this object to avoid the SSL stream being disposed of during the call
            lock (this)
            {
                // Check we have a valid SSL stream
                if (sslStream != null)
                {
                    // Write the soap message
                    using (var xmlWriter = XmlWriter.Create(sslStream, new XmlWriterSettings()
                    {
                        CloseOutput = false,
                        Encoding = new UTF8Encoding(false),
                    }))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("soap", "Envelope", 
                            "http://www.w3.org/2003/05/soap-envelope");
                        xmlWriter.WriteElementString("Header", 
                            "http://www.w3.org/2003/05/soap-envelope", null);
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
                }
            }
        }

        /// <summary>
        /// Listener thread procedure
        /// </summary>
        private void ListenerThreadProcedure()
        {
            // Loop until the thread is aborted
            while (true)
            {
                try
                {
                    // Check for a null listener
                    if (listener == null)
                    {
                        break;
                    }

                    // Wait for a client to connect
                    using (var client = listener.AcceptTcpClient())
                    {
                        // Lock this object and create the SSL stream
                        lock (this)
                        {
                            sslStream = new SslStream(client.GetStream(), false,
                                new RemoteCertificateValidationCallback(ValidateClientCertificate));
                        }

                        try
                        {
                            // Authenticate as a server
                            sslStream.AuthenticateAsServer(serverCertificate, true, SslProtocols.Tls, false);

                            // Initialise a buffer and bytes read
                            var buffer = new byte[READ_BUFFER_SIZE];
                            int read;

                            // Initialise a message string builder
                            StringBuilder messageBuilder = null;

                            // Initialise a XML document for validation
                            var xmlDocument = new XmlDocument() { XmlResolver = null };

                            // Loop until an empty message is read
                            do
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
                                    
                                    if (messageBuilder != null)
                                    {
                                        // Append this fragment to the message
                                        messageBuilder.Append(fragment);

                                        try
                                        {
                                            // Get the message as a string
                                            string message = messageBuilder.ToString();

                                            // Check for valid XML
                                            xmlDocument.LoadXml(message);

                                            // Queue the message
                                            MessageQueue.Instance.Push(message);
                                        }
                                        catch (XmlException)
                                        {
                                            // The XML is not valid
                                        }
                                    }
                                }
                            }
                            while (read != 0);
                        }
                        finally
                        {
                            // Lock this object and dispose of any SSL stream
                            lock (this)
                            {
                                // Check for an SSL stream
                                if (sslStream != null)
                                {
                                    // Close the stream
                                    sslStream.Dispose();
                                    sslStream = null;
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Something bad happened so start listening again
                }
            }
        }

        /// <summary>
        /// Static client certificate authentication
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="certificate">The client certificate</param>
        /// <param name="chain">The certificate chain</param>
        /// <param name="sslPolicyErrors">The policy errors</param>
        /// <returns>True if the client certificate is valid, false otherwise</returns>
        private static bool ValidateClientCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // Check for a valid certificate
            return (sslPolicyErrors == SslPolicyErrors.None);
        }
    }
}
