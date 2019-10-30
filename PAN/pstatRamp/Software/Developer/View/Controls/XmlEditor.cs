/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IO.View.Controls
{
    /// <summary>
    /// Script editor control
    /// </summary>
    public partial class XmlEditor : UserControl
    {
        /// <summary>
        /// The associated XML
        /// </summary>
        private string xml;

        /// <summary>
        /// The modified flag
        /// </summary>
        private bool modified = false;

        /// <summary>
        /// The type of object being serialised
        /// </summary>
        private Type objectType;

        /// <summary>
        /// The additional object types for serialisation
        /// </summary>
        private Type[] additionalObjectTypes;

        /// <summary>
        /// The associated XML
        /// </summary>
        public string Xml
        {
            get
            {
                return xml;
            }
            set
            {
                xml = value;

                // Update the controls
                UpdateControls();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public XmlEditor(Type type, Type[] additionalTypes)
        {
            // Initialise the object types
            objectType = type;
            additionalObjectTypes = additionalTypes;

            InitializeComponent();
        }

        /// <summary>
        /// Update the controls to reflect the current script
        /// </summary>
        private void UpdateControls()
        {
            // Clear any errors
            listBox.Items.Clear();

            // Initialsie the text to display
            string text = null;

            // Initialise an XML document
            var xmlDocument = new XmlDocument();

            try
            {
                // Load the XML
                xmlDocument.LoadXml(xml);

                // Write formatted XML to the text variable
                using (var memoryStream = new MemoryStream())
                {
                    using (var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode) 
                        { Formatting = Formatting.Indented })
                    {
                        xmlDocument.WriteContentTo(xmlTextWriter);
                        xmlTextWriter.Flush();

                        memoryStream.Flush();
                        memoryStream.Position = 0;

                        using (var streamReader = new StreamReader(memoryStream))
                        {
                            text = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            // Check for a null value
            if (text == null)
            {
                // Clear the data and disable the controls
                textBox.Text = null;
                textBox.Enabled = false;
                listBox.Enabled = false;
            }
            else
            {
                // Check for a modified value and set this unless we have already modified the script
                if ((modified == false) && (textBox.Text != text))
                {
                    // Set the new value without firing the changed event
                    textBox.TextChanged -= textBox_TextChanged;
                    textBox.Text = text;
                    textBox.TextChanged += textBox_TextChanged;
                }

                // Enable editing
                textBox.Enabled = true;
            }
        }

        /// <summary>
        /// Text changed event for the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            // Reset the timer
            timer.Enabled = false;
            timer.Enabled = true;

            // Set the modified flag
            modified = true;
        }

        /// <summary>
        /// Tick event handler for the text changed timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Disable the timer
            timer.Enabled = false;

            // Clear any errors
            listBox.Items.Clear();

            try
            {
                // Create the serialiser from the type
                var xmlSerializer = new XmlSerializer(objectType, additionalObjectTypes);

                // Create a string reader
                using (var stringReader = new StringReader(textBox.Text))
                {
                    // Deserialise the object and return it
                    xmlSerializer.Deserialize(stringReader);
                }

                // The XML is valid so set it
                xml = textBox.Text;

                // Notify of the change
                MessageQueue.Instance.Push(new MenuItemMessage("ConfigurationEdit")
                {
                    Parameters = new Dictionary<string, object>() { { "Value", xml } }
                });
            }
            catch (Exception exception)
            {
                // Add the error to the list box
                listBox.Items.Add(exception.Message);
            }

            // Clear the modified flag
            modified = false;
        }
    }
}
