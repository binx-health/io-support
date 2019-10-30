/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

namespace IO.Analysis
{
    /// <summary>
    /// Data object defining Gaussian curve parameters
    /// </summary>
    public class GaussianParameters : IPeakParameters
    {
        /// <summary>
        /// The standard deviation
        /// </summary>
        public double StandardDeviation { get; set; }

        /// <summary>
        /// The mean before the skew is applied
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// The absolute height of the peak
        /// </summary>
        public override double Height { get; set; }

        /// <summary>
        /// The skew value
        /// </summary>
        public double Skew { get; set; }

        /// <summary>
        /// The actual peak position
        /// </summary>
        public override double Position
        {
            get
            {
                return this.SkewGaussianPeakPosition();
            }
        }
    }
}
