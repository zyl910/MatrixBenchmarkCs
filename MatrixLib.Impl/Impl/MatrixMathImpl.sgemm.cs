using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif // NETCOREAPP3_0_OR_GREATER
using System.Text;
using System.Threading.Tasks;
using Zyl.VectorTraits;

namespace MatrixLib.Impl {

    partial class MatrixMathImpl {
#if NET7_0_OR_GREATER

        /*******************************************************/
        /* MACRO for performing the transpose of a 4x4 matrix  */
        /* of single precision floating point values.          */
        /* Arguments row0, row1, row2, and row3 are __m128     */
        /* values whose elements form the corresponding rows   */
        /* of a 4x4 matrix.  The matrix transpose is returned  */
        /* in arguments row0, row1, row2, and row3 where row0  */
        /* now holds column 0 of the original matrix, row1 now */
        /* holds column 1 of the original matrix, etc.         */
        /*******************************************************/
        //#define _MM_TRANSPOSE4_PS(row0, row1, row2, row3) {                 \
        //            __m128 _Tmp3, _Tmp2, _Tmp1, _Tmp0;                          \
        //                                                                    \
        //            _Tmp0   = _mm_shuffle_ps((row0), (row1), 0x44);          \
        //            _Tmp2   = _mm_shuffle_ps((row0), (row1), 0xEE);          \
        //            _Tmp1   = _mm_shuffle_ps((row2), (row3), 0x44);          \
        //            _Tmp3   = _mm_shuffle_ps((row2), (row3), 0xEE);          \
        //                                                                    \
        //            (row0) = _mm_shuffle_ps(_Tmp0, _Tmp1, 0x88);              \
        //            (row1) = _mm_shuffle_ps(_Tmp0, _Tmp1, 0xDD);              \
        //            (row2) = _mm_shuffle_ps(_Tmp2, _Tmp3, 0x88);              \
        //            (row3) = _mm_shuffle_ps(_Tmp2, _Tmp3, 0xDD);              \
        //        }
        /// <summary>
        /// Transpose of a 4x4 matrix of single precision floating point values. 
        /// </summary>
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void _MM_TRANSPOSE4_PS(ref Vector128<FLOAT> row0, ref Vector128<FLOAT> row1, ref Vector128<FLOAT> row2, ref Vector128<FLOAT> row3) {
            Vector128<FLOAT> _Tmp3, _Tmp2, _Tmp1, _Tmp0;
            _Tmp0 = Sse.Shuffle((row0), (row1), (byte)ShuffleControlG4.XYXY); // (x0 y0 x1 y1)
            _Tmp2 = Sse.Shuffle((row0), (row1), (byte)ShuffleControlG4.ZWZW); // (z0 w0 z1 w1)
            _Tmp1 = Sse.Shuffle((row2), (row3), (byte)ShuffleControlG4.XYXY); // (x2 y2 x3 y3)
            _Tmp3 = Sse.Shuffle((row2), (row3), (byte)ShuffleControlG4.ZWZW); // (z2 w2 z3 w3)
            (row0) = Sse.Shuffle(_Tmp0, _Tmp1, (byte)ShuffleControlG4.XZXZ); // (x0 x1 x2 x3)
            (row1) = Sse.Shuffle(_Tmp0, _Tmp1, (byte)ShuffleControlG4.YWYW); // (y0 y1 y2 y3)
            (row2) = Sse.Shuffle(_Tmp2, _Tmp3, (byte)ShuffleControlG4.XZXZ); // (z0 z1 z2 z3)
            (row3) = Sse.Shuffle(_Tmp2, _Tmp3, (byte)ShuffleControlG4.YWYW); // (w0 w1 w2 w3)
        }

