/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IO.View.Controls
{
    /// <summary>
    /// Switch widget
    /// </summary>
    public partial class SwitchWidget : DataWidget
    {
        /// <summary>
        /// Static dimentsions
        /// </summary>
        private static readonly int SWITCH_WIDTH = 60;
        private static readonly int SWITCH_HEIGHT = 24;

        /// <summary>
        /// The text rectangle
        /// </summary>
        private Rectangle textRectangle;

        /// <summary>
        /// The switch rectangle
        /// </summary>
        private Rectangle switchRectangle;

        /// <summary>
        /// The on knob rectangle
        /// </summary>
        private Rectangle onRectangle;

        /// <summary>
        /// The off knob rectangle
        /// </summary>
        private Rectangle offRectangle;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SwitchWidget()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Mouse click override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Check for editing or design mode
            if (DesignMode || ParentView.Editing || (Value.HasValue == false))
            {
            }
            // Check for the on knob
            else if (onRectangle.Contains(e.Location))
            {
                if (Value == 0)
                {
                    Main.Instance.SetDeviceValue(DataSourceName, 1);
                }
            }
            // Check for the off knob
            else if (offRectangle.Contains(e.Location))
            {
                if (Value != 0)
                {
                    Main.Instance.SetDeviceValue(DataSourceName, 0);
                }
            }
        }

        /// <summary>
        /// Resize override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            // Check the width and height
            Height = SWITCH_HEIGHT + 1;
            Width = Math.Max(SWITCH_WIDTH + 1, Width);

            base.OnResize(e);

            // Calculate the new rectangle sizes
            textRectangle = new Rectangle(0, 0, Width - (SWITCH_WIDTH + 1), (Height - 1));
            switchRectangle = new Rectangle(textRectangle.Right, 0, SWITCH_WIDTH, textRectangle.Height);
            onRectangle = switchRectangle;
            onRectangle.Inflate(-BEVEL, -BEVEL);
            onRectangle.Width = (SWITCH_WIDTH - DOUBLE_BEVEL) / 2;
            offRectangle = onRectangle;
            offRectangle.Offset(onRectangle.Width, 0);
        }

        /// <summary>
        /// Paint override
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Determine the text to display
            var text = string.IsNullOrEmpty(Text) ? DataSourceName : Text;

            if (string.IsNullOrEmpty(text) && (ParentView != null) && (ParentView.Editing))
            {
                text = "switch";
            }

            // Draw the text
            TextRenderer.DrawText(pe.Graphics, text, Font, textRectangle,
                Color.Black, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            // Calculate the graphic for the switch
            var gp = GraphicsPathForButton(switchRectangle);

            // Draw the switch
            pe.Graphics.DrawPath(Pens.Gray, gp);

            // Calculate the graphic for the knob
            if (Value.HasValue)
            {
                gp = GraphicsPathForButton((Value == 0) ? offRectangle : onRectangle);

                var brush = new LinearGradientBrush(new Point(0, offRectangle.Y),
                    new Point(0, offRectangle.Y + (offRectangle.Height / 2)), Color.Red, Color.Pink);

                // Draw the knob
                pe.Graphics.DrawPath(Pens.DarkRed, gp);
                pe.Graphics.FillPath(brush, gp);
            }

            // Draw the switch text
            TextRenderer.DrawText(pe.Graphics, "On", (Value == 1) ? new Font(Font, FontStyle.Bold) : Font, 
                onRectangle, (Value == 1) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine | 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(pe.Graphics, "Off", (Value == 0) ? new Font(Font, FontStyle.Bold) : Font, 
                offRectangle, (Value == 0) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine | 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            base.OnPaint(pe);
        }
    }
}
