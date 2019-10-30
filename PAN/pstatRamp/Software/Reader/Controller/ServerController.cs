/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using IO.Model.Serializable;
using IO.Comms.Server;
using IO.Scripting;
using IO.FileSystem;

namespace IO.Controller
{
    /// <summary>
    /// The controller object
    /// </summary>
    partial class Controller
    {
        /// <summary>
        /// The default value notify period in milliseconds
        /// </summary>
        private static readonly int SCHEDULED_SERVER_NOTIFY_PERIOD = 300000;

        /// <summary>
        /// Peak separator character
        /// </summary>
        private static readonly char[] commaSeparator = new char[] { ',' };

        /// <summary>
        /// The total queued results
        /// </summary>
        private uint queuedResults = 0;
        
        /// <summary>
        /// The total queued results
        /// </summary>
        public override uint QueuedResults
        {
            get
            {
                return queuedResults;
            }
        }

        /// <summary>
        /// Server thread procedure
        /// </summary>
        private void ServerThreadProcedure()
        {
            // Create the event for server notification
            var serverNotifyEvent = new AutoResetEvent(false);
            
            // Initialise the list of unsent tests
            var unsentTests = new List<ITest>();

            // Read the unsent tests file
            var unsentTestsFile = ILocalFileSystem.Instance.ReadTextFile("UnsentTests");

            if (unsentTestsFile != null)
            {
                // Loop through the sample IDs
                foreach (var sampleId in unsentTestsFile.Split(commaSeparator))
                {
                    // Get the test for the sample ID
                    var test = IResults.Instance[sampleId];

                    // If the test exists then add it to the list
                    if (test != null)
                    {
                        unsentTests.Add(test);

                        // Increment the queued results
                        queuedResults++;
                    }
                }
            }

            // Run until aborted
            while (true)
            {
                try
                {
                    // Initialise the list of new tests
                    var newTests = new List<ITest>();

                    // Wait for a message
                    if (Comms.Server.MessageQueue.Instance.WaitOne(SCHEDULED_SERVER_NOTIFY_PERIOD))
                    {
                        // Flush the queue
                        string sampleId;

                        while ((sampleId = Comms.Server.MessageQueue.Instance.Pop()) != null)
                        {
                            // Get the test from the results
                            var test = IResults.Instance[sampleId];

                            // Check for a valid test
                            if ((test != null) && 
                                (test.Result != null) && 
                                (test.Result.Assay != null) &&
                                HasObservations(test))
                            {
                                // Add it to the list of unsent tests
                                newTests.Add(test);
                            }
                        }
                    }

                    // Check for a server
                    if (string.IsNullOrEmpty(IConfiguration.Instance.Poct1ServerUri))
                    {
                        continue;
                    }

                    // Check for unsent or new observations
                    if ((unsentTests.Count > 0) || (newTests.Count > 0))
                    {
                        try
                        {
                            // Connect to the server and commence the communication
                            IClient.Instance.Connect(IConfiguration.Instance.Poct1ServerUri,
                                IConfiguration.Instance.Poct1ServerPort);
                            IClient.Instance.SendHello();
                            IClient.Instance.ReadAcknowledgement();
                            IClient.Instance.SendStatus(unsentTests.Count + newTests.Count);
                            IClient.Instance.ReadAcknowledgement();
                            IClient.Instance.ReadRequestForObservations();

                            // Loop through the unsent tests reporting any observations
                            foreach (var test in unsentTests)
                            {
                                ReportObservations(test);
                            }

                            // Loop through the new tests reporting any observations
                            foreach (var test in newTests)
                            {
                                ReportObservations(test);
                            }

                            // End the communication
                            IClient.Instance.SendEnd();
                            IClient.Instance.ReadEnd();
                            IClient.Instance.SendAcknowledgement();
                        }
                        catch (Exception e)
                        {
                            // Initialise a flag to indicate the first item in the file
                            bool first = unsentTests.Count == 0;

                            // If this sequence fails then add any new tests to the list of unsent tests
                            foreach (var test in newTests)
                            {
                                // Check the first flag
                                if (first)
                                {
                                    // Clear the flag
                                    first = false;
                                }
                                else
                                {
                                    // Append a comma
                                    ILocalFileSystem.Instance.AppendTextFile("UnsentTests", ",");
                                }

                                // Append the test to the unsent tests file
                                ILocalFileSystem.Instance.AppendTextFile("UnsentTests", 
                                    test.Result.SampleId);

                                // Add it to the list of unsent tests
                                unsentTests.Add(test);

                                // Increment the queued results
                                queuedResults++;
                            }

                            // Rethrow the error
                            throw e;
                        }
                        finally
                        {
                            IClient.Instance.Disconnect();
                        }
                    }

                    // Clear the list of unsent tests
                    unsentTests.Clear();

                    // Clear the queued results
                    queuedResults = 0;

                    // Delete any unsent tests file
                    ILocalFileSystem.Instance.WriteTextFile("UnsentTests", null);
                }
                catch (Exception)
                {
                    // Something bad happened so try again
                }
            }
        }

