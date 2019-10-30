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
using System.Windows.Forms;
using IO.Model.Volatile;
using IO.Comms.Network;
using IO.FileSystem;
using IO.Analysis;

namespace IO.Developer
{
    /// <summary>
    /// Developer program
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialise the file system
            using (var defaultFileSystem = new DefaultFileSystem("Data\\"))
            {
                ILocalFileSystem.Instance = defaultFileSystem;

                // Initialise the default analysis 
                Analysis.DefaultAnalysis.Initialise();

                // Initialise the scripting objects
                Scripting.Scripting.Initialise();

                // Initialise the model
                Model.Model.Initialise();

                // Create a new network client
                using (var client = new Client())
                {
                    IClient.Instance = client;

                    // Initialise the client
                    client.Initialise();

                    // Create a new controller
                    using (var controller = new IO.Controller.Controller())
                    {
                        // Initialise the controller
                        controller.Initialise();

                        // Initialise the application
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        // Create the serialiser from the main window
                        var xmlSerializer = new XmlSerializer(typeof(View.Main));

                        // Try to load the main window file
                        var fragment = ILocalFileSystem.Instance.ReadTextFile("Main");

                        if (fragment != null)
                        {
                            // Create a string reader
                            using (var stringReader = new StringReader(fragment))
                            {
                                // Deserialise the object and return it
                                View.Main.Instance = xmlSerializer.Deserialize(stringReader) as View.Main;
                            }
                        }
                        else
                        {
                            View.Main.Instance = new View.Main();
                        }

                        // Run the main window
                        Application.Run(View.Main.Instance);

                        // Create as string builder
                        var stringBuilder = new StringBuilder();

                        // Create an XML writer to serialise the object as a file
                        using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
                        {
                            Indent = true,
                        }))
                        {
                            // Serialise the object
                            xmlSerializer.Serialize(xmlWriter, View.Main.Instance);
                        }

                        // Return the XML fragment as a string
                        ILocalFileSystem.Instance.WriteTextFile("Main", stringBuilder.ToString());
                    }
                }
            }
        }
    }
}
