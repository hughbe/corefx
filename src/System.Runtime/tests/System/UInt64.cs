// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class UInt64Tests
    {
        [Fact]
        public static void TestCtorEmpty()
        {
            ulong i = new ulong();
            Assert.Equal((ulong)0, i);
        }

        [Fact]
        public static void TestCtorValue()
        {
            ulong i = 41;
            Assert.Equal((ulong)41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0xFFFFFFFFFFFFFFFF, ulong.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal((ulong)0, ulong.MinValue);
        }

        [Theory]
        [InlineData((ulong)234, 0)]
        [InlineData(ulong.MinValue, 1)]
        [InlineData((ulong)0, 1)]
        [InlineData((ulong)123, 1)]
        [InlineData((ulong)456, -1)]
        [InlineData(ulong.MaxValue, -1)]
        [InlineData(null, 1)]
        public static void TestCompareTo(object value, int expected)
        {
            ulong i = 234;
            if (value is ulong)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((ulong)value)));
            }

            IComparable iComparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (ulong)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a ulong
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a ulong
        }

        [Theory]
        [InlineData((ulong)789, (ulong)789, true)]
        [InlineData((ulong)788, (ulong)0, false)]
        [InlineData((ulong)0, (ulong)0, true)]
        [InlineData((ulong)789, null, false)]
        [InlineData((ulong)789, "789", false)]
        [InlineData((ulong)789, 789, false)]
        public static void TestEquals(ulong i1, object obj, bool expected)
        {
            if (obj is ulong)
            {
                Assert.Equal(expected, i1.Equals((ulong)obj));
                Assert.Equal(expected, i1.GetHashCode().Equals(((ulong)obj).GetHashCode()));
                Assert.Equal((int)i1, i1.GetHashCode());
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        [Theory]
        [InlineData((ulong)0, "0")]
        [InlineData((ulong)4567, "4567")]
        [InlineData(ulong.MaxValue, "18446744073709551615")]
        public static void TestToString(ulong i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        public static IEnumerable<object[]> ToString_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (ulong)0, defaultFormat, "0" };
            yield return new object[] { (ulong)6789, defaultFormat, "6789" };
        }

        [Theory, MemberData("ToString_FormatProvider_TestData")]
        public static void TestToString_FormatProvider(ulong i, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(provider));
        }

        public static IEnumerable<object[]> ToString_Format_TestData()
        {
            yield return new object[] { (ulong)6543, "G", "6543" };
            yield return new object[] { (ulong)0x2468, "X", "2468" };
            yield return new object[] { (ulong)2468, "N", string.Format("{0:N}", 2468.00) };
        }

        [Theory, MemberData("ToString_Format_TestData")]
        public static void TestToString_Format(ulong i, string format, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant()));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant()));
        }

        [Fact]
        public static void TestToString_Format_Invalid()
        {
            ulong i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y"));
        }

        public static IEnumerable<object[]> ToString_Format_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (ulong)6543, "G", defaultFormat, "6543" };
            yield return new object[] { (ulong)0x2468, "x", defaultFormat, "2468" };
            yield return new object[] { (ulong)2468, "N", defaultFormat, string.Format("{0:N}", 2468.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (ulong)2468, "N", customFormat, "2*468~00" };
        }

        [Theory, MemberData("ToString_Format_FormatProvider_TestData")]
        public static void TestToString_Format_FormatProvider(ulong i, string format, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Format_FormatProvider_Invalid()
        {
            ulong i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y", null));
        }

        public static IEnumerable<object[]> Parse_Valid_Data()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;
            var emptyFormat = new NumberFormatInfo();

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "0", defaultStyle, nullFormat, (ulong)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (ulong)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (ulong)123 };
            yield return new object[] { "18446744073709551615", defaultStyle, nullFormat, (ulong)18446744073709551615 };

            yield return new object[] { "12", NumberStyles.HexNumber, nullFormat, (ulong)0x12 };
            yield return new object[] { "1000", NumberStyles.AllowThousands, nullFormat, (ulong)1000 };

            yield return new object[] { "123", defaultStyle, emptyFormat, (ulong)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (ulong)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (ulong)0x12 };
            yield return new object[] { "abc", NumberStyles.HexNumber, emptyFormat, (ulong)0xabc };
            yield return new object[] { "$1,000", NumberStyles.Currency, customFormat, (ulong)1000 };
        }
        
        [Theory, MemberData("Parse_Valid_Data")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, ulong expected)
        {
            ulong i;
            //If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(ulong.TryParse(value, out i));
                Assert.Equal(expected, i);

                Assert.Equal(expected, ulong.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, ulong.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(ulong.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(expected, i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, ulong.Parse(value, style));
            }
            Assert.Equal(expected, ulong.Parse(value, style, provider ?? new NumberFormatInfo()));
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
            yield return new object[] { 100.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency
            yield return new object[] { 1000.ToString("N0"), defaultStyle, nullFormat, typeof(FormatException) }; // Thousands
            yield return new object[] { 678.90.ToString("F2"), defaultStyle, nullFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "abc", NumberStyles.None, nullFormat, typeof(FormatException) }; // Negative hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace

            yield return new object[] { "678.90", defaultStyle, customFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "-1", defaultStyle, nullFormat, typeof(OverflowException) }; // < min value
            yield return new object[] { "18446744073709551616", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, typeof(OverflowException) }; // Parentheses = negative
        }

        [Theory, MemberData("Parse_Invalid_Data")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            ulong i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(ulong.TryParse(value, out i));
                Assert.Equal(default(ulong), i);

                Assert.Throws(exceptionType, () => ulong.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => ulong.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(ulong.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(default(ulong), i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => ulong.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => ulong.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
    }
}
