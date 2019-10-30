/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

namespace IO.Analysis
{
    /// <summary>
    /// Peak data object
    /// </summary>
    public class PeakData
    {
        /// <summary>
        /// The peak type
        /// </summary>
        public PeakType Type { get; set; }

        /// <summary>
        /// The name of the peak for logging purposes
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The potentiostat to which this peak applies
        /// </summary>
        public int Potentiostat { get; set; }

        /// <summary>
        /// The min potential for the peak in millivolts
        /// </summary>
        public double MinPotential { get; set; }

        /// <summary>
        /// The max potential for the peak in millivolts
        /// </summary>
        public double MaxPotential { get; set; }

        /// <summary>
        /// The mean potential of the peak in millivolts
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// The tolerance potential for the peak in millivolts
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// The lower limit for the peak in nanoamps
        /// </summary>
        public double LowerLimit { get; set; }

        /// <summary>
        /// The upper limit for the peak in nanoamps
        /// </summary>
        public double UpperLimit { get; set; }

        /// <summary>
        /// The maximum allowable variance for curve fitting
        /// </summary>
        public double MaxVarianceForCurveFit { get; set; }

        /// <summary>
        /// The fraction of the peak data used to fit the curve
        /// </summary>
        public double TopFractionForCurveFit { get; set; }
    }
}
