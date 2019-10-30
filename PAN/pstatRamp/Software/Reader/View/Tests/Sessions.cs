/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using IO.Model.Serializable;
using IO.Model.Volatile;

namespace IO.View.Tests
{
    /// <summary>
    /// The sessions for the test
    /// </summary>
    public class Sessions : ISessions
    {
        /// <summary>
        /// The current session
        /// </summary>
        public override ISession CurrentSession
        {
            get
            {
                // Initialise the current session variable
                ISession currentSession = null;

                // Check for a current user
                if (CurrentUser != null)
                {
                    // Try to get the session for the current user
                    if (TryGetValue(CurrentUser.ID, out currentSession) == false)
                    {
                        // There isn't one so create it
                        currentSession = new Session();

                        // And add it to the list
                        Add(CurrentUser.ID, currentSession);
                    }
                }

                return currentSession;
            }
        }
    }
}
