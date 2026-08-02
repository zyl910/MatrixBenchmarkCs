#undef BENCHMARKS_OFF

using BenchmarkDotNet.Attributes;
using MatrixLib.MathTraits;
using MatrixLib.MathTraits.Numbers;
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

        private static TMy StaticSumVisitorIn(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumVisitorIn<TMy, NumberVisitorInt64>(default, src.AsSpan(0, srcCount)); // OK.
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
            var span2 = MemoryMarshal.Cast<TMy, NumberStruct<TMy>>(span1);
            //ReadOnlySpan<NumberStruct<TMy>> span3 = span2;
            //var rt = MathTraitsUtil.SumTraitsV2Using(span3);
            //if (true) {
            //    NumberStruct<TMy> num = default;
            //    bool flag = (num is INumberBaseVisitor<TMy>);
            //    Console.WriteLine("Out Is INumberBaseVisitor: {0}", flag);
            //}
            var rt = MathTraitsUtil.SumTraitsV2Using<NumberStruct<TMy>>(span2);
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

        [Benchmark]
        public void SumVisitorIn() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumVisitorIn(srcArray, srcArray.Length);
            CheckResult("SumVisitorIn");
        }

        private static TMy StaticSumVisitorGetItf(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumVisitorGetItf<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumVisitorGetItf() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumVisitorGetItf(srcArray, srcArray.Length);
            CheckResult("SumVisitorGetItf");
            // 性能差, 没有内联.
            // SumVisitorGetItf        1268.844        206.601 0.089479
        }

        private static TMy StaticSumVisitorOut(TMy[] src, int srcCount) {
            return MathTraitsUtil.SumVisitorOut<TMy>(src.AsSpan(0, srcCount)); // OK.
        }

        [Benchmark]
        public void SumVisitorOut() {
            if (BenchmarkUtil.IsLastRun) {
                Volatile.Write(ref dstTMy, 0);
                //Debugger.Break();
            }
            dstTMy = StaticSumVisitorOut(srcArray, srcArray.Length);
            CheckResult("SumVisitorOut");
        }

    }
}
