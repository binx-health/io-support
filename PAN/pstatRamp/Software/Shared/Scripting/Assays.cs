/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IO.Scripting
{
    /// <summary>
    /// Assays object
    /// </summary>
    [XmlRoot("Assays")]
    public class Assays : IAssays
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Assays()
        {
            Loaded = true;
        }

        /// <summary>
        /// Add a new assay
        /// </summary>
        /// <param name="name">The assay name</param>
        /// <returns>The new assay</returns>
        public override IAssay AddNew(string name)
        {
            // Check for a bad assay name or an assay of the same name
            if (string.IsNullOrEmpty(name) || Exists(x => x.Name == name) ||
                name.Contains(',') || name.Contains(' ') ||
                name.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                return null;
            }

            // Create a new assay
            var newAssay = new Assay()
            {
                Name = name,
                Script = name + "Main",
                UngScript = name + "MainUng",
                VoltammetryScript = name + "Voltammetry",
                MetricsName = name,
            };

            // Add it to the list
            Add(newAssay);

            return newAssay;
        }

        /// <summary>
        /// Get the assay for the scanned barcode
        /// </summary>
        /// <param name="barcode">The scanned barcode</param>
        /// <param name="assayCode">The assay code</param>
        /// <param name="expiryDate">The expiry date</param>
        /// <returns>The assay or null if none match</returns>
        public override IAssay GetAssayForBarcode(string barcode, out string assayCode, 
            out DateTime expiryDate)
        {
            // Validate the barcode length and content
            int expiryCode;

            if ((barcode.Length != 20) ||
                (int.TryParse(barcode.Substring(8, 3), out expiryCode) == false))
            {
                throw new ApplicationException("Invalid barcode");
            }

            // Extract the expiry date from the barcode
            expiryDate = new DateTime(2014, 1, 1).AddMonths(expiryCode - 1);

            // Extract the code from the barcode
            var assayCodeString = barcode.Substring(0, 3);

            // Set the out parameter
            assayCode = assayCodeString;

            return this.Where(x => x.Code == assayCodeString).FirstOrDefault();
        }

        /// <summary>
        /// The types for serialisation
        /// </summary>
        public override Type[] Types
        {
            get
            {
                return new Type[] { typeof(Assay), typeof(Disease) };
            }
        }
    }
}
