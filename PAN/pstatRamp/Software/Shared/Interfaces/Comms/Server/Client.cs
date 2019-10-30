/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using IO.Scripting;

namespace IO.Comms.Server
{
    /// <summary>
    /// Server client interface
    /// </summary>
    public abstract class IClient
    {
        /// <summary>
        /// Singleton variable
        /// </summary>
        public static IClient Instance { get; set; }

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="uri">The uri for the server</param>
        /// <param name="port">The port</param>
        public abstract void Connect(string uri, int port);

        /// <summary>
        /// Disconnect from the reader
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Send a hello message to the server
        /// </summary>
        public abstract void SendHello();

        /// <summary>
        /// Send a status message to the sserver
        /// </summary>
        /// <param name="observations">The number of observations</param>
        public abstract void SendStatus(int observations);

        /// <summary>
        /// Send an observation to the server
        /// </summary>
        /// <param name="startDateTime">The time the sample was taken</param>
        /// <param name="endDateTime">The time the analysis completed</param>
        /// <param name="calibrationOutsideTolerance">The calibration outside tolerance flag</param>
        /// <param name="patientId">The patient ID</param>
        /// <param name="positiveDiseases">The positive diseases</param>
        /// <param name="negativeDiseases">The negative diseases</param>
        /// <param name="userId">The user ID</param>
        /// <param name="userName">The user name</param>
        /// <param name="sampleId">The sample ID</param>
        /// <param name="patientName">The patient name (can be null)</param>
        /// <param name="dateOfBirth">The patient DOB (can be null)</param>
        public abstract void SendObservation(DateTime startDateTime, DateTime endDateTime,
            bool calibrationOutsideTolerance, string patientId,
            IEnumerable<IDisease> positiveDiseases, IEnumerable<IDisease> negativeDiseases,
            uint userId, string userName, string sampleId, string patientName = null,
            DateTime? dateOfBirth = null);

        /// <summary>
        /// Send an end message to the server
        /// </summary>
        public abstract void SendEnd();

        /// <summary>
        /// Send an acknowledgement to the server
        /// </summary>
        public abstract void SendAcknowledgement();

        /// <summary>
        /// Read an acknowledgement from the server
        /// </summary>
        public abstract void ReadAcknowledgement();

        /// <summary>
        /// Read a request for observations from the server
        /// </summary>
        public abstract void ReadRequestForObservations();

        /// <summary>
        /// Read an end message from the server
        /// </summary>
        public abstract void ReadEnd();
    }
}
