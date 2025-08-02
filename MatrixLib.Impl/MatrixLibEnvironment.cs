using System;
using System.Diagnostics;

namespace MatrixLib.Impl {
    /// <summary>
    /// MatrixLib Environment (矩阵库的环境).
    /// </summary>
    public static class MatrixLibEnvironment {
        private static bool m_Inited = false;
        private static readonly int m_InitCheckSum;

        /// <summary>
        /// Get init check sum.
        /// </summary>
        public static int InitCheckSum { get => m_InitCheckSum; }

        /// <summary>
        /// Do initialize (进行初始化).
        /// </summary>
        public static void Init() {
            if (m_Inited) return;
            m_Inited = true;
            // Initialize on static constructor.
            // done.
            Debug.WriteLine("MatrixLib.Impl initialize done.");
#if (NETSTANDARD1_1)
#else
            Trace.WriteLine("MatrixLib.Impl initialize done.");
#endif
        }

        static MatrixLibEnvironment() {
            m_InitCheckSum = 0;
        }

    }
}
