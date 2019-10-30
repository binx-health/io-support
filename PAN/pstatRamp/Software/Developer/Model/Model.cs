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
            Volatile.IDeviceValues.Instance = new Volatile.DeviceValues();

            // Initialise the serializable data
            Serializable.ITests.Instance = new Serializable.Tests();
            Serializable.IConfiguration.Instance = new Serializable.Configuration();
        }
    }
}
