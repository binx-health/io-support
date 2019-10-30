/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.View.Concrete
{
    /// <summary>
    /// Patient information edit form
    /// </summary>
    public partial class PatientInformationEdit : PatientInformation
    {
        /// <summary>
        /// Field values dictionary
        /// </summary>
        public Dictionary<string, object> FieldValues
        {
            get
            {
                return fieldValues;
            }
            set
            {
                fieldValues = value;

                UpdateValues();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PatientInformationEdit()
        {
            InitializeComponent();

            lock (ITests.Instance.CurrentTest)
            {
                // Check for a QC test
                if (ITests.Instance.CurrentTest.Result.QcTest)
                {
                    // Initialise the inherited controls
                    titleBar.Text = Properties.Resources.EditQcDetails;
                }
            }
        }
    }
}
