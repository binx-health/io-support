/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml;
using System.Xml.Serialization;
using IO.Scripting;

namespace IO.Comms.Tests
{
    /// <summary>
    /// Assays interface
    /// </summary>
    [XmlRoot("Assays")]
    public class Assays : IAssays
    {
        /// <summary>
        /// The types for serialisation
        /// </summary>
        public override Type[] Types
        {
            get
            {
                return new Type[] { typeof(Assay) };
            }
        }

        /// <summary>
        /// Get the assay for the scanned barcode
        /// </summary>
        /// <param name="barcode">The scanned barcode</param>
        /// <returns>The assay or null if none match</returns>
        public override IAssay GetAssayForBarcode(string barcode)
        {
            return null;
        }
    }
}
