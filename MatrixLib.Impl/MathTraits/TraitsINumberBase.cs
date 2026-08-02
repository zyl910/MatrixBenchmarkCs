using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// <see cref="INumberBase{TSelf}"/> 的类型萃取, 无约束.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct TraitsINumberBase<T>
//#if NET9_0_OR_GREATER
//        where T : allows ref struct // 因为泛型数学也不支持 ref struct，而且 ZeroOfTypes 不支持ref struct （它需要静态字段，而 ref struct 不能做成静态字段）.
//#endif // NET9_0_OR_GREATER
    {

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
                if (default(T) is not null) {
                    T caller = default!;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.Addition(left, right);
                    }
                } else {
                    T caller = ZeroOfTypes<T>.Zero;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.Addition(left, right);
                    }
                }
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
                if (default(T) is not null) {
                    T caller = default!;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.Multiply(left, right);
                    }
                } else {
                    T caller = ZeroOfTypes<T>.Zero;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.Multiply(left, right);
                    }
                }
                throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
            }
        }

        /// <summary>
        /// 零值. 为了能够高性能的内联编译, 对于结构体总是返回 default. 若结构体的 default 与 Zero 不同, 请使用 <see cref="ZeroOfTypes{T}"/>.
        /// </summary>
        public T Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (default(T) is not null) {
                    return default!;
                } else {
                    T caller = ZeroOfTypes<T>.Zero;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.Zero;
                    }
                    throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
                }
            }
        }

    }
}
