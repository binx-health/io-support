using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using AtlasGenetics.IOReader.FirmwareSimulator;
using AtlasGenetics.IOReader.TestHarness;
using AtlasGenetics.Math;
using AtlasGenetics.SignalDatabase;

using IO.FileSystem;
using IO.Model.Serializable;


namespace AtlasGenetics.MOB_VTP_027
{
    public class AlgorithmTest : ITest
    {
        public string Name
        {
            get { return "Algorithm Test"; }
        }

        public void Execute(IOReaderTestHarness harness)
        {
            bool result = true;
            // Initialise
            if (result)
            {
                result = harness.Assert(Assertion.Equal(new string[0], "Start Reader", harness.Start(true), true));
                harness.Simulator.DeviceData = false;
            }
            if (result)
            {
                result = harness.Assert(Assertion.Equal(new string[0], "Login to reader", harness.Login("admin", "", true), true)) &&
                         harness.Assert(Assertion.Equal(new string[0], "Turn off QC", harness.TurnOffQC(), true)) &&
                         harness.Assert(Assertion.Equal(new string[0], "Set Data Policy", harness.SetDataPolicy(FieldPolicy.Ignore, FieldPolicy.Ignore, FieldPolicy.Ignore, FieldPolicy.Ignore, FieldPolicy.Ignore, FieldPolicy.Ignore, FieldPolicy.Ignore), true)) &&
                         harness.Assert(Assertion.Equal(new string[0], "Add Test", harness.AddAssay(974), true));
            }
            // Verify functionality under test
            List<Tuple<IEnumerable<string>[], string, string>> testCaseList = new List<Tuple<IEnumerable<string>[], string, string>>
            {
                // Flat line
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "FL", "CH1", "1" },
                        null,
                        new string[] { "MODS210", "FL", "CH3", "1" },
                        new string[] { "MODS210", "FL", "CH4", "1" },
                    },
                    "Algorithm.FL.1.txt",
                    "Algorithm.FL.1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "FL", "CH1", "2" },
                        null,
                        new string[] { "MODS210", "FL", "CH3", "2" },
                        new string[] { "MODS210", "FL", "CH4", "2" },
                    },
                    "Algorithm.FL.2.txt",
                    "Algorithm.FL.2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "FL", "CH1", "3" },
                        null,
                        new string[] { "MODS210", "FL", "CH3", "3" },
                        new string[] { "MODS210", "FL", "CH4", "3" },
                    },
                    "Algorithm.FL.3.txt",
                    "Algorithm.FL.3.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "FL", "CH1", "4" },
                        null,
                        new string[] { "MODS210", "FL", "CH3", "4" },
                        new string[] { "MODS210", "FL", "CH4", "4" },
                    },
                    "Algorithm.FL.4.txt",
                    "Algorithm.FL.4.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "FL", "CH1", "5" },
                        null,
                        new string[] { "MODS210", "FL", "CH3", "5" },
                        new string[] { "MODS210", "FL", "CH4", "5" },
                    },
                    "Algorithm.FL.5.txt",
                    "Algorithm.FL.5.xml"),
                // Signal Quality
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "SQ", "CH1", "1" },
                        null,
                        new string[] { "MODS210", "SQ", "CH3", "1" },
                        new string[] { "MODS210", "SQ", "CH4", "1" },
                    },
                    "Algorithm.SQ.1.txt",
                    "Algorithm.SQ.1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "SQ", "CH1", "2" },
                        null,
                        new string[] { "MODS210", "SQ", "CH3", "2" },
                        new string[] { "MODS210", "SQ", "CH4", "2" },
                    },
                    "Algorithm.SQ.2.txt",
                    "Algorithm.SQ.2.xml"),
                // Baseline Quality
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "BQ", "CH1", "1" },
                        null,
                        new string[] { "MODS210", "BQ", "CH3", "1" },
                        new string[] { "MODS210", "BQ", "CH4", "1" },
                    },
                    "Algorithm.BQ.1.txt",
                    "Algorithm.BQ.1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS210", "BQ", "CH1", "2" },
                        null,
                        new string[] { "MODS210", "BQ", "CH3", "2" },
                        new string[] { "MODS210", "BQ", "CH4", "2" },
                    },
                    "Algorithm.BQ.2.txt",
                    "Algorithm.BQ.2.xml"),
                // Peak detection
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS220", "PD", "CTIC", "1" },
                        null,
                        new string[] { "MODS220", "PD", "NG1IC", "1" },
                        new string[] { "MODS220", "PD", "NG2IC", "1" },
                    },
                    "Algorithm.PD.IC.1.txt",
                    "Algorithm.PD.IC.1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS220", "PD", "CT", "1" },
                        null,
                        new string[] { "MODS220", "PD", "NG1", "1" },
                        new string[] { "MODS220", "PD", "NG2", "1" },
                    },
                    "Algorithm.PD.TG.1.txt",
                    "Algorithm.PD.TG.1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS220", "PD", "CTIC", "2" },
                        null,
                        new string[] { "MODS220", "PD", "NG1IC", "2" },
                        new string[] { "MODS220", "PD", "NG2IC", "2" },
                    },
                    "Algorithm.PD.IC.2.txt",
                    "Algorithm.PD.IC.2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS220", "PD", "CT", "2" },
                        null,
                        new string[] { "MODS220", "PD", "NG1", "2" },
                        new string[] { "MODS220", "PD", "NG2", "2" },
                    },
                    "Algorithm.PD.TG.2.txt",
                    "Algorithm.PD.TG.2.xml"),
                // Peak qualification
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "V1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "V1" },
                        new string[] { "MODS230", "PQ", "NG2IC", "V1" },
                    },
                    "Algorithm.PQ.IC.V1.txt",
                    "Algorithm.PQ.IC.V1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "V1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "V1" },
                        new string[] { "MODS230", "PQ", "NG2", "V1" },
                    },
                    "Algorithm.PQ.TG.V1.txt",
                    "Algorithm.PQ.TG.V1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "V2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "V2" },
                        new string[] { "MODS230", "PQ", "NG2IC", "V2" },
                    },
                    "Algorithm.PQ.IC.V2.txt",
                    "Algorithm.PQ.IC.V2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "V2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "V2" },
                        new string[] { "MODS230", "PQ", "NG2", "V2" },
                    },
                    "Algorithm.PQ.TG.V2.txt",
                    "Algorithm.PQ.TG.V2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "V3" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "V3" },
                        new string[] { "MODS230", "PQ", "NG2IC", "V3" },
                    },
                    "Algorithm.PQ.IC.V3.txt",
                    "Algorithm.PQ.IC.V3.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "V3" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "V3" },
                        new string[] { "MODS230", "PQ", "NG2", "V3" },
                    },
                    "Algorithm.PQ.TG.V3.txt",
                    "Algorithm.PQ.TG.V3.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "V4" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "V4" },
                        new string[] { "MODS230", "PQ", "NG2IC", "V4" },
                    },
                    "Algorithm.PQ.IC.V4.txt",
                    "Algorithm.PQ.IC.V4.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "V4" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "V4" },
                        new string[] { "MODS230", "PQ", "NG2", "V4" },
                    },
                    "Algorithm.PQ.TG.V4.txt",
                    "Algorithm.PQ.TG.V4.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "P1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "P1" },
                        new string[] { "MODS230", "PQ", "NG2IC", "P1" },
                    },
                    "Algorithm.PQ.IC.P1.txt",
                    "Algorithm.PQ.IC.P1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "P1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "P1" },
                        new string[] { "MODS230", "PQ", "NG2", "P1" },
                    },
                    "Algorithm.PQ.TG.P1.txt",
                    "Algorithm.PQ.TG.P1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "P2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "P2" },
                        new string[] { "MODS230", "PQ", "NG2IC", "P2" },
                    },
                    "Algorithm.PQ.IC.P2.txt",
                    "Algorithm.PQ.IC.P2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "P2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "P2" },
                        new string[] { "MODS230", "PQ", "NG2", "P2" },
                    },
                    "Algorithm.PQ.TG.P2.txt",
                    "Algorithm.PQ.TG.P2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "P3" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "P3" },
                        new string[] { "MODS230", "PQ", "NG2IC", "P3" },
                    },
                    "Algorithm.PQ.IC.P3.txt",
                    "Algorithm.PQ.IC.P3.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "P3" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "P3" },
                        new string[] { "MODS230", "PQ", "NG2", "P3" },
                    },
                    "Algorithm.PQ.TG.P3.txt",
                    "Algorithm.PQ.TG.P3.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "P4" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "P4" },
                        new string[] { "MODS230", "PQ", "NG2IC", "P4" },
                    },
                    "Algorithm.PQ.IC.P4.txt",
                    "Algorithm.PQ.IC.P4.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "P4" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "P4" },
                        new string[] { "MODS230", "PQ", "NG2", "P4" },
                    },
                    "Algorithm.PQ.TG.P4.txt",
                    "Algorithm.PQ.TG.P4.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "H1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "H1" },
                        new string[] { "MODS230", "PQ", "NG2IC", "H1" },
                    },
                    "Algorithm.PQ.IC.H1.txt",
                    "Algorithm.PQ.IC.H1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "H1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "H1" },
                        new string[] { "MODS230", "PQ", "NG2", "H1" },
                    },
                    "Algorithm.PQ.TG.H1.txt",
                    "Algorithm.PQ.TG.H1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "H2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "H2" },
                        new string[] { "MODS230", "PQ", "NG2IC", "H2" },
                    },
                    "Algorithm.PQ.IC.H2.txt",
                    "Algorithm.PQ.IC.H2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "H2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "H2" },
                        new string[] { "MODS230", "PQ", "NG2", "H2" },
                    },
                    "Algorithm.PQ.TG.H2.txt",
                    "Algorithm.PQ.TG.H2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "W1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "W1" },
                        new string[] { "MODS230", "PQ", "NG2IC", "W1" },
                    },
                    "Algorithm.PQ.IC.W1.txt",
                    "Algorithm.PQ.IC.W1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "W1" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "W1" },
                        new string[] { "MODS230", "PQ", "NG2", "W1" },
                    },
                    "Algorithm.PQ.TG.W1.txt",
                    "Algorithm.PQ.TG.W1.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "W2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "W2" },
                        new string[] { "MODS230", "PQ", "NG2IC", "W2" },
                    },
                    "Algorithm.PQ.IC.W2.txt",
                    "Algorithm.PQ.IC.W2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "W2" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "W2" },
                        new string[] { "MODS230", "PQ", "NG2", "W2" },
                    },
                    "Algorithm.PQ.TG.W2.txt",
                    "Algorithm.PQ.TG.W2.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "W3" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "W3" },
                        new string[] { "MODS230", "PQ", "NG2IC", "W3" },
                    },
                    "Algorithm.PQ.IC.W3.txt",
                    "Algorithm.PQ.IC.W3.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "W3" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "W3" },
                        new string[] { "MODS230", "PQ", "NG2", "W3" },
                    },
                    "Algorithm.PQ.TG.W3.txt",
                    "Algorithm.PQ.TG.W3.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CTIC", "W4" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1IC", "W4" },
                        new string[] { "MODS230", "PQ", "NG2IC", "W4" },
                    },
                    "Algorithm.PQ.IC.W4.txt",
                    "Algorithm.PQ.IC.W4.xml"),
                new Tuple<IEnumerable<string>[], string, string>(
                    new IEnumerable<string>[]
                    {
                        new string[] { "MODS230", "PQ", "CT", "W4" },
                        null,
                        new string[] { "MODS230", "PQ", "NG1", "W4" },
                        new string[] { "MODS230", "PQ", "NG2", "W4" },
                    },
                    "Algorithm.PQ.TG.W4.txt",
                    "Algorithm.PQ.TG.W4.xml"),
            };
            foreach (Tuple<IEnumerable<string>[], string, string> testCase in testCaseList)
            {
                if (result)
                {
                    result = ExecuteAssay(harness, testCase.Item1, testCase.Item2, testCase.Item3);
                }
            }
            // Clean up
            if (result)
            {
                result = harness.Assert(Assertion.Equal(new string[0], "Logout of reader", harness.Logout(true), true)) &&
                         harness.Assert(Assertion.Equal(new string[0], "Shutdown reader", harness.StopReader(), true));
            }
        }

        private bool ExecuteAssay(IOReaderTestHarness harness, IEnumerable<string>[] traceablility, string inputData, string outputData)
        {
            string barcode = Barcode.Format("974", "9", "0000000", DateTime.Today.AddMonths(1));
            bool result = false;
            List<Voltammogram> voltammograms = null;
            if (DataFileReader.Process(
                () => new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AtlasGenetics.MOB_VTP_027.Data." + inputData)),
                true,
                out List<List<Voltammogram>> voltammogramListList,
                out List<string> log,
                out Signal2D temperature,
                out string message))
            {
                voltammograms = voltammogramListList[0];
                using (StreamReader textReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AtlasGenetics.MOB_VTP_027.Data." + outputData)))
                {
                    string specimenId = Guid.NewGuid().ToString("N");
                    AssayRunResult expectedResult = XmlSerializer.LoadXml(textReader.ReadToEnd(), "assayRunResult", AssayRunResult.Deserialize);
                    result = harness.RunAssay(
                        specimenId,
                        barcode,
                        voltammograms,
                        () => expectedResult.CheckResult(harness, traceablility, specimenId));
                }
            }
            return harness.Assert(Assertion.Equal(new string[0], "Run assay", result, true));
        }
    }
}
