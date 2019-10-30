/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace IO.FileSystem
{
    /// <summary>
    /// Ftp file system object
    /// </summary>
    public class DefaultFtpFileSystem : IFtpFileSystem, IDisposable
    {
        /// <summary>
        /// The web client for the server
        /// </summary>
        private WebClient webClient;

        /// <summary>
        /// The URI for the data
        /// </summary>
        private readonly string dataUri;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="uri">The URI for the data</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        public DefaultFtpFileSystem(string uri, string userName, string password)
        {
            dataUri = uri;
            webClient = new WebClient()
            {
                Credentials = new NetworkCredential(userName, password),
            };
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            if (webClient != null)
            {
                webClient.Dispose();
                webClient = null;
            }
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
                // Get the stream from the web clisnt
                using (var stream = new MemoryStream(webClient.DownloadData(dataUri + name + ".txt")))
                {
                    // Read the contents as unicode
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                // Check for a 404 error which means the file doesn't exist
                if ((e.Response is FtpWebResponse) &&
                    (((FtpWebResponse)e.Response).StatusCode == (FtpStatusCode)404))
                {
                    return null;
                }

                throw e;
            }
        }

        /// <summary>
        /// Write the named file as text
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="value">The plain text</param>
        public override void WriteTextFile(string name, string value)
        {
            // Create a web request for the file
            var ftpWebRequest = (FtpWebRequest)WebRequest.Create(dataUri + name + ".txt");

            // Initialise the parameters
            ftpWebRequest.Credentials = webClient.Credentials;
            ftpWebRequest.Method = (value == null) ? WebRequestMethods.Ftp.DeleteFile : 
                WebRequestMethods.Ftp.UploadFile;
            ftpWebRequest.Proxy = null;

            // If we are writing then open the stream
            if (value != null)
            {
                using (var stream = ftpWebRequest.GetRequestStream())
                {
                    // Write the contents as unicode
                    using (var streamWriter = new StreamWriter(stream, Encoding.Unicode))
                    {
                        streamWriter.Write(value);
                    }
                }
            }

            // Get the web response
            using (var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
            {
            }
        }
    }
}
