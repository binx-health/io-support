/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;
using IO.MessageQueue;

namespace IO.Comms.Firmware
{
    /// <summary>
    /// Firmware manager class
    /// </summary>
    public class Manager : IManager, IDisposable
    {
        /// <summary>
        /// The response commands from the firmware
        /// </summary>
        private static readonly HashSet<char> RESPONSE_COMMANDS = new HashSet<char>() { 'A', 'N' };

        /// <summary>
        /// Firmware response timeout in ms
        /// </summary>
        private static readonly int RESPONSE_TIMEOUT = 2000;

        /// <summary>
        /// The script chunk size
        /// </summary>
        private static readonly int SCRIPT_CHUNK_SIZE = 4096;

        /// <summary>
        /// The serial port for firmware comms
        /// </summary>
        private SerialPort serialPort = null;

        /// <summary>
        /// The firmware version
        /// </summary>
        private string firmwareVersion = null;

        /// <summary>
        /// Initialise the response message queue
        /// </summary>
        private MessageQueue<string> responseMessageQueue = new MessageQueue<string>();

        /// <summary>
        /// Holder for a partial message from the serial port
        /// </summary>
        private string partialMessage = "";

        /// <summary>
        /// The firmware version
        /// </summary>
        public override string FirmwareVersion
        {
            get
            {
                return firmwareVersion;
            }
        }

