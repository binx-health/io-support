/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace IO.Scripting
{
    /// <summary>
    /// Metrics object
    /// </summary>
    public class Metrics : IMetrics
    {
        /// <summary>
        /// Internal collection of non-default metrics
        /// </summary>
        static readonly internal Dictionary<string, Metrics> NonDefault = new Dictionary<string, Metrics>();

        /// <summary>
        /// The parent object
        /// </summary>
        private IMetrics parent = null;

        /// <summary>
        /// The name for the script
        /// </summary>
        private string name = null;

        /// <summary>
        /// Error description
        /// </summary>
        private string error = null;

        /// <summary>
        /// Sorted dictionary of metric values
        /// </summary>
        private SortedDictionary<string, object> metrics;

        /// <summary>
        /// Parent object
        /// </summary>
        public override IMetrics Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// The metric name
        /// </summary>
        public override string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// The string representation of the metrics
        /// </summary>
        public override string Value
        {
            get
            {
                // Check for null metrics
                if (metrics == null)
                {
                    return null;
                }

                // Initialise a string writer
                var stringWriter = new StringWriter();

                // Loop through the metrics writing them
                foreach (var metric in metrics)
                {
                    stringWriter.Write(metric.Key);
                    stringWriter.Write(" ");

                    if (metric.Value is string)
                    {
                        stringWriter.Write("\"" + metric.Value + "\"");
                    }
                    else
                    {
                        stringWriter.Write(metric.Value);
                    }

                    stringWriter.Write("\r\n");
                }

                // Return the string value
                return stringWriter.ToString();
            }
            set
            {
                // Clear any error
                error = null;

                // Clear any metrics
                metrics = new SortedDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                // Check for a null value
                if (value != null)
                {
                    // Loop through the lines
                    foreach (var line in value.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                    {
                        // Tokenise the string
                        var tokens = line.Tokenise(out error);

                        // Check for the required number of tokens
                        if (tokens.Count != 2)
                        {
                            error = line;
                            return;
                        }

                        // Check for a repeated value
                        if (metrics.ContainsKey(tokens[0]))
                        {
                            error = line;
                            return;
                        }

                        // Check for a delimited string value
                        var stringValue = tokens[1].StringValue(true);
                        int integerValue;
                        double doubleValue;

                        if (stringValue != null)
                        {
                            metrics[tokens[0]] = stringValue;
                        }
                        // Check for an integer value
                        else if (tokens[1].IsInteger(out integerValue))
                        {
                            metrics[tokens[0]] = integerValue;
                        }
                        // Check for a double value
                        else if (tokens[1].IsDouble(out doubleValue))
                        {
                            metrics[tokens[0]] = doubleValue;
                        }
                        else
                        {
                            // This is an invalid value
                            error = line;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Enumeration of metric names
        /// </summary>
        public override IEnumerable<string> Names
        {
            get
            {
                if (parent != null)
                {
                    return parent.Names;
                }

                return metrics.Keys;
            }
        }

        /// <summary>
        /// Error text
        /// </summary>
        public string Error
        {
            get
            {
                return error;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="metricsName">The name</param>
        /// <param name="metricsParent">The parent object</param>
        public Metrics(string metricsName, IMetrics metricsParent)
        {
            name = metricsName;
            parent = metricsParent;
            metrics = new SortedDictionary<string, object>();
        }

        /// <summary>
        /// String metric getter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="inherited">Flag to indicate the value is inherited</param>
        /// <returns>The metric value</returns>
        public override string GetMetric(string name, out bool inherited)
        {
            // Initialise the result
            string result;

            // Try to get the named metric as a string
            object value;

            if (metrics.TryGetValue(name, out value) && (value is string))
            {
                result = (string)value;
                inherited = false;
            }
            // Try to get the parent value
            else
            {
                result =  (parent != null) ? parent.GetMetric(name, out inherited) : null;
                inherited = result != null;
            }

            return result;
        }

        /// <summary>
        /// String metric setter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        public override void SetMetric(string name, string value)
        {
            // Check for a null value or the same value as the parent
            bool inherited;

            if ((parent != null) && (string.IsNullOrEmpty(value) || (parent.GetMetric(name, out inherited) == value)))
            {
                // Remove the metric
                metrics.Remove(name);
            }
            else
            {
                // Set the value
                metrics[name] = value;
            }
        }

        /// <summary>
        /// Integer metric getter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        /// <param name="inherited">Flag to indicate the value is inherited</param>
        /// <returns>True if the metric is an integer</returns>
        public override bool TryGetIntegerMetric(string name, out int value, out bool inherited)
        {
            // Intitialise the result
            bool result = false;

            // Try to get the named metric as an int
            object intValue;

            if (metrics.TryGetValue(name, out intValue) && (intValue is int))
            {
                value = (int)intValue;
                inherited = false;
                result = true;
            }
            else if (parent != null)
            {
                result = parent.TryGetIntegerMetric(name, out value, out inherited);
                inherited = result;
            }
            else
            {
                value = 0;
                inherited = false;
            }

            return result;
        }

        /// <summary>
        /// Integer metric setter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        public override void SetIntegerMetric(string name, int value)
        {
            // Check for a null value or the same value as the parent
            bool inherited;
            int parentValue;

            if ((parent != null) && parent.TryGetIntegerMetric(name, out parentValue, out inherited) && 
                (parentValue == value))
            {
                // Remove the metric
                metrics.Remove(name);
            }
            else
            {
                // Set the value
                metrics[name] = value;
            }
        }
    }
}
