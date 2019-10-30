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
    public partial class Key : Control
    {
        /// <summary>
        /// Timer for debouncing
        /// </summary>
        private System.Threading.Timer debounceTimer = null;

        /// <summary>
        /// Down flag indicating the button is pressed
        /// </summary>
        private bool down = false;

        /// <summary>
        /// The toggled state of the button
        /// </summary>
        public bool Toggled { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Key()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Make transparent and double buffer to avoid flicker
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

            // Check for a lift outside of the key
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
        /// Mouse click event override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            // Check for a null debounce timer
            if (debounceTimer == null)
            {
                base.OnClick(e);

                // Start a new timer to debounce the key
                debounceTimer = new System.Threading.Timer(DebounceTimerCallback, null,
                    Form.KEY_DEBOUNCE_TIMER_PERIOD_MILLISECONDS, System.Threading.Timeout.Infinite);
            }
        }

        /// <summary>
        /// Mouse double click event override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            // Call click for the second click
            OnClick(e);
        }

        /// <summary>
        /// Timer callback method
        /// </summary>
        /// <param name="state">The timer state</param>
        private void DebounceTimerCallback(object state)
        {
            // Clear the timer
            debounceTimer = null;
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

            // If we are not toggled then fill the rectangle with the background color
            if ((Toggled == false) && (down == false))
            {
                pe.Graphics.FillRoundedRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle, 
                    Form.CORNER_RADIUS - Form.BORDER_WIDTH);
            }

            // Draw the text in the relevant color
            TextRenderer.DrawText(pe.Graphics, Text, Font, textRectangle, 
                (Toggled || down) ? Form.BACKGROUND_COLOR : Form.TEXT_COLOR,
                TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