        /// <summary>
        /// Check if a test has observations
        /// </summary>
        /// <param name="test">The test</param>
        /// <returns>True if the test has observations, false otherwise</returns>
        private bool HasObservations(ITest test)
        {
            // Loop through the positive peaks
            if (test.Result.PositivePeaks != null)
            {
                foreach (var peakName in test.Result.PositivePeaks.Split(commaSeparator))
                {
                    // Check for a LOINC code for this peak
                    if (test.Result.Assay.Diseases.Exists(x => (x.PeakName == peakName) &&
                        (string.IsNullOrEmpty(x.Loinc) == false)))
                    {
                        return true;
                    }
                }
            }

            // Loop through the negative peaks
            if (test.Result.NegativePeaks != null)
            {
                foreach (var peakName in test.Result.NegativePeaks.Split(commaSeparator))
                {
                    // Check for a LOINC code for this peak
                    if (test.Result.Assay.Diseases.Exists(x => (x.PeakName == peakName) &&
                        (string.IsNullOrEmpty(x.Loinc) == false)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Report the observations for a test
        /// </summary>
        /// <param name="test">The test</param>
        private void ReportObservations(ITest test)
        {
            // Get the patient name, surname and date of birth
            object patientName = null;
            object patientSurname = null;
            object patientDateOfBirth = null;

            test.Result.PatientInformation.TryGetValue("Name", out patientName);
            test.Result.PatientInformation.TryGetValue("Surname", out patientSurname);
            test.Result.PatientInformation.TryGetValue("DateOfBirth",
                out patientDateOfBirth);

            // Compile the name and surname
            string patientNameAndSurname = (patientName == null) ?
                ((patientSurname == null) ? null : patientSurname as string) :
                ((patientSurname == null) ? patientName as string :
                patientName as string + " " + patientSurname as string);

            // Get the user object
            var user = IUsers.Instance.Where(x => x.ID ==
                test.LockingUserId).FirstOrDefault();

            // Initialise the list of positive diseases
            var positiveDiseases = new List<IDisease>();

            // Loop through the positive peaks
            if (test.Result.PositivePeaks != null)
            {
                foreach (var peakName in test.Result.PositivePeaks.Split(commaSeparator))
                {
                    // Get the disease for this peak
                    var disease = test.Result.Assay.Diseases.Where(x => (x.PeakName == peakName) &&
                        (string.IsNullOrEmpty(x.Loinc) == false)).FirstOrDefault();

                    if (disease != null)
                    {
                        // Add it to the list
                        positiveDiseases.Add(disease);
                    }
                }
            }

            // Initialise the list of negative diseases
            var negativeDiseases = new List<IDisease>();

            // Loop through the negative peaks
            if (test.Result.NegativePeaks != null)
            {
                foreach (var peakName in test.Result.NegativePeaks.Split(commaSeparator))
                {
                    // Get the disease for this peak
                    var disease = test.Result.Assay.Diseases.Where(x => (x.PeakName == peakName) &&
                        (string.IsNullOrEmpty(x.Loinc) == false)).FirstOrDefault();

                    if (disease != null)
                    {
                        // Add it to the list
                        negativeDiseases.Add(disease);
                    }
                }
            }

            // Send the observation
            // The sample ID is sent as the patient ID
            IClient.Instance.SendObservation(
                test.Result.StartDateTime,
                test.Result.EndDateTime,
                test.Result.CalibrationOutsideTolerance,
                test.Result.SampleId,
                positiveDiseases,
                negativeDiseases,
                test.LockingUserId,
                (user == null) ? null : user.Name,
                test.Result.SampleId,
                patientNameAndSurname,
                (patientDateOfBirth == null) ? (DateTime?)null :
                    (DateTime)patientDateOfBirth);
            IClient.Instance.ReadAcknowledgement();
        }
    }
}
