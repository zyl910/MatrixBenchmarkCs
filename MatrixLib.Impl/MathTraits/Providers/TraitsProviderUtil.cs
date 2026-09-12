#define Allow_MakeGenericType

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 类型萃取提供者工具.
    /// </summary>
    public static class TraitsProviderUtil {

        /// <summary>
        /// 填充定义.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <param name="define">定义. 提供者可以填写其中的非空属性.</param>
        /// <param name="caller">调用者.</param>
        /// <param name="instance">实例. 值类型时可空, 引用类型时建议传递 零值.</param>
        /// <returns>返回是否成功.</returns>
        public static bool FillDefine<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, IBaseMathCaller caller, T instance = default!) {
            bool rt = false;
            _ = instance;
            if (caller is null) return rt;
            if (caller is not IBaseMathCaller<T>) return rt;
            // Fill by T.
            if (instance is IEquatable<T> equatable) {
                // OK.
            }
#if NET7_0_OR_GREATER && Allow_MakeGenericType
            //if (instance is INumberBase<T> numberBase) { // CS0314	类型“T”不能用作泛型类型或方法“INumberBase<TSelf>”中的类型参数“TSelf”。没有从“T”到“System.Numerics.INumberBase<T>”的装箱转换或类型参数转换
            //}
            Type numberInterface = typeof(INumberBase<>).MakeGenericType(typeof(T));
            if (numberInterface.IsAssignableFrom(typeof(T))) {
                Type typeCaller = typeof(GenericMaths.TraitsINumberBaseV3<>).MakeGenericType(typeof(T));
                object obj = Activator.CreateInstance(typeCaller)!;
                define.NumberBase = obj as INumberBaseCaller<T>;
                rt = true;
            }
#endif // NET7_0_OR_GREATER
            // Fill by caller.
            if (define.NumberBase is null && caller is INumberBaseCaller<T> itf) {
                define.NumberBase = itf;
                rt = true;
            }
            return rt;
        }

        /// <summary>
        /// 取得定义.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <param name="caller">调用者. 若该类型具有 IBaseMathCaller 系列接口时, 可空.</param>
        /// <param name="instance">实例. 值类型时可空, 引用类型时建议传递 零值.</param>
        /// <returns>返回是否成功.</returns>
        /// <exception cref="ArgumentNullException">请传递 caller 参数!</exception>
        /// <exception cref="NotSupportedException">caller 参数不支持该类型!</exception>
        public static NumberTraitsDefine<T> GetDefine<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif // NET5_0_OR_GREATER
        T>(IBaseMathCaller? caller, T instance = default!) {
            if (caller is null) {
                if (instance is IBaseMathCaller) {
                    var callerT = instance as IBaseMathCaller<T>;
                    if (callerT is null) {
                        throw new ArgumentNullException(nameof(caller), $"请传递 {nameof(caller)} 参数!");
                    }
                    caller = callerT;
                } else {
                    throw new ArgumentNullException(nameof(caller), $"请传递 {nameof(caller)} 参数!");
                }
            }
            NumberTraitsDefine<T> define = new();
            bool rt = FillDefine(define, caller, instance);
            if (!rt) {
                throw new NotSupportedException($"{nameof(caller)} 参数不支持该类型!");
            }
            return define;
        }

        /// <summary>
        /// Preheat (预热).
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        public static int Preheat<T>() {
            int hash = 0;
            try {
                var itf = NumberTraitsCacheV3<T>.NumberBase;
                if (itf is not null) {
                    hash = itf.GetHashCode();
                }
            } catch (Exception ex) {
                Debug.WriteLine("Preheat fail!" + ex);
            }
            return hash;
        }

    }
}
