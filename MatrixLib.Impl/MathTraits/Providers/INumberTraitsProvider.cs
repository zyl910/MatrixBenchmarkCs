using System;
using System.Collections.Generic;
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
        /// <param name="define">定义.</param>
        /// <returns>返回是否成功.</returns>
        public bool FillDefine<T>(NumberTraitsDefine<T> define);
    }
}
