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
        [OverloadResolutionPriority(1)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetINumberBase<T>(out TraitsINumberBase_Where<T> traits, T dummy) where T : INumberBase<T> {
            _ = dummy;
            traits = new TraitsINumberBase_Where<T>();
        }
#endif // NET7_0_OR_GREATER
    }

}
