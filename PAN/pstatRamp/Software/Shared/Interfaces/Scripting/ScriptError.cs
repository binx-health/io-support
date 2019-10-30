/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.Scripting
{
    /// <summary>
    /// Script error object
    /// </summary>
    public class ScriptError
    {
        /// <summary>
        /// The line number for the error (zero based)
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The description for the error (not localized)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Convert this object to a string
        /// </summary>
        /// <returns>The string representation of this object</returns>
        public override string ToString()
        {
            if (Line >= 0)
            {
                return Description + " at line " + (Line + 1);
            }

            return Description;
        }
    }
}
