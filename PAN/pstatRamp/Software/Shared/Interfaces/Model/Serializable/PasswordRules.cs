/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Password rules interface
    /// </summary>
    public abstract class IPasswordRules
    {
        /// <summary>
        /// The minimum length
        /// </summary>
        public virtual int MinimumLength { get; set; }

        /// <summary>
        /// The minimum length
        /// </summary>
        public virtual int MinimumAlphabetical { get; set; }

        /// <summary>
        /// The expiry time in days
        /// </summary>
        public virtual int ExpiryTimeInDays { get; set; }

        /// <summary>
        /// Override flag
        /// </summary>
        public virtual bool Override { get; set; }

        /// <summary>
        /// Check a password against the rules 
        /// </summary>
        /// <param name="password">The password</param>
        /// <returns>True for a pass, otherwise false</returns>
        public abstract bool CheckPassword(string password);
    }
}
