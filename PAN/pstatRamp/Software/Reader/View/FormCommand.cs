using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Home,                   // Go to the home screen
        Login,                  // Login or logout the current user
        Help,                   // Show help for the current screen
        Shutdown,               // Shutdown the system
    }
}
