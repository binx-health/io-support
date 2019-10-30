/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Tests
{
    /// <summary>
    /// Session object for the test
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
