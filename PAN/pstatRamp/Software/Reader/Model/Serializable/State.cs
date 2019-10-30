/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.FileSystem;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Serializable state
    /// </summary>
    public class State : IState
    {
        /// <summary>
        /// The last scanned barcode
        /// </summary>
        public override string ScannedBarcode
        {
            get
            {
                return ILocalFileSystem.Instance.ReadTextFile("ScannedBarcode");
            }
            set
            {
                ILocalFileSystem.Instance.WriteTextFile("ScannedBarcode", value);
            }
        }
    }
}
