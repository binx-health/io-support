/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View
{
    /// <summary>
    /// Enumeration of form commands
    /// </summary>
    public enum FormCommand
    {
        Initialise,             // Initialise the system
        Next,                   // Move to the next screen
        Abort,                  // Abort the current sequence
        Back,                   // Go back to the previous screen
        Menu,                   // Go to the last menu
        Home,                   // Go back to the home screen
        Login,                  // Login or logout the current user
        Shutdown,               // Shutdown the system
    }
}
