/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

namespace IO.FileSystem.Tests
{
    /// <summary>
    /// Test suite for the file system
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// The number of records to use for the append test (must be exact multiple of 10)
        /// </summary>
        private const int APPEND_TEST_RECORDS = 100000;

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Arguments that define the test data to use</param>
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("FileSystem Tests starting");

                // Initialise the tests
                Initialise();

                // Instantiate the new default file system
                using (var defaultFileSystem = new DefaultFileSystem("Data\\"))
                {
                    ILocalFileSystem.Instance = defaultFileSystem;

                    // Run the tests
                    TestFileReadback();
                    TestFileEnumeration();
                    TestFileAppend();
                    TestRemovableStorage();
                }

                // Instantiate the FTP file system
                using (var ftpFileSystem = new DefaultFtpFileSystem(
                    "ftp://212.250.215.12/To TTP/", "encore", "RoundOfApplause#"))
                {
                    IFtpFileSystem.Instance = ftpFileSystem;

                    // Run the tests
                    TestFtpFileReadback();
                }


                Console.WriteLine("FileSystem Tests passed");
            }
            catch (Exception e)
            {
                // Write the exception
                Console.WriteLine("FileSystem Tests failed");
                Console.Error.WriteLine(e.Message);
                Environment.ExitCode = 1;
            }
        }

        /// <summary>
        /// Initialise the tests
        /// </summary>
        private static void Initialise()
        {
            // Delete any existing data
            try
            {
                Directory.Delete("Data", true);
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        /// <summary>
        /// Test the local file system readback
        /// </summary>
        private static void TestFileReadback()
        {
            // Generate a file full of unicode characters
            var stringBuilder = new StringBuilder();

            for (char c = char.MinValue; c < char.MaxValue; ++c)
            {
                var category = Char.GetUnicodeCategory(c);

                if ((category != UnicodeCategory.Control) &&
                    (category != UnicodeCategory.Surrogate) &&
                    (category != UnicodeCategory.PrivateUse))
                {
                    stringBuilder.Append(c);
                }
            }

            // Write a test file
            ILocalFileSystem.Instance.WriteTextFile("Test", stringBuilder.ToString());

            // Read and verify the contents
            if (ILocalFileSystem.Instance.ReadTextFile("Test") != stringBuilder.ToString())
            {
                throw new ApplicationException("File readback failed for data");
            }

            // Delete the file
            ILocalFileSystem.Instance.WriteTextFile("Test", null);

            // Check the file is deleted
            if (ILocalFileSystem.Instance.ReadTextFile("Test") != null)
            {
                throw new ApplicationException("File readback failed for no data");
            }
        }

        /// <summary>
        /// Test the local file system enumeration
        /// </summary>
        private static void TestFileEnumeration()
        {
            // Generate four files
            var fileNames = new HashSet<string>() { "One", "Two", "Three", "Four" };

            foreach (var name in fileNames)
            {
                ILocalFileSystem.Instance.WriteTextFile("Enumerate\\" + name, "");
            }

            // Read back the files as a set
            var fileNamesRead = new HashSet<string>(ILocalFileSystem.Instance.GetFiles("Enumerate"));

            // Check for equality
            if (fileNames.SetEquals(fileNamesRead) == false)
            {
                throw new ApplicationException("File enumeration failed for files");
            }

            // Delete the folder
            ILocalFileSystem.Instance.DeleteFolder("Enumerate");

            if (ILocalFileSystem.Instance.GetFiles("Enumerate") != null)
            {
                throw new ApplicationException("File enumeration failed for no files");
            }
        }

        /// <summary>
        /// Test the local file system append
        /// </summary>
        private static void TestFileAppend()
        {
            // Initialise a values file containing APPEND_TEST_RECORDS integers
            int count;
            long position = 0;

            for (count = 0; count < APPEND_TEST_RECORDS; ++count)
            {
                ILocalFileSystem.Instance.AppendTextFile("Temp\\Values", count.ToString() + "\r\n");
            }

            // Count and check the number of records
            count = 0;

            while (ILocalFileSystem.Instance.ReadNextLine("Temp\\Values", ref position) != null)
            {
                ++count;
            }

            if (count != APPEND_TEST_RECORDS)
            {
                throw new ApplicationException("File append failed for " + APPEND_TEST_RECORDS + " records");
            }

            // Count and check the number of records that end with a 9
            var regex = new Regex("9$");

            count = 0;
            position = 0;

            while (ILocalFileSystem.Instance.ReadNextLine("Temp\\Values", ref position, regex) != null)
            {
                ++count;
            }

            if (count != APPEND_TEST_RECORDS / 10)
            {
                throw new ApplicationException("File append failed for " + APPEND_TEST_RECORDS + 
                    " records ending in 9");
            }

            // Make a copy of the file
            ILocalFileSystem.Instance.WriteTextFile("Values", 
                ILocalFileSystem.Instance.ReadTextFile("Temp\\Values"));

            // Append the next value
            ILocalFileSystem.Instance.AppendTextFile("Values", APPEND_TEST_RECORDS.ToString());

            // Delete the file
            ILocalFileSystem.Instance.WriteTextFile("Values", null);

            // Attempt to read the deleted file
            position = 0;

            if (ILocalFileSystem.Instance.ReadNextLine("Values", ref position) != null)
            {
                throw new ApplicationException("File append failed for no file");
            }

            // Delete the original and its folder
            ILocalFileSystem.Instance.DeleteFolder("Temp");
        }

        /// <summary>
        /// Test the removable storage
        /// </summary>
        private static void TestRemovableStorage()
        {
            // Generate a globally unique string
            var guid = Guid.NewGuid().ToString();

            // Get the removable drives
            var removableDrives = ILocalFileSystem.Instance.GetRemovableDrives();
            int count = 0;

            // Loop through the drives
            foreach (var simpleFileSystem in removableDrives)
            {
                // Write a text file
                simpleFileSystem.WriteTextFile(guid, guid);

                // Read and verify the file
                if (simpleFileSystem.ReadTextFile(guid) != guid)
                {
                    throw new ApplicationException("Removable storage file readback failed");
                }

                // Delete the file
                simpleFileSystem.WriteTextFile(guid, null);

                ++count;
            }

            // Check that there was at leas on removable drive
            if (count == 0)
            {
                throw new ApplicationException("Removable storage test failed as there are no drives");
            }
        }

        /// <summary>
        /// Test the FTP file system readback
        /// </summary>
        private static void TestFtpFileReadback()
        {
            // Generate a globally unique string
            var guid = Guid.NewGuid().ToString();

            // Write a text file to the ftp server
            IFtpFileSystem.Instance.WriteTextFile(guid, guid);

            // Read and verify the file
            if (IFtpFileSystem.Instance.ReadTextFile(guid) != guid)
            {
                throw new ApplicationException("FTP file readback failed");
            }

            // Delete the file
            IFtpFileSystem.Instance.WriteTextFile(guid, null);
        }
    }
}
