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
    /// Switch widget
    /// </summary>
    public partial class ValueWidget : DataWidget
    {
        /// <summary>
        /// Static dimentsions
        /// </summary>
        private static readonly int VALUE_WIDTH = 100;
        private static readonly int VALUE_HEIGHT = 24;

        /// <summary>
        /// The text rectangle
        /// </summary>
        private Rectangle textRectangle;

        /// <summary>
        /// The switch rectangle
        /// </summary>
        private Rectangle valueRectangle;

        /// <summary>
        /// The factor for the displayed value
        /// </summary>
        private double factor;

        /// <summary>
        /// The offset for the displayed value
        /// </summary>
        private double offset;

        /// <summary>
        /// The factor tool
        /// </summary>
        private ToolStripTextBox factorTool = null;

        /// <summary>
        /// The offset tool
        /// </summary>
        private ToolStripTextBox offsetTool = null;

        /// <summary>
        /// Factor property
        /// </summary>
        public double Factor
        {
            get
            {
                return factor;
            }
            set
            {
                factor = value;
                factorTool.Text = value.ToString();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Factor property
        /// </summary>
        public double Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                offsetTool.Text = value.ToString();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ValueWidget()
        {
            InitializeComponent();

            // Initialise the factor tool
            factorTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            factorTool.KeyPress += factorTool_KeyPress;
            factorTool.Leave += factorTool_Leave;

            // Initialise the offset tool
            offsetTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            offsetTool.KeyPress += offsetTool_KeyPress;
            offsetTool.Leave += offsetTool_Leave;

            // Augment the tools list
            tools.Add(new ToolStripLabel("Factor"));
            tools.Add(factorTool);
            tools.Add(new ToolStripLabel("Offset"));
            tools.Add(offsetTool);

            // Initialise the properties
            Factor = 1.0;
            Offset = 0.0;
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            Factor = ParseDouble(reader.GetAttribute("Factor"), 1.0);
            Offset = ParseDouble(reader.GetAttribute("Offset"), 0.0);
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeString("Factor", Factor.ToString());
            writer.WriteAttributeString("Offset", Offset.ToString());
        }

        /// <summary>
        /// Resize override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            // Check the width and height
            Height = VALUE_HEIGHT + 1;
            Width = Math.Max(VALUE_WIDTH + 1, Width);

            base.OnResize(e);

            // Calculate the new rectangle sizes
            textRectangle = new Rectangle(0, 0, Width - (VALUE_WIDTH + 1), (Height - 1));
            valueRectangle = new Rectangle(textRectangle.Right, 0, VALUE_WIDTH, textRectangle.Height);
            valueRectangle.Inflate(0, -BEVEL);
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
                text = "value";
            }

            // Draw the text
            TextRenderer.DrawText(pe.Graphics, text, Font, textRectangle,
                Color.Black, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter |
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            // Draw the value rectangle
            pe.Graphics.DrawRectangle(Pens.Gray, valueRectangle);

            if (Value.HasValue)
            {
                // Draw the value text
                TextRenderer.DrawText(pe.Graphics, ((Value.Value - offset) * factor).ToString("F1"),
                    Font, valueRectangle, Color.Black, TextFormatFlags.SingleLine |
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }

            base.OnPaint(pe);
        }

        /// <summary>
        /// Key press event handler for the factor tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void factorTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                factorTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the factor tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void factorTool_Leave(object sender, EventArgs e)
        {
            double value;

            if (double.TryParse(factorTool.Text, out value))
            {
                if (Factor != value)
                {
                    // Set the value, modified flag and refresh
                    Factor = value;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Factor = Factor;
            }
        }

        /// <summary>
        /// Key press event handler for the offset tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void offsetTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                offsetTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the offset tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void offsetTool_Leave(object sender, EventArgs e)
        {
            double value;

            if (double.TryParse(offsetTool.Text, out value))
            {
                if (Offset != value)
                {
                    // Set the value, modified flag and refresh
                    Offset = value;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Offset = Offset;
            }
        }
    }
}
