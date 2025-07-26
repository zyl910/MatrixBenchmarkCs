using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixBenchmarkCs {
    /// <summary>
    /// Abstract demo
    /// </summary>
    public abstract class AbstractBenchmark {
        /// <summary>
        /// Values for N default value.
        /// </summary>
        public static readonly int[] DefaultValuesForN = new int[] {
            //64
            32, 64
        };

        /// <summary>
        /// Is check mode.
        /// </summary>
        public bool CheckMode { get; set; }

        /// <summary>
        /// Values for N.
        /// </summary>
        public IEnumerable<int> ValuesForN { get; set; } = DefaultValuesForN;

        /// <summary>
        /// Test size.
        /// </summary>
        [ParamsSource(nameof(ValuesForN))]
        public int N { get; set; }

        /// <summary>
        /// Global setup
        /// </summary>
        public virtual void GlobalSetup() {
        }

    }
}
