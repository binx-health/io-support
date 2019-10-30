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
    /// File system interface
    /// </summary>
    public abstract class ILocalFileSystem: ISimpleFileSystem
    {
        /// <summary>
        /// Instance property
        /// </summary>
        public static ILocalFileSystem Instance { get; set; }

        /// <summary>
        /// Get a list of files
        /// </summary>
        /// <param name="path">The path for the folder (e.g. Folder)</param>
        /// <returns>The matching files</returns>
        public abstract string[] GetFiles(string path);

        /// <summary>
        /// Append text onto the end of a file
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="value">The text to append</param>
        /// <returns>The position where the text was appended</returns>
        public abstract long AppendTextFile(string name, string value);

        /// <summary>
        /// Read the next line from the file from a given position with an optional regular expression match
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="position">The starting position</param>
        /// <param name="regex">The regulare expression for matching or null</param>
        /// <returns>The next matching line</returns>
        public abstract string ReadNextLine(string name, ref long position, Regex regex = null);

        /// <summary>
        /// Delete a folder and all its contents from the file system
        /// </summary>
        /// <param name="path">The folder path</param>
        public abstract void DeleteFolder(string path);

        /// <summary>
        /// Get the removable drives as simple file systems
        /// </summary>
        /// <returns>The removable drives as simple file systems</returns>
        public abstract IEnumerable<ISimpleFileSystem> GetRemovableDrives();

        /// <summary>
        /// Get a simple file system for a folder
        /// </summary>
        /// <param name="folder">The folder</param>
        public abstract ISimpleFileSystem GetSimpleFileSystem(string folder);
    }
}
