/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;

namespace IO
{
    /// <summary>
    /// Last error container
    /// </summary>
    static public class LastError
    {
#if (DEBUG)
        /// <summary>
        /// Log file stream writer
        /// </summary>
        private static readonly StreamWriter logStreamWriter = new StreamWriter("Debug.log", true);
#endif
        /// <summary>
        /// The last error message
        /// </summary>
        public static string Message { get; set; }

        /// <summary>
        /// Log to file
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void Log(this string message)
        {
#if (DEBUG)
            lock (logStreamWriter)
            {
                logStreamWriter.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss ") + message);
                logStreamWriter.Flush();
            }
#endif
        }
    }
}
