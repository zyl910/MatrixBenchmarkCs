using System;

namespace MatrixLib.MathTraits {
using TraitsNS = MatrixLib.MathTraits;

    partial class DemoTraitsUtilCommon {

        /// <summary>
        /// 计算平方和.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        [Obsolete("It is recommended to replace it with DemoTraitsUtil, which has better performance. Except the scenario does not support DemoTraitsUtil (建议用性能更好的 DemoTraitsUtil 来替代. 除非场景不支持 DemoTraitsUtil).")]
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
