/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Quanrantine state enumeration
    /// </summary>
    public enum QuarantineState
    {
        DoNotQuarantine,
        Locked,
        Unlocked,
    }
}
