/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.Analysis
{
    /// <summary>
    /// Default analysis object
    /// </summary>
    public class DefaultAnalysis : IAnalysis
    {
        /// <summary>
        /// Constants for the default analysis
        /// </summary>
        public static readonly int RAW_DATA_SG_SMOOTHING_WIDTH = 9;
        public static readonly int RAW_DATA_SG_SMOOTHING_DEGREE = 4;
        public static readonly int SMOOTHED_DATA_POLYNOMIAL_DEGREE = 5;
        private static readonly double MAX_VARIANCE_FOR_RAW_DATA = 25.0;
        private static readonly double MAX_VARIANCE_FOR_POLYNOMIAL = 100.0;

        /// <summary>
        /// The maximum allowable variance for the raw data
        /// </summary>
        private double maxVarianceForRawData;

        /// <summary>
        /// The maximum allowable variance for the polynomial curve
        /// </summary>
        private double maxVarianceForPolynomial;

        /// <summary>
        /// Initialise the default analysis
        /// </summary>
        public static void Initialise()
        {
            // Add this type to the list of available types
            IAnalysis.Types.Add(typeof(DefaultAnalysis));
        }

        /// <summary>
        /// Method for setting parameters
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>True for success, false otherwise</returns>
        public override bool SetParameters(string[] parameters)
        {
            // Loop through the passed parameters
            for (int index = 0; index < parameters.Length; ++index)
            {
                // Check for the first parameter
                if (index == 0)
                {
                    double doubleValue;

                    if (double.TryParse(parameters[index], out doubleValue) == false)
                    {
                        return false;
                    }

                    maxVarianceForRawData = doubleValue;
                }
                // Check for the second parameter
                else if (index == 1)
                {
                    double doubleValue;

                    if (double.TryParse(parameters[index], out doubleValue) == false)
                    {
                        return false;
                    }

                    maxVarianceForPolynomial = doubleValue;
                }
                else
                {
                    // Too many parameters passed
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DefaultAnalysis()
        {
            // Initialise the parameters
            maxVarianceForRawData = MAX_VARIANCE_FOR_RAW_DATA;
            maxVarianceForPolynomial = MAX_VARIANCE_FOR_POLYNOMIAL;
        }

        /// <summary>
        /// Analyse the passed data
        /// </summary>
        /// <param name="analysisData">The data for analysis</param>
        /// <param name="callback">The callback to be called on completion</param>
        /// <returns>The token to recognise the callback</returns>
        public override int Analyse(AnalysisData analysisData,
            AnalysisComplete callback)
        {
            // Create the new analysis thread
            var thread = new DefaultAnalysisThread() 
            { 
                Data = analysisData, 
                Callback = callback,
                MaxVarianceForRawData = maxVarianceForRawData,
                MaxVarianceForPolynomial = maxVarianceForPolynomial,
            };

            // Start the thread
            return thread.Start();
        }
    }
}
