using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, IBaseMathCaller caller, T instance = default!) {
            bool rt = false;
            _ = instance;
            if (caller is null) return rt;
            if (caller is not IBaseMathCaller<T>) return rt;
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
        /// <param name="caller">调用者. 若该类型具有 IBaseMathCaller 系列接口时可空.</param>
        /// <param name="instance">实例. 值类型时可空, 引用类型时建议传递 零值.</param>
        /// <returns>返回是否成功.</returns>
        /// <exception cref="ArgumentNullException">请传递 caller 参数!</exception>
        /// <exception cref="NotSupportedException">caller 参数不支持该类型!</exception>
        public static NumberTraitsDefine<T> GetDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(IBaseMathCaller? caller, T instance = default!) {
            if (instance == null) {
                instance = Activator.CreateInstance<T>();
            }
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

    }
}
