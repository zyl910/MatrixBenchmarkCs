//#undef BENCHMARKS_OFF

using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MatrixBenchmarkCs.MultiplyMatrix {
#if BENCHMARKS_OFF
    using BenchmarkAttribute = FakeBenchmarkAttribute;
#else
#endif // BENCHMARKS_OFF


    // My type.
    using TMy = Int32;

    /// <summary>
    /// Matrix N*N multiply matrix N*N benchmark - Int32.
    /// </summary>
    public class MatrixNMultiplyBenchmark_Int32: MatrixNMultiplyBenchmark<TMy> {

        protected override void CheckResult(string name) {
            CheckResult_Report(name, dstTMy != baselineTMy, dstTMy, baselineTMy);
        }

        protected override TMy GetCheckSum() {
            return default;
        }

        /// <summary>
        /// Basic - Array.
        /// </summary>
        /// <param name="M">The number of rows in matrix A (矩阵A的行数).</param>
        /// <param name="N">The number of columns in matrix B (矩阵B的列数).</param>
        /// <param name="K">The number of columns in matrix A, or the number of rows in matrix B (矩阵A的列数, 或矩阵B的行数).</param>
        /// <param name="A">Matrix A.</param>
        /// <param name="strideA">Stride of A.</param>
        /// <param name="B">Matrix B.</param>
        /// <param name="strideB">Stride of B.</param>
        /// <param name="C">Matrix C.</param>
        /// <param name="strideC">Stride of C.</param>
        public static void StaticBasic(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            // Matrix matrix multiply.
            for (int i = 0; i < M; ++i) {
                for (int j = 0; j < N; ++j) {
                    TMy cur = 0;
                    for (int k = 0; k < K; ++k) {
                        int aIdx = i * strideA + k;
                        int bIdx = k * strideB + j;
                        cur += A[aIdx] * B[bIdx];
                    }
                    int cIdx = i * strideC + j;
                    C[cIdx] = cur;
                }
            }
        }

        [Benchmark(Baseline = true)]
        public void Basic() {
            StaticBasic(N, N, N, arrayA!, N, arrayB!, N, arrayC!, N);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                baselineTMy = dstTMy;
                BenchmarkUtil.WriteItem("# Basic", string.Format("{0}", baselineTMy));
            }
        }
    }
}
