using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>数值类型接受者. 类型参数有 DynamicallyAccessedMembers 特性, 能支持 Native AOT .</summary>
    /// <inheritdoc cref="INumberTypeAccepter"/>
    public class NumberTypeAccepter : INumberTypeAccepter {

        /// <summary>
        /// Create NumberTypeAccepter.
        /// </summary>
        public NumberTypeAccepter() {
        }

        /// <summary>
        /// Create NumberTypeAccepter, has child, acceptTypeAction params.
        /// </summary>
        /// <param name="child">Child accepter (子接受者).</param>
        /// <param name="acceptTypeAction">接受类型的动作.</param>
        public NumberTypeAccepter(INumberTypeAccepter? child, TypeAction? acceptTypeAction = null) {
            Child = child;
            AcceptTypeAction = acceptTypeAction;
        }

        public virtual void AcceptIBinaryFloatingPointIeee754<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryFloatingPointIeee754<T>
#endif // NET7_0_OR_GREATER
        {
            AcceptIFloatingPointIeee754<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptIBinaryFloatingPointIeee754<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIBinaryInteger<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryInteger<T>
#endif // NET7_0_OR_GREATER
        {
            AcceptIBinaryNumber<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptIBinaryInteger<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIBinaryIntegerWithSigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryInteger<T>, ISignedNumber<T>
#endif // NET7_0_OR_GREATER
        {
            if (0 == thisDeep) {
                AcceptINumberBaseWithSigned<T>(userData, thisDeep + 1);
                AcceptIBinaryInteger<T>(userData, thisDeep + 1);
                Child?.AcceptIBinaryIntegerWithSigned<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIBinaryIntegerWithUnsigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryInteger<T>, IUnsignedNumber<T>
#endif // NET7_0_OR_GREATER
        {
            if (0 == thisDeep) {
                AcceptINumberBaseWithUnsigned<T>(userData, thisDeep + 1);
                AcceptIBinaryInteger<T>(userData, thisDeep + 1);
                Child?.AcceptIBinaryIntegerWithUnsigned<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIBinaryNumber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IBinaryNumber<T>
#endif // NET7_0_OR_GREATER
        {
            AcceptINumber<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptIBinaryNumber<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIDecimalFloatingPointIeee754<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IFloatingPointIeee754<T>
#if NET11_0_OR_GREATER
            , IDecimalFloatingPointIeee754<T>
#endif // NET11_0_OR_GREATER
#endif // NET7_0_OR_GREATER
        {
            AcceptIFloatingPointIeee754<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptIDecimalFloatingPointIeee754<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIEquatable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T> {
            AcceptType<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptIEquatable<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIFloatingPoint<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IFloatingPoint<T>
#endif // NET7_0_OR_GREATER
        {
            AcceptINumber<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptIFloatingPoint<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptIFloatingPointIeee754<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , IFloatingPointIeee754<T>
#endif // NET7_0_OR_GREATER
        {
            AcceptIFloatingPoint<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptIFloatingPointIeee754<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptINumber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumber<T>
#endif // NET7_0_OR_GREATER
        {
            AcceptINumberBase<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptINumber<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptINumberBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            AcceptIEquatable<T>(userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptINumberBase<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptINumberBaseWithSigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>, ISignedNumber<T>
#endif // NET7_0_OR_GREATER
        {
            if (0 == thisDeep) {
                AcceptINumberBase<T>(userData, thisDeep + 1);
                Child?.AcceptINumberBaseWithSigned<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptINumberBaseWithUnsigned<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) where T : IEquatable<T>
#if NET7_0_OR_GREATER
            , INumberBase<T>, IUnsignedNumber<T>
#endif // NET7_0_OR_GREATER
        {
            if (0 == thisDeep) {
                AcceptINumberBase<T>(userData, thisDeep + 1);
                Child?.AcceptINumberBaseWithUnsigned<T>(userData, thisDeep);
            }
        }

        public virtual void AcceptType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) {
            AcceptTypeAction?.Invoke(typeof(T), userData, thisDeep + 1);
            if (0 == thisDeep) {
                Child?.AcceptType<T>(userData, thisDeep);
            }
        }

        public virtual TypeAction? AcceptTypeAction { get; set; }

        public virtual INumberTypeAccepter? Child { get; set; }

    }
}
