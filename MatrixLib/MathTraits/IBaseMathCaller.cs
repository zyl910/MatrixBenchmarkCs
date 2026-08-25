using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 基本数学访问器接口.
    /// </summary>
    public interface IBaseMathCaller {
        /// <summary>
        /// Element type (元素类型).
        /// </summary>
        Type CallElementType { get; }
    }
}
