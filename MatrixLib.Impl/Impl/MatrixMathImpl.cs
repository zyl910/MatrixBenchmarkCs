using System;
using System.Collections.Generic;
using System.Text;
using Zyl.VectorTraits;

namespace MatrixLib.Impl {
    /// <summary>
    /// Implements of MatrixMathBase (MatrixMathBase 的实现).
    /// </summary>
    public sealed partial class MatrixMathImpl : MatrixMathBase {

        public override string SupportedInstructionSets {
            get {
                return VectorEnvironment.SupportedInstructionSets;
            }

        }

    }
}
