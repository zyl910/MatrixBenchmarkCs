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
    public partial interface IProxyFromINumberBase<TSelf, T>: IProxyBase<TSelf, T> where T: INumberBase<T>, new() {

        public new static NumberBaseProxy<T> Zero {
            // Zero 同名. CS0108	A variable was declared with the same name as a variable in a base type. However, the new keyword was not used. This warning informs you that you should use new; the variable is declared as if new had been used in the declaration.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return T.Zero;
            }
        }

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
