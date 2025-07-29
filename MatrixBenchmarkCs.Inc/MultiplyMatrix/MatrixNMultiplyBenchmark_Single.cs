#undef BENCHMARKS_OFF

using BenchmarkDotNet.Attributes;
using MKLNET;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif // NETCOREAPP3_0_OR_GREATER
using System.Text;
using System.Threading.Tasks;
using Zyl.VectorTraits;

namespace MatrixBenchmarkCs.MultiplyMatrix {
#if BENCHMARKS_OFF
    using BenchmarkAttribute = FakeBenchmarkAttribute;
#else
#endif // BENCHMARKS_OFF


    // My type.
    using TMy = Single;

    /// <summary>
    /// Matrix N*N multiply matrix N*N benchmark - Single.
    /// </summary>
    public class MatrixNMultiplyBenchmark_Single : MatrixNMultiplyBenchmark<TMy> {

        protected const int BLOCK_SIZE = 8;

        protected MathNet.Numerics.LinearAlgebra.Matrix<TMy>? matA;
        protected MathNet.Numerics.LinearAlgebra.Matrix<TMy>? matB;
        protected MathNet.Numerics.LinearAlgebra.Matrix<TMy>? matC;

        protected override void ArraySetup() {
            base.ArraySetup();
            matA = MathNet.Numerics.LinearAlgebra.Matrix<TMy>.Build.DenseOfRowMajor(MatrixM, MatrixK, arrayA);
            matB = MathNet.Numerics.LinearAlgebra.Matrix<TMy>.Build.DenseOfRowMajor(MatrixK, MatrixN, arrayB);
            matC = MathNet.Numerics.LinearAlgebra.Matrix<TMy>.Build.Dense(MatrixM, MatrixN);
            //matA.Multiply(matB, matC);
        }

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

#if REDUCE_MEMORY_USAGE
        [Benchmark]
        public void Basic() {
            StaticBasic(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                baselineTMy = dstTMy;
                BenchmarkUtil.WriteItem("# Basic", string.Format("{0}", baselineTMy));
                //CheckResult("Basic");
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
#endif // REDUCE_MEMORY_USAGE

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

        [Benchmark(Baseline = true)]
        public void TileRowRef() {
            StaticTileRowRef(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                //CheckResult("TileRowRef");
                baselineTMy = dstTMy;
                BenchmarkUtil.WriteItem("# TileRowRef", string.Format("{0}", baselineTMy));
            }
        }

        /// <summary>TileRow on SIMD.</summary>
        /// <inheritdoc cref="StaticTileRow"/>
        public static void StaticTileRowSimd(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
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
                    Vector<TMy> vA = new Vector<TMy>(aValue);
                    // Last.
                    int pos = N - Vector<TMy>.Count;
                    ref Vector<TMy> pBLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pB0, pos));
                    ref Vector<TMy> pCLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pC0, pos));
                    Vector<TMy> vCLast = Vector.Add(Vectors.Multiply(vA, pBLast), pCLast);
                    // SIMD for.
                    if (cntBlock >= 0) {
                        ref Vector<TMy> pB = ref Unsafe.As<TMy, Vector<TMy>>(ref pB0);
                        ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                        for (int j = 0; j < cntBlock; ++j) {
                            pC = Vector.Add(Vectors.Multiply(vA, pB), pC); // pC += vA * pB;
                            pB = ref Unsafe.Add(ref pB, 1);
                            pC = ref Unsafe.Add(ref pC, 1);
                        }
                    }
                    pCLast = vCLast; // Overrride remainder items. 
                    // Next.
                    pA = ref Unsafe.Add(ref pA, 1);
                    pB0 = ref Unsafe.Add(ref pB0, strideB);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        [Benchmark]
        public void TileRowSimd() {
            StaticTileRowSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimd");
            }
        }

