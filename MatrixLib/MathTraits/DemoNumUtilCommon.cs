using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits {

    /// <summary>
    /// 演示-类型萃取工具-公共.
    /// </summary>
    public abstract partial class DemoTraitsUtilCommon {

        /// <summary>
        /// 计算平方和, 带有 <typeparamref name="TCaller"/> 泛型参数.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumSquaresCall<T, TCaller>(TCaller TC, ReadOnlySpan<T> src)
            where TCaller : INumberBaseCaller<T>
#if NET7_0_OR_GREATER
		//where T : INumberBase<T> // 可忽略.
#endif // NET7_0_OR_GREATER
        {
            T rt = TC.DoZero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = TC.DoMultiply(p, p);
                rt = TC.DoAddition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和-自动判断.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumSquaresIf<T>(ReadOnlySpan<T> src) {
            T num = default!;
            if (num is not null && num is INumberBaseCaller<T> ops) {
                return SumSquaresCall(ops, src);
            } else {
                return SumSquaresCall(TraitsINumberBaseV3<T>.Instance, src);
            }
        }

    }
}
