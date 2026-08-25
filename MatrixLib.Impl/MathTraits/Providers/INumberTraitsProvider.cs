using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 数值类型萃取提供者接口.
    /// </summary>
    public interface INumberTraitsProvider {
        /// <summary>
        /// 填充定义.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <param name="define">定义. 提供者可以填写其中的非空属性.</param>
        /// <param name="instance">实例.</param>
        /// <returns>返回是否成功.</returns>
        public bool FillDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, T instance);
    }
}
