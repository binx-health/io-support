/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.Model.Volatile
{
    /// <summary>
    /// Sessions interface
    /// </summary>
    public abstract class ISessions : Dictionary<uint, ISession>
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static ISessions Instance { get; set; }

        /// <summary>
        /// The current user
        /// </summary>
        public virtual IUser CurrentUser { get; set; }

        /// <summary>
        /// The current session
        /// </summary>
        public abstract ISession CurrentSession { get; }
    }
}