#if NET9_0_OR_GREATER
        /// <summary>TileRow on SIMD Fma.</summary>
        /// <inheritdoc cref="StaticTileRow"/>
        public static void StaticTileRowSimdFma(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
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
                    Vector<TMy> vA = new Vector<TMy>(aValue);
                    // Last.
                    int pos = N - Vector<TMy>.Count;
                    ref Vector<TMy> pBLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pB0, pos));
                    ref Vector<TMy> pCLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pC0, pos));
                    Vector<TMy> vCLast = Vector.FusedMultiplyAdd(vA, pBLast, pCLast);
                    // SIMD for.
                    if (cntBlock >= 0) {
                        ref Vector<TMy> pB = ref Unsafe.As<TMy, Vector<TMy>>(ref pB0);
                        ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                        for (int j = 0; j < cntBlock; ++j) {
                            pC = Vector.FusedMultiplyAdd(vA, pB, pC); // pC += vA * pB;
                            pB = ref Unsafe.Add(ref pB, 1);
                            pC = ref Unsafe.Add(ref pC, 1);
                        }
                    }
                    pCLast = vCLast; // Overrride remainder items. 
                    // Next.
                    pA = ref Unsafe.Add(ref pA, 1);
                    pB0 = ref Unsafe.Add(ref pB0, strideB);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        [Benchmark]
        public void TileRowSimdFma() {
            StaticTileRowSimdFma(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdFma");
            }
        }
#endif // NET9_0_OR_GREATER

#if REDUCE_MEMORY_USAGE
#if NETCOREAPP3_0_OR_GREATER
        /// <summary>TileRow on SIMD Fma X86.</summary>
        /// <inheritdoc cref="StaticTileRow"/>
        public static void StaticTileRowSimdFmaX86(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated || !Fma.IsSupported) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, ref C, strideC);
            // Matrix multiply.
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                ref TMy pA = ref pA0;
                ref TMy pB0 = ref Unsafe.AsRef(in B);
                for (int k = 0; k < K; ++k) {
                    TMy aValue = pA;
                    Vector<TMy> vA = new Vector<TMy>(aValue);
                    // Last.
                    int pos = N - Vector<TMy>.Count;
                    ref Vector<TMy> pBLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pB0, pos));
                    ref Vector<TMy> pCLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pC0, pos));
                    Vector<TMy> vCLast = Vector.Add(Vectors.Multiply(vA, pBLast), pCLast);
                    // SIMD for.
                    if (cntBlock >= 0) {
                        ref Vector<TMy> pB = ref Unsafe.As<TMy, Vector<TMy>>(ref pB0);
                        ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                        for (int j = 0; j < cntBlock; ++j) {
                            // pC += vA * pB;
                            if (Vector<byte>.Count == Vector256<byte>.Count) {
                                pC = Fma.MultiplyAdd(vA.AsVector256(), pB.AsVector256(), pC.AsVector256()).AsVector();
                            } else if (Vector<byte>.Count == Vector256<byte>.Count) {
                                pC = Fma.MultiplyAdd(vA.AsVector128(), pB.AsVector128(), pC.AsVector128()).AsVector();
                            } else {
                                pC = Vector.Add(Vectors.Multiply(vA, pB), pC);
                            }
                            pB = ref Unsafe.Add(ref pB, 1);
                            pC = ref Unsafe.Add(ref pC, 1);
                        }
                    }
                    pCLast = vCLast; // Overrride remainder items. 
                    // Next.
                    pA = ref Unsafe.Add(ref pA, 1);
                    pB0 = ref Unsafe.Add(ref pB0, strideB);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        [Benchmark]
        public void TileRowSimdFmaX86() {
            StaticTileRowSimdFmaX86(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdFmaX86");
            }
        }
#endif // NETCOREAPP3_0_OR_GREATER
#endif // REDUCE_MEMORY_USAGE

