/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Singleton configuration
    /// </summary>
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// Default values
        /// </summary>
        private static readonly int DEFAULT_AUTO_LOGOFF_PERIOD_IN_SECONDS = 60;
        private static readonly string DEFAULT_LOCALE = "English";
        private static readonly string DEFAULT_INSTRUMENT_NAME = "IO";
        private static readonly int DEFAULT_DEVELOPER_PORT = 443;

        /// <summary>
        /// The types for serialisation
        /// </summary>
        public override Type[] Types
        {
            get
            {
                return new Type[] { typeof(Field), typeof(PasswordRules) };
            }
        }

        /// <summary>
        /// The available storage
        /// </summary>
        public override long Storage
        {
            get
            {
                // Get the drive info for the system folder
                string drive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
                var driveInfo = new DriveInfo(drive);

                return driveInfo.AvailableFreeSpace;
            }
        }

        /// <summary>
        /// The date and time for the last good QC test
        /// </summary>
        [XmlElement("QcTestDateTime")]
        public override string QcTestDateTimeString
        {
            get
            {
                // Check for the default value
                if (QcTestDateTime == DateTime.MinValue)
                {
                    return null;
                }

                return QcTestDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                QcTestDateTime = DateTime.Parse(value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Configuration()
        {
            // Set the default values
            AutoLogoffPeriodInSeconds = DEFAULT_AUTO_LOGOFF_PERIOD_IN_SECONDS;
            Locale = DEFAULT_LOCALE;
            InstrumentName = DEFAULT_INSTRUMENT_NAME;
            PasswordRules = new PasswordRules();
            Fields = new List<IField>();
            DeveloperPort = DEFAULT_DEVELOPER_PORT;
        }
    }
}
