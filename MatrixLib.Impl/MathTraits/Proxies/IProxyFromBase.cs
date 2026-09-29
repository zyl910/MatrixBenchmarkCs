using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Proxies {

    /// <summary>
    /// 带有 From 等方法的代理的接口.
    /// </summary>
    public interface IProxyFromBase<TSelf, T> {

        // /// <summary>值.</summary>
        //public T Value {
        //    get;
        //}

        /// <summary>
        /// 将元素类型, 转为自身类型.
        /// </summary>
        /// <param name="src">源值.</param>
        /// <returns>返回转换后的值.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TSelf From(T src);

        /// <summary>
        /// 将自身类型, 转为元素类型.
        /// </summary>
        /// <param name="src">源值.</param>
        /// <returns>返回转换后的值.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T To(TSelf src);

    }
}
