#undef BENCHMARKS_OFF
#define Tensor_Primitives_ALLOW_FMA
#define Tensor_Primitives_ALLOW_T
//#define USED_EXSPANS

using BenchmarkDotNet.Attributes;
using MatrixLib;
using MatrixLib.Impl;
using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
#if USED_EXSPANS
using Zyl.ExSpans;
#endif // USED_EXSPANS

namespace MatrixBenchmarkCs.MultiplyMatrix {
#if BENCHMARKS_OFF
    using BenchmarkAttribute = FakeBenchmarkAttribute;
#else
#endif // BENCHMARKS_OFF
#if REDUCE_MEMORY_USAGE_B && !BENCHMARKS_OFF
    using Benchmark_BAttribute = BenchmarkAttribute;
#elif REDUCE_MEMORY_USAGE_B && BENCHMARKS_OFF
    using Benchmark_BAttribute = FakeBenchmarkAttribute;
#else
    using Benchmark_BAttribute = DisabledBenchmarkAttribute;
#endif // REDUCE_MEMORY_USAGE_B
#if REDUCE_MEMORY_USAGE_C && !BENCHMARKS_OFF
    using Benchmark_CAttribute = BenchmarkAttribute;
#elif REDUCE_MEMORY_USAGE_C && BENCHMARKS_OFF
    using Benchmark_CAttribute = FakeBenchmarkAttribute;
#else
    using Benchmark_CAttribute = DisabledBenchmarkAttribute;
#endif // REDUCE_MEMORY_USAGE_C
#if REDUCE_MEMORY_USAGE_D && !BENCHMARKS_OFF
    using Benchmark_DAttribute = BenchmarkAttribute;
#elif REDUCE_MEMORY_USAGE_D && BENCHMARKS_OFF
    using Benchmark_DAttribute = FakeBenchmarkAttribute;
#else
    using Benchmark_DAttribute = DisabledBenchmarkAttribute;
#endif // REDUCE_MEMORY_USAGE_D
#if REDUCE_MEMORY_USAGE_E && !BENCHMARKS_OFF
    using Benchmark_EAttribute = BenchmarkAttribute;
#elif REDUCE_MEMORY_USAGE_E && BENCHMARKS_OFF
    using Benchmark_EAttribute = FakeBenchmarkAttribute;
#else
    using Benchmark_EAttribute = DisabledBenchmarkAttribute;
#endif // REDUCE_MEMORY_USAGE
    using static MultiplyMatrixStatic;


    // My type.
    using TMy = Single;

    /// <summary>
    /// Matrix N*N multiply matrix N*N benchmark - Single.
    /// </summary>
    public class MatrixNMultiplyBenchmark_Single : MatrixNMultiplyBenchmark<TMy> {

        protected MathNet.Numerics.LinearAlgebra.Matrix<TMy>? matA;
        protected MathNet.Numerics.LinearAlgebra.Matrix<TMy>? matB;
        protected MathNet.Numerics.LinearAlgebra.Matrix<TMy>? matC;

        protected override void ArraySetup() {
            base.ArraySetup();
            try {
                matA = MathNet.Numerics.LinearAlgebra.Matrix<TMy>.Build.DenseOfRowMajor(MatrixM, MatrixK, arrayA);
                matB = MathNet.Numerics.LinearAlgebra.Matrix<TMy>.Build.DenseOfRowMajor(MatrixK, MatrixN, arrayB);
                matC = MathNet.Numerics.LinearAlgebra.Matrix<TMy>.Build.Dense(MatrixM, MatrixN);
                //matA.Multiply(matB, matC);
            } catch (Exception ex) {
                BenchmarkUtil.WriteItem("# Setup-error", string.Format("{0}", ex.Message));
            }
        }

        protected override void CheckResult(string name) {
            CheckResult_Report(name, dstTMy != baselineTMy, dstTMy, baselineTMy);
        }

        protected override TMy GetCheckSum() {
            return CheckSumUtil.Calculate2D(arrayC, MatrixN, MatrixM, StrideC);
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

        [Benchmark_E]
        public void Basic() {
            StaticBasic(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                //baselineTMy = dstTMy;
                //BenchmarkUtil.WriteItem("# Basic", string.Format("{0}", baselineTMy));
                CheckResult("Basic");
            }
        }

        [Benchmark_E]
        public void BasicSpan() {
            StaticBasicSpan(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BasicSpan");
                //BenchmarkUtil.WriteItem("# BasicSpan", string.Format("{0}", dstTMy));
            }
        }

        [Benchmark_E]
        public void BasicRef() {
            StaticBasicRef(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BasicRef");
            }
        }

        [Benchmark_E]
        public void LinearWriteSimd() {
            StaticLinearWriteSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("LinearWriteSimd");
            }
        }

