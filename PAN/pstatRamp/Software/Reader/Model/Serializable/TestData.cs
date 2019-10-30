/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Test data object
    /// </summary>
    public class TestData : ITestData
    {
        /// <summary>
        /// The current differentials for cell 1
        /// </summary>
        [XmlElement("Cell1Values")]
        public override string Cell1ValuesString
        {
            get
            {
                return WriteCellValues(CellValues[0]);
            }
            set
            {
                CellValues[0] = ReadCellValues(value).ToArray();
            }
        }

        /// <summary>
        /// The current differentials for cell 2
        /// </summary>
        [XmlElement("Cell2Values")]
        public override string Cell2ValuesString
        {
            get
            {
                return WriteCellValues(CellValues[1]);
            }
            set
            {
                CellValues[1] = ReadCellValues(value).ToArray();
            }
        }

        /// <summary>
        /// The current differentials for cell 3
        /// </summary>
        [XmlElement("Cell3Values")]
        public override string Cell3ValuesString
        {
            get
            {
                return WriteCellValues(CellValues[2]);
            }
            set
            {
                CellValues[2] = ReadCellValues(value).ToArray();
            }
        }

        /// <summary>
        /// The current differentials for cell 4
        /// </summary>
        [XmlElement("Cell4Values")]
        public override string Cell4ValuesString
        {
            get
            {
                return WriteCellValues(CellValues[3]);
            }
            set
            {
                CellValues[3] = ReadCellValues(value).ToArray();
            }
        }

        /// <summary>
        /// The temperature values for PCR
        /// </summary>
        [XmlElement("PcrValues")]
        public override string PcrValuesString
        {
            get
            {
                return WriteCellValues(PcrValues);
            }
            set
            {
                PcrValues = ReadCellValues(value);
            }
        }

        /// <summary>
        /// Write the cell values to a comma separated string
        /// </summary>
        /// <param name="values">The values</param>
        /// <returns>The string</returns>
        private string WriteCellValues(IEnumerable<double> values)
        {
            // Check for null
            if (values == null)
            {
                return null;
            }

            // Create a string builder
            var stringBuilder = new StringBuilder();
            
            // Initialise a falg for the first value
            bool first = true;

            // Loop through the values
            foreach (var value in values)
            {
                // Don't add a comma for the first value
                if (first)
                {
                    first = false;
                }
                else
                {
                    stringBuilder.Append(',');
                }

                // Append the value
                stringBuilder.Append(value.ToString("F1"));
            }

            // Wrtie the string
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Read the cell values from a comma separated string
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private List<double> ReadCellValues(string values)
        {
            // Check for null
            if (values == null)
            {
                return null;
            }

            // Initialise the result as a list
            var result = new List<double>();

            // Initialise the splitting characters
            var splits = new char[] { ',', ' ' };

            // Loop through the values
            foreach (var value in values.Split(splits, StringSplitOptions.RemoveEmptyEntries))
            {
                // Add the value to the list
                result.Add(double.Parse(value));
            }

            // Retrun the list as an array
            return result;
        }
    }
}
