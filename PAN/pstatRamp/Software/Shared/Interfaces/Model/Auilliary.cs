/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Model
{
    /// <summary>
    /// Auxilliary functions
    /// </summary>
    public static class Auilliary
    {
        /// <summary>
        /// Get the test outcome from a string
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>The test outcome</returns>
        public static Serializable.TestOutcome TestOutcomeValue(this string value)
        {
            // Switch to lower case
            string lowerValue = value.ToLower();

            if (lowerValue == "valid")
            {
                return Serializable.TestOutcome.Valid;
            }
            else if (lowerValue == "useraborted")
            {
                return Serializable.TestOutcome.UserAborted;
            }
            else
            {
                return Serializable.TestOutcome.Error;
            }
        }

        /// <summary>
        /// Get a boolean from a string
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>The boolean</returns>
        public static bool BooleanValue(this string value)
        {
            return value.ToLower().StartsWith("t");
        }
    }
}
