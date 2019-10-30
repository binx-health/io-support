/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Test data interface
    /// </summary>
    public abstract class ITestData
    {
        /// <summary>
        /// The test object that owns this data
        /// </summary>
        [XmlIgnore]
        public virtual ITest Owner { get; set; }

        /// <summary>
        /// The log generated while the test is running
        /// </summary>
        public virtual string Log { get; set; }

        /// <summary>
        /// The starting potential in millivolts
        /// </summary>
        public virtual double StartPotential { get; set; }

        /// <summary>
        /// The incremental potential in millivolts
        /// </summary>
        public virtual double IncrementalPotential { get; set; }

        /// <summary>
        /// The current differentials for all four potentiostats
        /// </summary>
        [XmlIgnore]
        public readonly double[][] CellValues = new double[4][];

        /// <summary>
        /// The current differentials for potentiostat 1
        /// </summary>
        [XmlElement("Cell1Values")]
        public abstract string Cell1ValuesString { get; set; }

        /// <summary>
        /// The current differentials for potentiostat 2
        /// </summary>
        [XmlElement("Cell2Values")]
        public abstract string Cell2ValuesString { get; set; }

        /// <summary>
        /// The current differentials for potentiostat 3
        /// </summary>
        [XmlElement("Cell3Values")]
        public abstract string Cell3ValuesString { get; set; }

        /// <summary>
        /// The current differentials for potentiostat 4
        /// </summary>
        [XmlElement("Cell4Values")]
        public abstract string Cell4ValuesString { get; set; }

        /// <summary>
        /// The analysis type required
        /// </summary>
        public virtual string AnalysisType { get; set; }

        /// <summary>
        /// The fast reporting period
        /// </summary>
        public virtual int FastReportingPeriod { get; set; }

        /// <summary>
        /// List of PCR values
        /// </summary>
        [XmlIgnore]
        public List<double> PcrValues { get; set; }

        /// <summary>
        /// The temperature values for PCR
        /// </summary>
        [XmlElement("PcrValues")]
        public abstract string PcrValuesString { get; set; }
    }
}
