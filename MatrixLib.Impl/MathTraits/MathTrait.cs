using MatrixLib.MathTraits.Numbers;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 泛型数学的类型萃取.
    /// </summary>
    public static class MathTrait {
        //        public static TraitsINumberBase<T> GetINumberBase<T>(T dummy)
        //#if NET9_0_OR_GREATER
        //        where T : allows ref struct
        //#endif // NET9_0_OR_GREATER
        //        {
        //            _ = dummy;
        //            return new TraitsINumberBase<T>();
        //        }

        //#if NET7_0_OR_GREATER
        //        public static TraitsINumberBase_Where<T> GetINumberBase<T>(T dummy) where T : INumberBase<T> {
        //            _ = dummy;
        //            return new TraitsINumberBase_Where<T>();
        //        }
        //#endif // NET7_0_OR_GREATER

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OutINumberBase<T>(out TraitsINumberBase<T> traits, T dummy)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif // NET9_0_OR_GREATER
        {
            _ = dummy;
            traits = new TraitsINumberBase<T>();
        }

#if NET7_0_OR_GREATER
        [OverloadResolutionPriority(1)] // 即使用了该特性, 但是 net9 点击进入依然不是它, 这种办法的可行性存疑. 故更推荐 Using 方案.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetINumberBase<T>(out HasWhere.TraitsINumberBase<T> traits, T dummy) where T : INumberBase<T> {
            _ = dummy;
            traits = new HasWhere.TraitsINumberBase<T>();
        }
#endif // NET7_0_OR_GREATER

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static INumberBaseVisitor<T> GetVisitorItf<T>() {
            if (false) {
            } else if (typeof(T) == typeof(long)) {
                return (INumberBaseVisitor<T>)(object)(INumberBaseVisitor<long>)(new NumberVisitorInt64());
            } else if (typeof(T) == typeof(ulong)) {
                return (INumberBaseVisitor<T>)(object)(INumberBaseVisitor<ulong>)(new NumberVisitorUInt64());
            }
            throw new NotSupportedException(string.Format("Not supported type {0}!", typeof(T).FullName));
        }
    }

}
