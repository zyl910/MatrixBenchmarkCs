using MatrixLib.MathTraits.Proxies;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        //public static T SumSquaresOnDynamic<T>(ReadOnlySpan<T> src) {
        //    T rt = default(T)!; // Result.
        //    int srcCount = src.Length;
        //    ref T p = ref Unsafe.AsRef(in src[0]);
        //    for (int i = 0; i < srcCount; ++i) {
        //        dynamic p1 = p!;
        //        rt += p1 * p1;
        //        // Next.
        //        p = ref Unsafe.Add(ref p, 1);
        //    }
        //    return rt;
        //}

        /// <summary>计算平方和 - 基于代理.</summary>
        /// <inheritdoc cref="SumSquares{T}"/>
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

        //public static U SumSquaresU<U>(ReadOnlySpan<U> src) {
        //    return default!;
        //}

        /// <summary>计算平方和 - 基于代理调用. 性能与直接调用代理相同, 主要是为了演示泛型函数转发.</summary>
        /// <inheritdoc cref="SumSquares{T}"/>
        public static T SumSquaresOnProxyCall<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>(ReadOnlySpan<NumberBaseProxy<T>> src) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>
#endif // NET7_0_OR_GREATER
            {
            //return SumSquaresU(src); // Error: CS0029 Cannot implicitly convert type 'NumberBaseProxy<T>' to 'T'
            //return SumSquaresU<NumberBaseProxy<T>>(src); // Error: CS0029 Cannot implicitly convert type 'NumberBaseProxy<T>' to 'T'.
            //return SumSquaresU(src).Value; // OK. 问题并不是在 NumberBaseProxy、ReadOnlySpan, 而是 返回值的类型必须一致.
            // -- body --
            //var call = NumberBaseProxy<NumberBaseProxy<T>>.Zero; // Error on SumSquaresCall: CS0315 The type 'NumberBaseProxy<NumberBaseProxy<T>>' cannot be used as type parameter 'TCaller' in the generic type or method 'DemoTraitsUtilCommon.SumSquaresCall<T, TCaller>(TCaller, ReadOnlySpan<T>)'. There is no boxing conversion from 'NumberBaseProxy<NumberBaseProxy<T>>' to 'INumOps<NumberBaseProxy<T>>'.
            var call = NumberBaseProxy<T>.Zero; // OK.
            //var call = NumberBaseProxy<T>.Zero as INumOps<T>; // Error on SumSquaresCall: CS0311 The type 'INumOps<T>' cannot be used as type parameter 'TCaller' in the generic type or method 'DemoTraitsUtilCommon.SumSquaresCall<T, TCaller>(TCaller, ReadOnlySpan<T>)'. There is no implicit reference conversion from 'INumOps<T>' to 'INumOps<NumberBaseProxy<T>>'.
            //var call = NumberBaseProxy<T>.Zero as INumOps<NumberBaseProxy<T>>; // OK.
            //return SumSquaresCall(call, src); // Error: CS0029 Cannot implicitly convert type 'NumberBaseProxy<T>' to 'T'
            return SumSquaresCall(call, src).Value;
        }

    }
}
