// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class SByteTests
    {
        [Fact]
        public static void TestCtorEmpty()
        {
            sbyte i = new sbyte();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void TestCtorValue()
        {
            sbyte i = 41;
            Assert.Equal(41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0x7F, sbyte.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal(-0x80, sbyte.MinValue);
        }

        [Theory]
        [InlineData((sbyte)114, 0)]
        [InlineData(sbyte.MinValue, 1)]
        [InlineData((sbyte)-123, 1)]
        [InlineData((sbyte)0, 1)]
        [InlineData((sbyte)123, -1)]
        [InlineData(sbyte.MaxValue, -1)]
        [InlineData(null, 1)]
        public static void TestCompareTo(object value, int expected)
        {
            sbyte i = 114;
            if (value is sbyte)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((sbyte)value)));
            }

            IComparable iComparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (sbyte)114;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a sbyte
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a sbyte
        }

        [Theory]
        [InlineData((sbyte)78, (sbyte)78, true)]
        [InlineData((sbyte)78, (sbyte)-78, false)]
        [InlineData((sbyte)78, (sbyte)0, false)]
        [InlineData((sbyte)0, (sbyte)0, true)]
        [InlineData((sbyte)78, null, false)]
        [InlineData((sbyte)78, "78", false)]
        [InlineData((sbyte)78, 78, false)]
        public static void TestEquals(sbyte i1, object obj, bool expected)
        {
            if (obj is sbyte)
            {
                Assert.Equal(expected, i1.Equals((sbyte)obj));
                Assert.Equal(expected, i1.GetHashCode().Equals(((sbyte)obj).GetHashCode()));
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        [Theory]
        [InlineData(sbyte.MinValue, "-128")]
        [InlineData((sbyte)-45, "-45")]
        [InlineData((sbyte)0, "0")]
        [InlineData((sbyte)45, "45")]
        [InlineData(sbyte.MaxValue, "127")]
        public static void TestToString(sbyte i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        public static IEnumerable<object[]> ToString_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (sbyte)-67, defaultFormat, "-67" };
            yield return new object[] { (sbyte)0, defaultFormat, "0" };
            yield return new object[] { (sbyte)67, defaultFormat, "67" };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (sbyte)-67, customFormat, "#67" };
            yield return new object[] { (sbyte)67, customFormat, "67" };
        }

        [Theory, MemberData("ToString_FormatProvider_TestData")]
        public static void TestToString_FormatProvider(sbyte i, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(provider));
        }

        public static IEnumerable<object[]> ToString_Format_TestData()
        {
            yield return new object[] { (sbyte)-63, "G", "-63" };
            yield return new object[] { (sbyte)63, "G", "63" };
            yield return new object[] { (sbyte)0x24, "X", "24" };
            yield return new object[] { (sbyte)24, "N", string.Format("{0:N}", 24.00) };
        }

        [Theory, MemberData("ToString_Format_TestData")]
        public static void TestToString_Format(sbyte i, string format, string expected)
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
            yield return new object[] { (sbyte)-63, "G", defaultFormat, "-63" };
            yield return new object[] { (sbyte)63, "G", defaultFormat, "63" };
            yield return new object[] { (sbyte)0x24, "x", defaultFormat, "24" };
            yield return new object[] { (sbyte)24, "N", defaultFormat, string.Format("{0:N}", 24.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (sbyte)-24, "N", customFormat, "#24~00" };
            yield return new object[] { (sbyte)24, "N", customFormat, "24~00" };
        }

        [Theory, MemberData("ToString_Format_FormatProvider_TestData")]
        public static void TestToString_Format_FormatProvider(sbyte i, string format, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Format_FormatProvider_Invalid()
        {
            sbyte i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y", null));
        }

        public static IEnumerable<object[]> Parse_Valid_Data()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;
            var emptyFormat = new NumberFormatInfo();

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "-123", defaultStyle, nullFormat, (sbyte)-123 };
            yield return new object[] { "0", defaultStyle, nullFormat, (sbyte)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (sbyte)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (sbyte)123 };
            yield return new object[] { "127", defaultStyle, nullFormat, (sbyte)127 };

            yield return new object[] { "12", NumberStyles.HexNumber, nullFormat, (sbyte)0x12 };
            yield return new object[] { "10", NumberStyles.AllowThousands, nullFormat, (sbyte)10 };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, (sbyte)-123 }; // Parentheses = negative

            yield return new object[] { "123", defaultStyle, emptyFormat, (sbyte)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (sbyte)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (sbyte)0x12 };
            yield return new object[] { "$100", NumberStyles.Currency, customFormat, (sbyte)100 };
        }
        
        [Theory, MemberData("Parse_Valid_Data")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, sbyte expected)
        {
            sbyte i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(sbyte.TryParse(value, out i));
                Assert.Equal(expected, i);

                Assert.Equal(expected, sbyte.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, sbyte.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(sbyte.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(expected, i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, sbyte.Parse(value, style));
            }
            Assert.Equal(expected, sbyte.Parse(value, style, provider ?? new NumberFormatInfo()));
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

            yield return new object[] { "ab", defaultStyle, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "1E23", defaultStyle, nullFormat, typeof(FormatException) }; // Exponent
            yield return new object[] { "(123)", defaultStyle, nullFormat, typeof(FormatException) }; // Parentheses
            yield return new object[] { 100.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency
            yield return new object[] { 1000.ToString("N0"), defaultStyle, nullFormat, typeof(FormatException) }; // Thousands
            yield return new object[] { 67.90.ToString("F2"), defaultStyle, nullFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "ab", NumberStyles.None, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace

            yield return new object[] { "67.90", defaultStyle, customFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "-129", defaultStyle, nullFormat, typeof(OverflowException) }; // < min value
            yield return new object[] { "128", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
        }

        [Theory, MemberData("Parse_Invalid_Data")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            sbyte i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(sbyte.TryParse(value, out i));
                Assert.Equal(default(sbyte), i);

                Assert.Throws(exceptionType, () => sbyte.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => sbyte.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(sbyte.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(default(sbyte), i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => sbyte.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => sbyte.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
    }
}
