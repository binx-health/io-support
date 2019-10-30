/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Test outcome enumeration
    /// </summary>
    public enum TestOutcome
    {
        Positive,
        Undefined,
        Negative,
        UserAborted,
        Error,
    }
}
