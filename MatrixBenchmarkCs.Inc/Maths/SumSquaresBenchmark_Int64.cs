#undef BENCHMARKS_OFF

using BenchmarkDotNet.Attributes;
using MatrixLib.MathTraits;
using MatrixLib.MathTraits.CallerNumbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Zyl.ExSpans;

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
            ref TMy p = ref src[0];
            for(int i=0; i<srcCount; ++i) {
                rt += p * p;
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
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

#if NET7_0_OR_GREATER

        private static TMy StaticSumGenericOp(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumGenericOp<TMy>(src.AsSpan(0, srcCount)); // OK.
            //return MathTraitsUtil.SumGenericOp((ReadOnlySpan<TMy>) src.AsSpan(0, srcCount)); // OK.
            //return MathTraitsUtil.SumGenericOp(new ReadOnlySpan<TMy>(src, 0, srcCount)); //OK.
            //return MathTraitsUtil.SumGenericOp(new ReadOnlySpan<TMy>(src).Slice(0, srcCount)); //OK.
            //return MathTraitsUtil.SumGenericOp(src.AsSpan().Slice(0, srcCount)); // C#13: CS0411 The type arguments for method SumGenericOp cannot be inferred from the usage. Try specifying the type arguments explicitly.
        }

        [Benchmark]
        public void SumGenericOp() {
            //Debugger.Break();
            dstTMy = StaticSumGenericOp(srcArray, srcArray.Length);
            CheckResult("SumGenericOp");
        }

#endif // NET7_0_OR_GREATER

        private static TMy StaticSumRawTypeOf(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumRawTypeOf<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumRawTypeOf() {
            //Debugger.Break();
            dstTMy = StaticSumRawTypeOf(srcArray, srcArray.Length);
            CheckResult("SumRawTypeOf");
        }

        private static TMy StaticSumRawTypeOfAs(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumRawTypeOfAs<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumRawTypeOfAs() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumRawTypeOfAs(srcArray, srcArray.Length);
            CheckResult("SumRawTypeOfAs");
        }

        private static TMy StaticSumTraitsRaw(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumTraitsRaw<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumTraitsRaw() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsRaw(srcArray, srcArray.Length);
            CheckResult("SumTraitsRaw");
        }

        private static TMy StaticSumTraitsOut(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumTraitsOut<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumTraitsOut() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsOut(srcArray, srcArray.Length);
            CheckResult("SumTraitsOut");
        }

        private static TMy StaticSumTraitsUsing(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumTraitsUsing<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumTraitsUsing() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsUsing(srcArray, srcArray.Length);
            CheckResult("SumTraitsUsing");
        }

        private static TMy StaticSumTraitsV2Using(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumTraitsV2Using<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumTraitsV2Using() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsV2Using(srcArray, srcArray.Length);
            CheckResult("SumTraitsV2Using");
        }

        private static TMy StaticSumTraitsV2UsingStruct(TMy[] src, int srcCount) {
            var span1 = src.AsSpan(0, srcCount);
            var span2 = MemoryMarshal.Cast<TMy, StructNumberBase<TMy>>(span1);
            //ReadOnlySpan<StructNumberBase<TMy>> span3 = span2;
            //var rt = MathTraitsUtil.SumTraitsV2Using(span3);
            //if (true) {
            //    StructNumberBase<TMy> num = default;
            //    bool flag = (num is INumberBaseCaller<TMy>);
            //    Console.WriteLine("Out Is INumberBaseCaller: {0}", flag);
            //}
            var rt = MathTraitsUtil.SumTraitsV2Using<StructNumberBase<TMy>>(span2);
            return rt.Value;
        }

        [Benchmark]
        public void SumTraitsV2UsingStruct() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsV2UsingStruct(srcArray, srcArray.Length);
            CheckResult("SumTraitsV2UsingStruct");
        }

        private static TMy StaticSumTraitsV2RawStruct(TMy[] src, int srcCount) {
            var span1 = src.AsSpan(0, srcCount);
            var span2 = MemoryMarshal.Cast<TMy, StructNumberBase<TMy>>(span1);
            var rt = MathTraitsUtil.SumTraitsV2Raw<StructNumberBase<TMy>>(span2);
            return rt.Value;
        }

        [Benchmark]
        public void SumTraitsV2RawStruct() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsV2RawStruct(srcArray, srcArray.Length);
            CheckResult("SumTraitsV2RawStruct");
        }

        private static TMy StaticSumCallerIn(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumCallerIn<TMy, CallerInt64>(default, src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumCallerIn() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumCallerIn(srcArray, srcArray.Length);
            CheckResult("SumCallerIn");
        }

        private static TMy StaticSumCallerGetItf(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumCallerGetItf<TMy>(src.AsSpan(0, srcCount));
        }

        [Benchmark]
        public void SumCallerGetItf() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumCallerGetItf(srcArray, srcArray.Length);
            CheckResult("SumCallerGetItf");
            // 性能差, .NET Framework 没有内联.
            // SumCallerGetItf        1268.844        206.601 0.089479
        }

        private static TMy StaticSumCallerOut(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumCallerOut<TMy>(src.AsSpan(0, srcCount));
        }

        [Benchmark]
        public void SumCallerOut() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumCallerOut(srcArray, srcArray.Length);
            CheckResult("SumCallerOut");
        }

        private static TMy StaticSumV3CallerIn(TMy[] src, int srcCount) {
            return DemoTraitsUtilCommon.SumSquaresCall<TMy, CallerInt64>(default, src.AsSpan(0, srcCount));
        }

        [Benchmark]
        public void SumV3CallerIn() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumV3CallerIn(srcArray, srcArray.Length);
            CheckResult("SumV3CallerIn");
        }

        private static TMy StaticSumTraitsV3Using(TMy[] src, int srcCount) {
            return DemoTraitsUtil.SumSquares<TMy>(src.AsSpan(0, srcCount));
        }

        [Benchmark]
        public void SumTraitsV3Using() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsV3Using(srcArray, srcArray.Length);
            CheckResult("SumTraitsV3Using");
        }

        //private static TMy StaticSumTraitsV3UsingOld(TMy[] src, int srcCount) {
        //    return DemoTraitsUtilCommon.SumSquares<TMy>(src.AsSpan(0, srcCount));
        //}

        //[Benchmark] // Same SumTraitsV3Using
        //public void SumTraitsV3UsingOld() {
        //    if (BenchmarkUtil.IsLastRun) {
        //        Volatile.Write(ref dstTMy, 0);
        //        //Debugger.Break();
        //    }
        //    dstTMy = StaticSumTraitsV3UsingOld(srcArray, srcArray.Length);
        //    CheckResult("SumTraitsV3UsingOld");
        //}

        private static TMy StaticSumTraitsV3Struct(TMy[] src, int srcCount) {
            var span2 = MemoryMarshal.Cast<TMy, StructNumberBase<TMy>>(src.AsSpan(0, srcCount));
            var rt = DemoTraitsUtil.SumSquares<StructNumberBase<TMy>>(span2);
            return rt.Value;
        }

        [Benchmark]
        public void SumTraitsV3Struct() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsV3Struct(srcArray, srcArray.Length);
            CheckResult("SumTraitsV3Struct");
        }

        private static TMy StaticSumTraitsV3StructCommon(TMy[] src, int srcCount) {
            var span2 = MemoryMarshal.Cast<TMy, StructNumberBase<TMy>>(src.AsSpan(0, srcCount));
            var rt = DemoTraitsUtilCommon.SumSquares<StructNumberBase<TMy>>(span2);
            return rt.Value;
        }

        [Benchmark]
        public void SumTraitsV3StructCommon() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumTraitsV3StructCommon(srcArray, srcArray.Length);
            CheckResult("SumTraitsV3StructOld");
        }

    }
}
