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
    /// Tests interface
    /// </summary>
    [XmlRoot("Tests")]
    public abstract class ITests : List<ITest>
    {
        /// <summary>
        /// Instance variable accessor
        /// </summary>
        public static ITests Instance { get; set; }

        /// <summary>
        /// The types for serialisation
        /// </summary>
        public abstract Type[] Types { get; }

        /// <summary>
        /// The current test
        /// </summary>
        public virtual ITest CurrentTest { get; set; }

        /// <summary>
        /// Create a new test
        /// </summary>
        /// <returns>The new test</returns>
        public abstract ITest CreateNewTest();
    }
}
