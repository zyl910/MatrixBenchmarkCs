using MatrixLib.MathTraits.GenericMaths;
using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
#if NET7_0_OR_GREATER
    using TraitsNS = MatrixLib.MathTraits.GenericMaths;
#else
    using TraitsNS = MatrixLib.MathTraits;
#endif // NET7_0_OR_GREATER

    partial struct NumberBaseProxy<T> : INumberBaseCaller<NumberBaseProxy<T>>, IBaseMathCallerRegister {

        /// <inheritdoc cref="IBaseMathCallerRegisterDocument.CallerRegister"/>
        public static bool CallerRegister() {
            return NumberTraitsManager.Instance.Register<NumberBaseProxy<T>>();
        }

        public NumberBaseProxy<T> CallZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return ZeroOfTypes<T>.Zero; } }

        public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(T); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberBaseProxy<T> CallAddition(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
#if NET7_0_OR_GREATER
            return left.Value + right.Value;
#else
            return TraitsINumberBaseV2<T>.Instance.Addition(left.Value, right.Value);
#endif // NET7_0_OR_GREATER
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberBaseProxy<T> CallMultiply(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
#if NET7_0_OR_GREATER
            return left.Value * right.Value;
#else
            return TraitsINumberBaseV2<T>.Instance.Multiply(left.Value, right.Value);
#endif // NET7_0_OR_GREATER
        }
    }
}
