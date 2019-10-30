/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.Scripting
{
    /// <summary>
    /// Metrics interface
    /// </summary>
    public abstract class IMetrics
    {
        /// <summary>
        /// Parent object
        /// </summary>
        public abstract IMetrics Parent { get; }

        /// <summary>
        /// Modified flag
        /// </summary>
        public virtual bool Modified { get; set; }

        /// <summary>
        /// Loaded flag
        /// </summary>
        public virtual bool Loaded { get; set; }

        /// <summary>
        /// The name for the metrics
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The serialised metric data
        /// </summary>
        public abstract string Value { get; set; }

        /// <summary>
        /// Enumeration of metric names
        /// </summary>
        public abstract IEnumerable<string> Names { get; }

        /// <summary>
        /// String metric getter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="inherited">Flag to indicate the value is inherited</param>
        /// <returns>The metric value</returns>
        public abstract string GetMetric(string name, out bool inherited);

        /// <summary>
        /// String metric setter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        public abstract void SetMetric(string name, string value);

        /// <summary>
        /// Integer metric getter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        /// <param name="inherited">Flag to indicate the value is inherited</param>
        /// <returns>True if the metric is an integer</returns>
        public abstract bool TryGetIntegerMetric(string name, out int value, out bool inherited);

        /// <summary>
        /// Integer metric setter
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        public abstract void SetIntegerMetric(string name, int value);
    }
}
