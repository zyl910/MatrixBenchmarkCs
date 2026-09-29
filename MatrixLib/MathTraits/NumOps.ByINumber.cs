using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MatrixLib.MathTraits {
    partial struct NumOps<T> {

        public bool SupportIComparable {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get {
                if (typeof(T) == typeof(float)
                    || typeof(T) == typeof(double)
                    || typeof(T) == typeof(sbyte)
                    || typeof(T) == typeof(short)
                    || typeof(T) == typeof(int)
                    || typeof(T) == typeof(long)
                    || typeof(T) == typeof(byte)
                    || typeof(T) == typeof(ushort)
                    || typeof(T) == typeof(uint)
                    || typeof(T) == typeof(ulong)
                    ) {
                    return true;
                } else if (NumberTraitsUtil.IsCommonTypes<T>()) {
                    return false;
                } else {
                    var CT = NumberTraitsCacheV3<T>.NumberBase;
                    if (CT is not null) {
                        return CT.SupportIComparable;
                    }
                    throw new NotSupportedException(string.Format("The type {0} not supported {1}!", typeof(T).FullName, nameof(SupportIComparable)));
                }
            }
        }

        public bool SupportIComparisonOperators {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (typeof(T) == typeof(float)
                    || typeof(T) == typeof(double)
                    || typeof(T) == typeof(sbyte)
                    || typeof(T) == typeof(short)
                    || typeof(T) == typeof(int)
                    || typeof(T) == typeof(long)
                    || typeof(T) == typeof(byte)
                    || typeof(T) == typeof(ushort)
                    || typeof(T) == typeof(uint)
                    || typeof(T) == typeof(ulong)
                    ) {
                    return true;
                } else if (NumberTraitsUtil.IsCommonTypes<T>()) {
                    return false;
                } else {
                    var CT = NumberTraitsCacheV3<T>.NumberBase;
                    if (CT is not null) {
                        return CT.SupportIComparisonOperators;
                    }
                    throw new NotSupportedException(string.Format("The type {0} not supported {1}!", typeof(T).FullName, nameof(SupportIComparisonOperators)));
                }
            }
        }

        public bool SupportIModulusOperators {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (typeof(T) == typeof(float)
                    || typeof(T) == typeof(double)
                    || typeof(T) == typeof(sbyte)
                    || typeof(T) == typeof(short)
                    || typeof(T) == typeof(int)
                    || typeof(T) == typeof(long)
                    || typeof(T) == typeof(byte)
                    || typeof(T) == typeof(ushort)
                    || typeof(T) == typeof(uint)
                    || typeof(T) == typeof(ulong)
                    ) {
                    return true;
                } else if (NumberTraitsUtil.IsCommonTypes<T>()) {
                    return false;
                } else {
                    var CT = NumberTraitsCacheV3<T>.NumberBase;
                    if (CT is not null) {
                        return CT.SupportIModulusOperators;
                    }
                    throw new NotSupportedException(string.Format("The type {0} not supported {1}!", typeof(T).FullName, nameof(SupportIModulusOperators)));
                }
            }
        }

        public int DoCompareTo(T self, T? other) {
            if (self is null && other is null) {
                return 0;
            } else if (self is null) {
                throw new NotSupportedException(string.Format("The type {0} self param is null!", typeof(T).FullName));
            } else if (typeof(T) == typeof(float)) {
                return ((float)(object)self).CompareTo((float)(object)other!);
            } else if (typeof(T) == typeof(double)) {
                return ((double)(object)self).CompareTo((double)(object)other!);
            } else if (typeof(T) == typeof(sbyte)) {
                return ((sbyte)(object)self).CompareTo((sbyte)(object)other!);
            } else if (typeof(T) == typeof(short)) {
                return ((short)(object)self).CompareTo((short)(object)other!);
            } else if (typeof(T) == typeof(int)) {
                return ((int)(object)self).CompareTo((int)(object)other!);
            } else if (typeof(T) == typeof(long)) {
                return ((long)(object)self).CompareTo((long)(object)other!);
            } else if (typeof(T) == typeof(byte)) {
                return ((byte)(object)self).CompareTo((byte)(object)other!);
            } else if (typeof(T) == typeof(ushort)) {
                return ((ushort)(object)self).CompareTo((ushort)(object)other!);
            } else if (typeof(T) == typeof(uint)) {
                return ((uint)(object)self).CompareTo((uint)(object)other!);
            } else if (typeof(T) == typeof(ulong)) {
                return ((ulong)(object)self).CompareTo((ulong)(object)other!);
            } else {
                var CT = NumberTraitsCacheV3<T>.NumberBase;
                if (CT is not null) {
                    return CT.DoCompareTo(self, other);
                }
                throw new NotSupportedException(string.Format("The type {0} not supported {1}!", typeof(T).FullName, nameof(DoCompareTo)));
            }
        }
    }
}
