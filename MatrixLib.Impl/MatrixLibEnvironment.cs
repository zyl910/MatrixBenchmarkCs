global using BLASLONG = long;
global using FLOAT = float;

using MatrixLib.Impl;
using System;
using System.Diagnostics;
using Zyl.VectorTraits;

[assembly: CLSCompliant(true)]

namespace MatrixLib {
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
        /// <remarks>If MatrixMath's methods throws NotImplementedException exception, please call `MatrixLibEnvironment.Init` method of `MatrixLib.Impl` first (若 MatrixMath 的方法抛出 NotSupportedException 异常, 请先调用 `MatrixLib.Impl` 的 `MatrixLibEnvironment.Init` 方法).</remarks>
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
            MatrixMath._instance = MatrixMathImpl.Instance;
            m_InitCheckSum = MatrixMath.Instance.GetHashCode();
        }

    }
}
