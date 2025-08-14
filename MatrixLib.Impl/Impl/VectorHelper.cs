﻿#if NET8_0_OR_GREATER
#else
#define VECTOR_WHERE_STRUCT// Since .NET8, Vector type not have `where T : struct`.
#endif // NET8_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Zyl.ExSpans.Impl;
using Zyl.VectorTraits;
using Zyl.VectorTraits.Extensions.SameW;
using Zyl.VectorTraits.Numerics;

namespace MatrixLib.Impl {
    /// <summary>
    /// <see cref="Vector"/> Helper.
    /// </summary>
    public static class VectorHelper {

        /// <summary>
        /// Extracts the most significant bit from each element in a vector.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="vector">The vector whose elements should have their most significant bit extracted.</param>
        /// <returns>The packed most significant bits extracted from the elements in vector.</returns>
        /// <exception cref="System.NotSupportedException">The type of vector (<typeparamref name="T"/>) is not supported.</exception>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ExtractMostSignificantBits<T>(this Vector<T> vector) where T : struct {
#if NETX_0_OR_GREATER
            return Vector.ExtractMostSignificantBits(vector);
#else
            switch (Unsafe.SizeOf<T>()) {
                case 8:
                    return Vectors.ExtractMostSignificantBits(vector.AsInt64());
                case 4:
                    return Vectors.ExtractMostSignificantBits(vector.AsInt32());
                case 2:
                    return Vectors.ExtractMostSignificantBits(vector.AsInt16());
                default:
                    return Vectors.ExtractMostSignificantBits(vector.AsByte());
            }

#endif // NETX_0_OR_GREATER
        }

        /// <summary>Determines the index of the last element in a vector that is equal to a given value.</summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="vector">The vector whose elements are being checked.</param>
        /// <param name="value">The value to check for in <paramref name="vector" /></param>
        /// <returns>The index into <paramref name="vector" /> representing the last element that was equal to <paramref name="value" />; otherwise, <c>-1</c> if no such element exists.</returns>
        /// <exception cref="NotSupportedException">The type of <paramref name="vector" /> and <paramref name="value" /> (<typeparamref name="T" />) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOf<T>(Vector<T> vector, T value) where T : struct {
#if NET10_0_OR_GREATER
            return Vector.LastIndexOf(vector, value);
#else
            return 63 - MathBitOperations.LeadingZeroCount(ExtractMostSignificantBits(Vector.Equals(vector, Vectors.Create<T>(value))));
#endif // NET10_0_OR_GREATER
        }

        /// <summary>Determines the index of the last element in a vector that has all bits set.</summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="vector">The vector whose elements are being checked.</param>
        /// <returns>The index into <paramref name="vector" /> representing the last element that had all bits set; otherwise, <c>-1</c> if no such element exists.</returns>
        /// <exception cref="NotSupportedException">The type of <paramref name="vector" /> (<typeparamref name="T" />) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOfWhereAllBitsSet<T>(Vector<T> vector) where T : struct {
#if NET10_0_OR_GREATER
            return Vector.LastIndexOf(vector, value);
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
        /// Loads a vector from the given source and element offset.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="source">The source to which elementOffset will be added before loading the vector.</param>
        /// <param name="elementOffset">The element offset from source from which the vector will be loaded.</param>
        /// <returns>The vector loaded from <paramref name="source"/> plus <paramref name="elementOffset"/>.</returns>
        /// <exception cref="System.NotSupportedException">The type of <paramref name="source"/> (<typeparamref name="T"/>) is not supported.</exception>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> LoadUnsafe<T>(ref readonly T source, nuint elementOffset)
#if VECTOR_WHERE_STRUCT
                where T : struct
#endif // VECTOR_WHERE_STRUCT
                {
#if NET8_0_OR_GREATER
            return Vector.LoadUnsafe(in source, elementOffset);
#else
            return Unsafe.As<T, Vector<T>>(ref ExUnsafe.Add(ref Unsafe.AsRef(in source), elementOffset));
#endif // NET8_0_OR_GREATER
        }

        /// <summary>
        /// Loads a vector from the given source.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="source">The source from which the vector will be loaded.</param>
        /// <returns>The vector loaded from <paramref name="source"/>.</returns>
        /// <exception cref="System.NotSupportedException">The type of <paramref name="source"/> (<typeparamref name="T"/>) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> LoadUnsafe<T>(ref readonly T source)