        [Benchmark_E]
        public void LinearWriteSimdLU() {
            StaticLinearWriteSimdLU(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("LinearWriteSimdLU");
            }
        }

        [Benchmark_D]
        public void LinearWriteSimdParallel() {
            int M = MatrixM;
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1);
            if (allowParallel) {
                Parallel.For(0, M, i => {
                    StaticLinearWriteSimdLU(1, MatrixN, MatrixK, ref arrayA![StrideA * i], StrideA, ref arrayB![0], StrideB, ref arrayC![StrideC * i], StrideC);
                });
            } else {
                StaticLinearWriteSimdLU(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("LinearWriteSimdParallel");
            }
        }

        [Benchmark_E]
        public void Transpose() {
            StaticTranspose(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("Transpose");
            }
        }

#if Tensor_Primitives_ALLOW_T
        [Benchmark_D]
        public void TransposeSpanTP() {
            StaticTransposeSpanTP(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TransposeSpanTP");
            }
        }
#endif // Tensor_Primitives_ALLOW_T

        [Benchmark_D]
        public void TransposeSimd() {
            StaticTransposeSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TransposeSimd");
            }
        }

        [Benchmark_C]
        public void TransposeSimdParallel() {
            int M = MatrixM;
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1);
            if (allowParallel) {
                int total = MatrixK * MatrixN;
                TMy[] BTrans = ArrayPool<TMy>.Shared.Rent(total);
                ref TMy pB0 = ref BTrans[0];
                int strideBTran = MatrixK;
                MatrixUtil.Transpose(MatrixK, MatrixN, ref arrayB![0], StrideB, ref pB0, strideBTran);
                try {
                    Parallel.For(0, M, i => {
                        StaticTransposeSimd(1, MatrixN, MatrixK, ref arrayA![StrideA * i], StrideA, ref BTrans[0], strideBTran, ref arrayC![StrideC * i], StrideC, true);
                    });
                } finally {
                    ArrayPool<TMy>.Shared.Return(BTrans);
                }
            } else {
                StaticTransposeSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TransposeSimdParallel");
            }
        }

        [Benchmark_B]
        public unsafe void TransposeSimdParallelAlign() {
            const int alignment = 64;
            int M = MatrixM;
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1);
            if (allowParallel) {
                int strideBTran = (MatrixK + alignment - 1) / alignment * alignment;
                int total = strideBTran * MatrixN + (alignment / sizeof(TMy));
                TMy[] BTrans = ArrayPool<TMy>.Shared.Rent(total);
                try {
                    fixed (TMy* pBTransRaw = &BTrans[0]) {
                        nint offset = 0;
                        nint rem = (nint)pBTransRaw % alignment;
                        if (0 != rem) {
                            offset = alignment - rem;
                        }
                        TMy* pBTrans = (TMy*)((byte*)pBTransRaw + offset);
                        MatrixUtil.Transpose(MatrixK, MatrixN, ref arrayB![0], StrideB, ref Unsafe.AsRef<TMy>(pBTrans), strideBTran);
                        Parallel.For(0, M, i => {
                            StaticTransposeSimd(1, MatrixN, MatrixK, ref arrayA![StrideA * i], StrideA, ref Unsafe.AsRef<TMy>(pBTrans), strideBTran, ref arrayC![StrideC * i], StrideC, true);
                        });
                    }
                } finally {
                    ArrayPool<TMy>.Shared.Return(BTrans);
                }
            } else {
                StaticTransposeSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TransposeSimdParallelAlign");
            }
        }

        [Benchmark_D]
        public void TileRow() {
            StaticTileRow(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRow");
            }
        }

        [Benchmark_E]
        public void TileRowSpan() {
            StaticTileRowSpan(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSpan");
            }
        }

#if Tensor_Primitives_ALLOW_T
        [Benchmark_D]
        public void TileRowTP() {
            StaticTileRowTP(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowTP");
            }
        }
#endif // Tensor_Primitives_ALLOW_T

#if Tensor_Primitives_ALLOW_FMA
#if NET8_0_OR_GREATER
        [Benchmark_D]
        public void TileRowTPFma() {
            StaticTileRowTPFma(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowTPFma");
            }
        }
#endif // NET8_0_OR_GREATER
#endif // Tensor_Primitives_ALLOW_FMA

        [Benchmark] // [Benchmark_C]
        public void TileRowSimd() {
            StaticTileRowSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimd");
            }
        }

#if NET9_0_OR_GREATER
        [Benchmark_D]
        public void TileRowSimdFma() {
            StaticTileRowSimdFma(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdFma");
            }
        }
#endif // NET9_0_OR_GREATER

