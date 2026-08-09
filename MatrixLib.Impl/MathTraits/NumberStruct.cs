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

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值结构体.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct NumberStruct<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T> : IEquatable<NumberStruct<T>>
#if NET7_0_OR_GREATER
        , INumberBase<NumberStruct<T>> where T : INumberBase<T>
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
        //static NumberStruct() {
        //    try {
        //        Debugger.Break();
        //        ZeroOfTypes<NumberStruct<T>>.Register(new NumberStruct<T>());
        //    } catch(Exception ex) {
        //        Debug.WriteLine("NumberStruct<" + typeof(T).Name + "> register fail! " + ex.ToString());
        //    }
        //}

        public NumberStruct(): this(default!) {
        }

        public NumberStruct(T value) {
            //if (value is null) throw new ArgumentNullException(nameof(value));
            m_value = value;
        }

#if NET7_0_OR_GREATER
        public static NumberStruct<T> One => T.One;

        public static int Radix => T.Radix;

        public static NumberStruct<T> Zero => T.Zero;

        public static NumberStruct<T> AdditiveIdentity => T.AdditiveIdentity;

        public static NumberStruct<T> MultiplicativeIdentity => T.MultiplicativeIdentity;

        public static NumberStruct<T> Abs(NumberStruct<T> value) {
            return T.Abs(value.Value);
        }

        public static bool IsCanonical(NumberStruct<T> value) {
            return T.IsCanonical(value.Value);
        }

        public static bool IsComplexNumber(NumberStruct<T> value) {
            return T.IsComplexNumber(value.Value);
        }

        public static bool IsEvenInteger(NumberStruct<T> value) {
            return T.IsEvenInteger(value.Value);
        }

        public static bool IsFinite(NumberStruct<T> value) {
            return T.IsFinite(value.Value);
        }

        public static bool IsImaginaryNumber(NumberStruct<T> value) {
            return T.IsImaginaryNumber(value.Value);
        }

        public static bool IsInfinity(NumberStruct<T> value) {
            return T.IsInfinity(value.Value);
        }

        public static bool IsInteger(NumberStruct<T> value) {
            return T.IsInteger(value.Value);
        }

        public static bool IsNaN(NumberStruct<T> value) {
            return T.IsNaN(value.Value);
        }

        public static bool IsNegative(NumberStruct<T> value) {
            return T.IsNegative(value.Value);
        }

        public static bool IsNegativeInfinity(NumberStruct<T> value) {
            return T.IsNegativeInfinity(value.Value);
        }

        public static bool IsNormal(NumberStruct<T> value) {
            return T.IsNormal(value.Value);
        }

        public static bool IsOddInteger(NumberStruct<T> value) {
            return T.IsOddInteger(value.Value);
        }

        public static bool IsPositive(NumberStruct<T> value) {
            return T.IsPositive(value.Value);
        }

        public static bool IsPositiveInfinity(NumberStruct<T> value) {
            return T.IsPositiveInfinity(value.Value);
        }

        public static bool IsRealNumber(NumberStruct<T> value) {
            return T.IsRealNumber(value.Value);
        }

        public static bool IsSubnormal(NumberStruct<T> value) {
            return T.IsSubnormal(value.Value);
        }

        public static bool IsZero(NumberStruct<T> value) {
            return T.IsZero(value.Value);
        }

        public static NumberStruct<T> MaxMagnitude(NumberStruct<T> x, NumberStruct<T> y) {
            return T.MaxMagnitude(x.Value, y.Value);
        }

        public static NumberStruct<T> MaxMagnitudeNumber(NumberStruct<T> x, NumberStruct<T> y) {
            return T.MaxMagnitudeNumber(x.Value, y.Value);
        }

        public static NumberStruct<T> MinMagnitude(NumberStruct<T> x, NumberStruct<T> y) {
            return T.MinMagnitude(x.Value, y.Value);
        }

        public static NumberStruct<T> MinMagnitudeNumber(NumberStruct<T> x, NumberStruct<T> y) {
            return T.MinMagnitudeNumber(x.Value, y.Value);
        }

        public static NumberStruct<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static NumberStruct<T> Parse(string s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static NumberStruct<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static NumberStruct<T> Parse(string s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out NumberStruct<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromChecked(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out NumberStruct<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromSaturating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out NumberStruct<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromTruncating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToChecked<TOther>(NumberStruct<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToChecked<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToSaturating<TOther>(NumberStruct<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToSaturating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToTruncating<TOther>(NumberStruct<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToTruncating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberStruct<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberStruct<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberStruct<T> result) {
            bool flag = T.TryParse(s, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberStruct<T> result) {
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

        public static NumberStruct<T> operator +(NumberStruct<T> value) {
            return +value.Value;
        }

        public static NumberStruct<T> operator +(NumberStruct<T> left, NumberStruct<T> right) {
            return left.Value + right.Value;
        }

        public static NumberStruct<T> operator -(NumberStruct<T> value) {
            return -value.Value;
        }

        public static NumberStruct<T> operator -(NumberStruct<T> left, NumberStruct<T> right) {
            return left.Value - right.Value;
        }

        public static NumberStruct<T> operator ++(NumberStruct<T> value) {
            return value.Value + T.One;
        }

        public static NumberStruct<T> operator --(NumberStruct<T> value) {
            return value.Value - T.One;
        }

        public static NumberStruct<T> operator *(NumberStruct<T> left, NumberStruct<T> right) {
            return left.Value * right.Value;
        }

        public static NumberStruct<T> operator /(NumberStruct<T> left, NumberStruct<T> right) {
            return left.Value / right.Value;
        }

#endif // NET7_0_OR_GREATER

        public static implicit operator NumberStruct<T>(T src) {
            return new NumberStruct<T>(src);
        }

        public bool Equals(NumberStruct<T> other) {
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
            return obj is NumberStruct<T> temp && Equals(temp);
        }

        public override int GetHashCode() {
            return (Value is null) ? 0 : Value.GetHashCode();
        }

        public static bool operator ==(NumberStruct<T> left, NumberStruct<T> right) {
            return left.Equals(right);
        }

        public static bool operator !=(NumberStruct<T> left, NumberStruct<T> right) {
            return !left.Equals(right);
        }

    }
}
