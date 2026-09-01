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

        /// <inheritdoc cref="INumberTraitsProvider.FillDefine"/>
        /// <param name="caller">调用者.</param>
        public static bool FillDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, T instance, IBaseMathCaller caller) {
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
    }
}
