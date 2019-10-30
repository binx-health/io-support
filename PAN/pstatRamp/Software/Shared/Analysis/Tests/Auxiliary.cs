/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IO.Analysis
{
    /// <summary>
    /// Auxiliary functions for tests
    /// </summary>
    internal static class Auxiliary
    {
        /// <summary>
        /// Analysis data parameters
        /// </summary>
        private static readonly double MIN_POTENTIAL_VALUE = double.NegativeInfinity;
        private static readonly double MAX_POTENTIAL_VALUE = double.PositiveInfinity;

        /// <summary>
        /// Readt the raw peak data from the passed peak file
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The array of peak data</returns>
        public static PeakData[] ReadDataFromPeaksFile(string filePath)
        {
            // Initialise the peak data list to store the peaks
            var peakDataList = new List<PeakData>();

            // Initialis the text reader from the file path
            using (var textReader = new StreamReader(filePath))
            {
                // Ignore the first line
                textReader.ReadLine();

                // Loop through the file to the first empty line
                string line;

                while (string.IsNullOrEmpty(line = textReader.ReadLine()) == false)
                {
                    // Split the line into comma separated tokens
                    string[] tokens = line.Split(',');

                    // Add the peak data to the list
                    peakDataList.Add(new PeakData()
                    {
                        Type = (PeakType)Enum.Parse(typeof(PeakType), tokens[0]),
                        Name = tokens[1].Trim(),
                        Potentiostat = int.Parse(tokens[2]),
                        MinPotential = double.Parse(tokens[3]),
                        MaxPotential = double.Parse(tokens[4]),
                        Mean = double.Parse(tokens[5]),
                        Tolerance = double.Parse(tokens[6]),
                        LowerLimit = double.Parse(tokens[7]),
                        UpperLimit = double.Parse(tokens[8]),
                        MaxVarianceForCurveFit = double.Parse(tokens[9]),
                        TopFractionForCurveFit = double.Parse(tokens[10]) / 100.0
                    });
                }
            }

            // Return the list as an array
            return peakDataList.ToArray();
        }

        /// <summary>
        /// Read the raw potentiostat data from the passed txt file
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="peaks">The array of peaks</param>
        /// <returns>The analysis data</returns>
        public static AnalysisData[] ReadDataFromTxtFile(string filePath, PeakData[] peaks)
        {
            // Intialise an array of four sest of analysis data            
            var result = new AnalysisData[1];

            // Initialis the text reader from the file path
            using (var textReader = new StreamReader(filePath))
            {
                // Read the name for the data
                string name = Path.GetFileNameWithoutExtension(filePath);

                // Ignore the first line
                textReader.ReadLine();

                // Loop through the file to the first empty line
                string line;
                var potentials = new List<double>();
                var currentDifferences = new List<double>();

                while (string.IsNullOrEmpty(line = textReader.ReadLine()) == false)
                {
                    // Split the line into comma separated tokens
                    char[] delimiter = { '\t' };
                    string[] tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    // Read the potential
                    double potential = double.Parse(tokens[0]) * 1.0e3;

                    // Only add values above the minumum potential value
                    if ((potential > MIN_POTENTIAL_VALUE) && (potential < MAX_POTENTIAL_VALUE))
                    {
                        potentials.Add(potential);
                        currentDifferences.Add(double.Parse(tokens[1]) * 1.0e9);
                    }
                }

                // Calculate the dimension and check it for validity
                int dimension = Math.Min(potentials.Count, currentDifferences.Count);

                if (dimension < 2)
                {
                    return new AnalysisData[0];
                }

                // Calculate the start, end and incremental potentials
                double startPotential = potentials.First();
                double endPotential = potentials.Last();
                double incrementalPotential = (endPotential - startPotential) / (dimension - 1);

                // Initialising the analasys data
                result[0] = new AnalysisData()
                {
                    Name = name,
                    StartPotential = startPotential,
                    IncrementalPotential = incrementalPotential,
                    CurrentDifferences = currentDifferences.ToArray(),
                    Peaks = peaks,
                };
            }

            // Return the array of analysis data
            return result;
        }

        /// <summary>
        /// Read the raw potentiostat data from the passed dat file
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="peaks">The array of peaks</param>
        /// <returns>The analysis data</returns>
        public static AnalysisData[] ReadDataFromDatFile(string filePath, PeakData[] peaks)
        {
            // Intialise an array of four sest of analysis data            
            var result = new AnalysisData[4];

            // Initialis the text reader from the file path
            using (var textReader = new StreamReader(filePath))
            {
                // Read the name for the data
                string name = Path.GetFileNameWithoutExtension(filePath);

                // Ignore the first two lines
                textReader.ReadLine();
                textReader.ReadLine();

                // Split the next line into comma separated tokens
                string[] tokens = textReader.ReadLine().Split(',');

                // Read the start, end and incremental potentials
                double startPotential = double.Parse(tokens[0]);
                double endPotential = double.Parse(tokens[1]);
                double incrementalPotential = double.Parse(tokens[2]);

                // Calculate the dimension of the data
                int dimension = (int)((endPotential - startPotential) / incrementalPotential) + 1;

                // Loop thorugh the four sets of data initialising them
                for (int index = 0; index < 4; ++index)
                {
                    result[index] = new AnalysisData()
                    {
                        Name = name + " (" + index + ")",
                        StartPotential = startPotential,
                        IncrementalPotential = incrementalPotential,
                        CurrentDifferences = new double[dimension],
                        Peaks = peaks,
                    };
                }

                // Loop through the data in the file reading the values
                for (int index = 0; index < dimension; ++index)
                {
                    // Split the next line into comma separated tokens
                    tokens = textReader.ReadLine().Split(',');

                    result[0].CurrentDifferences[index] = double.Parse(tokens[3]);
                    result[1].CurrentDifferences[index] = double.Parse(tokens[5]);
                    result[2].CurrentDifferences[index] = double.Parse(tokens[8]);
                    result[3].CurrentDifferences[index] = double.Parse(tokens[12]);
                }
            }

            // Return the array of analysis data
            return result;
        }

        /// <summary>
        /// Read the raw potentiostat data from the passed oew file
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="peaks">The array of peaks</param>
        /// <returns>The analysis data</returns>
        public static AnalysisData[] ReadDataFromOewFile(string filePath, PeakData[] peaks)
        {
            // Intialise an array of four sest of analysis data            
            var result = new AnalysisData[1];

            // Initialis the text reader from the file path
            using (var textReader = new StreamReader(filePath))
            {
                // Read the name for the data
                string name = Path.GetFileNameWithoutExtension(filePath);

                // Ignore the firsst two lines
                textReader.ReadLine();
                textReader.ReadLine();

                // Loop through the file to the first empty line
                string line;
                var potentials = new List<double>();
                var currentDifferences = new List<double>();

                while (string.IsNullOrEmpty(line = textReader.ReadLine()) == false)
                {
                    // Split the line into comma separated tokens
                    char[] delimiter = { ' ' };
                    string[] tokens = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    
                    // Read the potential
                    double potential = double.Parse(tokens[0]) * 1.0e3;

                    // Only add values above the minumum potential value
                    if ((potential > MIN_POTENTIAL_VALUE) && (potential < MAX_POTENTIAL_VALUE))
                    {
                        potentials.Add(potential);
                        currentDifferences.Add(double.Parse(tokens[1]) * 1.0e9);
                    }
                }

                // Calculate the dimension and check it for validity
                int dimension = Math.Min(potentials.Count, currentDifferences.Count);

                if (dimension < 2)
                {
                    return new AnalysisData[0];
                }

                // Calculate the start, end and incremental potentials
                double startPotential = potentials.First();
                double endPotential = potentials.Last();
                double incrementalPotential = (endPotential - startPotential) / (dimension - 1);

                // Initialising the analasys data
                result[0] = new AnalysisData()
                {
                    Name = name,
                    StartPotential = startPotential,
                    IncrementalPotential = incrementalPotential,
                    CurrentDifferences = currentDifferences.ToArray(),
                    Peaks = peaks,
                };
            }

            // Return the array of analysis data
            return result;
        }

        /// <summary>
        /// Write the log to a file
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="analysisResult">The analysis result object</param>
        public static void WriteLogFile(string filePath, string log)
        {
            // Initialise the text writer from the file path
            using (var textWriter = new StreamWriter(filePath))
            {
                // Write the log to the file
                textWriter.Write(log);
            }
        }

        /// <summary>
        /// Write value sets to a CSV file
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="start">The value of the start position</param>
        /// <param name="increment">The incremental value</param>
        /// <param name="valueSets">The value sets associated with the indices</param>
        public static void WriteValuesFile(string filePath, 
            double start, 
            double increment, 
            double[][] valueSets)
        {
            // Initialise the text writer from the file path
            using (var textWriter = new StreamWriter(filePath))
            {
                // Calculate the maximum dimension of the data
                int dimension = valueSets.Select(x => x.Length).Max();

                // Loop through the indices
                for (int index = 0; index < dimension; ++index)
                {
                    // Write the index value
                    textWriter.Write(start + (increment * index));

                    // Loop through the sets writing the indexed values
                    foreach (var set in valueSets)
                    {
                        textWriter.Write(", ");

                        // Only write real values
                        if ((set.Length > index) && (double.IsNaN(set[index]) == false))
                        {
                            textWriter.Write(set[index]);
                        }
                    }

                    // Write the end of line character
                    textWriter.WriteLine();
                }
            }
        }
    }
}
