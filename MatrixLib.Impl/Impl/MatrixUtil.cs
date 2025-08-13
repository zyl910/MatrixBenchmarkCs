using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Zyl.VectorTraits;

namespace MatrixLib.Impl {
    /// <summary>
    /// Matrix utility (矩阵工具).
    /// </summary>
    public static class MatrixUtil {

        /// <summary>
        /// Computes the dot product of two tensors containing single-precision floating-point numbers.
        /// </summary>
        /// <param name="count">The count (数量).</param>
        /// <param name="x">The first vector.</param>
        /// <param name="y">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static float Dot(nint count, ref readonly float x, ref readonly float y) {
            const int LU = 4; // Loop Unrolling.
            if (count <= 0) return 0;
            float rt = 0;
            nint cntBlock = count / (Vector<float>.Count * LU); // Block count.
            nint cntRem = count % (Vector<float>.Count * LU); // Remainder count.
            nint cntLastBlock = cntRem / Vector<float>.Count; // Last block count.
            nint cntLastRem = cntRem % Vector<float>.Count; // Last remainder count.
            ref Vector<float> pVX = ref Unsafe.As<float, Vector<float>>(ref Unsafe.AsRef(in x));
            ref Vector<float> pVY = ref Unsafe.As<float, Vector<float>>(ref Unsafe.AsRef(in y));
            if (cntBlock > 0 || cntLastBlock > 0) {
                Vector<float> vrt = Vector<float>.Zero;
                Vector<float> vrt1 = Vector<float>.Zero;
                Vector<float> vrt2 = Vector<float>.Zero;
                Vector<float> vrt3 = Vector<float>.Zero;
                if (cntBlock > 0) {
                    for (nint i = 0; i < cntBlock; ++i) {
                        vrt = Vector.Add(vrt, Vector.Multiply(pVX, pVY));
                        vrt1 = Vector.Add(vrt1, Vector.Multiply(Unsafe.Add(ref pVX, 1), Unsafe.Add(ref pVY, 1)));
                        vrt2 = Vector.Add(vrt2, Vector.Multiply(Unsafe.Add(ref pVX, 2), Unsafe.Add(ref pVY, 2)));
                        vrt3 = Vector.Add(vrt3, Vector.Multiply(Unsafe.Add(ref pVX, 3), Unsafe.Add(ref pVY, 3)));
                        pVX = ref Unsafe.Add(ref pVX, LU);
                        pVY = ref Unsafe.Add(ref pVY, LU);
                    }
                    vrt = Vector.Add(vrt, vrt1);
                    vrt2 = Vector.Add(vrt2, vrt3);
                    vrt = Vector.Add(vrt, vrt2);
                }
                if (cntLastBlock > 0) {
                    for (nint i = 0; i < cntLastBlock; ++i) {
                        vrt = Vector.Add(vrt, Vector.Multiply(pVX, pVY));
                        pVX = ref Unsafe.Add(ref pVX, 1);
                        pVY = ref Unsafe.Add(ref pVY, 1);
                    }
                }
                rt = Vectors.Sum(vrt);
            }
            if (cntRem > 0) {
                ref float pX = ref Unsafe.As<Vector<float>, float>(ref pVX);
                ref float pY = ref Unsafe.As<Vector<float>, float>(ref pVY);
                for (nint i = 0; i < cntLastRem; ++i) {
                    rt += pX * pY;
                    pX = ref Unsafe.Add(ref pX, 1);
                    pY = ref Unsafe.Add(ref pY, 1);
                }
            }
            return rt;
        }

