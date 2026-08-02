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
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class TraitsINumberBaseV2<T>
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
                if (default(T) is not null) {
                    T caller = default!;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.CallAddition(left, right);
                    }
                } else {
                    T caller = ZeroOfTypes<T>.Zero;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.CallAddition(left, right);
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
                        return CT.CallMultiply(left, right);
                    }
                } else {
                    T caller = ZeroOfTypes<T>.Zero;
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.CallMultiply(left, right);
                    }
                }
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
                    if ((caller is not null) && (caller is INumberBaseVisitor<T> CT)) {
                        return CT.CallZero;
                    }
                    throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
                }
            }
        }

    }
}
