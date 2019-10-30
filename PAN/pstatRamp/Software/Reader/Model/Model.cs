/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Xml.Serialization;
using IO.FileSystem;

namespace IO.Model
{
    /// <summary>
    /// Static model functions
    /// </summary>
    public static class Model
    {
        /// <summary>
        /// Initialise the model
        /// </summary>
        public static void Initialise()
        {
            // Initialise the volatile data
            Volatile.ISessions.Instance = new Volatile.Sessions();
            Volatile.IDeviceValues.Instance = new Volatile.DeviceValues();

            // Initialise the serialisable data
            Serializable.IAuditLog.Instance = new Serializable.AuditLog();
            Serializable.IState.Instance = new Serializable.State();
            Serializable.IConfiguration.Instance = LoadFromFile<Serializable.Configuration>("Configuration",
                new Type[] 
                { 
                    typeof(Serializable.PasswordRules), 
                    typeof(Serializable.Field) 
                });
            Serializable.IConfiguration.Instance.Modified = false;
            Serializable.IUsers.Instance = LoadFromFile<Serializable.Users>("Users",
                new Type[] 
                { 
                    typeof(Serializable.User), 
                });
            Serializable.ITests.Instance = LoadFromFile<Serializable.Tests>("Tests",
                new Type[] 
                { 
                    typeof(Serializable.Tests), 
                    typeof(Serializable.Test), 
                    typeof(Serializable.TestResult), 
                    typeof(Serializable.TestData),
                });
            Serializable.ITests.Instance.CurrentTest = new Serializable.Test();
            Serializable.IResults.Instance = new Serializable.Results();
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
                // Create the serialiser from the type
                var xmlSerializer = new XmlSerializer(typeof(T), extraTypes);

                // Create a string reader from the text file, read from the file system
                using (var stringReader = new StringReader(
                    ILocalFileSystem.Instance.ReadTextFile(name)))
                {
                    // Return the deserialised object
                    return (T)xmlSerializer.Deserialize(stringReader);
                }
            }
            catch (Exception e)
            {
                LastError.Message = e.Message;

                return new T();
            }
        }
    }
}
