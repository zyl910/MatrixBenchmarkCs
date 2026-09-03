using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 接收数值类型的接口.
    /// </summary>
    public interface IRecvNumberType {

        /// <summary>
        /// 接收类型.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        public void RecvType<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>()
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif // NET9_0_OR_GREATER
        ;

    }
}
