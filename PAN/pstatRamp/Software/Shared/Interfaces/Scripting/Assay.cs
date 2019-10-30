/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IO.Scripting
{
    /// <summary>
    /// Assay interface
    /// </summary>
    public abstract class IAssay: IEquatable<IAssay>
    {
        /// <summary>
        /// Modified flag
        /// </summary>
        [XmlIgnore]
        public virtual bool Modified { get; set; }

        /// <summary>
        /// Loaded flag
        /// </summary>
        [XmlIgnore]
        public virtual bool Loaded { get; set; }

        /// <summary>
        /// The metrics for the assay
        /// </summary>
        [XmlIgnore]
        public abstract IMetrics Metrics { get; }
        [XmlElement("Metrics")]
        public virtual string MetricsName { get; set; }

        /// <summary>
        /// Investigation use only
        /// </summary>
        [XmlIgnore]
        public abstract bool InvestigationUseOnly { get; }

        /// <summary>
        /// The root script for the assay
        /// </summary>
        public virtual string Script { get; set; }

        /// <summary>
        /// The root UNG script for the assay
        /// </summary>
        public virtual string UngScript { get; set; }

        /// <summary>
        /// The root voltammetry script for the assay
        /// </summary>
        public virtual string VoltammetryScript { get; set; }

        /// <summary>
        /// The name for the assay
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The name for the assay
        /// </summary>
        public virtual int Version { get; set; }

        /// <summary>
        /// The short name for the assay
        /// </summary>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// The 3 digit code for the assay
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// The estimated test duration
        /// </summary>
        public virtual int EstimatedDuration { get; set; }

        /// <summary>
        /// The list of diseases for this assay
        /// </summary>
        public virtual List<IDisease> Diseases { get; set; }

        /// <summary>
        /// The signature for this assay
        /// </summary>
        public virtual string Signature { get; set; }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="assay"></param>
        /// <returns></returns>
        public virtual bool Equals(IAssay assay)
        {
            return (MetricsName == assay.MetricsName) &&
                (Script == assay.Script) &&
                (UngScript == assay.UngScript) &&
                (VoltammetryScript == assay.VoltammetryScript) &&
                (Name == assay.Name) &&
                (Version == assay.Version) &&
                (ShortName == assay.ShortName) &&
                (Code == assay.Code) &&
                (EstimatedDuration == assay.EstimatedDuration);
        }

        /// <summary>
        /// Add a new disease
        /// </summary>
        /// <param name="peakName">The peak name</param>
        /// <returns>The new disease</returns>
        public abstract IDisease AddDisease(string peakName);
    }
}
