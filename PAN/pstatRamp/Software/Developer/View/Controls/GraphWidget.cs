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
using IO.Model.Volatile;

namespace IO.View.Controls
{
    /// <summary>
    /// Switch widget
    /// </summary>
    public partial class GraphWidget : DataWidget
    {
        /// <summary>
        /// Static dimentsions
        /// </summary>
        private static readonly int GRAPH_WIDTH = 600;
        private static readonly int GRAPH_HEIGHT = 400;
        private static readonly int Y_AXIS_WIDTH = 60;
        private static readonly int X_AXIS_HEIGHT = 30;
        private static readonly int TICK_LENGTH = 10;

        /// <summary>
        /// Live data recording constants
        /// </summary>
        private static readonly int LIVE_DATA_PERIOD = 100;
        private static readonly int LIVE_DATA_DEFAULT_WINDOW = 600;

        /// <summary>
        /// The pens for plotting the data
        /// </summary>
        private static readonly Pen[] pens = new Pen[] { Pens.Red, Pens.Green, Pens.Blue, Pens.Black };

        /// <summary>
        /// The minimum value
        /// </summary>
        private double min = double.MaxValue;

        /// <summary>
        /// The maximum value
        /// </summary>
        private double max = double.MinValue;

        /// <summary>
        /// The number of values
        /// </summary>
        private int values = 0;

        /// <summary>
        /// The tick count at the start of a live data feed
        /// </summary>
        private int startTicks = 0;

        /// <summary>
        /// The X axis ticks
        /// </summary>
        private IEnumerable<double> xTicks = new List<double>();

        /// <summary>
        /// The Y axis ticks
        /// </summary>
        private IEnumerable<double> yTicks = new List<double>();

        /// <summary>
        /// The X axis rectangle
        /// </summary>
        private Rectangle xAxisRectangle;

        /// <summary>
        /// The Y axis rectangle
        /// </summary>
        private Rectangle yAxisRectangle;

        /// <summary>
        /// The graph rectangle
        /// </summary>
        private Rectangle graphRectangle;

        /// <summary>
        /// List of data values
        /// </summary>
        private Queue<double>[] data = new Queue<double>[4];

        /// <summary>
        /// Start value
        /// </summary>
        private double start = 0.0;

        /// <summary>
        /// Incremental value
        /// </summary>
        private double increment = 1.0;

        /// <summary>
        /// The factor for the displayed value
        /// </summary>
        private double factor;

        /// <summary>
        /// The offset for the displayed value
        /// </summary>
        private double offset;

        /// <summary>
        /// The offset for the displayed value
        /// </summary>
        private int window;

        /// <summary>
        /// The factor tool
        /// </summary>
        private ToolStripTextBox factorTool = null;

        /// <summary>
        /// The offset tool
        /// </summary>
        private ToolStripTextBox offsetTool = null;

        /// <summary>
        /// The offset tool
        /// </summary>
        private ToolStripTextBox windowTool = null;

        /// <summary>
        /// Start property
        /// </summary>
        public double Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;

                // Calculate the X ticks
                xTicks = Ticks(start, start + (increment * (values - 1)));
                
                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Increment property
        /// </summary>
        public double Increment
        {
            get
            {
                return increment;
            }
            set
            {
                increment = value;

                // Calculate the X ticks
                xTicks = Ticks(start, start + (increment * (values - 1)));

                // Refresh the widget
                Refresh();
            }
        }

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
                if (factor != value)
                {
                    factor = value;

                    if (factorTool != null)
                    {
                        factorTool.Text = value.ToString();
                    }

                    // Clear the data
                    ClearData();

                    // Refresh the widget
                    Refresh();
                }
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

                if (offsetTool != null)
                {
                    offsetTool.Text = value.ToString();
                }

