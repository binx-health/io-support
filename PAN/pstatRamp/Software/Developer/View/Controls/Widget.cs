/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace IO.View.Controls
{
    /// <summary>
    /// Widget control base class
    /// </summary>
    public partial class Widget : Control, IXmlSerializable
    {
        /// <summary>
        /// Static dimensions
        /// </summary>
        private const int RESIZE = 16;
        protected const int HALF_BEVEL = 1;
        protected const int BEVEL = 2;
        protected const int DOUBLE_BEVEL = 4;

        /// <summary>
        /// The resize rectangle
        /// </summary>
        private Region resizeRegion;

        /// <summary>
        /// The pen used to draw the border in editing mode
        /// </summary>
        private Pen dashPen = null;

        /// <summary>
        /// The brush used to draw the resize tag
        /// </summary>
        private Brush hatchBrush = null;

        /// <summary>
        /// The text tool
        /// </summary>
        private ToolStripTextBox textTool = null;

        /// <summary>
        /// The widget tools
        /// </summary>
        protected List<ToolStripItem> tools = new List<ToolStripItem>();

        /// <summary>
        /// Default width and height
        /// </summary>
        protected readonly int DEFAULT_WIDTH = 200;
        protected readonly int DEFAULT_HEIGHT = 25;

        /// <summary>
        /// Parent view accessor which will be null in design mode
        /// </summary>
        public View ParentView
        {
            get
            {
                return base.Parent as View;
            }
        }

        /// <summary>
        /// The widget tools
        /// </summary>
        public virtual IEnumerable<ToolStripItem> Tools
        {
            get
            {
                return tools;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Widget()
        {
            InitializeComponent();

            // Initialise state
            DoubleBuffered = true;
            AllowDrop = true;

            // Initialise the text tool
            textTool = new ToolStripTextBox();
            textTool.KeyPress += textTool_KeyPress;
            textTool.Leave += textTool_Leave;

            // Augment the tools list
            tools.Add(new ToolStripLabel("Text"));
            tools.Add(textTool);

            // Initialise the pen for drawing the active rectangle
            dashPen = new Pen(Color.Red);
            dashPen.DashCap = DashCap.Round;
            dashPen.DashPattern = new float[] { 2f, 2f };

            // Initialise the brush for drawing the grabber
            hatchBrush = new HatchBrush(HatchStyle.DarkUpwardDiagonal, Color.Red,
                Color.Transparent);

            // Set the initial dimensions
            Width = DEFAULT_WIDTH;
            Height = DEFAULT_HEIGHT;
        }

        /// <summary>
        /// Get schema laways returns null
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public virtual void ReadXml(XmlReader reader)
        {
            SetBounds(ParseInt(reader.GetAttribute("Bounds.X"), 0),
                ParseInt(reader.GetAttribute("Bounds.Y"), 0),
                ParseInt(reader.GetAttribute("Bounds.Width"), DEFAULT_WIDTH),
                ParseInt(reader.GetAttribute("Bounds.Height"), DEFAULT_HEIGHT));
            Text = reader.GetAttribute("Text");
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Bounds.X", Bounds.X.ToString());
            writer.WriteAttributeString("Bounds.Y", Bounds.Y.ToString());
            writer.WriteAttributeString("Bounds.Width", Bounds.Width.ToString());
            writer.WriteAttributeString("Bounds.Height", Bounds.Height.ToString());
            writer.WriteAttributeString("Text", Text);
        }

        /// <summary>
        /// Generate a graphics path for a button
        /// </summary>
        /// <param name="rectangle">The button rectangle</param>
        /// <returns>The graphics path</returns>
        protected static GraphicsPath GraphicsPathForButton(Rectangle rectangle, int bevel = HALF_BEVEL)
        {
            var gp = new GraphicsPath();

            gp.AddLine(rectangle.X + bevel, rectangle.Y, rectangle.X + rectangle.Width - bevel, 
                rectangle.Y);
            gp.AddLine(rectangle.X + rectangle.Width, rectangle.Y + bevel,
                rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height - bevel);
            gp.AddLine(rectangle.X + rectangle.Width - bevel, rectangle.Y + rectangle.Height,
                rectangle.X + bevel, rectangle.Y + rectangle.Height);
            gp.AddLine(rectangle.X, rectangle.Y + rectangle.Height - bevel, rectangle.X,
                rectangle.Y + bevel);
            gp.CloseFigure();

            return gp;
        }

        /// <summary>
        /// Parse an int value from a string with a default
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="defaultValue">The default</param>
        /// <returns>The parsed string value or the default</returns>
        protected static int ParseInt(string value, int defaultValue)
        {
            int result;

            if (int.TryParse(value, out result) == false)
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Parse an optional int value from a string with a default
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>The parsed string value or the default</returns>
        protected static int? ParseOptionalInt(string value)
        {
            int result;

            if (int.TryParse(value, out result) == false)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Parse a double value from a string with a default
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="defaultValue">The default</param>
        /// <returns>The parsed string value or the default</returns>
        protected static double ParseDouble(string value, double defaultValue)
        {
            double result;

            if (double.TryParse(value, out result) == false)
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Text changed override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            textTool.Text = Text;
        }

        /// <summary>
        /// Resize override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Calculate the resize region
            var gp = new GraphicsPath();

            gp.AddLine(Width, Height, Width, 0);

            if ((Height > RESIZE) && (Width > RESIZE))
            {
                gp.AddPolygon(new Point[] { new Point(Width, Height), new Point(Width, Height - RESIZE), 
                    new Point(Width - RESIZE, Height) });
            }
            else if (Width >= Height)
            {
                gp.AddPolygon(new Point[] { new Point(Width, Height), new Point(Width, 0), 
                    new Point(Width - Height, Height) });
            }
            else
            {
                gp.AddPolygon(new Point[] { new Point(Width, Height), new Point(Width, Height - Width), 
                    new Point(0, Height) });
            }

            resizeRegion = new Region(gp);
        }

        /// <summary>
        /// Paint override
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            // If we are editing and the active control then draw the border
            if ((ParentView != null) && ParentView.Editing && (ParentView.ActiveControl == this))
            {
                pe.Graphics.DrawRectangle(dashPen, new Rectangle(0, 0, Width - 1, Height - 1));
                pe.Graphics.FillRegion(hatchBrush, resizeRegion);
            }
        }

        /// <summary>
        /// Enter override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            // If we have a parent view then inform it
            if (ParentView != null)
            {
                ParentView.WidgetEntered(this);
            }
        }

        /// <summary>
        /// Leave override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            // If we have a parent view then inform it
            if (ParentView != null)
            {
                ParentView.WidgetLeft(this);
            }
        }

        /// <summary>
        /// Mouse down override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Select this widget
            Select();
        }

        /// <summary>
        /// Mouse move event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // If we have capture and are in editing mode then commence a move
            if (Capture && (ParentView != null) && ParentView.Editing)
            {
                // Clear capture
                Capture = false;

                // Get the mouse position in parent client co-ordinates
                var parentPosition = e.Location;

                parentPosition.Offset(Bounds.X, Bounds.Y);

                if (resizeRegion.IsVisible(e.Location))
                {
                    // Start the resize on the parent view
                    ParentView.ResizeWidget(this, parentPosition);
                }
                else
                {
                    // Start the move on the parent view
                    ParentView.MoveWidget(this, parentPosition);
                }
            }
        }

        /// <summary>
        /// Key up override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // Check for the delete key
            if (e.KeyCode == Keys.Delete)
            {
                // if we are editing then delete the current widget
                if ((ParentView != null) && ParentView.Editing)
                {
                    ParentView.DeleteCurrentWidget();
                }
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// Key press event handler for the text tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                textTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the text tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textTool_Leave(object sender, EventArgs e)
        {
            if (Text != textTool.Text)
            {
                // Set the text, modified flag and refresh
                Text = textTool.Text;

                if (ParentView != null)
                {
                    ParentView.Modified = true;
                }

                Refresh();
            }
        }
    }
}
