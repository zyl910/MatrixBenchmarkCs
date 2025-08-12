#define Tensor_Primitives_ALLOW_T

using MatrixLib.Impl;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif // NETCOREAPP3_0_OR_GREATER
using System.Text;
using System.Threading.Tasks;
using Zyl.VectorTraits;

namespace MatrixBenchmarkCs.MultiplyMatrix {

    // My type.
    using TMy = Single;

    partial class MultiplyMatrixStatic {

        /// <summary>
        /// Basic on Array.
        /// </summary>
        /// <param name="M">The number of rows in matrix A (矩阵A的行数).</param>
        /// <param name="N">The number of columns in matrix B (矩阵B的列数).</param>
        /// <param name="K">The number of columns in matrix A, or the number of rows in matrix B (矩阵A的列数, 或矩阵B的行数).</param>
        /// <param name="A">Matrix A.</param>
        /// <param name="strideA">Stride of A.</param>
        /// <param name="B">Matrix B.</param>
        /// <param name="strideB">Stride of B.</param>
        /// <param name="C">Matrix C.</param>
        /// <param name="strideC">Stride of C.</param>
        public static void StaticBasic(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            // Matrix multiply.
            for (int i = 0; i < M; ++i) {
                for (int j = 0; j < N; ++j) {
                    int cIdx = i * strideC + j;
                    C[cIdx] = 0;
                    for (int k = 0; k < K; ++k) {
                        int aIdx = i * strideA + k;
                        int bIdx = k * strideB + j;
                        C[cIdx] += A[aIdx] * B[bIdx];
                    }
                }
            }
        }

        /// <summary>Basic on Span.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBasicSpan(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            // Matrix multiply.
            int aIdx0 = 0;
            //int bIdx0 = 0;
            int cIdx0 = 0;
            for (int i = 0; i < M; ++i) {
                int cIdx = cIdx0;
                for (int j = 0; j < N; ++j) {
                    TMy cur = 0;
                    int aIdx = aIdx0;
                    int bIdx = j;
                    for (int k = 0; k < K; ++k) {
                        cur += A[aIdx] * B[bIdx];
                        ++aIdx;
                        bIdx += strideB;
                    }
                    C[cIdx] = cur;
                    ++cIdx;
                }
                aIdx0 += strideA;
                cIdx0 += strideC;
            }
        }

        /// <summary>Basic on Ref.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBasicRef(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            // Matrix multiply.
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pB0 = ref Unsafe.AsRef(in B);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                ref TMy pC = ref pC0;
                for (int j = 0; j < N; ++j) {
                    TMy cur = 0;
                    ref TMy pA = ref pA0;
                    ref TMy pB = ref Unsafe.Add(ref pB0, j);
                    for (int k = 0; k < K; ++k) {
                        cur += pA * pB;
                        pA = ref Unsafe.Add(ref pA, 1);
                        pB = ref Unsafe.Add(ref pB, strideB);
                    }
                    pC = cur;
                    pC = ref Unsafe.Add(ref pC, 1);
                }
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        /// <summary>Transpose on Array.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTranspose(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            // Transpose matrix B.
            int total = K * N;
            TMy[] BTrans = ArrayPool<TMy>.Shared.Rent(total);
            try {
                MatrixUtil.Transpose(K, N, B, strideB, BTrans.AsSpan());
                // Matrix multiply.
                for (int i = 0; i < M; ++i) {
                    for (int j = 0; j < N; ++j) {
                        int cIdx = i * strideC + j;
                        C[cIdx] = 0;
                        for (int k = 0; k < K; ++k) {
                            int aIdx = i * strideA + k;
                            int bIdx = j * strideB + k;
                            C[cIdx] += A[aIdx] * BTrans[bIdx];
                        }
                    }
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(BTrans);
            }
        }

