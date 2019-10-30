/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.FileSystem;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Audit log singleton
    /// </summary>
    public class AuditLog : IAuditLog
    {
        /// <summary>
        /// Write a message to the audit log
        /// </summary>
        /// <param name="message">The message to write</param>
        public override void Log(string message)
        {
            ILocalFileSystem.Instance.AppendTextFile("AuditLog",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss ") + message + "\r\n");
        }
    }
}
