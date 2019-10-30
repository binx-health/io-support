/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Comms.Firmware
{
    /// <summary>
    /// Firmware manager interface
    /// </summary>
    public abstract class IManager
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IManager Instance { get; set; }

        /// <summary>
        /// The firmware version
        /// </summary>
        /// <returns>False if no firmware is found, true otherwise</returns>
        public abstract string FirmwareVersion { get; }

        /// <summary>
        /// Initialise the manager for use
        /// </summary>
        public abstract void Initialise();

        /// <summary>
        /// Commence the instrument shutdown sequence
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// Set the default metrics on the firmware
        /// </summary>
        /// <param name="value">The default metrics</param>
        public abstract void SetDefaultMetrics(string value);

        /// <summary>
        /// Set the test metrics on the firmware
        /// </summary>
        /// <param name="value">The test metrics</param>
        public abstract void SetTestMetrics(string value);

        /// <summary>
        /// Set a script on the firmware
        /// </summary>
        /// <param name="name">The script name</param>
        /// <param name="value">The script value</param>
        public abstract void SetScript(string name, string value);

        /// <summary>
        /// Execute a script on the firmware
        /// </summary>
        /// <param name="name">The script name</param>
        public abstract void ExecuteScript(string name);

        /// <summary>
        /// Abort the current script on the firmware
        /// </summary>
        public abstract void AbortScript();

        /// <summary>
        /// Delete all scripts on the firmware
        /// </summary>
        public abstract void DeleteScripts();
    }
}
