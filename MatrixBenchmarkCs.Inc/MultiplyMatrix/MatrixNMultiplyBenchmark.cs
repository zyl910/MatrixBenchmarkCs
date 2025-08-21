using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Zyl.VectorTraits;

namespace MatrixBenchmarkCs.MultiplyMatrix {
    /// <summary>
    /// Matrix(N*N) multiply matrix(N*N) benchmark.
    /// </summary>
    /// <typeparam name="T">The element type (元素的类型).</typeparam>
    public abstract class MatrixNMultiplyBenchmark<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif // NET6_0_OR_GREATER
        T> : AbstractMatrixMultiplyBenchmark<T>, ILoopCountGetter where T : struct
#if NET7_0_OR_GREATER
        , INumber<T>
#endif // NET7_0_OR_GREATER
        {

        /// <summary>
        /// Create MatrixNMultiplyBenchmark.
        /// </summary>
        protected MatrixNMultiplyBenchmark() : base() {
            ValuesForN = new int[] {
                //64,
                //32, 64, 65,
                //128, 256,
                //512, 513,
                1024,
                //2048,
                //4096,
            };
        }

        protected override void GlobalSetupCore() {
            base.GlobalSetupCore();
        }

    }
}
