//#undef BENCHMARKS_OFF

using BenchmarkDotNet.Attributes;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public class MatrixNMultiplyBenchmark_Int32 : MatrixNMultiplyBenchmark<TMy> {

        protected override void CheckResult(string name) {
            CheckResult_Report(name, dstTMy != baselineTMy, dstTMy, baselineTMy);
        }

        protected override TMy GetCheckSum() {
            return CheckSumUtil.Calculate2D(arrayC, MatrixN, MatrixM, StrideC);
        }

        /// <summary>
        /// Basic on Array.
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
            // Matrix multiply.
            for (int i = 0; i < M; ++i) {
                for (int j = 0; j < N; ++j) {
                    int cIdx = i * strideC + j;
                    C[cIdx] = 0;
                    for (int k = 0; k < K; ++k) {
                        int aIdx = i * strideA + k;
                        int bIdx = k * strideB + j;
                        C[cIdx] += A[aIdx] * B[bIdx];
                    }
                }
            }
        }

        [Benchmark(Baseline = true)]
        public void Basic() {
            StaticBasic(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                baselineTMy = dstTMy;
                BenchmarkUtil.WriteItem("# Basic", string.Format("{0}", baselineTMy));
            }
        }

        /// <summary>Basic on Span.</summary>
        /// <inheritdoc cref="StaticBasic"/>
        public static void StaticBasicSpan(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            // Matrix multiply.
            int aIdx0 = 0;
            //int bIdx0 = 0;
            int cIdx0 = 0;
            for (int i = 0; i < M; ++i) {
                int cIdx = cIdx0;
                for (int j = 0; j < N; ++j) {
                    TMy cur = 0;
                    int aIdx = aIdx0;
                    int bIdx = j;
                    for (int k = 0; k < K; ++k) {
                        cur += A[aIdx] * B[bIdx];
                        ++aIdx;
                        bIdx += strideB;
                    }
                    C[cIdx] = cur;
                    ++cIdx;
                }
                aIdx0 += strideA;
                cIdx0 += strideC;
            }
        }

        [Benchmark]
        public void BasicSpan() {
            StaticBasicSpan(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BasicSpan");
                //BenchmarkUtil.WriteItem("# BasicSpan", string.Format("{0}", dstTMy));
            }
        }

        /// <summary>Basic on Ref.</summary>
        /// <inheritdoc cref="StaticBasic"/>
        public static void StaticBasicRef(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            // Matrix multiply.
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pB0 = ref Unsafe.AsRef(in B);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                ref TMy pC = ref pC0;
                for (int j = 0; j < N; ++j) {
                    TMy cur = 0;
                    ref TMy pA = ref pA0;
                    ref TMy pB = ref Unsafe.Add(ref pB0, j);
                    for (int k = 0; k < K; ++k) {
                        cur += pA * pB;
                        pA = ref Unsafe.Add(ref pA, 1);
                        pB = ref Unsafe.Add(ref pB, strideB);
                    }
                    pC = cur;
                    pC = ref Unsafe.Add(ref pC, 1);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        [Benchmark]
        public void BasicRef() {
            StaticBasicRef(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BasicRef");
            }
        }

        /// <summary>Transpose on Array.</summary>
        /// <inheritdoc cref="StaticBasic"/>
        public static void StaticTranspose(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            // Transpose matrix B.
            int total = K * N;
            TMy[] BTrans = ArrayPool<TMy>.Shared.Rent(total);
            try {
                MatrixUtil.Transpose(K, N, B, strideB, BTrans.AsSpan());
                // Matrix multiply.
                for (int i = 0; i < M; ++i) {
                    for (int j = 0; j < N; ++j) {
                        int cIdx = i * strideC + j;
                        C[cIdx] = 0;
                        for (int k = 0; k < K; ++k) {
                            int aIdx = i * strideA + k;
                            int bIdx = j * strideB + k;
                            C[cIdx] += A[aIdx] * BTrans[bIdx];
                        }
                    }
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(BTrans);
            }
        }

        [Benchmark]
        public void Transpose() {
            StaticTranspose(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("Transpose");
            }
        }

        /// <summary>Tile row on Array (行分块 on 数组).</summary>
        /// <inheritdoc cref="StaticBasic"/>
        public static void StaticTileRow(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            // Clear matrix C.
            //C.AsSpan().Clear();
            MatrixUtil.Fill((TMy)0, M, N, C, strideC);
            // Matrix multiply.
            for (int i = 0; i < M; ++i) {
                for (int k = 0; k < K; ++k) {
                    int aIdx = i * strideA + k;
                    for (int j = 0; j < N; ++j) {
                        int bIdx = k * strideB + j;
                        int cIdx = i * strideC + j;
                        C[cIdx] += A[aIdx] * B[bIdx];
                    }
                }
            }
        }

        [Benchmark]
        public void TileRow() {
            StaticTileRow(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRow");
            }
        }

        /// <summary>TileRow on Span.</summary>
        /// <inheritdoc cref="StaticBasic"/>
        public static void StaticTileRowSpan(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, C, strideC);
            // Matrix multiply.
            int aIdx0 = 0;
            int cIdx0 = 0;
            for (int i = 0; i < M; ++i) {
                int aIdx = aIdx0;
                int bIdx0 = 0;
                for (int k = 0; k < K; ++k) {
                    int bIdx = bIdx0;
                    int cIdx = cIdx0;
                    for (int j = 0; j < N; ++j) {
                        C[cIdx] += A[aIdx] * B[bIdx];
                        ++bIdx;
                        ++cIdx;
                    }
                    ++aIdx;
                    bIdx0 += strideB;
                }
                aIdx0 += strideA;
                cIdx0 += strideC;
            }
        }

        [Benchmark]
        public void TileRowSpan() {
            StaticTileRowSpan(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSpan");
            }
        }

        /// <summary>TileRow on Ref.</summary>
        /// <inheritdoc cref="StaticTileRow"/>
        public static void StaticTileRowRef(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, ref C, strideC);
            // Matrix multiply.
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                ref TMy pA = ref pA0;
                ref TMy pB0 = ref Unsafe.AsRef(in B);
                for (int k = 0; k < K; ++k) {
                    TMy aValue = pA;
                    ref TMy pB = ref pB0;
                    ref TMy pC = ref pC0;
                    for (int j = 0; j < N; ++j) {
                        pC += aValue * pB;
                        pB = ref Unsafe.Add(ref pB, 1);
                        pC = ref Unsafe.Add(ref pC, 1);
                    }
                    pA = ref Unsafe.Add(ref pA, 1);
                    pB0 = ref Unsafe.Add(ref pB0, strideB);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        [Benchmark]
        public void TileRowRef() {
            StaticTileRowRef(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowRef");
            }
        }

    }
}
