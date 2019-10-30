/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IO.Controller
{
    /// <summary>
    /// Static auxilliary extension methods
    /// </summary>
    public static class Auxilliary
    {
        /// <summary>
        /// Serialise an object into XML
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The XML as a string</returns>
        public static string Serialise<T>(this T obj, Type[] types = null)
        {
            // Create a string builder
            var stringBuilder = new StringBuilder();

            // Create the serialiser from the type
            var xmlSerializer = new XmlSerializer(obj.GetType(), (types != null) ? types : new Type[0]);

            // Create an XML writer to serialise the object as a fragment
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
            {
                Indent = true,
            }))
            {
                // Serialise the object
                xmlSerializer.Serialize(xmlWriter, obj);
            }

            // Return the XML fragment as a string
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Serialise an object into an XML fragment
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The XML fragment as a string</returns>
        public static string SerialiseFragment<T>(this T obj, Type[] types = null)
        {
            // Create a string builder
            var stringBuilder = new StringBuilder();

            // Create the serialiser from the type
            var xmlSerializer = new XmlSerializer(obj.GetType(), (types != null) ? types : new Type[0]);

            // Create an XML writer to serialise the object as a fragment
            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = false,
            }))
            {
                // Serialise the object
                xmlSerializer.Serialize(xmlWriter, obj);
            }

            // Return the XML fragment as a string
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deserialise an object from XML
        /// </summary>
        /// <typeparam name="T">The type of object required</typeparam>
        /// <param name="obj">Any existing object instance</param>
        /// <param name="fragment">The XML</param>
        /// <param name="types">The additional types required for serialisation</param>
        /// <returns>The new object</returns>
        public static T Deserialise<T>(this T obj, string fragment, Type[] types = null)
        {
            // Create the serialiser from the type
            var xmlSerializer = new XmlSerializer(obj.GetType(), (types != null) ? types : new Type[0]);

            // Create a string reader
            using (var stringReader = new StringReader(fragment))
            {
                // Deserialise the object and return it
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}
