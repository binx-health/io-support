/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IO.View.Controls
{
    /// <summary>
    /// Box widget
    /// </summary>
    public partial class BoxWidget : Widget
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BoxWidget()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Paint override
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calculate the rectangle
            var rectangle = new Rectangle(HALF_BEVEL, HALF_BEVEL, Width - (1 + HALF_BEVEL), 
                Height - (1 + HALF_BEVEL));

            // Calculate the graphic for the switch
            var gp = Widget.GraphicsPathForButton(rectangle, DOUBLE_BEVEL);
            var pen = new Pen(Color.Blue, BEVEL);

            // Draw the switch
            pe.Graphics.DrawPath(pen, gp);

            // Check for text to draw
            if (string.IsNullOrEmpty(Text) == false)
            {
                // Include the border
                rectangle.Inflate(-DOUBLE_BEVEL, -DOUBLE_BEVEL);

                // Draw the text
                TextRenderer.DrawText(pe.Graphics, Text, Font, rectangle,
                    Color.Blue, TextFormatFlags.SingleLine | TextFormatFlags.Top |
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            base.OnPaint(pe);
        }
    }
}
