/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IO.Scripting
{
    /// <summary>
    /// Assays interface
    /// </summary>
    [XmlRoot("Assays")]
    public abstract class IAssays : List<IAssay>
    {
        /// <summary>
        /// The list of supporting scripts
        /// </summary>
        public static readonly string[] SupportingScripts = new string[]
        {
            "Clamp", "Error", "Load", "Reset", "StartUp", "StartUpUnload", "Unclamp", "Unload"
        };

        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IAssays Instance { get; set; }

        /// <summary>
        /// The types for serialisation
        /// </summary>
        public abstract Type[] Types { get; }

        /// <summary>
        /// Loaded flag
        /// </summary>
        [XmlIgnore]
        public virtual bool Loaded { get; set; }

        /// <summary>
        /// Add a new assay
        /// </summary>
        /// <param name="name">The assay name</param>
        /// <returns>The new assay</returns>
        public abstract IAssay AddNew(string name);

        /// <summary>
        /// Get the assay for the scanned barcode
        /// </summary>
        /// <param name="barcode">The scanned barcode</param>
        /// <param name="assayCode">The assay code</param>
        /// <param name="expiryDate">The expiry date</param>
        /// <returns>The assay or null if none match</returns>
        public abstract IAssay GetAssayForBarcode(string barcode, out string assayCode, 
            out DateTime expiryDate);
    }
}
