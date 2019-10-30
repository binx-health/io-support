/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Comms.Firmware
{
    /// <summary>
    /// Singleton message queue for UI messages
    /// </summary>
    public class MessageQueue : IO.MessageQueue.MessageQueue<string>
    {
        /// <summary>
        /// Instance variable for singleton
        /// </summary>
        private static readonly MessageQueue instance = new MessageQueue();

        /// <summary>
        /// Instance variable accessor
        /// </summary>
        public static MessageQueue Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
