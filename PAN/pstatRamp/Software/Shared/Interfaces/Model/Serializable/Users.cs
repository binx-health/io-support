/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Users interface
    /// </summary>
    [XmlRoot("Users")]
    public abstract class IUsers : List<IUser>
    {
        /// <summary>
        /// The maximum nummber of administrators
        /// </summary>
        public static readonly int MAXIMUM_ADMINISTRATORS = 3;

        /// <summary>
        /// Instance variable accessor
        /// </summary>
        public static IUsers Instance { get; set; }

        /// <summary>
        /// The types for serialisation
        /// </summary>
        public abstract Type[] Types { get; }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="type">The user type</param>
        /// <param name="name">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>The new user</returns>
        public abstract IUser CreateNewUser(
            UserType type,
            string name,
            string password);
    }
}
