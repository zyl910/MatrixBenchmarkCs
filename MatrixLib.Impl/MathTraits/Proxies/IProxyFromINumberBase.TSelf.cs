#if FALSE
using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies {
#if NET7_0_OR_GREATER
    partial interface IProxyFromINumberBase<TSelf, T> : INumOps<TSelf> {

        TSelf INumOps<TSelf>.DoZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TSelf.From(T.Zero);
            //get => Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //TSelf INumOps<TSelf>.DoAddition(TSelf left, TSelf right) => TSelf.From(TSelf.To(left) + TSelf.To(right)); // 直接实现. 代码简单, 但不能复用.
        //TSelf INumOps<TSelf>.Addition(TSelf left, TSelf right) => From(((INumOps<T>)this).Addition(To(left), To(right))); // 通过接口复用. 但它存在接口装箱、动态接口方法查找等.
        TSelf INumOps<TSelf>.DoAddition(TSelf left, TSelf right) => DoAddition(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //TSelf INumOps<TSelf>.DoMultiply(TSelf left, TSelf right) => TSelf.From(TSelf.To(left) * TSelf.To(right));
        TSelf INumOps<TSelf>.DoMultiply(TSelf left, TSelf right) => DoMultiply(left, right);

    }
#endif // NET7_0_OR_GREATER
}
#endif // FALSE
