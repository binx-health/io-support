/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.Comms.Network
{
    /// <summary>
    /// Network client interface
    /// </summary>
    public abstract class IClient
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IClient Instance { get; set; }

        /// <summary>
        /// List of discovered devices
        /// </summary>
        public abstract Dictionary<string, string> Devices { get; }

        /// <summary>
        /// Connected flag
        /// </summary>
        public abstract bool Connected { get; }

        /// <summary>
        /// Initialise the client for use
        /// </summary>
        public abstract void Initialise();

        /// <summary>
        /// Discover at least one device on the network
        /// </summary>
        public abstract void Discover();

        /// <summary>
        /// Connect to the reader
        /// </summary>
        /// <param name="uri">The uri for the reader</param>
        /// <param name="port">The port</param>
        public abstract void Connect(string uri, int port);

        /// <summary>
        /// Disconnect from the reader
        /// </summary>
        /// <param name="undiscover">Undiscover any readers</param>
        public abstract void Disconnect(bool undiscover = false);

        /// <summary>
        /// Call GetPhase on the reader
        /// </summary>
        /// <returns>The phase</returns>
        public abstract string GetPhase();

        /// <summary>
        /// Call GetConfiguration on the reader
        /// </summary>
        /// <returns>The serialised configuration</returns>
        public abstract string GetConfiguration();

        /// <summary>
        /// Call GetAssays on the reader
        /// </summary>
        /// <returns>The serialised assays</returns>
        public abstract string GetAssays();

        /// <summary>
        /// Call GetScript on the reader
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <returns>The script in plain text</returns>
        public abstract string GetScript(string name);

        /// <summary>
        /// Call GetDefaultMetrics on the reader
        /// </summary>
        /// <returns>The default metrics in plain text</returns>
        public abstract string GetDefaultMetrics();

        /// <summary>
        /// Call GetMetrics on the reader
        /// </summary>
        /// <param name="name">The name of the metrics</param>
        /// <returns>The metrics in plain text</returns>
        public abstract string GetMetrics(string name);

        /// <summary>
        /// Call GetDeviceValues on the reader
        /// </summary>
        /// <returns>The serialised device values</returns>
        public abstract string GetDeviceValues();

        /// <summary>
        /// Call GetAssays on the reader
        /// </summary>
        /// <returns>The serialised test data</returns>
        public abstract string GetTestData();

        /// <summary>
        /// Call SetConfiguration on the reader
        /// </summary>
        /// <param name="value">The serialised configuration</param>
        public abstract void SetConfiguration(string value);

        /// <summary>
        /// Call SetAssays on the reader
        /// </summary>
        /// <param name="value">The serialised assays</param>
        public abstract void SetAssays(string value);

        /// <summary>
        /// Call SetScript on the reader
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <param name="value">The script in plain text</param>
        public abstract void SetScript(string name, string value);

        /// <summary>
        /// Call SetDefaultMetrics on the reader
        /// </summary>
        /// <param name="value">The default metrics in plain text</param>
        public abstract void SetDefaultMetrics(string value);

        /// <summary>
        /// Call SetMetrics on the reader
        /// </summary>
        /// <param name="name">The name of the metrics</param>
        /// <param name="value">The metrics in plain text</param>
        public abstract void SetMetrics(string name, string value);

        /// <summary>
        /// Call ExecuteScript on the reader
        /// </summary>
        /// <param name="name">The name of the script</param>
        public abstract void ExecuteScript(string name);

        /// <summary>
        /// Call ExecuteCommand on the reader
        /// </summary>
        /// <param name="command">The command in plain text</param>
        public abstract void ExecuteCommand(string command);

        /// <summary>
        /// Call AbortScript on the reader
        /// </summary>
        public abstract void AbortScript();

        /// <summary>
        /// Call ResetReader on the reader
        /// </summary>
        public abstract void ResetReader();

        /// <summary>
        /// Call LaunchCommandPrompt on the reader
        /// </summary>
        public abstract void LaunchCommandPrompt();
    }
}
