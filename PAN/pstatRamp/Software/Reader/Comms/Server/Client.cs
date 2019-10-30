/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Net.Sockets;
using IO.Scripting;

namespace IO.Comms.Server
{
    /// <summary>
    /// POCT1.A client
    /// </summary>
    public class Client: IClient, IDisposable
    {
        /// <summary>
        /// Constants
        /// </summary>
        private const int RECEIVE_TIMEOUT_SECONDS = 10;
        private const int RESPONSE_TIMEOUT_SECONDS = 60;
        private const int MAX_MESSAGE_SIZE_BYTES = 1024;
        private const int MIN_CONTROL_ID = 10001;
        private const int MAX_CONTROL_ID = 19999;
        private const string VENDOR_ID = "ATLAS^0001A^00001A";
        
        /// <summary>
        /// The current control ID for the client
        /// </summary>
        private int controlId = 0;

        /// <summary>
        /// The last control ID from the server
        /// </summary>
        private int serverControlId = 0;

        /// <summary>
        /// The TCP client
        /// </summary>
        private TcpClient tcpClient = null;

        /// <summary>
        /// Accessor for the client control ID
        /// </summary>
        public int ControlId
        {
            get
            {
                return controlId;
            }
        }

        /// <summary>
        /// Accessor for the server control ID
        /// </summary>
        public int ServerControlId
        {
            get
            {
                return serverControlId;
            }
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="uri">The uri for the server</param>
        /// <param name="port">The port</param>
        public override void Connect(string uri, int port)
        {
            // Disconnect first
            Disconnect();

            // Initialise the client and server control IDs
            controlId = MIN_CONTROL_ID - 1;
            serverControlId = 0;

            try
            {
                // Create a tcp client
                tcpClient = new TcpClient() { ReceiveTimeout = RECEIVE_TIMEOUT_SECONDS * 1000 };

                // Try to connect to the server
                tcpClient.Connect(uri, port);
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
        public override void Disconnect()
        {
            // Close and derference the tcp client
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
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
        /// Send a hello message to the server
        /// </summary>
        public override void SendHello()
        {
            // Get the TCP stream
            var stream = tcpClient.GetStream();

            // Initialise the write buffer
            byte[] writeBuffer = null;

            // Increment the control ID
            controlId = (controlId >= MAX_CONTROL_ID) ? MIN_CONTROL_ID : controlId + 1;

            // Create a new memory stream
            using (var memoryStream = new System.IO.MemoryStream())
            {
                // Create a new XML writer with UTF8 encoding
                using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, 
                    new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = new System.Text.UTF8Encoding(false),
                }))
                {
                    xmlWriter.WriteStartElement("HEL.R01");
                    xmlWriter.WriteStartElement("HDR");
                    WriteValueElement(xmlWriter, "HDR.control_id", controlId.ToString());
                    WriteValueElement(xmlWriter, "HDR.version_id", "POCT1");
                    WriteValueElement(xmlWriter, "HDR.creation_dttm", 
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("DEV");
                    WriteValueElement(xmlWriter, "DEV.vendor_id", VENDOR_ID);
                    xmlWriter.WriteStartElement("DCP");
                    WriteValueElement(xmlWriter, "DCP.application_timeout", 
                        RESPONSE_TIMEOUT_SECONDS.ToString());
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("DSC");
                    WriteValueElement(xmlWriter, "DSC.connection_profile_cd", "SA");
                    WriteValueElement(xmlWriter, "DSC.max_message_sz", MAX_MESSAGE_SIZE_BYTES.ToString());
                }

                // Get the write buffer
                writeBuffer = memoryStream.ToArray();
            }

            // Write the buffer to the stream
            stream.Write(writeBuffer, 0, writeBuffer.Length);
        }

        /// <summary>
        /// Send a status message to the sserver
        /// </summary>
        /// <param name="observations">The number of observations</param>
        public override void SendStatus(int observations)
        {
            // Get the TCP stream
            var stream = tcpClient.GetStream();

            // Initialise the write buffer
            byte[] writeBuffer = null;

            // Increment the control ID
            controlId = (controlId >= MAX_CONTROL_ID) ? MIN_CONTROL_ID : controlId + 1;

            // Create a new memory stream
            using (var memoryStream = new System.IO.MemoryStream())
            {
                // Create a new XML writer with UTF8 encoding
                using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, 
                    new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = new System.Text.UTF8Encoding(false),
                }))
                {
                    xmlWriter.WriteStartElement("DST.R01");
                    xmlWriter.WriteStartElement("HDR");
                    WriteValueElement(xmlWriter, "HDR.control_id", controlId.ToString());
                    WriteValueElement(xmlWriter, "HDR.version_id", "POCT1");
                    WriteValueElement(xmlWriter, "HDR.creation_dttm", 
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("DST");
                    WriteValueElement(xmlWriter, "DST.new_observations_qty", observations.ToString());
                }

                // Get the write buffer
                writeBuffer = memoryStream.ToArray();
            }

            // Write the buffer to the stream
            stream.Write(writeBuffer, 0, writeBuffer.Length);
        }

