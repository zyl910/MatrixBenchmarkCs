using System.Runtime.CompilerServices;

namespace MatrixLib.MathTraits.Proxies {
#if NET7_0_OR_GREATER && FALSE
    // CS0695	'IProxyFromINumberBase<TSelf, T>' cannot implement both 'INumberBaseCaller<T>' and 'INumberBaseCaller<TSelf>' because they may unify for some type parameter substitutions
    // 同一个接口同时继承 `INumberBaseCaller<T>` 和 `INumberBaseCaller<TSelf>`，编译器判定：存在类型替换，使得 `T == TSelf`，此时变成同一个接口，造成重复实现冲突，C# 禁止这种可能发生泛型合一（unification）的情况。

    partial interface IProxyFromINumberBase<TSelf, T> : INumberBaseCaller<T> {

        T INumberBaseCaller<T>.Zero { [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => T.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumberBaseCaller<T>.Addition(T left, T right) => left + right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T INumberBaseCaller<T>.Multiply(T left, T right) => left * right;

    }
#endif // NET7_0_OR_GREATER
}
