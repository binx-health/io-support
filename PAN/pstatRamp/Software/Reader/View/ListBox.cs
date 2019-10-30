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

namespace IO.View
{
    /// <summary>
    /// List view control
    /// </summary>
    public partial class ListBox : Control
    {
        /// <summary>
        /// The height of each value in the list
        /// </summary>
        private static readonly int VALUE_HEIGHT = 35;

        /// <summary>
        /// The index of the currently selected value
        /// </summary>
        private int valueIndex = 0;

        /// <summary>
        /// The selected value
        /// </summary>
        public new string Text
        {
            get
            {
                // Get the value from the list
                return ((Values != null) && (Values.Count() > valueIndex)) ? Values.ElementAt(valueIndex) : null;
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
                            valueIndex = index;

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
        /// The number of values displayed on each scrollable page
        /// </summary>
        public int ValuesPerPage
        {
            get
            {
                return (Height - (Form.DOUBLE_BORDER_WIDTH + Form.DOUBLE_BORDER_WIDTH)) / VALUE_HEIGHT;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ListBox()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Make transparent and double buffer to avoid flicker
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        /// <summary>
        /// OnClick override to implement scrolling and index selection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            // Get the mouse coordinates
            var mouseEventArgs = (MouseEventArgs)e;

            // Check for a click on the scroll bar
            if (mouseEventArgs.X > Width - (Form.SCROLL_BAR_WIDTH + Form.DOUBLE_BORDER_WIDTH))
            {
                // Get the number of values per page
                int valuesPerPage = ValuesPerPage;

                // Check for a page-up or down
                if (mouseEventArgs.Y < Height / 2)
                {
                    // Decrement the valueIndex
                    valueIndex = Math.Max(0, valueIndex - ValuesPerPage);
                }
                else
                {
                    // Increment the value index
                    valueIndex = Math.Min(Values.Count() - 1, valueIndex + ValuesPerPage);
                }

                // Refresh the control
                Refresh();
            }
            else
            {
                // Set the value index for the selected item
                valueIndex = Math.Max(0, Math.Min(Values.Count() - 1, (mouseEventArgs.Y - Form.DOUBLE_BORDER_WIDTH) / VALUE_HEIGHT));

                // Fire the click event
                base.OnClick(e);
            }
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

            // Fill the rectangle with the background color
            pe.Graphics.FillRoundedRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle, 
                Form.CORNER_RADIUS - Form.BORDER_WIDTH);

            // Make room for the scroll bar
            textRectangle.Width -= Form.SCROLL_BAR_WIDTH + Form.DOUBLE_BORDER_WIDTH;

            // Initialise the dimensions of the scroll bar buttons
            var halfHeight = textRectangle.Height / 2;
            var topButtonRectangle = new Rectangle(textRectangle.Right, textRectangle.Top,
                Form.SCROLL_BAR_WIDTH + Form.DOUBLE_BORDER_WIDTH, halfHeight - Form.BORDER_WIDTH);
            var bottomButtonRectangle = new Rectangle(textRectangle.Right, 
                topButtonRectangle.Bottom + Form.DOUBLE_BORDER_WIDTH,
                Form.SCROLL_BAR_WIDTH + Form.DOUBLE_BORDER_WIDTH, halfHeight - Form.BORDER_WIDTH);

            // Add whitespace roung the buttons
            topButtonRectangle.Inflate(-Form.DOUBLE_BORDER_WIDTH, -Form.DOUBLE_BORDER_WIDTH);
            bottomButtonRectangle.Inflate(-Form.DOUBLE_BORDER_WIDTH, -Form.DOUBLE_BORDER_WIDTH);

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

            // Add whitespace to the text box
            textRectangle.Inflate(-Form.BORDER_WIDTH, -Form.BORDER_WIDTH);

            // Initialise the start index and the offset
            int valuesPerPage = ValuesPerPage;
            int startIndex = Math.Min(Math.Max(0, Values.Count() - valuesPerPage),
                ((valueIndex + 1) / valuesPerPage) * valuesPerPage);
            int offset = 0;

            // Loo through the list box entries drawing them
            for (int index = startIndex; index < Math.Min(Values.Count(), startIndex + valuesPerPage); 
                ++index)
            {
                // Initialise a rectangle for the text
                var subTextRectangle = new Rectangle(textRectangle.Left,
                    textRectangle.Top + offset, textRectangle.Width, VALUE_HEIGHT);

                // Draw the text
                TextRenderer.DrawText(pe.Graphics, Values.ElementAt(index), Font, subTextRectangle, 
                    Form.TEXT_COLOR, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                // Increment the offset
                offset += VALUE_HEIGHT;
            }
        }
    }
}
