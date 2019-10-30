/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Serializable state interface
    /// </summary>
    public abstract class IState
    {
        /// <summary>
        /// Instance variable accessor
        /// </summary>
        public static IState Instance { get; set; }

        /// <summary>
        /// The last scanned barcode
        /// </summary>
        public abstract string ScannedBarcode { get; set; }
    }
}
