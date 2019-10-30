/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Xml.Serialization;
using IO.FileSystem;

namespace IO.Scripting
{
    /// <summary>
    /// Static scripting functions
    /// </summary>
    public static class Scripting
    {
        /// <summary>
        /// Initialise the model
        /// </summary>
        public static void Initialise()
        {
            // Create the scripts object
            IScripts.Instance = new Scripts();

            // Create the default metrics object
            IDefaultMetrics.Instance = new Metrics("DefaultMetrics", null);

            // Try to load the model
            try
            {
                // Load the default metrics
                IDefaultMetrics.Instance.Value = ILocalFileSystem.Instance.ReadTextFile(
                    "Metrics\\DefaultMetrics");

                // Load the non-default metrics
                var metricsFiles = ILocalFileSystem.Instance.GetFiles("Metrics");

                if (metricsFiles != null)
                {
                    foreach (var path in metricsFiles)
                    {
                        if (path != "DefaultMetrics")
                        {
                            Metrics.NonDefault[path] = new Metrics(path, IDefaultMetrics.Instance)
                            {
                                Value = ILocalFileSystem.Instance.ReadTextFile("Metrics\\" + path)
                            };
                        }
                    }
                }
                // Initialise the assay data
                IAssays.Instance = LoadFromFile<Assays>("Assays", new Type[] { typeof(Assay), typeof(Disease) });

                // Load the scripts
                IScripts.Instance.Path = "Scripts";
            }
            catch (Exception e)
            {
                LastError.Message = e.Message;
            }

        }

        /// <summary>
        /// Load a generic object from the data path
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="name">The name of the file</param>
        /// <param name="extraTypes">The additional types required ofr serialisation</param>
        /// <returns>The deserialised object or a new instance of the object</returns>
        private static T LoadFromFile<T>(string name, Type[] extraTypes = null) where T : new()
        {
            // If serialisation fails the return a new object
            try
            {
                // Read the contents of the file
                var value = ILocalFileSystem.Instance.ReadTextFile(name);

                // Check for a missing file
                if (value == null)
                {
                    return new T();
                }
                else
                {
                    // Create the serialiser from the type
                    var xmlSerializer = new XmlSerializer(typeof(T), extraTypes);

                    // Create a string reader from the text file, read from the file system
                    using (var stringReader = new StringReader(value))
                    {
                        // Return the deserialised object
                        return (T)xmlSerializer.Deserialize(stringReader);
                    }
                }
            }
            catch (Exception e)
            {
                LastError.Message = e.Message;

                return default(T);
            }
        }
    }
}