#if REDUCE_MEMORY_USAGE
        /// <summary>TileRow on SIMD - Loop Unrolling 4.</summary>
        /// <inheritdoc cref="StaticTileRow"/>
        public static void StaticTileRowSimdLU4(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int LU = 2; // Loop Unrolling 4.
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
            if (0 != (cntBlock % LU) || cntBlock < 2) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            cntBlock /= LU;
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
                    Vector<TMy> vA = new Vector<TMy>(aValue);
                    // Last.
                    int pos = N - Vector<TMy>.Count;
                    ref Vector<TMy> pBLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pB0, pos));
                    ref Vector<TMy> pCLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pC0, pos));
                    Vector<TMy> vCLast = Vector.Add(Vectors.Multiply(vA, pBLast), pCLast);
                    // SIMD for.
                    if (cntBlock >= 0) {
                        ref Vector<TMy> pB = ref Unsafe.As<TMy, Vector<TMy>>(ref pB0);
                        ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                        for (int j = 0; j < cntBlock; ++j) {
                            Vector<TMy> vB0 = pB;
                            Vector<TMy> vB1 = Unsafe.Add(ref pB, 1);
                            //Vector<TMy> vB2 = Unsafe.Add(ref pB, 2);
                            //Vector<TMy> vB3 = Unsafe.Add(ref pB, 3);
                            ref Vector<TMy> pC1 = ref Unsafe.Add(ref pC, 1);
                            //ref Vector<TMy> pC2 = ref Unsafe.Add(ref pC, 2);
                            //ref Vector<TMy> pC3 = ref Unsafe.Add(ref pC, 3);
                            // pC += vA * pB;
                            pC = Vector.Add(Vectors.Multiply(vA, vB0), pC);
                            pC1 = Vector.Add(Vectors.Multiply(vA, vB1), pC1);
                            //pC2 = Vector.Add(Vectors.Multiply(vA, vB2), pC2);
                            //pC3 = Vector.Add(Vectors.Multiply(vA, vB3), pC3);
                            pB = ref Unsafe.Add(ref pB, LU);
                            pC = ref Unsafe.Add(ref pC, LU);
                        }
                    }
                    pCLast = vCLast; // Overrride remainder items. 
                    // Next.
                    pA = ref Unsafe.Add(ref pA, 1);
                    pB0 = ref Unsafe.Add(ref pB0, strideB);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        [Benchmark]
        public void TileRowSimdLU4() {
            StaticTileRowSimdLU4(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdLU4");
            }
        }
#endif // REDUCE_MEMORY_USAGE

        [Benchmark]
        public void TileRowSimdParallel() {
            int M = MatrixM;
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1);
            if (allowParallel) {
                Parallel.For(0, M, i => {
                    StaticTileRowSimd(1, MatrixN, MatrixK, ref arrayA![StrideA * i], StrideA, ref arrayB![0], StrideB, ref arrayC![StrideC * i], StrideC);
                });
            } else {
                StaticTileRowSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdParallel");
            }
        }

        [Benchmark]
        public void TileRowSimdParallel2() {
            const int batchSize = 4;
            int M = MatrixM;
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1);
            if (allowParallel) {
                int count = (M + batchSize - 1) / batchSize;
                Parallel.For(0, count
                , ParallelOptionsCPU
                , i => {
                    int idx = batchSize * i;
                    int curSize = batchSize;
                    if (curSize > M - idx) {
                        curSize = M - idx;
                    }
                    StaticTileRowSimd(curSize, MatrixN, MatrixK, ref arrayA![StrideA * idx], StrideA, ref arrayB![0], StrideB, ref arrayC![StrideC * idx], StrideC);
                });
            } else {
                StaticTileRowSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdParallel2");
            }
        }

