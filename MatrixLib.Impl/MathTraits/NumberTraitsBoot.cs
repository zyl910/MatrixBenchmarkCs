#define Allow_Obsolete_Code

using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {

    /// <summary>
    /// 数值类型萃取的启动.
    /// </summary>
    public static class NumberTraitsBoot {

        /// <summary>是否已初始化.</summary>
        public static bool Inited { get; private set; } = false;

        /// <summary>初始化后的 Hash.</summary>
        public static int InitedHash { get; private set; } = 0;

        /// <summary>
        /// Static create NumberTraitsBoot.
        /// </summary>
        static NumberTraitsBoot() {
            InitedHash ^= NumberTraitsManager.Instance.GetHashCode();
        }

        /// <summary>
        /// 初始化.
        /// </summary>
        public static void Init() {
            if (Inited) return;
            Inited = true;
            // Preheat common numeric types (常用数值类型).
            // Init Others.
        }

#if Allow_Obsolete_Code
        /// <inheritdoc cref="NumberTraitsManager.Register{T}(IBaseMathCaller?)"/>
        public static bool RegisterCaller1<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(this INumberBaseCaller<T> caller) {
            // CS0120	An object reference is required for the non-static field, method, or property 'NumberTraitsBoot.RegisterCaller1.
            return NumberTraitsManager.Instance.Register<T>(caller);
        }

        /// <inheritdoc cref="NumberTraitsManager.Register{T}(IBaseMathCaller?)"/>
        public static bool RegisterCaller<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T, TCaller>(this TCaller caller, T instance) where TCaller: INumberBaseCaller<T> {
            // An object reference is required for the non-static field, method, or property 'NumberTraitsBoot.RegisterCaller' // 看来 扩展方法无法支持它. C# 14 扩展 可能支持, 但可能有类型推导难题, 故还是用自身静态方法吧.
            return NumberTraitsManager.Instance.Register<T>(caller, instance);
        }
#endif // Allow_Obsolete_Code

        /// <summary>
        /// 接受核心数值类型. 核心数值类型是指 <see cref="System.Numerics.Vector{T}"/> 能支持的类型. e.g. Single, Double, SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64 ...
        /// </summary>
        /// <param name="accepter">接受者.</param>
        /// <remarks>
        /// <para>- .NET 7.0: Add <see cref="nint"/>, <see cref="nuint"/>.</para>
        /// </remarks>
        public static void AcceptTypesCore(INumberTypeAccepter accepter, object? userData = null) {
            Init();
            // AcceptIBinaryFloatingPointIeee754.
            accepter.AcceptIBinaryFloatingPointIeee754<float>(userData);
            accepter.AcceptIBinaryFloatingPointIeee754<double>(userData);
            // AcceptIBinaryIntegerWithSigned.
            accepter.AcceptIBinaryIntegerWithSigned<sbyte>(userData);
            accepter.AcceptIBinaryIntegerWithSigned<short>(userData);
            accepter.AcceptIBinaryIntegerWithSigned<int>(userData);
            accepter.AcceptIBinaryIntegerWithSigned<long>(userData);
            // AcceptIBinaryIntegerWithSigned.
            accepter.AcceptIBinaryIntegerWithUnsigned<byte>(userData);
            accepter.AcceptIBinaryIntegerWithUnsigned<ushort>(userData);
            accepter.AcceptIBinaryIntegerWithUnsigned<uint>(userData);
            accepter.AcceptIBinaryIntegerWithUnsigned<ulong>(userData);
#if NET7_0_OR_GREATER
            accepter.AcceptIBinaryIntegerWithSigned<nint>(userData);
            accepter.AcceptIBinaryIntegerWithUnsigned<nuint>(userData);
#endif // NET7_0_OR_GREATER
        }

        /// <summary>
        /// 接受常用数值类型. 常用数值类型是指 本库原生支持的标量数值类型. 它在 <see cref="SendTypesCore"/> 的基础上, 还增加了 BigInteger, Decimal, Half, Int128, UInt128 ...
        /// </summary>
        /// <param name="accepter">接受者.</param>
        public static void AcceptTypesCommon(INumberTypeAccepter accepter, object? userData = null) {
            AcceptTypesCore(accepter, userData);
            accepter.AcceptIFloatingPoint<decimal>(userData);
            accepter.AcceptIBinaryIntegerWithSigned<BigInteger>(userData);
            accepter.AcceptINumberBaseWithSigned<Complex>(userData);
#if NET7_0_OR_GREATER
#elif (NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER)
            accepter.AcceptIBinaryIntegerWithSigned<nint>(userData);
            accepter.AcceptIBinaryIntegerWithUnsigned<nuint>(userData);
#endif // NET7_0_OR_GREATER
#if NET5_0_OR_GREATER
            accepter.AcceptIBinaryFloatingPointIeee754<Half>(userData);
            accepter.AcceptIBinaryIntegerWithSigned<Int128>(userData);
            accepter.AcceptIBinaryIntegerWithUnsigned<UInt128>(userData);
#endif // NET5_0_OR_GREATER
#if NET11_0_OR_GREATER
            accepter.AcceptIDecimalFloatingPointIeee754<Decimal32>(userData);
            accepter.AcceptIDecimalFloatingPointIeee754<Decimal64>(userData);
            accepter.AcceptIDecimalFloatingPointIeee754<Decimal128>(userData);
#endif // NET11_0_OR_GREATER
        }

        /// <summary>
        /// 发送核心数值类型. 核心数值类型是指 <see cref="System.Numerics.Vector{T}"/> 能支持的类型.
        /// </summary>
        /// <param name="notify">通知者.</param>
        /// <remarks>e.g. Single, Double, SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr</remarks>
        public static void SendTypesCore(IRecvNumberType notify) {
            Init();
            notify.RecvType<float>();
            notify.RecvType<double>();
            notify.RecvType<sbyte>();
            notify.RecvType<byte>();
            notify.RecvType<short>();
            notify.RecvType<ushort>();
            notify.RecvType<int>();
            notify.RecvType<uint>();
            notify.RecvType<long>();
            notify.RecvType<ulong>();
            notify.RecvType<nint>();
            notify.RecvType<nuint>();
        }

        /// <summary>
        /// 发送常用数值类型. 常用数值类型是指 本库原生支持的标量数值类型. 它在 <see cref="SendTypesCore"/> 的基础上, 还增加了 BigInteger, Decimal, Half, Int128, UInt128 ...
        /// </summary>
        /// <param name="notify">通知者.</param>
        public static void SendTypesCommon(IRecvNumberType notify) {
            SendTypesCore(notify);
            notify.RecvType<decimal>();
            notify.RecvType<BigInteger>();
#if NET5_0_OR_GREATER
            notify.RecvType<Half>();
            notify.RecvType<Int128>();
            notify.RecvType<UInt128>();
#endif // NET5_0_OR_GREATER
        }

    }
}