        /// <summary>
        /// Send an observation to the server
        /// </summary>
        /// <param name="startDateTime">The time the sample was taken</param>
        /// <param name="endDateTime">The time the analysis completed</param>
        /// <param name="calibrationOutsideTolerance">The calibration outside tolerance flag</param>
        /// <param name="patientId">The patient ID</param>
        /// <param name="positiveDiseases">The positive diseases</param>
        /// <param name="negativeDiseases">The negative diseases</param>
        /// <param name="userId">The user ID</param>
        /// <param name="userName">The user name</param>
        /// <param name="sampleId">The sample ID</param>
        /// <param name="patientName">The patient name (can be null)</param>
        /// <param name="dateOfBirth">The patient DOB (can be null)</param>
        public override void SendObservation(DateTime startDateTime, DateTime endDateTime, 
            bool calibrationOutsideTolerance, string patientId,
            IEnumerable<IDisease> positiveDiseases, IEnumerable<IDisease> negativeDiseases,
            uint userId, string userName, string sampleId, string patientName = null, 
            DateTime? dateOfBirth = null)
        {
            // Get the TCP stream
            var stream = tcpClient.GetStream();

            // Initialise the write buffer
            byte[] writeBuffer = null;

            // Increment the control ID
            controlId = (controlId >= MAX_CONTROL_ID) ? MIN_CONTROL_ID : controlId + 1;

            // Create a new memory stream
            using (var memoryStream = new System.IO.MemoryStream())
            {
                // Create a new XML writer with UTF8 encoding
                using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, 
                    new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = new System.Text.UTF8Encoding(false),
                }))
                {
                    xmlWriter.WriteStartElement("OBS.R01");
                    xmlWriter.WriteStartElement("HDR");
                    WriteValueElement(xmlWriter, "HDR.control_id", controlId.ToString());
                    WriteValueElement(xmlWriter, "HDR.version_id", "POCT1");
                    WriteValueElement(xmlWriter, "HDR.creation_dttm", 
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("SVC");
                    WriteValueElement(xmlWriter, "SVC.role_cd", "OBS");
                    WriteValueElement(xmlWriter, "SVC.observation_dttm", 
                        endDateTime.ToString("yyyy-MM-ddTHH:mm:ss+00:00"));
                    WriteValueElement(xmlWriter, "SVC.status_cd", 
                        calibrationOutsideTolerance ? "OVR" : "NRM");
                    xmlWriter.WriteStartElement("PT");
                    WriteValueElement(xmlWriter, "PT.patient_id", patientId);
                    WriteValueElement(xmlWriter, "PT.name", patientName);

                    if (dateOfBirth.HasValue)
                    {
                        WriteValueElement(xmlWriter, "PT.birth_date", 
                            dateOfBirth.Value.ToString("yyyy-MM-dd"));
                    }

                    // Loop through the positive diseases
                    foreach (var disease in positiveDiseases)
                    {
                        xmlWriter.WriteStartElement("OBS");
                        xmlWriter.WriteStartElement("OBS.observation_id");
                        xmlWriter.WriteAttributeString("V", disease.Loinc);
                        xmlWriter.WriteAttributeString("SN", "LOINC");
                        xmlWriter.WriteAttributeString("DN", disease.PeakName);
                        xmlWriter.WriteEndElement();
                        // It should be possible to send a qualitative value but this does not work with
                        // POCCelerator which seems to require a quantitative value so % is used
                        // WriteValueElement(xmlWriter, "OBS.qualitative_value", "H");
                        xmlWriter.WriteStartElement("OBS.value");
                        xmlWriter.WriteAttributeString("V", "100");
                        xmlWriter.WriteAttributeString("U", "%");
                        xmlWriter.WriteEndElement();
                        WriteValueElement(xmlWriter, "OBS.method_cd", "M");
                        xmlWriter.WriteEndElement();
                    }

                    // Loop through the negative diseases
                    foreach (var disease in negativeDiseases)
                    {
                        xmlWriter.WriteStartElement("OBS");
                        xmlWriter.WriteStartElement("OBS.observation_id");
                        xmlWriter.WriteAttributeString("V", disease.Loinc);
                        xmlWriter.WriteAttributeString("SN", "LOINC");
                        xmlWriter.WriteAttributeString("DN", disease.PeakName);
                        xmlWriter.WriteEndElement();
                        // It should be possible to send a qualitative value but this does not work with
                        // POCCelerator which seems to require a quantitative value so % is used
                        // WriteValueElement(xmlWriter, "OBS.qualitative_value", "L");
                        xmlWriter.WriteStartElement("OBS.value");
                        xmlWriter.WriteAttributeString("V", "0");
                        xmlWriter.WriteAttributeString("U", "%");
                        xmlWriter.WriteEndElement();
                        WriteValueElement(xmlWriter, "OBS.method_cd", "M");
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("OPR");
                    WriteValueElement(xmlWriter, "OPR.operator_id", userId.ToString());
                    WriteValueElement(xmlWriter, "OPR.name", userName);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("SPC");
                    WriteValueElement(xmlWriter, "SPC.specimen_dttm", 
                        endDateTime.ToString("yyyy-MM-ddTHH:mm:ss+00:00"));
                    WriteValueElement(xmlWriter, "SPC.specimen_id", sampleId);
                }

                // Get the write buffer
                writeBuffer = memoryStream.ToArray();
            }

            // Write the buffer to the stream
            stream.Write(writeBuffer, 0, writeBuffer.Length);
        }

