using MatrixLib.MathTraits.GenericMaths;
using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.CallerNumbers {
#if NET7_0_OR_GREATER
    using TraitsNS = MatrixLib.MathTraits.GenericMaths;
#else
    using TraitsNS = MatrixLib.MathTraits;
#endif // NET7_0_OR_GREATER

    partial struct StructNumberBase<T> : INumberBaseCaller<StructNumberBase<T>>, IBaseMathCallerRegister {

        /// <inheritdoc cref="IBaseMathCallerRegisterDocument.CallerRegister"/>
        public static bool CallerRegister() {
            return NumberTraitsManager.Instance.Register<StructNumberBase<T>>();
        }

        public StructNumberBase<T> CallZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return ZeroOfTypes<T>.Zero; } }

        public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(T); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructNumberBase<T> CallAddition(StructNumberBase<T> left, StructNumberBase<T> right) {
#if NET7_0_OR_GREATER
            return left.Value + right.Value;
#else
            return TraitsINumberBaseV2<T>.Instance.Addition(left.Value, right.Value);
#endif // NET7_0_OR_GREATER
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructNumberBase<T> CallMultiply(StructNumberBase<T> left, StructNumberBase<T> right) {
#if NET7_0_OR_GREATER
            return left.Value * right.Value;
#else
            return TraitsINumberBaseV2<T>.Instance.Multiply(left.Value, right.Value);
#endif // NET7_0_OR_GREATER
        }
    }
}
