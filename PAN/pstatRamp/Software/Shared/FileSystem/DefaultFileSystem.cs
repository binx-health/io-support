/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace IO.FileSystem
{
    /// <summary>
    /// Default file system object
    /// </summary>
    public class DefaultFileSystem : ILocalFileSystem, IDisposable
    {
        /// <summary>
        /// Carriage return, line-feed and byte-order-mark characters as integers
        /// </summary>
        private static readonly int CR = '\r';
        private static readonly int LF = '\n';
        private static readonly int BOM = 255;

        /// <summary>
        /// The path to the application data
        /// </summary>
        private readonly string dataPath;

        /// <summary>
        /// The simple file system
        /// </summary>
        private readonly ISimpleFileSystem simpleFileSystem;

        /// <summary>
        /// Dictionary of open file streams
        /// </summary>
        private Dictionary<string, FileStream> fileStreamCache = new Dictionary<string, FileStream>();

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            // Clear and dispose of any open file streams in the cache
            ClearFileStreamCache();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="path">The path to the data</param>
        public DefaultFileSystem(string path)
        {
            // Initialise the data path
            dataPath = path;

            // Aggregate a simple file system object
            simpleFileSystem = new SimpleFileSystem(dataPath);
        }

        /// <summary>
        /// Get a list of files
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string[] GetFiles(string path)
        {
            try
            {
                // Initialise a list of files
                var files = new List<string>();

                // Loop through the paths adding them to the list
                foreach (var filePath in Directory.GetFiles(dataPath + path, "*.txt", SearchOption.TopDirectoryOnly))
                {
                    files.Add(Path.GetFileNameWithoutExtension(filePath));
                }

                return files.ToArray();
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>
        /// Read the named file as text
        /// </summary>
        /// <param name="name">The file name</param>
        /// <returns>The plain text</returns>
        public override string ReadTextFile(string name)
        {
            // Use the simple file system
            return simpleFileSystem.ReadTextFile(name);
        }

        /// <summary>
        /// Write the named file as text
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="value">The plain text</param>
        public override void WriteTextFile(string name, string value)
        {
            // Try to get the filestream from the cache
            FileStream fileStream;

            if (fileStreamCache.TryGetValue(name, out fileStream))
            {
                // Remove the file stream from the cache
                fileStreamCache.Remove(name);

                // Dispose of it
                fileStream.Dispose();
            }

            // Use the simple file system
            simpleFileSystem.WriteTextFile(name, value);
        }

        /// <summary>
        /// Append text onto the end of a file
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="value">The text to append</param>
        /// <returns>The position where the text was appended</returns>
        public override long AppendTextFile(string name, string value)
        {
            // Try to get the filestream from the cache
            FileStream fileStream;

            if (fileStreamCache.TryGetValue(name, out fileStream) == false)
            {
                // Create the directory
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath + name + ".txt"));

                // Create a new file
                fileStreamCache.Add(name, fileStream = File.Open(dataPath + name + ".txt", 
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
            }

            // Seek to the end
            fileStream.Seek(0, SeekOrigin.End);

            // Remember the positon at the end of the file
            long position = fileStream.Position;

            // Append the data to the file using unicode
            byte[] buffer = Encoding.Unicode.GetBytes(value);

            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Flush();

            return position;
        }

        /// <summary>
        /// Read the next line from the unicode file from a given position with an optional regular expression match
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="position">The starting position position</param>
        /// <param name="regex">The regulare expression for matching or null</param>
        /// <returns>The next matching line</returns>
        public override string ReadNextLine(string name, ref long position, Regex regex = null)
        {
            // Try to get the filestream from the cache
            FileStream fileStream;

            if (fileStreamCache.TryGetValue(name, out fileStream) == false)
            {
                // Try to open an existing file
                if (File.Exists(dataPath + name + ".txt"))
                {
                    fileStreamCache.Add(name, fileStream = File.Open(dataPath + name + ".txt",
                        FileMode.Open, FileAccess.ReadWrite, FileShare.Read));
                }
                else
                {
                    return null;
                }
            }

            // Check for a file overrun
            if (position >= fileStream.Length)
            {
                return null;
            }

            // Seek to the current position
            fileStream.Seek(position, SeekOrigin.Begin);

            // Initialise the line that has been read
            string line;

            // Intialise the bytes read in pairs (Unicode)
            int firstByte;
            int secondByte = 0;

            // Initialise the match variable
            bool match;

            // Loop until the end of the file or a match
            do
            {
                // Initialise a list of bytes
                var bytes = new List<byte>();

                // Loop until the end of the file
                while (((firstByte = fileStream.ReadByte()) != -1)
                    && ((secondByte = fileStream.ReadByte()) != -1))
                {
                    // Increment the postion by two bytes
                    position += 2;

                    // Check for a CR or BOM
                    if ((firstByte == CR) || (firstByte == BOM))
                    {
                        // Ignore
                    }
                    // Check for the end of the line
                    else if (firstByte == LF)
                    {
                        break;
                    }
                    else
                    {
                        // Add the bytes to the list
                        bytes.Add((byte)firstByte);
                        bytes.Add((byte)secondByte);
                    }
                }

                // Create the line from the bytes
                line = System.Text.Encoding.Unicode.GetString(bytes.ToArray());

                // Check for a match
                match = (regex == null) || regex.IsMatch(line);
            }
            while ((firstByte != -1) && (secondByte != -1) && (match == false));

            // Retrun the line if it matches
            return match ? line : null;
        }

        /// <summary>
        /// Delete a folder and all its contents from the file system
        /// </summary>
        /// <param name="path">The folder path</param>
        public override void DeleteFolder(string path)
        {
            // Clear the file stream cache
            ClearFileStreamCache();

            // Delete the folder
            if (Directory.Exists(dataPath + path))
            {
                Directory.Delete(dataPath + path, true);
            }
        }

        /// <summary>
        /// Get the removable drives as simple file systems
        /// </summary>
        /// <returns>The array of removable drives as simple file systems</returns>
        public override IEnumerable<ISimpleFileSystem> GetRemovableDrives()
        {
            // Loop through the available removable drives
            foreach (var removableDrive in DriveInfo.GetDrives().Where(x =>
                x.IsReady && (x.DriveType == DriveType.Removable)))
            {
                yield return new SimpleFileSystem(removableDrive.RootDirectory.FullName);
            }
        }

        /// <summary>
        /// Get a simple file system for a folder
        /// </summary>
        /// <param name="folder">The folder</param>
        public override ISimpleFileSystem GetSimpleFileSystem(string folder)
        {
            return new SimpleFileSystem(folder);
        }

        /// <summary>
        /// Clear and dispose of any file streams on the cache
        /// </summary>
        private void ClearFileStreamCache()
        {
            // Dispose of the file streams
            foreach (var fileStream in fileStreamCache.Values)
            {
                fileStream.Dispose();
            }

            // Clear the cache
            fileStreamCache.Clear();
        }
    }
}
