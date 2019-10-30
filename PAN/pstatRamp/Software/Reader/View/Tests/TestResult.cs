/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using IO.Model.Serializable;
using IO.Scripting;

namespace IO.View.Tests
{
    /// <summary>
    /// Test result object for the test
    /// </summary>
    public class TestResult : ITestResult
    {
        /// <summary>
        /// The start date and time for the test
        /// </summary>
        [XmlElement("StartDateTime")]
        public override string StartDateTimeString
        {
            get
            {
                return StartDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                StartDateTime = DateTime.Parse(value);
            }
        }

        /// <summary>
        /// The end date and time for the test
        /// </summary>
        [XmlElement("EndDateTime")]
        public override string EndDateTimeString
        {
            get
            {
                return EndDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                EndDateTime = DateTime.Parse(value);
            }
        }

        /// <summary>
        /// The assay
        /// </summary>
        [XmlElement("Assay")]
        public override string AssayName
        {
            get
            {
                return (Assay != null) ? Assay.Name : null;
            }
            set
            {
                Assay = IAssays.Instance.Where(x => x.Name == value).FirstOrDefault();
            }
        }
    }
}
