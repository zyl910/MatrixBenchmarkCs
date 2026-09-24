using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies {
#if NET7_0_OR_GREATER
    partial interface IProxyFromINumberBase<TSelf, T> : INumberBaseCaller<TSelf> {

        TSelf INumberBaseCaller<TSelf>.DoZero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => From(T.Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TSelf INumberBaseCaller<TSelf>.DoAddition(TSelf left, TSelf right) => From(To(left) + To(right)); // 直接实现. 代码简单, 但不能复用.
        //TSelf INumberBaseCaller<TSelf>.Addition(TSelf left, TSelf right) => From(((INumberBaseCaller<T>)this).Addition(To(left), To(right))); // 通过接口复用. 但它存在接口装箱、动态接口方法查找等.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TSelf INumberBaseCaller<TSelf>.DoMultiply(TSelf left, TSelf right) => From(To(left) * To(right));

    }
#endif // NET7_0_OR_GREATER
}
