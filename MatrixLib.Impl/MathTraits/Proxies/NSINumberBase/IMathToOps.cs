using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies.NSINumberBase {
#if NET7_0_OR_GREATER

    /// <summary>
    /// 泛型数学转为数值操作接口.
    /// </summary>
    public partial interface IMathToOps<T>: INumOps<T> where T : INumberBase<T> {

        /// <inheritdoc cref="INumOps{T}.DoZero"/>
        public static T StaticZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => T.Zero;
        }

        /// <inheritdoc cref="INumOps{T}.DoAddition"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T StaticAddition(T left, T right) => left + right;

        /// <inheritdoc cref="INumOps{T}.DoMultiply"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T StaticMultiply(T left, T right) => left * right;

        T INumOps<T>.DoZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => StaticZero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumOps<T>.DoAddition(T left, T right) => StaticAddition(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumOps<T>.DoMultiply(T left, T right) => StaticMultiply(left, right);

    }

#endif // NET7_0_OR_GREATER
}
