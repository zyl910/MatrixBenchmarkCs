using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
#if NET7_0_OR_GREATER
using TraitsNS = MatrixLib.MathTraits.GenericMaths;
#else
using TraitsNS = MatrixLib.MathTraits;
//using TraitsINumberBase_Where<T> = TraitsINumberBase<T>; // 语法不支持.
#endif // NET7_0_OR_GREATER

    /// <summary>
    /// 数学类型萃取工具.
    /// </summary>
    public static class MathTraitsUtil {

        public static void ThrowNotSupportedType(Type type0) {
            throw new NotSupportedException(string.Format("Not supported type {0}!", type0.FullName));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TypeOf_Addition<T>(T left, T right) {
            if (typeof(T) == typeof(float)) {
                var temp = Unsafe.As<T, float>(ref left) + Unsafe.As<T, float>(ref right); return Unsafe.As<float, T>(ref temp);
            } else if (typeof(T) == typeof(double)) {
                var temp = Unsafe.As<T, double>(ref left) + Unsafe.As<T, double>(ref right); return Unsafe.As<double, T>(ref temp);
            } else if (typeof(T) == typeof(sbyte)) {
                var temp = (sbyte)(Unsafe.As<T, sbyte>(ref left) + Unsafe.As<T, sbyte>(ref right)); return Unsafe.As<sbyte, T>(ref temp);
            } else if (typeof(T) == typeof(byte)) {
                var temp = (byte)(Unsafe.As<T, byte>(ref left) + Unsafe.As<T, byte>(ref right)); return Unsafe.As<byte, T>(ref temp);
            } else if (typeof(T) == typeof(short)) {
                var temp = (short)(Unsafe.As<T, short>(ref left) + Unsafe.As<T, short>(ref right)); return Unsafe.As<short, T>(ref temp);
            } else if (typeof(T) == typeof(ushort)) {
                var temp = (ushort)(Unsafe.As<T, ushort>(ref left) + Unsafe.As<T, ushort>(ref right)); return Unsafe.As<ushort, T>(ref temp);
            } else if (typeof(T) == typeof(int)) {
                var temp = Unsafe.As<T, int>(ref left) + Unsafe.As<T, int>(ref right); return Unsafe.As<int, T>(ref temp);
            } else if (typeof(T) == typeof(uint)) {
                var temp = Unsafe.As<T, uint>(ref left) + Unsafe.As<T, uint>(ref right); return Unsafe.As<uint, T>(ref temp);
            } else if (typeof(T) == typeof(long)) {
                var temp = Unsafe.As<T, long>(ref left) + Unsafe.As<T, long>(ref right); return Unsafe.As<long, T>(ref temp);
            } else if (typeof(T) == typeof(ulong)) {
                var temp = Unsafe.As<T, ulong>(ref left) + Unsafe.As<T, ulong>(ref right); return Unsafe.As<ulong, T>(ref temp);
            }
            throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TypeOf_Multiply<T>(T left, T right) {
            if (typeof(T) == typeof(float)) {
                var temp = Unsafe.As<T, float>(ref left) * Unsafe.As<T, float>(ref right); return Unsafe.As<float, T>(ref temp);
            } else if (typeof(T) == typeof(double)) {
                var temp = Unsafe.As<T, double>(ref left) * Unsafe.As<T, double>(ref right); return Unsafe.As<double, T>(ref temp);
            } else if (typeof(T) == typeof(sbyte)) {
                var temp = (sbyte)(Unsafe.As<T, sbyte>(ref left) * Unsafe.As<T, sbyte>(ref right)); return Unsafe.As<sbyte, T>(ref temp);
            } else if (typeof(T) == typeof(byte)) {
                var temp = (byte)(Unsafe.As<T, byte>(ref left) * Unsafe.As<T, byte>(ref right)); return Unsafe.As<byte, T>(ref temp);
            } else if (typeof(T) == typeof(short)) {
                var temp = (short)(Unsafe.As<T, short>(ref left) * Unsafe.As<T, short>(ref right)); return Unsafe.As<short, T>(ref temp);
            } else if (typeof(T) == typeof(ushort)) {
                var temp = (ushort)(Unsafe.As<T, ushort>(ref left) * Unsafe.As<T, ushort>(ref right)); return Unsafe.As<ushort, T>(ref temp);
            } else if (typeof(T) == typeof(int)) {
                var temp = Unsafe.As<T, int>(ref left) * Unsafe.As<T, int>(ref right); return Unsafe.As<int, T>(ref temp);
            } else if (typeof(T) == typeof(uint)) {
                var temp = Unsafe.As<T, uint>(ref left) * Unsafe.As<T, uint>(ref right); return Unsafe.As<uint, T>(ref temp);
            } else if (typeof(T) == typeof(long)) {
                var temp = Unsafe.As<T, long>(ref left) * Unsafe.As<T, long>(ref right); return Unsafe.As<long, T>(ref temp);
            } else if (typeof(T) == typeof(ulong)) {
                var temp = Unsafe.As<T, ulong>(ref left) * Unsafe.As<T, ulong>(ref right); return Unsafe.As<ulong, T>(ref temp);
            }
            throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TypeOfAs_Addition<T>(T left, T right) {
            if (typeof(T) == typeof(float)) {
                UnsafeAs<T, float> AsT; return AsT.From(AsT.To(left) + AsT.To(right));
            } else if (typeof(T) == typeof(double)) {
                UnsafeAs<T, double> AsT; return AsT.From(AsT.To(left) + AsT.To(right));
            } else if (typeof(T) == typeof(sbyte)) {
                UnsafeAs<T, sbyte> AsT; return AsT.From((sbyte)(AsT.To(left) + AsT.To(right)));
            } else if (typeof(T) == typeof(byte)) {
                UnsafeAs<T, byte> AsT; return AsT.From((byte)(AsT.To(left) + AsT.To(right)));
            } else if (typeof(T) == typeof(short)) {
                UnsafeAs<T, short> AsT; return AsT.From((short)(AsT.To(left) + AsT.To(right)));
            } else if (typeof(T) == typeof(ushort)) {
                UnsafeAs<T, ushort> AsT; return AsT.From((ushort)(AsT.To(left) + AsT.To(right)));
            } else if (typeof(T) == typeof(int)) {
                UnsafeAs<T, int> AsT; return AsT.From(AsT.To(left) + AsT.To(right));
            } else if (typeof(T) == typeof(uint)) {
                UnsafeAs<T, uint> AsT; return AsT.From(AsT.To(left) + AsT.To(right));
            } else if (typeof(T) == typeof(long)) {
                UnsafeAs<T, long> AsT; return AsT.From(AsT.To(left) + AsT.To(right));
            } else if (typeof(T) == typeof(ulong)) {
                UnsafeAs<T, ulong> AsT; return AsT.From(AsT.To(left) + AsT.To(right));
            }
            throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TypeOfAs_Multiply<T>(T left, T right) {
            if (typeof(T) == typeof(float)) {
                UnsafeAs<T, float> AsT; return AsT.From(AsT.To(left) * AsT.To(right));
            } else if (typeof(T) == typeof(double)) {
                UnsafeAs<T, double> AsT; return AsT.From(AsT.To(left) * AsT.To(right));
            } else if (typeof(T) == typeof(sbyte)) {
                UnsafeAs<T, sbyte> AsT; return AsT.From((sbyte)(AsT.To(left) * AsT.To(right)));
            } else if (typeof(T) == typeof(byte)) {
                UnsafeAs<T, byte> AsT; return AsT.From((byte)(AsT.To(left) * AsT.To(right)));
            } else if (typeof(T) == typeof(short)) {
                UnsafeAs<T, short> AsT; return AsT.From((short)(AsT.To(left) * AsT.To(right)));
            } else if (typeof(T) == typeof(ushort)) {
                UnsafeAs<T, ushort> AsT; return AsT.From((ushort)(AsT.To(left) * AsT.To(right)));
            } else if (typeof(T) == typeof(int)) {
                UnsafeAs<T, int> AsT; return AsT.From(AsT.To(left) * AsT.To(right));
            } else if (typeof(T) == typeof(uint)) {
                UnsafeAs<T, uint> AsT; return AsT.From(AsT.To(left) * AsT.To(right));
            } else if (typeof(T) == typeof(long)) {
                UnsafeAs<T, long> AsT; return AsT.From(AsT.To(left) * AsT.To(right));
            } else if (typeof(T) == typeof(ulong)) {
                UnsafeAs<T, ulong> AsT; return AsT.From(AsT.To(left) * AsT.To(right));
            }
            throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
        }

#if NET7_0_OR_GREATER

        /// <summary>
        /// 计算平方和, 使用泛型数学的运算符.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumGenericOp<T>(ReadOnlySpan<T> src) where T: INumberBase<T> {
            T rt = T.Zero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                rt += p * p;
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        ///// <summary>
        ///// 计算平方和, 使用泛型数学的方法.
        ///// </summary>
        ///// <typeparam name="T">元素类型.</typeparam>
        ///// <param name="src">源数据.</param>
        ///// <returns>返回结算结果.</returns>
        //public static T SumGenericMethod<T>(ReadOnlySpan<T> src) where T : INumberBase<T> {
        //    T rt = T.Zero; // Result.
        //    int srcCount = src.Length;
        //    ref T p = ref Unsafe.AsRef(in src[0]);
        //    for (int i = 0; i < srcCount; ++i) {
        //        var temp = T.op_Multiply(p, p); // CS0571	'IMultiplyOperators<T, T, T>.operator *(T, T)': cannot explicitly call operator or accessor
        //        rt = T.op_Addition(rt, temp); // CS0571	'IAdditionOperators<T, T, T>.operator +(T, T)': cannot explicitly call operator or accessor
        //        // Next.
        //        p = ref Unsafe.Add(ref p, 1);
        //    }
        //    return rt;
        //}

#endif // NET7_0_OR_GREATER

        /// <summary>
        /// 计算平方和, 使用 TypeOf_Addition 等函数.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T? SumRawTypeOf<T>(ReadOnlySpan<T> src) {
            T? rt = default; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = TypeOf_Multiply(p, p);
                rt = TypeOf_Addition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和, 使用 TypeOfAs_Addition 等函数.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T? SumRawTypeOfAs<T>(ReadOnlySpan<T> src) {
            T? rt = default; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = TypeOfAs_Multiply(p, p);
                rt = TypeOfAs_Addition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和, 直接使用 <see cref="TraitsINumberBase{T}"/> 来计算.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumTraitsRaw<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(ReadOnlySpan<T> src) {
            TraitsINumberBase<T> TT;
            MathTrait.OutINumberBase(out var TT1, default(T));
            T rt = TT.Zero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = TT.Multiply(p, p);
                rt = TT.Addition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和, 使用 OutINumberBase 来计算.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumTraitsOut<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(ReadOnlySpan<T> src)
#if NET7_0_OR_GREATER
            where T : INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            MathTrait.OutINumberBase(out var TT, default(T)!); // 不可靠.
            T rt = TT.Zero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = TT.Multiply(p, p);
                rt = TT.Addition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和, 使用 using  来计算.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumTraitsUsing<T>(ReadOnlySpan<T> src)
#if NET7_0_OR_GREATER
            where T : INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            TraitsNS.TraitsINumberBase<T> TT;
            T rt = TT.Zero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            for (int i = 0; i < srcCount; ++i) {
                var temp = TT.Multiply(p, p);
                rt = TT.Addition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和, 使用 TraitsINumberBaseV2 来计算.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumTraitsV2Raw<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(ReadOnlySpan<T> src)
        {
            var TT = TraitsINumberBaseV2<T>.Instance;
            T rt = TT.Zero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            //if (true) {
            //    bool flag = (p is INumberBaseCaller<T>);
            //    Console.WriteLine("Is INumberBaseCaller: {0}", flag);
            //}
            for (int i = 0; i < srcCount; ++i) {
                var temp = TT.Multiply(p, p);
                rt = TT.Addition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和, 使用 using TraitsINumberBaseV2 来计算.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumTraitsV2Using<T>(ReadOnlySpan<T> src)
#if NET7_0_OR_GREATER
            where T : INumberBase<T>
#endif // NET7_0_OR_GREATER
        {
            var TT = TraitsNS.TraitsINumberBaseV2<T>.Instance;
            T rt = TT.Zero; // Result.
            int srcCount = src.Length;
            ref T p = ref Unsafe.AsRef(in src[0]);
            //if (true) {
            //    bool flag = (p is INumberBaseCaller<T>);
            //    Console.WriteLine("Is INumberBaseCaller: {0}", flag);
            //}
            for (int i = 0; i < srcCount; ++i) {
                var temp = TT.Multiply(p, p);
                rt = TT.Addition(rt, temp);
                // Next.
                p = ref Unsafe.Add(ref p, 1);
            }
            return rt;
        }

        /// <summary>
        /// 计算平方和, 使用 INumberBaseCaller 来计算.
        /// </summary>
        /// <typeparam name="T">元素类型.</typeparam>
        /// <param name="src">源数据.</param>
        /// <returns>返回结算结果.</returns>
        public static T SumCallerIn<T, TCaller>(TCaller TV, ReadOnlySpan<T> src)
            where TCaller: INumberBaseCaller<T>
#if NET7_0_OR_GREATER
		//where T : INumberBase<T> // 可忽略.
#endif // NET7_0_OR_GREATER
		{
			T rt = TV.Zero; // Result.
			int srcCount = src.Length;
			ref T p = ref Unsafe.AsRef(in src[0]);
			for (int i = 0; i < srcCount; ++i) {
				var temp = TV.Multiply(p, p);
				rt = TV.Addition(rt, temp);
				// Next.
				p = ref Unsafe.Add(ref p, 1);
			}
			return rt;
		}

		/// <summary>
		/// 计算平方和, 使用 GetCallerItf  来计算.
		/// </summary>
		/// <typeparam name="T">元素类型.</typeparam>
		/// <param name="src">源数据.</param>
		/// <returns>返回结算结果.</returns>
		public static T SumCallerGetItf<T>(ReadOnlySpan<T> src)
#if NET7_0_OR_GREATER
			where T : INumberBase<T>
#endif // NET7_0_OR_GREATER
		{
			var TV = MathTrait.GetCallerItf<T>();
			T rt = TV.Zero; // Result.
			int srcCount = src.Length;
			ref T p = ref Unsafe.AsRef(in src[0]);
			for (int i = 0; i < srcCount; ++i) {
				var temp = TV.Multiply(p, p);
				rt = TV.Addition(rt, temp);
				// Next.
				p = ref Unsafe.Add(ref p, 1);
			}
			return rt;
		}

		/// <summary>
		/// 计算平方和, 使用 OutCaller  来计算.
		/// </summary>
		/// <typeparam name="T">元素类型.</typeparam>
		/// <param name="src">源数据.</param>
		/// <returns>返回结算结果.</returns>
		public static T SumCallerOut<T>(ReadOnlySpan<T> src)
#if NET7_0_OR_GREATER
			where T : INumberBase<T>
#endif // NET7_0_OR_GREATER
		{
			MathTrait.OutCaller<T, INumberBaseCaller<T>>(out var TV);
			T rt = TV.Zero; // Result.
			int srcCount = src.Length;
			ref T p = ref Unsafe.AsRef(in src[0]);
			for (int i = 0; i < srcCount; ++i) {
				var temp = TV.Multiply(p, p);
				rt = TV.Addition(rt, temp);
				// Next.
				p = ref Unsafe.Add(ref p, 1);
			}
			return rt;
		}

	}
}
