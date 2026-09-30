using System;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies.NSINumberBase {
#if NET7_0_OR_GREATER

    partial interface IMathToOps<T> {

        /// <inheritdoc cref="INumOps{T}.SupportIComparable"/>
        public static bool StaticSupportIComparable { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => false; }
        /// <inheritdoc cref="INumOps{T}.SupportIComparisonOperators"/>
        public static bool StaticSupportIComparisonOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => false; }
        /// <inheritdoc cref="INumOps{T}.SupportIModulusOperators"/>
        public static bool StaticSupportIModulusOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => false; }

        /// <inheritdoc cref="INumOps{T}.DoCompareTo"/>
        public static int StaticCompareTo(T self, T? other) => throw new NotSupportedException(string.Format("The type {0} not supported {1}!", typeof(T).FullName, nameof(DoCompareTo)));

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
