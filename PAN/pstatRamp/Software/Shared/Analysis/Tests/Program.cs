/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace IO.Analysis
{
    /// <summary>
    /// Test suite for default analysis object
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// Variance limits for raw and polynomial data
        /// </summary>
        private static readonly double MAX_VARIANCE_FOR_RAW_DATA = 25.0;
        private static readonly double MAX_VARIANCE_FOR_POLYNOMIAL = 100.0;

        /// <summary>
        /// The number of analysis running
        /// </summary>
        private static volatile int analysisCount = 0;

        /// <summary>
        /// Dixtionary of analysis data by token
        /// </summary>
        private static Dictionary<int, AnalysisResult> tokenToAnalysisResultDictionary = 
            new Dictionary<int, AnalysisResult>();

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Arguments that define the test data to use</param>
        public static void Main(string[] args)
        {
            // Initilaise the default analysis object
            DefaultAnalysis.Initialise();

            // Initialise the analysis data and the peak data variables
            var analysisDatasets = new List<AnalysisData>();
            PeakData[] peaks = null;

            // Check the arguments
            if (args.Length == 2)
            {
                try
                {
                    // Read the peaks
                    peaks = Auxiliary.ReadDataFromPeaksFile(args[0]);

                    // Get all the files that match argument 2
                    string[] files = Directory.GetFiles(Path.GetDirectoryName(args[1]), 
                        Path.GetFileName(args[1]), SearchOption.TopDirectoryOnly);

                    foreach (var file in files)
                    {
                        if (Path.GetExtension(args[1]).ToLower() == ".txt")
                        {
                            analysisDatasets.AddRange(Auxiliary.ReadDataFromTxtFile(file, peaks));
                        }
                        else if (Path.GetExtension(args[1]).ToLower() == ".dat")
                        {
                            analysisDatasets.AddRange(Auxiliary.ReadDataFromDatFile(file, peaks));
                        }
                        else
                        {
                            analysisDatasets.AddRange(Auxiliary.ReadDataFromOewFile(file, peaks));
                        }
                    }
                }
                catch (Exception exception)
                {
                    // Write any exceptions to the console
                    Console.WriteLine(exception.Message);
                    analysisDatasets = null;
                }
            }
            else
            {
                Console.WriteLine("The syntax of the command is incorrect");
                return;
            }

            // Check for bad data
            if (analysisDatasets == null)
            {
                Console.WriteLine("The syntax of the data is incorrect");
                return;
            }

            // Lock the data and start the analysis
            lock (tokenToAnalysisResultDictionary)
            {
                // Create the default analysis object
                var analysis = IAnalysis.Create(null,
                    new string[] { MAX_VARIANCE_FOR_RAW_DATA.ToString(), MAX_VARIANCE_FOR_POLYNOMIAL.ToString() });

                // Loop through the datasets kicking off the analysis thread
                foreach (var analysisData in analysisDatasets)
                {
                    // Lock the data to avoid the thread comlpleting to early
                    lock (analysisData)
                    {
                        // Kick-off the analysis and set the thread token in the dictionary
                        tokenToAnalysisResultDictionary[analysis.Analyse(analysisData, Callback)] = null;
                        
                        // Increment the thread count
                        ++analysisCount;
                    }
                }
            }

            // Wait for the analysis to complete
            while (analysisCount > 0)
            {
                Thread.Sleep(0);
            }

            var directory = Path.GetDirectoryName(args[1]) + Path.DirectorySeparatorChar;

            // Initialise the text writer from the file path
            using (var summaryTextWriter = new StreamWriter(directory + "Summary.csv"))
            {
                // Write the header for the summary file
                for (int index = 0; index < peaks.Length; ++index)
                {
                    summaryTextWriter.Write(",,, " + peaks[index].Name + ",, ");
                }

                summaryTextWriter.WriteLine();
                summaryTextWriter.Write("Name, Raw variance, Polynomial variance");

                for (int index = 0; index < peaks.Length; ++index)
                {
                    summaryTextWriter.Write(", height, position, width, skew, variance");
                }

                summaryTextWriter.WriteLine();

                // Loop through the analysis results writing the summary file, log and datasets
                foreach (var analysisResult in tokenToAnalysisResultDictionary)
                {
                    summaryTextWriter.Write(analysisResult.Value.Data.Name);
                    summaryTextWriter.Write(", ");

                    if (double.IsNaN(analysisResult.Value.VarianceForRawData) == false)
                    {
                        summaryTextWriter.Write(analysisResult.Value.VarianceForRawData.ToString("F2"));
                    }
                    
                    summaryTextWriter.Write(", ");

                    if (double.IsNaN(analysisResult.Value.VarianceForPolynomial) == false)
                    {
                        summaryTextWriter.Write(analysisResult.Value.VarianceForPolynomial.ToString("F2"));
                    }

                    for (int index = 0; index < peaks.Length; ++index)
                    {
                        summaryTextWriter.Write(", ");

                        if ((analysisResult.Value.PeakParameters != null) &&
                            (analysisResult.Value.PeakParameters[index] != null))
                        {
                            var parameters = analysisResult.Value.PeakParameters[index];
                            var gaussianParameters = (parameters is GaussianParameters) ?
                                (GaussianParameters)parameters : null;

                            summaryTextWriter.Write(parameters.Height.ToString("F2"));
                            summaryTextWriter.Write(", ");
                            summaryTextWriter.Write(parameters.Position.ToString("F2"));
                            summaryTextWriter.Write(", ");

                            if (gaussianParameters != null)
                            {
                                summaryTextWriter.Write((gaussianParameters.StandardDeviation * 4.0).ToString("F2"));
                            }

                            summaryTextWriter.Write(", ");

                            if (gaussianParameters != null)
                            {
                                summaryTextWriter.Write(gaussianParameters.Skew.ToString("F2"));
                            }
                        }
                        else
                        {
                            summaryTextWriter.Write(", ");
                            summaryTextWriter.Write(", ");
                            summaryTextWriter.Write(", ");
                        }

                        summaryTextWriter.Write(", ");

                        if ((analysisResult.Value.PeakVariances != null) &&
                            (double.IsNaN(analysisResult.Value.PeakVariances[index]) == false))
                        {
                            summaryTextWriter.Write(analysisResult.Value.PeakVariances[index].ToString("F2"));
                        }
                    }

                    summaryTextWriter.WriteLine();

                    // Write the log to file
                    Auxiliary.WriteLogFile(directory + analysisResult.Value.Data.Name + ".log",
                        analysisResult.Value.Log);

                    // Write the data sets to file
                    Auxiliary.WriteValuesFile(directory + analysisResult.Value.Data.Name + ".csv",
                        analysisResult.Value.Data.StartPotential,
                        analysisResult.Value.Data.IncrementalPotential,
                        analysisResult.Value.DataSets);
                }
            }
        }

        /// <summary>
        /// The callback from the analysis result object
        /// </summary>
        /// <param name="token">The token for the analysis result</param>
        /// <param name="result">The analysis result</param>
        private static void Callback(int token,
            AnalysisResult result)
        {
            lock (tokenToAnalysisResultDictionary)
            {
                // Check that the tokens match and set the result value
                if (tokenToAnalysisResultDictionary.ContainsKey(token))
                {
                    tokenToAnalysisResultDictionary[token] = result;

                    // Decrement the thread count
                    --analysisCount;
                }
            }
        }
    }
}
