using System;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Numbers {
    using TMy = Int64;

    /// <summary>
    /// <see cref="TMy"/> 的数值访问器.
    /// </summary>
    public readonly struct NumberVisitorInt64 : INumberBaseVisitor<TMy> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMy CallAddition(TMy left, TMy right) {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TMy CallMultiply(TMy left, TMy right) {
            return left * right;
        }

        public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(TMy); } }

        public TMy CallZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return default; } }

    }
}
