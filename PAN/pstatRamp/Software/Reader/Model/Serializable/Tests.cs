/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IO.Model.Serializable
{
    /// <summary>
    /// List of test objects
    /// </summary>
    [XmlRoot("Tests")]
    public class Tests : ITests
    {
        /// <summary>
        /// The maximum number of tests stored
        /// </summary>
        private static readonly int MAXIMUM_STORED_TESTS = 20;

        /// <summary>
        /// The types for serialisation
        /// </summary>
        public override Type[] Types
        {
            get
            {
                return new Type[] { typeof(Test), typeof(TestResult), typeof(TestData) };
            }
        }

        /// <summary>
        /// Create a new test
        /// </summary>
        /// <returns>The new test</returns>
        public override ITest CreateNewTest()
        {
            // If there is a current test then add it to the list
            if (CurrentTest != null)
            {
                // Remove any old tests
                while (Count >= MAXIMUM_STORED_TESTS)
                {
                    RemoveAt(0);
                }

                // Add the current test
                Add(CurrentTest);
            }

            // Create a new test and return it
            CurrentTest = new Test();
            return CurrentTest;
        }
    }
}
