/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using IO.FileSystem;
using IO.Scripting;

namespace IO.Model.Serializable
{
    /// <summary>
    /// Test results file
    /// </summary>
    public class Results: IResults
    {
        /// <summary>
        /// Stack of positions 
        /// </summary>
        private Stack<long> positions = new Stack<long>();

        /// <summary>
        /// Dictionary of sample IDs
        /// </summary>
        private Dictionary<string, long> sampleIds = new Dictionary<string, long>();

        /// <summary>
        /// The current regex used for filtering or null
        /// </summary>
        private Regex regex = null;

        /// <summary>
        /// The start date filter
        /// </summary>
        private DateTime? startDateTime;

        /// <summary>
        /// The locking user filter
        /// </summary>
        private uint lockingUserId;

        /// <summary>
        /// The sample ID filter
        /// </summary>
        private string sampleId;

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

                    UpdateRegex();
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

                    UpdateRegex();
                }
            }
        }

        /// <summary>
        /// The sample ID filter
        /// </summary>
        public override string SampleId
        {
            get
            {
                return sampleId;
            }
            set
            {
                // Check for a change and update
                if (sampleId != value)
                {
                    sampleId = value;

                    UpdateRegex();
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

                    UpdateRegex();
                }
            }
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
                // Get the position of the test in the file
                long testPosition;

                lock (sampleIds)
                {
                    if (sampleIds.TryGetValue(sampleId, out testPosition) == false)
                    {
                        return null;
                    }
                }

                // Read the line from the file
                string result = ILocalFileSystem.Instance.ReadNextLine("Results", ref testPosition);

                // Get the test object
                return TestFromString(result);
            }
        }

        /// <summary>
        /// Write the result text to string
        /// </summary>
        /// <param name="test">The test</param>
        /// <returns>The result text as a string</returns>
        public override string ResultText(ITest test)
        {
            // Create a new string builder
            var stringBuilder = new StringBuilder();

            // Append the sample ID
            AppendString(stringBuilder, test.Result.SampleId);

            // Check if the patient ID should be recorded and conditionally record it
            var field = IConfiguration.Instance.Fields.Where(x => x.Name == "PatientId").FirstOrDefault();
            object value;

            if ((field != null) && (field.Policy == FieldPolicy.Record) &&
                test.Result.PatientInformation.TryGetValue("PatientId", out value))
            {
                // Append the value
                AppendString(stringBuilder, value.ToString());
            }
            else
            {
                // Append and empty value
                AppendString(stringBuilder, "");
            }

            // Append the user
            stringBuilder.Append(',').Append(test.LockingUserId);

            // Append the start date and time
            AppendString(stringBuilder, test.Result.StartDateTimeString);

            // Append the assay name
            AppendString(stringBuilder, test.Result.AssayName);

            // Append the assay name
            AppendString(stringBuilder, test.Result.AssayVersion.ToString());

            // Append the test outcome
            AppendString(stringBuilder, test.Result.Outcome.ToString());

            // Append the positive peaks
            AppendString(stringBuilder, test.Result.PositivePeaks);

            // Append the negative peaks
            AppendString(stringBuilder, test.Result.NegativePeaks);

            // Append the invalid peaks
            AppendString(stringBuilder, test.Result.InvalidPeaks);

            // Append the cartridge data
            AppendString(stringBuilder, test.Result.CartridgeData);

            // Append the end date time
            AppendString(stringBuilder, test.Result.EndDateTimeString);

            // Append the calibration outside tolerance flag
            AppendString(stringBuilder, test.Result.CalibrationOutsideTolerance.ToString());

            // Append the QC test flag
            AppendString(stringBuilder, test.Result.QcTest.ToString());

            // Append the patient information
            foreach (var patientInfo in test.Result.PatientInformation)
            {
                // Don't rewrite the patiend ID field
                if (patientInfo.Key != "PatientId")
                {
                    // Get the field from the configuration
                    field = IConfiguration.Instance.Fields.Where(x =>
                        x.Name == patientInfo.Key).FirstOrDefault();

                    // Check for a recordable field
                    if ((field != null) && (field.Policy == FieldPolicy.Record))
                    {
                        // Append the name
                        AppendString(stringBuilder, field.Name);

                        // Append the data as the field type
                        if (field.FieldType == FieldType.Text)
                        {
                            AppendString(stringBuilder, patientInfo.Value.ToString());
                        }
                        else
                        {
                            AppendString(stringBuilder, ((DateTime)patientInfo.Value).
                                ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                }
            }

            // Append a comma for the checksum
            stringBuilder.Append(',');

            // Calculate the hash for the data
            byte[] hash = MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));

            // Append it to the string
            stringBuilder.Append(BitConverter.ToString(hash).Replace("-", ""));

            // Append the new line
            stringBuilder.Append("\r\n");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Add a result to the file
        /// </summary>
        /// <param name="test">The test</param>
        public override void AddResult(ITest test)
        {
            lock (sampleIds)
            {
                // Append the results to the file
                sampleIds[test.Result.SampleId] = ILocalFileSystem.Instance.AppendTextFile("Results",
                    ResultText(test));
            }
        }

        /// <summary>
        /// Read the next result from the file
        /// </summary>
        /// <returns></returns>
        public override ITest NextResult()
        {
            // Initialsie the return value;
            ITest test = null;

            // Calculate the position
            long position = (positions.Count == 0) ? 0 : positions.Peek();
            long previousPosition;

            do
            {
                // Update the previous position
                previousPosition = position;

                // Read the next matching line
                string result = ILocalFileSystem.Instance.ReadNextLine("Results", ref position, regex);

                // Check for the end of the file
                if (result == null)
                {
                    return null;
                }

                // Get the test object
                test = TestFromString(result);

                // Update the sampleIds for a valid test
                if (test != null)
                {
                    // Push the position
                    positions.Push(position);

                    lock (sampleIds)
                    {
                        // Update the sample IDs if there is no regex being used
                        if ((regex == null) &&
                            (sampleIds.ContainsKey(test.Result.SampleId) == false))
                        {
                            sampleIds[test.Result.SampleId] = previousPosition;
                        }
                    }
                }
            }
            while (test == null);

            return test;
        }

        /// <summary>
        /// Reset to the start of the file
        /// </summary>
        public override void Reset()
        {
            // Clear the positions stack
            positions.Clear();
        }

        /// <summary>
        /// Append a quoted string value to the string builder
        /// </summary>
        /// <param name="stringBuilder">The string builder</param>
        /// <param name="value">The string</param>
        private static void AppendString(StringBuilder stringBuilder, string value)
        {
            // Append a comma if necessary
            if (stringBuilder.Length != 0)
            {
                stringBuilder.Append(',');
            }

            // Append the first quote
            stringBuilder.Append('"');

            // Check for a value and replace any quotes
            if (string.IsNullOrEmpty(value) == false)
            {
                stringBuilder.Append(value.Replace("\"", "\"\""));
            }

            // Append the final quote
            stringBuilder.Append('"');
        }

        /// <summary>
        /// Get or create a test object from the passed result
        /// </summary>
        /// <param name="value">The result</param>
        /// <returns>The test object</returns>
        private static ITest TestFromString(string value)
        {
            // Check for a null or empty value
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            // Tokenise the string
            var tokens = new List<string>();
            int position = -1;

            // Loop to the end of the string
            while (++position < value.Length)
            {
                // Check for a string value
                if (value[position] == '"')
                {
                    // Read a string value
                    int startPosition = position + 1;

                    while (++position < value.Length)
                    {
                        // Check for the end of the string value
                        if ((value[position] == '"') &&
                            ((++position >= value.Length) || (value[position] == ',')))
                        {
                            // Add the string value
                            tokens.Add(value.Substring(
                                startPosition, (position - 1) - startPosition).Replace("\"\"", "\""));

                            break;
                        }
                    }
                }
                else
                {
                    // Read an integer
                    int startPosition = position;

                    // Loop to the end of the string
                    while (++position < value.Length)
                    {
                        // Check for a comma deliter
                        if (value[position] == ',')
                        {
                            // Add the integer value
                            tokens.Add(value.Substring(startPosition, position - startPosition));

                            break;
                        }
                    }

                    // Check for the end of the line
                    if (position == value.Length)
                    {
                        tokens.Add(value.Substring(startPosition, position - startPosition));
                    }
                }
            }

            // Throw if the string format is wrong
            if (tokens.Count < 14)
            {
                throw new ApplicationException("Invalid test result string: " + value);
            }

            // Get the sample ID
            string sampleId = tokens[0].StringValue();

            // Get the test from the tests
            var test = ITests.Instance.Where(x => x.Result.SampleId == sampleId).FirstOrDefault();

            // Check for a null test object
            if (test == null)
            {
                // Create a new test
                test = new Test()
                {
                    LockingUserId = uint.Parse(tokens[2].StringValue()),
                    Result = new TestResult()
                    {
                        SampleId = tokens[0].StringValue(),
                        PatientInformation = new Dictionary<string, object>()
                        {
                            { "PatientId" , tokens[1].StringValue() }
                        },
                        StartDateTime = DateTime.Parse(tokens[3].StringValue()),
                        AssayName = tokens[4].StringValue(),
                        AssayVersion = int.Parse(tokens[5].StringValue()),
                        Outcome = tokens[6].StringValue().TestOutcomeValue(),
                        PositivePeaks = tokens[7].StringValue(),
                        NegativePeaks = tokens[8].StringValue(),
                        InvalidPeaks = tokens[9].StringValue(),
                        CartridgeData = tokens[10].StringValue(),
                        EndDateTime = DateTime.Parse(tokens[11].StringValue()),
                        CalibrationOutsideTolerance = tokens[12].BooleanValue(),
                        QcTest = tokens[13].BooleanValue(),
                    },
                };
            }
            // Check for a serialized test object with no patient information
            else if (test.Result.PatientInformation == null)
            {
                test.LockingUserId = uint.Parse(tokens[2].StringValue());
                test.Result.PatientInformation = new Dictionary<string, object>()
                {
                    { "PatientId" , tokens[1].StringValue() }
                };
            }

            // Loop through the remaining parameters in pairs, adding them to patient information
            int index;

            for (index = 14; index < (tokens.Count - 1); index += 2)
            {
                // Get the field from the configuration
                var field = IConfiguration.Instance.Fields.Where(x => 
                    x.Name == tokens[index].StringValue()).FirstOrDefault();

                // Check for obsolete fields
                if (field != null)
                {
                    // Parse the data as the field type
                    if (field.FieldType == FieldType.Text)
                    {
                        test.Result.PatientInformation[field.Name] = tokens[index + 1].StringValue();
                    }
                    else
                    {
                        test.Result.PatientInformation[field.Name] = DateTime.Parse(tokens[index + 1].StringValue());
                    }
                }
            }

            // Check for an MD5 hash
            if (index < tokens.Count)
            {
                // Extract the hashed data
                var hashedData = value.Substring(0, value.LastIndexOf(',') + 1);

                // Calculate the hash for the data
                byte[] hash = MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(hashedData));

                // Check the hashes match
                if (tokens[index] != BitConverter.ToString(hash).Replace("-", ""))
                {
                    // The hashes do not match so return null
                    return null;
                }
            }

            return test;
        }

        /// <summary>
        /// Update the regex based on the filter values
        /// </summary>
        private void UpdateRegex()
        {
            // Initialise the new regex
            Regex newRegex = null;

            if ((startDateTime.HasValue) ||
                (lockingUserId != 0) ||
                (sampleId != null) ||
                (assayName != null))
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append("^");

                if (sampleId != null)
                {
                    stringBuilder.Append("\"" + sampleId + "\"");
                }
                else
                {
                    stringBuilder.Append("\"[^\"]*\"");
                }

                stringBuilder.Append(",\"[^\"]*\"");

                if (lockingUserId != 0)
                {
                    stringBuilder.Append("," + lockingUserId);
                }
                else
                {
                    stringBuilder.Append(",\\d+");
                }

                if (startDateTime.HasValue)
                {
                    stringBuilder.Append(",\"" + startDateTime.Value.ToString("yyyy-MM-dd") + "[^\"]*\"");
                }
                else
                {
                    stringBuilder.Append(",\"[^\"]*\"");
                }

                if (assayName != null)
                {
                    stringBuilder.Append(",\"" + assayName + "\"");
                }
                else
                {
                    stringBuilder.Append(",\"[^\"]*\"");
                }

                newRegex = new Regex(stringBuilder.ToString(), RegexOptions.IgnoreCase);
            }

            // If the regex has changed then reset the position
            if (((newRegex == null) && (regex != null)) ||
                ((newRegex != null) && (regex == null)) ||
                ((newRegex != null) && (regex != null) && (newRegex.ToString() != regex.ToString())))
            {
                regex = newRegex;

                Reset();
            }
        }
    }
}
