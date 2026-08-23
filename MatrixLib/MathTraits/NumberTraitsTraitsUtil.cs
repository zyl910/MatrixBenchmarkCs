using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值类型萃取工具.
    /// </summary>
    public static class NumberTraitsTraitsUtil {
        //public delegate NumberTraitsItem<T>? GetDefineFunc<T>();
        //public static GetDefineFunc? ItemFunc { get; internal set; } // CS0305	Using the generic type 'TraitsManager.ObtainItemFunc<T>' requires 1 type arguments
        public static INumberTraitsManager? TraitsManager { get; internal set; }

        /// <inheritdoc cref="INumberTraitsManager.GetDefine{T}"/>
        public static NumberTraitsDefine<T>? GetDefine<T>() {
            NumberTraitsDefine<T>? rt = null;
            if (TraitsManager is not null) {
                rt = TraitsManager.GetDefine<T>();
            }
            return rt;
        }
    }

}
