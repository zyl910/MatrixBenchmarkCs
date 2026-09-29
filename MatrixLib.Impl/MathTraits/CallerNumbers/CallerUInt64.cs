using System;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.CallerNumbers {
    using TMy = UInt64;

    /// <summary>
    /// <see cref="TMy"/> 的数值访问器.
    /// </summary>
    [CLSCompliant(false)]
    public readonly struct CallerUInt64 : INumOps<TMy> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMy DoAddition(TMy left, TMy right) {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMy DoMultiply(TMy left, TMy right) {
            return left + right;
        }

        public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(TMy); } }

        public TMy DoZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return default; } }

    }
}