        /// <summary>
        /// Initialise the manager for use
        /// </summary>
        public override void Initialise()
        {
            "Getting port names".Log();

            // Get the port names
            var portNames = SerialPort.GetPortNames().ToList();
            
            // Sort them alphabetically
            portNames.Sort();

            // Loop through the available serial ports
            foreach (var portName in portNames)
            {
                ("Trying port " + portName).Log();

                // Create a new ASCII serial port with a read timeout of 1 second
                serialPort = new SerialPort(portName)
                {
                    ReadTimeout = RESPONSE_TIMEOUT,
                    Encoding = Encoding.ASCII,
                };

                try
                {
                    // Open the port, write the message and read the response
                    serialPort.Open();
                    serialPort.ReadExisting();
                    serialPort.WriteLine("*V");

                    // Initialise a read buffer
                    string buffer;

                    // Create a stopwatch and start it
                    var stopwatch = new System.Diagnostics.Stopwatch();

                    stopwatch.Start();

                    // Loop until the timeout has elapsed or the port times out
                    while ((firmwareVersion == null) &&
                        (stopwatch.ElapsedMilliseconds < RESPONSE_TIMEOUT) &&
                        ((buffer = serialPort.ReadLine()) != null))
                    {
                        // Check for a valid response
                        if (buffer.StartsWith("*V="))
                        {
                            // Extract the firmware version from the string
                            firmwareVersion = buffer.Substring(3).Trim();

                            // Set the event for received data
                            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
                        }
                    }

                    // Check for a firmware version
                    if (firmwareVersion != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    // Something bad happened so continue looking
                }

                // Dispose of and dereference the serial port
                serialPort.Dispose();
                serialPort = null;
            }

            // Check for valid firmware
            if (serialPort == null)
            {
                throw new ApplicationException("No firmware detected");
            }

            // Send the hardware command
            SendAcknowledgedCommand("*H");
        }

        /// <summary>
        /// Commence the instrument shutdown sequence
        /// </summary>
        public override void Shutdown()
        {
            // Send the shutdown command
            SendAcknowledgedCommand("*Y");
        }

        /// <summary>
        /// Set the default metrics on the firmware
        /// </summary>
        /// <param name="value">The default metrics</param>
        public override void SetDefaultMetrics(string value)
        {
            // Send the script command
            SendAcknowledgedCommand("*Q=" + Encoding.ASCII.GetByteCount(value).ToString());

            // Send the script command
            SendAcknowledgedCommand(value);
        }

        /// <summary>
        /// Set the test metrics on the firmware
        /// </summary>
        /// <param name="value">The test metrics</param>
        public override void SetTestMetrics(string value)
        {
            // Send the script command
            SendAcknowledgedCommand("*M=" + Encoding.ASCII.GetByteCount(value).ToString());

            // Send the script command
            SendAcknowledgedCommand(value);
        }

        /// <summary>
        /// Set a script on the firmware
        /// </summary>
        /// <param name="name">The script name</param>
        /// <param name="value">The script value</param>
        public override void SetScript(string name, string value)
        {
            // Initialise the remainder of the script
            var remainder = value;

            // Send the script command
            SendAcknowledgedCommand("*S=" + name + ", " + 
                Encoding.ASCII.GetByteCount(remainder).ToString());

            while (remainder.Length > 0)
            {
                if (remainder.Length > SCRIPT_CHUNK_SIZE)
                {
                    // Send the script
                    SendAcknowledgedCommand(remainder.Substring(0, SCRIPT_CHUNK_SIZE));

                    // Modify the remainder
                    remainder = remainder.Substring(SCRIPT_CHUNK_SIZE);
                }
                else
                {
                    // Send the script
                    SendAcknowledgedCommand(remainder);

                    // Clear the remainder
                    remainder = string.Empty;
                }
            }
        }

        /// <summary>
        /// Execute a script on the firmware
        /// </summary>
        /// <param name="name">The script name</param>
        public override void ExecuteScript(string name)
        {
            // Send the script command
            SendAcknowledgedCommand("*E=" + name);
        }

        /// <summary>
        /// Abort the current script on the firmware
        /// </summary>
        public override void AbortScript()
        {
            // Send the script command
            SendAcknowledgedCommand("*X");
        }

        /// <summary>
        /// Delete all scripts on the firmware
        /// </summary>
        public override void DeleteScripts()
        {
            // Send the script command
            SendAcknowledgedCommand("*D");
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            // Dispose of the serial port
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
        }

        /// <summary>
        /// Send and acknowledged command to the firmware
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private void SendAcknowledgedCommand(string command)
        {
            var response = SendCommand(command);

            if (response == "*N")
            {
                throw new ApplicationException("Firmware did not acknowledge '" + command + "'");
            }
            else if (response != "*A")
            {
                throw new ApplicationException("Invalid firmware response '" + 
                    response + "' for '" + command + "'");
            }
        }

        /// <summary>
        /// Send a command to the firmware
        /// </summary>
        /// <param name="command">The command to send</param>
        /// <returns>The response from the firmware</returns>
        private string SendCommand(string command)
        {
            // Write the command to the serial port
            serialPort.WriteLine(command);

            // Initialise the wait handles
            var waitHandles = new WaitHandle[] { responseMessageQueue };

            // Wait for a response message in the queue
            if (WaitHandle.WaitAny(waitHandles, RESPONSE_TIMEOUT) == WaitHandle.WaitTimeout)
            {
                // Look for a carriage return as the command can be a whole script
                var index = command.IndexOf('\r');

                // Throw the error
                throw new ApplicationException("Firmware response timeout for '" +  
                    ((index == -1) ? command : command.Substring(0, index)) + "'");
            }

            // Return the message in the response queue
            return responseMessageQueue.Pop();
        }

        /// <summary>
        /// Event handler for data received from the serial port 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Get the serial port
            var serialPort = (SerialPort)sender;

            // Split the messages over line feeds
            var messages = serialPort.ReadExisting().Split(new char[] { '\n' });

            // Loop through the messages
            for (int index = 0; index < messages.Length; ++index)
            {
                // Check for the last message
                if (index == messages.Length - 1)
                {
                    // This is a partial message
                    partialMessage += messages[index];
                }
                else
                {
                    // Prefix the message with and clear the partial message
                    var message = partialMessage + messages[index].Trim();
                    partialMessage = "";

                    // Check for a valid message
                    if (message.StartsWith("*") && (message.Length > 1))
                    {
                        // Get the command from the message
                        char command = message[1];

                        // Push any read messages onto the relevant queue
                        if (RESPONSE_COMMANDS.Contains(command))
                        {
                            responseMessageQueue.Push(message);
                        }
                        else
                        {
                            MessageQueue.Instance.Push(message);
                        }
                    }
                    else
                    {
                        MessageQueue.Instance.Push(message);
                    }
                }
            }
        }
    }
}
