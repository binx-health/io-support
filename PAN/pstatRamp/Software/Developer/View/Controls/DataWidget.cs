/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Xml;
using System.Windows.Forms;

namespace IO.View.Controls
{
    /// <summary>
    /// Data widget control base class
    /// </summary>
    public partial class DataWidget : Widget
    {
        /// <summary>
        /// The data source name
        /// </summary>
        private string dataSourceName;

        /// <summary>
        /// The data value
        /// </summary>
        private int? rawValue;

        /// <summary>
        /// The data source tool
        /// </summary>
        private ToolStripTextBox dataSourceTool = null;

        /// <summary>
        /// Data source name accessor
        /// </summary>
        public virtual string DataSourceName
        {
            get
            {
                return dataSourceName;
            }
            set
            {
                dataSourceName = value;

                if (dataSourceTool != null)
                {
                    dataSourceTool.Text = (value == null) ? "" : value;
                }

                // Set or clear an advisory link
                if (Main.Instance != null)
                {
                    if (string.IsNullOrEmpty(dataSourceName))
                    {
                        Main.Instance.Unadvise(this);
                    }
                    else
                    {
                        Main.Instance.Advise(this, DataSourceName);
                    }
                }
            }
        }

        /// <summary>
        /// The virtual control value
        /// </summary>
        public virtual int? Value
        {
            get
            {
                return rawValue;
            }
            set
            {
                rawValue = value;

                // Refresh the control
                Refresh();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataWidget()
        {
            InitializeComponent();

            // Initialise the data source tool
            dataSourceTool = new ToolStripTextBox()
            {
                AutoSize = false,
                Width = 50,
            };
            dataSourceTool.KeyPress += dataSourceTool_KeyPress;
            dataSourceTool.Leave += dataSourceTool_Leave;

            // Augment the tools list
            tools.Add(new ToolStripLabel("Data Source"));
            tools.Add(dataSourceTool);
        }

        /// <summary>
        /// Read the values from XML
        /// </summary>
        /// <param name="reader">The XML reader</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            DataSourceName = reader.GetAttribute("DataSourceName");
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeString("DataSourceName", DataSourceName);
        }

        /// <summary>
        /// Drag eneter override
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            // Check for a data source holder
            if (drgevent.Data.GetDataPresent(typeof(DataSourceHolder)))
            {
                drgevent.Effect |= DragDropEffects.Link;
            }
        }

        /// <summary>
        /// Drag drop override
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            // Get the data source holder
            var dataSourceHolder = drgevent.Data.GetData(typeof(DataSourceHolder)) as DataSourceHolder;

            if (dataSourceHolder != null)
            {
                // Set the data source name
                DataSourceName = dataSourceHolder.Name;

                // Set the modified flag
                if (ParentView != null)
                {
                    ParentView.Modified = true;
                }
                
                // Refresh the widget
                Refresh();
            }
        }

        /// <summary>
        /// Key press event handler for the data source tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSourceTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the return key
            if (e.KeyChar == '\r')
            {
                dataSourceTool_Leave(sender, e);
            }
        }

        /// <summary>
        /// Leave event handler for the data source tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSourceTool_Leave(object sender, EventArgs e)
        {
            if (DataSourceName != dataSourceTool.Text)
            {
                // Set the data source, modified flag and refresh
                DataSourceName = dataSourceTool.Text;

                if (ParentView != null)
                {
                    ParentView.Modified = true;
                }

                Refresh();
            }
        }
    }
}
