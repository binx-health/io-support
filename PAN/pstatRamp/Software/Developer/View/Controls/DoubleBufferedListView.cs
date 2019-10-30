/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Windows.Forms;

namespace IO.View.Controls
{
    /// <summary>
    /// Double buffered list view
    /// </summary>
    class DoubleBufferedListView : ListView
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DoubleBufferedListView()
        {
            DoubleBuffered = true;
        }
    }
}
