/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.Model.Serializable;

namespace IO.View.Tests
{
    /// <summary>
    /// Field object for the test
    /// </summary>
    public class Field : IField
    {
        /// <summary>
        /// The field name
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// The field type
        /// </summary>
        public override FieldType FieldType { get; set; }

        /// <summary>
        /// The field policy
        /// </summary>
        public override FieldPolicy Policy { get; set; }
    }
}
