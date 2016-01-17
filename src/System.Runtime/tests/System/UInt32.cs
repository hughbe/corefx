// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class UInt32Tests
    {
        [Fact]
        public static void TestCtorEmpty()
        {
            uint i = new uint();
            Assert.Equal((uint)0, i);
        }

        [Fact]
        public static void TestCtorValue()
        {
            uint i = 41;
            Assert.Equal((uint)41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0xFFFFFFFF, uint.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal((uint)0, uint.MinValue);
        }

        [Theory]
        [InlineData((uint)234, 0)]
        [InlineData(uint.MinValue, 1)]
        [InlineData((uint)0, 1)]
        [InlineData((uint)123, 1)]
        [InlineData((uint)456, -1)]
        [InlineData(uint.MaxValue, -1)]
        [InlineData(null, 1)]
        public static void TestCompareTo(object value, int expected)
        {
            uint i = 234;
            if (value is uint)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((uint)value)));
            }

            IComparable iComparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (uint)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a uint
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a uint
        }
        
        [Theory]
        [InlineData((uint)789, (uint)789, true)]
        [InlineData((uint)788, (uint)0, false)]
        [InlineData((uint)0, (uint)0, true)]
        [InlineData((uint)789, null, false)]
        [InlineData((uint)789, "789", false)]
        [InlineData((uint)789, 789, false)]
        public static void TestEquals(uint i1, object obj, bool expected)
        {
            if (obj is uint)
            {
                Assert.Equal(expected, i1.Equals((uint)obj));
                Assert.Equal(expected, i1.GetHashCode().Equals(((uint)obj).GetHashCode()));
                Assert.Equal((int)i1, i1.GetHashCode());
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        [Theory]
        [InlineData((uint)0, "0")]
        [InlineData((uint)4567, "4567")]
        [InlineData(uint.MaxValue, "4294967295")]
        public static void TestToString(uint i, string expected)
        {
            Assert.Equal(expected, i.ToString());
        }

        public static IEnumerable<object[]> ToString_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (uint)0, defaultFormat, "0" };
            yield return new object[] { (uint)6789, defaultFormat, "6789" };
        }

        [Theory, MemberData("ToString_FormatProvider_TestData")]
        public static void TestToString_FormatProvider(uint i, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(provider));
        }

        public static IEnumerable<object[]> ToString_Format_TestData()
        {
            yield return new object[] { (uint)6543, "G", "6543" };
            yield return new object[] { (uint)0x2468, "X", "2468" };
            yield return new object[] { (uint)2468, "N", string.Format("{0:N}", 2468.00) };
        }

        [Theory, MemberData("ToString_Format_TestData")]
        public static void TestToString_Format(uint i, string format, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant()));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant()));
        }

        [Fact]
        public static void TestToString_Format_Invalid()
        {
            uint i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y"));
        }

        public static IEnumerable<object[]> ToString_Format_FormatProvider_TestData()
        {
            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (uint)6543, "G", defaultFormat, "6543" };
            yield return new object[] { (uint)0x2468, "x", defaultFormat, "2468" };
            yield return new object[] { (uint)2468, "N", defaultFormat, string.Format("{0:N}", 2468.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (uint)2468, "N", customFormat, "2*468~00" };
        }

        [Theory, MemberData("ToString_Format_FormatProvider_TestData")]
        public static void TestToString_Format_FormatProvider(uint i, string format, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, i.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected, i.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Format_FormatProvider_Invalid()
        {
            uint i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y", null));
        }

        public static IEnumerable<object[]> Parse_Valid_Data()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;
            var emptyFormat = new NumberFormatInfo();

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "0", defaultStyle, nullFormat, (uint)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (uint)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (uint)123 };
            yield return new object[] { "4294967295", defaultStyle, nullFormat, 4294967295 };

            yield return new object[] { "12", NumberStyles.HexNumber, nullFormat, (uint)0x12 };
            yield return new object[] { "1000", NumberStyles.AllowThousands, nullFormat, (uint)1000 };

            yield return new object[] { "123", defaultStyle, emptyFormat, (uint)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (uint)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (uint)0x12 };
            yield return new object[] { "abc", NumberStyles.HexNumber, emptyFormat, (uint)0xabc };
            yield return new object[] { "$1,000", NumberStyles.Currency, customFormat, (uint)1000 };
        }
        
        [Theory, MemberData("Parse_Valid_Data")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, uint expected)
        {
            uint i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(uint.TryParse(value, out i));
                Assert.Equal(expected, i);

                Assert.Equal(expected, uint.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, uint.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(uint.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(expected, i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, uint.Parse(value, style));
            }
            Assert.Equal(expected, uint.Parse(value, style, provider ?? new NumberFormatInfo()));
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
            yield return new object[] { "4294967296", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, typeof(OverflowException) }; // Parentheses = negative
        }

        [Theory, MemberData("Parse_Invalid_Data")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            uint i;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(uint.TryParse(value, out i));
                Assert.Equal(default(uint), i);

                Assert.Throws(exceptionType, () => uint.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => uint.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(uint.TryParse(value, style, provider ?? new NumberFormatInfo(), out i));
            Assert.Equal(default(uint), i);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => uint.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => uint.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
    }
}
