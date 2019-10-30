/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Comms.Network
{
    /// <summary>
    /// Network manager interface
    /// </summary>
    public abstract class IManager
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IManager Instance { get; set; }

        /// <summary>
        /// Initialise the manager for use
        /// </summary>
        public abstract void Initialise();

        /// <summary>
        /// Notify a developer client of new device values
        /// </summary>
        /// <param name="values">The device values XML</param>
        public abstract void NotifyDeviceValues(string values);

        /// <summary>
        /// Notify a developer client of new log lines
        /// </summary>
        /// <param name="log">The new log lines text</param>
        public abstract void NotifyLogLines(string log);

        /// <summary>
        /// Notify a developer client of a phase change
        /// </summary>
        /// <param name="phase">The new phase text</param>
        public abstract void NotifyPhaseChange(string phase);

        /// <summary>
        /// Notify a developer client of a completed script
        /// </summary>
        /// <param name="name">The script name</param>
        public abstract void NotifyScriptComplete(string name);

        /// <summary>
        /// Notify a developer client of new test data
        /// </summary>
        public abstract void NotifyTestData();

        /// <summary>
        /// Notify a developer client of new configuration
        /// </summary>
        public abstract void NotifyConfiguration();

        /// <summary>
        /// Respond to a get assays message from a developer client
        /// </summary>
        /// <param name="value">The assays as XML</param>
        public abstract void RespondError(string faultCode, string faultString);

        /// <summary>
        /// Respond to a get phase message from a developer client
        /// </summary>
        /// <param name="phase">The phase</param>
        public abstract void RespondGetPhase(string phase);

        /// <summary>
        /// Respond to a get configuration message from a developer client
        /// </summary>
        /// <param name="value">The configuration as XML</param>
        public abstract void RespondGetConfiguration(string value);

        /// <summary>
        /// Respond to a get assays message from a developer client
        /// </summary>
        /// <param name="value">The assays as XML</param>
        public abstract void RespondGetAssays(string value);

        /// <summary>
        /// Respond to a get script message from a developer client
        /// </summary>
        /// <param name="value">The script text</param>
        public abstract void RespondGetScript(string value);

        /// <summary>
        /// Respond to a get default metrics message from a developer client
        /// </summary>
        /// <param name="value">The default metrics text</param>
        public abstract void RespondGetDefaultMetrics(string value);

        /// <summary>
        /// Respond to a get metrics message from a developer client
        /// </summary>
        /// <param name="value">The metrics text</param>
        public abstract void RespondGetMetrics(string value);

        /// <summary>
        /// Respond to a get device values message from a developer client
        /// </summary>
        /// <param name="value">The device values XML</param>
        public abstract void RespondGetDeviceValues(string value);

        /// <summary>
        /// Respond to a get test data message from a developer client
        /// </summary>
        /// <param name="value">The test data XML</param>
        public abstract void RespondGetTestData(string value);

        /// <summary>
        /// Respond to a set configuration message from a developer client
        /// </summary>
        public abstract void RespondSetConfiguration();

        /// <summary>
        /// Respond to a set assays message from a developer client
        /// </summary>
        public abstract void RespondSetAssays();

        /// <summary>
        /// Respond to a set script message from a developer client
        /// </summary>
        public abstract void RespondSetScript();

        /// <summary>
        /// Respond to a set default metrics message from a developer client
        /// </summary>
        public abstract void RespondSetDefaultMetrics();

        /// <summary>
        /// Respond to a set metrics message from a developer client
        /// </summary>
        public abstract void RespondSetMetrics();

        /// <summary>
        /// Respond to an execute script message from a developer client
        /// </summary>
        public abstract void RespondExecuteScript();

        /// <summary>
        /// Respond to an execute command message from a developer client
        /// </summary>
        public abstract void RespondExecuteCommand();

        /// <summary>
        /// Respond to an abort script message from a developer client
        /// </summary>
        public abstract void RespondAbortScript();

        /// <summary>
        /// Respond to a reset reader message from a developer client
        /// </summary>
        public abstract void RespondResetReader();

        /// <summary>
        /// Respond to a reset reader message from a developer client
        /// </summary>
        public abstract void RespondLaunchCommandPrompt();

        /// <summary>
        /// Respond to a get all device values message from a developer client
        /// </summary>
        /// <param name="value">The device values XML</param>
        public abstract void RespondGetAllDeviceValues(string value);

        /// <summary>
        /// Respond to a get device value message from a developer client
        /// </summary>
        /// <param name="value">The device value</param>
        public abstract void RespondGetDeviceValue(int value);
    }
}
