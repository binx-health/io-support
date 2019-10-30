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
    /// Field interface
    /// </summary>
    public abstract class IField
    {
        /// <summary>
        /// The field name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The field type
        /// </summary>
        public virtual FieldType FieldType { get; set; }

        /// <summary>
        /// The field policy
        /// </summary>
        public virtual FieldPolicy Policy { get; set; }
    }
}
