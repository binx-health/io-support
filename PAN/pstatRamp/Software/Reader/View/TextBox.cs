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
    /// Text box control
    /// </summary>
    public partial class TextBox : Control
    {
        /// <summary>
        /// The maximum width of the arrow box
        /// </summary>
        private static readonly int MAX_ARROW_WIDTH = 20;

        /// <summary>
        /// The width of the title text in pixels
        /// </summary>
        public static readonly int DEFAULT_TITLE_WIDTH = 165;

        /// <summary>
        /// Down flag indicating the button is pressed
        /// </summary>
        private bool down = false;

        /// <summary>
        /// The character to display for passwords
        /// </summary>
        public static readonly char PASSWORD_CHAR = '*';

        /// <summary>
        /// Title width in pixels
        /// </summary>
        public int TitleWidth { get; set; }

        /// <summary>
        /// The title for the text box
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Password flag
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TextBox()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Make transparent and double buffer to avoid flicker
            DoubleBuffered = true;
            BackColor = Color.Transparent;

            // Initialise the title width
            TitleWidth = DEFAULT_TITLE_WIDTH;
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

            // Check for a lift outside of the text box
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

            // Initialise the title rectangle
            var titleRectangle = new Rectangle(Form.DOUBLE_BORDER_WIDTH, 0,
                TitleWidth - Form.DOUBLE_BORDER_WIDTH, Height);

            // Draw the title text in the background color
            TextRenderer.DrawText(pe.Graphics, Title, Font, titleRectangle, Form.BACKGROUND_COLOR, 
                TextFormatFlags.SingleLine |TextFormatFlags.VerticalCenter | 
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            // Initialise the text rectangle
            var textRectangle = new Rectangle(TitleWidth, Form.BORDER_WIDTH,
                Width - (TitleWidth + Form.BORDER_WIDTH), Height - Form.DOUBLE_BORDER_WIDTH);

            if (down == false)
            {
                // Fill the rectangle with the background color
                pe.Graphics.FillRightRoundedRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle,
                    Form.CORNER_RADIUS - Form.BORDER_WIDTH);
            }

            // Initialise values for painting a triangle
            var quarterHeight = Height / 4;
            var halfHeight = quarterHeight + quarterHeight;
            var halfWidth = Math.Min(halfHeight, MAX_ARROW_WIDTH);

            quarterHeight = halfWidth / 2;

            // Paint the triangle
            pe.Graphics.FillPolygon(Form.MAIN_COLOR_BRUSH, new Point[]
                {
                    new Point(TitleWidth + halfWidth, halfHeight),
                    new Point(TitleWidth + halfWidth - quarterHeight, halfHeight - quarterHeight),
                    new Point(TitleWidth + halfWidth - quarterHeight, halfHeight + quarterHeight)
                });

            // Offset the text to accomodate the triangle and add some whitespace
            textRectangle.X += halfWidth + Form.BORDER_WIDTH;
            textRectangle.Width -= halfWidth + Form.DOUBLE_BORDER_WIDTH;

            // Generate password text if necessary
            string text = Password ? new StringBuilder().Append(PASSWORD_CHAR, Text.Length).ToString() : Text;

            // Draw the text
            TextRenderer.DrawText(pe.Graphics, text, Font, textRectangle, 
                down ? Form.BACKGROUND_COLOR : Form.TEXT_COLOR, 
                TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | 
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }
    }
}