        /// <summary>Tile row on Array (行分块 on 数组).</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRow(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            // Clear matrix C.
            //C.AsSpan().Clear();
            MatrixUtil.Fill((TMy)0, M, N, C, strideC);
            // Matrix multiply.
            for (int i = 0; i < M; ++i) {
                for (int k = 0; k < K; ++k) {
                    int aIdx = i * strideA + k;
                    for (int j = 0; j < N; ++j) {
                        int bIdx = k * strideB + j;
                        int cIdx = i * strideC + j;
                        C[cIdx] += A[aIdx] * B[bIdx];
                    }
                }
            }
        }

        /// <summary>TileRow on Span.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowSpan(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, C, strideC);
            // Matrix multiply.
            int aIdx0 = 0;
            int cIdx0 = 0;
            for (int i = 0; i < M; ++i) {
                int aIdx = aIdx0;
                int bIdx0 = 0;
                for (int k = 0; k < K; ++k) {
                    int bIdx = bIdx0;
                    int cIdx = cIdx0;
                    for (int j = 0; j < N; ++j) {
                        C[cIdx] += A[aIdx] * B[bIdx];
                        ++bIdx;
                        ++cIdx;
                    }
                    ++aIdx;
                    bIdx0 += strideB;
                }
                aIdx0 += strideA;
                cIdx0 += strideC;
            }
        }

        /// <summary>TileRow on Ref.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowRef(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector<TMy> LinearWriteSimd_Vector(int K, int strideB, ref TMy pA0, ref TMy pB0) {
            Vector<TMy> cur = Vector<TMy>.Zero;
            ref TMy pA = ref pA0;
            ref TMy pB = ref pB0;
            for (int k = 0; k < K; ++k) {
                //cur += pA * pB;
                Vector<TMy> vA = new Vector<TMy>(pA);
                cur = Vector.Add(Vectors.Multiply(vA, Unsafe.As<TMy, Vector<TMy>>(ref pB)), cur); // pC += vA * pB;
                pA = ref Unsafe.Add(ref pA, 1);
                pB = ref Unsafe.Add(ref pB, strideB);
            }
            return cur;
        }

        /// <summary>LinearWrite on Ref Simd.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticLinearWriteSimd(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
            // Matrix multiply.
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pB0 = ref Unsafe.AsRef(in B);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                // Make last.
                int pos = N - Vector<TMy>.Count;
                ref Vector<TMy> pCLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pC0, pos));
                ref TMy pBLast = ref Unsafe.Add(ref pB0, pos);
                Vector<TMy> vCLast = LinearWriteSimd_Vector(K, strideB, ref pA0, ref pBLast);
                // SIMD for.
                ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                ref TMy pBBlock = ref pB0;
                for (int j = 0; j < cntBlock; ++j) {
                    Vector<TMy> cur = LinearWriteSimd_Vector(K, strideB, ref pA0, ref pBBlock);
                    pC = cur;
                    pBBlock = ref Unsafe.Add(ref pBBlock, Vector<TMy>.Count);
                    pC = ref Unsafe.Add(ref pC, 1);
                }
                pCLast = vCLast; // Overrride remainder items. 
                // Next.
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

        /// <summary>LinearWrite on Ref Simd - Loop Unrolling.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticLinearWriteSimdLU(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int LU = 4; // Loop Unrolling 4.
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
            int cntBlockUL = cntBlock / LU;
            cntBlock = cntBlock % LU;
            // Matrix multiply.
            ref TMy pA0 = ref Unsafe.AsRef(in A);
            ref TMy pB0 = ref Unsafe.AsRef(in B);
            ref TMy pC0 = ref C;
            for (int i = 0; i < M; ++i) {
                // Make last.
                int pos = N - Vector<TMy>.Count;
                ref Vector<TMy> pCLast = ref Unsafe.As<TMy, Vector<TMy>>(ref Unsafe.Add(ref pC0, pos));
                ref TMy pBLast = ref Unsafe.Add(ref pB0, pos);
                Vector<TMy> vCLast = LinearWriteSimd_Vector(K, strideB, ref pA0, ref pBLast);
                // SIMD for cntBlockUL.
                ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                ref TMy pBBlock = ref pB0;
                for (int j = 0; j < cntBlockUL; ++j) {
                    Vector<TMy> cur = Vector<TMy>.Zero;
                    Vector<TMy> cur1 = Vector<TMy>.Zero;
                    Vector<TMy> cur2 = Vector<TMy>.Zero;
                    Vector<TMy> cur3 = Vector<TMy>.Zero;
                    ref TMy pA = ref pA0;
                    ref TMy pB = ref pBBlock;
                    for (int k = 0; k < K; ++k) {
                        //cur += pA * pB;
                        Vector<TMy> vA = new Vector<TMy>(pA);
                        ref Vector<TMy> pBCur = ref Unsafe.As<TMy, Vector<TMy>>(ref pB);
                        cur = Vector.Add(Vectors.Multiply(vA, pBCur), cur);
                        cur1 = Vector.Add(Vectors.Multiply(vA, Unsafe.Add(ref pBCur, 1)), cur1);
                        cur2 = Vector.Add(Vectors.Multiply(vA, Unsafe.Add(ref pBCur, 2)), cur2);
                        cur3 = Vector.Add(Vectors.Multiply(vA, Unsafe.Add(ref pBCur, 3)), cur3);
                        pA = ref Unsafe.Add(ref pA, 1);
                        pB = ref Unsafe.Add(ref pB, strideB);
                    }
                    pC = cur;
                    Unsafe.Add(ref pC, 1) = cur1;
                    Unsafe.Add(ref pC, 2) = cur2;
                    Unsafe.Add(ref pC, 3) = cur3;
                    pBBlock = ref Unsafe.Add(ref pBBlock, Vector<TMy>.Count * LU);
                    pC = ref Unsafe.Add(ref pC, LU);
                }
                // SIMD for cntBlock.
                for (int j = 0; j < cntBlock; ++j) {
                    Vector<TMy> cur = LinearWriteSimd_Vector(K, strideB, ref pA0, ref pBBlock);
                    pC = cur;
                    pBBlock = ref Unsafe.Add(ref pBBlock, Vector<TMy>.Count);
                    pC = ref Unsafe.Add(ref pC, 1);
                }
                pCLast = vCLast; // Overrride remainder items. 
                // Next.
                pA0 = ref Unsafe.Add(ref pA0, strideA);
                pC0 = ref Unsafe.Add(ref pC0, strideC);
            }
        }

#if Tensor_Primitives_ALLOW_T
        /// <summary>Transpose on Span TensorPrimitives.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTransposeSpanTP(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            // Transpose matrix B.
            int total = K * N;
            TMy[] BTrans = ArrayPool<TMy>.Shared.Rent(total);
            try {
                var spanBTrans = BTrans.AsSpan();
                MatrixUtil.Transpose(K, N, B, strideB, spanBTrans);
                // Matrix multiply.
                int aIdx = 0;
                int cIdx0 = 0;
                for (int i = 0; i < M; ++i) {
                    int bIdx = 0;
                    int cIdx = cIdx0;
                    for (int j = 0; j < N; ++j) {
                        //int cIdx = i * strideC + j;
                        //C[cIdx] = 0;
                        //for (int k = 0; k < K; ++k) {
                        //    int aIdx = i * strideA + k;
                        //    int bIdx = j * strideB + k;
                        //    C[cIdx] += A[aIdx] * BTrans[bIdx];
                        //}
                        C[cIdx] = TensorPrimitives.Dot(A.Slice(aIdx, K), spanBTrans.Slice(bIdx, K));
                        bIdx += strideB;
                        ++cIdx;
                    }
                    aIdx += strideA;
                    cIdx0 += strideC;
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(BTrans);
            }
        }
