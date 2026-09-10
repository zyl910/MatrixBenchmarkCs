using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值类型萃取缓存. 元素类型 <typeparamref name="T"/> 必须有无参构造函数, 且可实现 INumberBaseCaller 等接口. 当没有 INumberBaseCaller 等接口时. One 等静态属性会是 Zero 或 default .
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class NumberTraitsCache<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T> {
        public static T Zero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = default!;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        //[MaybeNull]
        public static T One { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = default!;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public static int RegisterHash { get; } = 0;

        public static INumberBaseCaller<T>? NumberBase { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = null;
        public static Func<T, T, T>? Addition { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = null;
        public static Func<T, T, T>? Multiply { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = null;

        static NumberTraitsCache() {
            //Register(default!);
            T instance = Activator.CreateInstance<T>();
            if (instance is null) {
                throw new ArgumentNullException(typeof(T).FullName);
            }
            Zero = instance;
            RegisterHash = instance.GetHashCode();
            if (instance is INumberBaseCaller<T> itf) {
                //Debugger.Break();
                NumberBase = itf;
                Zero = itf.Zero;
                //One = itf.CallOne;
                Addition = itf.Addition;
                Multiply = itf.Multiply;
                // 预热.
                try {
                    var TT = TraitsINumberBaseV2<T>.Instance;
                    var t1 = TT.Addition(instance, instance);
                    t1 = TT.Multiply(t1, instance);
                    RegisterHash ^= t1?.GetHashCode() ?? 1;
                } catch (Exception ex) {
                    Debug.WriteLine("The type `" + typeof(T).Name + "` register INumberBaseCaller fail! " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 注册. 能触发静态构造函数.
        /// </summary>
        /// <returns>返回 RegisterHash.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int Register() {
            return RegisterHash;
        }

    }
}
