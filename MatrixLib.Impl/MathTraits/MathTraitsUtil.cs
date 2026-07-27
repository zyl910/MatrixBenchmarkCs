using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数学类型萃取工具.
    /// </summary>
    public static class MathTraitsUtil {

#if NET7_0_OR_GREATER

        /// <summary>
        /// 计算平方和, 使用泛型数学的运算符.
        /// </summary>
        /// <typeparam name="T">元素的类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumGenericOp<T>(ReadOnlySpan<T> src) where T: INumberBase<T> {
            T rt = T.Zero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                rt += p * p;
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

#endif // NET7_0_OR_GREATER

    }
}
