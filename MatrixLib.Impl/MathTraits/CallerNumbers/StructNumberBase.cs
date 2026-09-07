using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.CallerNumbers {
    /// <summary>
    /// 数值结构体.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct StructNumberBase<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T> : IEquatable<StructNumberBase<T>>
#if NET7_0_OR_GREATER
        , INumberBase<StructNumberBase<T>>
#endif // NET7_0_OR_GREATER
        where T : IEquatable<T>
#if NET7_0_OR_GREATER
        , INumberBase<T>
#endif // NET7_0_OR_GREATER
    {
        private T m_value;

        /// <summary>
        /// 值.
        /// </summary>
        public T Value {
            get { return m_value; }
            set { m_value = value; }
        }

        // 实测无效.
        //static StructNumberBase() {
        //    try {
        //        //Debugger.Break();
        //        //ZeroOfTypes<StructNumberBase<T>>.Register(new StructNumberBase<T>());
        //        NumberTraitsCache<StructNumberBase<T>>.Register();
        //    } catch (Exception ex) {
        //        Debug.WriteLine("StructNumberBase<" + typeof(T).Name + "> register fail! " + ex.ToString());
        //    }
        //}

        public StructNumberBase(): this(default!) {
        }

        public StructNumberBase(T value) {
            //if (value is null) throw new ArgumentNullException(nameof(value));
            m_value = value;
        }

#if NET7_0_OR_GREATER
        public static StructNumberBase<T> One => T.One;

        public static int Radix => T.Radix;

        public static StructNumberBase<T> Zero => T.Zero;

        public static StructNumberBase<T> AdditiveIdentity => T.AdditiveIdentity;

        public static StructNumberBase<T> MultiplicativeIdentity => T.MultiplicativeIdentity;

        public static StructNumberBase<T> Abs(StructNumberBase<T> value) {
            return T.Abs(value.Value);
        }

        public static bool IsCanonical(StructNumberBase<T> value) {
            return T.IsCanonical(value.Value);
        }

        public static bool IsComplexNumber(StructNumberBase<T> value) {
            return T.IsComplexNumber(value.Value);
        }

        public static bool IsEvenInteger(StructNumberBase<T> value) {
            return T.IsEvenInteger(value.Value);
        }

        public static bool IsFinite(StructNumberBase<T> value) {
            return T.IsFinite(value.Value);
        }

        public static bool IsImaginaryNumber(StructNumberBase<T> value) {
            return T.IsImaginaryNumber(value.Value);
        }

        public static bool IsInfinity(StructNumberBase<T> value) {
            return T.IsInfinity(value.Value);
        }

        public static bool IsInteger(StructNumberBase<T> value) {
            return T.IsInteger(value.Value);
        }

        public static bool IsNaN(StructNumberBase<T> value) {
            return T.IsNaN(value.Value);
        }

        public static bool IsNegative(StructNumberBase<T> value) {
            return T.IsNegative(value.Value);
        }

        public static bool IsNegativeInfinity(StructNumberBase<T> value) {
            return T.IsNegativeInfinity(value.Value);
        }

        public static bool IsNormal(StructNumberBase<T> value) {
            return T.IsNormal(value.Value);
        }

        public static bool IsOddInteger(StructNumberBase<T> value) {
            return T.IsOddInteger(value.Value);
        }

        public static bool IsPositive(StructNumberBase<T> value) {
            return T.IsPositive(value.Value);
        }

        public static bool IsPositiveInfinity(StructNumberBase<T> value) {
            return T.IsPositiveInfinity(value.Value);
        }

        public static bool IsRealNumber(StructNumberBase<T> value) {
            return T.IsRealNumber(value.Value);
        }

        public static bool IsSubnormal(StructNumberBase<T> value) {
            return T.IsSubnormal(value.Value);
        }

        public static bool IsZero(StructNumberBase<T> value) {
            return T.IsZero(value.Value);
        }

        public static StructNumberBase<T> MaxMagnitude(StructNumberBase<T> x, StructNumberBase<T> y) {
            return T.MaxMagnitude(x.Value, y.Value);
        }

        public static StructNumberBase<T> MaxMagnitudeNumber(StructNumberBase<T> x, StructNumberBase<T> y) {
            return T.MaxMagnitudeNumber(x.Value, y.Value);
        }

        public static StructNumberBase<T> MinMagnitude(StructNumberBase<T> x, StructNumberBase<T> y) {
            return T.MinMagnitude(x.Value, y.Value);
        }

        public static StructNumberBase<T> MinMagnitudeNumber(StructNumberBase<T> x, StructNumberBase<T> y) {
            return T.MinMagnitudeNumber(x.Value, y.Value);
        }

        public static StructNumberBase<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static StructNumberBase<T> Parse(string s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static StructNumberBase<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static StructNumberBase<T> Parse(string s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out StructNumberBase<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromChecked(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out StructNumberBase<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromSaturating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out StructNumberBase<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromTruncating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToChecked<TOther>(StructNumberBase<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToChecked<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToSaturating<TOther>(StructNumberBase<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToSaturating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToTruncating<TOther>(StructNumberBase<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToTruncating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out StructNumberBase<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out StructNumberBase<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out StructNumberBase<T> result) {
            bool flag = T.TryParse(s, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out StructNumberBase<T> result) {
            bool flag = T.TryParse(s, provider, out var temp);
            result = temp!;
            return flag;
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            throw new NotImplementedException();
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
            throw new NotImplementedException();
        }

        public static StructNumberBase<T> operator +(StructNumberBase<T> value) {
            return +value.Value;
        }

        public static StructNumberBase<T> operator +(StructNumberBase<T> left, StructNumberBase<T> right) {
            return left.Value + right.Value;
        }

        public static StructNumberBase<T> operator -(StructNumberBase<T> value) {
            return -value.Value;
        }

        public static StructNumberBase<T> operator -(StructNumberBase<T> left, StructNumberBase<T> right) {
            return left.Value - right.Value;
        }

        public static StructNumberBase<T> operator ++(StructNumberBase<T> value) {
            return value.Value + T.One;
        }

        public static StructNumberBase<T> operator --(StructNumberBase<T> value) {
            return value.Value - T.One;
        }

        public static StructNumberBase<T> operator *(StructNumberBase<T> left, StructNumberBase<T> right) {
            return left.Value * right.Value;
        }

        public static StructNumberBase<T> operator /(StructNumberBase<T> left, StructNumberBase<T> right) {
            return left.Value / right.Value;
        }

#endif // NET7_0_OR_GREATER

        public static implicit operator StructNumberBase<T>(T src) {
            return new StructNumberBase<T>(src);
        }

        public bool Equals(StructNumberBase<T> other) {
            if (Value is null) {
                if (other.Value is null) {
                    return true;
                } else {
                    return other.Value.Equals(Value);
                }
            }
            return Value.Equals(other.Value);
        }

        public override bool Equals(object? obj) {
            return obj is StructNumberBase<T> temp && Equals(temp);
        }

        public override int GetHashCode() {
            return (Value is null) ? 0 : Value.GetHashCode();
        }

        public static bool operator ==(StructNumberBase<T> left, StructNumberBase<T> right) {
            return left.Equals(right);
        }

        public static bool operator !=(StructNumberBase<T> left, StructNumberBase<T> right) {
            return !left.Equals(right);
        }

    }
}
