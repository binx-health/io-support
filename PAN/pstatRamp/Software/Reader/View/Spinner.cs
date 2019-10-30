/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace IO.View
{
    /// <summary>
    /// Spinner control
    /// </summary>
    public partial class Spinner : Control
    {
        /// <summary>
        /// Half the text height
        /// </summary>
        private static readonly int HALF_TEXT_HEIGHT = 26;

        /// <summary>
        /// The minimum period for scrolling
        /// </summary>
        private static readonly int MINIMUM_PERIOD = 100;

        /// <summary>
        /// The minimum period for scrolling
        /// </summary>
        private static readonly int MAXIMUM_PERIOD = 1000;

        /// <summary>
        /// Flag indicating direction
        /// </summary>
        private bool up = false;

        /// <summary>
        /// The index of the currently selected value
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The selected value
        /// </summary>
        public new string Text
        {
            get
            {
                // Get the value from the list
                return ((Values != null) && (Values.Count() > Index)) ? Values.ElementAt(Index) : null;
            }
            set
            {
                // Check for values
                if (Values != null)
                {
                    // Loop through the list looking for the value
                    int index = 0;

                    foreach (var val in Values)
                    {
                        if (val == value)
                        {
                            // Set the new index
                            Index = index;

                            // Repaint the control
                            Refresh();

                            break;
                        }

                        ++index;
                    }
                }
            }
        }

        /// <summary>
        /// The list of values
        /// </summary>
        public IEnumerable<string> Values { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Spinner()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Intialise the index to zero
            Index = 0;

            // Make transparent and double buffer to avoid flicker
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        /// <summary>
        /// OnMouseDown override to implement scrolling and index selection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Call the base class
            base.OnMouseDown(e);

            // Set the direction
            up = e.Y < Height / 2;

            // Call the tick event
            timer_Tick(this, e);

            // Enable the timer
            timer.Interval = MAXIMUM_PERIOD;
            timer.Enabled = true;
        }

        /// <summary>
        /// OnMouseDown override to implement scrolling and index selection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Call the base class
            base.OnMouseUp(e);

            // Disable the timer
            timer.Enabled = false;
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
            var textRectangle = new Rectangle(Form.BORDER_WIDTH, Form.BORDER_WIDTH,
                Width - Form.DOUBLE_BORDER_WIDTH, Height - Form.DOUBLE_BORDER_WIDTH);
            var halfHeight = (textRectangle.Top + textRectangle.Bottom) / 2;

            textRectangle.Y = halfHeight - HALF_TEXT_HEIGHT;
            textRectangle.Height = HALF_TEXT_HEIGHT + HALF_TEXT_HEIGHT;

            // Draw the text background in the relevant color
            pe.Graphics.FillRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle);

            // Draw the text in the relevant color
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle,
                ForeColor, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            var plusRectangle = new Rectangle(Form.BORDER_WIDTH, Form.BORDER_WIDTH,
                Width - Form.DOUBLE_BORDER_WIDTH,
                halfHeight - (HALF_TEXT_HEIGHT + Form.DOUBLE_BORDER_WIDTH));
            var minusRectangle = new Rectangle(Form.BORDER_WIDTH, halfHeight + HALF_TEXT_HEIGHT + Form.BORDER_WIDTH,
                Width - Form.DOUBLE_BORDER_WIDTH,
                halfHeight - (HALF_TEXT_HEIGHT + Form.DOUBLE_BORDER_WIDTH));
            var font = new Font(Font.Name, halfHeight / 2);

            // Draw the plus and minus in the relevant color
            TextRenderer.DrawText(pe.Graphics, "+", font, plusRectangle,
                Form.BACKGROUND_COLOR, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
            TextRenderer.DrawText(pe.Graphics, "−", font, minusRectangle,
                Form.BACKGROUND_COLOR, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
        }

        /// <summary>
        /// Ticke event handler for the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Remember the previous value
            int previousIndex = Index;

            // Check for a page-up or down
            if (up)
            {
                // Increment the value index
                Index = Math.Min(Values.Count() - 1, Index + 1);
            }
            else
            {
                // Decrement the valueIndex
                Index = Math.Max(0, Index - 1);
            }

            // Refresh the control
            Refresh();

            // If the value changed then fire the click event
            if (previousIndex != Index)
            {
                base.OnClick(e);
            }

            // Decrease the period down to the minimum
            timer.Interval = Math.Max(timer.Interval / 2, MINIMUM_PERIOD);
        }
    }
}
