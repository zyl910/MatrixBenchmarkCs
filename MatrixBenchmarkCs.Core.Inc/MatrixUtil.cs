using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixBenchmarkCs {
    /// <summary>
    /// Matrix utility (矩阵工具).
    /// </summary>
    internal static class MatrixUtil {

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
