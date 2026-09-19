using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 数值类型萃取定义的接口.
    /// </summary>
    public interface INumberTraitsDefine {
        /// <summary>Element type (元素类型).</summary>
        Type? CallElementType { get; set; }
    }
}
