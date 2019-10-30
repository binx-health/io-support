/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;

namespace IO.Scripting
{
    /// <summary>
    /// Auxilliary functions
    /// </summary>
    public static class Auilliary
    {
        /// <summary>
        /// Check the string for inverted commas and strip them if they are present
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="enforce">True if the string must be delimited</param>
        /// <returns>The string value of the token or null if the string is not delimited and the enforced
        /// flag is set to true</returns>
        public static string StringValue(this string token, bool enforce = false)
        {
            // Check for an inverted comma delimited string
            if ((token.Length > 1) && token.StartsWith("\"") && token.EndsWith("\""))
            {
                // Strip of the inverted commas
                return token.Substring(1, token.Length - 2);
            }

            // Otherwise return the original token
            return enforce ? null : token;
        }

        /// <summary>
        /// Check for a valid integer
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="value">The integer value</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsInteger(this string token, out int value)
        {
            // Parse the value as an int and return the result
            return int.TryParse(token, out value);
        }

        /// <summary>
        /// Check for a valid double
        /// </summary>
        /// <param name="token">The token</param>
        /// <param name="value">The double value</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsDouble(this string token, out double value)
        {
            // Initialsie the value to null;
            value = Double.NaN;

            // Get the index of the decimal point
            int index = token.IndexOf('.');

            // Check for no decimal point
            if (index == -1)
            {
                return false;
            }
            
            // Check for a number following the decimal point
            if (index == token.Length - 1)
            {
                return false;
            }
            
            // Check for more than 1 decimal point
            if (token.LastIndexOf('.') != token.IndexOf('.'))
            {
                return false;
            }

            // Parse the value as a double and return the result
            return double.TryParse(token, out value);
        }

        /// <summary>
        /// String tokeniser
        /// </summary>
        /// <param name="line">The line to tokenise</param>
        /// <param name="error">The error description or null</param>
        /// <returns>A list of tokens or null if an error occurs</returns>
        public static List<string> Tokenise(this string line, out string error)
        {
            // Initilaise variables
            int index = 0;
            var tokens = new List<string>();
            var whitespace = new List<char>() { ' ', '\t' };

            // Initialise the error to null
            error = null;

            // Tokenise the string on whitepace, comments and inverted commas
            while (index < line.Length)
            {
                // Read whitespace
                while ((index < line.Length) && whitespace.Contains(line[index]))
                {
                    ++index;
                }

                // Check for end of line
                if (index < line.Length)
                {
                    // Remember the start position
                    int startIndex = index;

                    if (line[index] == '#')
                    {
                        return tokens;
                    }
                    else if (line[index] == '"')
                    {
                        // Read to the next inverted comma
                        do
                        {
                            ++index;
                        }
                        while ((index < line.Length) && (line[index] != '"'));

                        if (index < line.Length)
                        {
                            // Add the token
                            tokens.Add(line.Substring(startIndex, ++index - startIndex));
                        }
                        else
                        {
                            error = "Unterminated string";
                            return null;
                        }
                    }
                    else
                    {
                        // Read non-whitespace
                        while ((index < line.Length) && (whitespace.Contains(line[index]) == false))
                        {
                            ++index;
                        }

                        // Add the token
                        tokens.Add(line.Substring(startIndex, index - startIndex));
                    }
                }
            }

            // Return the tokens
            return tokens;
        }
    }
}
