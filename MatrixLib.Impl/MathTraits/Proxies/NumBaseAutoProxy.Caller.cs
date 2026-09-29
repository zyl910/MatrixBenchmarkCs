using MatrixLib.MathTraits.Providers;
using System;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies {

    partial struct NumberBaseProxy<T> : INumOps<NumberBaseProxy<T>>, IBaseMathCallerRegister {

        /// <inheritdoc cref="IBaseMathCallerRegisterDocument.CallerRegister"/>
        public static bool CallerRegister() {
            return NumberTraitsManager.Instance.Register<NumberBaseProxy<T>>();
        }

        readonly NumberBaseProxy<T> INumOps<NumberBaseProxy<T>>.DoZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return Zero; } }

        public bool SupportIComparable => throw new NotImplementedException();

        public bool SupportIComparisonOperators => throw new NotImplementedException();

        public bool SupportIModulusOperators => throw new NotImplementedException();

        //public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(T); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly NumberBaseProxy<T> INumOps<NumberBaseProxy<T>>.DoAddition(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly NumberBaseProxy<T> INumOps<NumberBaseProxy<T>>.DoMultiply(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return left * right;
        }

        public int DoCompareTo(NumberBaseProxy<T> self, NumberBaseProxy<T> other) {
            throw new NotImplementedException();
        }
    }
}
