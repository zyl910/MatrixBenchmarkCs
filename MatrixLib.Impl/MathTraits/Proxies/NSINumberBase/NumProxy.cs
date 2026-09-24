using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Proxies.NSINumberBase {
#if NET7_0_OR_GREATER

    /// <summary>
    /// 数值代理.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public struct NumProxy<T>: IProxyFromINumberBase<NumProxy<T>, T> where T : INumberBase<T> {
        private T m_value;

        /// <summary>
        /// 值.
        /// </summary>
        public T Value {
            get { return m_value; }
            set { m_value = value; }
        }

        public NumProxy(T value) {
            //if (value is null) throw new ArgumentNullException(nameof(value));
            m_value = value;
        }

        public static NumProxy<T> From(T src) {
            return new NumProxy<T>(src);
        }

        public static T To(NumProxy<T> src) {
            return src.Value;
        }

    }

#endif // NET7_0_OR_GREATER
}
