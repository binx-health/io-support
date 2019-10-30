/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IO.View
{
    /// <summary>
    /// Button control
    /// </summary>
    public partial class Table : Control
    {
        /// <summary>
        /// The table row height constant
        /// </summary>
        private static readonly int VALUE_HEIGHT = 40;

        /// <summary>
        /// Column widths array whose size is the number of columns minus one
        /// </summary>
        public int[] ColumnWidths { get; set; }

        /// <summary>
        /// Array of values by row then column
        /// </summary>
        public List<object[]> Values { get; set; }

        /// <summary>
        /// The index of the top row for scrolling
        /// </summary>
        public int TopRow { get; set; }

        /// <summary>
        /// The selected row
        /// </summary>
        public int SelectedRow { get; set; }

        /// <summary>
        /// The number of values per page
        /// </summary>
        public int ValuesPerPage
        {
            get
            {
                return ((Height - Form.BORDER_WIDTH) + (VALUE_HEIGHT - 1)) / VALUE_HEIGHT;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Table()
        {
            // Add the transparent background style
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Make transparent and double buffer to avoid flicker
            DoubleBuffered = true;
            BackColor = Color.Transparent;

            // Initialise the top row to zero
            TopRow = 0;
            SelectedRow = 0;
        }

        /// <summary>
        /// OnClick override to implement scrolling and index selection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            // Get the mouse coordinates
            var mouseEventArgs = (MouseEventArgs)e;

            // Set the value index for the selected item
            SelectedRow = TopRow + Math.Max(0, (mouseEventArgs.Y - Form.BORDER_WIDTH) / VALUE_HEIGHT);

            // Refresh the control
            Refresh();

            // Fire the click event
            base.OnClick(e);
        }

        /// <summary>
        /// Paint event handler
        /// </summary>
        /// <param name="pe">Paint event arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Call the base class first
            base.OnPaint(pe);

            // Fill the control with the background colour
            pe.Graphics.FillRectangle(Form.MAIN_COLOR_BRUSH, new Rectangle(0, 0, Width, Height));

            // Initialise the number of columns
            int columns = (ColumnWidths == null) ? 0 : ColumnWidths.Length + 1;

            // Initialise the first row
            int row = TopRow;

            // Loop through the visible rows
            for (int top = Form.BORDER_WIDTH; top < Height; top += VALUE_HEIGHT, ++row)
            {
                // Initialise the column marker
                int left = Form.BORDER_WIDTH;

                // Loop through the columns
                for (int column = 0; column < columns; ++column)
                {
                    // Calculate the width of the column
                    int width = (column < ColumnWidths.Length) ? ColumnWidths[column] : Width - left;

                    // Initialse the rectangle for the text
                    var textRectangle = new Rectangle(left, top, width - Form.BORDER_WIDTH, VALUE_HEIGHT - Form.BORDER_WIDTH);

                    // Check to see if the cell has a value
                    if ((Values != null) && (row < Values.Count) && (column < Values[row].Length))
                    {
                        // If this is not a selected row then fill the box with the background colour
                        if (row != SelectedRow)
                        {
                            pe.Graphics.FillRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle);
                        }

                        // Check for the first column
                        if (column == 0)
                        {
                            // Initialise values for painting a triangle
                            var eigthHeight = (VALUE_HEIGHT + Form.BORDER_WIDTH) / 8;
                            var quarterHeight = eigthHeight + eigthHeight;
                            var halfHeight = quarterHeight + quarterHeight;

                            // Paint the triangle
                            pe.Graphics.FillPolygon(Form.MAIN_COLOR_BRUSH, 
                                new Point[]
                                {
                                    new Point(halfHeight + eigthHeight, (top - Form.BORDER_WIDTH) + 
                                        halfHeight),
                                    new Point(halfHeight - eigthHeight, (top - Form.BORDER_WIDTH) + 
                                        halfHeight - quarterHeight),
                                    new Point(halfHeight - eigthHeight, (top - Form.BORDER_WIDTH) + 
                                        halfHeight + quarterHeight)
                                });

                            // Offset the text by the width of the triangle
                            textRectangle.X += VALUE_HEIGHT;
                            textRectangle.Width -= VALUE_HEIGHT;
                        }

                        // Check for a null value
                        if (Values[row][column] != null)
                        {
                            // Convert the value into a string
                            var value = Values[row][column] is DateTime ?
                                ((DateTime)Values[row][column]).ToLocalTime().ToString("d MMM yyyy HH:mm") :
                                Values[row][column].ToString();

                            // Draw the text
                            TextRenderer.DrawText(pe.Graphics, value, Font, textRectangle,
                                (row == SelectedRow) ? Form.BACKGROUND_COLOR : Form.TEXT_COLOR,
                                TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                                ((column == 0) ? TextFormatFlags.Left : TextFormatFlags.HorizontalCenter) |
                                TextFormatFlags.EndEllipsis);
                        }
                    }
                    else
                    {
                        // Just fill the box with the background colour
                        pe.Graphics.FillRectangle(Form.BACKGROUND_COLOR_BRUSH, textRectangle);
                    }

                    // Offset the column marker
                    left += width;
                }
            }
        }
    }
}
