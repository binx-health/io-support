/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Configuration interface
    /// </summary>
    public abstract class IConfiguration
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IConfiguration Instance { get; set; }

        /// <summary>
        /// Modified flag
        /// </summary>
        [XmlIgnore]
        public virtual bool Modified { get; set; }

        /// <summary>
        /// The types for serialisation
        /// </summary>
        public abstract Type[] Types { get; }

        /// <summary>
        /// The auto logoff period in seconds
        /// </summary>
        public virtual int AutoLogoffPeriodInSeconds { get; set; }

        /// <summary>
        /// The current locale
        /// </summary>
        public virtual string Locale { get; set; }

        /// <summary>
        /// The instrument name
        /// </summary>
        public virtual string InstrumentName { get; set; }

        /// <summary>
        /// The password rules
        /// </summary>
        public virtual IPasswordRules PasswordRules { get; set; }

        /// <summary>
        /// The available storage
        /// </summary>
        public abstract long Storage { get; }

        /// <summary>
        /// The list of fields
        /// </summary>
        public virtual List<IField> Fields { get; set; }

        /// <summary>
        /// The developer port
        /// </summary>
        public virtual int DeveloperPort { get; set; }

        /// <summary>
        /// The ftp server URI
        /// </summary>
        public virtual string FtpServerUri { get; set; }

        /// <summary>
        /// The ftp server user name
        /// </summary>
        public virtual string FtpServerUserName { get; set; }

        /// <summary>
        /// The ftp server password
        /// </summary>
        public virtual string FtpServerPassword { get; set; }

        /// <summary>
        /// The UNG setting
        /// </summary>
        public virtual Boolean Ung { get; set; }

        /// <summary>
        /// The auto sample ID setting
        /// </summary>
        public virtual Boolean AutoSampleId { get; set; }

        /// <summary>
        /// The POCT1.A server URI
        /// </summary>
        public virtual string Poct1ServerUri { get; set; }

        /// <summary>
        /// The POCT1.A server port
        /// </summary>
        public virtual int Poct1ServerPort { get; set; }

        /// <summary>
        /// The printer port
        /// </summary>
        public virtual string PrinterPort { get; set; }

        /// <summary>
        /// The QC test frequency
        /// </summary>
        public virtual QcTestFrequency QcTestFrequency { get; set; }

        /// <summary>
        /// The quarantine state
        /// </summary>
        public virtual QuarantineState QuarantineState { get; set; }

        /// <summary>
        /// The date and time for the last good QC test
        /// </summary>
        [XmlIgnore]
        public virtual DateTime QcTestDateTime { get; set; }
        [XmlElement("QcTestDateTime")]
        public abstract string QcTestDateTimeString { get; set; }

        /// <summary>
        /// The number of PCR cycles executed on this instrument
        /// </summary>
        public virtual int PcrCycles { get; set; }

        /// <summary>
        /// The number of PCR cycles that should trigger a warning to the user
        /// </summary>
        public virtual int WarningPcrCycles { get; set; }

        /// <summary>
        /// The reader serial number
        /// </summary>
        public virtual string SerialNumber { get; set; }

        /// <summary>
        /// Arbitrary block of manufacturing data
        /// </summary>
        [XmlAnyElement("Manufacturing")]
        public virtual object Manufacturing { get; set; }
    }
}
