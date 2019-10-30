/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using IO.Model.Serializable;

namespace IO.View.Tests
{
    /// <summary>
    /// User object for the test
    /// </summary>
    class User : IUser
    {
        /// <summary>
        /// Flag to indicate the password has expired
        /// </summary>
        [XmlIgnore]
        public override bool PasswordExpired
        {
            get
            {
                // Calculate the password life
                var passwordLife = DateTime.UtcNow - PasswordDate;

                // Check for an expired password
                return ((IConfiguration.Instance.PasswordRules.Override == false) &&
                    (passwordLife.TotalMinutes >
                    (IConfiguration.Instance.PasswordRules.ExpiryTimeInDays * 1440.0)));
            }
        }

        /// <summary>
        /// The date the password was set
        /// </summary>
        [XmlElement("PasswordDate")]
        public override string PasswordDateString
        {
            get
            {
                return PasswordDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                PasswordDate = DateTime.Parse(value);
            }
        }
    }
}
