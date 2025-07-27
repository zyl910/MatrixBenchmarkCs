using MKLNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixBenchmarkCs {
    internal static class MklDemo {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Call(TextWriter writer) {
            int m = 2, n = 3, k = 4;
            double[] a = { 1, 2, 3, 4, 5, 6, 7, 8 };
            double[] b = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            double[] c = new double[m * n];

            Blas.gemm(Layout.RowMajor, Trans.No, Trans.No, m, n, k, 1.0, a, k, b, n, 0.0, c, n);

            writer.WriteLine("Mkl Result:");
            for (int i = 0; i < m; i++) {
                for (int j = 0; j < n; j++) {
                    writer.Write(c[i * n + j] + " ");
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }

    }
}
