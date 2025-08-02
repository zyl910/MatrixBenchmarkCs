using System;
using System.Collections.Generic;
using System.Text;
using Zyl.VectorTraits;

namespace MatrixLib.Impl {
    /// <summary>
    /// Implements of MatrixMathBase (MatrixMathBase 的实现).
    /// </summary>
    public sealed partial class MatrixMathImpl : MatrixMathBase {
        private static readonly MatrixMathImpl _instance = new MatrixMathImpl();

        /// <summary>
        /// The instance (实例).
        /// </summary>
        public static MatrixMathImpl Instance { get { return _instance; } }

        public override string SupportedInstructionSets {
            get {
                return VectorEnvironment.SupportedInstructionSets;
            }
        }

    }
}
