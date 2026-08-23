using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib {
    /// <summary>
    /// 将 TFrom 类型的给定值 重新解释为 TTo 类型的值。
    /// </summary>
    /// <typeparam name="TFrom">源类型.</typeparam>
    /// <typeparam name="TTo">目标类型.</typeparam>
    public readonly struct UnsafeAs<TFrom, TTo>
#if NET9_0_OR_GREATER
        where TFrom : allows ref struct where TTo : allows ref struct
#endif // NET9_0_OR_GREATER
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TFrom From(TTo source) {
            // 类似 Unsafe.BitCast<TFrom,TTo>(TFrom source)
            return Unsafe.As<TTo, TFrom>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TTo To(TFrom source) {
            // 类似 Unsafe.BitCast<TFrom,TTo>(TFrom source)
            return Unsafe.As<TFrom, TTo>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TFrom RefFrom(ref TTo source) {
            return ref Unsafe.As<TTo, TFrom>(ref source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TTo RefTo(ref TFrom source) {
            return ref Unsafe.As<TFrom, TTo>(ref source);
        }

    }

}
