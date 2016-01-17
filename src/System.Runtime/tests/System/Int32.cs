// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class Int32Tests
    {
        [Fact]
        public static void TestCtorEmpty()
        {
            int i = new int();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void TestCtorValue()
        {
            int i = 41;
            Assert.Equal(41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0x7FFFFFFF, int.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal(unchecked((int)0x80000000), int.MinValue);
        }

        [Theory]
        [InlineData(234, 0)]
        [InlineData(int.MinValue, 1)]
        [InlineData(-123, 1)]
        [InlineData(0, 1)]
        [InlineData(123, 1)]
        [InlineData(456, -1)]
        [InlineData(int.MaxValue, -1)]
        [InlineData(null, 1)]
        public static void TestCompareTo(object value, int expected)
        {
            int i = 234;
            if (value is int)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((int)value)));
            }

            IComparable iComparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = 234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not an int
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo((long)234)); // Obj is not an int
        }

        [Theory]
        [InlineData(789, 789, true)]
        [InlineData(789, -789, false)]
        [InlineData(789, 0, false)]
        [InlineData(0, 0, true)]
        [InlineData(789, null, false)]
        [InlineData(789, "789", false)]
        [InlineData(789, (long)789, false)]
        public static void TestEquals(int i1, object obj, bool expected)
        {
            if (obj is int)
            {
                Assert.Equal(expected, i1.Equals((int)obj));
                Assert.Equal(expected, i1.GetHashCode().Equals(((int)obj).GetHashCode()));
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        [Theory]
        [InlineData(int.MinValue, "-2147483648")]
        [InlineData(-4567, "-4567")]
        [InlineData(0, "0")]
        [InlineData(4567, "4567")]
        [InlineData(int.MaxValue, "2147483647")]
        public static void TestToString(int i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        public static IEnumerable<object[]> ToString_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { -6789, defaultFormat, "-6789" };
            yield return new object[] { 0, defaultFormat, "0" };
            yield return new object[] { 6789, defaultFormat, "6789" };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { -6789, customFormat, "#6789" };
            yield return new object[] { 6789, customFormat, "6789" };
        }

        [Theory, MemberData("ToString_FormatProvider_TestData")]
        public static void TestToString_FormatProvider(int i, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(provider));
        }

        public static IEnumerable<object[]> ToString_Format_TestData()
        {
            yield return new object[] { -6321, "G", "-6321" };
            yield return new object[] { 6321, "G", "6321" };
            yield return new object[] { 0x2468, "X", "2468" };
            yield return new object[] { 2468, "N", string.Format("{0:N}", 2468.00) };
        }

        [Theory, MemberData("ToString_Format_TestData")]
        public static void TestToString_Format(int i, string format, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant()));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant()));
        }

        [Fact]
        public static void TestToString_Format_Invalid()
        {
            int i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y"));
        }

        public static IEnumerable<object[]> ToString_Format_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { -6321, "G", defaultFormat, "-6321" };
            yield return new object[] { 6321, "G", defaultFormat, "6321" };
            yield return new object[] { 0x2468, "x", defaultFormat, "2468" };
            yield return new object[] { 2468, "N", defaultFormat, string.Format("{0:N}", 2468.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { -2468, "N", customFormat, "#2*468~00" };
            yield return new object[] { 2468, "N", customFormat, "2*468~00" };
        }

        [Theory, MemberData("ToString_Format_FormatProvider_TestData")]
        public static void TestToString_Format_FormatProvider(int i, string format, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Format_FormatProvider_Invalid()
        {
            long i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y", null));
        }

        public static IEnumerable<object[]> Parse_Valid_Data()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;
            var emptyFormat = new NumberFormatInfo();

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "-2147483648", defaultStyle, nullFormat, -2147483648 };
            yield return new object[] { "-123", defaultStyle, nullFormat, -123 };
            yield return new object[] { "0", defaultStyle, nullFormat, 0 };
            yield return new object[] { "123", defaultStyle, nullFormat, 123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, 123 };
            yield return new object[] { "2147483647", defaultStyle, nullFormat, 2147483647 };

            yield return new object[] { "123", NumberStyles.HexNumber, nullFormat, 0x123 };
            yield return new object[] { "abc", NumberStyles.HexNumber, nullFormat, 0xabc };
            yield return new object[] { "1000", NumberStyles.AllowThousands, nullFormat, 1000 };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, -123 }; // Parentheses = negative

            yield return new object[] { "123", defaultStyle, emptyFormat, 123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, 123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, 0x12 };
            yield return new object[] { "$1,000", NumberStyles.Currency, customFormat, 1000 };
        }
        
        [Theory, MemberData("Parse_Valid_Data")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, int expected)
        {
            int i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(int.TryParse(value, out i));
                Assert.Equal(expected, i);

                Assert.Equal(expected, int.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, int.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(int.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(expected, i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, int.Parse(value, style));
            }
            Assert.Equal(expected, int.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> Parse_Invalid_Data()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";
            customFormat.NumberDecimalSeparator = ".";

            yield return new object[] { null, defaultStyle, nullFormat, typeof(ArgumentNullException) };
            yield return new object[] { "", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { " ", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { "Garbage", defaultStyle, nullFormat, typeof(FormatException) };

            yield return new object[] { "abc", defaultStyle, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "1E23", defaultStyle, nullFormat, typeof(FormatException) }; // Exponent
            yield return new object[] { "(123)", defaultStyle, nullFormat, typeof(FormatException) }; // Parentheses
            yield return new object[] { 1000.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency
            yield return new object[] { 1000.ToString("N0"), defaultStyle, nullFormat, typeof(FormatException) }; // Thousands
            yield return new object[] { 678.90.ToString("F2"), defaultStyle, nullFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "abc", NumberStyles.None, nullFormat, typeof(FormatException) }; // Negative hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace

            yield return new object[] { "67.90", defaultStyle, customFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "-2147483649", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
            yield return new object[] { "2147483648", defaultStyle, nullFormat, typeof(OverflowException) }; // < min value
        }

        [Theory, MemberData("Parse_Invalid_Data")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            int i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(int.TryParse(value, out i));
                Assert.Equal(default(int), i);

                Assert.Throws(exceptionType, () => int.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => int.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(int.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(default(int), i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => int.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => int.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
    }
}
