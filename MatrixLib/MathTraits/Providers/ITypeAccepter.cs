using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixLib.MathTraits.Providers {

    /// <summary>
    /// 类型接受者. 类型参数有 DynamicallyAccessedMembers 特性, 能支持 Native AOT .
    /// </summary>
    /// <remarks>当 AcceptIEquatable 等方法被调用时, 会根据约束的继承树, 逐层调用基类型的方法, 直至 AcceptType, 期间 thisDeep 每次会+1. 对于名称含有“With”的方法, 仅在 thisDeep 为0时才会调用基类型的方法.</remarks>
    public interface ITypeAccepter {

#if FALSE
        // 【废弃】 因 IComparable 未继承 IEquatable .
        /// <summary>接受 <see cref="IComparable{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIComparable<T>(object? userData = null, int thisDeep = 0) where T : IComparable<T>;

        // 【待改】需要 DynamicallyAccessedMembers.
        /// <summary>接受 <see cref="IEquatable{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIEquatable<T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>;

        /// <summary>
        /// 接受无泛型约束的类型.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <param name="userData">用户自定义数据.</param>
        /// <param name="thisDeep">本类中的深度. 初始为0, 逐层调用基类型的方法时, 会 +1 .</param>
        public void AcceptType<T>(object? userData = null, int thisDeep = 0);
#endif

    }

}
