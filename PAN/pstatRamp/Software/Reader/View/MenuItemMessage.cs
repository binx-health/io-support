/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View
{
    /// <summary>
    /// Menu item message sent from a form
    /// </summary>
    public class MenuItemMessage : Message
    {
        /// <summary>
        /// The menu item associated with the message
        /// </summary>
        public string MenuItem { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="command">The form command</param>
        public MenuItemMessage(string menuItem)
        {
            MenuItem = menuItem;
        }
    }
}
