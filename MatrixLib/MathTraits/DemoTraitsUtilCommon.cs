using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 演示-类型萃取工具-公共.
    /// </summary>
    public abstract class DemoTraitsUtilCommon {

        /// <summary>
        /// 计算平方和, 使用 INumberBaseCaller 来计算.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumCallerIn<T, TCaller>(TCaller TC, ReadOnlySpan<T> src)
            where TCaller : INumberBaseCaller<T>
#if NET7_0_OR_GREATER
		//where T : INumberBase<T> // 可忽略.
#endif // NET7_0_OR_GREATER
        {
            T rt = TC.CallZero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = TC.CallMultiply(p, p);
                rt = TC.CallAddition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

    }
}
