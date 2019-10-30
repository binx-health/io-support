using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IO.Analysis
{
    /// <summary>
    /// Enumeration of peak outcomes
    /// </summary>
    public enum PeakOutcome
    {
        Ignore,
        Rescan,
        Positive,
        Negative,
        Invalid,
    }
}
