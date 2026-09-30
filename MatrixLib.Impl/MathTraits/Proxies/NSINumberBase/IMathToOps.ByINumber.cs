using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Proxies.NSINumberBase {
#if NET7_0_OR_GREATER

    partial interface IMathToOps<T> {

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

        /// <inheritdoc cref="INumOps{T}.SupportIComparable"/>
        public static bool StaticSupportIComparable { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => false; }
        /// <inheritdoc cref="INumOps{T}.SupportIComparisonOperators"/>
        public static bool StaticSupportIComparisonOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => false; }
        /// <inheritdoc cref="INumOps{T}.SupportIModulusOperators"/>
        public static bool StaticSupportIModulusOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => false; }

        /// <inheritdoc cref="INumOps{T}.DoCompareTo"/>
        public static int StaticCompareTo(T self, T? other) => throw new NotSupportedException(string.Format("The type {0} not supported {1}!", typeof(T).FullName, nameof(DoCompareTo)));

    }

#endif // NET7_0_OR_GREATER
}
