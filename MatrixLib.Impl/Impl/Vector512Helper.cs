#if NET8_0_OR_GREATER
#else
#define VECTOR_WHERE_STRUCT// Since .NET8, Vector type not have `where T : struct`.
#endif // NET8_0_OR_GREATER

#if NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using Zyl.ExSpans.Impl;
using Zyl.VectorTraits;

namespace MatrixLib.Impl {
    /// <summary>
    /// <see cref="Vector512"/> Helper.
    /// </summary>
    public static class Vector512Helper {

        /// <summary>Determines the index of the last element in a vector that is equal to a given value.</summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="vector">The vector whose elements are being checked.</param>
        /// <param name="value">The value to check for in <paramref name="vector" /></param>
        /// <returns>The index into <paramref name="vector" /> representing the last element that was equal to <paramref name="value" />; otherwise, <c>-1</c> if no such element exists.</returns>
        /// <exception cref="NotSupportedException">The type of <paramref name="vector" /> and <paramref name="value" /> (<typeparamref name="T" />) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf<T>(Vector512<T> vector, T value) where T : struct {
#if NET10_0_OR_GREATER
            return Vector512.LastIndexOf(vector, value);
#else
            return 63 - BitOperations.LeadingZeroCount(Vector512.ExtractMostSignificantBits(Vector512.Equals(vector, Vector512s.Create<T>(value))));
#endif // NET10_0_OR_GREATER
        }

        /// <summary>Determines the index of the last element in a vector that has all bits set.</summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="vector">The vector whose elements are being checked.</param>
        /// <returns>The index into <paramref name="vector" /> representing the last element that had all bits set; otherwise, <c>-1</c> if no such element exists.</returns>
        /// <exception cref="NotSupportedException">The type of <paramref name="vector" /> (<typeparamref name="T" />) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOfWhereAllBitsSet<T>(Vector512<T> vector) where T : struct {
#if NET10_0_OR_GREATER
            return Vector512.LastIndexOf(vector, value);
#else
            if (typeof(T) == typeof(float)) {
                return LastIndexOf(vector.AsInt32(), -1);
            } else if (typeof(T) == typeof(double)) {
                return LastIndexOf(vector.AsInt64(), -1);
            } else {
                return LastIndexOf(vector, Scalars<T>.AllBitsSet);
            }
#endif // NET10_0_OR_GREATER
        }

        /// <summary>
        /// Loads 3 vector from the given source.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="source">The source from which the vector will be loaded.</param>
        /// <param name="data0">The vector 0 loaded from <paramref name="source"/>.</param>
        /// <param name="data1">The vector 1 loaded from <paramref name="source"/>.</param>
        /// <param name="data2">The vector 2 loaded from <paramref name="source"/>.</param>
        /// <exception cref="System.NotSupportedException">The type of <paramref name="source"/> (<typeparamref name="T"/>) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadX3Unsafe<T>(ref readonly T source, out Vector512<T> data0, out Vector512<T> data1, out Vector512<T> data2)
#if VECTOR_WHERE_STRUCT
                where T : struct
#endif // VECTOR_WHERE_STRUCT
                {
            ref Vector512<T> p = ref Unsafe.As<T, Vector512<T>>(ref Unsafe.AsRef(in source));
            data0 = p;
            data1 = Unsafe.Add(ref p, 1);
            data2 = Unsafe.Add(ref p, 2);
        }

        /// <summary>
        /// Stores 3 vector at the given destination.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="destination">The destination at which source will be stored.</param>
        /// <param name="data0">The vector 0 that will be stored.</param>
        /// <param name="data1">The vector 1 that will be stored.</param>
        /// <param name="data2">The vector 2 that will be stored.</param>
        /// <exception cref="System.NotSupportedException">The type of <paramref name="source"/> (<typeparamref name="T"/>) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreX3Unsafe<T>(ref T destination, Vector512<T> data0, Vector512<T> data1, Vector512<T> data2)
#if VECTOR_WHERE_STRUCT
                where T : struct
#endif // VECTOR_WHERE_STRUCT
                {
            ref Vector512<T> p = ref Unsafe.As<T, Vector512<T>>(ref destination);
            p = data0;
            Unsafe.Add(ref p, 1) = data1;
            Unsafe.Add(ref p, 2) = data2;
        }

        /// <summary>Computes (<paramref name="left"/> * <paramref name="right"/>) + <paramref name="addend"/>, rounded as one ternary operation.</summary>
        /// <param name="left">The vector to be multiplied with <paramref name="right" />.</param>
        /// <param name="right">The vector to be multiplied with <paramref name="left" />.</param>
        /// <param name="addend">The vector to be added to the result of <paramref name="left" /> multiplied by <paramref name="right" />.</param>
        /// <returns>(<paramref name="left"/> * <paramref name="right"/>) + <paramref name="addend"/>, rounded as one ternary operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<float> MultiplyAdd(Vector512<float> left, Vector512<float> right, Vector512<float> addend) {
#if NET9_0_OR_GREATER
            //if (Vector512.IsHardwareAccelerated) {
            //    return Vector512.FusedMultiplyAdd(left, right, addend);
            //}
            return Vector512.FusedMultiplyAdd(left, right, addend);
#else
            return Vector512.Add(addend, Vector512.Multiply(left, right));
#endif // NET9_0_OR_GREATER
        }

        /// <inheritdoc cref="MultiplyAdd(Vector512{float}, Vector512{float}, Vector512{float})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<double> MultiplyAdd(Vector512<double> left, Vector512<double> right, Vector512<double> addend) {
#if NET9_0_OR_GREATER
            //if (Vector512.IsHardwareAccelerated) {
            //    return Vector512.FusedMultiplyAdd(left, right, addend);
            //}
            return Vector512.FusedMultiplyAdd(left, right, addend);
#else
            return Vector512.Add(addend, Vector512.Multiply(left, right));
#endif // NET9_0_OR_GREATER
        }

        /// <inheritdoc cref="MultiplyAdd(Vector512{float}, Vector512{float}, Vector512{float})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> MultiplyAdd<T>(Vector512<T> left, Vector512<T> right, Vector512<T> addend) {
            return Vector512.Add(addend, Vector512.Multiply(left, right));
        }

    }
}

#endif // NET8_0_OR_GREATER
