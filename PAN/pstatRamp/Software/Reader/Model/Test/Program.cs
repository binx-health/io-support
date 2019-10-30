/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : Chris Dawber                                                                      */
/* Reviewers :                                                                                   */
/*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using IO.FileSystem;

namespace IO.Model.Test
{
    /// <summary>
    /// Test program for the model
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Initialise the file system
            IFileSystem.Instance = new DefaultFileSystem();

            var configuration = IFileSystem.Instance.ReadTextFile("..\\..\\Data\\Configuration");
            var xmlSerializer = new XmlSerializer(typeof(Serializable.Configuration), 
                new Type[] { typeof(Serializable.PasswordRules), typeof(Serializable.Field) });

            using (var stringReader = new StringReader(configuration))
            {
                Serializable.Configuration.Instance = 
                    (Serializable.Configuration)xmlSerializer.Deserialize(stringReader);
            }

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, Serializable.Configuration.Instance);

                IFileSystem.Instance.WriteTextFile("..\\..\\Data\\Configuration", stringWriter.ToString());
            }

            xmlSerializer = new XmlSerializer(typeof(Serializable.Tests),
                new Type[] { typeof(Serializable.Test), typeof(Serializable.TestData), 
                    typeof(Serializable.TestResult), typeof(Assay) });

            Serializable.Tests.Instance = new Serializable.Tests();
            Serializable.Tests.Instance.Add(new Serializable.Test()
            {
                LockingUser = "Alan",
                PercentComplete = 0,
                TotalTimeInSeconds = 0,
                Result = new Serializable.TestResult()
                {
                    SampleId = "1",
                    PatientInformation = new Dictionary<string,object>()
                    {
                        { "PatientId", "999" },
                        { "DateOfBirth", new DateTime(1970, 1, 1) },
                    },
                    QcTest = false,
                    StartDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now,
                    Assay = new Assay() { Name = "Chlamydia", ShortName = "CT" },
                    CartridgeData = "1101101011",
                    CalibrationOutsideTolerance = true,
                    Result = Serializable.TestOutcome.Positive,
                },
                Data = new Serializable.TestData()
                {
                    Log = "etc.",
                    Readings = 2,
                    StartPotential = 99.98,
                    IncrementalPotential = 0.3,
                    Cell1Values = new double[] { 0.1, 0.2 },
                    Cell2Values = new double[] { 0.1, 0.2 },
                    Cell3Values = new double[] { 0.1, 0.2 },
                    Cell4Values = new double[] { 0.1, 0.2 },
                    AnalysisType = "Default",
                },
                Phase = "Idle",
            });

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, Serializable.Tests.Instance);

                IFileSystem.Instance.WriteTextFile("..\\..\\Data\\Tests", stringWriter.ToString());
            }

            var users = IFileSystem.Instance.ReadTextFile("..\\..\\Data\\Users");

            xmlSerializer = new XmlSerializer(typeof(Serializable.Users), new Type[] { typeof(Serializable.User) });

            using (var stringReader = new StringReader(users))
            {
                Serializable.Users.Instance = (Serializable.Users)xmlSerializer.Deserialize(stringReader);
            }

            Serializable.IUsers.Instance = new Serializable.Users();
            Serializable.IUsers.Instance.Add(new Serializable.User()
            {
                Name = "Administrator",
                Enabled = true,
                Type = Serializable.UserType.Administrator,
                Password = "password",
            });

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, Serializable.Users.Instance);

                IFileSystem.Instance.WriteTextFile("..\\..\\Data\\Users", stringWriter.ToString());
            }
        }
    }
}
