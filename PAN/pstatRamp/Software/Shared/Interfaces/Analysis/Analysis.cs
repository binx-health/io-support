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
    /// Delegate called when analysis is complete
    /// </summary>
    /// <param name="token">The token for the result</param>
    /// <param name="analysisResult">The result of the analysis</param>
    public delegate void AnalysisComplete(int token, AnalysisResult analysisResult);

    /// <summary>
    /// Analysis interface implemented by analysis objects
    /// </summary>
    public abstract class IAnalysis
    {
        /// <summary>
        /// List of known analysis types
        /// </summary>
        public static readonly List<Type> Types = new List<Type>();

        /// <summary>
        /// Create an analysis object
        /// </summary>
        /// <param name="typeName">The type of analysis object</param>
        /// <returns>The requested analysis object or null if the type is not recongised</returns>
        public static IAnalysis Create(string typeName, string[] parameters)
        {
            // Initialise the result
            IAnalysis result = null;

            // For an null parameter return the first known type
            if (string.IsNullOrEmpty(typeName))
            {
                if (Types.Count > 0)
                {
                    result = (IAnalysis)Activator.CreateInstance(Types[0]);
                }
            }
            else
            {

                // Strip off the IO namespace
                if (typeName.StartsWith("IO."))
                {
                    typeName = typeName.Substring(3);
                }

                // Strip off the Analyisis namespace
                if (typeName.StartsWith("Analyisis."))
                {
                    typeName = typeName.Substring(10);
                }

                // Loop through the types looking for a match
                foreach (var type in Types)
                {
                    if (type.Name == typeName)
                    {
                        result = (IAnalysis)Activator.CreateInstance(type);
                    }
                }
            }

            // If we have an analysis object then set the parameters
            if ((result != null) && (result.SetParameters(parameters) == false))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Method for setting parameters
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>True for success, false otherwise</returns>
        public abstract bool SetParameters(string[] parameters);

        /// <summary>
        /// Analyse the data on a worker thread and callback once complete
        /// </summary>
        /// <param name="analysisData">The data to analyse</param>
        /// <param name="callback">The callback routine</param>
        /// <returns>The thread token</returns>
        public abstract int Analyse(AnalysisData analysisData,
            AnalysisComplete callback);
    }
}
