/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using IO.Scripting;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Test result interface
    /// </summary>
    public abstract class ITestResult
    {
        /// <summary>
        /// The test object that owns this data
        /// </summary>
        [XmlIgnore]
        public virtual ITest Owner { get; set; }

        /// <summary>
        /// The sample ID entered by the operator
        /// </summary>
        public virtual string SampleId { get; set; }

        /// <summary>
        /// The patient information
        /// </summary>
        [XmlIgnore]
        public virtual Dictionary<string, object> PatientInformation { get; set; }

        /// <summary>
        /// The QC test flag
        /// </summary>
        public virtual bool QcTest { get; set; }

        /// <summary>
        /// The start date and time for the test
        /// </summary>
        [XmlIgnore]
        public virtual DateTime StartDateTime { get; set; }
        [XmlElement("StartDateTime")]
        public abstract string StartDateTimeString { get; set; }

        /// <summary>
        /// The end date and time for the test
        /// </summary>
        [XmlIgnore]
        public virtual DateTime EndDateTime { get; set; }
        [XmlElement("EndDateTime")]
        public abstract string EndDateTimeString { get; set; }

        /// <summary>
        /// The assay
        /// </summary>
        [XmlIgnore]
        public virtual IAssay Assay { get; set; }
        [XmlElement("Assay")]
        public abstract string AssayName { get; set; }

        /// <summary>
        /// The assay version used
        /// </summary>
        public virtual int AssayVersion { get; set; }

        /// <summary>
        /// The data on the barcode on the cartridge
        /// </summary>
        public virtual string CartridgeData { get; set; }

        /// <summary>
        /// Flag to indicate that the instrument was out of tolerance
        /// </summary>
        public virtual bool CalibrationOutsideTolerance { get; set; }

        /// <summary>
        /// The outcome of the test
        /// </summary>
        public virtual TestOutcome Outcome { get; set; }

        /// <summary>
        /// The comma separated peak names for peaks that were detected
        /// </summary>
        public virtual string PositivePeaks { get; set; }

        /// <summary>
        /// The comma separated peak names for peaks that were not detected
        /// </summary>
        public virtual string NegativePeaks { get; set; }

        /// <summary>
        /// The comma separated peak names for invalid peaks
        /// </summary>
        public virtual string InvalidPeaks { get; set; }
    }
}
