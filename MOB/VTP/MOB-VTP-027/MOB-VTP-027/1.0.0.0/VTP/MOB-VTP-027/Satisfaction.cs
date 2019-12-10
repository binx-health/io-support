using System;
using System.Collections.Generic;

using AtlasGenetics.IOReader.TestHarness;


namespace AtlasGenetics.MOB_VTP_027
{
    public static class Satisfaction
    {
        public static SatisfactionComponent RequirementSatisfaction = new SatisfactionComponent(
            "MOB-VTP-027",
            "Purpose\r\n" +
            "MOB-VTP-027 verifies that the io Reader determines the correct disease status from the captured voltammograms. The analysis of the captured voltammograms is performed by software located on the reader (PAN-D-051) which contains a generic implementation, this is made concrete by the parameters defined in the assay (MOB-D-019).\r\n" +
            "Scope\r\n" +
            "Verification is restricted to the operation of the software under the parameters of the assay and no attempt is made (or required) to verify the software with different parameters.\r\n" +
            "Methodology\r\n" +
            "To demonstrate that the functionality under test meets the requirements a set of test cases that exercise the functionality under test are derived. Each test case shall define a set of inputs and set of expected outputs with a test case only being passed if the observed outcomes match the expected outcomes.  Test cases are created by a top down division of each requirement into child components.  The division is continued until each component is sufficiently small that it can be demonstrated with a simple test case.  Each division of a parent into child components is justified with an argument that the division into child components is complete such that the satisfaction of all children implies satisfaction of the parent.  Thus, satisfaction of the requirements can be demonstrated by the passing of all the derived test cases.\r\n" +
            "Practicalities\r\n" +
            "Due to the nature of the biological and chemical processes that generate the voltammograms it is not practicable to generate/acquire specimens that fully exercise the functionality.  In practice, samples can be generated/acquired that exhibit macro properties such as a disease being present or not but not micro properties such as the exact numerical values of the voltammogram. Error conditions such as missing control peaks can also not be created practicably.\r\n" +
            "Due to the inability to generate samples that exercise all the functionality to be tested the verification of the functionality is performed by making use of the virtual reader environment (PAN-D-265).\r\n" +
            "The IOReaderTester (PAN-D-266) contains a firmware simulator that simulates the behaviour of the firmware to the io Reader software under test via the serial interface, this allows arbitrary voltammograms to be presented to the io Reader Software for analysis.  Voltammograms will be constructed that exercise and test all the functionality under test within the io Reader Software, each case will be executed by having the IOReaderTester start the CTNG test within the io Reader Software via the UI (utilizing the virtual environments ability to send mouse and keyboard input) and then using the firmware simulator to send the voltammograms to the io Reader software.  Voltammogram resolution within the firmware is 0.3814697265625 and as such all test data will need to be quantized to this resolution.\r\n" +
            "Since the functionality under test is fully contained within the io Reader software (PAN-D-051) and the virtual environment executes the deliverable binary on the same host OS, it is not considered credible that the software running on the io Reader hardware could behave differently to that running in the virtual environment but still successfully detect disease status to the level necessary to pass the clinical trial.\r\n" +
            "Test cases are constructed to exercise each of the significant decisions of the functionality under test.Where decisions are based on comparison of a numerical value to a threshold, input is constructed such that the values used to make the decision are close(reasonably practicably) to the threshold, this then verifies both the decision and the threshold.Voltammogram resolution within the firmware is 0.3814697265625(due to ADC quantisation) and as such all test data is limited this resolution.\r\n" +
            "Functionality is verified with the parameters and peaks defined by the CT / NG assay and as such each channel and peak must be threshold tested individually.Channel 2 has no peaks defined so the algorithm is not run on channel 2.\r\n" +
            "The functionality under test is split into two parts; the algorithm, as defined in Appendix 1 of 21011-SSD-001, which addresses MODS210, MODS220 and MODS230 and the logic which addresses MODS300.\r\n" +
            "The output of the algorithm is only visible to the user as a disease status after the logic has been applied, this does not give the granularity necessary to verify the algorithm implementation so the log file is checked to determine the intermediary output of the algorithm.\r\n" +
            "The expected output of the algorithm is determined by an independent reference implementation in Octave which was derived from the algorithm description in 21011-SSD-001. This was generated by an independent author in a different language and run in a different runtime environment so it is not considered credible that the two implementations would behave the same unless both are correct.\r\n" +
            "It is not considered credible that the algorithm implementation could correctly calculate the values on which decisions are based without correctly conforming to the algorithm definition and as such adherence to the algorithm definition at each step is demonstrated by test cases that threshold test a decision based on the output of that step.",
            new List<SatisfactionComponent>
            {
                new SatisfactionComponent(
                    "MODS210",
                    "Smoothing and baseline fit (MODS210) is demonstrated by test cases that demonstrate flatline detection (FL), Signal quality determination (SQ), Baseline fit (BF) and Baseline quality determination (BQ).",
                    new List<SatisfactionComponent>
                    {
                        new SatisfactionComponent(
                            "FL",
                            "Flatline detection is demonstrated by flatline detection on each channel that has peaks defined. [Step 1 of multipeak1()]",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CH1",
                                        "Signal is considered a flatline if and only if all values are between -50.0 and 50.0, satisfaction is demonstrated by test cases that demonstrate a single value above the positive threshold or a single value below the negative threshold prevent flatline."),
                                    new Tuple<string, string>(
                                        "CH3",
                                        "Signal is considered a flatline if and only if all values are between -50.0 and 50.0, satisfaction is demonstrated by test cases that demonstrate a single value above the positive threshold or a single value below the negative threshold prevent flatline."),
                                    new Tuple<string, string>(
                                        "CH4",
                                        "Signal is considered a flatline if and only if all values are between -50.0 and 50.0, satisfaction is demonstrated by test cases that demonstrate a single value above the positive threshold or a single value below the negative threshold prevent flatline."),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "All values are 0.0 => FlatLine.",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "All values are 49.9725341796875 => FlatLine.",
                                        null),
                                    new SatisfactionComponent(
                                        "3",
                                        "All values are -49.9725341796875 => FlatLine.",
                                        null),
                                    new SatisfactionComponent(
                                        "4",
                                        "All values are 0.0 except a single value of 50.35400390625 => NOT FlatLine.",
                                        null),
                                    new SatisfactionComponent(
                                        "5",
                                        "All values are 0.0 except a single value of -50.35400390625 => NOT FlatLine.",
                                        null)
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "SQ",
                            "Signal quality determination is demonstrated by signal quality determination on each channel that has peaks defined. [Steps 2 & 3 of multipeak1()]",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CH1",
                                        "Signal quality is considered acceptable if variance between the smoothed data and pre-smoothed data is less than 1000.0, satisfaction is demonstrated by test cases that demonstrate behaviour when variance between the smoothed data and pre-smoothed data is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "CH3",
                                        "Signal quality is considered acceptable if variance between the smoothed data and pre-smoothed data is less than 1000.0, satisfaction is demonstrated by test cases that demonstrate behaviour when variance between the smoothed data and pre-smoothed data is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "CH4",
                                        "Signal quality is considered acceptable if variance between the smoothed data and pre-smoothed data is less than 1000.0, satisfaction is demonstrated by test cases that demonstrate behaviour when variance between the smoothed data and pre-smoothed data is above and below the threshold."),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "Variance between the smoothed data and pre-smoothed data is greater than 1000.0 => Signal quality check failed.",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "Variance between the smoothed data and pre-smoothed data is less than 1000.0 => Signal quality check passed.",
                                        null),
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "BF",
                            "Baseline fit is demonstrated by baseline fit on each channel that has peaks defined. [Steps 4 & 5 of multipeak1()]",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CH1",
                                        "Baseline succeeds if polynomial fit (matrix inversion) succeeds at each iteration and the fit 'converges' in 100 or less cycles, satisfaction is demonstrated by test cases that demonstrate polynomial fit failure and non-convergence of the baseline."),
                                    new Tuple<string, string>(
                                        "CH3",
                                        "Baseline succeeds if polynomial fit (matrix inversion) succeeds at each iteration and the fit 'converges' in 100 or less cycles, satisfaction is demonstrated by test cases that demonstrate polynomial fit failure and non-convergence of the baseline."),
                                    new Tuple<string, string>(
                                        "CH4",
                                        "Baseline succeeds if polynomial fit (matrix inversion) succeeds at each iteration and the fit 'converges' in 100 or less cycles, satisfaction is demonstrated by test cases that demonstrate polynomial fit failure and non-convergence of the baseline."),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        null,
                                        "With equally spaced points (in x) and a polynomial degree of 6 both failure cases are unreachable.",
                                        null),
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "BQ",
                            "Baseline quality determination is demonstrated by baseline quality determination on each channel that has peaks defined. [Steps 6 & 7 of multipeak1()]",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CH1",
                                        "Baseline quality is considered acceptable if the variance between the fitted polynomial and parts of the signal it was fitted to is less than 1000.0, satisfaction is demonstrated by test cases that demonstrate behaviour when variance is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "CH3",
                                        "Baseline quality is considered acceptable if the variance between the fitted polynomial and parts of the signal it was fitted to is less than 1000.0, satisfaction is demonstrated by test cases that demonstrate behaviour when variance is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "CH4",
                                        "Baseline quality is considered acceptable if the variance between the fitted polynomial and parts of the signal it was fitted to is less than 1000.0, satisfaction is demonstrated by test cases that demonstrate behaviour when variance is above and below the threshold."),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "Variance between fitted polynomial and signal parts if was fitted to is greater than 1000.0 => Baseline quality check failed.",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "Variance between fitted polynomial and signal parts if was fitted to is less than 1000.0 => Baseline quality check passed.",
                                        null),
                                }
                            )
                        )
                    }
                ),
                new SatisfactionComponent(
                    "MODS220",
                    "Search for Gaussian peaks (MODS220) is demonstrated by test cases that demonstrate Peak detection (PD) and Gauss convergence (GC).",
                    new List<SatisfactionComponent>
                    {
                        new SatisfactionComponent(
                            "PD",
                            "Peak detection is demonstrated by peak detection for each peak defined. [Step 1 of gaussfit() in Step 8 of multipeak1()]",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CTIC",
                                        "Peak fit is only attempted if average power is greater than 25.0, satisfaction is demonstrated by test cases that demonstrate behaviour when average power is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "CT",
                                        "Peak fit is only attempted if average power is greater than 25.0, satisfaction is demonstrated by test cases that demonstrate behaviour when average power is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "NG1IC",
                                        "Peak fit is only attempted if average power is greater than 25.0, satisfaction is demonstrated by test cases that demonstrate behaviour when average power is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "NG1",
                                        "Peak fit is only attempted if average power is greater than 25.0, satisfaction is demonstrated by test cases that demonstrate behaviour when average power is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "NG2IC",
                                        "Peak fit is only attempted if average power is greater than 25.0, satisfaction is demonstrated by test cases that demonstrate behaviour when average power is above and below the threshold."),
                                    new Tuple<string, string>(
                                        "NG2",
                                        "Peak fit is only attempted if average power is greater than 25.0, satisfaction is demonstrated by test cases that demonstrate behaviour when average power is above and below the threshold."),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "Average power is less than 25.0 => Fit not attempted.",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "Average power is greater than 25.0 => Fit attempted.",
                                        null),
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "GC",
                            "Gauss convergence check is demonstrated by gauss convergence for each peak defined. [Steps 2 to 4 of gaussfit() in Step 8 of multipeak1()]",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CTIC",
                                        "Gauss fit converges if variance stabilises in less than 500 iterations, satisfaction is demonstrated by test cases that demonstrate variance stabilising in less than 500 iterations and variance not stabilising in less than 500 iterations."),
                                    new Tuple<string, string>(
                                        "CT",
                                        "Gauss fit converges if variance stabilises in less than 500 iterations, satisfaction is demonstrated by test cases that demonstrate variance stabilising in less than 500 iterations and variance not stabilising in less than 500 iterations."),
                                    new Tuple<string, string>(
                                        "NG1IC",
                                        "Gauss fit converges if variance stabilises in less than 500 iterations, satisfaction is demonstrated by test cases that demonstrate variance stabilising in less than 500 iterations and variance not stabilising in less than 500 iterations."),
                                    new Tuple<string, string>(
                                        "NG1",
                                        "Gauss fit converges if variance stabilises in less than 500 iterations, satisfaction is demonstrated by test cases that demonstrate variance stabilising in less than 500 iterations and variance not stabilising in less than 500 iterations."),
                                    new Tuple<string, string>(
                                        "NG2IC",
                                        "Gauss fit converges if variance stabilises in less than 500 iterations, satisfaction is demonstrated by test cases that demonstrate variance stabilising in less than 500 iterations and variance not stabilising in less than 500 iterations."),
                                    new Tuple<string, string>(
                                        "NG2",
                                        "Gauss fit converges if variance stabilises in less than 500 iterations, satisfaction is demonstrated by test cases that demonstrate variance stabilising in less than 500 iterations and variance not stabilising in less than 500 iterations."),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        null,
                                        "With 500 iterations the failure case is unreachable.",
                                        null),
                                }
                            )
                        ),
                    }
                ),
                new SatisfactionComponent(
                    "MODS230",
                    "Qualify peaks for noise, position, size (MODS230) can be demonstrated by test cases that demonstrate Peak qualification (PQ)).",
                    new List<SatisfactionComponent>
                    {
                        new SatisfactionComponent(
                            "PQ",
                            "Peak qualification is demonstrated by peak qualification for each peak defined [Steps 5 & 6 of gaussfit() in Step 8 of multipeak1() & Step 9 of multipeak1()]",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CTIC",
                                        "Peak is qualified as false, positive or negative based on variance, position, height and width.\r\n" +
                                        " - Peak is false if variance greater than Max(25.0, (0.04 * Height)^2), satisfaction is demonstrated by test cases that demonstrate variance greater and less than tolerance when tolerance is absolute and when tolerance is based on height (V1 to V4).\r\n" +
                                        " - Peak is negative if position less than 168.0 or greater than 232.0, satisfaction is demonstrated by test cases that demonstrate position less than lower threshold, greater than higher threshold and between the two thresholds (P1 to P4).\r\n" +
                                        " - Peak is negative if height less than 90.0, satisfaction is demonstrated by test cases that demonstrate position above and below threshold (H1 & H2).\r\n" +
                                        " - Peak is negative if width less than 90.0 or greater than 160, satisfaction is demonstrated by test cases that demonstrate width less than lower threshold, greater than higher threshold and between the two thresholds (W1 to W4).\r\n"),
                                    new Tuple<string, string>(
                                        "CT",
                                        " - Peak is false if variance greater than Max(25.0, (0.04 * Height)^2), satisfaction is demonstrated by test cases that demonstrate variance greater and less than tolerance when tolerance is absolute and when tolerance is based on height (V1 to V4).\r\n" +
                                        " - Peak is negative if position less than 330.0 or greater than 450.0, satisfaction is demonstrated by test cases that demonstrate position less than lower threshold, greater than higher threshold and between the two thresholds (P1 to P4).\r\n" +
                                        " - Peak is negative if height less than 115.0, satisfaction is demonstrated by test cases that demonstrate position above and below threshold (H1 & H2).\r\n" +
                                        " - Peak is negative if width less than 90.0 or greater than 160, satisfaction is demonstrated by test cases that demonstrate width less than lower threshold, greater than higher threshold and between the two thresholds (W1 to W4).\r\n"),
                                    new Tuple<string, string>(
                                        "NG1IC",
                                        " - Peak is false if variance greater than Max(25.0, (0.04 * Height)^2), satisfaction is demonstrated by test cases that demonstrate variance greater and less than tolerance when tolerance is absolute and when tolerance is based on height (V1 to V4).\r\n" +
                                        " - Peak is negative if position less than 168.0 or greater than 232.0, satisfaction is demonstrated by test cases that demonstrate position less than lower threshold, greater than higher threshold and between the two thresholds (P1 to P4).\r\n" +
                                        " - Peak is negative if height less than 90.0, satisfaction is demonstrated by test cases that demonstrate position above and below threshold (H1 & H2).\r\n" +
                                        " - Peak is negative if width less than 90.0 or greater than 160, satisfaction is demonstrated by test cases that demonstrate width less than lower threshold, greater than higher threshold and between the two thresholds (W1 to W4).\r\n"),
                                    new Tuple<string, string>(
                                        "NG1",
                                        " - Peak is false if variance greater than Max(25.0, (0.04 * Height)^2), satisfaction is demonstrated by test cases that demonstrate variance greater and less than tolerance when tolerance is absolute and when tolerance is based on height (V1 to V4).\r\n" +
                                        " - Peak is negative if position less than 330.0 or greater than 450.0, satisfaction is demonstrated by test cases that demonstrate position less than lower threshold, greater than higher threshold and between the two thresholds (P1 to P4).\r\n" +
                                        " - Peak is negative if height less than 50.0, satisfaction is demonstrated by test cases that demonstrate position above and below threshold (H1 & H2).\r\n" +
                                        " - Peak is negative if width less than 90.0 or greater than 160, satisfaction is demonstrated by test cases that demonstrate width less than lower threshold, greater than higher threshold and between the two thresholds (W1 to W4).\r\n"),
                                    new Tuple<string, string>(
                                        "NG2IC",
                                        " - Peak is false if variance greater than Max(25.0, (0.04 * Height)^2), satisfaction is demonstrated by test cases that demonstrate variance greater and less than tolerance when tolerance is absolute and when tolerance is based on height (V1 to V4).\r\n" +
                                        " - Peak is negative if position less than 168.0 or greater than 232.0, satisfaction is demonstrated by test cases that demonstrate position less than lower threshold, greater than higher threshold and between the two thresholds (P1 to P4).\r\n" +
                                        " - Peak is negative if height less than 90.0, satisfaction is demonstrated by test cases that demonstrate position above and below threshold (H1 & H2).\r\n" +
                                        " - Peak is negative if width less than 90.0 or greater than 160, satisfaction is demonstrated by test cases that demonstrate width less than lower threshold, greater than higher threshold and between the two thresholds (W1 to W4).\r\n"),
                                    new Tuple<string, string>(
                                        "NG2",
                                        " - Peak is false if variance greater than Max(25.0, (0.04 * Height)^2), satisfaction is demonstrated by test cases that demonstrate variance greater and less than tolerance when tolerance is absolute and when tolerance is based on height (V1 to V4).\r\n" +
                                        " - Peak is negative if position less than 330.0 or greater than 450.0, satisfaction is demonstrated by test cases that demonstrate position less than lower threshold, greater than higher threshold and between the two thresholds (P1 to P4).\r\n" +
                                        " - Peak is negative if height less than 45.0, satisfaction is demonstrated by test cases that demonstrate position above and below threshold (H1 & H2).\r\n" +
                                        " - Peak is negative if width less than 90.0 or greater than 160, satisfaction is demonstrated by test cases that demonstrate width less than lower threshold, greater than higher threshold and between the two thresholds (W1 to W4).\r\n"),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "V1",
                                        "Peak variance greater than threshold from height => false.",
                                        null),
                                    new SatisfactionComponent(
                                        "V2",
                                        "Peak variance less than threshold not from height => positive.",
                                        null),
                                    new SatisfactionComponent(
                                        "V3",
                                        "Peak variance greater than threshold from absolute => false.",
                                        null),
                                    new SatisfactionComponent(
                                        "V4",
                                        "Peak variance less than threshold from absolute => positive.",
                                        null),
                                    new SatisfactionComponent(
                                        "P1",
                                        "Peak position less than threshold => negative.",
                                        null),
                                    new SatisfactionComponent(
                                        "P2",
                                        "Peak position greater than threshold => positive.",
                                        null),
                                    new SatisfactionComponent(
                                        "P3",
                                        "Peak position greater than threshold => negative.",
                                        null),
                                    new SatisfactionComponent(
                                        "P4",
                                        "Peak position less than threshold => positive.",
                                        null),
                                    new SatisfactionComponent(
                                        "H1",
                                        "Peak height less than threshold => negative.",
                                        null),
                                    new SatisfactionComponent(
                                        "H2",
                                        "Peak height greater than threshold => positive.",
                                        null),
                                    new SatisfactionComponent(
                                        "W1",
                                        "Peak width less than threshold => negative.",
                                        null),
                                    new SatisfactionComponent(
                                        "W2",
                                        "Peak width greater than threshold => positive.",
                                        null),
                                    new SatisfactionComponent(
                                        "W3",
                                        "Peak width greater than threshold => negative.",
                                        null),
                                    new SatisfactionComponent(
                                        "W4",
                                        "Peak width less than threshold => positive.",
                                        null)
                                }
                            )
                        ),
                    }
                ),
                new SatisfactionComponent
                (
                    "MODS300",
                    "Functionality is verified with the diseases, peaks and logic defined by the CT/NG assay, 3 channels are used with each channel having 2 peaks CH1 => (CTIC, CT), CH2 => (NG1IC, NG1), CH3 => (NG2IC, NG2).\r\n" +
                    "Input to the logic is the status of each channel (flatline or not) and the status of each peak (false, negative, positive) if a channel is flatline then all peaks within it must be false. For each channel there are 10 input states (1 + 3 x 3) giving 1000 total input states (10^3).\r\n" +
                    "It is not practicable to test all input states so a representative subset is chosen that exercise the key decisions, with the expected output for each test case determined by hand.  Satisfaction is demonstrated by demonstration of Flatline decision (FL), False target decision (TF), False control decision (CF), Negative control decision (CN) and disease detected decision (DT).",
                    new List<SatisfactionComponent>
                    {
                        new SatisfactionComponent(
                            "FL",
                            "Flatline in any channel should result in all diseases being invalid, satisfaction is demonstrated by flatline decision on each channel that has peaks defined.",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CH1",
                                        "Channel 1 flatline results in all diseases being invalid, satisfaction is demonstrated by test cases that demonstrate flatline in channel with all other peaks positive(1) and flatline in channel with all other peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "CH3",
                                        "Channel 3 flatline results in all diseases being invalid, satisfaction is demonstrated by test cases that demonstrate flatline in channel with all other peaks positive(1) and flatline in channel with all other peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "CH4",
                                        "Channel 4 flatline results in all diseases being invalid, satisfaction is demonstrated by test cases that demonstrate flatline in channel with all other peaks positive(1) and flatline in channel with all other peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "Channel is flatline other channels are not flatline with positive peaks => (invalid, invalid).",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "Channel is flatline other channels are not flatline with negative peaks => (invalid, invalid).",
                                        null),
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "TF",
                            "False target peak results in all diseases being invalid, satisfaction is demonstrated by false target decision on each target peak.",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CT",
                                        "CT peak false results in all diseases being invalid, satisfaction is demonstrated by test cases that demonstrate target peak is false with all other peaks positive(1) and target peak is false with all other peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "NG1",
                                        "NG1 peak false results in all diseases being invalid, satisfaction is demonstrated by test cases that demonstrate target peak is false with all other peaks positive(1) and target peak is false with all other peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "NG2",
                                        "NG2 peak false results in all diseases being invalid, satisfaction is demonstrated by test cases that demonstrate target peak is false with all other peaks positive(1) and target peak is false with all other peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "Target peak is false all other peaks are positive => (invalid, invalid).",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "Target peak is false all other peaks are negative => (invalid, invalid).",
                                        null),
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "CF",
                            "False control peak results in all diseases being invalid unless target in same channel is positive, satisfaction is demonstrated by false control decision on each control peak.",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CTIC",
                                        "CTIC peak false results in all diseases being invalid unless CT is positive, satisfaction is demonstrated by test cases that demonstrate control peak is false and target peak is positive, all other control peaks positive and all other target peaks negative(1) and control peak is false and target peak is negative, all other control peaks positive and all other target peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "NG1IC",
                                        "NG1IC peak false results in all diseases being invalid unless NG1 is positive, satisfaction is demonstrated by test cases that demonstrate control peak is false and target peak is positive, all other control peaks positive and all other target peaks negative(1) and control peak is false and target peak is negative, all other control peaks positive and all other target peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "NG2",
                                        "NG2IC peak false results in all diseases being invalid unless NG2 is positive, satisfaction is demonstrated by test cases that demonstrate control peak is false and target peak is positive, all other control peaks positive and all other target peaks negative(1) and control peak is false and target peak is negative, all other control peaks positive and all other target peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "Control peak is false and target peak is positive, all other control peaks positive and all other target peaks negative => (detected, not detected).",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "Control peak is false and target peak is negative, all other control peaks positive and all other target peaks negative => (invalid, invalid).",
                                        null),
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "CN",
                            "Negative control peak results in all diseases being invalid unless target in same channel is positive, satisfaction is demonstrated by negative control decision on each target peak.",
                            SatisfactionComponent.Multiply(
                                new List<Tuple<string, string>>
                                {
                                    new Tuple<string, string>(
                                        "CTIC",
                                        "CTIC peak negative results in all diseases being invalid unless CT is positive, satisfaction is demonstrated by test cases that demonstrate control peak is negative and target peak is positive, all other control peaks positive and all other target peaks negative(1) and control peak is negative and target peak is negative, all other control peaks positive and all other target peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "NG1IC",
                                        "NG1IC peak negative results in all diseases being invalid unless NG1 is positive, satisfaction is demonstrated by test cases that demonstrate control peak is negative and target peak is positive, all other control peaks positive and all other target peaks negative(1) and control peak is negative and target peak is negative, all other control peaks positive and all other target peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                    new Tuple<string, string>(
                                        "NG2",
                                        "NG2IC peak negative results in all diseases being invalid unless NG2 is positive, satisfaction is demonstrated by test cases that demonstrate control peak is negative and target peak is positive, all other control peaks positive and all other target peaks negative(1) and control peak is negative and target peak is negative, all other control peaks positive and all other target peaks negative(2), it is not considered credible that different combinations of peak statuses would behave differently"),
                                },
                                new List<SatisfactionComponent>
                                {
                                    new SatisfactionComponent(
                                        "1",
                                        "Control peak is negative and target peak is positive, all other control peaks positive and all other target peaks negative => (detected, not detected).",
                                        null),
                                    new SatisfactionComponent(
                                        "2",
                                        "Control peak is negative and target peak is negative, all other control peaks positive and all other target peaks negative => (invalid, invalid).",
                                        null),
                                }
                            )
                        ),
                        new SatisfactionComponent(
                            "DT",
                            "Disease is detected if all required target peaks are positive and not detected if any required target peak is negative, satisfaction is demonstrated by disease detection for each disease.",
                            new List<SatisfactionComponent>
                            {
                                new SatisfactionComponent(
                                    "CT",
                                    "CT disease is detected if CT peak is positive, satisfaction is demonstrated by test cases that demonstrate CT peak positive, CTIC peak negative, other target peaks negative, other control peaks negative(1) and CT peak is negative all other peaks are positive(2), it is not considered credible that different combinations of peak statuses would behave differently.",
                                    new List<SatisfactionComponent>
                                    {
                                        new SatisfactionComponent(
                                            "1",
                                            "CT peak positive, CTIC peak negative, other target peaks negative, other control peaks negative => (detected, not detected).",
                                            null),
                                        new SatisfactionComponent(
                                            "2",
                                            "CT peak is negative all other peaks are positive => (not detected, detected).",
                                            null),
                                    }),
                               new SatisfactionComponent(
                                    "NG",
                                    "NG disease is detected if NG1 peak and NG2 peak are positive, satisfaction is demonstrated by test cases that demonstrate NG1 and NG2 peaks positive, NG1IC and NG2IC peaks negative, other target peaks negative, other control peaks negative(1), NG1 peak is negative all other peaks are positive(2) and NG2 peak is negative all other peaks are positive(3), it is not considered credible that different combinations of peak statuses would behave differently.",
                                    new List<SatisfactionComponent>
                                    {
                                        new SatisfactionComponent(
                                            "1",
                                            "NG1 and NG2 peaks positive, NG1IC and NG2IC peaks negative, other target peaks negative, other control peaks negative => (detected, not detected).",
                                            null),
                                        new SatisfactionComponent(
                                            "2",
                                            "NG1 peak is negative all other peaks are positive => (not detected, not detected).",
                                            null),
                                        new SatisfactionComponent(
                                            "3",
                                            "NG2 peak is negative all other peaks are positive => (not detected, not detected).",
                                            null),
                                    }),
                            }
                        ),
                    }
                )
            }
        );
    }
}
