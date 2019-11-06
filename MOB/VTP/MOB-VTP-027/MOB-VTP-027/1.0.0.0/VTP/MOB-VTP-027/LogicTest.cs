using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

using AtlasGenetics.IOReader.FirmwareSimulator;
using AtlasGenetics.IOReader.TestHarness;
using AtlasGenetics.Math;
using AtlasGenetics.SignalDatabase;

using IO.Model.Serializable;


namespace AtlasGenetics.MOB_VTP_027
{
    public class LogicTest : ITest
    {
        public string Name
        {
            get { return "Logic Test"; }
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
            List<Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>> testCaseList = new List<Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>>
            {
                // Flat line
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "FL", "CH1", "1" },
                    "FL",
                    "PosPos",
                    "PosPos",
                    "PosPos",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "FL", "CH1", "2" },
                    "FL",
                    "NegNeg",
                    "NegNeg",
                    "NegNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "FL", "CH3", "1" },
                    "PosPos",
                    "PosPos",
                    "FL",
                    "PosPos",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "FL", "CH3", "2" },
                    "NegNeg",
                    "NegNeg",
                    "FL",
                    "NegNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "FL", "CH4", "1" },
                    "PosPos",
                    "PosPos",
                    "PosPos",
                    "FL",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "FL", "CH4", "2" },
                    "NegNeg",
                    "NegNeg",
                    "NegNeg",
                    "FL",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                // False target
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "TF", "CT", "1" },
                    "PosFls",
                    "PosPos",
                    "PosPos",
                    "PosPos",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "TF", "CT", "2" },
                    "NegFls",
                    "NegNeg",
                    "NegNeg",
                    "NegNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "TF", "NG1", "1" },
                    "PosPos",
                    "PosPos",
                    "PosFls",
                    "PosPos",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "TF", "NG1", "2" },
                    "NegNeg",
                    "NegNeg",
                    "NegFls",
                    "NegNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "TF", "NG2", "1" },
                    "PosPos",
                    "PosPos",
                    "PosPos",
                    "PosFls",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "TF", "NG2", "2" },
                    "NegNeg",
                    "NegNeg",
                    "NegNeg",
                    "NegFls",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                // False control
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CF", "CTIC", "1" },
                    "FlsPos",
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Detected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CF", "CTIC", "2" },
                    "FlsNeg",
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CF", "NG1IC", "1" },
                    "PosNeg",
                    "PosNeg",
                    "FlsPos",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.NotDetected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CF", "NG1IC", "2" },
                    "PosNeg",
                    "PosNeg",
                    "FlsNeg",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CF", "NG2IC", "1" },
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    "FlsPos",
                    AssayRunResult.DiseaseStatus.NotDetected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CF", "NG2IC", "2" },
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    "FlsNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                // Negative control
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CN", "CTIC", "1" },
                    "NegPos",
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Detected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CN", "CTIC", "2" },
                    "NegNeg",
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CN", "NG1IC", "1" },
                    "PosNeg",
                    "PosNeg",
                    "NegPos",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.NotDetected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CN", "NG1IC", "2" },
                    "PosNeg",
                    "PosNeg",
                    "NegNeg",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CN", "NG2IC", "1" },
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    "NegPos",
                    AssayRunResult.DiseaseStatus.NotDetected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "CN", "NG2IC", "2" },
                    "PosNeg",
                    "PosNeg",
                    "PosNeg",
                    "NegNeg",
                    AssayRunResult.DiseaseStatus.Invalid,
                    AssayRunResult.DiseaseStatus.Invalid
                ),
                // Disease detection
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "DT", "CT", "1" },
                    "NegPos",
                    "NegNeg",
                    "PosNeg",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Detected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "DT", "CT", "2" },
                    "PosNeg",
                    "PosPos",
                    "PosPos",
                    "PosPos",
                    AssayRunResult.DiseaseStatus.NotDetected,
                    AssayRunResult.DiseaseStatus.Detected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "DT", "NG", "1" },
                    "PosNeg",
                    "NegNeg",
                    "NegPos",
                    "NegPos",
                    AssayRunResult.DiseaseStatus.NotDetected,
                    AssayRunResult.DiseaseStatus.Detected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "DT", "NG", "2" },
                    "PosPos",
                    "PosPos",
                    "PosNeg",
                    "PosPos",
                    AssayRunResult.DiseaseStatus.Detected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
                new Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus>(
                    new string[] { "MODS300", "DT", "NG", "3" },
                    "PosPos",
                    "PosPos",
                    "PosPos",
                    "PosNeg",
                    AssayRunResult.DiseaseStatus.Detected,
                    AssayRunResult.DiseaseStatus.NotDetected
                ),
            };
            foreach (Tuple<IEnumerable<string>, string, string, string, string, AssayRunResult.DiseaseStatus, AssayRunResult.DiseaseStatus> testCase in testCaseList)
            {
                if (result)
                {
                    result = ExecuteAssay(harness, testCase.Item1, testCase.Item2, testCase.Item3, testCase.Item4, testCase.Item5, testCase.Item6, testCase.Item7);
                }
            }
            // Clean up
            if (result)
            {
                result = harness.Assert(Assertion.Equal(new string[0], "Logout of reader", harness.Logout(true), true)) &&
                         harness.Assert(Assertion.Equal(new string[0], "Shutdown reader", harness.StopReader(), true));
            }
        }

        private bool ExecuteAssay(IOReaderTestHarness harness, IEnumerable<string> traceablility, string inputData1, string inputData2, string inputData3, string inputData4, AssayRunResult.DiseaseStatus ctResult, AssayRunResult.DiseaseStatus ngResult)
        {
            string barcode = Barcode.Format("974", "9", "0000000", DateTime.Today.AddMonths(1));
            bool result = false;
            Voltammogram[] voltammograms = new Voltammogram[]
                {
                    GetVoltammogram("Logic." + inputData1 + ".txt", 0),
                    GetVoltammogram("Logic." + inputData2 + ".txt", 1),
                    GetVoltammogram("Logic." + inputData3 + ".txt", 2),
                    GetVoltammogram("Logic." + inputData4 + ".txt", 3)
                };
            if (voltammograms[0] != null && voltammograms[1] != null && voltammograms[2] != null && voltammograms[3] != null)
            {
                string specimenId = Guid.NewGuid().ToString("N");
                result = harness.RunAssay(
                    specimenId,
                    barcode,
                    voltammograms,
                    () => CheckResult(harness, traceablility, ctResult, ngResult));
            }
            return harness.Assert(Assertion.Equal(new string[0], "Run assay", result, true));
        }

        private Voltammogram GetVoltammogram(string inputData, int channel)
        {
            if (DataFileReader.Process(
                () => new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AtlasGenetics.MOB_VTP_027.Data." + inputData)),
                true,
                out List<List<Voltammogram>> voltammogramListList,
                out List<string> log,
                out Signal2D temperature,
                out string message))
            {
                return voltammogramListList[0][channel];
            }
            return null;
        }

        private static void CheckResult(IOReaderTestHarness harness, IEnumerable<string> traceablility, AssayRunResult.DiseaseStatus ctResult, AssayRunResult.DiseaseStatus ngResult)
        {
            Bitmap expectedResultScreen = null;
            if (ctResult == AssayRunResult.DiseaseStatus.Detected && ngResult == AssayRunResult.DiseaseStatus.Detected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResultDetectedDetected;
            }
            else if (ctResult == AssayRunResult.DiseaseStatus.NotDetected && ngResult == AssayRunResult.DiseaseStatus.Detected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResultNotDetectedDetected;
            }
            else if (ctResult == AssayRunResult.DiseaseStatus.Detected && ngResult == AssayRunResult.DiseaseStatus.NotDetected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResultDetectedNotDetected;
            }
            else if (ctResult == AssayRunResult.DiseaseStatus.NotDetected && ngResult == AssayRunResult.DiseaseStatus.NotDetected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResultNotDetectedNotDetected;
            }
            else
            {
                expectedResultScreen = ReaderVM.Screens.TestResultInvalidInvalid;
            }
            harness.Assert(Assertion.Equal(traceablility, "Disease results as expected", harness.Reader.WaitForScreen(expectedResultScreen, TimeSpan.FromSeconds(0), ReaderVM.Masks.HeaderFooter, ReaderVM.Masks.InputLine1), true));
        }
    }
}
