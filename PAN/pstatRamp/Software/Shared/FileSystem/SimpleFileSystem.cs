/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.IO;

namespace IO.FileSystem
{
    /// <summary>
    /// Simple file system object
    /// </summary>
    public class SimpleFileSystem: ISimpleFileSystem
    {
        /// <summary>
        /// The path to the application data
        /// </summary>
        private readonly string dataPath;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="path">The path to the data</param>
        public SimpleFileSystem(string path)
        {
            // Initialise the data path
            dataPath = path;
        }

        /// <summary>
        /// Read the named file as text
        /// </summary>
        /// <param name="name">The file name</param>
        /// <returns>The plain text</returns>
        public override string ReadTextFile(string name)
        {
            try
            {
                // Create a stream reader and read the file
                using (var streamReader = new StreamReader(
                    new FileStream(dataPath + name + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                    Encoding.Unicode))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>
        /// Write the named file as text
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="value">The plain text</param>
        public override void WriteTextFile(string name, string value)
        {
            if (value == null)
            {
                try
                {
                    File.Delete(dataPath + name + ".txt");
                }
                catch (DirectoryNotFoundException)
                {
                }
            }
            else
            {
                // Create the directory
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath + name + ".txt"));

                // Create a stream writer and write the file
                using (var streamWriter = new StreamWriter(dataPath + name + ".txt", false, Encoding.Unicode))
                {
                    streamWriter.Write(value);
                }
            }
        }
    }
}
