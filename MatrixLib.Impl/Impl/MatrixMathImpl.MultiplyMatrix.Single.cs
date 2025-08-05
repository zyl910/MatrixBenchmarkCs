using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zyl.VectorTraits;

namespace MatrixLib.Impl {
    // My type.
    using TMy = Single;

    partial class MatrixMathImpl {
        private bool _Used_MultiplyMatrix = false;

        public override void MultiplyMatrix(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (!_Used_MultiplyMatrix) {
                _Used_MultiplyMatrix = true;
                Console.WriteLine(string.Format(" SupportedInstructionSets: {0}", VectorEnvironment.SupportedInstructionSets));
                Console.WriteLine(string.Format(" TargetFrameworkDisplayName-MatrixMathImpl:\t{0}", VectorTextUtil.GetTargetFrameworkDisplayName(typeof(MatrixMathImpl).Assembly)));
                Console.WriteLine(string.Format(" TargetFrameworkDisplayName-VectorEnvironment:\t{0}", VectorTextUtil.GetTargetFrameworkDisplayName(typeof(VectorEnvironment).Assembly)));
#if NETCOREAPP3_0_OR_GREATER // .NET 9.0 not output.
                Console.WriteLine(string.Format(" RuntimeInformation.FrameworkDescription: {0}", RuntimeInformation.FrameworkDescription));
#endif // NETCOREAPP3_0_OR_GREATER
#if NET8_0_OR_GREATER
                Console.WriteLine(" NET8_0_OR_GREATER");
#endif // NET8_0_OR_GREATER
            }
            MultiplyMatrix_TileRowSimd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
        }

        public void MultiplyMatrix_TileRowRef(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, ref C, strideC);
            // Matrix multiply.
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                ref TMy pA = ref pA0;
                ref TMy pB0 = ref Unsafe.AsRef(in B);
                for (int k = 0; k < K; ++k) {
                    TMy aValue = pA;
                    ref TMy pB = ref pB0;
                    ref TMy pC = ref pC0;
                    for (int j = 0; j < N; ++j) {
                        pC += aValue * pB;
                        pB = ref Unsafe.Add(ref pB, 1);
                        pC = ref Unsafe.Add(ref pC, 1);
                    }
                    pA = ref Unsafe.Add(ref pA, 1);
                    pB0 = ref Unsafe.Add(ref pB0, strideB);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        public void MultiplyMatrix_TileRowSimd(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                MultiplyMatrix_TileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, ref C, strideC);
            // Matrix multiply.
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                ref TMy pA = ref pA0;
                ref TMy pB0 = ref Unsafe.AsRef(in B);
                for (int k = 0; k < K; ++k) {
                    TMy aValue = pA;
                    Vector<TMy> vA = new Vector<TMy>(aValue);
                    // Last.
                    int pos = N - Vector<TMy>.Count;
                    ref Vector<TMy> pBLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pB0, pos));
                    ref Vector<TMy> pCLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pC0, pos));
                    Vector<TMy> vCLast = Vector.Add(Vectors.Multiply(vA, pBLast), pCLast);
                    // SIMD for.
                    if (cntBlock >= 0) {
                        ref Vector<TMy> pB = ref Unsafe.As<TMy, Vector<TMy>>(ref pB0);
                        ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                        for (int j = 0; j < cntBlock; ++j) {
                            pC = Vector.Add(Vectors.Multiply(vA, pB), pC); // pC += vA * pB;
                            pB = ref Unsafe.Add(ref pB, 1);
                            pC = ref Unsafe.Add(ref pC, 1);
                        }
                    }
                    pCLast = vCLast; // Overrride remainder items. 
                    // Next.
                    pA = ref Unsafe.Add(ref pA, 1);
                    pB0 = ref Unsafe.Add(ref pB0, strideB);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        public unsafe void MultiplyMatrix_TileRowSimdParallel(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            bool allowParallel = (M >= 16) && (Environment.ProcessorCount > 1);
            if (allowParallel) {
                fixed (TMy* pA0 = &A, pB0 = &B, pC0 = &C) {
                    nint addressA = (nint)pA0;
                    nint addressB = (nint)pB0;
                    nint addressC = (nint)pC0;
                    Parallel.For(0, M, i => {
                        ref TMy pA = ref Unsafe.AsRef<TMy>((void*)addressA);
                        ref TMy pB = ref Unsafe.AsRef<TMy>((void*)addressB);
                        ref TMy pC = ref Unsafe.AsRef<TMy>((void*)addressC);
                        MultiplyMatrix_TileRowSimd(1, N, K, ref Unsafe.Add(ref pA, strideA * i), strideA, ref pB, strideB, ref Unsafe.Add(ref pC, strideC * i), strideC);
                    });
                }
            } else {
                MultiplyMatrix_TileRowSimd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            }
        }

    }
}
