/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Threading;
using IO.Comms.Firmware;

namespace IO.PerformanceTest
{
    /// <summary>
    /// The performance test program
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // Initialise the iteration
            int iteration = 0;

            try
            {
                // Create a ne manager
                IManager.Instance = new Manager();

                // Initialise the manager
                IManager.Instance.Initialise();

                // Initialise the wait handles
                var waitHandles = new WaitHandle[] { Comms.Firmware.MessageQueue.Instance };
            
                while (true)
                {
                    // Report progress
                    if (++iteration % 100 == 0)
                    {
                        Console.WriteLine("Iteration " + iteration);
                    }

                    // Set and execute the test script
                    IManager.Instance.SetScript("Test", "delay 1\r\n");
                    IManager.Instance.ExecuteScript("Test");

                    // Wait for the finish message
                    if (WaitHandle.WaitAny(waitHandles, 2000) == WaitHandle.WaitTimeout)
                    {
                        throw new ApplicationException("Timeout on firmware");
                    }

                    // Read the message
                    var message = Comms.Firmware.MessageQueue.Instance.Pop();

                    // Check the response
                    if (message != "*F=Test")
                    {
                        throw new ApplicationException("Invalid message from firmware '" + message + "'");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " after iteration " + iteration);
            }
        }
    }
}
