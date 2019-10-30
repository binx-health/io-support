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
    /// Scroll bar control
    /// </summary>
    public partial class ScrollBar : Control
    {
        /// <summary>
        /// The current position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The step size
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// The maximum position
        /// </summary>
        public int Maximum { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScrollBar()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Make transparent and double buffer to avoid flicker
            DoubleBuffered = true;
            BackColor = Color.Transparent;

            // Initialise to sensible values
            Position = 0;
            Step = 1;
            Maximum = 100;
        }

        /// <summary>
        /// OnClick override to implement scrolling and index selection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            // Get the mouse coordinates
            var mouseEventArgs = (MouseEventArgs)e;

            // Check for a page-up or down
            if (mouseEventArgs.Y < Height / 2)
            {
                // Decrement the valueIndex
                Position = Math.Max(0, Position - Step);
            }
            else
            {
                // Increment the value index
                Position = Math.Min(Maximum, Position + Step);
            }

            // Fire the click event
            base.OnClick(e);
        }

        /// <summary>
        /// Paint event handler
        /// </summary>
        /// <param name="pe">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Call the base method
            base.OnPaint(pe);

            // Initialise the dimensions of the scroll bar buttons
            var thirdHeight = Height / 3;
            var topButtonRectangle = new Rectangle(0, 0, Width, thirdHeight);
            var bottomButtonRectangle = new Rectangle(0, Height - thirdHeight, Width, thirdHeight);

            // Draw the buttons
            pe.Graphics.FillRoundedRectangle(Form.MAIN_COLOR_BRUSH, topButtonRectangle,
                Form.CORNER_RADIUS);
            pe.Graphics.FillRoundedRectangle(Form.MAIN_COLOR_BRUSH, bottomButtonRectangle,
                Form.CORNER_RADIUS);

            // Initialise values for painting triangles
            var quarterWidth = topButtonRectangle.Width / 4;
            var halfWidth = quarterWidth + quarterWidth;

            // Paint the triangles in the buttons
            pe.Graphics.FillPolygon(Form.BACKGROUND_COLOR_BRUSH, new Point[]
                {
                    new Point(topButtonRectangle.Left + halfWidth, topButtonRectangle.Top + quarterWidth),
                    new Point(topButtonRectangle.Left + quarterWidth, topButtonRectangle.Top + halfWidth),
                    new Point(topButtonRectangle.Left + quarterWidth + halfWidth, 
                        topButtonRectangle.Top + halfWidth)
                });
            pe.Graphics.FillPolygon(Form.BACKGROUND_COLOR_BRUSH, new Point[]
                {
                    new Point(bottomButtonRectangle.Left + halfWidth, 
                        bottomButtonRectangle.Bottom - quarterWidth),
                    new Point(bottomButtonRectangle.Left + quarterWidth, 
                        bottomButtonRectangle.Bottom - halfWidth),
                    new Point(bottomButtonRectangle.Left + quarterWidth + halfWidth, 
                        bottomButtonRectangle.Bottom - halfWidth)
                });
        }
    }
}
