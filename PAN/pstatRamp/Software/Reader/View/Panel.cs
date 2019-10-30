/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace IO.View
{
    /// <summary>
    /// Panel control
    /// </summary>
    public partial class Panel : Control
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Panel()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Make transparent and double buffer to avoid flicker
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        /// <summary>
        /// Paint event handler
        /// </summary>
        /// <param name="pe">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Fill the control with the main color
            pe.Graphics.FillRoundedRectangle(Form.MAIN_COLOR_BRUSH, new Rectangle(0, 0, Width, Height), 
                Form.CORNER_RADIUS);

            // Intialise a rectangle for the text
            var textRectangle = new Rectangle(Form.BORDER_WIDTH, Form.BORDER_WIDTH,
                Width - Form.DOUBLE_BORDER_WIDTH, Height - Form.DOUBLE_BORDER_WIDTH);

            // Fill the rectangle with the background color
            pe.Graphics.FillRoundedRectangle(Form.PANEL_COLOR_BRUSH, textRectangle, 
                Form.CORNER_RADIUS - Form.BORDER_WIDTH);

            // Add whitespace to the rectange
            textRectangle.Inflate(-Form.CORNER_RADIUS, -Form.CORNER_RADIUS);

            // Draw the text in the relevant color
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle,
                Form.TEXT_COLOR, TextFormatFlags.VerticalCenter |
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
