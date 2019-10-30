/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.View
{
    /// <summary>
    /// Abstract base class for view messages
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// The name of the form
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// Dictionary of untyped message parameters
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }
    }
}
