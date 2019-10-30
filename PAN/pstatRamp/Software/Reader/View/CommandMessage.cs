/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View
{
    /// <summary>
    /// Command message sent from a form
    /// </summary>
    public class CommandMessage : Message
    {
        /// <summary>
        /// The form command associated with the message
        /// </summary>
        public FormCommand Command { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="command">The form command</param>
        public CommandMessage(FormCommand command)
        {
            Command = command;
        }
    }
}
