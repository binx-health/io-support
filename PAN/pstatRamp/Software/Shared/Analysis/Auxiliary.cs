/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace IO.Analysis
{
    /// <summary>
    /// Auxiliary functions for analysis
    /// </summary>
    internal static class Auxiliary
    {
        /// <summary>
        /// Constants used for calculation of gaussian values
        /// </summary>
        private static readonly double ROOT_TWO = Math.Sqrt(2.0);
        private static readonly double ROOT_TWO_PI = Math.Sqrt(2.0 * Math.PI);
        private static readonly double ROOT_TWO_OVER_PI = Math.Sqrt(2.0 / Math.PI);

        /// <summary>
        /// Error function approximation constants
        /// </summary>
        private static readonly double ERF_COEFFICIENT_1 = 0.254829592;
        private static readonly double ERF_COEFFICIENT_2 = -0.284496736;
        private static readonly double ERF_COEFFICIENT_3 = 1.421413741;
        private static readonly double ERF_COEFFICIENT_4 = -1.453152027;
        private static readonly double ERF_COEFFICIENT_5 = 1.061405429;
        private static readonly double ERF_FACTOR = 0.3275911;

        /// <summary>
        /// The multiplier for the standard deviation that defines peak range
        /// </summary>
        private static readonly double STANDARD_DEVIATION_MULTIPLIER_FOR_WIDTH = 4.0;

        /// <summary>
        /// Initial error parameters for skew Gaussian curve fit
        /// </summary>
        private static readonly double INITIAL_STANDARD_DEVIATION_DIVISOR_FOR_ERROR = 2.0;
        private static readonly double INITIAL_MEAN_DIVISOR_FOR_ERROR = 2.0;
        private static readonly double INITIAL_HEIGHT_DIVISOR_FOR_ERROR = 10.0;
        private static readonly double INITIAL_SKEW_ERROR = 2.0;

        /// <summary>
        /// Constants defining the iterative behaviour of the lower curve fit
        /// </summary>
        private static readonly int MAX_LOWER_CURVE_FIT_ITERATIONS = 100;

        /// <summary>
        /// Constants defining the iterative behaviour of the skew Gaussian curve fit
        /// </summary>
        private static readonly int MAX_PASS_ITERATIONS = 150;
        private static readonly int VALUE_ITERATIONS = 10;
        private static readonly double MAX_ERROR_FOR_CONVERGED_VARIANCE = 0.1;
        private static readonly double MAX_ERROR_FOR_CONVERGED_PEAK_POSITION = 0.01;

        /// <summary>
        /// Savitsky-Golay constants for smoothing
        /// </summary>
        private static Dictionary<int, int[]> savitskyGolayQuadraticConstantsByWidth = 
            new Dictionary<int, int[]>() 
        { 
            { 7, new int[] { -2, 3, 6, 7, 6, 3, -2 } },
            { 9, new int[] { -21, 14, 39, 54, 59, 54, 39, 14, -21 } }
        };
        private static Dictionary<int, int[]> savitskyGolayQuarticConstantsByWidth = new 
            Dictionary<int, int[]>()
        {
            { 7, new int[] { 5, -30, 75, 131, 75, -30, 5 } },
            { 9, new int[] { 15, -55, 30, 135, 179, 135, 30, -55, 15 } }
        };

        /// <summary>
        /// Log a line to the passed log string builder
        /// </summary>
        /// <param name="logStringBuilder">The log string builder</param>
        /// <param name="value">The line to append to the log</param>
        public static void LogLine(this StringBuilder logStringBuilder, string value)
        {
            // Check for a null log file
            if (logStringBuilder != null)
            {
                // Append the line and new-line strings
                logStringBuilder.Append(value);
                logStringBuilder.Append("\r\n");
            }
        }

        /// <summary>
        /// Perform Savitsky-Golay smoothing on the values and return the result
        /// </summary>
        /// <param name="values">The array of values</param>
        /// <param name="width">The width of the fit</param>
        /// <param name="degree">The degree of the polynomial</param>
        /// <returns>The array of smoothed values</returns>
        public static double[] SavitskyGolay(
            this double[] values,
            int width,
            int degree,
            StringBuilder logStringBuilder)
        {
            // Log the call
            logStringBuilder.LogLine("SavitskyGolay(" + width + ", " + degree + ")");

            // Get the constants for the specified degree
            Dictionary<int, int[]> constantsByWidth = null;

            if ((degree == 2) || (degree == 3))
            {
                // Quadratic and cubic constants are the same
                constantsByWidth = savitskyGolayQuadraticConstantsByWidth;
            }
            else if ((degree == 4) || (degree == 5))
            {
                // Quartic and quintic constants are the same
                constantsByWidth = savitskyGolayQuarticConstantsByWidth;
            }

            // Check fot a valid parameter
            if (constantsByWidth == null)
            {
                logStringBuilder.LogLine("Invalid degree (" + degree + ")");
                return null;
            }

            // Get the constants for the specified width
            int[] constants = null;

            if (constantsByWidth.TryGetValue(width, out constants) == false)
            {
                logStringBuilder.LogLine("Invalid width (" + width + ")");
                return null;
            }

            // Calculate the norm of the constants
            int norm = constants.Sum();

            // Calculate the dimension of the data
            int dimension = values.Length;

            // Calculate the half width
            int halfWidth = width >> 1;

            // Create the result array
            var result = new double[dimension];

            // Loop through the values
            for (int index = 0; index < dimension; ++index)
            {
                // Check for the end values which cannot be calculated
                if ((index < halfWidth) || (index >= (dimension - halfWidth)))
                {
                    result[index] = double.NaN;
                }
                else
                {
                    // Initialise the accumulator
                    double accumulator = 0.0;

                    // Loop through the points
                    for (int point = -halfWidth; point <= halfWidth; ++point)
                    {
                        // Sum the product of the value and the constant
                        accumulator += values[index + point] * constants[point + halfWidth];
                    }
                    
                    // Set the result to the accumulated value divided by the norm
                    result[index] = accumulator / norm;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate the variance between the signal and the noisy data
        /// </summary>
        /// <param name="signal">The signal values</param>
        /// <param name="noise">The noisy values</param>
        /// <returns>The variance</returns>
        public static double Variance(this double[] signal, double[] noise)
        {
            // Calculate the dimension of the data
            int dimension = Math.Min(signal.Length, noise.Length);

            // Intialise the sum of squares and the number of values
            double sumOfSquares = double.NaN;
            int values = 0;

            // Loop through the dataset
            for (int index = 0; index < dimension; ++index)
            {
                // Check for two defined values
                if ((double.IsNaN(signal[index]) == false) &&
                    (double.IsNaN(noise[index]) == false))
                {
                    // Calculate the difference
                    double difference = noise[index] - signal[index];

                    // Update the numerator and denominator for the variance
                    sumOfSquares = double.IsNaN(sumOfSquares) ? (difference * difference) :
                        (sumOfSquares + (difference * difference));

                    // Increment the number of values
                    values++;
                }
            }

            // Return the variance
            return (values == 0) ? double.NaN : (sumOfSquares / values);
        }

        /// <summary>
        /// Fit a polynomial of defined degree to the set of values and return a new set of values
        /// for the polynomial. The values are assumed to be equidistant and vaules set to NaN are
        /// ingored. The algorithm uses linear algebra to calculate the coefficients for the curve
        /// based on a least squares fit.
        /// </summary>
        /// <param name="values">Array of input values</param>
        /// <param name="degree">The degree of polynomial required</param>
        /// <param name="logStringBuilder">The log string builder</param>
        /// <returns>Then polynomial values</returns>
        public static double[] CurveFit(this double[] values,
            int degree,
            StringBuilder logStringBuilder)
        {
            // Log the call
            logStringBuilder.LogLine("CurveFit(" + degree + ")");

            if (degree < 0)
            {
                logStringBuilder.LogLine("Invalid degree (" + degree + ")");
                return null;
            }

            // Calculate the dimension of the data
            int dimension = values.Length;

            // Initialise double the degree
            int doubleDegree = degree + degree;

            // Initialise an array to store powers of indexes
            var powersOfIndexes = new double[dimension];

            // Initialise an array of sums of powers of indexes
            var powersOfIndexesSum = new double[doubleDegree + 1];

            // Calculate the sum of the powers of the indexes for powers up to double the degree
            for (int power = 0; power <= doubleDegree; ++power)
            {
                // Loop through the values
                for (int index = 0; index < dimension; ++index)
                {
                    // Ignore any NaN values
                    if (double.IsNaN(values[index]))
                    {
                        continue;
                    }

                    if (power == 0)
                    {
                        // For the zeroth power add 1 and initialise the array
                        powersOfIndexesSum[power] += 1.0;
                        powersOfIndexes[index] = index;
                    }
                    else
                    {
                        // For all higher powers increment the sum
                        powersOfIndexesSum[power] += powersOfIndexes[index];

                        // If this is not the last power then multiply up the array value
                        if (power < doubleDegree)
                        {
                            powersOfIndexes[index] *= index;
                        }
                    }
                }
            }

            // Initialise an array to store powers of indexes by values
            var powersOfIndexesByValues = (double[])values.Clone();

            // Initialise an array of sums of powers of indexes by values
            var powerssOfIndexesByValueSum = new double[degree + 1];

            // Calculate the sum of the powers of the indexes by values for powers up to the degree
            for (int power = 0; power <= degree; ++power)
            {
                // Loop through the values
                for (int index = 0; index < dimension; ++index)
                {
                    // Ignore any NaN values
                    if (double.IsNaN(values[index]))
                    {
                        continue;
                    }

                    // Increment the sum for this power
                    powerssOfIndexesByValueSum[power] += powersOfIndexesByValues[index];

                    // If this is not the last power then multiply up the array value
                    if (power < degree)
                    {
                        powersOfIndexesByValues[index] *= index;
                    }
                }
            }

            // Initialise a matrix to store the linear equation coefficients
            var matrix = new double[degree + 1, degree + 1];

            // Populate the matrix symmetrically with the coefficients
            for (int row = 0; row <= degree; ++row)
            {
                // First populate the diagonal element
                matrix[row, row] = powersOfIndexesSum[row + row];

                // Then populate the off-diagonals symetrically
                for (int column = 0; column < row; ++column)
                {
                    matrix[row, column] = matrix[column, row] = powersOfIndexesSum[column + row];
                }
            }

            // Cholesky decompose the matrix
            var decomposedMatrix = matrix.CholeskyDecompose();

            // Check for a valid matrix
            if (decomposedMatrix == null)
            {
                logStringBuilder.LogLine("Invalid matrix for decomposition");
                return null;
            }

            // Solve the equations by substitution 
            var solution = decomposedMatrix.CholeskySolve(powerssOfIndexesByValueSum);

            // Check for a valid solution
            if (solution == null)
            {
                logStringBuilder.LogLine("Decomposed matrix could not be solved");
                return null;
            }

            return GeneratePolynomial(solution, dimension);
        }

        /// <summary>
        /// Fit a polynomial curve to the passed values successively eliminating peak values that
        /// are above the curve
        /// </summary>
        /// <param name="values">The values to wich the polynomial is fitted</param>
        /// <param name="analysisData">The analysis data</param>
        /// <param name="logStringBuilder">The log string builder</param>
        /// <param name="maxVariance">The maximum variance allowed</param>
        /// <param name="variance">The variance for the polynomial</param>
        /// <returns>The polynomial values</returns>
        public static double[] LowerCurveFit(this double[] values, 
            AnalysisData analysisData, 
            StringBuilder logStringBuilder,
            double maxVariance,
            out double variance)
        {
            // Copy the values and count the points
            double[] valuesMinusPeaks = (double[])values.Clone();
            int totalPoints = valuesMinusPeaks.Count(x => double.IsNaN(x) == false);

            // Initialise the result 
            double[] polynomialData = null;

            // Initialise variable to store the numbe of points and the previous value
            int points = totalPoints;
            int previousPoints;

            // Remember the number of iterations
            int iterations = 0;

            // Loop until no more points can be removed
            do
            {
                // Check the number of iterations
                if (++iterations > MAX_LOWER_CURVE_FIT_ITERATIONS)
                {
                    // The variance for no polynomial is undefined
                    variance = double.NaN;

                    // Return null polynomial
                    return null;
                }

                // Remember the number of points
                previousPoints = points;

                // Fit the curve to the polynomial data
                polynomialData = valuesMinusPeaks.CurveFit(
                    DefaultAnalysis.SMOOTHED_DATA_POLYNOMIAL_DEGREE, logStringBuilder);

                // Check for a failed polynomial fit
                if (polynomialData == null)
                {
                    // The variance for no polynomial is undefined
                    variance = double.NaN;

                    // Return null polynomial
                    return null;
                }

                // Copy the values again and reset the points
                valuesMinusPeaks = (double[])values.Clone();
                points = totalPoints;

                // Loop through the data looking for peak data that is above the curve
                for (int index = 0; index < analysisData.CurrentDifferences.Length; ++index)
                {
                    // Only eliminate values that are within the peaks and above the line
                    if ((double.IsNaN(valuesMinusPeaks[index]) == false) &&
                        analysisData.Peaks.ContainsPotential(analysisData.StartPotential +
                        (analysisData.IncrementalPotential * index)))
                    {
                        double difference = valuesMinusPeaks[index] - polynomialData[index];

                        if ((difference > 0.0) && ((difference * difference) > maxVariance))
                        {
                            valuesMinusPeaks[index] = double.NaN;
                            --points;
                        }
                    }
                }
            }
            while (previousPoints != points);

            // Calculate the variance for smoothed signal on the raw data
            variance = polynomialData.Variance(valuesMinusPeaks);

            // Return the result
            return polynomialData;
        }

        /// <summary>
        /// Generate a polynomial curve as and array of values from the coefficients
        /// </summary>
        /// <param name="coefficients">The array of coefficients by order</param>
        /// <param name="dimension">The required dimension of the values</param>
        /// <returns>The array of polynomial values</returns>
        public static double[] GeneratePolynomial(this double[] coefficients, int dimension)
        {
            // Calculate the degree
            int degree = coefficients.Length - 1;

            // Create the result array of values
            var result = new double[dimension];

            // Populate the values fully using the calculated polynomial coefficients
            for (int index = 0; index < dimension; ++index)
            {
                // Initialise the zeroth power
                double powerOfIndex = 1.0;

                // Loop through the powers adding the product with the coefficients
                for (int power = 0; power <= degree; ++power)
                {
                    result[index] += coefficients[power] * powerOfIndex;

                    // If this is not the last power then multiply up the value
                    if (power < degree)
                    {
                        powerOfIndex *= index;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Subtract one set of values from another
        /// </summary>
        /// <param name="first">The first set of values</param>
        /// <param name="second">The second set of values</param>
        /// <returns>The first set minus the second set of values</returns>
        public static double[] Minus(this double[] first, double[] second)
        {
            // Calculate the dimension of the data
            int dimension = Math.Min(first.Length, second.Length);

            // Initialise the result set
            var result = new double[dimension];

            // Loop through the indices
            for (int index = 0; index < dimension; ++index)
            {
                // Check for NaN
                if (double.IsNaN(first[index]) || double.IsNaN(second[index]))
                {
                    result[index] = double.NaN;
                }
                else
                {
                    result[index] = first[index] - second[index];
                }
            }

            return result;
        }

        /// <summary>
        /// Find the gaussian parameters for the defined peak on the passed data
        /// </summary>
        /// <param name="values">The dataset to which the peak will be fitted</param>
        /// <param name="startPotential">The value at the starting point</param>
        /// <param name="incrementalPotential">The incremental value for each point</param>
        /// <param name="peakData">The specific peak</param>
        /// <param name="variance">The variance for the resulting curve</param>
        /// <returns>The parameters for the fitted peak</returns>
        public static GaussianParameters FitGaussianForPeak(this double[] values,
            double startPotential,
            double incrementalPotential,
            PeakData peakData,
            StringBuilder logStringBuilder,
            out double variance)
        {
            // Log the call
            logStringBuilder.LogLine("FitGaussianForPeak(...)");

            // Calculate the dimension of the data
            int dimension = values.Length;

            // Initialise the isolated peak dataset
            var isolatedPeakValues = new double[dimension];

            // Get the start and end index for the peak data
            int minIndex = Math.Min(Math.Max((int)((peakData.MinPotential - startPotential) / 
                incrementalPotential), 0), dimension);
            int maxIndex = Math.Min(Math.Max((int)((peakData.MaxPotential - startPotential) /
                incrementalPotential) + 1, 0), dimension);

            // Loop through the indices extracting the subset for this peak
            for (int index = 0; index < minIndex; ++index)
            {
                isolatedPeakValues[index] = double.NaN;
            }

            for (int index = minIndex; index < maxIndex; ++index)
            {
                isolatedPeakValues[index] = values[index];
            }

            for (int index = maxIndex; index < dimension; ++index)
            {
                isolatedPeakValues[index] = double.NaN;
            }

            // Create the parameters object
            var parameters = new GaussianParameters();

            // Create an initial guess based on the data in the peak region
            parameters.Height = Math.Max(isolatedPeakValues.Max(), 0.0);
            parameters.Mean = peakData.Mean;
            parameters.StandardDeviation = (peakData.MaxPotential - peakData.MinPotential) / STANDARD_DEVIATION_MULTIPLIER_FOR_WIDTH;
            parameters.Skew = 0.0;

            // Initialise a counter for the number of values over the threshold
            int valueCount = 0;

            // Initialise the sum of the squares
            double sumOfSquares = 0.0;

            // Loop through the values setting low values to NaN
            for (int index = 0; index < dimension; ++index)
            {
                if (double.IsNaN(isolatedPeakValues[index]) == false)
                {
                    // Add this to the sum of the squares
                    sumOfSquares += isolatedPeakValues[index] * isolatedPeakValues[index];

                    // Increment the value count
                    valueCount++;
                }
            }

            // Calculate the variance from the axis
            variance = sumOfSquares / valueCount;

            // Check for no measurable peak
            if (variance < peakData.MaxVarianceForCurveFit)
            {
                // Set the height to zero
                parameters.Height = 0.0;

                // Return parameters for a zero height peak
                return parameters;
            }

            // Reset the value count
            valueCount = 0;

            // Calulate the low value threshold
            double lowValueThreshold = isolatedPeakValues.Max() * (1.0 - peakData.TopFractionForCurveFit);

            // Loop through the values setting low values to NaN
            for (int index = 0; index < dimension; ++index)
            {
                if (double.IsNaN(isolatedPeakValues[index]) == false)
                {
                    if (isolatedPeakValues[index] >= lowValueThreshold)
                    {
                        // Increment the value count
                        ++valueCount;
                    }
                    else
                    {
                        isolatedPeakValues[index] = double.NaN;
                    }
                }
            }

            // Check for insufficient points to fit
            if (valueCount < 2)
            {
                logStringBuilder.LogLine("There are insufficient points to fit");

                // The variance for no points is undefined
                variance = double.NaN;

                // Return null parameters
                return null;
            }

            // Initialise the set of Gaussian values
            var gaussianValues = new double[dimension];

            // Create a skew Gaussian curve and measure the variance
            variance = parameters.SkewGaussianCurve(gaussianValues, startPotential, 
                incrementalPotential).Variance(isolatedPeakValues);

            // Initialise the error parameters
            var parameterErrorForIteration = new GaussianParameters()
            {
                StandardDeviation = parameters.StandardDeviation / INITIAL_STANDARD_DEVIATION_DIVISOR_FOR_ERROR,
                Mean = parameters.StandardDeviation / INITIAL_MEAN_DIVISOR_FOR_ERROR,
                Height = parameters.Height / INITIAL_HEIGHT_DIVISOR_FOR_ERROR,
                Skew = INITIAL_SKEW_ERROR
            };

            // Variable to remember the variance for the previous iteration
            double previousVariance;

            // Count of consecutive iterations that are close to the previous value
            int consecutiveCloseIterations = 0;

            // Loop through the pass iterations, each pass optimised all variables
            for (int passIteration = 0; passIteration < MAX_PASS_ITERATIONS; ++passIteration)
            {
                // Remember the variance
                previousVariance = variance;

                // Loop through the variables
                foreach (GaussianParameterType errorType in Enum.GetValues(typeof(GaussianParameterType)))
                {
                    // Initialise the error variable
                    double error;

                    if (errorType == GaussianParameterType.StandardDeviation)
                    {
                        error = parameterErrorForIteration.StandardDeviation;
                    }
                    else if (errorType == GaussianParameterType.Mean)
                    {
                        error = parameterErrorForIteration.Mean;
                    }
                    else if (errorType == GaussianParameterType.Height)
                    {
                        error = parameterErrorForIteration.Height;
                    }
                    else
                    {
                        error = parameterErrorForIteration.Skew;
                    }

                    // Loop through the value iterations for this variable
                    for (int valueIteration = 0; valueIteration < VALUE_ITERATIONS; 
                        ++valueIteration)
                    {
                        // Calculate the forward and reverse parameters for this variable and error
                        var forwardParameters = parameters.AddError(errorType, error);
                        var reverseParameters = parameters.AddError(errorType, -error);

                        // Calculate the variance for the forward and reverse parameters
                        double forwardVariance = forwardParameters.SkewGaussianCurve(gaussianValues,
                            startPotential, incrementalPotential).
                            Variance(isolatedPeakValues);
                        double reverseVariance = reverseParameters.SkewGaussianCurve(gaussianValues,
                            startPotential, incrementalPotential).
                            Variance(isolatedPeakValues);

                        // Check for an improved forward position
                        if ((forwardVariance < reverseVariance) && (forwardVariance < variance))
                        {
                            // Update the current position
                            variance = forwardVariance;
                            parameters = forwardParameters;
                        }
                        // Check for an improved reverse position
                        else if (reverseVariance < variance)
                        {
                            // Update the current position
                            variance = reverseVariance;
                            parameters = reverseParameters;
                        }

                        // Check for the theroetical maximum variance
                        if (variance == 0.0)
                        {
                            break;
                        }

                        // Halve the error for the next iteration
                        error /= 2.0;
                    }
                }

                // Check for the theroetical maximum variance
                if (variance == 0.0)
                {
                    // Log the result
                    logStringBuilder.LogLine("Algorithm converged after " + (passIteration + 1) +
                        " iteration" + ((passIteration > 0) ? "s" : "") + " with variance of " + variance);

                    // Return the converged parameters
                    return parameters;
                }

                // Increment the consecutive close iterations if the variance has converged
                consecutiveCloseIterations = (Math.Abs(previousVariance - variance) < 
                    MAX_ERROR_FOR_CONVERGED_VARIANCE) ? consecutiveCloseIterations + 1 : 0;

                // Check for two consecutive converged iterations 
                if (consecutiveCloseIterations == 2)
                {
                    // Log the result
                    logStringBuilder.LogLine("Algorithm converged after " + (passIteration + 1) +
                        " iteration" + ((passIteration > 0) ? "s" : "") + " with variance of " + variance);

                    // Return the converged parameters
                    return parameters;
                }

                // Log the iteration value
                logStringBuilder.LogLine("Variance is " + variance + " after iteration " + (passIteration + 1));
            }

            // The convergence has failed so log this anr return null parameters
            logStringBuilder.LogLine("Algorithm failed to converge");

            // The variance for no points is undefined
            variance = double.NaN;

            // Return null parameters
            return null;
        }

        /// <summary>
        /// Calcualte the peak position for the skew gaussian curve
        /// </summary>
        /// <param name="parameters">The Gaussian curve parameters</param>
        /// <returns>The curve values</returns>
        public static double SkewGaussianPeakPosition(this GaussianParameters parameters)
        {
            // Initialise the peak position at the mean
            double position = parameters.Mean + GetMeanShiftForSkew(parameters.StandardDeviation, parameters.Skew);

            // Calculate the initial value
            double value = SkewGaussianValue(position, parameters.StandardDeviation, parameters.Mean, 
                parameters.Skew);

            // Set the initial error to the standard deviation
            double error = parameters.StandardDeviation;

            // Loop until the error is within tolerance
            do
            {
                // Calculate forward and reverse values
                double forwardValue = SkewGaussianValue(position + error, parameters.StandardDeviation,
                    parameters.Mean, parameters.Skew);
                double reverseValue = SkewGaussianValue(position - error, parameters.StandardDeviation,
                    parameters.Mean, parameters.Skew);

                // Check for a better forward value
                if ((forwardValue > value) && (forwardValue > reverseValue))
                {
                    position += error;
                    value = forwardValue;
                }
                // Check for a better reverse value
                else if (reverseValue > value)
                {
                    position -= error;
                    value = reverseValue;
                }

                // halve the error
                error /= 2.0;
            }
            while (error > MAX_ERROR_FOR_CONVERGED_PEAK_POSITION);

            // Return the result
            return position;
        }
        
        /// <summary>
        /// Generate a dataset for a skew Gaussian curve over the defined range
        /// </summary>
        /// <param name="parameters">The Gaussian curve parameters</param>
        /// <param name="values">The curve values to set</param>
        /// <param name="startPotential">The value at the starting point</param>
        /// <param name="incrementalPotential">The incremental value for each point</param>
        /// <returns>The curve values</returns>
        public static double[] SkewGaussianCurve(this GaussianParameters parameters,
            double[] values,
            double startPotential,
            double incrementalPotential)
        {
            // Calculate the dimension of the data
            var dimension = values.Length;

            // Loop through the values setting them
            for (int index = 0; index < dimension; ++index)
            {
                values[index] = SkewGaussianValue(
                    startPotential + (incrementalPotential * index),
                    parameters.StandardDeviation, parameters.Mean, parameters.Skew);
            }

            // Calculate the height scaling factor
            var heightFactor = parameters.Height / values.Max();

            // Scale the values accordingly
            for (int index = 0; index < dimension; ++index)
            {
                values[index] *= heightFactor;
            }

            return values;
        }

        /// <summary>
        /// Calculate the outcome for a peak using the best fit skew Gaussian curve parameters
        /// </summary>
        /// <param name="peak">The peak data</param>
        /// <param name="parameters">The skew Gaussian parameters</param>
        /// <returns>The outcome for the peak</returns>
        public static PeakOutcome OutcomeForPeak(this PeakData peak,
            GaussianParameters parameters,
            double varianceForPeak,
            StringBuilder logStringBuilder)
        {
            // If this is a peak to ignore then return a null result
            if (peak.Type == PeakType.Ignore)
            {
                logStringBuilder.LogLine("Peak " + peak.Name + " is ignored");

                return PeakOutcome.Ignore;
            }

            // Check for an undefined variance
            if (double.IsNaN(varianceForPeak))
            {
                logStringBuilder.LogLine("Variance for peak " + peak.Name + " is undefined");

                return (peak.Type == PeakType.Rescan) ? PeakOutcome.Ignore : PeakOutcome.Invalid;
            }

            // Check this value against the minimum allowable value
            if (varianceForPeak > peak.MaxVarianceForCurveFit)
            {
                logStringBuilder.LogLine("Variance for peak " + peak.Name + " of " +
                    varianceForPeak + " is more than the maximum of " + peak.MaxVarianceForCurveFit);

                return (peak.Type == PeakType.Rescan) ? PeakOutcome.Ignore : PeakOutcome.Invalid;
            }

            // Log the variance value for the peak
            logStringBuilder.LogLine("Variance for peak " + peak.Name + " is " + varianceForPeak);

            // Check for the existence of parameters
            if (parameters != null)
            {
                // Calculate the actual peak position
                double peakPosition = parameters.Position;

                // Check the position of the peak is within tolerance
                if (peakPosition < (peak.Mean - peak.Tolerance))
                {
                    logStringBuilder.LogLine("Peak position for peak " + peak.Name + " of " +
                        peakPosition + " is lower than the minimum of " +
                        (peak.Mean - peak.Tolerance));

                    return (peak.Type == PeakType.Rescan) ? PeakOutcome.Ignore : PeakOutcome.Invalid;
                }
                else if (peakPosition > (peak.Mean + peak.Tolerance))
                {
                    logStringBuilder.LogLine("Peak position for peak " + peak.Name + " of " +
                        peakPosition + " is higher than the maximum of " +
                        (peak.Mean + peak.Tolerance));

                    return (peak.Type == PeakType.Rescan) ? PeakOutcome.Ignore : PeakOutcome.Invalid;
                }

                // If this is a positive peak then check the height thresholds
                if (peak.Type == PeakType.Positive)
                {
                    if (parameters.Height < peak.LowerLimit)
                    {
                        logStringBuilder.LogLine("Peak " + peak.Name + " is negative (" +
                            parameters.Height + ")");
                        return PeakOutcome.Negative;
                    }
                    else if (parameters.Height < peak.UpperLimit)
                    {
                        logStringBuilder.LogLine("Peak " + peak.Name + " is positive (" + 
                            parameters.Height + ")");
                        return PeakOutcome.Positive;
                    }
                    else
                    {
                        logStringBuilder.LogLine("Peak " + peak.Name + " is invalid (" +
                            parameters.Height + ")");
                        return PeakOutcome.Invalid;
                    }
                }
                // If this is a rescan peak then check the upper height threshold
                else if (peak.Type == PeakType.Rescan)
                {
                    if (parameters.Height > peak.LowerLimit)
                    {
                        logStringBuilder.LogLine("Peak " + peak.Name + " is rescan (" +
                            parameters.Height + ")");
                        return PeakOutcome.Rescan;
                    }

                    return PeakOutcome.Ignore;
                }
            }

            // All other results are undefined
            logStringBuilder.LogLine("Peak " + peak.Name + " is undefined");

            return (peak.Type == PeakType.Rescan) ? PeakOutcome.Ignore : PeakOutcome.Invalid;
        }

        /// <summary>
        /// Calculate the shift in the mean caused by a finite skew of the skew Gaussian curve
        /// </summary>
        /// <param name="standardDeviation">The standard deviation</param>
        /// <param name="skew">The skew</param>
        /// <returns>The shift in the mean</returns>
        private static double GetMeanShiftForSkew(double standardDeviation,
            double skew)
        {
            return standardDeviation * skew * ROOT_TWO_OVER_PI / Math.Sqrt(1.0 + (skew * skew));
        }

        /// <summary>
        /// Check to see if the passed potential is in range of any peak
        /// </summary>
        /// <param name="peaks">The array of peaks</param>
        /// <param name="potential">The potential to check for</param>
        /// <returns>True if in range, false otherwise</returns>
        private static bool ContainsPotential(this PeakData[] peaks, double potential)
        {
            // Loop through the peaks checking each in turn
            foreach (var peak in peaks)
            {
                if (peak.ContainsPotential(potential))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check to see if the passed potential is in range of a peak
        /// </summary>
        /// <param name="peak">The peak data</param>
        /// <param name="potential">The potential to check for</param>
        /// <returns>True if in range, false otherwise</returns>
        private static bool ContainsPotential(this PeakData peak, double potential)
        {
            // Check to see if the potential is outside of the peak width
            if ((potential >= peak.MinPotential) && (potential <= peak.MaxPotential))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add an error of the specified type to the skew Gaussian parameters
        /// </summary>
        /// <param name="parameters">The parameters to modify</param>
        /// <param name="errorType">The type of error</param>
        /// <param name="error">The error value</param>
        /// <returns>The modified parameters</returns>
        private static GaussianParameters AddError(this GaussianParameters parameters,
            GaussianParameterType errorType, double error)
        {
            // If this is a skew error then we must also shift the mean to compensate for the
            // effect that the skew has on the shape of the curve
            double meanShift = (errorType != GaussianParameterType.Skew) ? 0 :
                (GetMeanShiftForSkew(parameters.StandardDeviation, parameters.Skew) -
                GetMeanShiftForSkew(parameters.StandardDeviation, parameters.Skew + error));

            // Create and return the new parameters with the relvant error applied
            return new GaussianParameters()
            {
                StandardDeviation = parameters.StandardDeviation +
                    ((errorType == GaussianParameterType.StandardDeviation) ? error : 0.0),
                Mean = parameters.Mean + meanShift +
                    ((errorType == GaussianParameterType.Mean) ? error : 0.0),
                Height = parameters.Height +
                    ((errorType == GaussianParameterType.Height) ? error : 0.0),
                Skew = parameters.Skew +
                    ((errorType == GaussianParameterType.Skew) ? error : 0.0),
            };
        }

        /// <summary>
        /// Solve the passed cholesky decomposed matrix (lower) for the passed values
        /// </summary>
        /// <param name="matrix">The lower cholesky decomposition</param>
        /// <param name="values">The values to solve for</param>
        /// <returns>The solution values</returns>
        private static double[] CholeskySolve(this double[,] matrix, double[] values)
        {
            // Calculate the dimension of the matrix
            int dimension = matrix.GetLength(0);

            // Ensure that the matrix is square
            if (dimension != matrix.GetLength(1))
            {
                return null;
            }

            // Create an array of solution values from the values to solve for
            var result = (double[])values.Clone();

            // Solve the lower half of the decomposition using back substisution
            for (int row = 0; row < dimension; ++row)
            {
                for (int column = 0; column < row; ++column)
                {
                    result[row] -= result[column] * matrix[row, column];
                }

                result[row] /= matrix[row, row];

                // Check for a division by zero
                if (double.IsInfinity(result[row]))
                {
                    return null;
                }
            }

            // Solve the upper half of the decomposition using back substisution
            for (int row = dimension - 1; row >= 0; --row)
            {
                for (int column = row + 1; column < dimension; ++column)
                {
                    result[row] -= result[column] * matrix[column, row];
                }

                result[row] /= matrix[row, row];

                // Check for a division by zero
                if (double.IsInfinity(result[row]))
                {
                    return null;
                }
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Cholesky decompose the passed matrix
        /// </summary>
        /// <param name="matrixIn">The matrix to decompose</param>
        /// <returns>The lower half of the Cholesky decomposition</returns>
        private static double[,] CholeskyDecompose(this double[,] matrixIn)
        {
            // Calculate the dimension of the matrix
            int dimension = matrixIn.GetLength(0);

            // Ensure that the matrix is square
            if (dimension != matrixIn.GetLength(1))
            {
                return null;
            }

            // Create the lower decomposed matrix
            var matrixOut = new double[dimension, dimension];

            // Loop through the rows
            for (int row = 0; row < dimension; ++row)
            {
                // Intialise the pivot
                double pivot = 0.0;

                // Loop through the columns
                for (int column = 0; column < row; ++column)
                {
                    // Initialise the sum
                    double sum = 0.0;

                    // Sum the products
                    for (int subRow = 0; subRow < column; ++subRow)
                    {
                        sum += matrixOut[column, subRow] * matrixOut[row, subRow];
                    }

                    // Set the off-diagonal values for this row and update the pivot
                    matrixOut[row, column] = sum = (matrixIn[row, column] - sum) / matrixOut[column, column];

                    // Check for a division by zero
                    if (double.IsInfinity(matrixOut[row, column]))
                    {
                        return null;
                    }

                    pivot = pivot + (sum * sum);
                }

                // Set the pivot and the diagonal value for this row
                pivot = matrixIn[row, row] - pivot;

                // Check for a negative pivot
                if (pivot < 0.0)
                {
                    return null;
                }

                matrixOut[row, row] = Math.Sqrt(pivot);
            }

            // Return the lower decomposed matrix
            return matrixOut;
        }

        /// <summary>
        /// Calculate the skew Gaussian value for the passed parameters
        /// </summary>
        /// <param name="position">The position on the curve</param>
        /// <param name="standardDeviation">The standard deviation</param>
        /// <param name="mean">The unskewed mean</param>
        /// <param name="skew">The skew factor</param>
        /// <returns>The skew Gaussian value</returns>
        private static double SkewGaussianValue(double position,
            double standardDeviation,
            double mean,
            double skew)
        {
            // Calculate the relative position
            double relativePosition = (position - mean) / standardDeviation;

            // The skew Gaussian value is calculated using the Gaussian value and the error function
            return GaussianValue(relativePosition) * (1.0 + ErfValue((relativePosition * skew) / ROOT_TWO)) / standardDeviation;
        }

        /// <summary>
        /// Calculate the Gaussian value for the passed parameters
        /// </summary>
        /// <param name="position">The position on the curve</param>
        /// <returns>The Gaussian value</returns>
        private static double GaussianValue(double position)
        {
            return Math.Exp((position * position) / (-2.0)) / ROOT_TWO_PI;
        }

        /// <summary>
        /// Calculate the error function value using a polynomial estimate
        /// </summary>
        /// <param name="position">The position on the curve</param>
        /// <returns>The error function value</returns>
        private static double ErfValue(double position)
        {
            // Record the sign of the value and the absolute position as the error function is odd
            // and the estimate is only valid for positive positions
            int sign = Math.Sign(position);
            double absolutePosition = Math.Abs(position);

            // Calsulate the polynomial value using the inverse of the absolute position
            double inversePosition = 1.0 / (1.0 + (ERF_FACTOR * absolutePosition));        
            double polynomialValue = ((((((((ERF_COEFFICIENT_5 * inversePosition) + 
                ERF_COEFFICIENT_4) * inversePosition) + 
                ERF_COEFFICIENT_3) * inversePosition) + 
                ERF_COEFFICIENT_2) * inversePosition) + 
                ERF_COEFFICIENT_1) * inversePosition;
            
            // Return the estimated value and apply the sign
            return (1.0 - (polynomialValue * Math.Exp(-absolutePosition * absolutePosition))) * sign;    
        }    
    }
}
