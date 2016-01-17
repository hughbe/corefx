// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class Int16Tests
    {
        [Fact]
        public static void TestCtorEmpty()
        {
            short i = new short();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void TestCtorValue()
        {
            short i = 41;
            Assert.Equal(41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0x7FFF, short.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal(unchecked((short)0x8000), short.MinValue);
        }

        [Theory]
        [InlineData((short)234, 0)]
        [InlineData(short.MinValue, 1)]
        [InlineData((short)-123, 1)]
        [InlineData((short)0, 1)]
        [InlineData((short)123, 1)]
        [InlineData((short)456, -1)]
        [InlineData(short.MaxValue, -1)]
        [InlineData(null, 1)]
        public static void TestCompareTo(object value, int expected)
        {
            short i = 234;
            if (value is short)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((short)value)));
            }

            IComparable iComparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (short)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a short
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a short
        }

        [Theory]
        [InlineData((short)789, (short)789, true)]
        [InlineData((short)789, (short)-789, false)]
        [InlineData((short)789, (short)0, false)]
        [InlineData((short)0, (short)0, true)]
        [InlineData((short)789, null, false)]
        [InlineData((short)789, "789", false)]
        [InlineData((short)789, 789, false)]
        public static void TestEquals(short i1, object obj, bool expected)
        {
            if (obj is short)
            {
                Assert.Equal(expected, i1.Equals((short)obj));
                Assert.Equal(expected, i1.GetHashCode().Equals(((short)obj).GetHashCode()));
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        [Theory]
        [InlineData(short.MinValue, "-32768")]
        [InlineData((short)-4567, "-4567")]
        [InlineData((short)0, "0")]
        [InlineData((short)4567, "4567")]
        [InlineData(short.MaxValue, "32767")]
        public static void TestToString(short i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        public static IEnumerable<object[]> ToString_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (short)-6789, defaultFormat, "-6789" };
            yield return new object[] { (short)0, defaultFormat, "0" };
            yield return new object[] { (short)6789, defaultFormat, "6789" };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (short)-6789, customFormat, "#6789" };
            yield return new object[] { (short)6789, customFormat, "6789" };
        }

        [Theory, MemberData("ToString_FormatProvider_TestData")]
        public static void TestToString_FormatProvider(short i, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(provider));
        }

        public static IEnumerable<object[]> ToString_Format_TestData()
        {
            yield return new object[] { (short)-6321, "G", "-6321" };
            yield return new object[] { (short)6321, "G", "6321" };
            yield return new object[] { (short)0x2468, "X", "2468" };
            yield return new object[] { (short)2468, "N", string.Format("{0:N}", 2468.00) };
        }

        [Theory, MemberData("ToString_Format_TestData")]
        public static void TestToString_Format(short i, string format, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant()));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant()));
        }

        public static IEnumerable<object[]> ToString_Format_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (short)-6321, "G", defaultFormat, "-6321" };
            yield return new object[] { (short)6321, "G", defaultFormat, "6321" };
            yield return new object[] { (short)0x2468, "x", defaultFormat, "2468" };
            yield return new object[] { (short)2468, "N", defaultFormat, string.Format("{0:N}", 2468.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (short)-2468, "N", customFormat, "#2*468~00" };
            yield return new object[] { (short)2468, "N", customFormat, "2*468~00" };
        }

        [Fact]
        public static void TestToString_Format_Invalid()
        {
            short i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y"));
        }

        [Theory, MemberData("ToString_Format_FormatProvider_TestData")]
        public static void TestToString_Format_FormatProvider(short i, string format, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Format_FormatProvider_Invalid()
        {
            short i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y", null));
        }

        public static IEnumerable<object[]> ParseValidData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;
            var emptyFormat = new NumberFormatInfo();

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "-32768", defaultStyle, nullFormat, (short)-32768 };
            yield return new object[] { "-123", defaultStyle, nullFormat, (short)-123 };
            yield return new object[] { "0", defaultStyle, nullFormat, (short)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (short)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (short)123 };
            yield return new object[] { "32767", defaultStyle, nullFormat, (short)32767 };

            yield return new object[] { "123", NumberStyles.HexNumber, nullFormat, (short)0x123 };
            yield return new object[] { "abc", NumberStyles.HexNumber, nullFormat, (short)0xabc };
            yield return new object[] { "1000", NumberStyles.AllowThousands, nullFormat, (short)1000 };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, (short)-123 }; // Parentheses = negative

            yield return new object[] { "123", defaultStyle, emptyFormat, (short)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (short)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (short)0x12 };
            yield return new object[] { "$1,000", NumberStyles.Currency, customFormat, (short)1000 };
        }
        
        [Theory, MemberData("ParseValidData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, short expected)
        {
            short i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(short.TryParse(value, out i));
                Assert.Equal(expected, i);

                Assert.Equal(expected, short.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, short.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(short.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(expected, i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, short.Parse(value, style));
            }
            Assert.Equal(expected, short.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> ParseInvalidData()
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

            yield return new object[] { "-32769", defaultStyle, nullFormat, typeof(OverflowException) }; // < min value
            yield return new object[] { "32768", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
        }

        [Theory, MemberData("ParseInvalidData")]
        public static void TestParseInvalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            short i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(short.TryParse(value, out i));
                Assert.Equal(default(short), i);

                Assert.Throws(exceptionType, () => short.Parse(value));

                //If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => short.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(short.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(default(short), i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => short.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => short.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
    }
}
