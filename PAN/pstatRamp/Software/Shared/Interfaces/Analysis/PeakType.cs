/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

namespace IO.Analysis
{
    /// <summary>
    /// Enumeration of peak criteria
    /// </summary>
    public enum PeakType
    {
        Ignore,         // Ignore the peak data
        Rescan,         // Rescan the data if the peak height is above the upper threshold
        Positive,       // Detection peak for named disease or control
    }
}
