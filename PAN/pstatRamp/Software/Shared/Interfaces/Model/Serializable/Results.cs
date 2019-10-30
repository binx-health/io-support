/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Test results interface
    /// </summary>
    public abstract class IResults
    {
        /// <summary>
        /// Instance variable accessor
        /// </summary>
        public static IResults Instance { get; set; }

        /// <summary>
        /// The start date filter
        /// </summary>
        public abstract DateTime? StartDateTime { get; set; }

        /// <summary>
        /// The locking user filter
        /// </summary>
        public abstract uint LockingUserId { get; set; }

        /// <summary>
        /// The sample ID filter
        /// </summary>
        public abstract string SampleId { get; set; }

        /// <summary>
        /// The assay name filter
        /// </summary>
        public abstract string AssayName { get; set; }

        /// <summary>
        /// Indexer for tests
        /// </summary>
        /// <param name="sampleId">The sample ID</param>
        /// <returns>The test</returns>
        public abstract ITest this[string sampleId] { get; }

        /// <summary>
        /// Write the result text to string
        /// </summary>
        /// <param name="test">The test</param>
        /// <returns>The result text as a string</returns>
        public abstract string ResultText(ITest test);

        /// <summary>
        /// Add a result to the file
        /// </summary>
        /// <param name="test">The test</param>
        public abstract void AddResult(ITest test);

        /// <summary>
        /// Read the next result from the file
        /// </summary>
        /// <returns>The test object</returns>
        public abstract ITest NextResult();

        /// <summary>
        /// Reset to the start of the file
        /// </summary>
        public abstract void Reset();
    }
}
