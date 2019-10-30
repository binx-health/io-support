/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using IO.Scripting;

namespace IO.View.Tests
{
    /// <summary>
    /// Assay object for the test
    /// </summary>
    public class Assay : IAssay
    {
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
    }
}
