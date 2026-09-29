using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Proxies {
    /// <summary>
    /// 基本代理接口.
    /// </summary>
    public interface IProxyBase {
    }

#if NET7_0_OR_GREATER && FALSE
    /// <summary>
    /// 带类型的基本代理接口.
    /// </summary>
    public interface IProxyBase<TSelf, T> {

        /// <summary>值.</summary>
        public T Value {
            get;
        }

        /// <summary>
        /// 将元素类型, 转为自身类型.
        /// </summary>
        /// <param name="src">源值.</param>
        /// <returns>返回转换后的值.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract static TSelf From(T src);

        /// <summary>
        /// 将自身类型, 转为元素类型.
        /// </summary>
        /// <param name="src">源值.</param>
        /// <returns>返回转换后的值.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract static T To(TSelf src);

    }
#endif // NET7_0_OR_GREATER

}