#if Tensor_Primitives_ALLOW_FMA
#if NETCOREAPP3_0_OR_GREATER
        [Benchmark_D]
        public void TileRowSimdFmaX86() {
            StaticTileRowSimdFmaX86(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdFmaX86");
            }
        }
#endif // NETCOREAPP3_0_OR_GREATER
#endif // Tensor_Primitives_ALLOW_FMA

        [Benchmark_D]
        public void TileRowSimdLU4() {
            StaticTileRowSimdLU4(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("TileRowSimdLU4");
            }
        }

        [Benchmark_B]
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

        [Benchmark_C]
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

#if NETCOREAPP3_0_OR_GREATER
        [Benchmark] // [Benchmark_C]
        public unsafe void OtherGemmAvxBlock() {
            arrayC!.AsSpan().Clear();
            fixed (TMy* pA = &arrayA![0], pB = &arrayB![0], pC = &arrayC![0]) {
                StaticOtherGemmAvxBlock(MatrixM, MatrixN, MatrixK, pA, StrideA, pB, StrideB, pC, StrideC, false);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("OtherGemmAvxBlock");
            }
        }

        [Benchmark_B]
        public unsafe void OtherGemmAvxBlockParallel() {
            const int BLOCKSIZE = 32;
            bool allowParallel = (MatrixM >= (BLOCKSIZE * 2)) && (Environment.ProcessorCount > 1);
            if (!allowParallel) {
                throw new NotSupportedException(string.Format("No parallel, the matrix too small({0}, {1}, {2})!", MatrixM, MatrixN, MatrixK));
            }
            arrayC!.AsSpan().Clear();
            fixed (TMy* pA = &arrayA![0], pB = &arrayB![0], pC = &arrayC![0]) {
                StaticOtherGemmAvxBlock(MatrixM, MatrixN, MatrixK, pA, StrideA, pB, StrideB, pC, StrideC, allowParallel);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("OtherGemmAvxBlockParallel");
            }
        }
#endif // NETCOREAPP3_0_OR_GREATER

        [Benchmark_E]
        public void BlockCopy2() {
            StaticBlockCopy2(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2");
            }
        }

        [Benchmark_E]
        public void BlockCopy2Span() {
            StaticBlockCopy2Span(MatrixM, MatrixN, MatrixK, arrayA!, StrideA, arrayB!, StrideB, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2Span");
            }
        }

        [Benchmark_E]
        public void BlockCopy2Ref() {
            StaticBlockCopy2Ref(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2Ref");
            }
        }

        [Benchmark_E]
        public void BlockCopy2Simd() {
            StaticBlockCopy2Simd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2Simd");
            }
        }

        [Benchmark_E]
        public void BlockCopy2SimdRegi() {
            StaticBlockCopy2SimdRegi(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2SimdRegi");
                //BenchmarkUtil.WriteItem("# Vector<TMy>.Count", string.Format("{0}", Vector<TMy>.Count));
            }
        }

        [Benchmark_E]
        public void BlockCopy2SimdRegi2() {
            StaticBlockCopy2SimdRegi2(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2SimdRegi2");
            }
        }

        [Benchmark_E]
        public void BlockCopy2SimdParallel() {
            int batchSize = Vector<TMy>.Count;
            int M = MatrixM;
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1) && (0 == (MatrixM % batchSize)) && (0 == (MatrixN % batchSize)) && (0 == (MatrixK % batchSize));
            if (allowParallel) {
                int count = (M + batchSize - 1) / batchSize;
                Parallel.For(0, count
                //, ParallelOptionsCPU
                , i => {
                    int idx = batchSize * i;
                    int curSize = batchSize;
                    if (curSize > M - idx) {
                        curSize = M - idx;
                    }
                    StaticBlockCopy2SimdRegi(curSize, MatrixN, MatrixK, ref arrayA![StrideA * idx], StrideA, ref arrayB![0], StrideB, ref arrayC![StrideC * idx], StrideC);
                });
            } else {
                // StaticTileRowSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
                throw new NotSupportedException(string.Format("{0} is not an integer multiple of {1}!", M, batchSize));
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2SimdParallel");
            }
        }

        [Benchmark_E]
        public void BlockCopy2SimdParallel2() {
            int batchSize = Vector<TMy>.Count;
            int M = MatrixM;
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1) && (0 == (MatrixM % batchSize)) && (0 == (MatrixN % batchSize)) && (0 == (MatrixK % batchSize));
            if (allowParallel) {
                int count = (M + batchSize - 1) / batchSize;
                Parallel.For(0, count
                //, ParallelOptionsCPU
                , i => {
                    int idx = batchSize * i;
                    int curSize = batchSize;
                    if (curSize > M - idx) {
                        curSize = M - idx;
                    }
                    StaticBlockCopy2SimdRegi2(curSize, MatrixN, MatrixK, ref arrayA![StrideA * idx], StrideA, ref arrayB![0], StrideB, ref arrayC![StrideC * idx], StrideC);
                });
            } else {
                // StaticTileRowSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
                throw new NotSupportedException(string.Format("{0} is not an integer multiple of {1}!", M, batchSize));
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockCopy2SimdParallel2");
            }
        }

        [Benchmark]
        public void BlockM4Nv1_ijk_32() {
            StaticBlockM4Nv1_ijk_32(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv1_ijk_32");
            }
        }

        [Benchmark]
        public unsafe void BlockM4Nv1_ijk_32Parallel() {
            StaticBlockM4Nv1_ijk_32(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC, true);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv1_ijk_32Parallel");
            }
        }

        [Benchmark]
        public void BlockM4Nv1_ikj_32() {
            StaticBlockM4Nv1_ikj_32(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv1_ikj_32");
            }
        }

        [Benchmark]
        public unsafe void BlockM4Nv1_ikj_32Parallel() {
            StaticBlockM4Nv1_ikj_32(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC, true);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv1_ikj_32Parallel");
            }
        }

        [Benchmark]
        public void BlockM4Nv1_ikj_32K() {
            StaticBlockM4Nv1_ikj_32K(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv1_ikj_32K");
            }
        }

        [Benchmark]
        public unsafe void BlockM4Nv1_ikj_32KParallel() {
            StaticBlockM4Nv1_ikj_32K(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC, true);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv1_ikj_32KParallel");
            }
        }

        [Benchmark]
        public void BlockM4Nv3_ikj_32() {
            StaticBlockM4Nv3_ikj_32(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv3_ikj_32");
            }
        }

        [Benchmark]
        public unsafe void BlockM4Nv3_ikj_32Parallel() {
            StaticBlockM4Nv3_ikj_32(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC, true);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv3_ikj_32Parallel");
            }
        }

        [Benchmark]
        public void BlockM4Nv3_ikj_32K() {
            StaticBlockM4Nv3_ikj_32K(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv3_ikj_32K");
            }
        }

        [Benchmark]
        public unsafe void BlockM4Nv3_ikj_32KParallel() {
            StaticBlockM4Nv3_ikj_32K(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC, true);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("BlockM4Nv3_ikj_32KParallel");
            }
        }

