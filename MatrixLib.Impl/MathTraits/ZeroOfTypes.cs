using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 各类型的零值. 对于 类这样的引用类型、或 零值与default不同的结构体, 应事先给它的 <see cref="Zero"/> 属性赋值, 随后 <see cref="TraitsINumberBase{T}"/> 的 Pi 等属性能正常的获取值.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public class ZeroOfTypes<T> {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        //[MaybeNull]
        public static T Zero { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        /// <summary>
        /// 注册.
        /// </summary>
        /// <param name="zero"></param>
        public static void Register(T zero) {
            Zero = zero;
        }

    }
}
