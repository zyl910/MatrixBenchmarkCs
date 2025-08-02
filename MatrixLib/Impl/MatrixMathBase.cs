using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.Impl {
    /// <summary>
    /// MatrixMath Base.
    /// </summary>
    public class MatrixMathBase {

        /// <inheritdoc cref="MatrixMath.MultiplyMatrix"/>
        public virtual void MultiplyMatrix(int M, int N, int K, ref readonly float A, int strideA, ref readonly float B, int strideB, ref float C, int strideC) {
            ThrowNotSupportedException();
        }

        /// <summary>
        /// throw new NotSupportedException.
        /// </summary>
        /// <param name="memberName">The memberName.</param>
        /// <exception cref="NotSupportedException"></exception>
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ThrowNotSupportedException([CallerMemberName] string memberName = "") {
            string msg = "The " + memberName  + " methods is not supported!";
            throw new NotSupportedException(msg);
        }

        /// <inheritdoc cref="MatrixMath.SupportedInstructionSets"/>
        public virtual string SupportedInstructionSets {
            get {
                ThrowNotSupportedException();
                return string.Empty;
            }
        }

    }
}