#endif // Tensor_Primitives_ALLOW_T

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
#endif
        /// <summary>Transpose on Ref Simd.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTransposeSimd(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC, bool transposedB = false) {
            // Transpose matrix B.
            TMy[] BTrans;
            if (transposedB) {
                BTrans = [];
            } else {
                int total = K * N;
                BTrans = ArrayPool<TMy>.Shared.Rent(total);
            }
            try {
                ref TMy pB0 = ref Unsafe.AsRef(in B);
                nint strideBTran = strideB;
                if (transposedB) {
                    // Nothing.
                } else {
                    pB0 = ref BTrans[0];
                    strideBTran = K;
                    MatrixUtil.Transpose(K, N, in B, strideB, ref pB0, strideBTran);
                }
                // Matrix multiply.
                ref TMy pA0 = ref Unsafe.AsRef(in A);
                ref TMy pC0 = ref C;
                for (int i = 0; i < M; ++i) {
                    ref TMy pB = ref pB0;
                    ref TMy pC = ref pC0;
                    for (int j = 0; j < N; ++j) {
                        //int cIdx = i * strideC + j;
                        //C[cIdx] = 0;
                        //for (int k = 0; k < K; ++k) {
                        //    int aIdx = i * strideA + k;
                        //    int bIdx = j * strideB + k;
                        //    C[cIdx] += A[aIdx] * BTrans[bIdx];
                        //}
                        //pC = TensorPrimitives.Dot(MemoryMarshal.CreateReadOnlySpan(ref pA0, K), MemoryMarshal.CreateReadOnlySpan(ref pB, K)); // C[cIdx] = TensorPrimitives.Dot(A.Slice(aIdx, K), spanBTrans.Slice(bIdx, K));
                        pC = MatrixUtil.Dot(K, ref pA0, ref pB);
                        pB = ref Unsafe.Add(ref pB, strideBTran);
                        pC = ref Unsafe.Add(ref pC, 1);
                    }
                    pA0 = ref Unsafe.Add(ref pA0, strideA);
                    pC0 = ref Unsafe.Add(ref pC0, strideC);
                }
            } finally {
                if (transposedB) {
                    // Nothing.
                } else {
                    ArrayPool<TMy>.Shared.Return(BTrans);
                }
            }
        }

#if Tensor_Primitives_ALLOW_T
        /// <summary>TileRow on Span TensorPrimitives.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowTP(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, C, strideC);
            // Matrix multiply.
            int aIdx0 = 0;
            int cIdx0 = 0;
            for (int i = 0; i < M; ++i) {
                int aIdx = aIdx0;
                int bIdx0 = 0;
                for (int k = 0; k < K; ++k) {
                    //int bIdx = bIdx0;
                    //int cIdx = cIdx0;
                    //for (int j = 0; j < N; ++j) {
                    //    C[cIdx] += A[aIdx] * B[bIdx];
                    //    ++bIdx;
                    //    ++cIdx;
                    //}
                    TMy aValue = A[aIdx];
                    Span<TMy> rowB = B.Slice(bIdx0, N);
                    Span<TMy> rowC = C.Slice(cIdx0, N);
                    TensorPrimitives.MultiplyAdd(rowB, aValue, rowC, rowC);
                    // Next.
                    ++aIdx;
                    bIdx0 += strideB;
                }
                aIdx0 += strideA;
                cIdx0 += strideC;
            }
        }
#endif // Tensor_Primitives_ALLOW_T

#if NET8_0_OR_GREATER
        /// <summary>TileRow on Span TensorPrimitives - FMA.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowTPFma(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, C, strideC);
            // Matrix multiply.
            int aIdx0 = 0;
            int cIdx0 = 0;
            for (int i = 0; i < M; ++i) {
                int aIdx = aIdx0;
                int bIdx0 = 0;
                for (int k = 0; k < K; ++k) {
                    //int bIdx = bIdx0;
                    //int cIdx = cIdx0;
                    //for (int j = 0; j < N; ++j) {
                    //    C[cIdx] += A[aIdx] * B[bIdx];
                    //    ++bIdx;
                    //    ++cIdx;
                    //}
                    TMy aValue = A[aIdx];
                    Span<TMy> rowB = B.Slice(bIdx0, N);
                    Span<TMy> rowC = C.Slice(cIdx0, N);
                    TensorPrimitives.FusedMultiplyAdd(rowB, aValue, rowC, rowC);
                    // Next.
                    ++aIdx;
                    bIdx0 += strideB;
                }
                aIdx0 += strideA;
                cIdx0 += strideC;
            }
        }

#endif // NET8_0_OR_GREATER

        /// <summary>TileRow on SIMD.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowSimd(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
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

