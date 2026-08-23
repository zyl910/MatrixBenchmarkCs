using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 数值类型萃取定义。
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class NumberTraitsDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T>: INumberTraitsDefine {
        public Type? CallElementType { get; set; }

        // /// <summary>零值.</summary>
        //public T Zero { get; set; } = default!;

        /// <summary>数值基础调用者.</summary>
        public INumberBaseCaller<T>? NumberBase { get; set; }
    }
}