#if REDUCE_MEMORY_USAGE
        /// <summary>BlockCopy2 on Array (块复制2 on 数组).</summary>
        /// <inheritdoc cref="StaticBasic"/>
        public static void StaticBlockCopy2(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRow(M, N, K, A, strideA, B, strideB, C, strideC);
                return;
            }
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            try {
                Span<TMy> localA = buf.AsSpan().Slice(0, local2DSize);
                Span<TMy> localB = buf.AsSpan().Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.AsSpan().Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                //#pragma omp parallel for
                for (int bi = 0; bi < blockM; bi++) {
                    for (int bj = 0; bj < blockN; bj++) {
                        // Clear localC.
                        //for (int i = 0; i < BLOCK_SIZE; i++) {
                        //    for (int j = 0; j < BLOCK_SIZE; j++) {
                        //        localC[i * BLOCK_SIZE + j] = 0;
                        //    }
                        //}
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    int aIdx = bi * BLOCK_SIZE * blockNum * BLOCK_SIZE +
                                //                  i * blockNum * BLOCK_SIZE + bk * BLOCK_SIZE + j;
                                //    int bIdx = bk * BLOCK_SIZE * blockNum * BLOCK_SIZE +
                                //                  i * blockNum * BLOCK_SIZE + bj * BLOCK_SIZE + j;
                                //    localA[i][j] = matA[aIdx];
                                //    localB[i][j] = matB[bIdx];
                                //}
                                int aIdx = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                int bIdx = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                A.AsSpan().Slice(aIdx, BLOCK_SIZE).CopyTo(localA.Slice(i * BLOCK_SIZE, BLOCK_SIZE));
                                B.AsSpan().Slice(bIdx, BLOCK_SIZE).CopyTo(localB.Slice(i * BLOCK_SIZE, BLOCK_SIZE));
                            }
                            // Block GEMM.
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    //#pragma omp simd
                                    for (int j = 0; j < BLOCK_SIZE; j++) {
                                        localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                    }
                                }
                            }
                        }
                        // Copy localC back.
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //for (int j = 0; j < BLOCK_SIZE; j++) {
                            //    int cIdx = bi * BLOCK_SIZE * blockNum * BLOCK_SIZE +
                            //                  i * blockNum * BLOCK_SIZE + bj * BLOCK_SIZE + j;
                            //    matC[cIdx] = localC[i][j];
                            //}
                            int cIdx = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            localC.Slice(i * BLOCK_SIZE, BLOCK_SIZE).CopyTo(C.AsSpan().Slice(cIdx, BLOCK_SIZE));
                        }
                    }
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        [Benchmark]
        public void BlockCopy2() {
            StaticBlockCopy2(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2");
            }
        }

        /// <summary>BlockCopy2 on Span.</summary>
        /// <inheritdoc cref="StaticBasic"/>
        public static void StaticBlockCopy2Span(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowSpan(M, N, K, A, strideA, B, strideB, C, strideC);
                return;
            }
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            try {
                Span<TMy> localA = buf.AsSpan().Slice(0, local2DSize);
                Span<TMy> localB = buf.AsSpan().Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.AsSpan().Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                int idxA, idxB, idxC;
                int idxC0;
                int idxCLocal;
                for (int bi = 0; bi < blockM; bi++) {
                    for (int bj = 0; bj < blockN; bj++) {
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            idxA = (bi * BLOCK_SIZE) * strideA + bk * BLOCK_SIZE;
                            idxB = (bk * BLOCK_SIZE) * strideB + bj * BLOCK_SIZE;
                            idxCLocal = 0;
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                idxA += strideA;
                                idxB += strideB;
                                idxCLocal += BLOCK_SIZE;
                            }
                            // Block GEMM.
                            idxA = 0;
                            idxC0 = 0;
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                idxB = 0;
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    idxC = idxC0;
                                    for (int j = 0; j < BLOCK_SIZE; j++) {
                                        //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                        localC[idxC] += localA[idxA] * localB[idxB];
                                        ++idxB;
                                        ++idxC;
                                    }
                                    ++idxA;
                                }
                                idxC0 += BLOCK_SIZE;
                            }
                        }
                        // Copy localC back.
                        idxC = (bi * BLOCK_SIZE) * strideC + bj * BLOCK_SIZE;
                        idxCLocal = 0;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            idxC += strideC;
                            idxCLocal += BLOCK_SIZE;
                        }
                    }
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        [Benchmark]
        public void BlockCopy2Span() {
            StaticBlockCopy2Span(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2Span");
            }
        }

        /// <summary>BlockCopy2 on Ref.</summary>
        /// <inheritdoc cref="StaticBlockCopy2"/>
        public static void StaticBlockCopy2Ref(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            try {
                Span<TMy> localA = buf.AsSpan().Slice(0, local2DSize);
                Span<TMy> localB = buf.AsSpan().Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.AsSpan().Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            ref TMy pACur = ref pA;
                            ref TMy pALocal = ref localA[0];
                            ref TMy pBLocal = ref localB[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                                pACur = ref Unsafe.Add(ref pACur, strideA);
                                pB = ref Unsafe.Add(ref pB, strideB);
                                pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                                pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            }
                            // Block GEMM.
                            ref TMy pACore = ref localA[0];
                            ref TMy pCCore0 = ref localC[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                ref TMy pBCore = ref localB[0];
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    ref TMy pCCore = ref pCCore0;
                                    for (int j = 0; j < BLOCK_SIZE; j++) {
                                        //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                        pCCore += pACore * pBCore;
                                        pBCore = ref Unsafe.Add(ref pBCore, 1);
                                        pCCore = ref Unsafe.Add(ref pCCore, 1);
                                    }
                                    pACore = ref Unsafe.Add(ref pACore, 1);
                                }
                                pCCore0 = ref Unsafe.Add(ref pCCore0, BLOCK_SIZE);
                            }
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        [Benchmark]
        public void BlockCopy2Ref() {
            StaticBlockCopy2Ref(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2Ref");
            }
        }
#endif // REDUCE_MEMORY_USAGE

        /// <summary>BlockCopy2 on ref SIMD.</summary>
        /// <inheritdoc cref="StaticBlockCopy2"/>
        public static void StaticBlockCopy2Simd(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize * 3];
            try {
                Span<TMy> localA = buf.Slice(0, local2DSize);
                Span<TMy> localB = buf.Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            ref TMy pACur = ref pA;
                            ref TMy pALocal = ref localA[0];
                            ref TMy pBLocal = ref localB[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                                pACur = ref Unsafe.Add(ref pACur, strideA);
                                pB = ref Unsafe.Add(ref pB, strideB);
                                pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                                pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            }
                            // Block GEMM.
                            ref TMy pACore = ref localA[0];
                            //ref TMy pCCore0 = ref localC[0];
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //ref TMy pBCore = ref localB[0];
                                ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    Vector<TMy> vA = new Vector<TMy>(pACore);
                                    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                    //}
                                    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                    pACore = ref Unsafe.Add(ref pACore, 1);
                                    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                }
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
            } finally {
                //ArrayPool<TMy>.Shared.Return(buf);
            }
        }

