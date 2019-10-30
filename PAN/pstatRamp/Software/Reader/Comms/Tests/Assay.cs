/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using IO.Scripting;

namespace IO.Comms.Tests
{
    /// <summary>
    /// Assay object for the test
    /// </summary>
    public class Assay : IAssay
    {
        /// <summary>
        /// Modified flag
        /// </summary>
        [XmlIgnore]
        public override bool Modified
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The metrics for the assay
        /// </summary>
        [XmlIgnore]
        public override IMetrics Metrics
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The root script for the assay
        /// </summary>
        [XmlIgnore]
        public override IScript Script
        {
            get
            {
                return null;
            }
        }
    }
}
