using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies.NSINumberBase {
#if NET7_0_OR_GREATER

    /// <summary>
    /// 泛型数学转为数值操作接口.
    /// </summary>
    public partial interface IMathToOps<T>: INumOps<T> where T : INumberBase<T> {

        T INumOps<T>.DoZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => StaticZero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumOps<T>.DoAddition(T left, T right) => StaticAddition(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumOps<T>.DoMultiply(T left, T right) => StaticMultiply(left, right);

        /// <inheritdoc cref="INumOps{T}.SupportIComparable"/>
        bool INumOps<T>.SupportIComparable { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => StaticSupportIComparable; }
        /// <inheritdoc cref="INumOps{T}.SupportIComparisonOperators"/>
        bool INumOps<T>.SupportIComparisonOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => StaticSupportIComparisonOperators; }
        /// <inheritdoc cref="INumOps{T}.SupportIModulusOperators"/>
        bool INumOps<T>.SupportIModulusOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => StaticSupportIModulusOperators; }

        /// <inheritdoc cref="INumOps{T}.DoCompareTo"/>
        int INumOps<T>.DoCompareTo(T self, T? other) => StaticCompareTo(self, other);
    }

#endif // NET7_0_OR_GREATER
}
