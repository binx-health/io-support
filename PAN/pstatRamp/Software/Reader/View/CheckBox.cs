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
    /// Check box control
    /// </summary>
    public partial class CheckBox : Control
    {
        /// <summary>
        /// The image to display for the button
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// The state of the checkbox
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckBox()
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

            // Initialise the text rectangle
            var checkRectangle = new Rectangle(Width - Height, Form.BORDER_WIDTH,
                Height - Form.DOUBLE_BORDER_WIDTH, Height - Form.DOUBLE_BORDER_WIDTH);

            // Create some space around the check
            checkRectangle.Inflate(-Form.DOUBLE_BORDER_WIDTH, -Form.DOUBLE_BORDER_WIDTH);

            // Fill the background with the background color
            pe.Graphics.FillRectangle(Form.BACKGROUND_COLOR_BRUSH, checkRectangle);

            // Check for a valid check mark
            if (Checked && (Image != null))
            {
                // Draw the image in the rectangle
                pe.Graphics.DrawImageUnscaledAndClipped(Image, checkRectangle);
            }

            // Initialise the text rectangle
            var textRectangle = new Rectangle(Form.DOUBLE_BORDER_WIDTH, Form.BORDER_WIDTH,
                Width - Height, Height - Form.DOUBLE_BORDER_WIDTH);

            // Draw the text in the background color
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle, Form.BACKGROUND_COLOR,
                TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }
    }
}
