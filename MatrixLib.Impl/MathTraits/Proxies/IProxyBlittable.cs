using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies {

    /// <summary>
    /// 可按位复制的代理接口. struct 才可实现此接口, 且需保证它是可按位复制的. 即能够通过 <see cref="UnsafeAs{TFrom, TTo}"/>, 将自身类型, 与 <see cref="GetBlittableType"/> 返回的类型进行重新解释.
    /// </summary>
    public interface IProxyBlittable {

        /// <summary>
        /// 取得可按位复制的类型. 即能够通过 <see cref="UnsafeAs{TFrom, TTo}"/>, 将自身类型, 与返回的类型进行重新解释.
        /// </summary>
        /// <returns>返回可按位复制的类型.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type GetBlittableType();

    }

}
