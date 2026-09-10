using MatrixLib.MathTraits.Providers;
using System;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits {

    partial struct NumberBaseProxy<T> : INumberBaseCaller<NumberBaseProxy<T>>, IBaseMathCallerRegister {

        /// <inheritdoc cref="IBaseMathCallerRegisterDocument.CallerRegister"/>
        public static bool CallerRegister() {
            return NumberTraitsManager.Instance.Register<NumberBaseProxy<T>>();
        }

        public NumberBaseProxy<T> CallZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return Zero; } }

        public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(T); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberBaseProxy<T> CallAddition(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberBaseProxy<T> CallMultiply(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return left * right;
        }
    }
}
