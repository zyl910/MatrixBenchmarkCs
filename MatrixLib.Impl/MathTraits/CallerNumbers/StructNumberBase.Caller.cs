using MatrixLib.MathTraits.GenericMaths;
using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.CallerNumbers {
    partial struct StructNumberBase<T> : INumberBaseCaller<StructNumberBase<T>>, IBaseMathCallerRegister {

        /// <inheritdoc cref="IBaseMathCallerRegisterDocument.CallerRegister"/>
        public static bool CallerRegister() {
            return NumberTraitsManager.Instance.Register<StructNumberBase<T>>();
        }

        public StructNumberBase<T> CallZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return ZeroOfTypes<T>.Zero; } }

        public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(T); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructNumberBase<T> CallAddition(StructNumberBase<T> left, StructNumberBase<T> right) {
            return TraitsINumberBaseV2<T>.Instance.Addition(left.Value, right.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructNumberBase<T> CallMultiply(StructNumberBase<T> left, StructNumberBase<T> right) {
            return TraitsINumberBaseV2<T>.Instance.Multiply(left.Value, right.Value);
        }
    }
}