                // Clear the data
                ClearData();

                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Window property
        /// </summary>
        public int Window
        {
            get
            {
                return window;
            }
            set
            {
                if (window != value)
                {
                    window = value;

                    if (windowTool != null)
                    {
                        windowTool.Text = ((value * LIVE_DATA_PERIOD) / 1000).ToString();
                    }

                    // Clear the data
                    ClearData();

                    // Refresh the widget
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Data source name accessor
        /// </summary>
        public override string DataSourceName
        {
            get
            {
                return base.DataSourceName;
            }
            set
            {
                if (value != base.DataSourceName)
                {
                    // Clear the data
                    ClearData();

                    base.DataSourceName = value;
                }
            }
        }

        /// <summary>
        /// The virtual control value
        /// </summary>
        public override int? Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;

                // Check for a value
                if (value.HasValue)
                {
                    // Get the current tick count
                    int ticks;

                    if (IDeviceValues.Instance.TryGetValue("ticks", out ticks))
                    {
                        // Check for the first data value
                        if (data[0] == null)
                        {
                            data[0] = new Queue<double>();

                            // Initialise the ticks and increment
                            startTicks = ticks;
                            Start = startTicks / 1000.0;
                            Increment = LIVE_DATA_PERIOD / 1000.0;
                        }

                        // Get the index of the new value, based on the ticks
                        int valueIndex = (ticks - startTicks) / LIVE_DATA_PERIOD;

                        // Add NaNs and the new value at the index
                        while (data[0].Count <= valueIndex)
                        {
                            if (data[0].Count == valueIndex)
                            {
                                // Calculate the actual value
                                double actualValue = ((double)value.Value - offset) * factor;

                                // Queue this value
                                data[0].Enqueue(actualValue);

                                // Recalculate min and max if necessary
                                if ((actualValue < min) || (actualValue > max))
                                {
                                    min = Math.Min(actualValue, min);
                                    max = Math.Max(actualValue, max);

                                    // Calculate the Y ticks
                                    yTicks = Ticks(min, max);
                                }
                            }
                            else
                            {
                                // We missed a value so queue a NaN
                                data[0].Enqueue(double.NaN);
                            }
                        }

                        // Update the number of values
                        if (values < data[0].Count)
                        {
                            values = data[0].Count;

                            // Crop any values outside the window
                            while (values >= window)
                            {
                                data[0].Dequeue();
                                values--;
                                startTicks += LIVE_DATA_PERIOD;
                            }

                            // Update the start value
                            Start = startTicks / 1000.0;

                            // Calculate the X ticks
                            xTicks = Ticks(start, start + (increment * (values - 1)));
                        }
                    }
                }

                // Refresh the control
                Refresh();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GraphWidget()
        {
            InitializeComponent();

            // Initialise the widget size
            Width = GRAPH_WIDTH;
            Height = GRAPH_HEIGHT;

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

            // Initialise the offset tool
            windowTool = new ToolStripTextBox()
            {
                TextBoxTextAlign = HorizontalAlignment.Right,
                AutoSize = false,
                Width = 50,
            };
            windowTool.KeyPress += windowTool_KeyPress;
            windowTool.Leave += windowTool_Leave;

            // Augment the tools list
            tools.Add(new ToolStripLabel("Factor"));
            tools.Add(factorTool);
            tools.Add(new ToolStripLabel("Offset"));
            tools.Add(offsetTool);
            tools.Add(new ToolStripLabel("Window"));
            tools.Add(windowTool);

            // Initialise the properties
            Factor = 1.0;
            Offset = 0.0;
            Window = LIVE_DATA_DEFAULT_WINDOW;
        }

        /// <summary>
        /// Set channel data directly
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="channelValues">The array of values</param>
        public void SetData(int channel, double[] channelValues)
        {
            // Set the value
            data[channel] = (channelValues == null) ? new Queue<double>() :
                new Queue<double>(channelValues);

            // Initialise the number of values and the minimum and maximum
            values = 0;
            min = double.MaxValue;
            max = double.MinValue;

            // Loop through the channels
            foreach (var channelData in data)
            {
                // Check for data
                if (channelData != null)
                {
                    // Update the number of values
                    if (values < channelData.Count)
                    {
                        values = channelData.Count;
                    }

                    // Loop through channel data
                    foreach (var value in channelData)
                    {
                        // Check for valid data
                        if (double.IsNaN(value) == false)
                        {
                            // Update min and max
                            if (value < min)
                            {
                                min = value;
                            }
                            if (value > max)
                            {
                                max = value;
                            }
                        }
                    }
                }
            }

            // Calculate the Y ticks
            yTicks = Ticks(min, max);

            // Calculate the X ticks
            xTicks = Ticks(start, start + (increment * (values - 1)));

            // Refresh the widget
            Refresh();
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        public void ClearData()
        {
            min = double.MaxValue;
            max = double.MinValue;
            values = 0;
            startTicks = 0;
            xTicks = new List<double>();
            yTicks = new List<double>();
            data = new Queue<double>[4];
            start = 0.0;
            increment = 1.0;
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
            Window = ParseInt(reader.GetAttribute("Window"), LIVE_DATA_DEFAULT_WINDOW);
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
            writer.WriteAttributeString("Window", Window.ToString());
        }

        /// <summary>
        /// Calculate tick positions
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>An enumeration of tick positions</returns>
        private IEnumerable<double> Ticks(double min, double max)
        {
            // Create the result
            var result = new List<double>();

            // Check for valid data
            if (min >= max)
            {
                return result;
            }

            // Calculate the range, exponent, power and fraction
            double range = max - min;
            double exponent = Math.Floor(Math.Log10(range));
            double power = Math.Pow(10, exponent);
            double fraction = range / power;

            // Make the fraction integer
            if (fraction < 1.0)
            {
                fraction = 1.0;
            }
            else if (fraction < 2.0)
            {
                fraction = 2.0;
            }
            else if (fraction < 5.0)
            {
                fraction = 5.0;
            }
            else
            {
                fraction = 10.0;
            }

            // Calculate the increment, start and end
            double increment = (fraction * power) / 10.0;
            double start = Math.Ceiling(min / increment) * increment;
            double end = Math.Floor(max / increment) * increment;

            // Generate the enumeration 
            while (start <= end)
            {
                result.Add(start);
                start += increment;
            }

            return result;
        }

        /// <summary>
        /// Resize override
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Calculate the graph and axes rectangles
            graphRectangle = new Rectangle(Y_AXIS_WIDTH, 0, Width - Y_AXIS_WIDTH, Height - X_AXIS_HEIGHT);
            xAxisRectangle = new Rectangle(Y_AXIS_WIDTH, graphRectangle.Bottom, graphRectangle.Width, 
                X_AXIS_HEIGHT);
            yAxisRectangle = new Rectangle(0, 0, Y_AXIS_WIDTH, graphRectangle.Height);
        }

        /// <summary>
        /// Paint override
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            // Remember the smoothing mode
            var lastSmoothignMode = pe.Graphics.SmoothingMode;

            // Set the smoothing mode
            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw the X and Y axes
            pe.Graphics.DrawLine(Pens.Black, yAxisRectangle.Right, yAxisRectangle.Top,
                yAxisRectangle.Right, yAxisRectangle.Bottom);
            pe.Graphics.DrawLine(Pens.Black, xAxisRectangle.Left, xAxisRectangle.Top,
                xAxisRectangle.Right, xAxisRectangle.Top);

            // Check for actual data
            if ((values < 2) || (min >= max))
            {
                pe.Graphics.SmoothingMode = lastSmoothignMode;
                return;
            }

            // Calculate the X axis factor and offset
            double xFactor = graphRectangle.Width / (increment * (values - 1));
            double xOffset = 0f - start;

            // Generate the initial rectangle for the label
            Rectangle labelRectangle = new Rectangle(0, xAxisRectangle.Top + TICK_LENGTH, 0,
                xAxisRectangle.Height - TICK_LENGTH);

            // Loop through the ticks
            foreach (var tick in xTicks)
            {
                // Calculate the screen position
                int x = xAxisRectangle.Left + (int)((tick + xOffset) * xFactor);

                // Draw the tick
                pe.Graphics.DrawLine(Pens.Black, x, xAxisRectangle.Top, x, xAxisRectangle.Top + 
                    TICK_LENGTH);

                // Draw the grid line
                pe.Graphics.DrawLine(Pens.Gray, x, graphRectangle.Top, x, graphRectangle.Bottom);

                // Modify the label width
                labelRectangle.Width = x - labelRectangle.X;

                // Draw the label
                TextRenderer.DrawText(pe.Graphics, tick.ToString("G"), Font, labelRectangle,
                    Color.Black, TextFormatFlags.SingleLine | TextFormatFlags.Top | 
                    TextFormatFlags.Right);

                // Move to the next label position
                labelRectangle.X = x;
            }

            // Calculate the Y axis factor and offset
            double yFactor = graphRectangle.Height / (max - min);
            double yOffset = 0f - min;

            // Generate the initial rectangle for the label
            labelRectangle = new Rectangle(0, Height, yAxisRectangle.Width - TICK_LENGTH, 0);

            // Loop through the ticks
            foreach (var tick in yTicks)
            {
                // Calculate the screen position
                int y = yAxisRectangle.Bottom - (int)((tick + yOffset) * yFactor);

                // Draw the tick
                pe.Graphics.DrawLine(Pens.Black, yAxisRectangle.Right - TICK_LENGTH, y, 
                    yAxisRectangle.Right, y);

                // Draw the grid line
                pe.Graphics.DrawLine(Pens.Gray, graphRectangle.Left, y, graphRectangle.Right, y);

                // Move to the label position
                labelRectangle.Height = labelRectangle.Y - y;
                labelRectangle.Y = y;

                // Draw the label
                TextRenderer.DrawText(pe.Graphics, tick.ToString("G"), Font, labelRectangle,
                    Color.Black, TextFormatFlags.SingleLine | TextFormatFlags.Top |
                    TextFormatFlags.Right);
            }

            // Calculate the X increment
            float xIncrement = graphRectangle.Width / (float)(values - 1);

            // Initialise the channel counter
            int channel = 0;

            // Loop through the channels
            foreach (var channelData in data)
            {
                // Check for data
                if ((channelData != null) && (channelData.Count > 1))
                {
                    // Intialise the x position
                    float x = graphRectangle.Left;

                    // Initialise the graphics path
                    var gp = new GraphicsPath();

                    // Initialise the last and next points
                    PointF? last = null;
                    PointF next = new PointF();

                    // Loop through the channel data
                    foreach (var value in channelData)
                    {
                        // Check for a value
                        if (double.IsNaN(value) == false)
                        {
                            // Set the next value
                            next.X = x;
                            next.Y = graphRectangle.Bottom - (float)((value + yOffset) * yFactor);

                            // Check for a previous value
                            if (last.HasValue)
                            {
                                gp.AddLine(last.Value, next);
                            }

                            // Set the last value to this one
                            last = next;
                        }

                        // Increment the X offset
                        x += xIncrement;
                    }

                    // Draw the line
                    pe.Graphics.DrawPath(pens[channel++], gp);
                }
            }

            // Reset the smoothing mode
            pe.Graphics.SmoothingMode = lastSmoothignMode;

            // Determine the text to display
            var text = string.IsNullOrEmpty(Text) ? DataSourceName : Text;

            if (string.IsNullOrEmpty(text) && (ParentView != null) && (ParentView.Editing))
            {
                text = "value";
            }

            // Draw the text
            TextRenderer.DrawText(pe.Graphics, text, Font, graphRectangle,
                Color.Black, TextFormatFlags.SingleLine | TextFormatFlags.Top |
                TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);

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

        /// <summary>
        /// Key press event handler for the window tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void windowTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                windowTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the window tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void windowTool_Leave(object sender, EventArgs e)
        {
            int value;

            if (int.TryParse(windowTool.Text, out value))
            {
                // Calculate the new window
                int newWindow = (1000 * value) / LIVE_DATA_PERIOD;

                if (window != newWindow)
                {
                    // Set the value, modified flag and refresh
                    window = newWindow;

                    if (ParentView != null)
                    {
                        ParentView.Modified = true;
                    }

                    Refresh();
                }
            }
            else
            {
                Window = window;
            }
        }
    }
}
