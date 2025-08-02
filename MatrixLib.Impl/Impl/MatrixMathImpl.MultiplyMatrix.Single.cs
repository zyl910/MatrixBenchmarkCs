using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Zyl.VectorTraits;

namespace MatrixLib.Impl {
    // My type.
    using TMy = Single;

    partial class MatrixMathImpl {

        public override void MultiplyMatrix(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            ThrowNotSupportedException();
        }

    }
}
