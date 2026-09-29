using System.Numerics;
using System.Runtime.InteropServices;

namespace MatrixLib.MathTraits.Proxies.NSINumberBase {
#if NET7_0_OR_GREATER

    /// <summary>
    /// 泛型数学转为数值操作者.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct MathToOps<T>: IProxyBase, INumberBaseCaller<T>, IMathToOps<T>
    where T : INumberBase<T> {

    }

#endif // NET7_0_OR_GREATER
}
