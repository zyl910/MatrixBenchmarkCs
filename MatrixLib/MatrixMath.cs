//#define USED_EXSPANS

using MatrixLib.Impl;
using System;
using System.Runtime.CompilerServices;
#if USED_EXSPANS
using Zyl.ExSpans;
#endif // USED_EXSPANS

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("MatrixLib.Impl")]

namespace MatrixLib {
    /// <summary>
    /// Matrix Math (矩阵数学).
    /// </summary>
    /// <remarks>If MatrixMath's methods throws NotImplementedException exception, please call `MatrixLibEnvironment.Init` method of `MatrixLib.Impl` first (若 MatrixMath 的方法抛出 NotSupportedException 异常, 请先调用 `MatrixLib.Impl` 的 `MatrixLibEnvironment.Init` 方法).</remarks>
    public static class MatrixMath {
        internal static MatrixMathBase _instance = new MatrixMathBase();

        /// <summary>
        /// Matrix M*K multiply matrix K*N.
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
        public static void MultiplyMatrix(int M, int N, int K, ref readonly float A, int strideA, ref readonly float B, int strideB, ref float C, int strideC) {
            _instance.MultiplyMatrix(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
        }

#if USED_EXSPANS
        /// <inheritdoc cref="MultiplyMatrix"/>
        public static void MultiplyMatrixSpan(int M, int N, int K, ReadOnlyExSpan<float> A, int strideA, ReadOnlyExSpan<float> B, int strideB, ExSpan<float> C, int strideC) {
            MultiplyMatrix(M, N, K, in A[0], strideA, in B[0], strideB, ref C[0], strideC);
        }
#endif // USED_EXSPANS

        /// <summary>
        /// The instance (实例).
        /// </summary>
        public static MatrixMathBase Instance { get { return _instance; } }

        /// <summary>
        /// Supported instruction sets. The separator is a comma char ',' (支持的指令集. 分隔符是逗号',').
        /// </summary>
        public static string SupportedInstructionSets {
            get {
                return _instance.SupportedInstructionSets;
            }
        }

    }
}
