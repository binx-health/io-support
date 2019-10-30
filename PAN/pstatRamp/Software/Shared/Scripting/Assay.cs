/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace IO.Scripting
{
    /// <summary>
    /// Assay object
    /// </summary>
    public class Assay : IAssay
    {
        /// <summary>
        /// Range of assay codes for investigation use only
        /// </summary>
        private const int INVESTIGATION_USE_ONLY_CODE_FIRST = 930;
        private const int INVESTIGATION_USE_ONLY_CODE_LAST = 999;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Assay()
        {
            Diseases = new List<IDisease>();
        }

        /// <summary>
        /// The metrics for the assay
        /// </summary>
        [XmlIgnore]
        public override IMetrics Metrics
        {
            get
            {
                // Try to get the metrics from the cache
                Metrics metrics;

                if (IO.Scripting.Metrics.NonDefault.TryGetValue(MetricsName, out metrics) == false)
                {
                    // If they are not in the cache then create a new set
                    IO.Scripting.Metrics.NonDefault.Add(MetricsName, metrics =
                        new Metrics(MetricsName, IDefaultMetrics.Instance));
                }

                return metrics;
            }
        }

        /// <summary>
        /// Investigation use only
        /// </summary>
        [XmlIgnore]
        public override bool InvestigationUseOnly
        {
            get
            {
                // Parse the code as an integer and check the value
                int code;

                return (string.IsNullOrEmpty(Code) == false) &&
                    int.TryParse(Code, out code) &&
                    (code >= INVESTIGATION_USE_ONLY_CODE_FIRST) &&
                    (code <= INVESTIGATION_USE_ONLY_CODE_LAST);
            }
        }


        /// <summary>
        /// Add a new disease
        /// </summary>
        /// <param name="peakName">The peak name</param>
        /// <returns>The new disease</returns>
        public override IDisease AddDisease(string peakName)
        {
            // Create the new disease
            var disease = new Disease() { PeakName = peakName };

            // Add it to the list
            Diseases.Add(disease);

            return disease;
        }
    }
}
