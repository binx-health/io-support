/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.Model.Serializable;

namespace IO.View.Tests
{
    /// <summary>
    /// State object for test
    /// </summary>
    public class State : IState
    {
        /// <summary>
        /// The last scanned barcode
        /// </summary>
        public override string ScannedBarcode { get; set; }
    }
}
