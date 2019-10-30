/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;

namespace IO.View
{
    /// <summary>
    /// Widget holder for drag and drop
    /// </summary>
    internal class WidgetHolder
    {
        /// <summary>
        /// The contained widget
        /// </summary>
        public Controls.Widget Widget { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">The widget type</param>
        public WidgetHolder(string type)
        {
            Widget = Activator.CreateInstance(Type.GetType(type)) as Controls.Widget;
        }
    }
}
