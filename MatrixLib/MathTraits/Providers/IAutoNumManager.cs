using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 数值类型萃取管理器接口.
    /// </summary>
    public interface INumberTraitsManager {

        /// <summary>
        /// 取得类型萃取定义.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <returns>返回已注册的类型萃取项目. 找不到时返回 null.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NumberTraitsDefine<T>? GetDefine<T>();

    }
}
