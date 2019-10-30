/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;

namespace IO.Model.Serializable
{
    /// <summary>
    /// User interface
    /// </summary>
    public abstract class IUser
    {
        /// <summary>
        /// The user name, used to uniquely identify the user
        /// </summary>
        public virtual uint ID { get; set; }

        /// <summary>
        /// The user name, used to uniquely identify the user
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The type of user
        /// </summary>
        public virtual UserType Type { get; set; }

        /// <summary>
        /// Flag to indicate the password has expired
        /// </summary>
        [XmlIgnore]
        public abstract bool PasswordExpired { get; }

        /// <summary>
        /// The date the password was set
        /// </summary>
        [XmlIgnore]
        public virtual DateTime PasswordDate { get; set; }
        [XmlElement("PasswordDate")]
        public abstract string PasswordDateString { get; set; }

        /// <summary>
        /// Non-serialised setter property for typed passwords
        /// </summary>
        public virtual string Password
        {
            set
            {
                PasswordHash = HashPassword(value);
            }
        }

        /// <summary>
        /// The serialised password hash
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// Hash a password
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>The hash</returns>
        public static string HashPassword(string password)
        {
            // Use the MD5 hash to generate the password hash value
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                // Base 64 encode the result
                return System.Convert.ToBase64String(
                    md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
