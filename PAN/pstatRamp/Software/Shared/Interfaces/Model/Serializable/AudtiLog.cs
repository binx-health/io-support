/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Audit log interface
    /// </summary>
    public abstract class IAuditLog
    {
        /// <summary>
        /// Instance variable accessor
        /// </summary>
        public static IAuditLog Instance { get; set; }

        /// <summary>
        /// Write a message to the audit log
        /// </summary>
        /// <param name="message">The message to write</param>
        public abstract void Log(string message);
    }
}
