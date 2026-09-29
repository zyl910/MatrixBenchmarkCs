using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits {
    // public interface INumber<TSelf>
    //     : IComparable,
    //       IComparable<TSelf>,
    //       IComparisonOperators<TSelf, TSelf, bool>,
    //       IModulusOperators<TSelf, TSelf, TSelf>,
    //       INumberBase<TSelf>
    //     where TSelf : INumber<TSelf>?

    partial interface INumOps<T> {

        /// <summary>Is support <see cref="IComparable{T}"/> (是否支持 <see cref="IComparable{T}"/>)</summary>
        public bool SupportIComparable { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        /// <summary>Is support <see cref="IComparisonOperators{TSelf, TSelf, bool}"/> (是否支持 <see cref="IComparisonOperators{TSelf, TSelf, bool}"/>)</summary>
        public bool SupportIComparisonOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }
        /// <summary>Is support <see cref="IModulusOperators{TSelf, TSelf, TSelf}"/> (是否支持 <see cref="IModulusOperators{TSelf, TSelf, TSelf}"/>)</summary>
        public bool SupportIModulusOperators { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="self">The current instance.</param>    
        /// <param name="other">An object to compare with this instance.</param>
        public int DoCompareTo(T self, T? other);

    }
}
