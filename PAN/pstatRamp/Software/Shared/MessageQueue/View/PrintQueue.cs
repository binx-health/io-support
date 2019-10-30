/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View
{
    /// <summary>
    /// Singleton print queue
    /// </summary>
    public class PrintQueue : IO.MessageQueue.MessageQueue<string>
    {
        /// <summary>
        /// Instance variable for singleton
        /// </summary>
        private static readonly PrintQueue instance = new PrintQueue();

        /// <summary>
        /// Instance variable accessor
        /// </summary>
        public static PrintQueue Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
