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
    /// Matrix N*N multiply matrix N*N benchmark.
    /// </summary>
    /// <typeparam name="T">The element type (元素的类型).</typeparam>
    public abstract class MatrixNMultiplyBenchmark<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif // NET6_0_OR_GREATER
        T> : AbstractBenchmark, ILoopCountGetter where T : struct
#if NET7_0_OR_GREATER
        , INumber<T>
#endif // NET7_0_OR_GREATER
        {

        protected T dstTMy = default;
        protected T baselineTMy = default;
        /// <summary>Left matrix</summary>
        protected T[]? arrayA;
        /// <summary>Result matrix</summary>
        protected T[]? arrayB;
        /// <summary>Result matrix</summary>
        protected T[]? arrayC;

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

        /// <summary>
        /// Array setup
        /// </summary>
        protected virtual void ArraySetup() {
            Random random = new Random(1);
            int total = N * N;
            arrayA = new T[total];
            arrayB = new T[total];
            arrayC = new T[total];
            bool isFloat = BenchmarkUtil.IsFloatType<T>();
            double scale = 1.0;
            if (!isFloat) {
                int n = Unsafe.SizeOf<T>() * 8 - 18;
                if (n <= (8 - 18)) {
                    n = 2;
                } else if (n < 4) {
                    n = 4;
                }
                scale = 1 << n;
            }
            int idx = 0;
            for (int i = 0; i < N; ++i) {
                for (int j = 0; j < N; ++j) {
                    arrayA[idx] = GetByDouble(random.NextDouble() * scale);
                    arrayB[idx] = GetByDouble(random.NextDouble() * scale);
                    ++idx;
                }
            }
            ParallelOptionsCPU = new ParallelOptions {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
        }

        /// <summary>
        /// Get check sum.
        /// </summary>
        /// <returns>Returns check sum.</returns>
        protected abstract T GetCheckSum();

        protected T GetByDouble(double src) {
#if NET7_0_OR_GREATER
            return T.CreateTruncating(src);
#else
            return Scalars.GetByDouble<T>(src);
#endif // NET7_0_OR_GREATER
        }

        /// <inheritdoc/>
        [GlobalSetup]
        public override void GlobalSetup() {
            base.GlobalSetup();
            ArraySetup();
            // Check.
            BenchmarkUtil.CheckAllBenchmark(this);
        }

        /// <summary>
        /// Check result.
        /// </summary>
        /// <param name="name">Method name.</param>
        protected abstract void CheckResult(string name);

        /// <summary>
        /// Check result - report.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isok">The isok.</param>
        /// <param name="dstT">The dstT.</param>
        /// <param name="baselineT">The baselineT.</param>
        protected virtual void CheckResult_Report(string name, bool isok, T dstT, T baselineT) {
            if (!CheckMode) return;
            if (isok) {
                string msg = string.Format("Check `{0}` mismatch. {1}!={2}", name, dstT, baselineT);
                // throw new ApplicationException(msg);
                string itemname = string.Format("Check-{0}", name);
                BenchmarkUtil.WriteItem(itemname, msg);
            } else {
                // Succeed. No output.
                string msg = string.Format("Check `{0}` Succeed.", name);
                //writer.WriteLine(indent + msg);
                Debug.WriteLine(msg);
            }
        }

        /// <summary>ParallelOptions of CPU.</summary>
        public ParallelOptions ParallelOptionsCPU { get; set; } = new ParallelOptions {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        /// <inheritdoc cref="ILoopCountGetter.LoopCount" />
        public long LoopCount { get { return N * N; } set { } }

        /// <summary>The number of columns in matrix A, or the number of rows in matrix B (矩阵A的列数, 或矩阵B的行数).</summary>
        public int MatrixK { get { return N; } }

        /// <summary>The number of rows in matrix A (矩阵A的行数).</summary>
        public int MatrixM { get { return N; } }

        /// <summary>The number of columns in matrix B (矩阵B的列数).</summary>
        public int MatrixN { get { return N; } }

        /// <summary>Stride of A.</summary>
        public int StrideA { get { return N; } }

        /// <summary>Stride of B.</summary>
        public int StrideB { get { return N; } }

        /// <summary>Stride of C.</summary>
        public int StrideC { get { return N; } }

    }
}
