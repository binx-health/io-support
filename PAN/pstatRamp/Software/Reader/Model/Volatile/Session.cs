/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.Model.Serializable;
using IO.View;

namespace IO.Model.Volatile
{
    /// <summary>
    /// The session object
    /// </summary>
    public class Session : ISession
    {
        /// <summary>
        /// The current form for the session
        /// </summary>
        public override IForm CurrentForm { get; set; }

        /// <summary>
        /// The current test for the user
        /// </summary>
        public override ITest CurrentTest { get; set; }
    }
}
