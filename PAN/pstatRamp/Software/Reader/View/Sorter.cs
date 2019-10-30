/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IO.View
{
    /// <summary>
    /// Key control
    /// </summary>
    public partial class Sorter : Control
    {
        /// <summary>
        /// The toggled state of the button
        /// </summary>
        public bool Toggled { get; set; }

        /// <summary>
        /// The type of arrow on the button
        /// </summary>
        public ArrowType ArrowType { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Sorter()
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
            // Call the base method
            base.OnPaint(pe);

            // Fill the control with the main color
            pe.Graphics.FillRectangle(Form.MAIN_COLOR_BRUSH, new Rectangle(0, 0, Width, Height));

            // Intialise a rectangle for the text
            var textRectangle = new Rectangle(Form.BORDER_WIDTH, Form.BORDER_WIDTH,
                Width - Form.DOUBLE_BORDER_WIDTH, Height - Form.DOUBLE_BORDER_WIDTH);

            // If we are not toggled then fill the rectangle with the background color
            if (Toggled == false)
            {
                pe.Graphics.FillRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle);
            }

            // Initialise variables for drawing
            int eightHeight = Height / 8;
            int quarterHeight = eightHeight + eightHeight;
            int halfHeight = quarterHeight + quarterHeight;

            // Remember and set the smoothing mode to anti-alias
            var originalSmoothingMode = pe.Graphics.SmoothingMode;

            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Check for the arrow direction
            if (ArrowType == ArrowType.Down)
            {
                // Paint the triangle
                pe.Graphics.FillPolygon(Toggled ? Form.BACKGROUND_COLOR_BRUSH : Form.MAIN_COLOR_BRUSH, 
                    new Point[]
                    {
                        new Point(halfHeight + eightHeight, halfHeight - eightHeight),
                        new Point(halfHeight - eightHeight, halfHeight - eightHeight),
                        new Point(halfHeight, halfHeight),
                    });
            }
            else if (ArrowType == ArrowType.Up)
            {
                // Paint the triangle
                pe.Graphics.FillPolygon(Toggled ? Form.BACKGROUND_COLOR_BRUSH : Form.MAIN_COLOR_BRUSH, 
                    new Point[]
                    {
                        new Point(halfHeight, halfHeight - eightHeight),
                        new Point(halfHeight + eightHeight, halfHeight),
                        new Point(halfHeight - eightHeight, halfHeight),
                    });
            }

            // Reset the smoothing mode
            pe.Graphics.SmoothingMode = originalSmoothingMode;

            // Offset the text by the size of the arrow
            textRectangle.X += halfHeight;
            textRectangle.Width -= halfHeight;

            // Draw the text in the relevant color
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle, 
                Toggled ? Form.BACKGROUND_COLOR : Form.TEXT_COLOR,
                TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