        /// <inheritdoc cref="Dot(nint, ref readonly float, ref readonly float)"/>
        public static double Dot(nint count, ref readonly double x, ref readonly double y) {
            const int LU = 4; // Loop Unrolling.
            if (count <= 0) return 0;
            double rt = 0;
            nint cntBlock = count / (Vector<double>.Count * LU); // Block count.
            nint cntRem = count % (Vector<double>.Count * LU); // Remainder count.
            nint cntLastBlock = cntRem / Vector<double>.Count; // Last block count.
            nint cntLastRem = cntRem % Vector<double>.Count; // Last remainder count.
            ref Vector<double> pVX = ref Unsafe.As<double, Vector<double>>(ref Unsafe.AsRef(in x));
            ref Vector<double> pVY = ref Unsafe.As<double, Vector<double>>(ref Unsafe.AsRef(in y));
            if (cntBlock > 0 || cntLastBlock > 0) {
                Vector<double> vrt = Vector<double>.Zero;
                Vector<double> vrt1 = Vector<double>.Zero;
                Vector<double> vrt2 = Vector<double>.Zero;
                Vector<double> vrt3 = Vector<double>.Zero;
                if (cntBlock > 0) {
                    for (nint i = 0; i < cntBlock; ++i) {
                        vrt = Vector.Add(vrt, Vector.Multiply(pVX, pVY));
                        vrt1 = Vector.Add(vrt1, Vector.Multiply(Unsafe.Add(ref pVX, 1), Unsafe.Add(ref pVY, 1)));
                        vrt2 = Vector.Add(vrt2, Vector.Multiply(Unsafe.Add(ref pVX, 2), Unsafe.Add(ref pVY, 2)));
                        vrt3 = Vector.Add(vrt3, Vector.Multiply(Unsafe.Add(ref pVX, 3), Unsafe.Add(ref pVY, 3)));
                        pVX = ref Unsafe.Add(ref pVX, LU);
                        pVY = ref Unsafe.Add(ref pVY, LU);
                    }
                    vrt = Vector.Add(vrt, vrt1);
                    vrt2 = Vector.Add(vrt2, vrt3);
                    vrt = Vector.Add(vrt, vrt2);
                }
                if (cntLastBlock > 0) {
                    for (nint i = 0; i < cntLastBlock; ++i) {
                        vrt = Vector.Add(vrt, Vector.Multiply(pVX, pVY));
                        pVX = ref Unsafe.Add(ref pVX, 1);
                        pVY = ref Unsafe.Add(ref pVY, 1);
                    }
                }
                rt = Vectors.Sum(vrt);
            }
            if (cntRem > 0) {
                ref double pX = ref Unsafe.As<Vector<double>, double>(ref pVX);
                ref double pY = ref Unsafe.As<Vector<double>, double>(ref pVY);
                for (nint i = 0; i < cntLastRem; ++i) {
                    rt += pX * pY;
                    pX = ref Unsafe.Add(ref pX, 1);
                    pY = ref Unsafe.Add(ref pY, 1);
                }
            }
            return rt;
        }

