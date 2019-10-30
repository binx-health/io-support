/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.Analysis;

namespace IO.Analysis
{
    /// <summary>
    /// Default analysis object for tests
    /// </summary>
    public class DefaultAnalysis : IAnalysis
    {
        /// <summary>
        /// Method for setting parameters
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>True for success, false otherwise</returns>
        public override bool SetParameters(string[] parameters)
        {
            return true;
        }

        /// <summary>
        /// Analyse the data on a worker thread and callback once complete
        /// </summary>
        /// <param name="analysisData">The data to analyse</param>
        /// <param name="callback">The callback routine</param>
        /// <returns>The thread token</returns>
        public override int Analyse(AnalysisData analysisData,
            AnalysisComplete callback)
        {
            return 0;
        }
    }
}