        /// <summary>
        /// Send an end message to the server
        /// </summary>
        public override void SendEnd()
        {
            // Get the TCP stream
            var stream = tcpClient.GetStream();

            // Initialise the write buffer
            byte[] writeBuffer = null;

            // Increment the control ID
            controlId = (controlId >= MAX_CONTROL_ID) ? MIN_CONTROL_ID : controlId + 1;

            // Create a new memory stream
            using (var memoryStream = new System.IO.MemoryStream())
            {
                // Create a new XML writer with UTF8 encoding
                using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, 
                    new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = new System.Text.UTF8Encoding(false),
                }))
                {
                    xmlWriter.WriteStartElement("EOT.R01");
                    xmlWriter.WriteStartElement("HDR");
                    WriteValueElement(xmlWriter, "HDR.control_id", controlId.ToString());
                    WriteValueElement(xmlWriter, "HDR.version_id", "POCT1");
                    WriteValueElement(xmlWriter, "HDR.creation_dttm", 
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("EOT");
                    WriteValueElement(xmlWriter, "EOT.topic_cd", "OBS");
                }

                // Get the write buffer
                writeBuffer = memoryStream.ToArray();
            }

            // Write the buffer to the stream
            stream.Write(writeBuffer, 0, writeBuffer.Length);
        }

        /// <summary>
        /// Send an acknowledgement to the server
        /// </summary>
        public override void SendAcknowledgement()
        {
            // Get the TCP stream
            var stream = tcpClient.GetStream();

            // Initialise the write buffer
            byte[] writeBuffer = null;

            // Increment the control ID
            controlId = (controlId >= MAX_CONTROL_ID) ? MIN_CONTROL_ID : controlId + 1;

            // Create a new memory stream
            using (var memoryStream = new System.IO.MemoryStream())
            {
                // Create a new XML writer with UTF8 encoding
                using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, 
                    new System.Xml.XmlWriterSettings()
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = new System.Text.UTF8Encoding(false),
                }))
                {
                    xmlWriter.WriteStartElement("ACK.R01");
                    xmlWriter.WriteStartElement("HDR");
                    WriteValueElement(xmlWriter, "HDR.control_id", controlId.ToString());
                    WriteValueElement(xmlWriter, "HDR.version_id", "POCT1");
                    WriteValueElement(xmlWriter, "HDR.creation_dttm", 
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00"));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("ACK");
                    WriteValueElement(xmlWriter, "ACK.type_cd", "AA");
                    WriteValueElement(xmlWriter, "ACK.ack_control_id", serverControlId.ToString());
                }

                // Get the write buffer
                writeBuffer = memoryStream.ToArray();
            }

            // Write the buffer to the stream
            stream.Write(writeBuffer, 0, writeBuffer.Length);
        }

        /// <summary>
        /// Read an acknowledgement from the server
        /// </summary>
        public override void ReadAcknowledgement()
        {
            // Read an XML message
            var xmlDocument = ReadXml();

            // Get the server control ID
            var serverControlIdString = ReadValueElement(xmlDocument, "/ACK.R01/HDR/HDR.control_id");

            // Read and check the message
            int newServerControlId;

            if (string.IsNullOrEmpty(serverControlIdString) || 
                (int.TryParse(serverControlIdString, out newServerControlId) == false) ||
                (newServerControlId == serverControlId) ||
                (ReadValueElement(xmlDocument, "/ACK.R01/ACK/ACK.type_cd") != "AA") ||
                (ReadValueElement(xmlDocument, "/ACK.R01/ACK/ACK.ack_control_id") != controlId.ToString()))
            {
                throw new ApplicationException("Server failed to acknowledge");
            }

            // Update the server control ID
            serverControlId = newServerControlId;
        }

        /// <summary>
        /// Read a request for observations from the server
        /// </summary>
        public override void ReadRequestForObservations()
        {
            // Read an XML message
            var xmlDocument = ReadXml();

            // Get the server control ID
            var serverControlIdString = ReadValueElement(xmlDocument, "/REQ.R01/HDR/HDR.control_id");

            // Read and check the message
            int newServerControlId;

            if (string.IsNullOrEmpty(serverControlIdString) ||
                (int.TryParse(serverControlIdString, out newServerControlId) == false) ||
                (newServerControlId == serverControlId) ||
                (ReadValueElement(xmlDocument, "/REQ.R01/REQ/REQ.request_cd") != "ROBS"))
            {
                throw new ApplicationException("Server failed to request observations");
            }

            // Update the server control ID
            serverControlId = newServerControlId;
        }

        /// <summary>
        /// Read an end message from the server
        /// </summary>
        public override void ReadEnd()
        {
            // Read an XML message
            var xmlDocument = ReadXml();

            // Get the server control ID
            var serverControlIdString = ReadValueElement(xmlDocument, "/END.R01/HDR/HDR.control_id");

            // Read and check the message
            int newServerControlId;

            if (string.IsNullOrEmpty(serverControlIdString) ||
                (int.TryParse(serverControlIdString, out newServerControlId) == false) ||
                (newServerControlId == serverControlId) ||
                (ReadValueElement(xmlDocument, "/END.R01/TRM/TRM.reason_cd") != "NRM"))
            {
                throw new ApplicationException("Server failed to end");
            }

            // Update the server control ID
            serverControlId = newServerControlId;
        }

        /// <summary>
        /// Read a complete XML message from the server
        /// </summary>
        /// <returns></returns>
        private XmlDocument ReadXml()
        {
            // Get the TCP stream
            var stream = tcpClient.GetStream();

            // Initialise the read buffer
            byte[] readBuffer = new Byte[MAX_MESSAGE_SIZE_BYTES];

            // Define the timeout
            DateTime end = DateTime.UtcNow.AddSeconds(RESPONSE_TIMEOUT_SECONDS);

            // Initialise the offset for partial messages
            int offset = 0;

            // Initialse the result
            XmlDocument xmlDocument = null;

            // Loop until the buffer is full or the timeout
            do
            {
                // Read data from the stream
                var length = stream.Read(readBuffer, offset, MAX_MESSAGE_SIZE_BYTES - offset);

                // Check the length
                if (length > 0)
                {
                    // Increment the offset
                    offset += length;

                    // Get the XML as a string
                    var xml = System.Text.Encoding.UTF8.GetString(readBuffer, 0, offset);

                    try
                    {
                        // Create an XML document and parse the string
                        xmlDocument = new XmlDocument() { XmlResolver = null };
                        xmlDocument.LoadXml(xml);

                        break;
                    }
                    catch (Exception)
                    {
                        // The XML is not valid so clear the result
                        xmlDocument = null;
                    }
                }
            }
            while ((offset < MAX_MESSAGE_SIZE_BYTES) && (DateTime.UtcNow < end));

            return xmlDocument;
        }

        /// <summary>
        /// Write a value element
        /// </summary>
        /// <param name="xmlWriter">The XML writer</param>
        /// <param name="name">The value name</param>
        /// <param name="value">The value</param>
        private void WriteValueElement(XmlWriter xmlWriter, string name, string value)
        {
            // Check for a null value
            if (value != null)
            {
                xmlWriter.WriteStartElement(name);
                xmlWriter.WriteAttributeString("V", value);
                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Read a value element from the XML document
        /// </summary>
        /// <param name="xmlDocument">The XML document</param>
        /// <param name="path">The path</param>
        /// <returns>The value or null</returns>
        private string ReadValueElement(XmlDocument xmlDocument, string path)
        {
            // Get the element containing the value
            var element = (xmlDocument != null) ? xmlDocument.SelectSingleNode(path) as XmlElement : null;

            // Return the value or null
            return ((element != null) && element.HasAttribute("V")) ? element.GetAttribute("V") : null;
        }
    }
}
