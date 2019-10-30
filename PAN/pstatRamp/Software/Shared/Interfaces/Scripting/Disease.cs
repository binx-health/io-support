/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Scripting
{
    /// <summary>
    /// Disease interface
    /// </summary>
    public abstract class IDisease
    {
        /// <summary>
        /// The name of the peak associated with this disease
        /// </summary>
        public virtual string PeakName { get; set; }

        /// <summary>
        /// The LOINC for this disease
        /// </summary>
        public virtual string Loinc { get; set; }
    }
}
