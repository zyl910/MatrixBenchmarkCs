//#define USE_DELEGATE // 是否使用委托来转发.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// <see cref="INumberBase{TSelf}"/> 的类型萃取, 无约束.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public struct TraitsINumberBaseV3<T>: INumberBaseCaller<T> {
        /// <summary>
        /// 实例.
        /// </summary>
        public static TraitsINumberBaseV3<T> Instance { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new();

        //T INumberBaseCaller<T>.Zero {
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    get {
        //        return Zero;
        //    }
        //}

        //Type IBaseMathCaller.CallElementType => typeof(T);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //T INumberBaseCaller<T>.Addition(T left, T right) {
        //    return Addition(left, right);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //T INumberBaseCaller<T>.Multiply(T left, T right) {
        //    return Multiply(left, right);
        //}

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
                //        return CT.Addition(left, right);
                //    }
                //} else {
                //    T caller = NumberTraitsCacheV3<T>.Zero;
                //    if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                //        return CT.Addition(left, right);
                //    }
                //}
#if USE_DELEGATE
#if USE_ZERO_OF_TYPES
                var func = NumberTraitsCacheV3<T>.Addition;
#else
                var func = NumberTraitsCacheV3<T>.Addition;
#endif // USE_ZERO_OF_TYPES
                if (func is not null) {
                    return func(left, right);
                }
#else // USE_DELEGATE
#if USE_ZERO_OF_TYPES
            var CT = NumberTraitsCacheV3<T>.NumberBase;
#else
                var CT = NumberTraitsCacheV3<T>.NumberBase;
#endif // USE_ZERO_OF_TYPES
                if (CT is not null) {
                    return CT.Addition(left, right);
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
                //        return CT.Multiply(left, right);
                //    }
                //} else {
                //    T caller = NumberTraitsCacheV3<T>.Zero;
                //    if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                //        return CT.Multiply(left, right);
                //    }
                //}
#if USE_IS_NOT_NULL
                if (default(T) is not null) {
                    T caller = default!;
                    if (caller is INumberBaseCaller<T> CT2) {
                        return CT2.Multiply(left, right);
                    }
                }
#endif // USE_IS_NOT_NULL
#if USE_DELEGATE
                var func = NumberTraitsCacheV3<T>.Multiply;
                if (func is not null) {
                    return func(left, right);
                }
#else
                var CT = NumberTraitsCacheV3<T>.NumberBase;
                if (CT is not null) {
                    return CT.Multiply(left, right);
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
                    return NumberTraitsCacheV3<T>.Zero;
                    //var caller = NumberTraitsCacheV3<T>.NumberBase;
                    //if ((caller is not null) && (caller is INumberBaseCaller<T> CT)) {
                    //    return CT.Zero;
                    //}
                    //throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
                }
            }
        }

    }
}
