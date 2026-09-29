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

    }

#endif // NET7_0_OR_GREATER
}