#if REDUCE_MEMORY_USAGE
        [Benchmark]
#endif // REDUCE_MEMORY_USAGE
        public void BlockCopy2Simd() {
            StaticBlockCopy2Simd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2Simd");
            }
        }

        /// <summary>BlockCopy2 on ref SIMD register.</summary>
        /// <inheritdoc cref="StaticBlockCopy2"/>
        public static void StaticBlockCopy2SimdRegi(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            int BLOCK_SIZE = Vector<TMy>.Count;
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowSimd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            if (8 == BLOCK_SIZE) {
                StaticBlockCopy2SimdRegi_8(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            } else {
                StaticBlockCopy2Simd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            }
        }

        private static void StaticBlockCopy2SimdRegi_8(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int BLOCK_SIZE = 8;   // On Vector<TMy>.Count = 8.
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize * 2]; // local2DSize * 3
            try {
                Span<TMy> localC = buf.Slice(0, local2DSize);
                Span<TMy> localA = buf.Slice(local2DSize * 1, local2DSize);
                //Span<TMy> localB = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            ref TMy pBBak = ref pB;
                            ref TMy pACur = ref pA;
                            ref TMy pALocal = ref localA[0];
                            //ref TMy pBLocal = ref localB[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                                //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                                pACur = ref Unsafe.Add(ref pACur, strideA);
                                //pB = ref Unsafe.Add(ref pB, strideB);
                                pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                                //pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            }
                            pB = ref pBBak;
                            Vector<TMy> b0 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b1 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b2 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b3 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b4 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b5 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b6 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b7 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            // Block GEMM.
                            ref TMy pACore = ref localA[0];
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                //for (int k = 0; k < BLOCK_SIZE; k++) {
                                //    Vector<TMy> vA = new Vector<TMy>(pACore);
                                //    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                //    //}
                                //    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                //    pACore = ref Unsafe.Add(ref pACore, 1);
                                //    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                //}
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b0), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b1), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b2), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b3), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b4), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b5), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b6), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b7), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                // Next.
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
            } finally {
                //ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        [Benchmark]
        public void BlockCopy2SimdRegi() {
            StaticBlockCopy2SimdRegi(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2SimdRegi");
            }
        }

        /// <summary>BlockCopy2 on ref SIMD register v2.</summary>
        /// <inheritdoc cref="StaticBlockCopy2"/>
        public static void StaticBlockCopy2SimdRegi2(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            int BLOCK_SIZE = Vector<TMy>.Count;
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowSimd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            if (8 == BLOCK_SIZE) {
                StaticBlockCopy2SimdRegi2_8(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            } else {
                StaticBlockCopy2Simd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            }
        }

        private static void StaticBlockCopy2SimdRegi2_8(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int BLOCK_SIZE = 8;   // On Vector<TMy>.Count = 8.
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize]; // local2DSize * 3
            do {
                Span<TMy> localC = buf.Slice(0, local2DSize);
                //Span<TMy> localA = buf.Slice(local2DSize * 1, local2DSize);
                //Span<TMy> localB = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            //ref TMy pBBak = ref pB;
                            //ref TMy pACur = ref pA;
                            //ref TMy pALocal = ref localA[0];
                            ////ref TMy pBLocal = ref localB[0];
                            //for (int i = 0; i < BLOCK_SIZE; i++) {
                            //    //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                            //    //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                            //    //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                            //    //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                            //    Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                            //    //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                            //    pACur = ref Unsafe.Add(ref pACur, strideA);
                            //    //pB = ref Unsafe.Add(ref pB, strideB);
                            //    pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                            //    //pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            //}
                            //pB = ref pBBak;
                            Vector<TMy> b0 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b1 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b2 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b3 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b4 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b5 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b6 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b7 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            // Block GEMM.
                            //ref TMy pACore = ref localA[0];
                            ref TMy pACore0 = ref pA;
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                ref TMy pACore = ref pACore0;
                                //ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                //for (int k = 0; k < BLOCK_SIZE; k++) {
                                //    Vector<TMy> vA = new Vector<TMy>(pACore);
                                //    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                //    //}
                                //    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                //    pACore = ref Unsafe.Add(ref pACore, 1);
                                //    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                //}
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b0), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b1), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b2), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b3), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b4), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b5), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b6), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b7), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                // Next.
                                pACore0 = ref Unsafe.Add(ref pACore0, strideA);
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            Unsafe.As<TMy, Vector<TMy>>(ref pCCur) = Unsafe.As<TMy, Vector<TMy>>(ref pCLocal);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
                //} finally {
                //    //ArrayPool<TMy>.Shared.Return(buf);
            } while (false);
        }

        [Benchmark]
        public void BlockCopy2SimdRegi2() {
            StaticBlockCopy2SimdRegi2(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2SimdRegi2");
            }
        }

        [Benchmark]
        public void UseMathNet() {
            matA!.Multiply(matB, matC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("UseMathNet");
            }
        }

        [Benchmark]
        public void UseMKL() {
            Blas.gemm(Layout.RowMajor, Trans.No, Trans.No, MatrixM, MatrixN, MatrixK, 1, arrayA!, StrideA, arrayB!, StrideB, 0, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("UseMKL");
            }
        }

    }
}
