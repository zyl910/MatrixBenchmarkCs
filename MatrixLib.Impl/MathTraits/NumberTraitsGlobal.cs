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
    /// 数值类型萃取的全局配置.
    /// </summary>
    public static class NumberTraitsGlobal {

        /// <summary>是否已初始化.</summary>
        public static bool Inited { get; private set; } = false;

        /// <summary>初始化后的 Hash.</summary>
        public static int InitedHash { get; private set; } = 0;

        /// <summary>
        /// Static create NumberTraitsGlobal.
        /// </summary>
        static NumberTraitsGlobal() {
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
        /// 发送常用数值类型. 常用数值类型是指 本库原生支持的标量数值类型. 它在 <see cref="SendTypesCore"/> 的基础上, 还增加了 BigInteger, Decimal, Half, Int128, UInt128 .
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
