//#define USE_DELEGATE // 是否使用委托来转发.
//#define USE_ZERO_OF_TYPES // 是否使用 ZeroOfTypes.
//#define USE_IS_NOT_NULL // 是否使用 `is not null` 来分支处理结构体.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// <see cref="INumberBase{TSelf}"/> 的类型萃取, 无约束.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class TraitsINumberBaseV2<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T>
#if NET9_0_OR_GREATER
        //where T : allows ref struct
#endif // NET9_0_OR_GREATER
    {
        /// <summary>
        /// 实例.
        /// </summary>
        public static TraitsINumberBaseV2<T> Instance { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Addition(T left, T right) {
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
            } else {
                //if (default(T) is not null) {
                //    T caller = default!;
                //    if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                //        return CT.CallAddition(left, right);
                //    }
                //} else {
                //    T caller = ZeroOfTypes<T>.Zero;
                //    if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                //        return CT.CallAddition(left, right);
                //    }
                //}
#if USE_IS_NOT_NULL
                if (default(T) is not null) {
                    T caller = default!;
                    if (caller is INumberBaseVisitor<T> CT2) {
                        return CT2.CallAddition(left, right);
                    }
                }
#endif // USE_IS_NOT_NULL
#if USE_DELEGATE
#if USE_ZERO_OF_TYPES
                var func = ZeroOfTypes<T>.CallAddition;
#else
                var func = NumberTraitsCache<T>.CallAddition;
#endif // USE_ZERO_OF_TYPES
                if (func is not null) {
                    return func(left, right);
                }
#else // USE_DELEGATE
#if USE_ZERO_OF_TYPES
            var CT = ZeroOfTypes<T>.NumberBase;
#else
                var CT = NumberTraitsCache<T>.NumberBase;
#endif // USE_ZERO_OF_TYPES
            if (CT is not null) {
                    return CT.CallAddition(left, right);
                }
#endif // USE_DELEGATE
                throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Multiply(T left, T right) {
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
            } else {
                //if (default(T) is not null) {
                //    T caller = default!;
                //    if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                //        return CT.CallMultiply(left, right);
                //    }
                //} else {
                //    T caller = ZeroOfTypes<T>.Zero;
                //    if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                //        return CT.CallMultiply(left, right);
                //    }
                //}
#if USE_IS_NOT_NULL
                if (default(T) is not null) {
                    T caller = default!;
                    if (caller is INumberBaseVisitor<T> CT2) {
                        return CT2.CallMultiply(left, right);
                    }
                }
#endif // USE_IS_NOT_NULL
#if USE_DELEGATE
                var func = ZeroOfTypes<T>.CallMultiply;
                if (func is not null) {
                    return func(left, right);
                }
#else
                var CT = ZeroOfTypes<T>.NumberBase;
                if (CT is not null) {
                    return CT.CallMultiply(left, right);
                }
#endif
                throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
            }
        }

        public T Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (default(T) is not null) {
                    return default!;
                } else {
                    T caller = ZeroOfTypes<T>.Zero;
                    //if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                    //    return CT.CallZero;
                    //}
                    throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
                }
            }
        }

    }
}
