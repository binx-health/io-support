/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IO.Model.Serializable
{
    /// <summary>
    /// List of user objects
    /// </summary>
    [XmlRoot("Users")]
    public class Users : IUsers
    {
        /// <summary>
        /// The types for serialisation
        /// </summary>
        public override Type[] Types
        {
            get
            {
                return new Type[] { typeof(User) };
            }
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="type">The user type</param>
        /// <param name="name">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>The new user</returns>
        public override IUser CreateNewUser(
            UserType type,
            string name,
            string password)
        {
            // Create a new user with an ID one greater than the maximum
            IUser newUser = new User()
            {
                ID = this.Select(x => x.ID).Max() + 1,
                Type = type, 
                Name = name, 
                Password = password, 
                PasswordDate = DateTime.UtcNow,
            };

            // Add the user to the list
            Add(newUser);

            return newUser;
        }
    }
}
