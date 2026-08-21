using MatrixLib.MathTraits.HasWhere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    partial struct NumberStruct<T> : INumberBaseCaller<NumberStruct<T>> {
        public NumberStruct<T> CallZero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return ZeroOfTypes<T>.Zero; } }

        public Type CallElementType { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return typeof(T); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberStruct<T> CallAddition(NumberStruct<T> left, NumberStruct<T> right) {
            return TraitsINumberBaseV2<T>.Instance.Addition(left.Value, right.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberStruct<T> CallMultiply(NumberStruct<T> left, NumberStruct<T> right) {
            return TraitsINumberBaseV2<T>.Instance.Multiply(left.Value, right.Value);
        }
    }
}
