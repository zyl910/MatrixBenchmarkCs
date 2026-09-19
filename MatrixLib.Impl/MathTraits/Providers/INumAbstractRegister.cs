using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 基本数学访问器的静态注册接口.
    /// </summary>
    public interface IBaseMathCallerRegister : IBaseMathCaller {

#if NET7_0_OR_GREATER
        /// <inheritdoc cref="IBaseMathCallerRegisterDocument.CallerRegister"/>
        public static abstract bool CallerRegister();
#endif // NET7_0_OR_GREATER

    }
}
