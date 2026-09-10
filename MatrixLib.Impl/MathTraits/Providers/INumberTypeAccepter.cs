using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {

    /// <summary>
    /// 类型的委托. 参数有 DynamicallyAccessedMembers 特性, 能支持 Native AOT .
    /// </summary>
    /// <param name="type">The type (类型).</param>
    /// <param name="userData">用户自定义数据.</param>
    /// <param name="thisDeep">本类中的深度. 初始为0, 逐层调用基类型的方法时, 会 +1 .</param>
    public delegate void TypeAction([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, object? userData = null, int thisDeep = 0);


    /// <summary>
    /// 数值类型接受者接口. 类型参数有 DynamicallyAccessedMembers 特性, 能支持 Native AOT .
    /// </summary>
    /// <remarks>
    /// <para>当 AcceptINumber 等方法被调用时, 会根据约束的继承树, 逐层调用基类型的方法, 直至 AcceptType, 期间 thisDeep 每次会+1. 对于名称含有“With”的方法, 仅在 thisDeep 为0时才会调用基类型的方法.</para>
    /// <para>当派生类 override 方法时, 建议先执行自己的代码, 再调用基类.</para>
    /// <para>- AcceptIBinaryInteger: AcceptIBinaryNumber: AcceptINumber: AcceptINumberBase: AcceptIEquatable: AcceptType: AcceptTypeAction</para>
    /// <para>- AcceptIBinaryFloatingPointIeee754: AcceptIFloatingPointIeee754: IFloatingPoint: AcceptINumber // e.g. <see cref="float"/>, <see cref="double"/>, <see cref="Half"/></para>
    /// <para>- AcceptIDecimalFloatingPointIeee754: AcceptIFloatingPointIeee754 // e.g. <see cref="decimal"/> </para>
    /// <para>- AcceptIBinaryIntegerWithSigned: AcceptIBinaryInteger // e.g. <see cref="sbyte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="nint"/>, <see cref="Int128"/>, <see cref="BigInteger"/></para>
    /// <para>- AcceptIBinaryIntegerWithUnsigned: AcceptIBinaryInteger // e.g. <see cref="byte"/>, <see cref="ushort"/>, <see cref="uint"/>, <see cref="ulong"/>, <see cref="nuint"/>, <see cref="UInt128"/></para>
    /// <para>- AcceptINumberBaseWithSigned: AcceptINumberBase // e.g. <see cref="Complex"/></para>
    /// </remarks>
    public interface INumberTypeAccepter : ITypeAccepter {

        /// <summary>接受 <see cref="IBinaryFloatingPointIeee754{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIBinaryFloatingPointIeee754<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryFloatingPointIeee754<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="IBinaryInteger{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIBinaryInteger<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryInteger<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="IBinaryInteger{T}"/> 与 <see cref="ISignedNumber{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIBinaryIntegerWithSigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryInteger<T>, ISignedNumber<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="IBinaryInteger{T}"/> 与 <see cref="IUnsignedNumber{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIBinaryIntegerWithUnsigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryInteger<T>, IUnsignedNumber<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="IBinaryNumber{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIBinaryNumber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryNumber<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="IDecimalFloatingPointIeee754{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIDecimalFloatingPointIeee754<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IFloatingPointIeee754<T>
#if NET11_0_OR_GREATER
            , IDecimalFloatingPointIeee754<T>
#endif // NET11_0_OR_GREATER
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="IEquatable{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIEquatable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>;

        /// <summary>接受 <see cref="IFloatingPoint{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIFloatingPoint<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IFloatingPoint<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="IFloatingPointIeee754{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptIFloatingPointIeee754<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IFloatingPointIeee754<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="INumber{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptINumber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumber<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="INumberBase{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptINumberBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="INumberBase{T}"/> 与 <see cref="ISignedNumber{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptINumberBaseWithSigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>, ISignedNumber<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>接受 <see cref="INumberBase{T}"/> 与 <see cref="IUnsignedNumber{T}"/> 约束的类型.</summary>
        /// <inheritdoc cref="AcceptType"/>
        public void AcceptINumberBaseWithUnsigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>, IUnsignedNumber<T>
#endif // NET7_0_OR_GREATER
        ;

        /// <summary>
        /// 接受无泛型约束的类型.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <param name="userData">用户自定义数据.</param>
        /// <param name="thisDeep">本类中的深度. 初始为0, 逐层调用基类型的方法时, 会 +1 .</param>
        public void AcceptType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0);

        /// <summary>
        /// 接受类型的动作.
        /// </summary>
        public TypeAction? AcceptTypeAction { get; set; }

        /// <summary>
        /// Child accepter (子接受者).
        /// </summary>
        public INumberTypeAccepter? Child { get; set; }

    }

}
