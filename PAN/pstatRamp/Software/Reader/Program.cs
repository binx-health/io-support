/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using IO.View;
using IO.FileSystem;
using IO.Controller;
using IO.Model;
using IO.Comms;
using IO.Scripting;
using IO.Analysis;

namespace IO.Reader
{
    /// <summary>
    /// The static reader application object
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Get the working directory
                var workingDirectory = Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);

                // Set the current directory to the working directory
                Directory.SetCurrentDirectory(workingDirectory);

                "Creating file system".Log();

                // Create a new file system
                using (var defaultFileSystem = new DefaultFileSystem("Data\\"))
                {
                    ILocalFileSystem.Instance = defaultFileSystem;

                    "Initialising analysis".Log();

                    // Initialise the analysis
                    Analysis.DefaultAnalysis.Initialise();

                    "Initialising scripting".Log();

                    // Initialise the scripting
                    Scripting.Scripting.Initialise();

                    "Initialising model".Log();

                    // Initialise the model
                    Model.Model.Initialise();

                    "Initialising ftp file system".Log();

                    // Create the ftp file system
                    IFtpFileSystem.Instance = new DefaultFtpFileSystem(
                        Model.Serializable.IConfiguration.Instance.FtpServerUri,
                        Model.Serializable.IConfiguration.Instance.FtpServerUserName,
                        Model.Serializable.IConfiguration.Instance.FtpServerPassword);

                    "Creating controller".Log();

                    // Create the controller
                    using (var controller = new Controller.Controller())
                    {
                        "Initialising controller".Log();

                        // Initialise the controller
                        controller.Initialise();

                        // Set the controller instance
                        IController.Instance = controller;

                        "Creating firmware manager".Log();

                        // Create the firmware manager
                        using (var firmwareManager = new Comms.Firmware.Manager())
                        {
                            // Set the firmware manager instance
                            Comms.Firmware.IManager.Instance = firmwareManager;

                            "Creating network manager".Log();

                            // Create the network manager
                            using (var networkManager = new Comms.Network.Manager())
                            {
                                // Set the network manager instance
                                Comms.Network.IManager.Instance = networkManager;

                                "Creating server client".Log();

                                // Create the server client
                                using (var serverClient = new Comms.Server.Client())
                                {
                                    // Set the server client instance
                                    Comms.Server.Client.Instance = serverClient;

                                    "Creating view manager".Log();

                                    // Create the view manager
                                    IViewManager.Instance = new ViewManager();

                                    "Running application".Log();

                                    // Run the application
                                    IViewManager.Instance.Run();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Catch any UI thread exceptions and log them
                e.Message.Log();
            }
        }
    }
}
