/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Test interface
    /// </summary>
    public abstract class ITest
    {
        /// <summary>
        /// A test is read-only if it is not the current test
        /// </summary>
        [XmlIgnore]
        public abstract bool ReadOnly { get; }

        /// <summary>
        /// The locking user
        /// </summary>
        [XmlIgnore]
        public virtual uint LockingUserId { get; set; }

        /// <summary>
        /// The percentage complete
        /// </summary>
        public virtual int PercentComplete { get; set; }

        /// <summary>
        /// The test result
        /// </summary>
        public virtual ITestResult Result { get; set; }

        /// <summary>
        /// The test data
        /// </summary>
        public virtual ITestData Data { get; set; }

        /// <summary>
        /// Clear the test data
        /// </summary>
        public abstract void Clear();
    }
}
