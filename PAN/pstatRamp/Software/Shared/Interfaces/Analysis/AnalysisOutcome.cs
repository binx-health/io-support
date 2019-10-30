/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

namespace IO.Analysis
{
    /// <summary>
    /// Possible outcomes of analysis
    /// </summary>
    public enum AnalysisOutcome
    {
        Null,           // No analysis possible
        Positive,       // Positive outcome
        Negative,       // Negative outcome
        Rescan,         // Data should be rescanned
        Undefined       // No result
    }
}
