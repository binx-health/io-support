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
    /// Button control
    /// </summary>
    public partial class Button : Control
    {
        /// <summary>
        /// The maximum width of the arrow box
        /// </summary>
        private static readonly int MAX_ARROW_WIDTH = 20;

        /// <summary>
        /// Down flag indicating the button is pressed
        /// </summary>
        private bool down = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Button()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Double buffer to avoid flicker
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        /// <summary>
        /// Mouse down event override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Set the down flag
            down = true;

            // Refresh the control
            Refresh();
        }

        /// <summary>
        /// Mouse up event override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
 	        base.OnMouseUp(e);

            // Check for a lift outside of the button
            if (ClientRectangle.Contains(e.Location) == false)
            {
                base.OnClick(e);
            }

            // Set the down flag
            down = false;

            // Refresh the control
            Refresh();
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
            pe.Graphics.FillRoundedRectangle(Form.MAIN_COLOR_BRUSH, new Rectangle(0, 0, Width, Height),
                Form.CORNER_RADIUS);

            // Initialise values for painting a triangle
            var eigthHeight = Height / 8;
            var quarterHeight = eigthHeight + eigthHeight;
            var halfHeight = quarterHeight + quarterHeight;
            var halfWidth = Math.Min(halfHeight, MAX_ARROW_WIDTH);

            quarterHeight = halfWidth / 2;
            eigthHeight = quarterHeight / 2;

            // Paint the triangle
            pe.Graphics.FillPolygon(Brushes.White, new Point[]
            {
                new Point(halfWidth + eigthHeight, halfHeight),
                new Point(halfWidth - eigthHeight, halfHeight - quarterHeight),
                new Point(halfWidth - eigthHeight, halfHeight + quarterHeight)
            });

            // Intialise a rectangle for the text
            var textRectangle = new Rectangle((halfWidth + halfWidth) - Form.BORDER_WIDTH, Form.BORDER_WIDTH,
                Width - (halfWidth + halfWidth), Height - Form.DOUBLE_BORDER_WIDTH);

            if (down == false)
            {
                // Fill the rectangle with the background color
                pe.Graphics.FillRightRoundedRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle,
                    Form.CORNER_RADIUS - Form.BORDER_WIDTH);
            }

            // Pad the string with whitespace
            textRectangle.Inflate(-Form.DOUBLE_BORDER_WIDTH, 0);

            // Draw the text
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle, 
                down ? Form.BACKGROUND_COLOR : Form.TEXT_COLOR, 
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.WordBreak);
        }
    }
}
