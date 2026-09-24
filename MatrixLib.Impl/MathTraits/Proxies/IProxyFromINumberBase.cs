using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Proxies {
#if NET7_0_OR_GREATER
    /// <summary>
    /// 将 <see cref="INumberBase{T}"/> 转为 数值运算接口.
    /// </summary>
    /// <typeparam name="TSelf">Self type (自身类型).</typeparam>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    public partial interface IProxyFromINumberBase<TSelf, T>: IProxyBase<TSelf, T> where TSelf : IProxyBase<TSelf, T>, new() where T : INumberBase<T> {
        /// <summary>
        /// 实例.
        /// </summary>
        public static TSelf Instance { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; } = new();


        /// <inheritdoc cref="INumberBaseCaller{T}.DoZero"/>
        public static NumberBaseProxy<T> Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return T.Zero;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSelf Addition(TSelf left, TSelf right) => Instance.From(Instance.To(left) + Instance.To(right));
        //public static TSelf Addition(TSelf left, TSelf right) => Instance.DoAddition(left, right); // CS1061 'TSelf' does not contain a definition for 'DoAddition' and no accessible extension method 'DoAddition' accepting a first argument of type 'TSelf' could be found (are you missing a using directive or an assembly reference?)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSelf Multiply(TSelf left, TSelf right) => Instance.From(Instance.To(left) * Instance.To(right));

        // CS0563	One of the parameters of a binary operator must be the containing type
        // CS0563：二元运算符其中一个参数必须是包含类型。`public static` 的二元运算符，**至少有一个参数必须是当前所在类型（包含类型）**。
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static NumberBaseProxy<T> operator +(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
        //    return left.Value + right.Value;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static NumberBaseProxy<T> operator *(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
        //    return left.Value * right.Value;
        //}

    }
#endif // NET7_0_OR_GREATER
}
