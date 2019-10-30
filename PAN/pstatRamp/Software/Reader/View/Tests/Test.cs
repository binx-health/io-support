/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using IO.Model.Serializable;

namespace IO.View.Tests
{
    /// <summary>
    /// Test object for the test
    /// </summary>
    public class Test : ITest
    {
        /// <summary>
        /// A test is read-only if it is not the current test
        /// </summary>
        [XmlIgnore]
        public override bool ReadOnly
        {
            get
            {
                lock (ITests.Instance.CurrentTest)
                {
                    return ITests.Instance.CurrentTest != this;
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Test()
        {
            // Initialise the values to defaults
            Clear();
        }

        /// <summary>
        /// Clear the test data
        /// </summary>
        public override void Clear()
        {
            // Initialise the values to defaults
            LockingUserId = 0;
            PercentComplete = 0;
            Result = new TestResult();
            Data = null;
        }
    }
}
