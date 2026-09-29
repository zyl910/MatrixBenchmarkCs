#if FALSE
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Proxies {
#if NET7_0_OR_GREATER
    /// <summary>
    /// 将 <see cref="INumberBase{T}"/> 转为 数值运算接口.
    /// </summary>
    /// <typeparam name="TSelf">Self type (自身类型).</typeparam>
    /// <typeparam name="T">Element type (元素类型).</typeparam>
    [Obsolete("发现该接口不适合 纯 .NET Standard、泛型类型推导等情况. 建议改为 IMathNumProxy 等.")]
    public partial interface IProxyFromINumberBase<TSelf, T>: IProxyBase<TSelf, T> where TSelf : IProxyBase<TSelf, T>, new() where T : INumberBase<T> {

        public static virtual TSelf One => TSelf.From(T.One);

        public static virtual int Radix => T.Radix;

        public static virtual TSelf AdditiveIdentity => TSelf.From(T.AdditiveIdentity);

        public static virtual TSelf MultiplicativeIdentity => TSelf.From(T.MultiplicativeIdentity);

        /// <inheritdoc cref="INumOps{T}.DoZero"/>
        public static virtual TSelf Zero {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return TSelf.From(T.Zero);
            }
        }


        public static virtual TSelf Abs(TSelf value) {
            return TSelf.From(T.Abs(TSelf.To(value)));
        }

        public static virtual bool IsCanonical(TSelf value) {
            return T.IsCanonical(TSelf.To(value));
        }

        public static virtual bool IsComplexNumber(TSelf value) {
            return T.IsComplexNumber(TSelf.To(value));
        }

        public static virtual bool IsEvenInteger(TSelf value) {
            return T.IsEvenInteger(TSelf.To(value));
        }

        public static virtual bool IsFinite(TSelf value) {
            return T.IsFinite(TSelf.To(value));
        }

        public static virtual bool IsImaginaryNumber(TSelf value) {
            return T.IsImaginaryNumber(TSelf.To(value));
        }

        public static virtual bool IsInfinity(TSelf value) {
            return T.IsInfinity(TSelf.To(value));
        }

        public static virtual bool IsInteger(TSelf value) {
            return T.IsInteger(TSelf.To(value));
        }

        public static virtual bool IsNaN(TSelf value) {
            return T.IsNaN(TSelf.To(value));
        }

        public static virtual bool IsNegative(TSelf value) {
            return T.IsNegative(TSelf.To(value));
        }

        public static virtual bool IsNegativeInfinity(TSelf value) {
            return T.IsNegativeInfinity(TSelf.To(value));
        }

        public static virtual bool IsNormal(TSelf value) {
            return T.IsNormal(TSelf.To(value));
        }

        public static virtual bool IsOddInteger(TSelf value) {
            return T.IsOddInteger(TSelf.To(value));
        }

        public static virtual bool IsPositive(TSelf value) {
            return T.IsPositive(TSelf.To(value));
        }

        public static virtual bool IsPositiveInfinity(TSelf value) {
            return T.IsPositiveInfinity(TSelf.To(value));
        }

        public static virtual bool IsRealNumber(TSelf value) {
            return T.IsRealNumber(TSelf.To(value));
        }

        public static virtual bool IsSubnormal(TSelf value) {
            return T.IsSubnormal(TSelf.To(value));
        }

        public static virtual bool IsZero(TSelf value) {
            return T.IsZero(TSelf.To(value));
        }

        public static virtual TSelf MaxMagnitude(TSelf x, TSelf y) {
            return TSelf.From(T.MaxMagnitude(TSelf.To(x), TSelf.To(y)));
        }

        public static virtual TSelf MaxMagnitudeNumber(TSelf x, TSelf y) {
            return TSelf.From(T.MaxMagnitudeNumber(TSelf.To(x), TSelf.To(y)));
        }

        public static virtual TSelf MinMagnitude(TSelf x, TSelf y) {
            return TSelf.From(T.MinMagnitude(TSelf.To(x), TSelf.To(y)));
        }

        public static virtual TSelf MinMagnitudeNumber(TSelf x, TSelf y) {
            return TSelf.From(T.MinMagnitudeNumber(TSelf.To(x), TSelf.To(y)));
        }

        public static virtual TSelf Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
            return TSelf.From(T.Parse(s, style, provider));
        }

        public static virtual TSelf Parse(string s, NumberStyles style, IFormatProvider? provider) {
            return TSelf.From(T.Parse(s, style, provider));
        }

        public static virtual TSelf Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
            return TSelf.From(T.Parse(s, provider));
        }

        public static virtual TSelf Parse(string s, IFormatProvider? provider) {
            return TSelf.From(T.Parse(s, provider));
        }

        //public static virtual bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out TSelf result) where TOther : INumberBase<TOther> {
        //    bool flag = T.TryConvertFromChecked(value, out T temp);
        //    result = temp!;
        //    return flag;
        //}

        //public static virtual bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out TSelf result) where TOther : INumberBase<TOther> {
        //    bool flag = T.TryConvertFromSaturating(value, out T temp);
        //    result = temp!;
        //    return flag;
        //}

        //public static virtual bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out TSelf result) where TOther : INumberBase<TOther> {
        //    bool flag = T.TryConvertFromTruncating(value, out T temp);
        //    result = temp!;
        //    return flag;
        //}

        //public static virtual bool TryConvertToChecked<TOther>(TSelf value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
        //    bool flag = T.TryConvertToChecked<TOther>(TSelf.To(value), out var temp);
        //    result = temp!;
        //    return flag;
        //}

        //public static virtual bool TryConvertToSaturating<TOther>(TSelf value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
        //    bool flag = T.TryConvertToSaturating<TOther>(TSelf.To(value), out var temp);
        //    result = temp!;
        //    return flag;
        //}

        //public static virtual bool TryConvertToTruncating<TOther>(TSelf value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> {
        //    bool flag = T.TryConvertToTruncating<TOther>(TSelf.To(value), out var temp);
        //    result = temp!;
        //    return flag;
        //}

        public static virtual bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = TSelf.From(temp!);
            return flag;
        }

        public static virtual bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result) {
            bool flag = T.TryParse(s, style, provider, out var temp);
            result = TSelf.From(temp!);
            return flag;
        }

        public static virtual bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result) {
            bool flag = T.TryParse(s, provider, out var temp);
            result = TSelf.From(temp!);
            return flag;
        }

        public static virtual bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TSelf result) {
            bool flag = T.TryParse(s, provider, out var temp);
            result = TSelf.From(temp!);
            return flag;
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            return Value.ToString(format, formatProvider);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
            return Value.TryFormat(destination, out charsWritten, format, provider);
        }

        // == Ops --

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static virtual TSelf Addition(TSelf left, TSelf right) => TSelf.From(TSelf.To(left) + TSelf.To(right));
        //public static virtual TSelf Addition(TSelf left, TSelf right) => TSelf.DoAddition(left, right); // CS1061 'TSelf' does not contain a definition for 'DoAddition' and no accessible extension method 'DoAddition' accepting a first argument of type 'TSelf' could be found (are you missing a using directive or an assembly reference?)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static virtual TSelf Multiply(TSelf left, TSelf right) => TSelf.From(TSelf.To(left) * TSelf.To(right));

        // CS0563	One of the parameters of a binary operator must be the containing type
        // CS0563：二元运算符其中一个参数必须是包含类型。`public static virtual` 的二元运算符，**至少有一个参数必须是当前所在类型（包含类型）**。
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static virtual TSelf operator +(TSelf left, TSelf right) {
        //    return TSelf.From(TSelf.To(left) + TSelf.To(right));
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static virtual TSelf operator *(TSelf left, TSelf right) {
        //    return TSelf.From(TSelf.To(left) * TSelf.To(right));
        //}

    }
#endif // NET7_0_OR_GREATER
}
#endif // FALSE
