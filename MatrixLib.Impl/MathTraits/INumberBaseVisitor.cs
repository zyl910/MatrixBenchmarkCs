using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits {

    /// <summary>
    /// <see cref="INumberBase{TSelf}"/> 的访问器.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public interface INumberBaseVisitor<T>: IBaseMathVisitor<T> {

        /// <summary>
        /// 加法.
        /// </summary>
        /// <param name="left">左值.</param>
        /// <param name="right">右值.</param>
        /// <returns>返回结果.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Addition(T left, T right);

        /// <summary>
        /// 乘法.
        /// </summary>
        /// <param name="left">左值.</param>
        /// <param name="right">右值.</param>
        /// <returns>返回结果.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Multiply(T left, T right);

        /// <summary>
        /// 零值.
        /// </summary>
        public T Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }

}
