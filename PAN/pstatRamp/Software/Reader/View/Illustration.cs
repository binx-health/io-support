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
    /// Image control
    /// </summary>
    public partial class Illustration : Control
    {
        /// <summary>
        /// The image to display for the button
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Illustration()
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

            // Intialise a rectangle for the text
            var imageRectangle = new Rectangle(Form.BORDER_WIDTH, Form.BORDER_WIDTH, Width - Form.DOUBLE_BORDER_WIDTH,
                Height - Form.DOUBLE_BORDER_WIDTH);

            // Fill the rectangle with the background color
            pe.Graphics.FillRoundedRectangle(Form.BACKGROUND_COLOR_BRUSH, imageRectangle,
                Form.CORNER_RADIUS - Form.BORDER_WIDTH);

            if (Image != null)
            {
                imageRectangle.Inflate(-Form.CORNER_RADIUS - Form.BORDER_WIDTH, 0);
                imageRectangle.Inflate((Image.Width - imageRectangle.Width) / 2,
                    (Image.Height - imageRectangle.Height) / 2);

                pe.Graphics.DrawImageUnscaled(Image, imageRectangle);
            }
        }
    }
}
