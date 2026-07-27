#undef BENCHMARKS_OFF

using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixBenchmarkCs.Maths {
#if BENCHMARKS_OFF
    using BenchmarkAttribute = FakeBenchmarkAttribute;
#else
#endif // BENCHMARKS_OFF

    // My type.
    using TMy = Int64;

    /// <summary>
    /// SumSquares benchmark - Int64.
    /// </summary>
#if NETCOREAPP3_0_OR_GREATER && DRY_JOB
    [DryJob]
#endif // NETCOREAPP3_0_OR_GREATER && DRY_JOB
    internal class SumSquaresBenchmark_Int64 : AbstractSharedBenchmark_Int64 {

        private static TMy StaticSumScalar(TMy[] src, int srcCount) {
            TMy rt = 0; // Result.
            return rt;
        }

        [Benchmark(Baseline = true)]
        public void SumScalar() {
            //Debugger.Break();
            dstTMy = StaticSumScalar(srcArray, srcArray.Length);
            if (CheckMode) {
                baselineTMy = dstTMy;
                BenchmarkUtil.WriteItem("# SumScalar", string.Format("{0}", baselineTMy));
            }
        }

    }
}
