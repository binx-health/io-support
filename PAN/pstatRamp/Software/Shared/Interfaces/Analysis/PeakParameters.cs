/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

namespace IO.Analysis
{
    /// <summary>
    /// Peak parameters interface
    /// </summary>
    public abstract class IPeakParameters
    {
        /// <summary>
        /// The absolute height of the peak
        /// </summary>
        public abstract double Height { get; set; }

        /// <summary>
        /// The actual peak position
        /// </summary>
        public abstract double Position { get; }
    }
}
