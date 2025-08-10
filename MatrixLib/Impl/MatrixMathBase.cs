using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.Impl {
    /// <summary>
    /// MatrixMath Base.
    /// </summary>
    /// <remarks>If MatrixMathBase's methods throws NotImplementedException exception, please call `MatrixLibEnvironment.Init` method of `MatrixLib.Impl` first (若 MatrixMathBase 的方法抛出 NotSupportedException 异常, 请先调用 `MatrixLib.Impl` 的 `MatrixLibEnvironment.Init` 方法).</remarks>
    public class MatrixMathBase {

        /// <inheritdoc cref="MatrixMath.MultiplyMatrix(int, int, int, ref readonly float, int, ref readonly float, int, ref float, int)"/>
        public virtual void MultiplyMatrix(int M, int N, int K, ref readonly float A, int strideA, ref readonly float B, int strideB, ref float C, int strideC) {
            ThrowNotImplementedException();
        }

        /// <inheritdoc cref="MatrixMath.MultiplyMatrix(int, int, int, ref readonly double, int, ref readonly double, int, ref double, int)"/>
        public virtual void MultiplyMatrix(int M, int N, int K, ref readonly double A, int strideA, ref readonly double B, int strideB, ref double C, int strideC) {
            ThrowNotImplementedException();
        }

        /// <summary>
        /// throw new NotImplementedException.
        /// </summary>
        /// <param name="memberName">The memberName.</param>
        /// <exception cref="NotImplementedException">If MatrixMath's methods throws NotImplementedException exception, please call `MatrixLibEnvironment.Init` method of `MatrixLib.Impl` first (若 MatrixMath 的方法抛出 NotSupportedException 异常, 请先调用 `MatrixLib.Impl` 的 `MatrixLibEnvironment.Init` 方法).</exception>
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ThrowNotImplementedException([CallerMemberName] string memberName = "") {
            string msg = "The " + memberName  + " methods is not implemented! Please call `MatrixLibEnvironment.Init` method of `MatrixLib.Impl` first.";
            throw new NotImplementedException(msg);
        }

        /// <inheritdoc cref="MatrixMath.SupportedInstructionSets"/>
        public virtual string SupportedInstructionSets {
            get {
                ThrowNotImplementedException();
                return string.Empty;
            }
        }

    }
}
