/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Password rules object
    /// </summary>
    public class PasswordRules : IPasswordRules
    {
        /// <summary>
        /// Default values
        /// </summary>
        private static readonly int DEFAULT_MINIMUM_LENGTH = 6;
        private static readonly int DEFAULT_MINIMUM_ALPHABETICAL = 2;
        private static readonly int DEFAULT_EXPIRY_TIME_IN_DAYS = 0;
        private static readonly bool DEFAULT_OVERRIDE = true;

        /// <summary>
        /// Check a password against the rules 
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>True for a pass, otherwise false</returns>
        public override bool CheckPassword(string password)
        {
            return false;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PasswordRules()
        {
            // Set the default values
            MinimumLength = DEFAULT_MINIMUM_LENGTH;
            MinimumAlphabetical = DEFAULT_MINIMUM_ALPHABETICAL;
            ExpiryTimeInDays = DEFAULT_EXPIRY_TIME_IN_DAYS;
            Override = DEFAULT_OVERRIDE;
        }
    }
}
