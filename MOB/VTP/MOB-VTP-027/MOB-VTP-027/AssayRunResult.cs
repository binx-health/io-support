using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

using AtlasGenetics.ClassA.IOReader.TestHarness;
using AtlasGenetics.ClassB.IOReader.FileSystem;
using AtlasGenetics.ClassB.IOReader.Model.Serializable;


namespace AtlasGenetics.MOB_VTP_027
{
    public class AssayRunResult : ISerializable
    {
        public enum DiseaseStatus
        {
            Invalid,
            Detected,
            NotDetected
        }

        private List<string>[] expectedLogLists;
        private DiseaseStatus ctResult;
        private DiseaseStatus ngResult;

        public AssayRunResult(List<string>[] expectedLogLists, DiseaseStatus ctResult, DiseaseStatus ngResult)
        {
            this.expectedLogLists = expectedLogLists;
            this.ctResult = ctResult;
            this.ngResult = ngResult;
        }

        public AssayRunResult(ISerializationNode serializationNode)
        {
            expectedLogLists = new List<string>[4];
            for (int i = 0; i < expectedLogLists.Length; ++i)
            {
                expectedLogLists[i] = new List<string>(serializationNode.GetChildrenString("expectedLogList" + i.ToString(CultureInfo.InvariantCulture)));
            }
            ctResult = (DiseaseStatus)Enum.Parse(typeof(DiseaseStatus), serializationNode.GetChildString("ctResult"));
            ngResult = (DiseaseStatus)Enum.Parse(typeof(DiseaseStatus), serializationNode.GetChildString("ngResult"));
        }

        public void Serialize(ISerializationNode serializationNode)
        {
            for (int i = 0; i < expectedLogLists.Length; ++i)
            {
                serializationNode.AddChildren("expectedLogList" + i.ToString(CultureInfo.InvariantCulture), expectedLogLists[i]);
            }
            serializationNode.AddChild("ctResult", ctResult.ToString());
            serializationNode.AddChild("ngResult", ngResult.ToString());
        }

        public static AssayRunResult Deserialize(ISerializationNode serializationNode)
        {
            return new AssayRunResult(serializationNode);
        }

        public void CheckResult(IOReaderTestHarness harness, IEnumerable<string>[] traceablility, string specimenId)
        {
            List<string>[] actualLogLists = new List<string>[4];
            for (int i = 0; i < actualLogLists.Length; ++i)
            {
                actualLogLists[i] = new List<string>();
            }
            int potentiostat = -1;
            Test test = null;
            foreach (Test possibleTest in harness.GetTests())
            {
                if (possibleTest.Result.SampleId == specimenId)
                {
                    test = possibleTest;
                }
            }
            if (test != null)
            {
                foreach (string logLine in test.Data.Log.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (potentiostat == -1)
                    {
                        if (logLine.Length == 48 &&
                            logLine.Substring(12, 35) == "Analysis starting for potentiostat " &&
                            int.TryParse(logLine.Substring(47, 1), out int newPotentiostat) &&
                            newPotentiostat >= 1 && newPotentiostat <= 4)
                        {
                            potentiostat = newPotentiostat;
                        }
                    }
                    else if (logLine.Length == 44 &&
                            logLine.Substring(12, 31) == "Analysis done for potentiostat " &&
                            int.TryParse(logLine.Substring(43, 1), out int newPotentiostat) &&
                            newPotentiostat == potentiostat)
                    {
                        potentiostat = -1;
                    }
                    else
                    {
                        if (logLine.Length > 13)
                        {
                            actualLogLists[potentiostat - 1].Add(logLine.Substring(12));
                        }
                    }
                }
            }
            Bitmap expectedResultScreen = null;
            if (ctResult == DiseaseStatus.Detected && ngResult == DiseaseStatus.Detected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResult.DetectedDetectedBitmap;
            }
            else if (ctResult == DiseaseStatus.NotDetected && ngResult == DiseaseStatus.NotDetected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResult.NotDetectedNotDetectedBitmap;
            }
            else
            {
                expectedResultScreen = ReaderVM.Screens.TestResult.InvalidInvalidBitmap;
            }
            bool diseaserResultAsexpected = harness.Reader.WaitForScreen(expectedResultScreen, TimeSpan.FromSeconds(0), ReaderVM.Screens.TestResult.HeaderFooterMask, ReaderVM.Screens.TestResult.InputLine1Mask);
            for (int i = 0; i < expectedLogLists.Length; ++i)
            {
                if (traceablility[i] != null)
                {
                    harness.Assert(Assertion.Equal(traceablility[i], "Log as expected", actualLogLists[i], expectedLogLists[i]));
                    harness.Assert(Assertion.Equal(traceablility[i], "Disease results as expected", diseaserResultAsexpected, true));
                }
            }
        }
    }
}
