using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        new public static T SumSquares<T>(ReadOnlySpan<T> src)
#if NET7_0_OR_GREATER
            where T : INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            var TT = TraitsNS.TraitsINumberBaseV3<T>.Instance;
            return SumSquaresCall(TT, src);
        }

        public static T SumSquaresOnProxy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            T>(ReadOnlySpan<NumberBaseProxy<T>> src) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>
#endif // NET7_0_OR_GREATER
            {
            NumberBaseProxy<T> rt = NumberBaseProxy<T>.Zero; // Result.
            int srcCount = src.Length;
            ref NumberBaseProxy<T> p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = p + p;
                rt += p * p;
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt.Value;
        }

    }
}
