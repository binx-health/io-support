/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace IO.FileSystem
{
    /// <summary>
    /// Simple file system interface
    /// </summary>
    public abstract class ISimpleFileSystem
    {
        /// <summary>
        /// Read the named file as text
        /// </summary>
        /// <param name="name">The file name</param>
        /// <returns></returns>
        public abstract string ReadTextFile(string name);

        /// <summary>
        /// Write the named file as text
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="value"></param>
        public abstract void WriteTextFile(string name, string value);
    }
}
