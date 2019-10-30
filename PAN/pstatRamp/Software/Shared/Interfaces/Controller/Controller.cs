/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Controller
{
    /// <summary>
    /// Controller interface
    /// </summary>
    public abstract class IController
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IController Instance { get; set; }

        /// <summary>
        /// The total stored results
        /// </summary>
        public abstract uint TotalResults { get; }

        /// <summary>
        /// The total queued results
        /// </summary>
        public abstract uint QueuedResults { get; }

        /// <summary>
        /// Printer not found flag
        /// </summary>
        public abstract bool PrinterNotFound { get; }

        /// <summary>
        /// Initialise the controller
        /// </summary>
        public abstract void Initialise();
    }
}
