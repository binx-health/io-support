/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using IO.Scripting;
using IO.Analysis;
using IO.FileSystem;

namespace IO.Scripting.Tests
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
            try
            {
                Console.WriteLine("Scripting Tests starting");

                // Initialise the tests
                Initialise();

                // Instantiate the new default file system
                using (var defaultFileSystem = new DefaultFileSystem("Data\\"))
                {
                    ILocalFileSystem.Instance = defaultFileSystem;

                    // Run the tests
                    TestLoadAssays();
                    TestLoadDefaultMetrics();
                    TestParseScripts();
                }

                Console.WriteLine("Scripting Tests passed");
            }
            catch (Exception e)
            {
                // Write the exception
                Console.WriteLine("Scripting Tests failed");
                Console.Error.WriteLine(e.Message);
                Environment.ExitCode = 1;
            }
        }

        /// <summary>
        /// Initialise the tests
        /// </summary>
        private static void Initialise()
        {
            // Initialise the test analysis object
            IAnalysis.Types.Add(typeof(DefaultAnalysis));
        }

        /// <summary>
        /// Test the assay serialisation
        /// </summary>
        private static void TestLoadAssays()
        {
            // Create a serialiser for assays
            var xmlSerializer = new XmlSerializer(typeof(Assays), 
                new Type[] { typeof(Assay), typeof(Disease) });

            // Read the good assays file
            var goodAssays = ILocalFileSystem.Instance.ReadTextFile("Assays");

            // Create a string reader from the text file, read from the file system
            using (var stringReader = new StringReader(goodAssays))
            {
                IAssays.Instance = (IAssays)xmlSerializer.Deserialize(stringReader);
            }

            // Write the serialised assays to file
            using (var stringWriter = new StringWriter())
            {
                // Write the assays to string
                xmlSerializer.Serialize(stringWriter, IAssays.Instance);

                // Compare the assays
                var assays = stringWriter.ToString();

                if (assays != goodAssays)
                {
                    // Write the bad assays
                    ILocalFileSystem.Instance.WriteTextFile("BadAssays", assays);

                    throw new ApplicationException("Assay compare failed (see BadAssays)");
                }
            }
        }

        /// <summary>
        /// Test loading default metrics
        /// </summary>
        private static void TestLoadDefaultMetrics()
        {
            // Initialise the default metrics
            IDefaultMetrics.Instance = new Metrics("DefaultMetrics", null)
            {
                Value = ILocalFileSystem.Instance.ReadTextFile("Metrics\\DefaultMetrics"),
            };
        }

        /// <summary>
        /// Test script loading and parsing
        /// </summary>
        private static void TestParseScripts()
        {
            // Initialise the scripts
            IScripts.Instance = new Scripts() { Path = "Scripts\\" };

            // Create a summary file
            using (var stringWriter = new StringWriter())
            {
                // Loop through the scripts
                foreach (var script in IScripts.Instance)
                {
                    // Check for any errors
                    if (script.Errors.Any())
                    {
                        // Write the script name
                        stringWriter.WriteLine(script.Name);

                        // Loop through the errors writing them to the file
                        foreach (var error in script.Errors)
                        {
                            // Check for a line number
                            if (error.Line >= 0)
                            {
                                stringWriter.WriteLine(error.Description + " at line " + error.Line);
                            }
                            else
                            {
                                stringWriter.WriteLine(error.Description);
                            }
                        }

                        // Write a blank line to separate scripts
                        stringWriter.WriteLine();
                    }
                }

                // Compare the summary with the good summary
                var summary = stringWriter.ToString();

                if (summary != ILocalFileSystem.Instance.ReadTextFile("Summary"))
                {
                    // Write the bad summary
                    ILocalFileSystem.Instance.WriteTextFile("BadSummary", summary);

                    throw new ApplicationException("Summary compare failed (see BadSummary)");
                }
            }
        }
    }
}
