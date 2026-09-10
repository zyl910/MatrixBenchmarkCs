#if NET7_0_OR_GREATER
#define ALLOW_INTERFACE_STATIC
#endif // NET7_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值基本代理. 用它包装数值类型后, 使低版本 .NET Standard 里的泛型代码也能使用数学运算符.
    /// </summary>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public partial struct NumberBaseProxy<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
    T> : IEquatable<NumberBaseProxy<T>>
#if NET7_0_OR_GREATER
        , INumberBase<NumberBaseProxy<T>>
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
        //static NumberBaseProxy() {
        //    try {
        //        //Debugger.Break();
        //        //ZeroOfTypes<NumberBaseProxy<T>>.Register(new NumberBaseProxy<T>());
        //        NumberTraitsCache<NumberBaseProxy<T>>.Register();
        //    } catch (Exception ex) {
        //        Debug.WriteLine("NumberBaseProxy<" + typeof(T).Name + "> register fail! " + ex.ToString());
        //    }
        //}

        public NumberBaseProxy(): this(default!) {
        }

        public NumberBaseProxy(T value) {
            //if (value is null) throw new ArgumentNullException(nameof(value));
            m_value = value;
        }

        public static NumberBaseProxy<T> Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
#if ALLOW_INTERFACE_STATIC
                return T.Zero;
#else
                return default;
                //return TraitsINumberBaseV3<T>.Instance.CallZero();
#endif
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumberBaseProxy<T> operator +(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
#if ALLOW_INTERFACE_STATIC
            return left.Value + right.Value;
#else
            return TraitsINumberBaseV3<T>.Instance.Addition(left.Value, right.Value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumberBaseProxy<T> operator *(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
#if ALLOW_INTERFACE_STATIC
            return left.Value * right.Value;
#else
            return TraitsINumberBaseV3<T>.Instance.Multiply(left.Value, right.Value);
#endif
        }

#if NET7_0_OR_GREATER
        public static NumberBaseProxy<T> One => T.One;

        public static int Radix => T.Radix;

        public static NumberBaseProxy<T> AdditiveIdentity => T.AdditiveIdentity;

        public static NumberBaseProxy<T> MultiplicativeIdentity => T.MultiplicativeIdentity;

        public static NumberBaseProxy<T> Abs(NumberBaseProxy<T> value) {
            return T.Abs(value.Value);
        }

        public static bool IsCanonical(NumberBaseProxy<T> value) {
            return T.IsCanonical(value.Value);
        }

        public static bool IsComplexNumber(NumberBaseProxy<T> value) {
            return T.IsComplexNumber(value.Value);
        }

        public static bool IsEvenInteger(NumberBaseProxy<T> value) {
            return T.IsEvenInteger(value.Value);
        }

        public static bool IsFinite(NumberBaseProxy<T> value) {
            return T.IsFinite(value.Value);
        }

        public static bool IsImaginaryNumber(NumberBaseProxy<T> value) {
            return T.IsImaginaryNumber(value.Value);
        }

        public static bool IsInfinity(NumberBaseProxy<T> value) {
            return T.IsInfinity(value.Value);
        }

        public static bool IsInteger(NumberBaseProxy<T> value) {
            return T.IsInteger(value.Value);
        }

        public static bool IsNaN(NumberBaseProxy<T> value) {
            return T.IsNaN(value.Value);
        }

        public static bool IsNegative(NumberBaseProxy<T> value) {
            return T.IsNegative(value.Value);
        }

        public static bool IsNegativeInfinity(NumberBaseProxy<T> value) {
            return T.IsNegativeInfinity(value.Value);
        }

        public static bool IsNormal(NumberBaseProxy<T> value) {
            return T.IsNormal(value.Value);
        }

        public static bool IsOddInteger(NumberBaseProxy<T> value) {
            return T.IsOddInteger(value.Value);
        }

        public static bool IsPositive(NumberBaseProxy<T> value) {
            return T.IsPositive(value.Value);
        }

        public static bool IsPositiveInfinity(NumberBaseProxy<T> value) {
            return T.IsPositiveInfinity(value.Value);
        }

        public static bool IsRealNumber(NumberBaseProxy<T> value) {
            return T.IsRealNumber(value.Value);
        }

        public static bool IsSubnormal(NumberBaseProxy<T> value) {
            return T.IsSubnormal(value.Value);
        }

        public static bool IsZero(NumberBaseProxy<T> value) {
            return T.IsZero(value.Value);
        }

        public static NumberBaseProxy<T> MaxMagnitude(NumberBaseProxy<T> x, NumberBaseProxy<T> y) {
            return T.MaxMagnitude(x.Value, y.Value);
        }

        public static NumberBaseProxy<T> MaxMagnitudeNumber(NumberBaseProxy<T> x, NumberBaseProxy<T> y) {
            return T.MaxMagnitudeNumber(x.Value, y.Value);
        }

        public static NumberBaseProxy<T> MinMagnitude(NumberBaseProxy<T> x, NumberBaseProxy<T> y) {
            return T.MinMagnitude(x.Value, y.Value);
        }

        public static NumberBaseProxy<T> MinMagnitudeNumber(NumberBaseProxy<T> x, NumberBaseProxy<T> y) {
            return T.MinMagnitudeNumber(x.Value, y.Value);
        }

        public static NumberBaseProxy<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static NumberBaseProxy<T> Parse(string s, NumberStyles style, IFormatProvider? provider) {
            return T.Parse(s, style, provider);
        }

        public static NumberBaseProxy<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static NumberBaseProxy<T> Parse(string s, IFormatProvider? provider) {
            return T.Parse(s, provider);
        }

        public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out NumberBaseProxy<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromChecked(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out NumberBaseProxy<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromSaturating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out NumberBaseProxy<T> result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertFromTruncating(value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToChecked<TOther>(NumberBaseProxy<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToChecked<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToSaturating<TOther>(NumberBaseProxy<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToSaturating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryConvertToTruncating<TOther>(NumberBaseProxy<T> value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
            bool flag = T.TryConvertToTruncating<TOther>(value.Value, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberBaseProxy<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberBaseProxy<T> result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberBaseProxy<T> result) {
            bool flag = T.TryParse(s, provider, out var temp);
            result = temp!;
            return flag;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberBaseProxy<T> result) {
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

        public static NumberBaseProxy<T> operator +(NumberBaseProxy<T> value) {
            return +value.Value;
        }

        public static NumberBaseProxy<T> operator -(NumberBaseProxy<T> value) {
            return -value.Value;
        }

        public static NumberBaseProxy<T> operator -(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return left.Value - right.Value;
        }

        public static NumberBaseProxy<T> operator ++(NumberBaseProxy<T> value) {
            return value.Value + T.One;
        }

        public static NumberBaseProxy<T> operator --(NumberBaseProxy<T> value) {
            return value.Value - T.One;
        }

        public static NumberBaseProxy<T> operator /(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return left.Value / right.Value;
        }

#endif // NET7_0_OR_GREATER

        public static implicit operator NumberBaseProxy<T>(T src) {
            return new NumberBaseProxy<T>(src);
        }

        public bool Equals(NumberBaseProxy<T> other) {
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
            return obj is NumberBaseProxy<T> temp && Equals(temp);
        }

        public override int GetHashCode() {
            return (Value is null) ? 0 : Value.GetHashCode();
        }

        public static bool operator ==(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return left.Equals(right);
        }

        public static bool operator !=(NumberBaseProxy<T> left, NumberBaseProxy<T> right) {
            return !left.Equals(right);
        }

    }
}
