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
    /// 各类型的零值. 对于 类这样的引用类型、或 零值与default不同的结构体, 应事先给它的 <see cref="Zero"/> 属性赋值, 随后 <see cref="TraitsINumberBase{T}"/> 的 Pi 等属性能正常的获取值.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class ZeroOfTypes<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T> {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        //[MaybeNull]
        public static T Zero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; } = default!;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public static int RegisterHash { get; private set; } = 0;
        
        public static INumberBaseVisitor<T>? NumberBase { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; } = null;
        public static Func<T, T, T>? CallAddition { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; } = null;
        public static Func<T, T, T>? CallMultiply { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; } = null;

        static ZeroOfTypes() {
            Register(default!);
        }

        /// <summary>
        /// 注册.
        /// </summary>
        /// <param name="zero"></param>
        public static void Register(T zero) {
            Zero = zero; // 未来 它应改名 Instance. ZeroOfTypes 应改名 NumberTraitsCache.
            RegisterHash = 0;
            if (zero is null) return;
            if (zero is INumberBaseVisitor<T> itf) {
                //Debugger.Break();
                NumberBase = itf;
                Zero = itf.CallZero; // Instance 、Zero 未来应拆开.
                CallAddition = itf.CallAddition;
                CallMultiply = itf.CallMultiply;
                // 预热.
                try {
                    var TT = TraitsINumberBaseV2<T>.Instance;
                    var t1 = TT.Addition(zero, zero);
                    t1 = TT.Multiply(t1, zero);
                    RegisterHash ^= t1?.GetHashCode() ?? 1;
                } catch (Exception ex) {
                    Debug.WriteLine("The type `" + typeof(T).Name + "` register INumberBaseVisitor fail! " + ex.ToString());
                }
            }
        }

    }
}
