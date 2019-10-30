/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace IO.Model.Volatile
{
    /// <summary>
    /// Device values 
    /// </summary>
    public class DeviceValues : IDeviceValues
    {
        /// <summary>
        /// Update new values into this object
        /// </summary>
        /// <param name="newValues">The new values to merge</param>
        public override void Update(IDeviceValues newValues)
        {
            // Loop through the values updating them
            foreach (var value in newValues)
            {
                this[value.Key] = value.Value;
            }
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public override void ReadXml(XmlReader reader)
        {
            // Clear the values
            Clear();

            // Read the XML to the end
            while (true)
            {
                // Check for an element
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Depth == 1))
                {
                    // Add the value to the dictionary
                    Add(reader.Name, reader.ReadElementContentAsInt());
                }
                else if (reader.Read() == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public override void WriteXml(XmlWriter writer)
        {
            // Loop through the values writing the elements
            foreach (var deviceValue in this)
            {
                writer.WriteElementString(deviceValue.Key, deviceValue.Value.ToString());
            }
        }

        /// <summary>
        /// Clone this object
        /// </summary>
        /// <param name="copyDeviceValues">Flag to clone the device values also</param>
        /// <returns>The cloned object</returns>
        public override object Clone(bool copyDeviceValues = true)
        {
            // Create a clone
            var clone = new DeviceValues();

            if (copyDeviceValues)
            {
                // Copy the values
                foreach (var value in this)
                {
                    clone.Add(value.Key, value.Value);
                }
            }

            return clone;
        }
    }
}
