using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixBenchmarkCs {
    /// <summary>
    /// Getter of LoopCount.
    /// </summary>
    internal interface ILoopCountGetter {
        /// <summary>
        /// Property LoopCount.
        /// </summary>
        long LoopCount { get; }
    }
}