        /// <inheritdoc cref="Dot(nint, ref readonly float, ref readonly float)"/>
        public static int Dot(nint count, ref readonly int x, ref readonly int y) {
            const int LU = 4; // Loop Unrolling.
            if (count <= 0) return 0;
            int rt = 0;
            nint cntBlock = count / (Vector<int>.Count * LU); // Block count.
            nint cntRem = count % (Vector<int>.Count * LU); // Remainder count.
            nint cntLastBlock = cntRem / Vector<int>.Count; // Last block count.
            nint cntLastRem = cntRem % Vector<int>.Count; // Last remainder count.
            ref Vector<int> pVX = ref Unsafe.As<int, Vector<int>>(ref Unsafe.AsRef(in x));
            ref Vector<int> pVY = ref Unsafe.As<int, Vector<int>>(ref Unsafe.AsRef(in y));
            if (cntBlock > 0 || cntLastBlock > 0) {
                Vector<int> vrt = Vector<int>.Zero;
                Vector<int> vrt1 = Vector<int>.Zero;
                Vector<int> vrt2 = Vector<int>.Zero;
                Vector<int> vrt3 = Vector<int>.Zero;
                if (cntBlock > 0) {
                    for (nint i = 0; i < cntBlock; ++i) {
                        vrt = Vector.Add(vrt, Vector.Multiply(pVX, pVY));
                        vrt1 = Vector.Add(vrt1, Vector.Multiply(Unsafe.Add(ref pVX, 1), Unsafe.Add(ref pVY, 1)));
                        vrt2 = Vector.Add(vrt2, Vector.Multiply(Unsafe.Add(ref pVX, 2), Unsafe.Add(ref pVY, 2)));
                        vrt3 = Vector.Add(vrt3, Vector.Multiply(Unsafe.Add(ref pVX, 3), Unsafe.Add(ref pVY, 3)));
                        pVX = ref Unsafe.Add(ref pVX, LU);
                        pVY = ref Unsafe.Add(ref pVY, LU);
                    }
                    vrt = Vector.Add(vrt, vrt1);
                    vrt2 = Vector.Add(vrt2, vrt3);
                    vrt = Vector.Add(vrt, vrt2);
                }
                if (cntLastBlock > 0) {
                    for (nint i = 0; i < cntLastBlock; ++i) {
                        vrt = Vector.Add(vrt, Vector.Multiply(pVX, pVY));
                        pVX = ref Unsafe.Add(ref pVX, 1);
                        pVY = ref Unsafe.Add(ref pVY, 1);
                    }
                }
                rt = Vectors.Sum(vrt);
            }
            if (cntRem > 0) {
                ref int pX = ref Unsafe.As<Vector<int>, int>(ref pVX);
                ref int pY = ref Unsafe.As<Vector<int>, int>(ref pVY);
                for (nint i = 0; i < cntLastRem; ++i) {
                    rt += pX * pY;
                    pX = ref Unsafe.Add(ref pX, 1);
                    pY = ref Unsafe.Add(ref pY, 1);
                }
            }
            return rt;
        }

        /// <summary>
        /// Fill value (填充值).
        /// </summary>
        /// <typeparam name="T">The element type (元素的类型).</typeparam>
        /// <param name="value">The value (值).</param>
        /// <param name="rows">The number of rows in matrix (矩阵的行数).</param>
        /// <param name="cols">The number of columns in matrix (矩阵的列数).</param>
        /// <param name="matrix">The matrix (矩阵).</param>
        /// <param name="stride">The stride of matrix. When it is 0, use cols (矩阵的跨距. 为 0 时 使用 cols).</param>
        /// <param name="start">The start index of matrix (矩阵的开始索引).</param>
        public static void Fill<T>(T value, int rows, int cols, Span<T> matrix, int stride = 0, int start = 0) {
            if (0 == stride) {
                stride = cols;
            }
            int idx0 = start;
            for (int i = 0; i < rows; i++) {
                Span<T> span = matrix.Slice(idx0, cols);
                span.Fill(value);
                // Next.
                idx0 += stride;
            }
        }

        /// <summary>
        /// Fill value (填充值).
        /// </summary>
        /// <typeparam name="T">The element type (元素的类型).</typeparam>
        /// <param name="value">The value (值).</param>
        /// <param name="rows">The number of rows in matrix (矩阵的行数).</param>
        /// <param name="cols">The number of columns in matrix (矩阵的列数).</param>
        /// <param name="matrix">The matrix (矩阵).</param>
        /// <param name="stride">The stride of matrix. When it is 0, use cols (矩阵的跨距. 为 0 时 使用 cols).</param>
        public static void Fill<T>(T value, nint rows, nint cols, ref T matrix, nint stride = 0) {
            if (0 == stride) {
                stride = cols;
            }
            int colsInt = (int)cols;
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            bool useSpan = (long)cols < int.MaxValue;
#else
            bool useSpan = false;
#endif
            ref T p0 = ref matrix;
            for (nint i = 0; i < rows; i++) {
                if (useSpan) {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                    Span<T> span = MemoryMarshal.CreateSpan(ref p0, colsInt);
                    span.Fill(value);
#endif
                } else {
                    ref T p = ref p0;
                    for (nint j = 0; j < cols; j++) {
                        p = value;
                        p = ref Unsafe.Add(ref p, 1);
                    }
                }
                // Next.
                p0 = ref Unsafe.Add(ref p0, stride);
            }
        }

