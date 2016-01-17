// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class ByteTests
    {
        [Fact]
        public static void TestCtorEmpty()
        {
            byte i = new byte();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void TestCtorValue()
        {
            byte i = 41;
            Assert.Equal(41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0xFF, byte.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal(0, byte.MinValue);
        }

        [Theory]
        [InlineData((byte)234, 0)]
        [InlineData(byte.MinValue, 1)]
        [InlineData((byte)0, 1)]
        [InlineData((byte)123, 1)]
        [InlineData((byte)235, -1)]
        [InlineData(byte.MaxValue, -1)]
        [InlineData(null, 1)]
        public static void TestCompareTo(object value, int expected)
        {
            byte i = 234;
            if (value is byte)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((byte)value)));
            }

            IComparable iComparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (byte)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a byte
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a byte
        }

        [Theory]
        [InlineData((byte)78, (byte)78, true)]
        [InlineData((byte)78, (byte)0, false)]
        [InlineData((byte)0, (byte)0, true)]
        [InlineData((byte)78, null, false)]
        [InlineData((byte)78, "78", false)]
        [InlineData((byte)78, 78, false)] 
        public static void TestEquals(byte i1, object obj, bool expected)
        {
            if (obj is byte)
            {
                Assert.Equal(expected, i1.Equals((byte)obj));
                Assert.Equal(expected, i1.GetHashCode().Equals(((byte)obj).GetHashCode()));
                Assert.Equal(i1, i1.GetHashCode());
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        [Theory]
        [InlineData((byte)0, "0")]
        [InlineData((byte)45, "45")]
        [InlineData(byte.MaxValue, "255")]
        public static void TestToString(byte i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        public static IEnumerable<object[]> ToString_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (byte)0, defaultFormat, "0" };
            yield return new object[] { (byte)67, defaultFormat, "67" };
        }

        [Theory, MemberData("ToString_FormatProvider_TestData")]
        public static void TestToString_FormatProvider(byte i, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(provider));
        }

        public static IEnumerable<object[]> ToString_Format_TestData()
        {
            yield return new object[] { (byte)63, "G", "63" };
            yield return new object[] { (byte)0x24, "X", "24" };
            yield return new object[] { (byte)246, "N", string.Format("{0:N}", 246.00) };
        }

        [Theory, MemberData("ToString_Format_TestData")]
        public static void TestToString_Format(byte i, string format, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant()));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant()));
        }

        [Fact]
        public static void TestToString_Format_Invalid()
        {
            byte i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y"));
        }

        public static IEnumerable<object[]> ToString_Format_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (byte)63, "G", defaultFormat, "63" };
            yield return new object[] { (byte)0x24, "x", defaultFormat, "24" };
            yield return new object[] { (byte)246, "N", defaultFormat, string.Format("{0:N}", 246.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NumberDecimalSeparator = "~";
            yield return new object[] { (byte)24, "N", customFormat, "24~00" };
        }

        [Theory, MemberData("ToString_Format_FormatProvider_TestData")]
        public static void TestToString_Format_FormatProvider(byte i, string format, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Format_FormatProvider_Invalid()
        {
            byte i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y", null));
        }

        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;
            var emptyFormat = new NumberFormatInfo();

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "0", defaultStyle, nullFormat, (byte)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (byte)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (byte)123 };
            yield return new object[] { "255", defaultStyle, nullFormat, (byte)255 };

            yield return new object[] { "12", NumberStyles.HexNumber, nullFormat, (byte)0x12 };
            yield return new object[] { "10", NumberStyles.AllowThousands, nullFormat, (byte)10 };

            yield return new object[] { "123", defaultStyle, emptyFormat, (byte)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (byte)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (byte)0x12 };
            yield return new object[] { "ab", NumberStyles.HexNumber, emptyFormat, (byte)0xab };
            yield return new object[] { "$100", NumberStyles.Currency, customFormat, (byte)100 };
        }
        
        [Theory, MemberData("Parse_Valid_TestData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, byte expected)
        {
            byte i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(byte.TryParse(value, out i));
                Assert.Equal(expected, i);

                Assert.Equal(expected, byte.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, byte.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(byte.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(expected, i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, byte.Parse(value, style));
            }
            Assert.Equal(expected, byte.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
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

            yield return new object[] { "ab", defaultStyle, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "1E23", defaultStyle, nullFormat, typeof(FormatException) }; // Exponent
            yield return new object[] { "(123)", defaultStyle, nullFormat, typeof(FormatException) }; // Parentheses
            yield return new object[] { 100.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency
            yield return new object[] { 1000.ToString("N0"), defaultStyle, nullFormat, typeof(FormatException) }; // Thousands
            yield return new object[] { 67.90.ToString("F2"), defaultStyle, nullFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "ab", NumberStyles.None, nullFormat, typeof(FormatException) }; // Negative hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace

            yield return new object[] { "67.90", defaultStyle, customFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "-1", defaultStyle, nullFormat, typeof(OverflowException) }; // < min value
            yield return new object[] { "256", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, typeof(OverflowException) }; // Parentheses = negative
        }

        [Theory, MemberData("Parse_Invalid_TestData")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            byte i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(byte.TryParse(value, out i));
                Assert.Equal(default(byte), i);

                Assert.Throws(exceptionType, () => byte.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => byte.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(byte.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(default(byte), i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => byte.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => byte.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
    }
}
