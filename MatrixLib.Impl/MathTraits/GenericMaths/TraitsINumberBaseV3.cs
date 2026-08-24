using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits.GenericMaths {

#if NET7_0_OR_GREATER
    /// <summary>
    /// <see cref="INumberBase{TSelf}"/> 的类型萃取, 有约束.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class TraitsINumberBaseV3<T>: TraitsINumberBaseV2<T>, INumberBaseCaller<T>
        where T : INumberBase<T>
#if NET9_0_OR_GREATER
        //, allows ref struct // 这是因为 INumberBase 的 T 尚不支持 `allows ref struct` // CS9244 The type 'T' may not be a ref struct or a type parameter allowing ref structs in order to use it as parameter 'TSelf' in the generic type or method 'INumberBase<TSelf>'
#endif // NET9_0_OR_GREATER
    {
        /// <summary>
        /// 实例.
        /// </summary>
        public static new TraitsINumberBaseV3<T> Instance { [MethodImpl(MethodImplOptions.AggressiveInlining)]  get; } = new();

        T INumberBaseCaller<T>.CallZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return Zero;
            }
        }

        Type IBaseMathCaller.CallElementType => typeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumberBaseCaller<T>.CallAddition(T left, T right) {
            return Addition(left, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumberBaseCaller<T>.CallMultiply(T left, T right) {
            return Multiply(left, right);
        }
    }
#endif // NET7_0_OR_GREATER

}
