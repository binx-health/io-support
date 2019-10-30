/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace IO.Analysis
{
    /// <summary>
    /// Default analysis thread object
    /// </summary>
    internal class DefaultAnalysisThread
    {
        /// <summary>
        /// The token for this thread
        /// </summary>
        private int token;

        /// <summary>
        /// String builder for the log file
        /// </summary>
        private StringBuilder logStringBuilder = new StringBuilder();

        /// <summary>
        /// The analysis data
        /// </summary>
        public AnalysisData Data { get; set; }

        /// <summary>
        /// The callback function
        /// </summary>
        public AnalysisComplete Callback { get; set; }

        /// <summary>
        /// The maximum allowable variance for the raw data in DB
        /// </summary>
        public double MaxVarianceForRawData { get; set; }

        /// <summary>
        /// The maximum allowable variance for the polynomial curve in DB
        /// </summary>
        public double MaxVarianceForPolynomial { get; set; }

        /// <summary>
        /// Start the thread to process the data
        /// </summary>
        /// <returns>The token for the thread</returns>
        public int Start()
        {
            // Create a new thread
            var thread = new Thread(ThreadProcedure);
            
            // Set the token to the managed thread ID
            token = thread.ManagedThreadId;

            // Start the thread
            thread.Start();

            // Return the token
            return token;
        }

        /// <summary>
        /// Thread procedure to analyse the data
        /// </summary>
        private void ThreadProcedure()
        {
            // Create an analysis result object
            var analysisResult = new AnalysisResult() { Data = Data };

            if (Data == null)
            {
                logStringBuilder.LogLine("No analysis data for potentiostat " + Data.Potentiostat);
            }
            else
            {
                logStringBuilder.LogLine("Analysis starting for potentiostat " + Data.Potentiostat);

                // Lock the data for the duration of the processing
                lock (Data)
                {
                    // Analyse the data
                    AnalyseData(analysisResult);
                }

                logStringBuilder.LogLine("Analysis done for potentiostat " + Data.Potentiostat);
            }

            // If we have a callback then call it with the results of the analysis
            if (Callback != null)
            {
                // Copy the log string into the result
                analysisResult.Log = logStringBuilder.ToString();

                // Do the callback
                Callback(token, analysisResult);
            }
        }

        /// <summary>
        /// Main data analysis function called on the worker thread
        /// </summary>
        /// <param name="values">The analysis result</param>
        private void AnalyseData(AnalysisResult analysisResult)
        {
            // List of datasets for the analysis result
            var dataSets = new List<double[]>();

            try
            {
                // Append the raw data to the datasests
                dataSets.Add(Data.CurrentDifferences);

                // Apply Savitsky-Golay smoothing to the raw data
                var smoothedRawData = Data.CurrentDifferences.SavitskyGolay(
                    DefaultAnalysis.RAW_DATA_SG_SMOOTHING_WIDTH,
                    DefaultAnalysis.RAW_DATA_SG_SMOOTHING_DEGREE,
                    logStringBuilder);

                // Check for a smoothing error
                if (smoothedRawData == null)
                {
                    return;
                }

                // Append the smoothed data to the datasests
                dataSets.Add(smoothedRawData);

                // Calculate the variance for the smoothed signal on the raw data
                analysisResult.VarianceForRawData = smoothedRawData.Variance(Data.CurrentDifferences);

                // Check for a lack of data
                if (double.IsNaN(analysisResult.VarianceForRawData))
                {
                    logStringBuilder.LogLine("Variance for raw data cannot be calculated");
                    return;
                }

                // Check this value against the minimum allowable value
                if (analysisResult.VarianceForRawData > MaxVarianceForRawData)
                {
                    logStringBuilder.LogLine("Variance for raw data of " + analysisResult.VarianceForRawData +
                        " is more than the maximum of " + MaxVarianceForRawData);
                    return;
                }

                // Log the variance value
                logStringBuilder.LogLine("Variance for raw data is " + analysisResult.VarianceForRawData);

                // Initialis a variable for the variance
                double variance;

                // Fit the lower polynomial
                var polynomialData = Data.CurrentDifferences.LowerCurveFit(Data, logStringBuilder,
                    MaxVarianceForPolynomial, out variance);

                // Check for a failed curve fit
                if (polynomialData == null)
                {
                    logStringBuilder.LogLine("Failed to fit a polynomial to the smoothed data");
                    return;
                }

                // Set the variance in the result
                analysisResult.VarianceForPolynomial = variance;
                
                // Append the polynomial data to the datasests
                dataSets.Add(polynomialData);

                // Check for a lack of data
                if (double.IsNaN(analysisResult.VarianceForPolynomial))
                {
                    logStringBuilder.LogLine("Variance for polynomial cannot be calculated");
                    return;
                }

                // Check this value against the minimum allowable value
                if (analysisResult.VarianceForPolynomial > MaxVarianceForPolynomial)
                {
                    logStringBuilder.LogLine("Variance for polynomial of " + analysisResult.VarianceForPolynomial +
                        " is more than the maximum of " + MaxVarianceForPolynomial);
                    return;
                }

                // Log the variance value
                logStringBuilder.LogLine("Variance for polynomial is " + analysisResult.VarianceForPolynomial);

                // Calculate the isolated peak data by subtracting the polynomial from the smoothed
                // raw data
                var isolatedPeakData = Data.CurrentDifferences.Minus(polynomialData);

                // Append the isolated peak data to the datasests
                dataSets.Add(isolatedPeakData);

                // Allocate the array to store the peak results
                analysisResult.PeakParameters = new IPeakParameters[Data.Peaks.Length];
                analysisResult.PeakVariances = new double[Data.Peaks.Length];
                analysisResult.PeakOutcomes = new PeakOutcome[Data.Peaks.Length];

                // Loop through the peaks
                for (int index = 0; index < Data.Peaks.Length; ++index)
                {
                    // Get the peak data from the array
                    var peak = Data.Peaks[index];

                    // Fit a skew Gaussian curve to the isolated peak data
                    var gaussianParameters = isolatedPeakData.FitGaussianForPeak(
                        Data.StartPotential, Data.IncrementalPotential,
                        peak, logStringBuilder, out analysisResult.PeakVariances[index]);

                    // If no parameters are returned then the fit is not possible
                    if (gaussianParameters != null)
                    {
                        // Generate the curve data for the analysis datasets
                        var gaussianCurveData = new double[Data.CurrentDifferences.Length];

                        gaussianParameters.SkewGaussianCurve(gaussianCurveData,
                            Data.StartPotential, Data.IncrementalPotential);

                        // Append the skew Gaussian curve to the datasests
                        dataSets.Add(gaussianCurveData);

                        // Set the peak data in the analysis result
                        analysisResult.PeakParameters[index] = gaussianParameters;
                    }

                    // Calculate the outcome for the peak
                    analysisResult.PeakOutcomes[index] = peak.OutcomeForPeak(gaussianParameters,
                        analysisResult.PeakVariances[index], logStringBuilder);
                }
            }
            finally
            {
                // Set the datasets in the analysis result
                analysisResult.DataSets = dataSets.ToArray();
            }
        }
    }
}