        /// <summary>
        /// Matrix transpose (矩阵转置).
        /// </summary>
        /// <typeparam name="T">The element type (元素的类型).</typeparam>
        /// <param name="rows">The number of rows in source matrix (源矩阵的行数).</param>
        /// <param name="cols">The number of columns in source matrix (源矩阵的列数).</param>
        /// <param name="source">The source matrix (源矩阵).</param>
        /// <param name="sourceStride">The stride of source matrix. When it is 0, use cols (源矩阵的跨距. 为 0 时 使用 cols).</param>
        /// <param name="destination">The destination matrix (目标矩阵).</param>
        /// <param name="destinationStride">The stride of destination matrix. When it is 0, use rows (目标矩阵的跨距. 为 0 时 使用 rows).</param>
        /// <param name="sourceStart">The start index of source matrix (源矩阵的开始索引).</param>
        /// <param name="destinationStart">The start index of destination matrix (目标矩阵的开始索引).</param>
        public static void Transpose<T>(nint rows, nint cols, ReadOnlySpan<T> source, nint sourceStride, Span<T> destination, nint destinationStride = 0, nint sourceStart = 0, nint destinationStart = 0) {
            ref T pSrc = ref Unsafe.Add(ref Unsafe.AsRef(in source.GetPinnableReference()), sourceStart);
            ref T pDst = ref Unsafe.Add(ref destination.GetPinnableReference(), destinationStart);
            Transpose(rows, cols, ref pSrc, sourceStride, ref pDst, destinationStride);
        }

        /// <summary>
        /// Matrix transpose (矩阵转置).
        /// </summary>
        /// <typeparam name="T">The element type (元素的类型).</typeparam>
        /// <param name="rows">The number of rows in source matrix (源矩阵的行数).</param>
        /// <param name="cols">The number of columns in source matrix (源矩阵的列数).</param>
        /// <param name="source">The source matrix (源矩阵).</param>
        /// <param name="sourceStride">The stride of source matrix. When it is 0, use rows (源矩阵的跨距. 为 0 时 使用 rows).</param>
        /// <param name="destination">The destination matrix (目标矩阵).</param>
        /// <param name="destinationStride">The stride of destination matrix. When it is 0, use cols (目标矩阵的跨距. 为 0 时 使用 cols).</param>
        /// <remarks>
        /// <para>See more: [mkl_?omatcopy](https://www.intel.com/content/www/us/en/docs/onemkl/developer-reference-c/2025-2/mkl-omatcopy.html)</para>
        /// </remarks>
        public static void Transpose<T>(nint rows, nint cols, ref readonly T source, nint sourceStride, ref T destination, nint destinationStride = 0) {
            ref T pSrc0 = ref Unsafe.AsRef(in source);
            ref T pDst0 = ref destination;
            if (0 == sourceStride) {
                sourceStride = cols;
            }
            if (0 == destinationStride) {
                destinationStride = rows;
            }
            for (nint i = 0; i < cols; i++) {
                ref T pSrc = ref pSrc0;
                ref T pDst = ref pDst0;
                for (nint j = 0; j < rows; j++) {
                    pDst = pSrc;
                    // Next.
                    pSrc = ref Unsafe.Add(ref pSrc, sourceStride);
                    pDst = ref Unsafe.Add(ref pDst, 1);
                }
                pSrc0 = ref Unsafe.Add(ref pSrc0, 1);
                pDst0 = ref Unsafe.Add(ref pDst0, destinationStride);
            }
        }

    }
}
