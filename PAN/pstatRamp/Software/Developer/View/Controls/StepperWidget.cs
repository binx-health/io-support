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
    public partial class StepperWidget : DataWidget
    {
        /// <summary>
        /// Static dimentsions
        /// </summary>
        private static readonly int BUTTON_WIDTH = 60;
        private static readonly int BUTTON_HEIGHT = 24;

        /// <summary>
        /// The text rectangle
        /// </summary>
        private Rectangle textRectangle;

        /// <summary>
        /// The up button rectangle
        /// </summary>
        private Rectangle upButtonRectangle;

        /// <summary>
        /// The down button rectangle
        /// </summary>
        private Rectangle downButtonRectangle;

        /// <summary>
        /// The speed to move at
        /// </summary>
        private int speed;

        /// <summary>
        /// The steps to move
        /// </summary>
        private int steps;

        /// <summary>
        /// The speed tool
        /// </summary>
        private ToolStripTextBox speedTool = null;

        /// <summary>
        /// The steps tool
        /// </summary>
        private ToolStripTextBox stepsTool = null;

        /// <summary>
        /// Speed property
        /// </summary>
        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
                speedTool.Text = value.ToString();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Steps property
        /// </summary>
        public int Steps
        {
            get
            {
                return steps;
            }
            set
            {
                steps = value;
                stepsTool.Text = value.ToString();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public StepperWidget()
        {
            InitializeComponent();

            // Initialise the speed tool
            speedTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            speedTool.KeyPress += speedTool_KeyPress;
            speedTool.Leave += speedTool_Leave;

            // Initialise the steps tool
            stepsTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            stepsTool.KeyPress += stepsTool_KeyPress;
            stepsTool.Leave += stepsTool_Leave;

            // Augment the tools list
            tools.Add(new ToolStripLabel("Speed"));
            tools.Add(speedTool);
            tools.Add(new ToolStripLabel("Steps"));
            tools.Add(stepsTool);

            // Initialise the properties
            Speed = 100;
            Steps = 100;
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            Speed = ParseInt(reader.GetAttribute("Speed"), 100);
            Steps = ParseInt(reader.GetAttribute("Steps"), 100);
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeString("Speed", Speed.ToString());
            writer.WriteAttributeString("Steps", Steps.ToString());
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
            else if (upButtonRectangle.Contains(e.Location))
            {
                // Set the value to 1 to debounce the button
                Value = 1;

                // Form the stepper command
                var command = "stepper " + DataSourceName + " " + Speed.ToString() + " " + (-Steps).ToString();

                // Issue the command
                MessageQueue.Instance.Push(new MenuItemMessage("Command")
                {
                    Parameters = new Dictionary<string, object>() { { "Value", command } }
                });
            }
            // Check for the off knob
            else if (downButtonRectangle.Contains(e.Location))
            {
                // Set the value to 1 to debounce the button
                Value = 1;

                // Form the stepper command
                var command = "stepper " + DataSourceName + " " + Speed.ToString() + " " + Steps.ToString();

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
            Height = BUTTON_HEIGHT + BUTTON_HEIGHT + 1;
            Width = Math.Max(BUTTON_WIDTH + 1, Width);

            base.OnResize(e);

            // Calculate the new rectangle sizes
            textRectangle = new Rectangle(0, 0, Width - (BUTTON_WIDTH + 1), (Height - 1));
            upButtonRectangle = new Rectangle(textRectangle.Right, 0, BUTTON_WIDTH, BUTTON_HEIGHT);
            downButtonRectangle = new Rectangle(textRectangle.Right, BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT);
            upButtonRectangle.Inflate(-BEVEL, -BEVEL);
            downButtonRectangle.Inflate(-BEVEL, -BEVEL);
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

            // Calculate the graphic for the up button
            var gp = GraphicsPathForButton(upButtonRectangle);
            var brush = new LinearGradientBrush(new Point(0, upButtonRectangle.Y),
                new Point(0, upButtonRectangle.Y + (upButtonRectangle.Height / 2)),
                (Value == 0) ? Color.Gray : Color.LightGray, (Value == 0) ? Color.LightGray : Color.White);

            // Draw the up button
            pe.Graphics.DrawPath(Pens.DarkGray, gp);
            pe.Graphics.FillPath(brush, gp);

            // Calculate the graphic for the down button
            gp = GraphicsPathForButton(downButtonRectangle);
            brush = new LinearGradientBrush(new Point(0, downButtonRectangle.Y),
                new Point(0, downButtonRectangle.Y + (downButtonRectangle.Height / 2)),
                (Value == 0) ? Color.Gray : Color.LightGray, (Value == 0) ? Color.LightGray : Color.White);

            // Draw the up button
            pe.Graphics.DrawPath(Pens.DarkGray, gp);
            pe.Graphics.FillPath(brush, gp);

            // Draw the switch text
            TextRenderer.DrawText(pe.Graphics, "Up", (Value == 0) ? new Font(Font, FontStyle.Bold) : Font, 
                upButtonRectangle, (Value == 0) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine | 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(pe.Graphics, "Down", (Value == 0) ? new Font(Font, FontStyle.Bold) : Font, 
                downButtonRectangle, (Value == 0) ? Color.Black : Color.Gray, TextFormatFlags.SingleLine | 
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            base.OnPaint(pe);
        }

        /// <summary>
        /// Key press event handler for the speed tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void speedTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                speedTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the speed tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void speedTool_Leave(object sender, EventArgs e)
        {
            int value;

            if (int.TryParse(speedTool.Text, out value))
            {
                if (Speed != value)
                {
                    // Set the value, modified flag and refresh
                    Speed = value;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Speed = Speed;
            }
        }

        /// <summary>
        /// Key press event handler for the steps tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stepsTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                stepsTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the steps tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stepsTool_Leave(object sender, EventArgs e)
        {
            int value;

            if (int.TryParse(stepsTool.Text, out value))
            {
                if (Steps != value)
                {
                    // Set the value, modified flag and refresh
                    Steps = value;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Steps = Steps;
            }
        }
    }
}
