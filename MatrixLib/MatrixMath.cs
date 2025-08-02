using MatrixLib.Impl;
using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]

[assembly: InternalsVisibleTo("MatrixLib.Impl")]

namespace MatrixLib {
    /// <summary>
    /// Matrix Math (矩阵数学).
    /// </summary>
    public static class MatrixMath {
        internal static MatrixMathBase _instance = new MatrixMathBase();

        /// <summary>
        /// The instance (实例).
        /// </summary>
        public static MatrixMathBase Instance { get { return _instance; } }

        /// <summary>
        /// Supported instruction sets. The separator is a comma char ',' (支持的指令集. 分隔符是逗号',').
        /// </summary>
        public static string SupportedInstructionSets {
            get {
                return _instance.SupportedInstructionSets;
            }
        }

    }
}
