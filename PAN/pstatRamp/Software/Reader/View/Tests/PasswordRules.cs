/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using IO.Model.Serializable;

namespace IO.View.Tests
{
    /// <summary>
    /// Password rules for the test
    /// </summary>
    class PasswordRules : IPasswordRules
    {
        /// <summary>
        /// Default values
        /// </summary>
        private static readonly int DEFAULT_MINIMUM_LENGTH = 6;
        private static readonly int DEFAULT_MINIMUM_ALPHABETICAL = 2;
        private static readonly int DEFAULT_MINIMUM_ALPHANUMERIC = 4;
        private static readonly int DEFAULT_EXPIRY_TIME_IN_DAYS = 0;
        private static readonly bool DEFAULT_OVERRIDE = true;

        /// <summary>
        /// Check a password against the rules 
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>True for a pass, otherwise false</returns>
        public override bool CheckPassword(string password)
        {
            // Check the override flag
            if (Override)
            {
                return true;
            }

            // Check the minimum length and minimum character types
            if ((password.Length < MinimumLength) ||
                (password.Count(x => char.IsLetter(x)) < MinimumAlphabetical) ||
                (password.Count(x => char.IsNumber(x)) < MinimumAlphanumeric))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PasswordRules()
        {
            // Set the default values
            MinimumLength = DEFAULT_MINIMUM_LENGTH;
            MinimumAlphabetical = DEFAULT_MINIMUM_ALPHABETICAL;
            MinimumAlphanumeric = DEFAULT_MINIMUM_ALPHANUMERIC;
            ExpiryTimeInDays = DEFAULT_EXPIRY_TIME_IN_DAYS;
            Override = DEFAULT_OVERRIDE;
        }
    }
}
