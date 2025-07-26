using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixBenchmarkCs {
    /// <summary>
    /// Check sum utility (校验和工具).
    /// </summary>
    internal static class CheckSumUtil {

        /// <summary>
        /// Calculate the checksum of a 2D buffer. Used Span (计算二维缓冲区的校验和. 使用Span).
        /// </summary>
        /// <param name="buffer">The buffer (缓冲区).</param>
        /// <param name="width">The width (宽度).</param>
        /// <param name="height">The height (高度).</param>
        /// <param name="stride">The stride. When it is 0, use width (跨距. 为 0 时 使用 width).</param>
        /// <param name="start">The start index (开始索引).</param>
        /// <returns>Returns check sum (返回校验和).</returns>
        public static int Calculate2D(Span<int> buffer, nint width, nint height, nint stride = 0, nint start = 0) {
            ref int p = ref Unsafe.Add(ref buffer[0], start);
            return Calculate2D(ref p, width, height, stride);
        }

        /// <summary>
        /// Calculate the checksum of a 2D buffer. Used ref (计算二维缓冲区的校验和. 使用引用).
        /// </summary>
        /// <param name="buffer">The buffer (缓冲区).</param>
        /// <param name="width">The width (宽度).</param>
        /// <param name="height">The height (高度).</param>
        /// <param name="stride">The stride. When it is 0, use width (跨距. 为 0 时 使用 width).</param>
        /// <returns>Returns check sum (返回校验和).</returns>
        public static int Calculate2D(ref readonly int buffer, nint width, nint height, nint stride = 0) {
            int rt = default;
            ref int p0 = ref Unsafe.AsRef(in buffer);
            if (0 == stride) {
                stride = width;
            }
            for (nint i = 0; i < height; ++i) {
                ref int p = ref p0;
                for (int j = 0; j < width; ++j) {
                    rt += p;
                    // Next.
                    p = ref Unsafe.Add(ref p, 1);
                }
                p0 = ref Unsafe.Add(ref p0, stride);
            }
            return rt;
        }

    }
}
