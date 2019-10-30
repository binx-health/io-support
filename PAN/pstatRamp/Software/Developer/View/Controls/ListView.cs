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
    /// Local override of list view
    /// </summary>
    public class ListView : System.Windows.Forms.ListView
    {
        /// <summary>
        /// Scroll event
        /// </summary>
        public event ScrollEventHandler Scroll;

        /// <summary>
        /// Windows procedure override
        /// </summary>
        /// <param name="m">The message</param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if ((m.Msg == 0x115) && (Scroll != null))
            {
                Scroll(this, new ScrollEventArgs((ScrollEventType)(m.WParam.ToInt32() & 0xffff), 0));
            }
        }
    }
}
