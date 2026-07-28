using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits.HasWhere {

#if NET7_0_OR_GREATER
    /// <summary>
    /// <see cref="INumberBase{TSelf}"/> 的类型萃取, 有约束.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct TraitsINumberBase<T>
        where T : INumberBase<T>
#if NET9_0_OR_GREATER
        //, allows ref struct // 这是因为 INumberBase 的 T 尚不支持 `allows ref struct` // CS9244 The type 'T' may not be a ref struct or a type parameter allowing ref structs in order to use it as parameter 'TSelf' in the generic type or method 'INumberBase<TSelf>'
#endif // NET9_0_OR_GREATER
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Addition(T left, T right) {
            return left + right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Multiply(T left, T right) {
            return left * right;
        }

        public T Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return T.Zero;
            }
        }

    }
#endif // NET7_0_OR_GREATER

}
