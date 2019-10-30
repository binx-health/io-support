/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Xml.Serialization;
using IO.Scripting;

namespace IO.View.Tests
{
    /// <summary>
    /// Assay interface
    /// </summary>
    [XmlRoot("Assays")]
    public class Assays : IAssays
    {
        /// <summary>
        /// Add a new assay
        /// </summary>
        /// <param name="name">The assay name</param>
        /// <returns>The new assay</returns>
        public override IAssay AddNew(string name)
        {
            // Check for an assay of the same name
            if (string.IsNullOrEmpty(name) || Exists(x => x.Name == name))
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
        /// <param name="expiryDate">The expiry date</param>
        /// <returns>The assay or null if none match</returns>
        public override IAssay GetAssayForBarcode(string barcode, out DateTime expiryDate)
        {
            // Validate the barcode length and content
            int manufactureDay;

            if ((barcode.Length != 18) ||
                (int.TryParse(barcode.Substring(1, 4), out manufactureDay) == false))
            {
                throw new ApplicationException("Invalid barcode");
            }

            // Extract the expiry date from the barcode
            expiryDate = new DateTime(2013, 1, 1).AddDays(manufactureDay - 1);

            var expiry = barcode.Substring(6, 1);

            if (expiry == "1")
            {
                expiryDate = expiryDate.AddMonths(6);
            }
            else if (expiry == "2")
            {
                expiryDate = expiryDate.AddYears(1);
            }
            else if (expiry == "3")
            {
                expiryDate = expiryDate.AddMonths(18);
            }
            else if (expiry == "4")
            {
                expiryDate = expiryDate.AddYears(2);
            }
            else if (expiry == "5")
            {
                expiryDate = expiryDate.AddYears(3);
            }
            else if (expiry == "6")
            {
                expiryDate = expiryDate.AddYears(4);
            }
            else if (expiry == "7")
            {
                expiryDate = expiryDate.AddYears(5);
            }
            else if (expiry == "8")
            {
                expiryDate = expiryDate.AddYears(6);
            }
            else if (expiry == "9")
            {
                expiryDate = expiryDate.AddYears(7);
            }

            // Extract the code from the barcode
            var code = barcode.Substring(7, 3);

            return this.Where(x => x.Code == code).FirstOrDefault();
        }

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
    }
}