        /// <summary>
        /// OpenBLAS: /kernel/x86_64/sgemm_ncopy_4_skylakex.c
        /// </summary>
        [CLSCompliant(false)]
        public unsafe static int sgemm_ncopy_4_skylakex(BLASLONG m, BLASLONG n, FLOAT* a, BLASLONG lda, FLOAT* b) {
            // int CNAME(BLASLONG m, BLASLONG n, FLOAT * __restrict a, BLASLONG lda, FLOAT * __restrict b){
            BLASLONG i, j;

            FLOAT* a_offset, a_offset1, a_offset2, a_offset3, a_offset4;
            FLOAT* b_offset;
            FLOAT ctemp1, ctemp2, ctemp3, ctemp4;
            FLOAT ctemp5, ctemp6, ctemp7, ctemp8;
            FLOAT ctemp9, ctemp13;

            a_offset = a;
            b_offset = b;

            j = (n >> 2);
            if (j > 0) {
                do {
                    a_offset1 = a_offset;
                    a_offset2 = a_offset1 + lda;
                    a_offset3 = a_offset2 + lda;
                    a_offset4 = a_offset3 + lda;
                    a_offset += 4 * lda;

                    i = (m >> 2);
                    if (i > 0) {
                        do {
                            //__m128 row0, row1, row2, row3;
                            Vector128<FLOAT> row0, row1, row2, row3;

                            //row0 = _mm_loadu_ps(a_offset1);
                            //row1 = _mm_loadu_ps(a_offset2);
                            //row2 = _mm_loadu_ps(a_offset3);
                            //row3 = _mm_loadu_ps(a_offset4);
                            row0 = Vector128.Load(a_offset1);
                            row1 = Vector128.Load(a_offset2);
                            row2 = Vector128.Load(a_offset3);
                            row3 = Vector128.Load(a_offset4);

                            _MM_TRANSPOSE4_PS(ref row0, ref row1, ref row2, ref row3);

                            //_mm_storeu_ps(b_offset + 0, row0);
                            //_mm_storeu_ps(b_offset + 4, row1);
                            //_mm_storeu_ps(b_offset + 8, row2);
                            //_mm_storeu_ps(b_offset + 12, row3);
                            Vector128.Store(row0, b_offset + 0);
                            Vector128.Store(row1, b_offset + 4);
                            Vector128.Store(row2, b_offset + 8);
                            Vector128.Store(row3, b_offset + 12);

                            a_offset1 += 4;
                            a_offset2 += 4;
                            a_offset3 += 4;
                            a_offset4 += 4;

                            b_offset += 16;
                            i--;
                        } while (i > 0);
                    }

                    i = (m & 3);
                    if (i > 0) {
                        do {
                            ctemp1 = *(a_offset1 + 0);
                            ctemp5 = *(a_offset2 + 0);
                            ctemp9 = *(a_offset3 + 0);
                            ctemp13 = *(a_offset4 + 0);

                            *(b_offset + 0) = ctemp1;
                            *(b_offset + 1) = ctemp5;
                            *(b_offset + 2) = ctemp9;
                            *(b_offset + 3) = ctemp13;

                            a_offset1++;
                            a_offset2++;
                            a_offset3++;
                            a_offset4++;

                            b_offset += 4;
                            i--;
                        } while (i > 0);
                    }
                    j--;
                } while (j > 0);
            } /* end of if(j > 0) */

            if (0 != (n & 2)) {
                a_offset1 = a_offset;
                a_offset2 = a_offset1 + lda;
                a_offset += 2 * lda;

                i = (m >> 2);
                if (i > 0) {
                    do {
                        ctemp1 = *(a_offset1 + 0);
                        ctemp2 = *(a_offset1 + 1);
                        ctemp3 = *(a_offset1 + 2);
                        ctemp4 = *(a_offset1 + 3);

                        ctemp5 = *(a_offset2 + 0);
                        ctemp6 = *(a_offset2 + 1);
                        ctemp7 = *(a_offset2 + 2);
                        ctemp8 = *(a_offset2 + 3);

                        *(b_offset + 0) = ctemp1;
                        *(b_offset + 1) = ctemp5;
                        *(b_offset + 2) = ctemp2;
                        *(b_offset + 3) = ctemp6;

                        *(b_offset + 4) = ctemp3;
                        *(b_offset + 5) = ctemp7;
                        *(b_offset + 6) = ctemp4;
                        *(b_offset + 7) = ctemp8;

                        a_offset1 += 4;
                        a_offset2 += 4;
                        b_offset += 8;
                        i--;
                    } while (i > 0);
                }

                i = (m & 3);
                if (i > 0) {
                    do {
                        ctemp1 = *(a_offset1 + 0);
                        ctemp5 = *(a_offset2 + 0);

                        *(b_offset + 0) = ctemp1;
                        *(b_offset + 1) = ctemp5;

                        a_offset1++;
                        a_offset2++;
                        b_offset += 2;
                        i--;
                    } while (i > 0);
                }
            } /* end of if(j > 0) */

            if (0 != (n & 1)) {
                a_offset1 = a_offset;

                i = (m >> 2);
                if (i > 0) {
                    do {
                        ctemp1 = *(a_offset1 + 0);
                        ctemp2 = *(a_offset1 + 1);
                        ctemp3 = *(a_offset1 + 2);
                        ctemp4 = *(a_offset1 + 3);

                        *(b_offset + 0) = ctemp1;
                        *(b_offset + 1) = ctemp2;
                        *(b_offset + 2) = ctemp3;
                        *(b_offset + 3) = ctemp4;

                        a_offset1 += 4;
                        b_offset += 4;
                        i--;
                    } while (i > 0);
                }

                i = (m & 3);
                if (i > 0) {
                    do {
                        ctemp1 = *(a_offset1 + 0);
                        *(b_offset + 0) = ctemp1;
                        a_offset1++;
                        b_offset += 1;
                        i--;
                    } while (i > 0);
                }
            } /* end of if(j > 0) */

            return 0;
        }

#endif // NET7_0_OR_GREATER
    }
}
