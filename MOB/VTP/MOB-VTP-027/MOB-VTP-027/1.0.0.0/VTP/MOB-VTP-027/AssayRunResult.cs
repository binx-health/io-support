using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

using AtlasGenetics.IOReader.TestHarness;

using IO.FileSystem;


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
            string tests = harness.GetTests();
            XmlSerializer serializer = new XmlSerializer();
            serializer.LoadXml(tests.Substring(1), "Tests");
            string log = string.Empty;
            foreach (ISerializationNode node in serializer.Root.GetChildrenNode("ITest"))
            {
                if (node.GetChildNode("Result").GetChildString("SampleId") == specimenId)
                {
                    log = node.GetChildNode("Data").GetChildString("Log");
                }
            }
            int potentiostat = -1;
            List<string>[] actualLogLists = new List<string>[4];
            for (int i = 0; i < actualLogLists.Length; ++i)
            {
                actualLogLists[i] = new List<string>();
            }
            foreach (string logLine in log.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (potentiostat == -1)
                {
                    if (logLine.Length == 36 &&
                        logLine.Substring(0, 35) == "Analysis starting for potentiostat " &&
                        int.TryParse(logLine.Substring(35, 1), out int newPotentiostat) &&
                        newPotentiostat >= 1 && newPotentiostat <= 4)
                    {
                        potentiostat = newPotentiostat;
                    }
                }
                else if (logLine.Length == 32 &&
                        logLine.Substring(0, 31) == "Analysis done for potentiostat " &&
                        int.TryParse(logLine.Substring(31, 1), out int newPotentiostat) &&
                        newPotentiostat == potentiostat)
                {
                    potentiostat = -1;
                }
                else
                {
                    actualLogLists[potentiostat - 1].Add(logLine);
                }
            }
            Bitmap expectedResultScreen = null;
            if (ctResult == DiseaseStatus.Detected && ngResult == DiseaseStatus.Detected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResultDetectedDetected;
            }
            else if (ctResult == DiseaseStatus.NotDetected && ngResult == DiseaseStatus.NotDetected)
            {
                expectedResultScreen = ReaderVM.Screens.TestResultNotDetectedNotDetected;
            }
            else
            {
                expectedResultScreen = ReaderVM.Screens.TestResultInvalidInvalid;
            }
            bool diseaserResultAsexpected = harness.Reader.WaitForScreen(expectedResultScreen, TimeSpan.FromSeconds(0), ReaderVM.Masks.HeaderFooter, ReaderVM.Masks.InputLine1);
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
