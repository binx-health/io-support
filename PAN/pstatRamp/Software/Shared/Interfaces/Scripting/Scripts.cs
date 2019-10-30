/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IO.Scripting
{
    /// <summary>
    /// Scripts interface
    /// </summary>
    [XmlRoot("Scripts")]
    public abstract class IScripts : List<IScript>
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IScripts Instance { get; set; }

        /// <summary>
        /// The path from which scripts are loaded
        /// </summary>
        [XmlIgnore]
        public abstract string Path { get; set; }

        /// <summary>
        /// Enumeration of script variables
        /// </summary>
        [XmlIgnore]
        public abstract IDictionary<string, IScriptVariable> Variables { get; }

        /// <summary>
        /// Reload the scripts from the path
        /// </summary>
        public abstract void Reload();

        /// <summary>
        /// Set the value for a script
        /// </summary>
        /// <param name="name">The name of the script</param>
        /// <param name="value">The new value or null to delete the script</param>
        public abstract IScript SetScript(string name, string value);
    }
}
