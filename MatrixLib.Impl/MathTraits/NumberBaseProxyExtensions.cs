using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// Extensions of <see cref="NumberBaseProxy{T}"/> (的扩展).
    /// </summary>
    public static class NumberBaseProxyExtensions {

        /// <summary>
        /// 将 ReadOnlySpan&lt;NumberBaseProxy&lt;T&gt;&gt; 转为 ReadOnlySpan&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回转换结果.</returns>
        public static ReadOnlySpan<T> AsFromNumberBaseProxy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>(this ReadOnlySpan<NumberBaseProxy<T>> src) where T : struct, IEquatable<T>
#if NET7_0_OR_GREATER
        , INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            return MemoryMarshal.Cast<NumberBaseProxy<T>, T>(src);
        }

        /// <summary>
        /// 将 Span&lt;NumberBaseProxy&lt;T&gt;&gt; 转为 Span&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回转换结果.</returns>
        public static Span<T> AsFromNumberBaseProxy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>(this Span<NumberBaseProxy<T>> src) where T : struct, IEquatable<T>
#if NET7_0_OR_GREATER
        , INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            return MemoryMarshal.Cast<NumberBaseProxy<T>, T>(src);
        }

        /// <summary>
        /// 将 ReadOnlySpan&lt;T&gt;Span&lt; 转为 ReadOnlySpan&lt;NumberBaseProxy&lt;T&gt;&gt;.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回转换结果.</returns>
        public static ReadOnlySpan<NumberBaseProxy<T>> AsNumberBaseProxy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>(this ReadOnlySpan<T> src) where T : struct, IEquatable<T>
#if NET7_0_OR_GREATER
        , INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            return MemoryMarshal.Cast<T, NumberBaseProxy<T>>(src);
        }

        /// <summary>
        /// 将 Span&lt;T&gt;Span&lt; 转为 Span&lt;NumberBaseProxy&lt;T&gt;&gt;.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回转换结果.</returns>
        public static Span<NumberBaseProxy<T>> AsNumberBaseProxy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        T>(this Span<T> src) where T : struct, IEquatable<T>
#if NET7_0_OR_GREATER
        , INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            return MemoryMarshal.Cast<T, NumberBaseProxy<T>>(src);
        }

    }

}
