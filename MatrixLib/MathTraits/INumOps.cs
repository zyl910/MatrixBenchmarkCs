using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits {

    /// <summary>
    /// 数值运算接口.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public partial interface INumOps<T> : IBaseMathCaller<T> {

        /// <summary>
        /// 零值.
        /// </summary>
        public T DoZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        /// <summary>
        /// 加法.
        /// </summary>
        /// <param name="left">左值.</param>
        /// <param name="right">右值.</param>
        /// <returns>返回结果.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DoAddition(T left, T right);

        /// <summary>
        /// 乘法.
        /// </summary>
        /// <param name="left">左值.</param>
        /// <param name="right">右值.</param>
        /// <returns>返回结果.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DoMultiply(T left, T right);
    }

}
