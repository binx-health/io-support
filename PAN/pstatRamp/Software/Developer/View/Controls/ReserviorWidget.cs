/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace IO.View.Controls
{
    /// <summary>
    /// Switch widget
    /// </summary>
    public partial class ReserviorWidget : DataWidget
    {
        /// <summary>
        /// Static dimentsions
        /// </summary>
        private static readonly int SWITCH_WIDTH = 80;
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
        /// The pressure to hold
        /// </summary>
        private int pressure;

        /// <summary>
        /// The minimum pump time
        /// </summary>
        private int tMin;

        /// <summary>
        /// The pressure tool
        /// </summary>
        private ToolStripTextBox pressureTool = null;

        /// <summary>
        /// The minimum pump time tool
        /// </summary>
        private ToolStripTextBox tMinTool = null;

        /// <summary>
        /// Pressure property
        /// </summary>
        public int Pressure
        {
            get
            {
                return pressure;
            }
            set
            {
                pressure = value;
                pressureTool.Text = value.ToString();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Tmin property
        /// </summary>
        public int Tmin
        {
            get
            {
                return tMin;
            }
            set
            {
                tMin = value;
                tMinTool.Text = value.ToString();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ReserviorWidget()
        {
            InitializeComponent();

            // Initialise the pressure tool
            pressureTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            pressureTool.KeyPress += pressureTool_KeyPress;
            pressureTool.Leave += pressureTool_Leave;

            // Initialise the minimum pump time tool
            tMinTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            tMinTool.KeyPress += tMinTool_KeyPress;
            tMinTool.Leave += tMinTool_Leave;

            // Augment the tools list
            tools.Add(new ToolStripLabel("Pressure"));
            tools.Add(pressureTool);
            tools.Add(new ToolStripLabel("Tmin"));
            tools.Add(tMinTool);

            // Initialise the properties
            Pressure = 1000;
            Tmin = 20;
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            Pressure = ParseInt(reader.GetAttribute("Pressure"), 1000);
            Tmin = ParseInt(reader.GetAttribute("Tmin"), 20);
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeString("Pressure", Pressure.ToString());
            writer.WriteAttributeString("Tmin", Tmin.ToString());
        }

        /// <summary>
        /// Mouse click override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Check for editing or design mode
            if (DesignMode || ParentView.Editing)
            {
            }
            // Check for the on knob
            else if (onRectangle.Contains(e.Location))
            {
                // Form the reservior command
                var command = "res " + DataSourceName + " hold " + Pressure.ToString() + " " + 
                    (Pressure * 2).ToString() + " " + Tmin.ToString();

                // Issue the command
                MessageQueue.Instance.Push(new MenuItemMessage("Command")
                {
                    Parameters = new Dictionary<string, object>() { { "Value", command } }
                });
            }
            // Check for the off knob
            else if (offRectangle.Contains(e.Location))
            {
                // Form the reservior command
                var command = "res " + DataSourceName + " dump";

                // Issue the command
                MessageQueue.Instance.Push(new MenuItemMessage("Command")
                {
                    Parameters = new Dictionary<string, object>() { { "Value", command } }
                });
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
                text = "stepper";
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
            if (Value.HasValue && (Value != 0))
            {
                gp = GraphicsPathForButton((Value == 1) ? offRectangle : onRectangle);

                var brush = new LinearGradientBrush(new Point(0, offRectangle.Y),
                    new Point(0, offRectangle.Y + (offRectangle.Height / 2)), Color.Red, Color.Pink);

                // Draw the knob
                pe.Graphics.DrawPath(Pens.DarkRed, gp);
                pe.Graphics.FillPath(brush, gp);
            }

            // Draw the switch text
            TextRenderer.DrawText(pe.Graphics, "Hold", (Value == 2) ? new Font(Font, FontStyle.Bold) : Font,
                onRectangle, (Value == 2) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine |
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(pe.Graphics, "Dump", (Value == 1) ? new Font(Font, FontStyle.Bold) : Font,
                offRectangle, (Value == 1) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine |
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            base.OnPaint(pe);
        }

        /// <summary>
        /// Key press event handler for the pressure tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pressureTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                pressureTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the pressure tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pressureTool_Leave(object sender, EventArgs e)
        {
            int value;

            if (int.TryParse(pressureTool.Text, out value))
            {
                if (Pressure != value)
                {
                    // Set the value, modified flag and refresh
                    Pressure = value;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Pressure = Pressure;
            }
        }

        /// <summary>
        /// Key press event handler for the minimum pump time tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tMinTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                tMinTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the tMin tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tMinTool_Leave(object sender, EventArgs e)
        {
            int value;

            if (int.TryParse(tMinTool.Text, out value))
            {
                if (Tmin != value)
                {
                    // Set the value, modified flag and refresh
                    Tmin = value;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Tmin = Tmin;
            }
        }
    }
}
