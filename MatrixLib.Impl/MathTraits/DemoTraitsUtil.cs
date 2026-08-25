using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits {
#if NET7_0_OR_GREATER
using TraitsNS = MatrixLib.MathTraits.GenericMaths;
#else
using TraitsNS = MatrixLib.MathTraits;
//using TraitsINumberBase_Where<T> = TraitsINumberBase<T>; // 语法不支持.
#endif // NET7_0_OR_GREATER

    /// <summary>
    /// 演示-类型萃取工具-公共.
    /// </summary>
    public abstract class DemoTraitsUtil: DemoTraitsUtilCommon {

        /// <summary>
        /// 计算平方和.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumSquares<T>(ReadOnlySpan<T> src)
#if NET7_0_OR_GREATER
            where T : INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            var TT = TraitsNS.TraitsINumberBaseV3<T>.Instance;
            return SumSquaresCall(TT, src);
        }

    }
}
