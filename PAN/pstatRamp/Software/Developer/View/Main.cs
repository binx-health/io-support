/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Diagnostics;
using IO.Comms.Network;
using IO.Scripting;
using IO.Model.Volatile;
using IO.Model.Serializable;
using IO.FileSystem;

namespace IO.View
{
    /// <summary>
    /// Main developer application form
    /// </summary>
    public partial class Main : Form, IXmlSerializable
    {
        /// <summary>
        /// Period for updating all values in ms
        /// </summary>
        private static readonly int COMPLETE_VALUE_UPDATE_PERIOD = 2000;

        /// <summary>
        /// Set of valve names
        /// </summary>
        private readonly HashSet<string> VALVES = new HashSet<string>()
            { "v1", "v2", "v3", "v4", "v5", "v6", "v7", "v8", "v9", "v10", "v11", "v12", "v13", "v14", 
                "v15", "v16", "v17" };

        /// <summary>
        /// Set of solenoid names
        /// </summary>
        private readonly HashSet<string> SOLENOIDS = new HashSet<string>()
            { "s1", "m1" };

        /// <summary>
        /// Static instance variable
        /// </summary>
        public static Main Instance { get; set; }

        /// <summary>
        /// The index of the selected device
        /// </summary>
        private int selectedDeviceIndex = -1;

        /// <summary>
        /// The assays tree node
        /// </summary>
        private TreeNode assaysNode = null;

        /// <summary>
        /// The root scripts tree node
        /// </summary>
        private TreeNode rootScriptsNode = null;

        /// <summary>
        /// The default metrics tree node
        /// </summary>
        private TreeNode defaultMetricsNode = null;

        /// <summary>
        /// The test data tree node
        /// </summary>
        private TreeNode testDataNode = null;

        /// <summary>
        /// The configuration tree node
        /// </summary>
        private TreeNode configurationNode = null;

        /// <summary>
        /// The current control
        /// </summary>
        private UserControl control = null;

        /// <summary>
        /// The name of the running script
        /// </summary>
        private string runningScript = null;

        /// <summary>
        /// Flag to indicate the current command is at the end of the terminal
        /// </summary>
        private bool currentCommandAtEnd = true;

        /// <summary>
        /// Command history
        /// </summary>
        private LinkedList<string> commandHistory = new LinkedList<string>();

        /// <summary>
        /// Dictionary of advised widgets by data source name
        /// </summary>
        private Dictionary<string, HashSet<Controls.DataWidget>> advisedWidgets =
            new Dictionary<string, HashSet<Controls.DataWidget>>();

        /// <summary>
        /// Loaded flag
        /// </summary>
        private bool loaded = false;

        /// <summary>
        /// Dictionary of active views
        /// </summary>
        private Dictionary<string, View> views = new Dictionary<string, View>();

        /// <summary>
        /// The preferred device
        /// </summary>
        private string preferredDevice = null;

        /// <summary>
        /// Stopwatch for ensuring all values are updated at least once per second
        /// </summary>
        private Stopwatch stopwatch = null;

        /// <summary>
        /// The time of the next complete value update in milliseconds
        /// </summary>
        private long nextCompleteValueUpdate;

        /// <summary>
        /// The reader port
        /// </summary>
        private int readerPort = 443;

        /// <summary>
        /// Flag for editing supporting scripts
        /// </summary>
        private bool editSupportingScripts = false;

        /// <summary>
        /// The reader port
        /// </summary>
        public int ReaderPort
        {
            get
            {
                return readerPort;
            }
        }

        /// <summary>
        /// Flag for editing supporting scripts
        /// </summary>
        public bool EditSupportingScripts
        {
            get
            {
                return editSupportingScripts;
            }
        }

        /// <summary>
        /// The list of open views
        /// </summary>
        public List<string> OpenViews { get; private set; }

        /// <summary>
        /// The current configuration
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// The preferred device
        /// </summary>
        public string PreferredDevice
        {
            get
            {
                lock (preferredDevice)
                {
                    return string.Copy(preferredDevice);
                }
            }
        }

