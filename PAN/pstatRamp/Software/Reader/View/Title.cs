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
    /// Title control
    /// </summary>
    public partial class Title : Control
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Title()
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
            pe.Graphics.FillRoundedRectangle(Form.TITLE_COLOR_BRUSH, new Rectangle(0, 0, Width, Height), 
                Form.CORNER_RADIUS);

            // Intialise a rectangle for the text
            var textRectangle = new Rectangle(0, 0, Width, Height);

            // Add whitespace to the rectange
            textRectangle.Inflate(-Form.CORNER_RADIUS, -Form.CORNER_RADIUS);

            // Draw the text in the relevant color
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle,
                Form.BACKGROUND_COLOR, TextFormatFlags.VerticalCenter |
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