#if NET9_0_OR_GREATER
        /// <summary>TileRow on SIMD Fma.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowSimdFma(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
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
                    Vector<TMy> vCLast = Vector.FusedMultiplyAdd(vA, pBLast, pCLast);
                    // SIMD for.
                    if (cntBlock >= 0) {
                        ref Vector<TMy> pB = ref Unsafe.As<TMy, Vector<TMy>>(ref pB0);
                        ref Vector<TMy> pC = ref Unsafe.As<TMy, Vector<TMy>>(ref pC0);
                        for (int j = 0; j < cntBlock; ++j) {
                            pC = Vector.FusedMultiplyAdd(vA, pB, pC); // pC += vA * pB;
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

#endif // NET9_0_OR_GREATER

#if NETCOREAPP3_0_OR_GREATER
        /// <summary>TileRow on SIMD Fma X86.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowSimdFmaX86(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated || !Fma.IsSupported) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            // Clear matrix C.
            MatrixUtil.Fill((TMy)0, M, N, ref C, strideC);
            // Matrix multiply.
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
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
                            // pC += vA * pB;
                            if (Vector<byte>.Count == Vector256<byte>.Count) {
                                pC = Fma.MultiplyAdd(vA.AsVector256(), pB.AsVector256(), pC.AsVector256()).AsVector();
                            } else if (Vector<byte>.Count == Vector256<byte>.Count) {
                                pC = Fma.MultiplyAdd(vA.AsVector128(), pB.AsVector128(), pC.AsVector128()).AsVector();
                            } else {
                                pC = Vector.Add(Vectors.Multiply(vA, pB), pC);
                            }
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
#endif // NETCOREAPP3_0_OR_GREATER

        /// <summary>TileRow on SIMD - Loop Unrolling 4.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticTileRowSimdLU4(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int LU = 2; // Loop Unrolling 4.
            if (N < Vector<TMy>.Count || !Vector.IsHardwareAccelerated) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            int cntRem = N % Vector<TMy>.Count; // Remainder count.
            int cntBlockRaw = N / Vector<TMy>.Count; // Block count raw.
            int cntBlock = cntBlockRaw;
            if (0 == cntRem) {
                --cntBlock; // Use vCLast.
            }
            if (0 != (cntBlock % LU) || cntBlock < 2) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            cntBlock /= LU;
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
                            Vector<TMy> vB0 = pB;
                            Vector<TMy> vB1 = Unsafe.Add(ref pB, 1);
                            //Vector<TMy> vB2 = Unsafe.Add(ref pB, 2);
                            //Vector<TMy> vB3 = Unsafe.Add(ref pB, 3);
                            ref Vector<TMy> pC1 = ref Unsafe.Add(ref pC, 1);
                            //ref Vector<TMy> pC2 = ref Unsafe.Add(ref pC, 2);
                            //ref Vector<TMy> pC3 = ref Unsafe.Add(ref pC, 3);
                            // pC += vA * pB;
                            pC = Vector.Add(Vectors.Multiply(vA, vB0), pC);
                            pC1 = Vector.Add(Vectors.Multiply(vA, vB1), pC1);
                            //pC2 = Vector.Add(Vectors.Multiply(vA, vB2), pC2);
                            //pC3 = Vector.Add(Vectors.Multiply(vA, vB3), pC3);
                            pB = ref Unsafe.Add(ref pB, LU);
                            pC = ref Unsafe.Add(ref pC, LU);
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

#if NETCOREAPP3_0_OR_GREATER
        /// <summary>OtherGemmAvxBlock - dgemm_avx_unroll_blk.c from https://gitee.com/hillgao/dgemm.git .</summary>
        /// <remarks>
        /// <para>[矩阵乘法优化过程（DGEMM）](https://zhuanlan.zhihu.com/p/76347262)</para>
        /// <para>[dgemm矩阵乘优化，分块，unroll，avx512](https://zhuanlan.zhihu.com/p/574357927)</para>
        /// </remarks>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public unsafe static void StaticOtherGemmAvxBlock(int M, int N, int K, TMy* A, int strideA, TMy* B, int strideB, TMy* C, int strideC, bool allowParallel = false) {
            const int UNROLL = 4;
            const int BLOCKSIZE = 32;
            if (0 != (M % (BLOCKSIZE))) {
                throw new NotSupportedException(string.Format("{0} is not an integer multiple of {1}!", M, BLOCKSIZE));
            }
            if (M != N || M != K) {
                throw new NotSupportedException(string.Format("{0} is not equals {1} or {2}!", M, N, K));
            }
            int n = M;
            if (allowParallel) {
                nint addressA = (nint)A;
                nint addressB = (nint)B;
                nint addressC = (nint)C;
                int cntJ = n / BLOCKSIZE;
                Parallel.For(0, cntJ, j => {
                    int sj = j * BLOCKSIZE;
                    dgemm_avx_unroll_blk(n, (TMy*)addressA, (TMy*)addressB, (TMy*)addressC, sj);
                });
            } else {
                for (int sj = 0; sj < n; sj += BLOCKSIZE) {
                    dgemm_avx_unroll_blk(n, A, B, C, sj);
                }
            }

            // #define UNROLL 4
            // #define BLOCKSIZE 32
            // 
            // void dgemm_avx_unroll_blk(size_t n, TMy *A, TMy *B, TMy *C)
            // {
            // //#pragma omp parallel for
            // 	for (int sj = 0; sj < n; sj += BLOCKSIZE) {
            // 		for (int si = 0; si < n; si += BLOCKSIZE) {
            // 			for (int sk = 0; sk < n; sk += BLOCKSIZE) {
            // 				do_block(n, si, sj, sk, A, B, C);
            // 			}
            // 		}
            // 	}
            // }
            static void dgemm_avx_unroll_blk(int n, TMy* A, TMy* B, TMy* C, int sj) {
                for (int si = 0; si < n; si += BLOCKSIZE) {
                    for (int sk = 0; sk < n; sk += BLOCKSIZE) {
                        do_block(n, si, sj, sk, A, B, C);
                    }
                }
            }

            // static inline void do_block(int n, int si, int sj, int sk,
            // 			TMy *A, TMy *B, TMy *C)
            // {
            // 	for (int i = si; i < si + BLOCKSIZE; i += UNROLL) {
            // 		for (int j = sj; j < sj + BLOCKSIZE; j += 4) {
            // 			__m256d c[UNROLL];
            // 			for (int x = 0; x < UNROLL; x++) {
            // 				c[x] = _mm256_load_pd(C+(i+x)*n+j);
            // 			}
            // 			for (int k = sk; k < sk + BLOCKSIZE; k++) {
            // 				__m256d b = _mm256_load_pd(B+k*n+j);
            // 				for (int x = 0; x < UNROLL; x++) {
            //                     __m256d a = _mm256_broadcast_sd(A+(i+x)*n+k);
            // 					c[x] = _mm256_add_pd(c[x], _mm256_mul_pd(a, b));
            // 				}
            // 			}
            // 			for (int x = 0; x < UNROLL; x++) {
            // 				_mm256_store_pd(C+(i+x)*n+j, c[x]);
            // 			}
            // 		}
            // 	}
            // }
            static void do_block(int n, int si, int sj, int sk, TMy* A, TMy* B, TMy* C) {
                for (int i = si; i < si + BLOCKSIZE; i += UNROLL) {
                    for (int j = sj; j < sj + BLOCKSIZE; j += 4) {
                        //Vector256<TMy>[] c = new Vector256<TMy>[UNROLL];
                        Vector256<TMy> c0;
                        Vector256<TMy> c1;
                        Vector256<TMy> c2;
                        Vector256<TMy> c3;
                        //for (int x = 0; x < UNROLL; x++) {
                        //    c[x] = Avx.LoadVector256(&C[(i + x) * n + j]);
                        //}
                        TMy* pCLine = &C[i * n + j];
                        TMy* pC = pCLine;
                        c0 = Avx.LoadVector256(pC); pC += n;
                        c1 = Avx.LoadVector256(pC); pC += n;
                        c2 = Avx.LoadVector256(pC); pC += n;
                        c3 = Avx.LoadVector256(pC);

                        TMy* pALine = &A[(i) * n + sk];
                        TMy* pB = &B[sk * n + j];
                        for (int k = sk; k < sk + BLOCKSIZE; k++) {
                            //Vector256<TMy> b = Avx.LoadVector256(&B[k * n + j]);
                            Vector256<TMy> b = Avx.LoadVector256(pB);
                            TMy* pA = pALine;
                            //for (int x = 0; x < UNROLL; x++) {
                            //    Vector256<TMy> a = Vector256.Create(A[(i + x) * n + k]);
                            //    c[x] = Avx.Add(c[x], Avx.Multiply(a, b));
                            //}
                            c0 = Avx.Add(c0, Avx.Multiply(Vector256.Create(*pA), b)); pA += n;
                            c1 = Avx.Add(c1, Avx.Multiply(Vector256.Create(*pA), b)); pA += n;
                            c2 = Avx.Add(c2, Avx.Multiply(Vector256.Create(*pA), b)); pA += n;
                            c3 = Avx.Add(c3, Avx.Multiply(Vector256.Create(*pA), b));
                            // Next.
                            pALine += 1;
                            pB += n;
                        }

                        //for (int x = 0; x < UNROLL; x++) {
                        //    Avx.Store(&C[(i + x) * n + j], c[x]);
                        //}
                        pC = pCLine;
                        Avx.Store(pC, c0); pC += n;
                        Avx.Store(pC, c1); pC += n;
                        Avx.Store(pC, c2); pC += n;
                        Avx.Store(pC, c3);
                    }
                }
            }

        }

#endif // NETCOREAPP3_0_OR_GREATER

        /// <summary>BlockCopy2 on Array (块复制2 on 数组).</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBlockCopy2(int M, int N, int K, TMy[] A, int strideA, TMy[] B, int strideB, TMy[] C, int strideC) {
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRow(M, N, K, A, strideA, B, strideB, C, strideC);
                return;
            }
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            try {
                Span<TMy> localA = buf.AsSpan().Slice(0, local2DSize);
                Span<TMy> localB = buf.AsSpan().Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.AsSpan().Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                //#pragma omp parallel for
                for (int bi = 0; bi < blockM; bi++) {
                    for (int bj = 0; bj < blockN; bj++) {
                        // Clear localC.
                        //for (int i = 0; i < BLOCK_SIZE; i++) {
                        //    for (int j = 0; j < BLOCK_SIZE; j++) {
                        //        localC[i * BLOCK_SIZE + j] = 0;
                        //    }
                        //}
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    int aIdx = bi * BLOCK_SIZE * blockNum * BLOCK_SIZE +
                                //                  i * blockNum * BLOCK_SIZE + bk * BLOCK_SIZE + j;
                                //    int bIdx = bk * BLOCK_SIZE * blockNum * BLOCK_SIZE +
                                //                  i * blockNum * BLOCK_SIZE + bj * BLOCK_SIZE + j;
                                //    localA[i][j] = matA[aIdx];
                                //    localB[i][j] = matB[bIdx];
                                //}
                                int aIdx = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                int bIdx = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                A.AsSpan().Slice(aIdx, BLOCK_SIZE).CopyTo(localA.Slice(i * BLOCK_SIZE, BLOCK_SIZE));
                                B.AsSpan().Slice(bIdx, BLOCK_SIZE).CopyTo(localB.Slice(i * BLOCK_SIZE, BLOCK_SIZE));
                            }
                            // Block GEMM.
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    //#pragma omp simd
                                    for (int j = 0; j < BLOCK_SIZE; j++) {
                                        localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                    }
                                }
                            }
                        }
                        // Copy localC back.
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //for (int j = 0; j < BLOCK_SIZE; j++) {
                            //    int cIdx = bi * BLOCK_SIZE * blockNum * BLOCK_SIZE +
                            //                  i * blockNum * BLOCK_SIZE + bj * BLOCK_SIZE + j;
                            //    matC[cIdx] = localC[i][j];
                            //}
                            int cIdx = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            localC.Slice(i * BLOCK_SIZE, BLOCK_SIZE).CopyTo(C.AsSpan().Slice(cIdx, BLOCK_SIZE));
                        }
                    }
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        /// <summary>BlockCopy2 on Span.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBlockCopy2Span(int M, int N, int K, Span<TMy> A, int strideA, Span<TMy> B, int strideB, Span<TMy> C, int strideC) {
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowSpan(M, N, K, A, strideA, B, strideB, C, strideC);
                return;
            }
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            try {
                Span<TMy> localA = buf.AsSpan().Slice(0, local2DSize);
                Span<TMy> localB = buf.AsSpan().Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.AsSpan().Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                int idxA, idxB, idxC;
                int idxC0;
                int idxCLocal;
                for (int bi = 0; bi < blockM; bi++) {
                    for (int bj = 0; bj < blockN; bj++) {
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            idxA = (bi * BLOCK_SIZE) * strideA + bk * BLOCK_SIZE;
                            idxB = (bk * BLOCK_SIZE) * strideB + bj * BLOCK_SIZE;
                            idxCLocal = 0;
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                idxA += strideA;
                                idxB += strideB;
                                idxCLocal += BLOCK_SIZE;
                            }
                            // Block GEMM.
                            idxA = 0;
                            idxC0 = 0;
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                idxB = 0;
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    idxC = idxC0;
                                    for (int j = 0; j < BLOCK_SIZE; j++) {
                                        //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                        localC[idxC] += localA[idxA] * localB[idxB];
                                        ++idxB;
                                        ++idxC;
                                    }
                                    ++idxA;
                                }
                                idxC0 += BLOCK_SIZE;
                            }
                        }
                        // Copy localC back.
                        idxC = (bi * BLOCK_SIZE) * strideC + bj * BLOCK_SIZE;
                        idxCLocal = 0;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            idxC += strideC;
                            idxCLocal += BLOCK_SIZE;
                        }
                    }
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        /// <summary>BlockCopy2 on Ref.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBlockCopy2Ref(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            try {
                Span<TMy> localA = buf.AsSpan().Slice(0, local2DSize);
                Span<TMy> localB = buf.AsSpan().Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.AsSpan().Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            ref TMy pACur = ref pA;
                            ref TMy pALocal = ref localA[0];
                            ref TMy pBLocal = ref localB[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                                pACur = ref Unsafe.Add(ref pACur, strideA);
                                pB = ref Unsafe.Add(ref pB, strideB);
                                pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                                pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            }
                            // Block GEMM.
                            ref TMy pACore = ref localA[0];
                            ref TMy pCCore0 = ref localC[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                ref TMy pBCore = ref localB[0];
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    ref TMy pCCore = ref pCCore0;
                                    for (int j = 0; j < BLOCK_SIZE; j++) {
                                        //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                        pCCore += pACore * pBCore;
                                        pBCore = ref Unsafe.Add(ref pBCore, 1);
                                        pCCore = ref Unsafe.Add(ref pCCore, 1);
                                    }
                                    pACore = ref Unsafe.Add(ref pACore, 1);
                                }
                                pCCore0 = ref Unsafe.Add(ref pCCore0, BLOCK_SIZE);
                            }
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
            } finally {
                ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        /// <summary>BlockCopy2 on ref SIMD.</summary>
        /// <inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBlockCopy2Simd(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            // If 8==BLOCK_SIZE, need Vector is 256 bit.
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowRef(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize * 3];
            try {
                Span<TMy> localA = buf.Slice(0, local2DSize);
                Span<TMy> localB = buf.Slice(local2DSize * 1, local2DSize);
                Span<TMy> localC = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            ref TMy pACur = ref pA;
                            ref TMy pALocal = ref localA[0];
                            ref TMy pBLocal = ref localB[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                                pACur = ref Unsafe.Add(ref pACur, strideA);
                                pB = ref Unsafe.Add(ref pB, strideB);
                                pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                                pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            }
                            // Block GEMM.
                            ref TMy pACore = ref localA[0];
                            //ref TMy pCCore0 = ref localC[0];
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //ref TMy pBCore = ref localB[0];
                                ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                for (int k = 0; k < BLOCK_SIZE; k++) {
                                    Vector<TMy> vA = new Vector<TMy>(pACore);
                                    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                    //}
                                    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                    pACore = ref Unsafe.Add(ref pACore, 1);
                                    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                }
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
            } finally {
                //ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        /// <summary>BlockCopy2 on ref SIMD register.</summary>
        /// <<inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBlockCopy2SimdRegi(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            int BLOCK_SIZE = Vector<TMy>.Count;
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowSimd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            if (4 == BLOCK_SIZE) {
                StaticBlockCopy2SimdRegi_4(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            } else if (8 == BLOCK_SIZE) {
                StaticBlockCopy2SimdRegi_8(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            } else {
                StaticBlockCopy2Simd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            }
        }

        private static void StaticBlockCopy2SimdRegi_4(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int BLOCK_SIZE = 4;   // On Vector<TMy>.Count = 4.
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize * 2]; // local2DSize * 3
            try {
                Span<TMy> localC = buf.Slice(0, local2DSize);
                Span<TMy> localA = buf.Slice(local2DSize * 1, local2DSize);
                //Span<TMy> localB = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            ref TMy pBBak = ref pB;
                            ref TMy pACur = ref pA;
                            ref TMy pALocal = ref localA[0];
                            //ref TMy pBLocal = ref localB[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                                //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                                pACur = ref Unsafe.Add(ref pACur, strideA);
                                //pB = ref Unsafe.Add(ref pB, strideB);
                                pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                                //pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            }
                            pB = ref pBBak;
                            Vector<TMy> b0 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b1 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b2 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b3 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            // Block GEMM.
                            ref TMy pACore = ref localA[0];
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                //for (int k = 0; k < BLOCK_SIZE; k++) {
                                //    Vector<TMy> vA = new Vector<TMy>(pACore);
                                //    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                //    //}
                                //    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                //    pACore = ref Unsafe.Add(ref pACore, 1);
                                //    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                //}
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b0), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b1), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b2), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b3), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                // Next.
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
            } finally {
                //ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        private static void StaticBlockCopy2SimdRegi_8(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int BLOCK_SIZE = 8;   // On Vector<TMy>.Count = 8.
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize * 2]; // local2DSize * 3
            try {
                Span<TMy> localC = buf.Slice(0, local2DSize);
                Span<TMy> localA = buf.Slice(local2DSize * 1, local2DSize);
                //Span<TMy> localB = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            ref TMy pBBak = ref pB;
                            ref TMy pACur = ref pA;
                            ref TMy pALocal = ref localA[0];
                            //ref TMy pBLocal = ref localB[0];
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                                //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                                //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                                //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                                Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                                //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                                pACur = ref Unsafe.Add(ref pACur, strideA);
                                //pB = ref Unsafe.Add(ref pB, strideB);
                                pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                                //pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            }
                            pB = ref pBBak;
                            Vector<TMy> b0 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b1 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b2 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b3 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b4 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b5 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b6 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b7 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            // Block GEMM.
                            ref TMy pACore = ref localA[0];
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                //ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                //for (int k = 0; k < BLOCK_SIZE; k++) {
                                //    Vector<TMy> vA = new Vector<TMy>(pACore);
                                //    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                //    //}
                                //    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                //    pACore = ref Unsafe.Add(ref pACore, 1);
                                //    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                //}
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b0), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b1), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b2), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b3), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b4), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b5), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b6), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b7), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                // Next.
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
            } finally {
                //ArrayPool<TMy>.Shared.Return(buf);
            }
        }

        /// <summary>BlockCopy2 on ref SIMD register v2.</summary>
        /// <<inheritdoc cref="StaticBasic(int, int, int, TMy[], int, TMy[], int, TMy[], int)"/>
        public static void StaticBlockCopy2SimdRegi2(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            int BLOCK_SIZE = Vector<TMy>.Count;
            if (0 != (M % BLOCK_SIZE) || 0 != (N % BLOCK_SIZE) || 0 != (K % BLOCK_SIZE)) {
                StaticTileRowSimd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
                return;
            }
            if (4 == BLOCK_SIZE) {
                StaticBlockCopy2SimdRegi2_4(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            } else if (8 == BLOCK_SIZE) {
                StaticBlockCopy2SimdRegi2_8(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            } else {
                StaticBlockCopy2Simd(M, N, K, in A, strideA, in B, strideB, ref C, strideC);
            }
        }

        private static void StaticBlockCopy2SimdRegi2_4(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int BLOCK_SIZE = 4;   // On Vector<TMy>.Count = 4.
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize]; // local2DSize * 3
            do {
                Span<TMy> localC = buf.Slice(0, local2DSize);
                //Span<TMy> localA = buf.Slice(local2DSize * 1, local2DSize);
                //Span<TMy> localB = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            //ref TMy pBBak = ref pB;
                            //ref TMy pACur = ref pA;
                            //ref TMy pALocal = ref localA[0];
                            ////ref TMy pBLocal = ref localB[0];
                            //for (int i = 0; i < BLOCK_SIZE; i++) {
                            //    //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                            //    //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                            //    //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                            //    //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                            //    Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                            //    //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                            //    pACur = ref Unsafe.Add(ref pACur, strideA);
                            //    //pB = ref Unsafe.Add(ref pB, strideB);
                            //    pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                            //    //pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            //}
                            //pB = ref pBBak;
                            Vector<TMy> b0 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b1 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b2 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b3 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            // Block GEMM.
                            //ref TMy pACore = ref localA[0];
                            ref TMy pACore0 = ref pA;
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                ref TMy pACore = ref pACore0;
                                //ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                //for (int k = 0; k < BLOCK_SIZE; k++) {
                                //    Vector<TMy> vA = new Vector<TMy>(pACore);
                                //    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                //    //}
                                //    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                //    pACore = ref Unsafe.Add(ref pACore, 1);
                                //    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                //}
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b0), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b1), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b2), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b3), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                // Next.
                                pACore0 = ref Unsafe.Add(ref pACore0, strideA);
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            Unsafe.As<TMy, Vector<TMy>>(ref pCCur) = Unsafe.As<TMy, Vector<TMy>>(ref pCLocal);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
                //} finally {
                //    //ArrayPool<TMy>.Shared.Return(buf);
            } while (false);
        }

        private static void StaticBlockCopy2SimdRegi2_8(int M, int N, int K, ref readonly TMy A, int strideA, ref readonly TMy B, int strideB, ref TMy C, int strideC) {
            const int BLOCK_SIZE = 8;   // On Vector<TMy>.Count = 8.
            uint cbBlockSize = (uint)(BLOCK_SIZE * Unsafe.SizeOf<TMy>());
            int local2DSize = BLOCK_SIZE * BLOCK_SIZE;
            //TMy[] buf = ArrayPool<TMy>.Shared.Rent(local2DSize * 3);
            Span<TMy> buf = stackalloc TMy[local2DSize]; // local2DSize * 3
            do {
                Span<TMy> localC = buf.Slice(0, local2DSize);
                //Span<TMy> localA = buf.Slice(local2DSize * 1, local2DSize);
                //Span<TMy> localB = buf.Slice(local2DSize * 2, local2DSize);
                int blockM = M / BLOCK_SIZE;
                int blockN = N / BLOCK_SIZE;
                int blockK = K / BLOCK_SIZE;
                // Traverse blocks.
                ref TMy pALine = ref Unsafe.AsRef(in A);
                ref TMy pCLine = ref C;
                for (int bi = 0; bi < blockM; bi++) {
                    ref TMy pBLine = ref Unsafe.AsRef(in B);
                    ref TMy pC = ref pCLine;
                    for (int bj = 0; bj < blockN; bj++) {
                        ref TMy pA = ref pALine;
                        ref TMy pB = ref pBLine;
                        // Clear localC.
                        localC.Clear();
                        for (int bk = 0; bk < blockK; bk++) {
                            // Copy local block.
                            //ref TMy pBBak = ref pB;
                            //ref TMy pACur = ref pA;
                            //ref TMy pALocal = ref localA[0];
                            ////ref TMy pBLocal = ref localB[0];
                            //for (int i = 0; i < BLOCK_SIZE; i++) {
                            //    //idxA = (bi * BLOCK_SIZE + i) * strideA + bk * BLOCK_SIZE;
                            //    //idxB = (bk * BLOCK_SIZE + i) * strideB + bj * BLOCK_SIZE;
                            //    //A.Slice(idxA, BLOCK_SIZE).CopyTo(localA.Slice(idxCLocal, BLOCK_SIZE));
                            //    //B.Slice(idxB, BLOCK_SIZE).CopyTo(localB.Slice(idxCLocal, BLOCK_SIZE));
                            //    Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pALocal), ref Unsafe.As<TMy, byte>(ref pACur), cbBlockSize);
                            //    //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pBLocal), ref Unsafe.As<TMy, byte>(ref pB), cbBlockSize);
                            //    pACur = ref Unsafe.Add(ref pACur, strideA);
                            //    //pB = ref Unsafe.Add(ref pB, strideB);
                            //    pALocal = ref Unsafe.Add(ref pALocal, BLOCK_SIZE);
                            //    //pBLocal = ref Unsafe.Add(ref pBLocal, BLOCK_SIZE);
                            //}
                            //pB = ref pBBak;
                            Vector<TMy> b0 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b1 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b2 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b3 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b4 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b5 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b6 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            Vector<TMy> b7 = Unsafe.As<TMy, Vector<TMy>>(ref pB); pB = ref Unsafe.Add(ref pB, strideB);
                            // Block GEMM.
                            //ref TMy pACore = ref localA[0];
                            ref TMy pACore0 = ref pA;
                            ref Vector<TMy> pCCore0 = ref Unsafe.As<TMy, Vector<TMy>>(ref localC[0]);
                            for (int i = 0; i < BLOCK_SIZE; i++) {
                                ref TMy pACore = ref pACore0;
                                //ref Vector<TMy> pBCore = ref Unsafe.As<TMy, Vector<TMy>>(ref localB[0]);
                                //for (int k = 0; k < BLOCK_SIZE; k++) {
                                //    Vector<TMy> vA = new Vector<TMy>(pACore);
                                //    //for (int j = 0; j < BLOCK_SIZE; j++) {
                                //    //    //localC[i * BLOCK_SIZE + j] += localA[i * BLOCK_SIZE + k] * localB[k * BLOCK_SIZE + j];
                                //    //}
                                //    pCCore0 = Vector.Add(Vectors.Multiply(vA, pBCore), pCCore0); // pC += vA * pB;
                                //    pACore = ref Unsafe.Add(ref pACore, 1);
                                //    pBCore = ref Unsafe.Add(ref pBCore, 1);
                                //}
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b0), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b1), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b2), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b3), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b4), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b5), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b6), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                pCCore0 = Vector.Add(Vectors.Multiply(new Vector<TMy>(pACore), b7), pCCore0); pACore = ref Unsafe.Add(ref pACore, 1);
                                // Next.
                                pACore0 = ref Unsafe.Add(ref pACore0, strideA);
                                pCCore0 = ref Unsafe.Add(ref pCCore0, 1);
                            }
                            // Next.
                            pA = ref Unsafe.Add(ref pA, BLOCK_SIZE);
                        }
                        // Copy localC back.
                        ref TMy pCLocal = ref localC[0];
                        ref TMy pCCur = ref pC;
                        for (int i = 0; i < BLOCK_SIZE; i++) {
                            //int idxC = (bi * BLOCK_SIZE + i) * strideC + bj * BLOCK_SIZE;
                            //localC.Slice(idxCLocal, BLOCK_SIZE).CopyTo(C.Slice(idxC, BLOCK_SIZE));
                            //Unsafe.CopyBlockUnaligned(ref Unsafe.As<TMy, byte>(ref pCCur), ref Unsafe.As<TMy, byte>(ref pCLocal), cbBlockSize);
                            Unsafe.As<TMy, Vector<TMy>>(ref pCCur) = Unsafe.As<TMy, Vector<TMy>>(ref pCLocal);
                            pCCur = ref Unsafe.Add(ref pCCur, strideC);
                            pCLocal = ref Unsafe.Add(ref pCLocal, BLOCK_SIZE);
                        }
                        pBLine = ref Unsafe.Add(ref pBLine, BLOCK_SIZE);
                        pC = ref Unsafe.Add(ref pC, BLOCK_SIZE);
                    }
                    pALine = ref Unsafe.Add(ref pALine, BLOCK_SIZE * strideA);
                    pCLine = ref Unsafe.Add(ref pCLine, BLOCK_SIZE * strideC);
                }
                //} finally {
                //    //ArrayPool<TMy>.Shared.Return(buf);
            } while (false);
        }

    }
}
