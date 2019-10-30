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
    /// Radio button control
    /// </summary>
    public partial class Radio : Control
    {
        /// <summary>
        /// The checked state of the box
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Radio()
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

            // Initialise the button rectangle
            var buttonRectangle = new Rectangle(1, 1, Height -2, Height - 2);

            // Rememeber the current smoothing mode
            var originalSmoothingMode = pe.Graphics.SmoothingMode;

            // Set the smoothing to anti-alias
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw the edge of the button
            pe.Graphics.FillEllipse(Form.PANEL_COLOR_BRUSH, buttonRectangle);

            // Draw the middle of the button
            buttonRectangle.Inflate(-1, -1);
            pe.Graphics.FillEllipse(Form.BACKGROUND_COLOR_BRUSH, buttonRectangle);

            // If we are checked draw the chekced button
            if (Checked)
            {
                buttonRectangle.Inflate(-3, -3);
                pe.Graphics.FillEllipse(Form.BLUE_COLOR_BRUSH, buttonRectangle);
            }

            // Reset the smoothing mode
            pe.Graphics.SmoothingMode = originalSmoothingMode;

            // Define the text rectangle
            var textRectangle = new Rectangle(Height, 0, Width - Height, Height);

            // Draw the text in the relevant color
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle, Form.TEXT_COLOR,
                TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }
    }
}
