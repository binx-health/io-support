/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.Scripting
{
    /// <summary>
    /// Default metrics interface
    /// </summary>
    public abstract class IDefaultMetrics
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IMetrics Instance { get; set; }
    }
}
