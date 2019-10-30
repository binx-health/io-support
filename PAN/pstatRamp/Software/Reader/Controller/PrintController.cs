/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Text;
using IO.Model.Serializable;

namespace IO.Controller
{
    /// <summary>
    /// The controller object
    /// </summary>
    partial class Controller
    {
        /// <summary>
        /// Printer not found flag
        /// </summary>
        private bool printerNotFound = false;

        /// <summary>
        /// Printer not found flag
        /// </summary>
        public override bool PrinterNotFound
        {
            get
            {
                return printerNotFound;
            }
        }

        /// <summary>
        /// Server thread procedure
        /// </summary>
        private void PrintThreadProcedure()
        {
            // Run until aborted
            while (true)
            {
                try
                {
                    // Wait for a message
                    if (View.PrintQueue.Instance.WaitOne())
                    {
                        // Get the next print job
                        string printJob;

                        while ((printJob = View.PrintQueue.Instance.Pop()) != null)
                        {
                            // Create a temporary file which will be copied to the printer
                            var temporaryFilePath = Path.GetTempFileName();

                            try
                            {
                                // Copy the file to the printer
                                System.IO.File.Copy(temporaryFilePath, IConfiguration.Instance.PrinterPort);

                                // Create a stream writer
                                using (var streamWriter = new StreamWriter(temporaryFilePath, false,
                                    new UnicodeEncoding(false, false)))
                                {
                                    // Print a separator
                                    streamWriter.Write(printJob);
                                }

                                try
                                {
                                    // Copy the file to the printer
                                    System.IO.File.Copy(temporaryFilePath, IConfiguration.Instance.PrinterPort);

                                    // Clear the printer not found flag
                                    printerNotFound = false;
                                }
                                catch (System.IO.IOException)
                                {
                                    // Set the printer not found flag
                                    printerNotFound = true;
                                }
                            }
                            finally
                            {
                                // Delete the temporary file
                                File.Delete(temporaryFilePath);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Something bad happened so try again
                }
            }
        }
    }
}