        /// <summary>
        /// Thread-safe phase accessor
        /// </summary>
        public string Phase
        {
            get
            {
                string value = null;

                Invoke((MethodInvoker)delegate
                {
                    value = toolStripStatusLabelPhase.Text;
                });

                return value;
            }
            set
            {
                Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabelPhase.Text = value;
                });
            }
        }

        /// <summary>
        /// Thread-safe running script accessor
        /// </summary>
        public string RunningScript
        {
            get
            {
                string value = null;

                Invoke((MethodInvoker)delegate
                {
                    value = runningScript;
                });

                return value;
            }
            set
            {
                Invoke((MethodInvoker)delegate
                {
                    runningScript = value;

                    // Update the buttons to reflect the change
                    UpdateButtons();
                });
            }
        }

        /// <summary>
        /// Thread-safe modified flag accessor
        /// </summary>
        public bool Modified
        {
            get
            {
                bool modified = false;

                Invoke((MethodInvoker)delegate
                {
                    modified = toolStripButtonSave.Enabled;
                });

                return modified;
            }
            set
            {
                Invoke((MethodInvoker)delegate
                {
                    toolStripButtonSave.Enabled = value;
                });
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Main()
        {
            InitializeComponent();

            // Initialise the open views
            OpenViews = new List<string>();

            // Clear the phase text
            toolStripStatusLabelPhase.Text = null;

            // Add an empty command
            commandHistory.AddLast("");

            // Create and start the stopwatch
            stopwatch = new Stopwatch();
            stopwatch.Start();

            // Set the time for the next second
            nextCompleteValueUpdate = stopwatch.ElapsedMilliseconds + COMPLETE_VALUE_UPDATE_PERIOD;
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
        public void ReadXml(XmlReader reader)
        {
            // Read the window position
            SetBounds(int.Parse(reader.GetAttribute("Bounds.X")),
                int.Parse(reader.GetAttribute("Bounds.Y")),
                int.Parse(reader.GetAttribute("Bounds.Width")),
                int.Parse(reader.GetAttribute("Bounds.Height")));
            WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), 
                reader.GetAttribute("WindowState"));

            // Read the preferred device
            preferredDevice = reader.GetAttribute("PreferredDevice");

            // Read the reader port
            var readerPortAttribute = reader.GetAttribute("ReaderPort");
            int readerPortValue;

            if (int.TryParse(readerPortAttribute, out readerPortValue))
            {
                readerPort = readerPortValue;
            }

            // Read the edit supporting scripts flag
            var editSupportingScriptsAttribute = reader.GetAttribute("EditSupportingScripts");
            bool editSupportingScriptsValue;

            if (bool.TryParse(editSupportingScriptsAttribute, out editSupportingScriptsValue))
            {
                editSupportingScripts = editSupportingScriptsValue;
            }

            // Read the XML to the end
            while (true)
            {
                // Check for an element
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Depth == 1))
                {
                    // Add the value to the dictionary
                    OpenViews.Add(reader.ReadElementContentAsString());
                }
                else if (reader.Read() == false)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Write the values to XML
        /// </summary>
        /// <param name="writer">The XML writer</param>
        public void WriteXml(XmlWriter writer)
        {
            // Write the window position
            writer.WriteAttributeString("Bounds.X", Bounds.X.ToString());
            writer.WriteAttributeString("Bounds.Y", Bounds.Y.ToString());
            writer.WriteAttributeString("Bounds.Width", Bounds.Width.ToString());
            writer.WriteAttributeString("Bounds.Height", Bounds.Height.ToString());
            writer.WriteAttributeString("WindowState", WindowState.ToString());

            // Write the preferred device
            writer.WriteAttributeString("PreferredDevice", preferredDevice);

            // Write the reader port
            writer.WriteAttributeString("ReaderPort", readerPort.ToString());

            // Write the edit supporting scripts flag
            writer.WriteAttributeString("EditSupportingScripts", editSupportingScripts.ToString());

            // Write the open views
            foreach (var openView in OpenViews)
            {
                writer.WriteElementString("OpenView", openView);
            }
        }

        /// <summary>
        /// Update the device values
        /// </summary>
        public void UpdateDeviceValues(IDeviceValues values)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check for a second having elapsed
                if (stopwatch.ElapsedMilliseconds >= nextCompleteValueUpdate)
                {
                    // Reset the next second
                    nextCompleteValueUpdate += COMPLETE_VALUE_UPDATE_PERIOD;

                    // Check for a period of inactivity
                    if (nextCompleteValueUpdate <= stopwatch.ElapsedMilliseconds)
                    {
                        nextCompleteValueUpdate = stopwatch.ElapsedMilliseconds + 
                            COMPLETE_VALUE_UPDATE_PERIOD;
                    }

                    // Loop through tall of the device values updating them
                    foreach (var key in IDeviceValues.Instance.Keys.Union(values.Keys).ToArray())
                    {
                        int newValue;

                        if (values.TryGetValue(key, out newValue))
                        {
                            UpdateDeviceValue(key, newValue);
                        }
                        else
                        {
                            UpdateDeviceValue(key, IDeviceValues.Instance[key]);
                        }
                    }
                }
                else
                {
                    // Loop through the modified device values updating them
                    foreach (var value in values)
                    {
                        int currentValue;

                        if (IDeviceValues.Instance.TryGetValue(value.Key, out currentValue))
                        {
                            if (currentValue != value.Value)
                            {
                                UpdateDeviceValue(value.Key, value.Value);
                            }
                        }
                        else
                        {
                            UpdateDeviceValue(value.Key, value.Value);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Update the status bar
        /// </summary>
        public void UpdateStatus()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Check for an error
                if (string.IsNullOrEmpty(LastError.Message) == false)
                {
                    toolStripStatusLabel.Text = LastError.Message;
                    toolStripStatusLabel.Image = Properties.Resources.Error;
                }
                // Check if we are connected
                else if (IClient.Instance.Connected)
                {
                    toolStripStatusLabel.Text = Properties.Resources.Connected;
                    toolStripStatusLabel.Image = Properties.Resources.Ok;
                }
                // Check if we have discovered devices
                else if (IClient.Instance.Devices.Count() > 0)
                {
                    toolStripStatusLabel.Text = Properties.Resources.Found;
                    toolStripStatusLabel.Image = Properties.Resources.Ok;
                }
                // We are searching
                else
                {
                    toolStripStatusLabel.Text = Properties.Resources.Searching;
                    toolStripStatusLabel.Image = Properties.Resources.Ok;
                }

                // Conditionally display the phase
                toolStripStatusLabelPhase.Visible = IClient.Instance.Connected;

                // Enable the read, write and reset buttons based on the connection status
                toolStripButtonRead.Enabled = IClient.Instance.Connected;
                toolStripButtonWrite.Enabled = (loaded == false) && IClient.Instance.Connected;
                toolStripButtonPower.Enabled = IClient.Instance.Connected;
                toolStripButtonReset.Enabled = selectedDeviceIndex != -1;
                toolStripButtonCommandPrompt.Enabled = IClient.Instance.Connected;
            });
        }

        /// <summary>
        /// Thread-safe method to update the assays in the tree
        /// </summary>
        public void UpdateAssays()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Initialise overall modified and loaded flags
                bool modified = false;
                loaded = true;

                // Create a default metrics node if required
                if (defaultMetricsNode == null)
                {
                    defaultMetricsNode = treeViewExplorer.Nodes.Add(Properties.Resources.DefaultMetrics,
                        Properties.Resources.DefaultMetrics);
                    defaultMetricsNode.Tag = IDefaultMetrics.Instance;
                }

                // Set the image based on its state
                defaultMetricsNode.ImageIndex = defaultMetricsNode.SelectedImageIndex = 
                    IDefaultMetrics.Instance.Modified ? 6 : 5;

                // Update the flags
                modified |= IDefaultMetrics.Instance.Modified;
                loaded &= IDefaultMetrics.Instance.Loaded;

                // Create an assays node if required
                if (assaysNode == null)
                {
                    assaysNode = treeViewExplorer.Nodes.AddExpanded(Properties.Resources.Assays,
                        Properties.Resources.Assays, 7);
                    assaysNode.Tag = IAssays.Instance;
                }

                // Remove any child nodes not in the assays list
                RemoveChildNodes(assaysNode.Nodes, IAssays.Instance.Select(x => x.Name));

                // Initialise a set of assay scripts
                var assayScripts = new List<string>();

                // Loop through the assays
                foreach (var assay in IAssays.Instance)
                {
                    // Find any existing node
                    var assayNode = FindChildNode(assaysNode.Nodes, assay.Name);

                    // Create a node for this assay if required
                    if (assayNode == null)
                    {
                        assayNode = assaysNode.Nodes.AddExpanded(assay.Name, assay.Name, 3);
                    }

                    assayNode.Tag = assay;

                    // Set the image to reflect the status
                    assayNode.ImageIndex = assayNode.SelectedImageIndex = assay.Modified ? 4 : 3;

                    // Remove any metrics that are not the assay metrics
                    RemoveChildNodes(assayNode.Nodes, new string[] { assay.MetricsName }, typeof(IMetrics));

                    // Find any existing metrics node
                    var node = FindChildNode(assayNode.Nodes, assay.MetricsName, typeof(IMetrics));

                    // Create a node for the assay metrics if required
                    if (node == null)
                    {
                        node = assayNode.Nodes.Add(assay.MetricsName, assay.MetricsName, 
                            assay.Metrics.Modified ? 6 : 5);
                    }

                    // Set the node data and image index to reflect its state
                    node.Tag = assay.Metrics;
                    node.ImageIndex = node.SelectedImageIndex = assay.Metrics.Modified ? 6 : 5;

                    // Update the flag
                    modified |= assay.Metrics.Modified;
                    loaded &= assay.Metrics.Loaded;

                    // Create an array of assay scripts
                    var scripts = new string[] { assay.Script, assay.UngScript, assay.VoltammetryScript };

                    // Remove any script nodes not in the list
                    RemoveChildNodes(assayNode.Nodes, scripts, typeof(IScript));

                    // Add the scripts for this assay
                    assayScripts.AddRange(AddScriptTree(assayNode.Nodes, scripts, ref modified, ref loaded));

                    // Update the flags
                    modified |= assay.Modified;
                    loaded &= assay.Loaded;
                }

                // Update the loaded flag
                loaded &= IAssays.Instance.Loaded;

                // Check for and create a root scrips node if required
                if (rootScriptsNode == null)
                {
                    rootScriptsNode = treeViewExplorer.Nodes.AddExpanded(Properties.Resources.RootScripts,
                        Properties.Resources.RootScripts, 7);
                }

                // Create a distinct list of scripts called by other scripts (not themselves)
                var calledScripts = IScripts.Instance.SelectMany(x => x.Children.Where(
                    y => string.Compare(y, x.Name, true) != 0)).Distinct();

                // Remove any empty root scripts
                foreach (var script in IScripts.Instance.Where(x => string.IsNullOrEmpty(x.Value)).Select(
                    x => x.Name).Except(calledScripts, StringComparer.CurrentCultureIgnoreCase).Except(
                    assayScripts, StringComparer.CurrentCultureIgnoreCase).Except(
                    IAssays.SupportingScripts, StringComparer.CurrentCultureIgnoreCase).ToArray())
                {
                    IScripts.Instance.SetScript(script, null);
                }

                // Initialise an array of root scripts
                var rootScripts = IScripts.Instance.Select(x => x.Name).Except(
                    assayScripts, StringComparer.CurrentCultureIgnoreCase).ToArray();

                // Remove any script nodes not in the list
                RemoveChildNodes(rootScriptsNode.Nodes, rootScripts, typeof(IScript));

                // Add the supporting root scripts
                AddScriptTree(rootScriptsNode.Nodes, rootScripts.Intersect(IAssays.SupportingScripts, 
                    StringComparer.CurrentCultureIgnoreCase), ref modified, ref loaded);

                // Add the other root scripts
                bool otherLoaded = false;

                // Remove any called scripts
                rootScripts = rootScripts.Except(calledScripts, 
                    StringComparer.CurrentCultureIgnoreCase).ToArray();

                AddScriptTree(rootScriptsNode.Nodes, rootScripts.Except(IAssays.SupportingScripts,
                    StringComparer.CurrentCultureIgnoreCase), ref modified, ref otherLoaded);

                // Set the flag states
                Modified = modified;

                // Update the write button
                toolStripButtonWrite.Enabled = (loaded == false) && IClient.Instance.Connected;

                // Update the buttons
                UpdateButtons();
            });
        }

        /// <summary>
        /// Thread-safe method to update the test-data in the tree
        /// </summary>
        public void UpdateTestData()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Create a test data node if required
                if (testDataNode == null)
                {
                    testDataNode = treeViewExplorer.Nodes.AddExpanded(Properties.Resources.TestData,
                        Properties.Resources.TestData, 7);
                    testDataNode.Tag = ITests.Instance;
                }
                else
                {
                    // Remove any obsolete child nodes
                    RemoveChildNodes(testDataNode.Nodes, ITests.Instance.Select(x => x.Result.SampleId));
                }

                // Loop through the tests adding them in reverse order
                foreach (var test in ITests.Instance)
                {
                    // Check for an existing node
                    var existingTestNode = FindChildNode(testDataNode.Nodes, test.Result.SampleId);

                    if (existingTestNode == null)
                    {
                        // Create a new test data node
                        var testNode = testDataNode.Nodes.Insert(0, test.Result.SampleId, 
                            test.Result.StartDateTime.ToLocalTime().ToString());
                        testNode.Tag = test;
                        testNode.ImageIndex = testNode.SelectedImageIndex = 8;
                    }
                }
            });
        }

        /// <summary>
        /// Update the configuration
        /// </summary>
        public void UpdateConfiguration()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                if (Configuration != null)
                {
                    // Create a configuration node if required
                    if (configurationNode == null)
                    {
                        configurationNode = treeViewExplorer.Nodes.AddExpanded(
                            Properties.Resources.Configuration, Properties.Resources.Configuration, 0);
                    }
                    else
                    {
                        if (control is Controls.XmlEditor)
                        {
                            // Set the new script
                            (control as Controls.XmlEditor).Xml = Configuration;
                        }
                    }
                }
                else
                {
                    // Remove the configuration node if required
                    if (configurationNode != null)
                    {
                        treeViewExplorer.Nodes.Remove(configurationNode);

                        configurationNode = null;
                    }
                }
            });
        }

        /// <summary>
        /// Thread-safe method to update the current control
        /// </summary>
        public void UpdateControl()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                UpdateControl(treeViewExplorer.SelectedNode);
            });
        }

        /// <summary>
        /// Thread-safe method to update the list of devices
        /// </summary>
        /// <param name="devices">The list of devices</param>
        public void UpdateDevices(IEnumerable<string> devices = null)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // Create a list of items to remove
                var itemsToRemove = new List<string>();

                // Create a list of remaining devices
                var remainingDevices = (devices == null) ? new List<string>() : new List<string>(devices);

                // Initialise the selected device to null
                string selectedDevice = null;

                // Loop through the items in the devies combo
                foreach (string item in toolStripComboBoxDevices.Items)
                {
                    // Try to find an existing device
                    var existingDevice = remainingDevices.Find(x => x == item);

                    // If it does not exist then add it to the remove list
                    if (existingDevice == null)
                    {
                        itemsToRemove.Add(item);
                    }
                    else
                    {
                        // Check for no preferred device or the preferred device and select it
                        if ((selectedDevice == null) && 
                            ((preferredDevice == null) || (existingDevice == preferredDevice)))
                        {
                            selectedDevice = existingDevice;
                        }

                        // Remove this from the remaining items
                        remainingDevices.Remove(existingDevice);
                    }
                }

                // Remove the items to remove
                foreach (var item in itemsToRemove)
                {
                    // Check for removal of the selected device
#pragma warning disable 252
                    if (toolStripComboBoxDevices.SelectedItem == item)
#pragma warning restore 252
                    {
                        selectedDeviceIndex = -1;
                        toolStripButtonReset.Enabled = false;
                    }

                    toolStripComboBoxDevices.Items.Remove(item);
                }

                // Add the remaining items
                foreach (var item in remainingDevices)
                {
                    toolStripComboBoxDevices.Items.Add(item);

                    // Check for no preferred device or the preferred device and select it
                    if ((selectedDevice == null) &&
                        ((preferredDevice == null) || (item == preferredDevice)))
                    {
                        selectedDevice = item;
                    }
                }

                // Enable the control and select the selcted item
                toolStripComboBoxDevices.Enabled = toolStripComboBoxDevices.Items.Count > 0;
                toolStripComboBoxDevices.SelectedItem = selectedDevice;
            });
        }

        /// <summary>
        /// Thread-safe log method
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            // Check for a message
            if (string.IsNullOrEmpty(message) == false)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    // Remember where the selection is
                    var selectionStart = textBoxTerminal.Text.Length;

                    // Append the message and update the selection
                    textBoxTerminal.AppendText(message);
                    textBoxTerminal.SelectionStart = selectionStart;
                    textBoxTerminal.SelectionLength = textBoxTerminal.Text.Length - textBoxTerminal.SelectionStart;
                    textBoxTerminal.ScrollToCaret();

                    // The current command is no longer at the end of the terminal
                    currentCommandAtEnd = false;
                });
            }
        }

        /// <summary>
        /// Called by a view when it is closed
        /// </summary>
        /// <param name="name"></param>
        public void ViewClosed(string name)
        {
            // Get the view
            var view = views[name];

            if (view != null)
            {
                // Check the modified flag
                if (view.Modified)
                {
                    // Create a string builder
                    var stringBuilder = new StringBuilder();

                    // Create the serialiser from the type
                    var xmlSerializer = new XmlSerializer(typeof(View));

                    // Create an XML writer to serialise the object as a file
                    using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings()
                    {
                        Indent = true,
                    }))
                    {
                        // Serialise the object
                        xmlSerializer.Serialize(xmlWriter, view);
                    }

                    // Return the XML fragment as a string
                    ILocalFileSystem.Instance.WriteTextFile("Views\\" + name, stringBuilder.ToString());
                }

                // Remove the view
                views[name] = null;
            }
        }

        /// <summary>
        /// Rename a view
        /// </summary>
        /// <param name="oldName">The old name</param>
        /// <param name="newName">The new name</param>
        /// <returns>True if the new name is valid</returns>
        public bool RenameView(string oldName, string newName)
        {
            // Try to get the existing view and check the new name is valid and not used
            View view;

            if (views.TryGetValue(oldName, out view) &&
                (newName.Intersect(Path.GetInvalidFileNameChars()).Count() == 0) &&
                (views.ContainsKey(newName) == false))
            {
                // Write the new file
                ILocalFileSystem.Instance.WriteTextFile("Views\\" + newName,
                    ILocalFileSystem.Instance.ReadTextFile("Views\\" + oldName));

                // Delete the old file
                ILocalFileSystem.Instance.WriteTextFile("Views\\" + oldName, null);

                // Reindex the view
                views.Remove(oldName);
                views[newName] = view;

                // Update the tool strip drop-down
                for (int index = 2; index < toolStripDropDownButtonViews.DropDownItems.Count; ++index)
                {
                    if (toolStripDropDownButtonViews.DropDownItems[index].Text == oldName)
                    {
                        toolStripDropDownButtonViews.DropDownItems[index].Text = newName;
                        break;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Set up an advisor for a widget foir a data source
        /// </summary>
        /// <param name="widget">The widget</param>
        /// <param name="dataSourceName">The data source name</param>
        public void Advise(Controls.DataWidget widget, string dataSourceName)
        {
            // Clear any current advisors
            Unadvise(widget);

            // Try to get the current set for this data source
            HashSet<Controls.DataWidget> widgets;

            if (advisedWidgets.TryGetValue(dataSourceName, out widgets))
            {
                // Add this to the set
                widgets.Add(widget);
            }
            else
            {
                // Create a new advisor for this data source
                advisedWidgets[dataSourceName] = new HashSet<Controls.DataWidget>() { widget };
            }

            // Try to get the current value from the device
            int currentValue;

            if (IDeviceValues.Instance.TryGetValue(dataSourceName, out currentValue))
            {
                // Set the widget value
                widget.Value = currentValue;
            }
            else
            {
                // Clear the widget value
                widget.Value = null;
            }
        }

        /// <summary>
        /// Demove any advisors for this widget
        /// </summary>
        /// <param name="widget">The widget</param>
        public void Unadvise(Controls.DataWidget widget)
        {
            // Compile a list of data sources to remove
            var dataSourceNamesToRemove = new List<string>();

            // Loop through the advisors
            foreach (var advisor in advisedWidgets)
            {
                // Remove this widget and check for an empty data source
                if (advisor.Value.Remove(widget) && (advisor.Value.Count == 0))
                {
                    // Add it to the list to remove
                    dataSourceNamesToRemove.Add(advisor.Key);
                }
            }

            // Loop through the removal list
            foreach (var dataSourceName in dataSourceNamesToRemove)
            {
                advisedWidgets.Remove(dataSourceName);
            }
        }

        /// <summary>
        /// Set the value of a device
        /// </summary>
        /// <param name="dataSourceName">The device name</param>
        /// <param name="value">The new value</param>
        /// <returns>True if the device is a recognised type, false otherwise</returns>
        public bool SetDeviceValue(string deviceName, int value)
        {
            // Generate the command for the data source
            string command;

            // Check for a valve
            if (VALVES.Contains(deviceName))
            {
                command = "valve " + deviceName + " " + ((value == 0) ? "off" : "on");
            }
            // Check for a solenoid
            else if (SOLENOIDS.Contains(deviceName))
            {
                command = "solenoid " + deviceName + " " + ((value == 0) ? "off" : "on");
            }
            else
            {
                return false;
            }

            // Issue the command
            MessageQueue.Instance.Push(new MenuItemMessage("Command")
            {
                Parameters = new Dictionary<string, object>() { { "Value", command } }
            });

            return true;
        }

        /// <summary>
        /// Load a named view from storage
        /// </summary>
        /// <param name="name">The name o fthe view</param>
        /// <returns>The view</returns>
        private View LoadView(string name)
        {
            var fragment = ILocalFileSystem.Instance.ReadTextFile("Views\\" + name);

            if (fragment != null)
            {
                // Create the serialiser from the type
                var xmlSerializer = new XmlSerializer(typeof(View));

                // Create a string reader
                using (var stringReader = new StringReader(fragment))
                {
                    // Deserialise the object and return it
                    return xmlSerializer.Deserialize(stringReader) as View;
                }
            }

            return new View();
        }

        /// <summary>
        /// Update a device value in the list
        /// </summary>
        /// <param name="name">The device name</param>
        /// <param name="value">The device value</param>
        private void UpdateDeviceValue(string name, int value)
        {
            // Set the value in the device values cache
            IDeviceValues.Instance[name] = value;

            // Find the exisiting list item
            var listViewItem = listViewDeviceValues.Items.Find(name, false).FirstOrDefault();

            if (listViewItem == null)
            {
                // Add a new item
                listViewItem = listViewDeviceValues.Items.Add(new ListViewItem() 
                    { Name = name, Text = name });
                listViewItem.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = value.ToString() });
            }
            else
            {
                // Update the existing item
                listViewItem.SubItems[1].Text = value.ToString();
            }

            // Get any advised widgets
            HashSet<Controls.DataWidget> widgets;

            if (advisedWidgets.TryGetValue(name, out widgets))
            {
                // Advise them of the change
                foreach (var widget in widgets)
                {
                    widget.Value = value;
                }
            }
        }

        /// <summary>
        /// Update the buttons
        /// </summary>
        private void UpdateButtons()
        {
            // Get the currently selected script node
            var scriptNode = ((treeViewExplorer.SelectedNode != null) &&
                (treeViewExplorer.SelectedNode.Tag is IScript)) ? 
                treeViewExplorer.SelectedNode.Tag as IScript : null;

            // Update the script buttons
            toolStripButtonExecute.Enabled = (scriptNode != null) &&
                (string.IsNullOrEmpty(runningScript) || (runningScript == "?"));
            toolStripButtonAbort.Enabled = string.IsNullOrEmpty(runningScript) == false;
            toolStripButtonCommand.Enabled = (scriptNode != null) &&
                (string.IsNullOrEmpty(runningScript) || (runningScript == "?"));
        }

        /// <summary>
        /// Add scripts to the tree
        /// </summary>
        /// <param name="nodes">The nodes collection to add to</param>
        /// <param name="scriptNames">The names of the scripts to add</param>
        /// <param name="modified">The cumulative modified flag</param>
        /// <param name="loaded">The cumulative loaded flag</param>
        /// <param name="ancestry">The current ancestry</param>
        /// <returns>The scripts added at this level</returns>
        private IEnumerable<string> AddScriptTree(TreeNodeCollection nodes, IEnumerable<string> scriptNames, 
            ref bool modified, ref bool loaded, Stack<string> ancestry = null)
        {
            // Initialise a list of scripts added
            var result = new List<string>();

            // Create the ancestry if required
            if (ancestry == null)
            {
                ancestry = new Stack<string>();
            }

            // Loop through the scripts
            foreach (var scriptName in scriptNames.Except(ancestry).OrderBy(x => x))
            {
                // Get the script object and the associated node
                var script = IScripts.Instance.Where(
                    x => string.Compare(x.Name, scriptName, true) == 0).FirstOrDefault();
                var node = FindChildNode(nodes, scriptName, typeof(IScript));

                // Create a script if required
                if (script == null)
                {
                    script = IScripts.Instance.SetScript(scriptName, "");
                }

                // Add a node if required
                if (node == null)
                {
                    node = nodes.Add(scriptName, scriptName, 0);
                }

                // Set the script data for the node
                node.Tag = script;

                // Add the script to the list
                result.Add(scriptName);

                // Set the image to reflect the state
                node.ImageIndex = node.SelectedImageIndex =
                    (script.Errors.FirstOrDefault() == null) ? (script.Modified ? 2 : 0) : 1;

                // Check for the current editor
                if ((treeViewExplorer.SelectedNode == node) && (control != null) && 
                    (control is Controls.ScriptEditor))
                {
                    // Update the script
                    (control as Controls.ScriptEditor).Script = script;
                }

                // Push the script name onto the ancestry
                ancestry.Push(scriptName);

                // Remove any script nodes not in the list
                RemoveChildNodes(node.Nodes, script.Children, typeof(IScript));

                // Reccursively add the child scripts
                AddScriptTree(node.Nodes, script.Children, ref modified, ref loaded, ancestry);

                // Pop the script name off the ancestry
                ancestry.Pop();

                // Update the flags
                modified |= script.Modified;
                loaded &= script.Loaded;
            }

            return result;
        }

        /// <summary>
        /// Remove any unwanted tree nodes
        /// </summary>
        /// <param name="nodes">The list of nodes</param>
        /// <param name="names">The names of the nodes to keep</param>
        /// <param name="type">The type of objects</param>
        private void RemoveChildNodes(TreeNodeCollection nodes, IEnumerable<string> names, Type type = null)
        {
            // Initialise a list of nodes to remove
            var nodesToRemove = new List<TreeNode>();

            // Loop through the nodes
            foreach (TreeNode node in nodes)
            {
                // Check for matching node types that are not in the list
                if ((names.Contains(node.Name) == false) &&
                    ((type == null) || (node.Tag.GetType().IsSubclassOf(type))))
                {
                    nodesToRemove.Add(node);
                }
            }

            // Remove the unwanted nodes
            foreach (var node in nodesToRemove)
            {
                nodes.Remove(node);
            }
        }

        /// <summary>
        /// Find an named tree node of a given type
        /// </summary>
        /// <param name="nodes">The nodes</param>
        /// <param name="name">The name</param>
        /// <param name="type">The type of object</param>
        /// <returns>The tree node</returns>
        private TreeNode FindChildNode(TreeNodeCollection nodes, string name, Type type = null)
        {
            // Loop through the nodes looking for a match
            foreach (TreeNode node in nodes)
            {
                if ((node.Name == name) && ((type == null) || (node.Tag.GetType().IsSubclassOf(type))))
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Update the current control for the selected node
        /// </summary>
        /// <param name="treeNode">The selected node</param>
        public void UpdateControl(TreeNode treeNode)
        {
            // Check for a null node
            if (treeNode == null)
            {
                // Check for an active control
                if (control != null)
                {
                    // Clear the panel
                    splitContainerExplorer.Panel2.Controls.Remove(control);
                    control.Dispose();
                    control = null;
                }

                // Disable the delete button
                toolStripButtonDelete.Enabled = false;
                toolStripButtonAdd.Enabled = treeNode == assaysNode;
            }
            // Check for a script node
            else if (treeNode.Tag is IScript)
            {
                // Check for no current control
                if (control == null)
                {
                    // Create the control
                    control = new Controls.ScriptEditor() { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.ScriptEditor).Script = treeNode.Tag as IScript;
                }
                // Check for an existing script editor
                else if (control is Controls.ScriptEditor)
                {
                    // Set the new script
                    (control as Controls.ScriptEditor).Script = treeNode.Tag as IScript;
                }
                // Another type of control is present
                else
                {
                    // Remove the control and dispose of it
                    splitContainerExplorer.Panel2.Controls.Remove(control);
                    control.Dispose();

                    // Create the new control
                    control = new Controls.ScriptEditor() { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.ScriptEditor).Script = treeNode.Tag as IScript;
                }

                // Conditionally enable the delete button
                toolStripButtonDelete.Enabled = (treeNode.Parent == rootScriptsNode) &&
                    (IAssays.SupportingScripts.Contains(treeNode.Name, 
                    StringComparer.CurrentCultureIgnoreCase) == false);
                toolStripButtonAdd.Enabled = false;
            }
            // Check for a metrics node
            else if (treeNode.Tag is IMetrics)
            {
                // Check for default metrics
                bool readOnly = (treeNode.Tag == IDefaultMetrics.Instance) &&
                    (EditSupportingScripts == false);

                // Check for no current control
                if (control == null)
                {
                    // Create the control
                    control = new Controls.MetricsEditor() { Dock = DockStyle.Fill, ReadOnly = readOnly };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.MetricsEditor).Metrics = treeNode.Tag as IMetrics;
                }
                // Check for an existing script editor
                else if (control is Controls.MetricsEditor)
                {
                    // Set the new script
                    (control as Controls.MetricsEditor).Metrics = treeNode.Tag as IMetrics;
                    (control as Controls.MetricsEditor).ReadOnly = readOnly;
                }
                // Another type of control is present
                else
                {
                    // Remove the control and dispose of it
                    splitContainerExplorer.Panel2.Controls.Remove(control);
                    control.Dispose();

                    // Create the new control
                    control = new Controls.MetricsEditor() { Dock = DockStyle.Fill, ReadOnly = readOnly };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.MetricsEditor).Metrics = treeNode.Tag as IMetrics;
                }

                // Disable the delete button
                toolStripButtonDelete.Enabled = false;
                toolStripButtonAdd.Enabled = false;
            }
            // Check for an assay node
            else if (treeNode.Tag is IAssay)
            {
                // Check for no current control
                if (control == null)
                {
                    // Create the control
                    control = new Controls.AssayEditor() { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.AssayEditor).Assay = treeNode.Tag as IAssay;
                }
                // Check for an existing script editor
                else if (control is Controls.AssayEditor)
                {
                    // Set the new script
                    (control as Controls.AssayEditor).Assay = treeNode.Tag as IAssay;
                }
                // Another type of control is present
                else
                {
                    // Remove the control and dispose of it
                    splitContainerExplorer.Panel2.Controls.Remove(control);
                    control.Dispose();

                    // Create the new control
                    control = new Controls.AssayEditor() { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the assay
                    (control as Controls.AssayEditor).Assay = treeNode.Tag as IAssay;
                }

                // Enable the delete button
                toolStripButtonDelete.Enabled = true;
                toolStripButtonAdd.Enabled = false;
            }
            // Check for a test node
            else if (treeNode.Tag is ITest)
            {
                // Check for no current control
                if (control == null)
                {
                    // Create the control
                    control = new Controls.TestViewer() { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.TestViewer).Test = treeNode.Tag as ITest;
                }
                // Check for an existing script editor
                else if (control is Controls.TestViewer)
                {
                    // Set the new script
                    (control as Controls.TestViewer).Test = treeNode.Tag as ITest;
                }
                // Another type of control is present
                else
                {
                    // Remove the control and dispose of it
                    splitContainerExplorer.Panel2.Controls.Remove(control);
                    control.Dispose();

                    // Create the new control
                    control = new Controls.TestViewer() { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.TestViewer).Test = treeNode.Tag as ITest;
                }

                // Enable the delete button
                toolStripButtonDelete.Enabled = false;
                toolStripButtonAdd.Enabled = false;
            }
            else if (treeNode == configurationNode)
            {
                // Check for no current control
                if (control == null)
                {
                    // Create the control
                    control = new Controls.XmlEditor(IConfiguration.Instance.GetType(),
                        IConfiguration.Instance.Types) { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.XmlEditor).Xml = Configuration;
                }
                // Check for an existing script editor
                else if (control is Controls.XmlEditor)
                {
                    // Set the new script
                    (control as Controls.XmlEditor).Xml = Configuration;
                }
                // Another type of control is present
                else
                {
                    // Remove the control and dispose of it
                    splitContainerExplorer.Panel2.Controls.Remove(control);
                    control.Dispose();

                    // Create the new control
                    control = new Controls.XmlEditor(IConfiguration.Instance.GetType(),
                        IConfiguration.Instance.Types) { Dock = DockStyle.Fill };

                    // Add it to the panel
                    splitContainerExplorer.Panel2.Controls.Add(control);

                    // Set the script
                    (control as Controls.XmlEditor).Xml = Configuration;
                }

                // Enable the delete button
                toolStripButtonDelete.Enabled = false;
                toolStripButtonAdd.Enabled = false;
            }
            else
            {
                // Check for an active control
                if (control != null)
                {
                    // Clear the panel
                    splitContainerExplorer.Panel2.Controls.Remove(control);
                    control.Dispose();
                    control = null;
                }

                // Disable the delete button
                toolStripButtonDelete.Enabled = false;
                toolStripButtonAdd.Enabled = treeNode == assaysNode;
            }
        }

        /// <summary>
        /// Load event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            // Push the initialise message
            MessageQueue.Instance.Push(new CommandMessage(Text, FormCommand.Initialise));
        }

        /// <summary>
        /// Closed event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Clear the open views
            OpenViews.Clear();

            // Close any open forms
            foreach (var viewName in views.Where(x => x.Value != null).Select(x => x.Key).ToArray())
            {
                // Add to the list of open views
                OpenViews.Add(viewName);

                // Close the view
                ViewClosed(viewName);
            }

            // Save the settings
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Shown event handler for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Shown(object sender, EventArgs e)
        {
            // Flag to create a new fiew
            bool createNewView = true;

            // Add all existing views
            var viewFiles = ILocalFileSystem.Instance.GetFiles("Views");

            if (viewFiles != null)
            {
                foreach (var name in viewFiles)
                {
                    // Add a drop-down item
                    toolStripDropDownButtonViews.DropDownItems.Add(name, null, toolStripMenuItem_Click);

                    if (OpenViews.Contains(name))
                    {
                        // Load the view from storage
                        var view = LoadView(name);
                        var bounds = view.Bounds;
                        var windowState = view.WindowState;

                        // Add the view to the dictionary
                        views[name] = view;

                        // Set the name and state
                        view.Name = name;
                        view.Show();
                        view.BringToFront();
                        view.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                        view.WindowState = windowState;

                        // Clear the modified flag
                        view.Modified = false;
                    }
                    else
                    {
                        // Add to the dictionary
                        views[name] = null;
                    }

                    // Clear the create new flag
                    createNewView = false;
                }
            }

            if (createNewView)
            {
                // Create a new view
                newToolStripMenuItem_Click(this, e);
            }
        }

        /// <summary>
        /// Click event handler for the power button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonPower_Click(object sender, EventArgs e)
        {
            // Push the reset message
            MessageQueue.Instance.Push(new MenuItemMessage("Reset"));
        }

        /// <summary>
        /// Click event handler for the reset button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonReset_Click(object sender, EventArgs e)
        {
            // Clear the terminal
            textBoxTerminal.Clear();

            // Push the reset message
            MessageQueue.Instance.Push(new MenuItemMessage("Reconnect"));
        }

        /// <summary>
        /// Click event handler for the command prompt button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonCommandPrompt_Click(object sender, EventArgs e)
        {
            // Push the initialise message
            MessageQueue.Instance.Push(new MenuItemMessage("CommandPrompt"));
        }

        /// <summary>
        /// Click event handler for the reload button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonReload_Click(object sender, EventArgs e)
        {
            // Push the initialise message
            MessageQueue.Instance.Push(new MenuItemMessage("Load"));
        }

        /// <summary>
        /// Click event handler for the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            // Push the initialise message
            MessageQueue.Instance.Push(new MenuItemMessage("Save"));
        }

        /// <summary>
        /// Click event handler for the export button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            // Select a folder
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Push the initialise message
                MessageQueue.Instance.Push(new MenuItemMessage("Export")
                {
                    Parameters = new Dictionary<string, object>()
                    { 
                        { "Folder", folderBrowserDialog.SelectedPath },
                    },
                });
            }
        }

        /// <summary>
        /// After select event handler for the tree view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateControl(e.Node);
            UpdateButtons();
        }

        /// <summary>
        /// Click event handler for the read button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonRead_Click(object sender, EventArgs e)
        {
            // Push the read message
            MessageQueue.Instance.Push(new MenuItemMessage("Read"));

            // Disable the button
            toolStripButtonRead.Enabled = false;
        }

        /// <summary>
        /// Click event handler for the write button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonWrite_Click(object sender, EventArgs e)
        {
            // Push the write message
            MessageQueue.Instance.Push(new MenuItemMessage("Write"));

            // Disable the button
            toolStripButtonWrite.Enabled = false;
        }

        /// <summary>
        /// Key press event handler for the terminal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTerminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the current command is not at the end then rewrite it
            if (currentCommandAtEnd == false)
            {
                textBoxTerminal.AppendText(commandHistory.Last.Value);
                currentCommandAtEnd = true;
            }

            // Check for a non-control character
            if (char.IsControl(e.KeyChar) == false)
            {
                // Appenf it to the text box and the command
                textBoxTerminal.AppendText(e.KeyChar.ToString());
                commandHistory.Last.Value += e.KeyChar;
            }
            // Check for a backspace
            else if (e.KeyChar == '\b')
            {
                // If there is a command then truncate it
                if (commandHistory.Last.Value.Length > 0)
                {
                    commandHistory.Last.Value = commandHistory.Last.Value.Substring(0, 
                        commandHistory.Last.Value.Length - 1);
                    textBoxTerminal.Text = textBoxTerminal.Text.Substring(0, 
                        textBoxTerminal.Text.Length - 1);
                }
            }
            // Check for the enter key
            else if (e.KeyChar == '\r')
            {
                // If there is a command then execute it
                if (commandHistory.Last.Value.Length > 0)
                {
                    MessageQueue.Instance.Push(new MenuItemMessage("Command")
                    {
                        Parameters = new Dictionary<string, object>() 
                            { { "Value", commandHistory.Last.Value } }
                    });

                    // Add a new empty command
                    commandHistory.AddLast("");
                }

                // Write a new line
                textBoxTerminal.AppendText("\r\n");
            }

            // Select the end of the text box
            textBoxTerminal.Select(textBoxTerminal.Text.Length, 0);
            textBoxTerminal.ScrollToCaret();
        }

        /// <summary>
        /// Click event handler for the execute button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonExecute_Click(object sender, EventArgs e)
        {
            // Get the current script node
            var scriptNode = ((treeViewExplorer.SelectedNode != null) &&
                (treeViewExplorer.SelectedNode.Tag is IScript)) ? treeViewExplorer.SelectedNode.Tag as IScript : null;

            if (scriptNode != null)
            {
                // Push the execute message
                MessageQueue.Instance.Push(new MenuItemMessage("Execute")
                {
                    Parameters = new Dictionary<string, object> { { "Name", scriptNode.Name } }
                });
            }
        }

        /// <summary>
        /// Click event handler for the abort button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAbort_Click(object sender, EventArgs e)
        {
            // Push the abort message
            MessageQueue.Instance.Push(new MenuItemMessage("Abort"));
        }

        /// <summary>
        /// Click event handler for the command button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonCommand_Click(object sender, EventArgs e)
        {
            // Get the current script editor and the current line
            var scriptEditor = ((control != null) && (control is Controls.ScriptEditor)) ? 
                control as Controls.ScriptEditor : null;
            var currentLine = (scriptEditor == null) ? null : scriptEditor.CurrentCommand();

            // Check for a current line
            if (string.IsNullOrEmpty(currentLine) == false)
            {
                // Push the command message
                MessageQueue.Instance.Push(new MenuItemMessage("Command")
                {
                    Parameters = new Dictionary<string, object> { { "Value", currentLine } }
                });

                // Move to the next line
                scriptEditor.NextCommand();
            }
        }

        /// <summary>
        /// Resize event handler for the device values list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewDeviceValues_Resize(object sender, EventArgs e)
        {
            // Resize the columns
            foreach (ColumnHeader column in listViewDeviceValues.Columns)
            {
                column.Width = listViewDeviceValues.ClientSize.Width / 2;
            }
        }

        /// <summary>
        /// Key down handler for the terminal text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            // Suppress left and right
            if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right))
            {
                e.Handled = true;
            }
            // Handle up and down to scroll through the command history
            else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down))
            {
                // Check for a command history list
                if (commandHistory.Count > 1)
                {
                    // If the current command is not at the front then remove the current command
                    if (currentCommandAtEnd)
                    {
                        textBoxTerminal.Text = textBoxTerminal.Text.Substring(0,
                            textBoxTerminal.Text.Length - commandHistory.Last.Value.Length);
                    }

                    if (e.KeyCode == Keys.Up)
                    {
                        // Go back by removing the last item and making it the first item
                        if (string.IsNullOrEmpty(commandHistory.Last()) == false)
                        {
                            commandHistory.AddFirst(commandHistory.Last());
                        }
                        commandHistory.RemoveLast();
                    }
                    else
                    {
                        // Go forward by removing the first item and making it the last item
                        if (string.IsNullOrEmpty(commandHistory.Last()))
                        {
                            commandHistory.Last.Value = commandHistory.First();
                        }
                        else
                        {
                            commandHistory.AddLast(commandHistory.First());
                        }
                        commandHistory.RemoveFirst();
                    }

                    // Append the new value
                    textBoxTerminal.AppendText(commandHistory.Last.Value);

                    // Set the flag
                    currentCommandAtEnd = true;
                }

                // Select the end of the text box
                textBoxTerminal.Select(textBoxTerminal.Text.Length, 0);
                textBoxTerminal.ScrollToCaret();

                // Mark as handled to suppress cursor movement
                e.Handled = true;
            }
        }

        /// <summary>
        /// Click event handler for the delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if ((treeViewExplorer.SelectedNode == null) ||
                (treeViewExplorer.SelectedNode.Tag == null))
            {
                return;
            }

            if (treeViewExplorer.SelectedNode.Tag is IScript)
            {
                IScripts.Instance.SetScript(treeViewExplorer.SelectedNode.Name, null);

                UpdateAssays();
            }
            else if (treeViewExplorer.SelectedNode.Tag is IAssay)
            {
                IAssays.Instance.Remove(treeViewExplorer.SelectedNode.Tag as IAssay);
                IAssays.Instance.Loaded = false;

                UpdateAssays();
            }
            else
            {
                toolStripButtonDelete.Enabled = false;
            }
        }

        /// <summary>
        /// Click event handler for the add button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            if (treeViewExplorer.SelectedNode != assaysNode)
            {
                toolStripButtonAdd.Enabled = false;
                return;
            }

            TreeNode newNode = assaysNode.Nodes.AddExpanded("", "", 4);
            
            treeViewExplorer.LabelEdit = true;
            
            newNode.BeginEdit();

            toolStripButtonAdd.Enabled = false;
        }

        /// <summary>
        /// After label edit event handler for the tree view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewExplorer_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Cancel label editing
            treeViewExplorer.LabelEdit = false;

            // Re-enable the add button
            toolStripButtonAdd.Enabled = true;

            // Add the new assay
            var newAssay = IAssays.Instance.AddNew((e.Label == null) ? null : e.Label.Trim());

            // Check for an invalid name
            if (newAssay == null)
            {
                // Remove the node if it exists
                if (e.Node != null)
                {
                    assaysNode.Nodes.Remove(e.Node);
                }
            }
            else
            {
                // Update the assays
                UpdateAssays();
            }
        }

        /// <summary>
        /// Clisk event for the new menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Generate a unique view name
            string name = null;

            for (int index = 1; index < int.MaxValue; ++index)
            {
                name = "New View " + index.ToString();

                if (views.ContainsKey(name) == false)
                {
                    break;
                }
            }

            // Create the new view
            var view = new View();

            // Add the drip-down item
            toolStripDropDownButtonViews.DropDownItems.Add(name, null, toolStripMenuItem_Click);

            // Add the view to the dictionary
            views[name] = view;

            // Set the name and state
            view.Name = name;
            view.Show();
            view.BringToFront();

            // Clear the modified flag
            view.Modified = false;
        }

        /// <summary>
        /// Clisk event for the new menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            var toolStripDropDownItem = sender as ToolStripDropDownItem;
            View view;

            if (views.TryGetValue(toolStripDropDownItem.Text, out view) && (view != null))
            {
                view.BringToFront();
            }
            else
            {
                // Get the view name
                var name = toolStripDropDownItem.Text;

                // Load the view from storage
                view = LoadView(name);

                // Add the view to the dictionary
                views[name] = view;

                // Set the name and state
                view.Name = name;
                view.Show();
                view.BringToFront();

                // Clear the modified flag
                view.Modified = false;
            }
        }

        /// <summary>
        /// Item drag event handler for the device values list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewDeviceValues_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Drag the item
            DoDragDrop(new DataSourceHolder((e.Item as ListViewItem).Name), DragDropEffects.Link);
        }

        /// <summary>
        /// Selected index changed event handler for the devices combo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check for a changed selection
            if (toolStripComboBoxDevices.SelectedIndex != selectedDeviceIndex)
            {
                // Update the selction
                selectedDeviceIndex = toolStripComboBoxDevices.SelectedIndex;

                // Check for a valid selection
                if (selectedDeviceIndex >= 0)
                {
                    // Set the preferred device
                    preferredDevice = toolStripComboBoxDevices.SelectedItem as string;

                    // Clear the terminal
                    textBoxTerminal.Clear();

                    // Push the reconnect message
                    MessageQueue.Instance.Push(new MenuItemMessage("Reconnect"));
                }
            }
        }
    }
}
