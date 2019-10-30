/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

namespace IO.Analysis
{
    /// <summary>
    /// Analysis data object
    /// </summary>
    public class AnalysisData
    {
        /// <summary>
        /// The name of the analysis data
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The potentiostat to which this data applies
        /// </summary>
        public int Potentiostat { get; set; }

        /// <summary>
        /// The starting potential in millivolts
        /// </summary>
        public double StartPotential { get; set; }

        /// <summary>
        /// The incremental potential in millivolts
        /// </summary>
        public double IncrementalPotential { get; set; }

        /// <summary>
        /// The array of current differences in nanoamps
        /// </summary>
        public double[] CurrentDifferences { get; set; }

        /// <summary>
        /// The array of peaks
        /// </summary>
        public PeakData[] Peaks { get; set; }
    }
}
