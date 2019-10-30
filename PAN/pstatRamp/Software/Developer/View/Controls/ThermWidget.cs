/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IO.View.Controls
{
    /// <summary>
    /// Peltier widget
    /// </summary>
    public partial class ThermWidget : DataWidget
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
        /// The demand temperature to move to
        /// </summary>
        private int temperature;

        /// <summary>
        /// The temperature tool
        /// </summary>
        private ToolStripTextBox temperatureTool = null;

        /// <summary>
        /// Temperature property
        /// </summary>
        public int Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                temperature = value;
                temperatureTool.Text = value.ToString();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ThermWidget()
        {
            InitializeComponent();

            // Initialise the temperature tool
            temperatureTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            temperatureTool.KeyPress += temperatureTool_KeyPress;
            temperatureTool.Leave += temperatureTool_Leave;

            // Augment the tools list
            tools.Add(new ToolStripLabel("Temperature"));
            tools.Add(temperatureTool);

            // Initialise the properties
            Temperature = 25;
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            Temperature = ParseInt(reader.GetAttribute("Temperature"), 25);
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeString("Temperature", Temperature.ToString());
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
                if (Value != Temperature)
                {
                    // Set the value to 1 to debounce the switch
                    Value = Temperature;

                    // Form the therm command
                    var command = "therm " + DataSourceName + " on " + Temperature.ToString() + " ack";

                    // Issue the command
                    MessageQueue.Instance.Push(new MenuItemMessage("Command")
                    {
                        Parameters = new Dictionary<string, object>() { { "Value", command } }
                    });
                }
            }
            // Check for the off knob
            else if (offRectangle.Contains(e.Location))
            {
                if (Value == Temperature)
                {
                    // Set the value to 0 to debounce the switch
                    Value = 0;

                    // Form the therm command
                    var command = "therm " + DataSourceName + " off";

                    // Issue the command
                    MessageQueue.Instance.Push(new MenuItemMessage("Command")
                    {
                        Parameters = new Dictionary<string, object>() { { "Value", command } }
                    });
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
                gp = GraphicsPathForButton((Value != Temperature) ? offRectangle : onRectangle);

                var brush = new LinearGradientBrush(new Point(0, offRectangle.Y),
                    new Point(0, offRectangle.Y + (offRectangle.Height / 2)), Color.Red, Color.Pink);

                // Draw the knob
                pe.Graphics.DrawPath(Pens.DarkRed, gp);
                pe.Graphics.FillPath(brush, gp);
            }

            // Draw the switch text
            TextRenderer.DrawText(pe.Graphics, "On", (Value == Temperature) ? 
                new Font(Font, FontStyle.Bold) : Font,
                onRectangle, (Value == Temperature) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine | 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(pe.Graphics, "Off", (Value != Temperature) ? 
                new Font(Font, FontStyle.Bold) : Font,
                offRectangle, (Value != Temperature) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine | 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            base.OnPaint(pe);
        }

        /// <summary>
        /// Key press event handler for the temperature tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void temperatureTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                temperatureTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the temperature tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void temperatureTool_Leave(object sender, EventArgs e)
        {
            int value;

            if (int.TryParse(temperatureTool.Text, out value))
            {
                if (Temperature != value)
                {
                    // Set the value, modified flag and refresh
                    Temperature = value;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Temperature = Temperature;
            }
        }
    }
}