#if USE_MATRIX_LIB
        [Benchmark_C]
        public void CallLib() {
            MatrixMath.MultiplyMatrix(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("CallLib");
            }
        }

#if USED_EXSPANS
        [Benchmark_C]
        public void CallLibSpan() {
            MatrixMath.MultiplyMatrixSpan(MatrixM, MatrixN, MatrixK, arrayA.AsExSpan(), StrideA, arrayB.AsExSpan(), StrideB, arrayC.AsExSpan(), StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("CallLibSpan");
            }
        }
#endif // USED_EXSPANS

        [Benchmark_D]
        public void CallLibSimd() {
            MatrixMathImpl.Instance.MultiplyMatrix_TileRowSimd(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("CallLibSimd");
            }
        }

        [Benchmark_B]
        public void CallLibSimdParallel() {
            MatrixMathImpl.Instance.MultiplyMatrix_TileRowSimdParallel(MatrixM, MatrixN, MatrixK, ref arrayA![0], StrideA, ref arrayB![0], StrideB, ref arrayC![0], StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("CallLibSimdParallel");
            }
        }
#endif // USE_MATRIX_LIB

        [Benchmark_B]
        public void UseMathNet() {
            matA!.Multiply(matB, matC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("UseMathNet");
            }
        }

#if USE_NATIVE_DLL
        [Benchmark]
        public unsafe void UseOpenBLAS() {
            fixed (TMy* pA = &arrayA![0], pB = &arrayB![0], pC = &arrayC![0]) {
                OpenBlasSharp.Blas.Sgemm(OpenBlasSharp.Order.RowMajor, OpenBlasSharp.Transpose.NoTrans, OpenBlasSharp.Transpose.NoTrans, MatrixM, MatrixN, MatrixK, 1, pA, StrideA, pB, StrideB, 0, pC, StrideC);
            }
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("UseOpenBLAS");
            }
        }

        [Benchmark]
        public void UseMKL() {
            MKLNET.Blas.gemm(MKLNET.Layout.RowMajor, MKLNET.Trans.No, MKLNET.Trans.No, MatrixM, MatrixN, MatrixK, 1, arrayA!, StrideA, arrayB!, StrideB, 0, arrayC!, StrideC);
            if (CheckMode) {
                dstTMy = GetCheckSum();
                CheckResult("UseMKL");
            }
        }
#endif // USE_NATIVE_DLL

    }
}
