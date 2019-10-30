/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.View.Tests
{
    /// <summary>
    /// Tests object for the test
    /// </summary>
    [XmlRoot("Tests")]
    public class Tests : ITests
    {
        /// <summary>
        /// The types for serialisation
        /// </summary>
        public override Type[] Types
        {
            get
            {
                return new Type[] { typeof(Test), typeof(TestResult) };
            }
        }

        /// <summary>
        /// The current test
        /// </summary>
        public override ITest CurrentTest { get; set; }

        /// <summary>
        /// Create a new test
        /// </summary>
        /// <returns>The new test</returns>
        public override ITest CreateNewTest()
        {
            // If there is a current test then add it to the list
            if (CurrentTest != null)
            {
                Add(CurrentTest);
            }

            // Create a new test and return it
            CurrentTest = new Test();
            return CurrentTest;
        }
    }
}
