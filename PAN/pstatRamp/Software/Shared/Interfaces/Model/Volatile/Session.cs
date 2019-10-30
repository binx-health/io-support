/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.View;
using IO.Model.Serializable;

namespace IO.Model.Volatile
{
    /// <summary>
    /// Session interface
    /// </summary>
    public abstract class ISession
    {
        /// <summary>
        /// The current form for the session
        /// </summary>
        public abstract IForm CurrentForm { get; set; }

        /// <summary>
        /// The current test for the user
        /// </summary>
        public abstract ITest CurrentTest { get; set; }
    }
}
