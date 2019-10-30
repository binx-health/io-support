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
    /// FTP file system interface
    /// </summary>
    public abstract class IFtpFileSystem : ISimpleFileSystem
    {
        /// <summary>
        /// Instance property
        /// </summary>
        public static IFtpFileSystem Instance { get; set; }
    }
}