#if VECTOR_WHERE_STRUCT
                where T : struct
#endif // VECTOR_WHERE_STRUCT
                {
#if NET8_0_OR_GREATER
            return Vector.LoadUnsafe(in source);
#else
            return Unsafe.As<T, Vector<T>>(ref Unsafe.AsRef(in source));
#endif // NET8_0_OR_GREATER
        }

        /// <summary>
        /// Stores a vector at the given destination.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="source">The vector that will be stored.</param>
        /// <param name="destination">The destination at which source will be stored.</param>
        /// <exception cref="System.NotSupportedException">The type of <paramref name="source"/> (<typeparamref name="T"/>) is not supported.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreUnsafe<T>(
#if !NET8_0_OR_GREATER
            this
#endif // NET8_0_OR_GREATER
            Vector<T> source, ref T destination)
#if VECTOR_WHERE_STRUCT
                where T : struct
#endif // VECTOR_WHERE_STRUCT
                {
#if NET8_0_OR_GREATER
            Vector.StoreUnsafe(source, ref destination);
#else
            Unsafe.As<T, Vector<T>>(ref destination) = source;
#endif // NET8_0_OR_GREATER
        }

        /// <summary>
        /// Stores a vector at the given destination.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="source">The vector that will be stored.</param>
        /// <param name="destination">The destination to which elementOffset will be added before the vector will be stored.</param>
        /// <param name="elementOffset">The element offset from destination from which the vector will be stored.</param>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreUnsafe<T>(
#if !NET8_0_OR_GREATER
            this
#endif // NET8_0_OR_GREATER
            Vector<T> source, ref T destination, nuint elementOffset)
#if VECTOR_WHERE_STRUCT
                where T : struct
#endif // VECTOR_WHERE_STRUCT
                {
#if NET8_0_OR_GREATER
            Vector.StoreUnsafe(source, ref destination, elementOffset);
#else
            Unsafe.As<T, Vector<T>>(ref ExUnsafe.Add(ref destination, elementOffset)) = source;
#endif // NET8_0_OR_GREATER
        }

        /// <summary>Computes (<paramref name="left"/> * <paramref name="right"/>) + <paramref name="addend"/>, rounded as one ternary operation.</summary>
        /// <param name="left">The vector to be multiplied with <paramref name="right" />.</param>
        /// <param name="right">The vector to be multiplied with <paramref name="left" />.</param>
        /// <param name="addend">The vector to be added to the result of <paramref name="left" /> multiplied by <paramref name="right" />.</param>
        /// <returns>(<paramref name="left"/> * <paramref name="right"/>) + <paramref name="addend"/>, rounded as one ternary operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<float> MultiplyAdd(Vector<float> left, Vector<float> right, Vector<float> addend) {
#if NET9_0_OR_GREATER
            //if (Vector.IsHardwareAccelerated) {
            //    return Vector.FusedMultiplyAdd(left, right, addend);
            //}
            return Vector.FusedMultiplyAdd(left, right, addend);
#else
            return Vector.Add(addend, Vector.Multiply(left, right));
#endif // NET9_0_OR_GREATER
        }

        /// <inheritdoc cref="MultiplyAdd(Vector{float}, Vector{float}, Vector{float})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<double> MultiplyAdd(Vector<double> left, Vector<double> right, Vector<double> addend) {
#if NET9_0_OR_GREATER
            //if (Vector.IsHardwareAccelerated) {
            //    return Vector.FusedMultiplyAdd(left, right, addend);
            //}
            return Vector.FusedMultiplyAdd(left, right, addend);
#else
            return Vector.Add(addend, Vector.Multiply(left, right));
#endif // NET9_0_OR_GREATER
        }

        /// <inheritdoc cref="MultiplyAdd(Vector{float}, Vector{float}, Vector{float})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> MultiplyAdd<T>(Vector<T> left, Vector<T> right, Vector<T> addend)
#if VECTOR_WHERE_STRUCT
            where T : struct
#endif // VECTOR_WHERE_STRUCT
            {
            return Vector.Add(addend, Vector.Multiply(left, right));
        }

    }
}
