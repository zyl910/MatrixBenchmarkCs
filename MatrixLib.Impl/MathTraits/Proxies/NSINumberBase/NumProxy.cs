#if FALSE
#define ALLOW_INTERFACE_STATIC

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MatrixLib.MathTraits.Proxies.NSINumberBase {
#if NET7_0_OR_GREATER

    /// <summary>
    /// 数值代理.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public struct NumProxy<T>: IProxyFromINumberBase<NumProxy<T>, T>, INumberBase<NumProxy<T>>
            where T : INumberBase<T> {
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

        public static implicit operator NumProxy<T>(T src) {
            return new NumProxy<T>(src);
        }

        public bool Equals(NumProxy<T> other) {
            if (Value is null) {
                if (other.Value is null) {
                    return true;
                } else {
                    return other.Value.Equals(Value!);
                }
            }
            return Value.Equals(other.Value);
        }

        public override bool Equals(object? obj) {
            return obj is NumProxy<T> temp && Equals(temp);
        }

        public override int GetHashCode() {
            return (Value is null) ? 0 : Value.GetHashCode();
        }

        public static bool operator ==(NumProxy<T> left, NumProxy<T> right) {
            return left.Value == right.Value;
        }

        public static bool operator !=(NumProxy<T> left, NumProxy<T> right) {
            return left.Value != right.Value;
        }

        public static NumProxy<T> Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
#if ALLOW_INTERFACE_STATIC
                return T.Zero;
#else
                //return INumberBase<NumProxy<T>>.base.Zero;
                //return base(INumberBase<NumProxy<T>>).Zero;
                return TraitsINumberBaseV3<T>.Instance.DoZero;
                //return INumberBase<NumProxy<T>>.Zero; // CS8926	A static virtual or abstract interface member can be accessed only on a type parameter.
                //return ((INumberBase<NumProxy<T>>)this).DoZero; // CS0026 Keyword 'this' is not valid in a static property, static method, or static field initializer
#endif
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumProxy<T> operator +(NumProxy<T> left, NumProxy<T> right) {
#if ALLOW_INTERFACE_STATIC
            return left.Value + right.Value;
#else
            //return ((INumberBase<NumProxy<T>>)this).DoAddition(left.Value, right.Value);
            return TraitsINumberBaseV3<T>.Instance.DoAddition(left.Value, right.Value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumProxy<T> operator *(NumProxy<T> left, NumProxy<T> right) {
#if ALLOW_INTERFACE_STATIC
            return left.Value * right.Value;
#else
            return TraitsINumberBaseV3<T>.Instance.DoMultiply(left.Value, right.Value);
#endif
        }

#if NET7_0_OR_GREATER
        public static NumProxy<T> One => T.One;

        public static int Radix => T.Radix;

        public static NumProxy<T> AdditiveIdentity => T.AdditiveIdentity;

        public static NumProxy<T> MultiplicativeIdentity => T.MultiplicativeIdentity;

        public static NumProxy<T> Abs(NumProxy<T> value) {
            return T.Abs(value.Value);
        }

        public static bool IsCanonical(NumProxy<T> value) {
            return T.IsCanonical(value.Value);
        }

        public static bool IsComplexNumber(NumProxy<T> value) {
            return T.IsComplexNumber(value.Value);
        }

        public static bool IsEvenInteger(NumProxy<T> value) {
            return T.IsEvenInteger(value.Value);
        }

        public static bool IsFinite(NumProxy<T> value) {
            return T.IsFinite(value.Value);
        }

        public static bool IsImaginaryNumber(NumProxy<T> value) {
            return T.IsImaginaryNumber(value.Value);
        }

        public static bool IsInfinity(NumProxy<T> value) {
            return T.IsInfinity(value.Value);
        }

        public static bool IsInteger(NumProxy<T> value) {
            return T.IsInteger(value.Value);
        }

        public static bool IsNaN(NumProxy<T> value) {
            return T.IsNaN(value.Value);
        }

        public static bool IsNegative(NumProxy<T> value) {
            return T.IsNegative(value.Value);
        }

        public static bool IsNegativeInfinity(NumProxy<T> value) {
            return T.IsNegativeInfinity(value.Value);
        }

        public static bool IsNormal(NumProxy<T> value) {
            return T.IsNormal(value.Value);
        }

        public static bool IsOddInteger(NumProxy<T> value) {
            return T.IsOddInteger(value.Value);
        }

        public static bool IsPositive(NumProxy<T> value) {
            return T.IsPositive(value.Value);
        }

        public static bool IsPositiveInfinity(NumProxy<T> value) {
            return T.IsPositiveInfinity(value.Value);
        }

        public static bool IsRealNumber(NumProxy<T> value) {
            return T.IsRealNumber(value.Value);
        }

        public static bool IsSubnormal(NumProxy<T> value) {
            return T.IsSubnormal(value.Value);
        }

        public static bool IsZero(NumProxy<T> value) {
            return T.IsZero(value.Value);
        }

        public static NumProxy<T> MaxMagnitude(NumProxy<T> x, NumProxy<T> y) {
            return T.MaxMagnitude(x.Value, y.Value);
        }

        public static NumProxy<T> MaxMagnitudeNumber(NumProxy<T> x, NumProxy<T> y) {
            return T.MaxMagnitudeNumber(x.Value, y.Value);
        }

        public static NumProxy<T> MinMagnitude(NumProxy<T> x, NumProxy<T> y) {
            return T.MinMagnitude(x.Value, y.Value);
        }

        public static NumProxy<T> MinMagnitudeNumber(NumProxy<T> x, NumProxy<T> y) {
            return T.MinMagnitudeNumber(x.Value, y.Value);
        }

        public static NumProxy<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static NumProxy<T> Parse(string s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static NumProxy<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static NumProxy<T> Parse(string s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out NumProxy<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromChecked(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out NumProxy<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromSaturating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out NumProxy<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromTruncating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToChecked<TOther>(NumProxy<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToChecked<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToSaturating<TOther>(NumProxy<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToSaturating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToTruncating<TOther>(NumProxy<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToTruncating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out NumProxy<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out NumProxy<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumProxy<T> result) {
            bool flag = T.TryParse(s, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumProxy<T> result) {
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

        public static NumProxy<T> operator +(NumProxy<T> value) {
            return +value.Value;
        }

        public static NumProxy<T> operator -(NumProxy<T> value) {
            return -value.Value;
        }

        public static NumProxy<T> operator -(NumProxy<T> left, NumProxy<T> right) {
            return left.Value - right.Value;
        }

        public static NumProxy<T> operator ++(NumProxy<T> value) {
            return value.Value + T.One;
        }

        public static NumProxy<T> operator --(NumProxy<T> value) {
            return value.Value - T.One;
        }

        public static NumProxy<T> operator /(NumProxy<T> left, NumProxy<T> right) {
            return left.Value / right.Value;
        }

#endif // NET7_0_OR_GREATER

    }

#endif // NET7_0_OR_GREATER
}
#endif // FALSE
