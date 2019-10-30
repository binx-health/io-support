/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace IO.View
{
    /// <summary>
    /// Combo box control
    /// </summary>
    public partial class ComboBox : Control
    {
        /// <summary>
        /// The width of the title text in pixels
        /// </summary>
        public static readonly int TITLE_WIDTH = 165;

        /// <summary>
        /// The title for the text box
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ComboBox()
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
            pe.Graphics.FillRoundedRectangle(Form.MAIN_COLOR_BRUSH, new Rectangle(0, 0, Width, Height),
                Form.CORNER_RADIUS);

            // Initialise the title rectangle
            var titleRectangle = new Rectangle(Form.DOUBLE_BORDER_WIDTH, 0,
                TITLE_WIDTH - Form.DOUBLE_BORDER_WIDTH, Height);

            // Draw the title text in the background color
            TextRenderer.DrawText(pe.Graphics, Title, Font, titleRectangle, Form.BACKGROUND_COLOR, 
                TextFormatFlags.SingleLine |TextFormatFlags.VerticalCenter | 
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            // Initialise the text rectangle
            var textRectangle = new Rectangle(TITLE_WIDTH, Form.BORDER_WIDTH, 
                Width - (TITLE_WIDTH + Form.BORDER_WIDTH), Height - Form.DOUBLE_BORDER_WIDTH);

            // Fill the rectangle with the background color
            pe.Graphics.FillRightRoundedRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle, 
                Form.CORNER_RADIUS - Form.BORDER_WIDTH);

            textRectangle.Inflate(-Form.BORDER_WIDTH, -Form.BORDER_WIDTH);

            // Initialise values for painting a triangle
            var halfheight = Height / 2;
            var eigthWidth = Form.SCROLL_BAR_WIDTH / 8;
            var quarterWidth = eigthWidth + eigthWidth;
            var halfWidth = quarterWidth + quarterWidth;

            // Paint the triangle
            pe.Graphics.FillPolygon(Form.MAIN_COLOR_BRUSH, new Point[]
                {
                    new Point(((textRectangle.Right - halfWidth) + quarterWidth), halfheight - eigthWidth),
                    new Point(((textRectangle.Right - halfWidth) - quarterWidth), halfheight - eigthWidth),
                    new Point(textRectangle.Right - halfWidth, halfheight + eigthWidth)
                });

            // Offset the text to accomodate the triangle and add some whitespace
            textRectangle.Width -= Form.SCROLL_BAR_WIDTH;

            // Add some whitespace
            textRectangle.Inflate(-Form.BORDER_WIDTH, 0);

            // Draw the text
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle, Form.TEXT_COLOR, 
                TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | 
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }
    }
}
