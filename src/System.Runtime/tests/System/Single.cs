// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class SingleTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            float i = new float();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void TestCtor_Value()
        {
            float i = 41;
            Assert.Equal(41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal((float)3.40282346638528859e+38, float.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal((float)(-3.40282346638528859e+38), float.MinValue);
        }

        [Fact]
        public static void TestEpsilon()
        {
            Assert.Equal((float)1.4e-45, float.Epsilon);
        }
       
        [Theory]
        [InlineData(float.PositiveInfinity, true)]
        [InlineData(float.NegativeInfinity, true)]
        [InlineData(0.0, false)]
        public static void TestIsInfinity(float f, bool expected)
        {
            Assert.Equal(expected, float.IsInfinity(f));
        }

        [Fact]
        public static void TestNaN()
        {
            Assert.Equal((float)0.0 / (float)0.0, float.NaN);
        }

        [Theory]
        [InlineData(float.NaN, true)]
        [InlineData(0.0, false)]
        public static void TestIsNaN(float f, bool expected)
        {
            Assert.Equal(expected, float.IsNaN(f));
        }

        [Theory]
        [InlineData(float.PositiveInfinity, true)]
        [InlineData(float.NegativeInfinity, false)]
        [InlineData(0.0, false)]
        public static void TestIsPositiveInfinity(float f, bool expected)
        {
            Assert.Equal(expected, float.IsPositiveInfinity(f));
        }

        [Fact]
        public static void TestPositiveInfinity()
        {
            Assert.Equal((float)1.0 / (float)0.0, float.PositiveInfinity);
        }

        [Fact]
        public static void TestNegativeInfinity()
        {
            Assert.Equal((float)-1.0 / (float)0.0, float.NegativeInfinity);
        }

        [Theory]
        [InlineData(float.NegativeInfinity, true)]
        [InlineData(float.PositiveInfinity, false)]
        [InlineData(0.0, false)]
        public static void TestIsNegativeInfinity(float f, bool expected)
        {
            Assert.Equal(expected, float.IsNegativeInfinity(f));
        }

        [Theory]
        [InlineData((float)234, (float)234, 0)]
        [InlineData((float)234, float.MinValue, 1)]
        [InlineData((float)234, (float)-123, 1)]
        [InlineData((float)234, (float)0, 1)]
        [InlineData((float)234, (float)123, 1)]
        [InlineData((float)234, (float)456, -1)]
        [InlineData((float)234, float.MaxValue, -1)]
        [InlineData((float)234, float.NaN, 1)]
        [InlineData(float.NaN, float.NaN, 0)]
        [InlineData(float.NaN, (float)0, -1)]
        [InlineData((float)234, null, 1)]
        public static void TestCompareTo(float f, object value, int expected)
        {
            if (value is float)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(f.CompareTo((float)value)));
            }
            IComparable iComparable = f;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (float)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a float
        }
        
        [Theory]
        [InlineData((float)789, true)]
        [InlineData((float)(-789), false)]
        [InlineData((float)0, false)]
        public static void TestEqualsObject(object obj, bool expected)
        {
            float i = 789;
            Assert.Equal(expected, i.Equals(obj));
        }

        [Theory]
        [InlineData((float)789, (float)789, true)]
        [InlineData((float)789, (float)-789, false)]
        [InlineData((float)789, (float)0, false)]
        [InlineData(float.NaN, float.NaN, true)]
        public static void TestEquals(float d1, object value, bool expected)
        {
            if (value is float)
            {
                float d2 = (float)value;
                Assert.Equal(expected, d1.Equals(d2));
                Assert.NotEqual(0, d1.GetHashCode());
                Assert.Equal(expected, d1.GetHashCode().Equals(d2.GetHashCode()));
            }
            Assert.Equal(expected, d1.Equals(value));
        }
        [Theory]
        [InlineData((float)6310, "6310")]
        [InlineData((float)-6310, "-6310")]
        [InlineData(float.NaN, "NaN")]
        [InlineData(float.PositiveInfinity, "∞")]
        [InlineData(float.NegativeInfinity, "-∞")]
        [InlineData(float.Epsilon, "1.401298E-45")]
        public static void TestToString(float d, string expected)
        {
            Assert.Equal(expected, d.ToString());
        }

        public static IEnumerable<object[]> ToString_FormatProvider_TestData()
        {
            NumberFormatInfo nullFormat = null;
            yield return new object[] { (float)6310, nullFormat, "6310" };
            yield return new object[] { (float)-6310, nullFormat, "-6310" };

            var defaultFormat = new NumberFormatInfo();
            yield return new object[] { (float)6310, defaultFormat, "6310" };
            yield return new object[] { (float)-6310, defaultFormat, "-6310" };

            // Changing the negative pattern doesn't do anything without also passing in a format string
            var customFormat = new NumberFormatInfo();
            customFormat.NumberNegativePattern = 0;
            yield return new object[] { (float)-6310, customFormat, "-6310" };

            var invariantFormat = NumberFormatInfo.InvariantInfo;
            yield return new object[] { float.NaN, invariantFormat, "NaN" };
            yield return new object[] { float.PositiveInfinity, invariantFormat, "Infinity" };
            yield return new object[] { float.NegativeInfinity, invariantFormat, "-Infinity" };
        }

        [Theory, MemberData("ToString_FormatProvider_TestData")]
        public static void TestToString_FormatProvider(float d, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, d.ToString(provider));
        }

        public static IEnumerable<object[]> ToString_Format_TestData()
        {
            yield return new object[] { (float)6310, "G", "6310" };
            yield return new object[] { (float)-6310, "G", "-6310" };
            yield return new object[] { (float)-2468, "N", string.Format("{0:N}", -2468.00) };
        }

        [Theory, MemberData("ToString_Format_TestData")]
        public static void TestToString_Format(float d, string format, string expected)
        {
            Assert.Equal(expected, d.ToString(format.ToUpperInvariant()));
            Assert.Equal(expected, d.ToString(format.ToLowerInvariant()));
        }

        [Fact]
        public static void TestToString_Format_Invalid()

        {
            float d = 0.0F;
            Assert.Throws<FormatException>(() => d.ToString("Y"));
        }

        public static IEnumerable<object[]> ToString_Format_FormatProvider_TestData()
        {
            NumberFormatInfo nullFormat = null;
            yield return new object[] { (float)6310, "G", nullFormat, "6310" };
            yield return new object[] { (float)-6310, "G", nullFormat, "-6310" };

            var emptyFormat = new NumberFormatInfo();
            yield return new object[] { (float)6310, "G", emptyFormat, "6310" };
            yield return new object[] { (float)-6310, "G", emptyFormat, "-6310" };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "xx"; // Set to trash to make sure it doesn't show up
            customFormat.NumberGroupSeparator = "*";
            customFormat.NumberNegativePattern = 0;
            yield return new object[] { (float)-2468, "N", customFormat, "(2*468.00)" };
        }

        [Theory, MemberData("ToString_Format_FormatProvider_TestData")]
        public static void TestToStringFormatFormatProvider(float d, string format, IFormatProvider provider, string expected)
        {
            Assert.Equal(expected, d.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected, d.ToString(format.ToLowerInvariant(), provider));
        }

        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            // Defaults: AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign | AllowDecimalPoint | AllowExponent | AllowThousands
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Float;

            var emptyFormat = new NumberFormatInfo();

            var customFormat1 = new NumberFormatInfo();
            customFormat1.CurrencySymbol = "$";
            customFormat1.CurrencyGroupSeparator = ",";

            var customFormat2 = new NumberFormatInfo();
            customFormat2.NumberDecimalSeparator = ".";

            NumberFormatInfo invariantFormat = NumberFormatInfo.InvariantInfo;

            yield return new object[] { "-123", defaultStyle, nullFormat, (float)-123 };
            yield return new object[] { "0", defaultStyle, nullFormat, (float)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (float)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (float)123 };
            yield return new object[] { "567.89", defaultStyle, nullFormat, (float)567.89 };
            yield return new object[] { "-567.89", defaultStyle, nullFormat, (float)-567.89 };
            yield return new object[] { "1E23", defaultStyle, nullFormat, (float)1E23 };

            yield return new object[] { "123.1", NumberStyles.AllowDecimalPoint, nullFormat, (float)123.1 };
            yield return new object[] { 1000.ToString("N0"), NumberStyles.AllowThousands, nullFormat, (float)1000 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (float)123 };
            yield return new object[] { "123.567", NumberStyles.Any, emptyFormat, 123.567 };
            yield return new object[] { "123", NumberStyles.Float, emptyFormat, (float)123 };
            yield return new object[] { "$1000", NumberStyles.Currency, customFormat1, (float)1000 };
            yield return new object[] { "123.123", NumberStyles.Float, customFormat2, (float)123.123 };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, customFormat2, (float)-123 };

            yield return new object[] { "NaN", NumberStyles.Any, invariantFormat, float.NaN };
            yield return new object[] { "Infinity", NumberStyles.Any, invariantFormat, float.PositiveInfinity };
            yield return new object[] { "-Infinity", NumberStyles.Any, invariantFormat, float.NegativeInfinity };
        }

        [Theory, MemberData("Parse_Valid_TestData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, float expected)
        {
            float f;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.True(float.TryParse(value, out f));
                Assert.Equal(expected, f);

                Assert.Equal(expected, float.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, float.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(float.TryParse(value, style, provider ?? new NumberFormatInfo(), out f));
            Assert.Equal(expected, f);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, float.Parse(value, style));
            }
            Assert.Equal(expected, float.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Float;

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";
            customFormat.NumberDecimalSeparator = ".";

            yield return new object[] { null, defaultStyle, nullFormat, typeof(ArgumentNullException) };
            yield return new object[] { "", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { " ", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { "Garbage", defaultStyle, nullFormat, typeof(FormatException) };

            yield return new object[] { "ab", defaultStyle, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "(123)", defaultStyle, nullFormat, typeof(FormatException) }; // Parentheses
            yield return new object[] { 100.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency

            yield return new object[] { "123.456", NumberStyles.Integer, nullFormat, typeof(FormatException) }; // Decimal
            yield return new object[] { "  123.456", NumberStyles.None, nullFormat, typeof(FormatException) }; // Leading space
            yield return new object[] { "123.456   ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Leading space
            yield return new object[] { "1E23", NumberStyles.None, nullFormat, typeof(FormatException) }; // Exponent

            yield return new object[] { "ab", NumberStyles.None, nullFormat, typeof(FormatException) }; // Negative hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace
        }

        [Theory, MemberData("Parse_Invalid_TestData")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            float d;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.False(float.TryParse(value, out d));
                Assert.Equal(default(float), d);

                Assert.Throws(exceptionType, () => float.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => float.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(float.TryParse(value, style, provider ?? new NumberFormatInfo(), out d));
            Assert.Equal(default(float), d);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => float.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => float.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
    }
}
