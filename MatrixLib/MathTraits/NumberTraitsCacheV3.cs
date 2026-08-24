using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值类型萃取缓存. 元素类型 <typeparamref name="T"/> 必须有无参构造函数, 且可实现 INumberBaseCaller 等接口. 当没有 INumberBaseCaller 等接口时. One 等静态属性会是 Zero 或 default .
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class NumberTraitsCacheV3<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T> {
        public static T Zero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = default!;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        //[MaybeNull]
        public static T One { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = default!;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        /// <summary>初始化后的 Hash.</summary>
        public static int InitHash { get; } = 0;

        public static INumberBaseCaller<T>? NumberBase { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = null;
        public static Func<T, T, T>? CallAddition { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = null;
        public static Func<T, T, T>? CallMultiply { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = null;

        /// <summary>
        /// Static create NumberTraitsCacheV3{T}.
        /// </summary>
        /// <exception cref="NotSupportedException">Not supported type! Please check if the Add methods of NumberTraitsManager have been called. (请检查是否已调用了 MathTraitsManager 的 Add 方法)</exception>
        static NumberTraitsCacheV3() {
            NumberTraitsDefine<T>? define = NumberTraitsUtil.GetDefine<T>();
            if (define is null) {
                _ = define; // To ignore the `IDE0270 Null check can be simplified` message on `if (define is null)`.
                throw new NotSupportedException(string.Format("Not supported type {0}! Please check if the Add methods of NumberTraitsManager have been called.", typeof(T).FullName));
            }
            InitHash = define.GetHashCode();
            NumberBase = define.NumberBase;
            if (NumberBase is not null) {
                Zero = NumberBase.CallZero;
                CallAddition = NumberBase.CallAddition;
                CallMultiply = NumberBase.CallMultiply;
                // 预热.
                try {
                    var TT = TraitsINumberBaseV3<T>.Instance;
                    var t1 = TT.Addition(Zero, Zero);
                    t1 = TT.Multiply(t1, Zero);
                    InitHash ^= t1?.GetHashCode() ?? 1;
                } catch (Exception ex) {
                    Debug.WriteLine("The type `" + typeof(T).Name + "` register INumberBaseCaller fail! " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 初始化. 能触发静态构造函数.
        /// </summary>
        /// <returns>返回 InitHash.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int Init() {
            return InitHash;
        }

    }

}
