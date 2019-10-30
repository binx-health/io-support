/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using IO.Model.Serializable;

namespace IO.View.Tests
{
    /// <summary>
    /// Results object for the test
    /// </summary>
    public class Results : IResults
    {
        /// <summary>
        /// The current index
        /// </summary>
        private int index = -1;

        /// <summary>
        /// The tests
        /// </summary>
        private List<ITest> tests;

        /// <summary>
        /// The start date filter
        /// </summary>
        private DateTime? startDateTime;

        /// <summary>
        /// The locking user filter
        /// </summary>
        private uint lockingUserId;

        /// <summary>
        /// The patient ID filter
        /// </summary>
        private string patientId;

        /// <summary>
        /// The assay name filter
        /// </summary>
        private string assayName;

        /// <summary>
        /// The start date filter
        /// </summary>
        public override DateTime? StartDateTime
        {
            get
            {
                return startDateTime;
            }
            set
            {
                // Check for a change and update
                if (startDateTime != value)
                {
                    startDateTime = value;

                    UpdateTests();
                }
            }
        }

        /// <summary>
        /// The locking user filter
        /// </summary>
        public override uint LockingUserId
        {
            get
            {
                return lockingUserId;
            }
            set
            {
                // Check for a change and update
                if (lockingUserId != value)
                {
                    lockingUserId = value;

                    UpdateTests();
                }
            }
        }

        /// <summary>
        /// The patient ID filter
        /// </summary>
        public override string PatientId
        {
            get
            {
                return patientId;
            }
            set
            {
                // Check for a change and update
                if (patientId != value)
                {
                    patientId = value;

                    UpdateTests();
                }
            }
        }

        /// <summary>
        /// The assay name filter
        /// </summary>
        public override string AssayName
        {
            get
            {
                return assayName;
            }
            set
            {
                // Check for a change and update
                if (assayName != value)
                {
                    assayName = value;

                    UpdateTests();
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Results()
        {
            tests = ITests.Instance;
        }

        /// <summary>
        /// Indexer for tests
        /// </summary>
        /// <param name="sampleId">The sample ID</param>
        /// <returns>The test</returns>
        public override ITest this[string sampleId]
        {
            get
            {
                return ITests.Instance.Where(x => x.Result.SampleId == sampleId).FirstOrDefault();
            }
        }

        /// <summary>
        /// Add a result to the file
        /// </summary>
        /// <param name="test">The test</param>
        public override void AddResult(ITest test)
        {
        }

        /// <summary>
        /// Read the next result from the file
        /// </summary>
        /// <returns></returns>
        public override ITest NextResult()
        {
            if (index < tests.Count)
            {
                ++index;
            }

            if (index < tests.Count)
            {
                return tests[index];
            }

            return null;
        }

        /// <summary>
        /// Read the previous result from the file
        /// </summary>
        /// <returns></returns>
        public override ITest PreviousResult()
        {
            if (index > -1)
            {
                --index;
            }

            if (index > 0)
            {
                return tests[index];
            }

            return null;
        }

        /// <summary>
        /// Reset to the start of the file
        /// </summary>
        public override void Reset()
        {
            index = -1;
        }

        /// <summary>
        /// Update the tests
        /// </summary>
        private void UpdateTests()
        {
            tests = ITests.Instance.Where(x =>
                ((StartDateTime.HasValue == false) || (x.Result.StartDateTime.Date == StartDateTime.Value.Date)) &&
                ((LockingUserId == 0) || (x.LockingUserId == LockingUserId)) &&
                ((PatientId == null) || (x.Result.PatientInformation.ContainsKey("PatientId") &&
                ((string)x.Result.PatientInformation["PatientId"] == PatientId))) &&
                ((AssayName == null) || (x.Result.AssayName == AssayName))).ToList();

            Reset();
        }
    }
}
