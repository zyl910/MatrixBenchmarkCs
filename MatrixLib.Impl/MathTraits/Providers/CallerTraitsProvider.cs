using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 调用者的类型萃取提供者. 若该类型是通过外部类型来提供 IBaseMathCaller 系列接口的, 则使用本类型来处理.
    /// </summary>
    [Obsolete("扩建 Caller 方式, 难以支持AOT方式. 建议改为使用 带 caller 参数的 NumberTraitsManager.Register")]
    public class CallerTraitsProvider : INumberTraitsProvider {

        /// <summary>Caller list (调用者列表).</summary>
        internal List<Type> List { get; } = [];

        public bool FillDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, T instance) {
            throw new NotImplementedException();
        }

    }
}
