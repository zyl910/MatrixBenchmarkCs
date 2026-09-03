using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {

    /// <summary>
    /// 自身类型萃取提供者. 若该类型自己已实现了 IBaseMathCaller 系列接口, 则使用本类型来处理.
    /// </summary>
    public class SelfTraitsProvider : INumberTraitsProvider {

        public bool FillDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, T instance) {
            bool rt = false;
            if (instance is IBaseMathCaller caller) {
                rt = TraitsProviderUtil.FillDefine(define, caller, instance);
            }
            return rt;
        }

    }
}
