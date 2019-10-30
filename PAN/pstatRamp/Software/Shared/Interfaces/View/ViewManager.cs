/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Globalization;
using System.Collections.Generic;
using IO.Model.Serializable;
using IO.FileSystem;

namespace IO.View
{
    public abstract class IViewManager
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IViewManager Instance { get; set; }

        /// <summary>
        /// The current form
        /// </summary>
        public abstract IForm CurrentForm { get; set; }

        /// <summary>
        /// The list of locales
        /// </summary>
        public abstract Dictionary<string, CultureInfo> Locales { get; set; }

        /// <summary>
        /// Flag to indicate that the view manager is running
        /// </summary>
        public abstract bool IsRunning { get; }

        /// <summary>
        /// Run the application
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Stop the application
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Create a new root form and return the previous stack
        /// </summary>
        /// <param name="name">The name of the new root form to show</param>
        /// <returns>The previous current form</returns>
        public abstract IForm ReplaceRoot(string name);

        /// <summary>
        /// Replace the current stack with a new stack
        /// </summary>
        /// <param name="form">The form at the top of the new stack</param>
        /// <returns>The previous current form</returns>
        public abstract IForm ReplaceRoot(IForm form);

        /// <summary>
        /// Update the current user
        /// </summary>
        /// <param name="value">The new value</param>
        public abstract void UpdateCurrentUser(IUser value);

        /// <summary>
        /// Update the timeout for logging out a user in seconds
        /// </summary>
        /// <param name="value">The new value</param>
        public abstract void UpdateAutoLogoffPeriodInSeconds(int value);

        /// <summary>
        /// Update the current culture
        /// </summary>
        /// <param name="value">The new value</param>
        public abstract void UpdateCurrentCulture(string value);

        /// <summary>
        /// Print a result
        /// </summary>
        /// <param name="sampleId">The sample ID</param>
        /// <param name="patientInformation">The patient information</param>
        /// <param name="test">The test</param>
        public abstract void PrintResult(string sampleId, Dictionary<string, object> patientInformation,
            ITest test);

        /// <summary>
        /// Export the audit log to file
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        public abstract void ExportAuditLog(ISimpleFileSystem fileSystem);

        /// <summary>
        /// Export the results to file
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        public abstract void ExportResults(ISimpleFileSystem fileSystem);
    }
}
