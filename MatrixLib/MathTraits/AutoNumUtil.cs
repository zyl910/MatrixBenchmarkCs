using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

//[assembly: InternalsVisibleTo("MatrixLib.Impl")]

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值类型萃取工具. 计划改名为 NumberTraitsManagerUtil
    /// </summary>
    public static class NumberTraitsUtil {
        //public delegate NumberTraitsItem<T>? GetDefineFunc<T>();
        //public static GetDefineFunc? ItemFunc { get; internal set; } // CS0305	Using the generic type 'TraitsManager.ObtainItemFunc<T>' requires 1 type arguments
        /// <summary>
        /// 类型萃取管理者.
        /// </summary>
        public static INumberTraitsManager? TraitsManager { get; internal set; } // Set by NumberTraitsManager of `MatrixLib.Impl`.

        /// <inheritdoc cref="INumberTraitsManager.GetDefine{T}"/>
        public static NumberTraitsDefine<T>? GetDefine<T>() {
            NumberTraitsDefine<T>? rt = null;
            if (TraitsManager is not null) {
                rt = TraitsManager.GetDefine<T>();
            }
            return rt;
        }

        /// <summary>
        /// 是否为核心数值类型. 核心数值类型是指 <see cref="System.Numerics.Vector{T}"/> 能支持的类型. e.g. Single, Double, SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64 ...
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <returns>若是则返回 true，否则返回 false.</returns>
        /// <remarks>
        /// <para>- .NET 7.0: Add <see cref="nint"/>, <see cref="nuint"/>.</para>
        /// </remarks>
        public static bool IsCoreTypes<T>() {
            if (typeof(T) == typeof(float)
                || typeof(T) == typeof(double)
                || typeof(T) == typeof(sbyte)
                || typeof(T) == typeof(short)
                || typeof(T) == typeof(int)
                || typeof(T) == typeof(long)
                || typeof(T) == typeof(byte)
                || typeof(T) == typeof(ushort)
                || typeof(T) == typeof(uint)
                || typeof(T) == typeof(ulong)
#if NET7_0_OR_GREATER
                || typeof(T) == typeof(nint)
                || typeof(T) == typeof(nuint)
#endif // NET7_0_OR_GREATER
                ) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否为公共数值类型. 公共数值类型是指 本库原生支持的标量数值类型. 它在 <see cref="SendTypesCore"/> 的基础上, 还增加了 BigInteger, Decimal, Half, Int128, UInt128 ...
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <returns>若是则返回 true，否则返回 false.</returns>
        public static bool IsCommonTypes<T>() {
            if (IsCoreTypes<T>()) {
                return true;
            } else if (typeof(T) == typeof(decimal)
                || typeof(T) == typeof(BigInteger)
                || typeof(T) == typeof(Complex)
#if NET7_0_OR_GREATER
#else
                || typeof(T) == typeof(nint)
                || typeof(T) == typeof(nuint)
#endif // NET7_0_OR_GREATER
#if NET5_0_OR_GREATER
                || typeof(T) == typeof(Half)
                || typeof(T) == typeof(Int128)
                || typeof(T) == typeof(UInt128)
#endif // NET5_0_OR_GREATER
#if NET11_0_OR_GREATER
                || typeof(T) == typeof(Decimal32)
                || typeof(T) == typeof(Decimal64)
                || typeof(T) == typeof(Decimal128)
#endif // NET11_0_OR_GREATER
                ) {
                return true;
            }
            return false;
        }

    }

}
