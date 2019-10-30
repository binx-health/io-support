/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace IO.Model.Volatile
{
    /// <summary>
    /// Device values 
    /// </summary>
    public abstract class IDeviceValues : Dictionary<string, int>, IXmlSerializable
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IDeviceValues Instance { get; set; }

        /// <summary>
        /// Update new values into this object
        /// </summary>
        /// <param name="newValues">The new values to merge</param>
        public abstract void Update(IDeviceValues newValues);

        /// <summary>
        /// Get schema always returns null
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public abstract void ReadXml(XmlReader reader);

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public abstract void WriteXml(XmlWriter writer);

        /// <summary>
        /// Clone this object
        /// </summary>
        /// <param name="copyDeviceValues">Flag to clone the device values also</param>
        /// <returns>The cloned object</returns>
        public abstract object Clone(bool copyDeviceValues = true);
    }
}
