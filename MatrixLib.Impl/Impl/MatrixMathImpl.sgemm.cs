//#define LEFT
//#define TRANSA
//#define TRMMKERNEL

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

        /// <summary>
        /// OpenBLAS: /kernel/x86_64/sgemm_kernel_8x4_haswell.c
        /// </summary>
        [CLSCompliant(false)]
        public unsafe static int sgemm_kernel_8x4_haswell(BLASLONG m, BLASLONG n, BLASLONG k, float alpha, float* A, float* B, float* C, BLASLONG LDC
#if TRMMKERNEL
            , BLASLONG offset
#endif
            ) {
            // int CNAME(BLASLONG m, BLASLONG n, BLASLONG k, float alpha, float * __restrict__ A, float * __restrict__ B, float * __restrict__ C, BLASLONG LDC
            if(m==0||n==0) return 0;
            int64_t ldc_in_bytes = (int64_t)LDC * sizeof(float);
            float constval = alpha;
            float *const_val=&constval;
            int64_t M = (int64_t)m, K = (int64_t)k;
#if TRMMKERNEL
            int64_t off = 0;
    #if LEFT
            off = offset;
    #else
            off = -offset;
    #endif
#endif
            BLASLONG n_count = n;
            float *a_pointer = A,b_pointer = B,c_pointer = C,ctemp = C,next_b = B;
            Vector256<FLOAT> ymm0 = default;
            BLASLONG r11 = default;
            BLASLONG r12 = default;
            BLASLONG r13 = default;
            FLOAT* r14 = default;
            for (; n_count > 11; n_count -= 12) COMPUTE(12);
            for (; n_count > 7; n_count -= 8) COMPUTE(8);
            for (; n_count > 3; n_count -= 4) COMPUTE(4);
            for (; n_count > 1; n_count -= 2) COMPUTE(2);
            if (n_count > 0) COMPUTE(1);
            return 0;

            // /* %0 = "+r"(a_pointer), %1 = "+r"(b_pointer), %2 = "+r"(c_pointer), %3 = "+r"(ldc_in_bytes), %4 for k_count, %5 for c_store, %6 = &alpha, %7 = b_pref */
            // /* r11 = m_counter, r12 = k << 2(const), r13 = k_skip << 2, r14 = b_head_pos(const), r15 for assisting prefetch */
            // 
            // //recommended settings: GEMM_P = 320, GEMM_Q = 320.
            // #ifdef TRMMKERNEL
            //   #define mult_alpha(acc,alpha,...) "vmulps "#acc","#alpha","#acc";"
            // #else
            //   #define mult_alpha(acc,alpha,...) "vfmadd213ps ("#__VA_ARGS__"),"#alpha","#acc";"
            // #endif
            // 
            // #if defined(TRMMKERNEL) && !defined(LEFT)
            //   #ifdef TRANSA
            //     #define HEAD_SET_OFFSET(ndim) {}
            //     #define TAIL_SET_OFFSET(ndim) {off+=ndim;}
            //   #else
            //     #define HEAD_SET_OFFSET(ndim) {off+=(ndim>4?4:ndim);}
            //     #define TAIL_SET_OFFSET(ndim) {off+=(ndim>4?(ndim-4):0);}
            //   #endif
            // #else
            //   #define HEAD_SET_OFFSET(ndim) {}
            //   #define TAIL_SET_OFFSET(ndim) {}
            // #endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void HEAD_SET_OFFSET(int ndim) {
#if TRMMKERNEL && !LEFT
#if TRANSA
                off+=(ndim>4?4:ndim);
#else
#endif // TRANSA
#else
#endif // TRMMKERNEL && !LEFT
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void TAIL_SET_OFFSET(int ndim) {
#if TRMMKERNEL && !LEFT
#if TRANSA
                off+=ndim;
#else
                off+=(ndim>4?(ndim-4):0);
#endif // TRANSA
#else
#endif // TRMMKERNEL && !LEFT
            }

            // #if defined(TRMMKERNEL) && defined(LEFT)
            //   #ifdef TRANSA
            //     #define init_update_kskip(val) "subq $"#val",%%r13;"
            //     #define save_update_kskip(val) ""
            //   #else
            //     #define init_update_kskip(val) ""
            //     #define save_update_kskip(val) "addq $"#val",%%r13;"
            //   #endif
            // #else
            //   #define init_update_kskip(val) ""
            //   #define save_update_kskip(val) ""
            // #endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void init_update_kskip(int val) {
#if TRMMKERNEL && !LEFT
#if TRANSA
                r13 -= val;
#else
#endif // TRANSA
#else
#endif // TRMMKERNEL && !LEFT
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void save_update_kskip(int val) {
#if TRMMKERNEL && !LEFT
#if TRANSA
#else
                r13 += val;
#endif // TRANSA
#else
#endif // TRMMKERNEL && !LEFT
            }

            // #ifdef TRMMKERNEL
            //   #define init_set_k "movq %%r12,%4; subq %%r13,%4;"
            //   #if (defined(LEFT) && !defined(TRANSA)) || (!defined(LEFT) && defined(TRANSA))
            //     #define INIT_SET_KSKIP "movq %9,%%r13; salq $2,%%r13;"
            //     #define init_set_pointers(a_copy,b_copy) "leaq (%0,%%r13,"#a_copy"),%0; leaq (%1,%%r13,"#b_copy"),%1;"
            //     #define save_set_pointers(a_copy,b_copy) ""
            //   #else
            //     #define INIT_SET_KSKIP "movq %4,%%r13; subq %9,%%r13; salq $2,%%r13;"
            //     #define init_set_pointers(a_copy,b_copy) ""
            //     #define save_set_pointers(a_copy,b_copy) "leaq (%0,%%r13,"#a_copy"),%0; leaq (%1,%%r13,"#b_copy"),%1;"
            //   #endif
            // #else
            //   #define INIT_SET_KSKIP "xorq %%r13,%%r13;"
            //   #define init_set_k "movq %%r12,%4;"
            //   #define init_set_pointers(a_copy,b_copy) ""
            //   #define save_set_pointers(a_copy,b_copy) ""
            // #endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void init_set_k() {
#if TRMMKERNEL
                K = r12 - r13;
#else
                K = r12;
#endif // TRMMKERNEL
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void INIT_SET_KSKIP() {
#if TRMMKERNEL && !LEFT
#if (LEFT && !TRANSA) || (!LEFT && TRANSA)
                r13 = off << 2;
#else
                r13 = (K - off) << 2;
#endif // (LEFT && !TRANSA) || (!LEFT && TRANSA)
#else
                r13 = 0;
#endif // TRMMKERNEL
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void init_set_pointers(void* a_copy, void* b_copy) {
#if TRMMKERNEL && !LEFT
#if (LEFT && !TRANSA) || (!LEFT && TRANSA)
                // leaq (%0,%%r13,"#a_copy"),%0; leaq (%1,%%r13,"#b_copy"),%1;
#else
#endif // (LEFT && !TRANSA) || (!LEFT && TRANSA)
#else
#endif // TRMMKERNEL
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void save_set_pointers(void* a_copy, void* b_copy) {
#if TRMMKERNEL && !LEFT
#if (LEFT && !TRANSA) || (!LEFT && TRANSA)
#else
                // leaq (%0,%%r13,"#a_copy"),%0; leaq (%1,%%r13,"#b_copy"),%1;
#endif // (LEFT && !TRANSA) || (!LEFT && TRANSA)
#else
#endif // TRMMKERNEL
            }

            // #define init_set_pa_pb_n12(mdim) init_set_pointers(mdim,4)
            // #define init_set_pa_pb_n8(mdim) init_set_pointers(mdim,4)
            // #define init_set_pa_pb_n4(mdim) init_set_pointers(mdim,4)
            // #define init_set_pa_pb_n2(mdim) init_set_pointers(mdim,2)
            // #define init_set_pa_pb_n1(mdim) init_set_pointers(mdim,1)
            // #define save_set_pa_pb_n12(mdim) save_set_pointers(mdim,4)
            // #define save_set_pa_pb_n8(mdim) save_set_pointers(mdim,4)
            // #define save_set_pa_pb_n4(mdim) save_set_pointers(mdim,4)
            // #define save_set_pa_pb_n2(mdim) save_set_pointers(mdim,2)
            // #define save_set_pa_pb_n1(mdim) save_set_pointers(mdim,1)
            // 
            // #if defined(TRMMKERNEL) && !defined(LEFT) && defined(TRANSA)
            //   #define kernel_kstart_n8(mdim) \
            //     KERNEL_k1m##mdim##n4 KERNEL_k1m##mdim##n4 KERNEL_k1m##mdim##n4 KERNEL_k1m##mdim##n4 "subq $16,%4;"
            //   #define kernel_kstart_n12(mdim) \
            //     KERNEL_k1m##mdim##n4 KERNEL_k1m##mdim##n4 KERNEL_k1m##mdim##n4 KERNEL_k1m##mdim##n4\
            //     KERNEL_k1m##mdim##n8 KERNEL_k1m##mdim##n8 KERNEL_k1m##mdim##n8 KERNEL_k1m##mdim##n8 "subq $32,%4;"
            // #else
            //   #define kernel_kstart_n8(mdim) ""
            //   #define kernel_kstart_n12(mdim) ""
            // #endif
            // #define kernel_kstart_n4(mdim) ""
            // #define kernel_kstart_n2(mdim) ""
            // #define kernel_kstart_n1(mdim) ""
            // 
            // /* m = 8 *//* ymm0 for alpha, ymm1-ymm3 for temporary use, ymm4-ymm15 for accumulators */
            // #define KERNEL_k1m8n1 \
            //     "vmovups (%0),%%ymm1; addq $32,%0;"\
            //     "vbroadcastss (%1),%%ymm2; vfmadd231ps %%ymm1,%%ymm2,%%ymm4;"\
            //     "addq $4,%1;"
            // #define KERNEL_h_k1m8n2 \
            //     "vmovsldup (%0),%%ymm1; vmovshdup (%0),%%ymm2; addq $32,%0;"\
            //     "vbroadcastsd (%1),%%ymm3; vfmadd231ps %%ymm1,%%ymm3,%%ymm4; vfmadd231ps %%ymm2,%%ymm3,%%ymm5;"
            // #define KERNEL_k1m8n2 KERNEL_h_k1m8n2 "addq $8,%1;"
            // #define KERNEL_h_k1m8n4 \
            //     KERNEL_h_k1m8n2 "vbroadcastsd 8(%1),%%ymm3; vfmadd231ps %%ymm1,%%ymm3,%%ymm6; vfmadd231ps %%ymm2,%%ymm3,%%ymm7;"
            // #define KERNEL_k1m8n4 KERNEL_h_k1m8n4 "addq $16,%1;"
            // #define unit_kernel_k1m8n4(c1,c2,c3,c4,boff1,boff2,...) \
            //     "vbroadcastsd "#boff1"("#__VA_ARGS__"),%%ymm3; vfmadd231ps %%ymm1,%%ymm3,"#c1"; vfmadd231ps %%ymm2,%%ymm3,"#c2";"\
            //     "vbroadcastsd "#boff2"("#__VA_ARGS__"),%%ymm3; vfmadd231ps %%ymm1,%%ymm3,"#c3"; vfmadd231ps %%ymm2,%%ymm3,"#c4";"
            // #define KERNEL_h_k1m8n8 KERNEL_h_k1m8n4 unit_kernel_k1m8n4(%%ymm8,%%ymm9,%%ymm10,%%ymm11,0,8,%1,%%r12,4)
            // #define KERNEL_k1m8n8 KERNEL_h_k1m8n8 "addq $16,%1;"
            // #define KERNEL_h_k1m8n12 KERNEL_h_k1m8n8 unit_kernel_k1m8n4(%%ymm12,%%ymm13,%%ymm14,%%ymm15,0,8,%1,%%r12,8)
            // #define KERNEL_k1m8n12 KERNEL_h_k1m8n12 "addq $16,%1;"
            // #define KERNEL_k2m8n1 KERNEL_k1m8n1 KERNEL_k1m8n1
            // #define KERNEL_k2m8n2 KERNEL_k1m8n2 KERNEL_k1m8n2
            // #define KERNEL_k2m8n4 KERNEL_k1m8n4 KERNEL_k1m8n4
            // #define KERNEL_k2m8n8 KERNEL_k1m8n8 KERNEL_k1m8n8
            // #define KERNEL_k2m8n12 \
            //     "vmovsldup (%0),%%ymm1; vmovshdup (%0),%%ymm2;"\
            //     unit_kernel_k1m8n4(%%ymm4,%%ymm5,%%ymm6,%%ymm7,0,8,%1)\
            //     unit_kernel_k1m8n4(%%ymm8,%%ymm9,%%ymm10,%%ymm11,0,8,%1,%%r12,4)\
            //     unit_kernel_k1m8n4(%%ymm12,%%ymm13,%%ymm14,%%ymm15,0,8,%1,%%r12,8)\
            //     "vmovsldup 32(%0),%%ymm1; vmovshdup 32(%0),%%ymm2; prefetcht0 512(%0); addq $64,%0;"\
            //     unit_kernel_k1m8n4(%%ymm4,%%ymm5,%%ymm6,%%ymm7,16,24,%1)\
            //     unit_kernel_k1m8n4(%%ymm8,%%ymm9,%%ymm10,%%ymm11,16,24,%1,%%r12,4)\
            //     unit_kernel_k1m8n4(%%ymm12,%%ymm13,%%ymm14,%%ymm15,16,24,%1,%%r12,8) "addq $32,%1;"
            // #if defined(TRMMKERNEL) && !defined(LEFT) && !defined(TRANSA)
            //   #define unit_kernel_endn4_k1m8n8(offa1,offb1,offb2) \
            //     "vmovsldup "#offa1"(%0),%%ymm1; vmovshdup "#offa1"(%0),%%ymm2;"\
            //     unit_kernel_k1m8n4(%%ymm8,%%ymm9,%%ymm10,%%ymm11,offb1,offb2,%1,%%r12,4)
            //   #define unit_kernel_endn4_k1m8n12(offa1,offb1,offb2) \
            //     "vmovsldup "#offa1"(%0),%%ymm1; vmovshdup "#offa1"(%0),%%ymm2;"\
            //     unit_kernel_k1m8n4(%%ymm12,%%ymm13,%%ymm14,%%ymm15,offb1,offb2,%1,%%r12,8)
            //   #define unit_kernel_endn8_k1m8n12(offa1,offb1,offb2) unit_kernel_endn4_k1m8n8(offa1,offb1,offb2)\
            //     unit_kernel_k1m8n4(%%ymm12,%%ymm13,%%ymm14,%%ymm15,offb1,offb2,%1,%%r12,8)
            //   #define kernel_kend_m8n8 \
            //     unit_kernel_endn4_k1m8n8(0,0,8) unit_kernel_endn4_k1m8n8(32,16,24)\
            //     unit_kernel_endn4_k1m8n8(64,32,40) unit_kernel_endn4_k1m8n8(96,48,56)
            //   #define kernel_kend_m8n12 \
            //     unit_kernel_endn8_k1m8n12(0,0,8) unit_kernel_endn8_k1m8n12(32,16,24)\
            //     unit_kernel_endn8_k1m8n12(64,32,40) unit_kernel_endn8_k1m8n12(96,48,56)\
            //     unit_kernel_endn4_k1m8n12(128,64,72) unit_kernel_endn4_k1m8n12(160,80,88)\
            //     unit_kernel_endn4_k1m8n12(192,96,104) unit_kernel_endn4_k1m8n12(224,112,120)
            // #else
            //   #define kernel_kend_m8n8 ""
            //   #define kernel_kend_m8n12 ""
            // #endif
            // #define kernel_kend_m8n4 ""
            // #define kernel_kend_m8n2 ""
            // #define kernel_kend_m8n1 ""
            // #define INIT_m8n1 "vpxor %%ymm4,%%ymm4,%%ymm4;"
            // #define INIT_m8n2 INIT_m8n1 "vpxor %%ymm5,%%ymm5,%%ymm5;"
            // #define INIT_m8n4 INIT_m8n2 "vpxor %%ymm6,%%ymm6,%%ymm6;vpxor %%ymm7,%%ymm7,%%ymm7;"
            // #define unit_init_m8n4(c1,c2,c3,c4) \
            //     "vpxor "#c1","#c1","#c1";vpxor "#c2","#c2","#c2";vpxor "#c3","#c3","#c3";vpxor "#c4","#c4","#c4";"
            // #define INIT_m8n8  INIT_m8n4 unit_init_m8n4(%%ymm8,%%ymm9,%%ymm10,%%ymm11)
            // #define INIT_m8n12 INIT_m8n8 unit_init_m8n4(%%ymm12,%%ymm13,%%ymm14,%%ymm15)
            // #define SAVE_m8n1 mult_alpha(%%ymm4,%%ymm0,%2) "vmovups %%ymm4,(%2);"
            // #define unit_save_m8n2(c1,c2) \
            //     "vunpcklps "#c2","#c1",%%ymm2; vunpckhps "#c2","#c1",%%ymm3; vunpcklpd %%ymm3,%%ymm2,"#c1"; vunpckhpd %%ymm3,%%ymm2,"#c2";"\
            //     mult_alpha(c1,%%ymm0,%5) "vmovups "#c1",(%5);"\
            //     mult_alpha(c2,%%ymm0,%5,%3,1) "vmovups "#c2",(%5,%3,1);"\
            //     "leaq (%5,%3,2),%5;"
            // #define SAVE_m8n2 "movq %2,%5;" unit_save_m8n2(%%ymm4,%%ymm5)
            // #define SAVE_m8n4  SAVE_m8n2  unit_save_m8n2(%%ymm6,%%ymm7)
            // #define SAVE_m8n8  SAVE_m8n4  unit_save_m8n2(%%ymm8,%%ymm9)   unit_save_m8n2(%%ymm10,%%ymm11)
            // #define SAVE_m8n12 SAVE_m8n8  unit_save_m8n2(%%ymm12,%%ymm13) unit_save_m8n2(%%ymm14,%%ymm15)
            // #define COMPUTE_m8(ndim) \
            //     init_update_kskip(32) INIT_m8n##ndim\
            //     init_set_k "movq %%r14,%1;" init_set_pa_pb_n##ndim(8) "movq %2,%5; movq $0,%%r15;"\
            //     kernel_kstart_n##ndim(8)\
            //     "cmpq $64,%4; jb "#ndim"882f;"\
            //     #ndim"881:\n\t"\
            //     "cmpq $62,%%r15; movq $62,%%r15; cmoveq %3,%%r15;"\
            //     KERNEL_k2m8n##ndim KERNEL_k2m8n##ndim\
            //     "prefetcht1 (%5); subq $31,%5;"\
            //     KERNEL_k2m8n##ndim KERNEL_k2m8n##ndim\
            //     "addq %%r15,%5; prefetcht1 (%7); addq $16,%7;"\
            //     "subq $32,%4; cmpq $64,%4; jnb "#ndim"881b;"\
            //     "movq %2,%5;"\
            //     #ndim"882:\n\t"\
            //     "testq %4,%4; jz "#ndim"883f;"\
            //     "prefetcht0 (%5); prefetcht0 31(%5);"\
            //     KERNEL_k1m8n##ndim\
            //     "prefetcht0 (%5,%3,4); prefetcht0 31(%5,%3,4); addq %3,%5;"\
            //     "subq $4,%4; jmp "#ndim"882b;"\
            //     #ndim"883:\n\t"\
            //     kernel_kend_m8n##ndim "prefetcht0 (%%r14); prefetcht0 64(%%r14);"\
            //     save_set_pa_pb_n##ndim(8) SAVE_m8n##ndim "addq $32,%2;" save_update_kskip(32)
            // 
            // /* m = 4 *//* xmm0 for alpha, xmm1-xmm3 for temporary use, xmm4-xmm15 for accumulators */
            // #define KERNEL_k1m4n1 \
            //     "vmovups (%0),%%xmm1; addq $16,%0;"\
            //     "vbroadcastss (%1),%%xmm2; vfmadd231ps %%xmm1,%%xmm2,%%xmm4;"\
            //     "addq $4,%1;"
            // #define KERNEL_h_k1m4n2 \
            //     "vmovsldup (%0),%%xmm1; vmovshdup (%0),%%xmm2; addq $16,%0;"\
            //     "vmovddup (%1),%%xmm3; vfmadd231ps %%xmm1,%%xmm3,%%xmm4; vfmadd231ps %%xmm2,%%xmm3,%%xmm5;"
            // #define KERNEL_k1m4n2 KERNEL_h_k1m4n2 "addq $8,%1;"
            // #define KERNEL_h_k1m4n4 \
            //     KERNEL_h_k1m4n2 "vmovddup 8(%1),%%xmm3; vfmadd231ps %%xmm1,%%xmm3,%%xmm6; vfmadd231ps %%xmm2,%%xmm3,%%xmm7;"
            // #define KERNEL_k1m4n4 KERNEL_h_k1m4n4 "addq $16,%1;"
            // #define unit_kernel_k1m4n4(c1,c2,c3,c4,offb1,offb2,...) \
            //     "vmovddup "#offb1"("#__VA_ARGS__"),%%xmm3; vfmadd231ps %%xmm1,%%xmm3,"#c1"; vfmadd231ps %%xmm2,%%xmm3,"#c2";"\
            //     "vmovddup "#offb2"("#__VA_ARGS__"),%%xmm3; vfmadd231ps %%xmm1,%%xmm3,"#c3"; vfmadd231ps %%xmm2,%%xmm3,"#c4";"
            // #define KERNEL_h_k1m4n8 KERNEL_h_k1m4n4 unit_kernel_k1m4n4(%%xmm8,%%xmm9,%%xmm10,%%xmm11,0,8,%1,%%r12,4)
            // #define KERNEL_k1m4n8 KERNEL_h_k1m4n8 "addq $16,%1;"
            // #define KERNEL_h_k1m4n12 KERNEL_h_k1m4n8 unit_kernel_k1m4n4(%%xmm12,%%xmm13,%%xmm14,%%xmm15,0,8,%1,%%r12,8)
            // #define KERNEL_k1m4n12 KERNEL_h_k1m4n12 "addq $16,%1;"
            // #if defined(TRMMKERNEL) && !defined(LEFT) && !defined(TRANSA)
            //   #define unit_kernel_endn4_k1m4n8(offa1,offb1,offb2) \
            //     "vmovsldup "#offa1"(%0),%%xmm1; vmovshdup "#offa1"(%0),%%xmm2;"\
            //     unit_kernel_k1m4n4(%%xmm8,%%xmm9,%%xmm10,%%xmm11,offb1,offb2,%1,%%r12,4)
            //   #define unit_kernel_endn4_k1m4n12(offa1,offb1,offb2) \
            //     "vmovsldup "#offa1"(%0),%%xmm1; vmovshdup "#offa1"(%0),%%xmm2;"\
            //     unit_kernel_k1m4n4(%%xmm12,%%xmm13,%%xmm14,%%xmm15,offb1,offb2,%1,%%r12,8)
            //   #define unit_kernel_endn8_k1m4n12(offa1,offb1,offb2) unit_kernel_endn4_k1m4n8(offa1,offb1,offb2)\
            //     unit_kernel_k1m4n4(%%xmm12,%%xmm13,%%xmm14,%%xmm15,offb1,offb2,%1,%%r12,8)
            //   #define kernel_kend_m4n8 \
            //     unit_kernel_endn4_k1m4n8(0,0,8) unit_kernel_endn4_k1m4n8(16,16,24)\
            //     unit_kernel_endn4_k1m4n8(32,32,40) unit_kernel_endn4_k1m4n8(48,48,56)
            //   #define kernel_kend_m4n12 \
            //     unit_kernel_endn8_k1m4n12(0,0,8) unit_kernel_endn8_k1m4n12(16,16,24)\
            //     unit_kernel_endn8_k1m4n12(32,32,40) unit_kernel_endn8_k1m4n12(48,48,56)\
            //     unit_kernel_endn4_k1m4n12(64,64,72) unit_kernel_endn4_k1m4n12(80,80,88)\
            //     unit_kernel_endn4_k1m4n12(96,96,104) unit_kernel_endn4_k1m4n12(112,112,120)
            // #else
            //   #define kernel_kend_m4n8 ""
            //   #define kernel_kend_m4n12 ""
            // #endif
            // #define kernel_kend_m4n4 ""
            // #define kernel_kend_m4n2 ""
            // #define kernel_kend_m4n1 ""
            // #define INIT_m4n1 "vpxor %%xmm4,%%xmm4,%%xmm4;"
            // #define INIT_m4n2 INIT_m4n1 "vpxor %%xmm5,%%xmm5,%%xmm5;"
            // #define INIT_m4n4 INIT_m4n2 "vpxor %%xmm6,%%xmm6,%%xmm6;vpxor %%xmm7,%%xmm7,%%xmm7;"
            // #define unit_init_m4n4(c1,c2,c3,c4) \
            //     "vpxor "#c1","#c1","#c1";vpxor "#c2","#c2","#c2";vpxor "#c3","#c3","#c3";vpxor "#c4","#c4","#c4";"
            // #define INIT_m4n8  INIT_m4n4 unit_init_m4n4(%%xmm8,%%xmm9,%%xmm10,%%xmm11)
            // #define INIT_m4n12 INIT_m4n8 unit_init_m4n4(%%xmm12,%%xmm13,%%xmm14,%%xmm15)
            // #define SAVE_m4n1 \
            //     mult_alpha(%%xmm4,%%xmm0,%2) "vmovups %%xmm4,(%2);"
            // #define unit_save_m4n2(c1,c2) \
            //     "vunpcklps "#c2","#c1",%%xmm2; vunpckhps "#c2","#c1",%%xmm3; vunpcklpd %%xmm3,%%xmm2,"#c1"; vunpckhpd %%xmm3,%%xmm2,"#c2";"\
            //     mult_alpha(c1,%%xmm0,%5) "vmovups "#c1",(%5);"\
            //     mult_alpha(c2,%%xmm0,%5,%3,1) "vmovups "#c2",(%5,%3,1);"\
            //     "leaq (%5,%3,2),%5;"
            // #define SAVE_m4n2 "movq %2,%5;" unit_save_m4n2(%%xmm4,%%xmm5)
            // #define SAVE_m4n4  SAVE_m4n2  unit_save_m4n2(%%xmm6,%%xmm7)
            // #define SAVE_m4n8  SAVE_m4n4  unit_save_m4n2(%%xmm8,%%xmm9)   unit_save_m4n2(%%xmm10,%%xmm11)
            // #define SAVE_m4n12 SAVE_m4n8  unit_save_m4n2(%%xmm12,%%xmm13) unit_save_m4n2(%%xmm14,%%xmm15)
            // #define COMPUTE_m4(ndim) \
            //     init_update_kskip(16) INIT_m4n##ndim\
            //     init_set_k "movq %%r14,%1;" init_set_pa_pb_n##ndim(4)\
            //     kernel_kstart_n##ndim(4)\
            //     #ndim"442:\n\t"\
            //     "testq %4,%4; jz "#ndim"443f;"\
            //     KERNEL_k1m4n##ndim\
            //     "subq $4,%4; jmp "#ndim"442b;"\
            //     #ndim"443:\n\t"\
            //     kernel_kend_m4n##ndim save_set_pa_pb_n##ndim(4) SAVE_m4n##ndim "addq $16,%2;" save_update_kskip(16)
            // 
            // /* m = 2 *//* xmm0 for alpha, xmm1-xmm3 and xmm10 for temporary use, xmm4-xmm9 for accumulators */
            // #define INIT_m2n1 "vpxor %%xmm4,%%xmm4,%%xmm4;"
            // #define KERNEL_k1m2n1 \
            //     "vmovsd (%0),%%xmm1; addq $8,%0;"\
            //     "vbroadcastss (%1),%%xmm2; vfmadd231ps %%xmm1,%%xmm2,%%xmm4;"\
            //     "addq $4,%1;"
            // #ifdef TRMMKERNEL
            //  #define SAVE_m2n1 "vmulps %%xmm4,%%xmm0,%%xmm4; vmovsd %%xmm4,(%2);"
            // #else
            //  #define SAVE_m2n1 "vmovsd (%2),%%xmm1; vfmadd213ps %%xmm1,%%xmm0,%%xmm4; vmovsd %%xmm4,(%2);"
            // #endif
            // #define INIT_m2n2 INIT_m2n1 "vpxor %%xmm5,%%xmm5,%%xmm5;"
            // #define KERNEL_k1m2n2 \
            //     "vmovsd (%0),%%xmm1; addq $8,%0;"\
            //     "vbroadcastss  (%1),%%xmm2; vfmadd231ps %%xmm1,%%xmm2,%%xmm4;"\
            //     "vbroadcastss 4(%1),%%xmm3; vfmadd231ps %%xmm1,%%xmm3,%%xmm5;"\
            //     "addq $8,%1;"
            // #ifdef TRMMKERNEL
            //   #define SAVE_m2n2 SAVE_m2n1 "vmulps %%xmm5,%%xmm0,%%xmm5; vmovsd %%xmm5,(%2,%3,1);"
            // #else
            //   #define SAVE_m2n2 SAVE_m2n1 "vmovsd (%2,%3,1),%%xmm1; vfmadd213ps %%xmm1,%%xmm0,%%xmm5; vmovsd %%xmm5,(%2,%3,1);"
            // #endif
            // #define INIT_m2n4  INIT_m2n2
            // #define INIT_m2n8  INIT_m2n4 "vpxor %%xmm6,%%xmm6,%%xmm6; vpxor %%xmm7,%%xmm7,%%xmm7;"
            // #define INIT_m2n12 INIT_m2n8 "vpxor %%xmm8,%%xmm8,%%xmm8; vpxor %%xmm9,%%xmm9,%%xmm9;"
            // #define KERNEL_k1m2n4 \
            //     "vmovups (%1),%%xmm3; addq $16,%1;"\
            //     "vbroadcastss  (%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm4;"\
            //     "vbroadcastss 4(%0),%%xmm2; vfmadd231ps %%xmm3,%%xmm2,%%xmm5;"\
            //     "addq $8,%0;"
            // #define KERNEL_k1m2n8 \
            //     "vmovups (%1),%%xmm3; vmovups (%1,%%r12,4),%%xmm2; addq $16,%1;"\
            //     "vbroadcastss  (%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm4; vfmadd231ps %%xmm2,%%xmm1,%%xmm6;"\
            //     "vbroadcastss 4(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm5; vfmadd231ps %%xmm2,%%xmm1,%%xmm7;"\
            //     "addq $8,%0;"
            // #define KERNEL_k1m2n12 \
            //     "vmovups (%1),%%xmm3; vmovups (%1,%%r12,4),%%xmm2; vmovups (%1,%%r12,8),%%xmm1; addq $16,%1;"\
            //     "vbroadcastss  (%0),%%xmm10; vfmadd231ps %%xmm3,%%xmm10,%%xmm4; vfmadd231ps %%xmm2,%%xmm10,%%xmm6; vfmadd231ps %%xmm1,%%xmm10,%%xmm8;"\
            //     "vbroadcastss 4(%0),%%xmm10; vfmadd231ps %%xmm3,%%xmm10,%%xmm5; vfmadd231ps %%xmm2,%%xmm10,%%xmm7; vfmadd231ps %%xmm1,%%xmm10,%%xmm9;"\
            //     "addq $8,%0;"
            // #if defined(TRMMKERNEL) && !defined(LEFT) && !defined(TRANSA)
            //   #define unit_kernel_endn4_k1m2n8(aoff1,aoff2,boff) \
            //     "vmovups "#boff"(%1,%%r12,4),%%xmm3;"\
            //     "vbroadcastss "#aoff1"(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm6;"\
            //     "vbroadcastss "#aoff2"(%0),%%xmm2; vfmadd231ps %%xmm3,%%xmm2,%%xmm7;"
            //   #define unit_kernel_endn4_k1m2n12(aoff1,aoff2,boff) \
            //     "vmovups "#boff"(%1,%%r12,8),%%xmm3;"\
            //     "vbroadcastss "#aoff1"(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm8;"\
            //     "vbroadcastss "#aoff2"(%0),%%xmm2; vfmadd231ps %%xmm3,%%xmm2,%%xmm9;"
            //   #define unit_kernel_endn8_k1m2n12(aoff1,aoff2,boff) \
            //     "vmovups "#boff"(%1,%%r12,4),%%xmm3; vmovups "#boff"(%1,%%r12,8),%%xmm2;"\
            //     "vbroadcastss "#aoff1"(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm6; vfmadd231ps %%xmm2,%%xmm1,%%xmm8;"\
            //     "vbroadcastss "#aoff2"(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm7; vfmadd231ps %%xmm2,%%xmm1,%%xmm9;"
            //   #define kernel_kend_m2n8 \
            //     unit_kernel_endn4_k1m2n8(0,4,0) unit_kernel_endn4_k1m2n8(8,12,16)\
            //     unit_kernel_endn4_k1m2n8(16,20,32) unit_kernel_endn4_k1m2n8(24,28,48)
            //   #define kernel_kend_m2n12 \
            //     unit_kernel_endn8_k1m2n12(0,4,0) unit_kernel_endn8_k1m2n12(8,12,16)\
            //     unit_kernel_endn8_k1m2n12(16,20,32) unit_kernel_endn8_k1m2n12(24,28,48)\
            //     unit_kernel_endn4_k1m2n12(32,36,64) unit_kernel_endn4_k1m2n12(40,44,80)\
            //     unit_kernel_endn4_k1m2n12(48,52,96) unit_kernel_endn4_k1m2n12(56,60,112)
            // #else
            //   #define kernel_kend_m2n8 ""
            //   #define kernel_kend_m2n12 ""
            // #endif
            // #define kernel_kend_m2n4 ""
            // #define kernel_kend_m2n2 ""
            // #define kernel_kend_m2n1 ""
            // #ifdef TRMMKERNEL
            //   #define unit_save_m2n4(c1,c2) \
            //     "vunpcklps "#c2","#c1",%%xmm1; vunpckhps "#c2","#c1",%%xmm2;"\
            //     "vmulps %%xmm1,%%xmm0,%%xmm1; vmovsd %%xmm1,(%5); vmovhpd %%xmm1,(%5,%3,1); leaq (%5,%3,2),%5;"\
            //     "vmulps %%xmm2,%%xmm0,%%xmm2; vmovsd %%xmm2,(%5); vmovhpd %%xmm2,(%5,%3,1); leaq (%5,%3,2),%5;"
            // #else
            //   #define unit_save_m2n4(c1,c2) \
            //     "vunpcklps "#c2","#c1",%%xmm1; vunpckhps "#c2","#c1",%%xmm2;"\
            //     "vmovsd (%5),%%xmm3; vmovhpd (%5,%3,1),%%xmm3,%%xmm3; vfmadd213ps %%xmm3,%%xmm0,%%xmm1;"\
            //     "vmovsd %%xmm1,(%5); vmovhpd %%xmm1,(%5,%3,1); leaq (%5,%3,2),%5;"\
            //     "vmovsd (%5),%%xmm3; vmovhpd (%5,%3,1),%%xmm3,%%xmm3; vfmadd213ps %%xmm3,%%xmm0,%%xmm2;"\
            //     "vmovsd %%xmm2,(%5); vmovhpd %%xmm2,(%5,%3,1); leaq (%5,%3,2),%5;"
            // #endif
            // #define SAVE_m2n4 "movq %2,%5;" unit_save_m2n4(%%xmm4,%%xmm5)
            // #define SAVE_m2n8   SAVE_m2n4    unit_save_m2n4(%%xmm6,%%xmm7)
            // #define SAVE_m2n12  SAVE_m2n8   unit_save_m2n4(%%xmm8,%%xmm9)
            // #define COMPUTE_m2(ndim) \
            //     init_update_kskip(8) INIT_m2n##ndim\
            //     init_set_k "movq %%r14,%1;" init_set_pa_pb_n##ndim(2)\
            //     kernel_kstart_n##ndim(2)\
            //     #ndim"222:\n\t"\
            //     "testq %4,%4; jz "#ndim"223f;"\
            //     KERNEL_k1m2n##ndim\
            //     "subq $4,%4; jmp "#ndim"222b;"\
            //     #ndim"223:\n\t"\
            //     kernel_kend_m2n##ndim save_set_pa_pb_n##ndim(2) SAVE_m2n##ndim "addq $8,%2;" save_update_kskip(8)
            // 
            // /* m = 1 *//* xmm0 for alpha, xmm1-xmm3 and xmm10 for temporary use, xmm4-xmm6 for accumulators */
            // #define INIT_m1n1 "vpxor %%xmm4,%%xmm4,%%xmm4;"
            // #define KERNEL_k1m1n1 \
            //     "vmovss (%1),%%xmm3; addq $4,%1;"\
            //     "vmovss (%0),%%xmm1; vfmadd231ss %%xmm3,%%xmm1,%%xmm4;"\
            //     "addq $4,%0;"
            // #ifdef TRMMKERNEL
            //   #define SAVE_m1n1 "vmulss %%xmm4,%%xmm0,%%xmm4; vmovss %%xmm4,(%2);"
            // #else
            //   #define SAVE_m1n1 "vfmadd213ss (%2),%%xmm0,%%xmm4; vmovss %%xmm4,(%2);"
            // #endif
            // #define INIT_m1n2 INIT_m1n1
            // #define KERNEL_k1m1n2 \
            //     "vmovsd (%1),%%xmm3; addq $8,%1;"\
            //     "vbroadcastss  (%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm4;"\
            //     "addq $4,%0;"
            // #ifdef TRMMKERNEL
            //   #define SAVE_m1n2 \
            //     "vmulps %%xmm4,%%xmm0,%%xmm4; vmovss %%xmm4,(%2); vextractps $1,%%xmm4,(%2,%3,1);"
            // #else
            //   #define SAVE_m1n2 \
            //     "vmovss (%2),%%xmm3; vinsertps $16,(%2,%3,1),%%xmm3,%%xmm3; vfmadd213ps %%xmm3,%%xmm0,%%xmm4;"\
            //     "vmovss %%xmm4,(%2); vextractps $1,%%xmm4,(%2,%3,1);"
            // #endif
            // #define INIT_m1n4  INIT_m1n2
            // #define INIT_m1n8  INIT_m1n4 "vpxor %%xmm5,%%xmm5,%%xmm5;"
            // #define INIT_m1n12 INIT_m1n8 "vpxor %%xmm6,%%xmm6,%%xmm6;"
            // #define KERNEL_k1m1n4 \
            //     "vmovups (%1),%%xmm3; addq $16,%1;"\
            //     "vbroadcastss  (%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm4;"\
            //     "addq $4,%0;"
            // #define KERNEL_k1m1n8 \
            //     "vmovups (%1),%%xmm3; vmovups (%1,%%r12,4),%%xmm2; addq $16,%1;"\
            //     "vbroadcastss  (%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm4; vfmadd231ps %%xmm2,%%xmm1,%%xmm5;"\
            //     "addq $4,%0;"
            // #define KERNEL_k1m1n12 \
            //     "vmovups (%1),%%xmm3; vmovups (%1,%%r12,4),%%xmm2; vmovups (%1,%%r12,8),%%xmm1; addq $16,%1;"\
            //     "vbroadcastss  (%0),%%xmm10; vfmadd231ps %%xmm3,%%xmm10,%%xmm4; vfmadd231ps %%xmm2,%%xmm10,%%xmm5; vfmadd231ps %%xmm1,%%xmm10,%%xmm6;"\
            //     "addq $4,%0;"
            // #if defined(TRMMKERNEL) && !defined(LEFT) && !defined(TRANSA)
            //   #define unit_kernel_endn4_k1m1n8(aoff,boff) \
            //     "vmovups "#boff"(%1,%%r12,4),%%xmm3;"\
            //     "vbroadcastss "#aoff"(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm5;"
            //   #define unit_kernel_endn4_k1m1n12(aoff,boff) \
            //     "vmovups "#boff"(%1,%%r12,8),%%xmm3;"\
            //     "vbroadcastss "#aoff"(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm6;"
            //   #define unit_kernel_endn8_k1m1n12(aoff,boff) \
            //     "vmovups "#boff"(%1,%%r12,4),%%xmm3; vmovups "#boff"(%1,%%r12,8),%%xmm2;"\
            //     "vbroadcastss "#aoff"(%0),%%xmm1; vfmadd231ps %%xmm3,%%xmm1,%%xmm5; vfmadd231ps %%xmm2,%%xmm1,%%xmm6;"
            //   #define kernel_kend_m1n8 \
            //     unit_kernel_endn4_k1m1n8(0,0) unit_kernel_endn4_k1m1n8(4,16)\
            //     unit_kernel_endn4_k1m1n8(8,32) unit_kernel_endn4_k1m1n8(12,48)
            //   #define kernel_kend_m1n12 \
            //     unit_kernel_endn8_k1m1n12(0,0) unit_kernel_endn8_k1m1n12(4,16)\
            //     unit_kernel_endn8_k1m1n12(8,32) unit_kernel_endn8_k1m1n12(12,48)\
            //     unit_kernel_endn4_k1m1n12(16,64) unit_kernel_endn4_k1m1n12(20,80)\
            //     unit_kernel_endn4_k1m1n12(24,96) unit_kernel_endn4_k1m1n12(28,112)
            // #else
            //   #define kernel_kend_m1n8 ""
            //   #define kernel_kend_m1n12 ""
            // #endif
            // #define kernel_kend_m1n4 ""
            // #define kernel_kend_m1n2 ""
            // #define kernel_kend_m1n1 ""
            // #ifdef TRMMKERNEL
            //   #define unit_save_m1n4(c1) \
            //     "vpxor %%xmm10,%%xmm10,%%xmm10; vmovsd "#c1",%%xmm10,%%xmm2; vmovhlps "#c1",%%xmm10,%%xmm1;"\
            //     "vmulps %%xmm2,%%xmm0,%%xmm2; vmovss %%xmm2,(%5); vextractps $1,%%xmm2,(%5,%3,1); leaq (%5,%3,2),%5;"\
            //     "vmulps %%xmm1,%%xmm0,%%xmm1; vmovss %%xmm1,(%5); vextractps $1,%%xmm1,(%5,%3,1); leaq (%5,%3,2),%5;"
            // #else
            //   #define unit_save_m1n4(c1) \
            //     "vpxor %%xmm10,%%xmm10,%%xmm10; vmovsd "#c1",%%xmm10,%%xmm2; vmovhlps "#c1",%%xmm10,%%xmm1;"\
            //     "vmovss (%5),%%xmm3; vinsertps $16,(%5,%3,1),%%xmm3,%%xmm3; vfmadd213ps %%xmm3,%%xmm0,%%xmm2;"\
            //     "vmovss %%xmm2,(%5); vextractps $1,%%xmm2,(%5,%3,1); leaq (%5,%3,2),%5;"\
            //     "vmovss (%5),%%xmm3; vinsertps $16,(%5,%3,1),%%xmm3,%%xmm3; vfmadd213ps %%xmm3,%%xmm0,%%xmm1;"\
            //     "vmovss %%xmm1,(%5); vextractps $1,%%xmm1,(%5,%3,1); leaq (%5,%3,2),%5;"
            // #endif
            // #define SAVE_m1n4 "movq %2,%5;" unit_save_m1n4(%%xmm4)
            // #define SAVE_m1n8  SAVE_m1n4    unit_save_m1n4(%%xmm5)
            // #define SAVE_m1n12 SAVE_m1n8    unit_save_m1n4(%%xmm6)

            // #define COMPUTE_m1(ndim) \
            //     init_update_kskip(4) INIT_m1n##ndim\
            //     init_set_k "movq %%r14,%1;" init_set_pa_pb_n##ndim(1)\
            //     kernel_kstart_n##ndim(1)\
            //     #ndim"112:\n\t"\
            //     "testq %4,%4; jz "#ndim"113f;"\
            //     KERNEL_k1m1n##ndim\
            //     "subq $4,%4; jmp "#ndim"112b;"\
            //     #ndim"113:\n\t"\
            //     kernel_kend_m1n##ndim save_set_pa_pb_n##ndim(1) SAVE_m1n##ndim "addq $4,%2;" save_update_kskip(4)
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void COMPUTE_m1(int ndim) {
                init_update_kskip(4);
                //INIT_m1n##ndim
                init_set_k();
                b_pointer = r14; // movq %%r14,%1;
                //init_set_pa_pb_n##ndim(1)
                //kernel_kstart_n##ndim(1)
                while (0 != K) { // testq %4,%4; jz "#ndim"113f;
                    //KERNEL_k1m1n##ndim
                    K -= 4; // subq $4,%4;
                } // jmp "#ndim"112b
                // kernel_kend_m1n##ndim
                // save_set_pa_pb_n##ndim(1)
                // SAVE_m1n##ndim
                c_pointer += 4;// "addq $4,%2;"
                save_update_kskip(4);
            }

            // #define COMPUTE(ndim) {\
            //     HEAD_SET_OFFSET(ndim) next_b = b_pointer + ndim * K;\
            //     __asm__ __volatile__(\
            //     "vbroadcastss (%6),%%ymm0;"\
            //     "movq %4,%%r12; salq $2,%%r12; movq %1,%%r14; movq %8,%%r11;" INIT_SET_KSKIP\
            //     "cmpq $8,%%r11;jb 33101"#ndim"f;"\
            //     "33109"#ndim":\n\t"\
            //     COMPUTE_m8(ndim)\
            //     "subq $8,%%r11;cmpq $8,%%r11;jnb 33109"#ndim"b;"\
            //     "33101"#ndim":\n\t"\
            //     "cmpq $4,%%r11;jb 33103"#ndim"f;"\
            //     COMPUTE_m4(ndim)\
            //     "subq $4,%%r11;"\
            //     "33103"#ndim":\n\t"\
            //     "cmpq $2,%%r11;jb 33104"#ndim"f;"\
            //     COMPUTE_m2(ndim)\
            //     "subq $2,%%r11;"\
            //     "33104"#ndim":\n\t"\
            //     "testq %%r11,%%r11;jz 33105"#ndim"f;"\
            //     COMPUTE_m1(ndim)\
            //     "33105"#ndim":\n\t"\
            //     "movq %%r12,%4; sarq $2,%4; movq %%r14,%1; vzeroupper;"\
            //     :"+r"(a_pointer),"+r"(b_pointer),"+r"(c_pointer),"+r"(ldc_in_bytes),"+r"(K),"+r"(ctemp),"+r"(const_val),"+r"(next_b)\
            //     :"m"(M),"m"(off):"r11","r12","r13","r14","r15",\
            //     "xmm0","xmm1","xmm2","xmm3","xmm4","xmm5","xmm6","xmm7","xmm8","xmm9","xmm10","xmm11","xmm12","xmm13","xmm14","xmm15","cc","memory");\
            //     TAIL_SET_OFFSET(ndim) a_pointer -= M * K; b_pointer += ndim * K; c_pointer += (LDC * ndim - M);\
            // }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void COMPUTE(int ndim) {
                HEAD_SET_OFFSET(ndim); next_b = b_pointer + ndim * K;
                ymm0 = Vector256.Create(alpha); // vbroadcastss (%6),%%ymm0;
                r12 = K; // movq %4,%%r12;
                r12 <<= 2; // salq $2,%%r12;
                r14 = b_pointer;// movq %1,%%r14;
                r11 = M; // movq %8,%%r11;
                r13 = 0; // #define INIT_SET_KSKIP "xorq %%r13,%%r13;"
                while (r11 >= 8) { // cmpq $8,%%r11;jb 33101
                    //COMPUTE_m8(ndim);
                    r11 -= 8; // subq $8,%%r11;
                } // cmpq $8,%%r11;jnb 33109
                if (r11 >= 4) { // cmpq $4,%%r11;jb 33103
                    //COMPUTE_m4(ndim);
                    r11 -= 4; // subq $4,%%r11;
                }
                if (r11 >= 2) { // cmpq $2,%%r11;jb 33104
                    //COMPUTE_m2(ndim);
                    r11 -= 2; // subq $2,%%r11;
                }
                if (r11 >= 1) { // testq %%r11,%%r11;jz 33105
                    COMPUTE_m1(ndim);
                }
                K = r12; // movq %%r12,%4;
                K >>= 2; // sarq $2,%4;
                b_pointer = r14; // movq %%r14,%1;
                TAIL_SET_OFFSET(ndim); a_pointer -= M * K; b_pointer += ndim * K; c_pointer += (LDC * ndim - M);
            }
            // Finish - sgemm_ncopy_4_skylakex
        }

#endif // NET7_0_OR_GREATER
    }
}
