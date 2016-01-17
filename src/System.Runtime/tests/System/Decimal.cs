// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class DecimalTests
    {
        [Fact]
        public static void TestCtor_Float()
        {
            Assert.Equal((decimal)((float)123456789.123456), new decimal((float)123456789.123456));
        }

        [Fact]
        public static void TestCtor_Double()
        {
            Assert.Equal((decimal)123456789.123456, new decimal(123456789.123456));
        }

        [Fact]
        public static void TestCtor_Int()
        {
            Assert.Equal(int.MaxValue, new decimal(int.MaxValue));
        }

        [Fact]
        public static void TestCtor_Long()
        {
            Assert.Equal(long.MaxValue, new decimal(long.MaxValue));
        }

        [Fact]
        public static void TestCtor_UInt()
        {
            Assert.Equal(uint.MaxValue, new decimal(uint.MaxValue));
        }

        [Fact]
        public static void TestCtor_ULong()
        {
            Assert.Equal(ulong.MaxValue, new decimal(ulong.MaxValue));
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Bool_Byte()
        {
            var d1 = new decimal(1, 1, 1, false, 0);
            decimal d2 = 3;
            d2 += uint.MaxValue;
            d2 += ulong.MaxValue;
            Assert.Equal(d2, d1);
        }

        [Fact]
        public static void TestCtor_IntArray()
        {
            var d1 = new decimal(new int[] { 1, 1, 1, 0 });
            decimal d2 = 3;
            d2 += uint.MaxValue;
            d2 += ulong.MaxValue;
            Assert.Equal(d2, d1);
        }

        [Fact]
        public static void TestCtor_IntArray_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => new decimal(null)); // Bits is null
            Assert.Throws<ArgumentException>(() => new decimal(new int[3])); // Bits.Length is not 4
            Assert.Throws<ArgumentException>(() => new decimal(new int[5])); // Bits.Length is not 4
        }

        public static IEnumerable<object[]> EqualsTestData()
        {
            yield return new object[] { decimal.Zero, decimal.Zero, true };
            yield return new object[] { decimal.Zero, decimal.One, false };
            yield return new object[] { decimal.MaxValue, decimal.MaxValue, true };
            yield return new object[] { decimal.MinValue, decimal.MinValue, true };
            yield return new object[] { decimal.MaxValue, decimal.MinValue, false };

            yield return new object[] { decimal.One, null, false };
            yield return new object[] { decimal.One, 1, false };
            yield return new object[] { decimal.One, "one", false };
        }

        [Theory, MemberData("EqualsTestData")]
        public static void TestEquals(object obj1, object obj2, bool expected)
        {
            if (obj1 is decimal)
            {
                decimal d1 = (decimal)obj1;
                if (obj2 is decimal)
                {
                    decimal d2 = (decimal)obj2;
                    Assert.Equal(expected, d1.Equals(d2));
                    Assert.Equal(expected, decimal.Equals(d1, d2));
                    Assert.Equal(expected, d1 == d2);
                    Assert.Equal(!expected, d1 != d2);
                    Assert.Equal(expected, d1.GetHashCode().Equals(d2.GetHashCode()));

                    Assert.True(d1.Equals(d1));
                    Assert.True(d1 == d1);
                    Assert.False(d1 != d1);
                    Assert.Equal(d1.GetHashCode(), d1.GetHashCode());
                }
                Assert.Equal(expected, d1.Equals(obj2));
            }
            Assert.Equal(expected, Equals(obj1, obj2));
        }

        public static IEnumerable<object[]> CompareTestData()
        {
            yield return new object[] { 5m, 15m, -1 };
            yield return new object[] { 15m, 15m, 0 };
            yield return new object[] { 15m, 5m, 1 };

            yield return new object[] { decimal.Zero, decimal.Zero, 0 };
            yield return new object[] { decimal.Zero, decimal.One, -1 };
            yield return new object[] { decimal.One, decimal.Zero, 1 };
            yield return new object[] { decimal.MaxValue, decimal.MaxValue, 0 };
            yield return new object[] { decimal.MinValue, decimal.MinValue, 0 };
            yield return new object[] { decimal.MaxValue, decimal.MinValue, 1 };
            yield return new object[] { decimal.MinValue, decimal.MaxValue, -1 };
        }

        [Theory, MemberData("CompareTestData")]
        public static void TestCompare(decimal d1, object obj, int expected)
        {
            if (obj is decimal)
            {
                decimal d2 = (decimal)obj;
                Assert.Equal(expected, CompareHelper.NormalizeCompare(d1.CompareTo(d2)));
                if (expected >= 0)
                {
                    Assert.True(d1 >= d2);
                    Assert.False(d1 < d2);
                }
                if (expected > 0)
                {
                    Assert.True(d1 > d2);
                    Assert.False(d1 <= d2);
                }
                if (expected <= 0)
                {
                    Assert.True(d1 <= d2);
                    Assert.False(d1 > d2);
                }
                if (expected < 0)
                {
                    Assert.True(d1 < d2);
                    Assert.False(d1 >= d2);
                }
                Assert.Equal(expected, CompareHelper.NormalizeCompare(decimal.Compare(d1, d2)));
            }
            IComparable iComparable = d1;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(obj)));
        }

        [Fact]
        public static void TestCompare_Invalid()
        {
            IComparable iComparable = 248m;
            Assert.Throws<ArgumentException>(() => iComparable.CompareTo("248"));
            Assert.Throws<ArgumentException>(() => iComparable.CompareTo(248));
        }

        public static IEnumerable<object[]> Add_Valid_TestData()
        {
            yield return new object[] { 1m, 1m, 2m };
            yield return new object[] { -1m, 1m, 0m };
            yield return new object[] { 1m, -1m, 0m };
            yield return new object[] { 1m, 0, 1m };
            yield return new object[] { 79228162514264337593543950330m, 5m, decimal.MaxValue };
            yield return new object[] { 79228162514264337593543950335m, -5m, 79228162514264337593543950330m };
            yield return new object[] { -79228162514264337593543950330m, 5m, -79228162514264337593543950325m };
            yield return new object[] { -79228162514264337593543950330m, -5m, decimal.MinValue };
            yield return new object[] { 1234.5678m, 0.00009m, 1234.56789m };
            yield return new object[] { -1234.5678m, 0.00009m, -1234.56771m };
            yield return new object[] { 0.1111111111111111111111111111m, 0.1111111111111111111111111111m, 0.2222222222222222222222222222m };
            yield return new object[] { 0.5555555555555555555555555555m, 0.5555555555555555555555555555m, 1.1111111111111111111111111110m };

            yield return new object[] { decimal.MinValue, decimal.Zero, decimal.MinValue };
            yield return new object[] { decimal.MaxValue, decimal.Zero, decimal.MaxValue };
            yield return new object[] { decimal.MinValue, decimal.Zero, decimal.MinValue };
        }

        [Theory, MemberData("Add_Valid_TestData")]
        public static void TestAdd(decimal d1, decimal d2, decimal expected)
        {
            Assert.Equal(expected, d1 + d2);
            Assert.Equal(expected, decimal.Add(d1, d2));
        }

        public static IEnumerable<object[]> Add_Invalid_TestData()
        {
            yield return new object[] { decimal.MaxValue, decimal.MaxValue };
            yield return new object[] { 79228162514264337593543950330m, 6 };
            yield return new object[] { -79228162514264337593543950330m, -6 };
        }

        [Theory, MemberData("Add_Invalid_TestData")]
        public static void TestAdd_Invalid(decimal d1, decimal d2)
        {
            Assert.Throws<OverflowException>(() => d1 + d2);
            Assert.Throws<OverflowException>(() => decimal.Add(d1, d2));
        }

        public static IEnumerable<object[]> CeilingTestData()
        {
            yield return new object[] { 123m, 123m };
            yield return new object[] { 123.123m, 124m };
            yield return new object[] { 123.456m, 124m };
            yield return new object[] { -123.123m, -123m };
            yield return new object[] { -123.456m, -123m };
        }

        [Theory, MemberData("CeilingTestData")]
        public static void TestCeiling(decimal d, decimal expected)
        {
            Assert.Equal(expected, decimal.Ceiling(d));
        }

        public static IEnumerable<object[]> Divide_Valid_TestData()
        {
            yield return new object[] { decimal.One, decimal.One, decimal.One };
            yield return new object[] { decimal.MinusOne, decimal.MinusOne, decimal.One };
            yield return new object[] { 15m, 2m, 7.5m };
            yield return new object[] { 10m, 2m, 5m };
            yield return new object[] { -10m, -2m, 5m };
            yield return new object[] { 10m, -2m, -5m };
            yield return new object[] { -10m, 2m, -5m };
            yield return new object[] { 0.9214206543486529434634231456m, decimal.MaxValue, decimal.Zero };
            yield return new object[] { 38214206543486529434634231456m, 0.49214206543486529434634231456m, 77648730371625094566866001277m };
            yield return new object[] { -78228162514264337593543950335m, decimal.MaxValue, -0.987378225516463811113412343m };

            yield return new object[] { decimal.MaxValue, decimal.MinusOne, decimal.MinValue };
            yield return new object[] { decimal.MinValue, decimal.MaxValue, decimal.MinusOne };
            yield return new object[] { decimal.MaxValue, decimal.MaxValue, decimal.One };
            yield return new object[] { decimal.MinValue, decimal.MinValue, decimal.One };

            // Tests near MaxValue
            yield return new object[] { 792281625142643375935439503.4m, 0.1m, 7922816251426433759354395034m };
            yield return new object[] { 79228162514264337593543950.34m, 0.1m, 792281625142643375935439503.4m };
            yield return new object[] { 7922816251426433759354395.034m, 0.1m, 79228162514264337593543950.34m };
            yield return new object[] { 792281625142643375935439.5034m, 0.1m, 7922816251426433759354395.034m };
            yield return new object[] { 79228162514264337593543950335m, 10m, 7922816251426433759354395033.5m };
            yield return new object[] { 79228162514264337567774146561m, 10m, 7922816251426433756777414656.1m };
            yield return new object[] { 79228162514264337567774146560m, 10m, 7922816251426433756777414656m };
            yield return new object[] { 79228162514264337567774146559m, 10m, 7922816251426433756777414655.9m };
            yield return new object[] { 79228162514264337593543950335m, 1.1m, 72025602285694852357767227577m };
            yield return new object[] { 79228162514264337593543950335m, 1.01m, 78443725261647859003508861718m };
            yield return new object[] { 79228162514264337593543950335m, 1.001m, 79149013500763574019524425909.091m };
            yield return new object[] { 79228162514264337593543950335m, 1.0001m, 79220240490215316061937756559.344m };
            yield return new object[] { 79228162514264337593543950335m, 1.00001m, 79227370240561931974224208092.919m };
            yield return new object[] { 79228162514264337593543950335m, 1.000001m, 79228083286181051412492537842.462m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000001m, 79228154591448878448656105469.389m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000001m, 79228161721982720373716746597.833m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000001m, 79228162435036175158507775176.492m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000001m, 79228162506341521342909798200.709m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000001m, 79228162513472055968409229775.316m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000001m, 79228162514185109431029765225.569m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000001m, 79228162514256414777292524693.522m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000001m, 79228162514263545311918807699.547m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000001m, 79228162514264258365381436070.742m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000001m, 79228162514264329670727698908.567m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000001m, 79228162514264336801262325192.357m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000001m, 79228162514264337514315787820.736m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000000001m, 79228162514264337585621134083.574m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000000001m, 79228162514264337592751668709.857m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000000001m, 79228162514264337593464722172.486m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000000000001m, 79228162514264337593536027518.749m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000000000001m, 79228162514264337593543158053.375m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000000000001m, 79228162514264337593543871106.837m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000000000000001m, 79228162514264337593543942412.184m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000000000000001m, 79228162514264337593543949542.718m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000000000000001m, 79228162514264337593543950255.772m };
            yield return new object[] { 7922816251426433759354395033.5m, 0.9999999999999999999999999999m, 7922816251426433759354395034m };
            yield return new object[] { 79228162514264337593543950335m, 10000000m, 7922816251426433759354.3950335m };
            yield return new object[] { 7922816251426433759354395033.5m, 1.000001m, 7922808328618105141249253784.2m };
            yield return new object[] { 7922816251426433759354395033.5m, 1.0000000000000000000000000001m, 7922816251426433759354395032.7m };
            yield return new object[] { 7922816251426433759354395033.5m, 1.0000000000000000000000000002m, 7922816251426433759354395031.9m };
            yield return new object[] { 7922816251426433759354395033.5m, 0.9999999999999999999999999999m, 7922816251426433759354395034m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000000000000000001m, 79228162514264337593543950327m };

            decimal boundary7 = new decimal((int)429u, (int)2133437386u, 0, false, 0);
            decimal boundary71 = new decimal((int)429u, (int)2133437387u, 0, false, 0);
            decimal maxValueBy7 = decimal.MaxValue * 0.0000001m;
            yield return new object[] { maxValueBy7, 1m, maxValueBy7 };
            yield return new object[] { maxValueBy7, 1m, maxValueBy7 };
            yield return new object[] { maxValueBy7, 0.0000001m, decimal.MaxValue };
            yield return new object[] { boundary7, 1m, boundary7 };
            yield return new object[] { boundary7, 0.000000100000000000000000001m, 91630438009337286849083695.62m };
            yield return new object[] { boundary71, 0.000000100000000000000000001m, 91630438052286959809083695.62m };
            yield return new object[] { 7922816251426433759354.3950335m, 1m, 7922816251426433759354.3950335m };
            yield return new object[] { 7922816251426433759354.3950335m, 0.0000001m, 79228162514264337593543950335m };
        }

        [Theory, MemberData("Divide_Valid_TestData")]
        public static void TestDivide(decimal d1, decimal d2, decimal expected)
        {
            Assert.Equal(expected, d1 / d2);
            Assert.Equal(expected, decimal.Divide(d1, d2));
        }

        public static IEnumerable<object[]> Divide_Invalid_TestData()
        {
            yield return new object[] { decimal.One, decimal.Zero, typeof(DivideByZeroException) };
            yield return new object[] { decimal.Zero, decimal.Zero, typeof(DivideByZeroException) };
            yield return new object[] { 0.0m, 0.0m, typeof(DivideByZeroException) };

            yield return new object[] { 79228162514264337593543950335m, -0.9999999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 792281625142643.37593543950335m, -0.0000000000000079228162514264337593543950335m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.1m, typeof(OverflowException) };
            yield return new object[] { 7922816251426433759354395034m, 0.1m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.99999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.999999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.999999999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, -0.1m, typeof(OverflowException) };
            yield return new object[] { 79228162514264337593543950335m, -0.9999999999999999999999999m, typeof(OverflowException) };
            yield return new object[] { decimal.MaxValue / 2m, 0.5m, typeof(OverflowException) };
        }

        [Theory, MemberData("Divide_Invalid_TestData")]
        public static void TestDivide_Invalid(decimal d1, decimal d2, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => d1 / d2);
            Assert.Throws(exceptionType, () => decimal.Divide(d1, d2));
        }

        public static IEnumerable<object[]> FloorTestData()
        {
            yield return new object[] { 123m, 123m };
            yield return new object[] { 123.123m, 123m };
            yield return new object[] { 123.456m, 123m };
            yield return new object[] { -123.123m, -124m };
            yield return new object[] { -123.456m, -124m };
        }

        [Theory, MemberData("FloorTestData")]
        public static void TestFloor(decimal d, decimal expected)
        {
            Assert.Equal(expected, decimal.Floor(d));
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(79228162514264337593543950335m, decimal.MaxValue);
        }

        [Fact]
        public static void TestMinusOne()
        {
            Assert.Equal(-1, decimal.MinusOne);
        }

        [Fact]
        public static void TestZero()
        {
            Assert.Equal(0, decimal.Zero);
        }

        [Fact]
        public static void TestOne()
        {
            Assert.Equal(1, decimal.One);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal(-79228162514264337593543950335m, decimal.MinValue);
        }

        public static IEnumerable<object[]> Multiply_Valid_TestData()
        {
            yield return new object[] { decimal.One, decimal.One, decimal.One };
            yield return new object[] { 7922816251426433759354395033.5m, new decimal(10), decimal.MaxValue };
            yield return new object[] { 0.2352523523423422342354395033m, 56033525474612414574574757495m, 13182018677937129120135020796m };
            yield return new object[] { 46161363632634613634.093453337m, 461613636.32634613634083453337m, 21308714924243214928823669051m };
            yield return new object[] { 0.0000000000000345435353453563m, .0000000000000023525235234234m, 0.0000000000000000000000000001m };

            // Near decimal.MaxValue
            yield return new object[] { 79228162514264337593543950335m, 0.9m, 71305346262837903834189555302m };
            yield return new object[] { 79228162514264337593543950335m, 0.99m, 78435880889121694217608510832m };
            yield return new object[] { 79228162514264337593543950335m, 0.9999999999999999999999999999m, 79228162514264337593543950327m };
            yield return new object[] { -79228162514264337593543950335m, 0.9m, -71305346262837903834189555302m };
            yield return new object[] { -79228162514264337593543950335m, 0.99m, -78435880889121694217608510832m };
            yield return new object[] { -79228162514264337593543950335m, 0.9999999999999999999999999999m, -79228162514264337593543950327m };
        }

        [Theory, MemberData("Multiply_Valid_TestData")]
        public static void TestMultiply(decimal d1, decimal d2, decimal expected)
        {
            Assert.Equal(expected, d1 * d2);
            Assert.Equal(expected, decimal.Multiply(d1, d2));
        }

        public static IEnumerable<object[]> Multiply_Invalid_TestData()
        {
            yield return new object[] { decimal.MaxValue, decimal.MinValue };
            yield return new object[] { decimal.MinValue, 1.1m };
            yield return new object[] { 79228162514264337593543950335m, 1.1m };
            yield return new object[] { 79228162514264337593543950335m, 1.01m };
            yield return new object[] { 79228162514264337593543950335m, 1.001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.0000000000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.00000000000000000000000001m };
            yield return new object[] { 79228162514264337593543950335m, 1.000000000000000000000000001m };
            yield return new object[] { decimal.MaxValue / 2, 2m };
        }

        [Theory, MemberData("Multiply_Invalid_TestData")]
        public static void TestMultiply_Invalid(decimal d1, decimal d2)
        {
            Assert.Throws<OverflowException>(() => d1 * d2);
            Assert.Throws<OverflowException>(() => decimal.Multiply(d1, d2));
        }

        public static IEnumerable<object[]> NegateTestData()
        {
            yield return new object[] { 1m, -1m };
            yield return new object[] { 0m, 0m };
            yield return new object[] { -1m, 1m };
            yield return new object[] { decimal.MaxValue, decimal.MinValue };
            yield return new object[] { decimal.MinValue, decimal.MaxValue };
        }

        [Theory, MemberData("NegateTestData")]
        public static void TestNegate(decimal d, decimal expected)
        {
            Assert.Equal(expected, decimal.Negate(d));
        }

        public static IEnumerable<object[]> DecrementTestData()
        {
            yield return new object[] { 1m, 0m };
            yield return new object[] { 0m, -1m };
            yield return new object[] { -1m, -2m };
            yield return new object[] { 12345m, 12344m };
            yield return new object[] { 12345.678m, 12344.678m };
            yield return new object[] { -12345.678m, -12346.678m };
        }

        [Theory, MemberData("DecrementTestData")]
        public static void TestDecrementOperator(decimal d, decimal expected)
        {
            Assert.Equal(expected, --d);
        }

        public static IEnumerable<object[]> IncrementTestData()
        {
            yield return new object[] { 1m, 2m };
            yield return new object[] { 0m, 1m };
            yield return new object[] { -1m, -0m };
            yield return new object[] { 12345m, 12346m };
            yield return new object[] { 12345.678m, 12346.678m };
            yield return new object[] { -12345.678m, -12344.678m };
        }

        [Theory, MemberData("IncrementTestData")]
        public static void TestIncrementOperator(decimal d, decimal expected)
        {
            Assert.Equal(expected, ++d);
        }

        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Float;

            var defaultFormat = new NumberFormatInfo();

            var customFormat1 = new NumberFormatInfo();
            customFormat1.CurrencySymbol = "$";
            customFormat1.CurrencyGroupSeparator = ",";

            var customFormat2 = new NumberFormatInfo();
            customFormat2.NumberDecimalSeparator = ".";

            var customFormat3 = new NumberFormatInfo();
            customFormat3.NumberGroupSeparator = ",";

            var customFormat4 = new NumberFormatInfo();
            customFormat4.NumberDecimalSeparator = ".";

            yield return new object[] { "-123", defaultStyle, nullFormat, -123m };
            yield return new object[] { "0", defaultStyle, nullFormat, 0m };
            yield return new object[] { "123", defaultStyle, nullFormat, 123m };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, 123m };
            yield return new object[] { "567.89", defaultStyle, nullFormat, 567.89m };
            yield return new object[] { "-567.89", defaultStyle, nullFormat, -567.89m };

            yield return new object[] { "79228162514264337593543950335", defaultStyle, nullFormat, 79228162514264337593543950335m };
            yield return new object[] { "-79228162514264337593543950335", defaultStyle, nullFormat, -79228162514264337593543950335m };
            yield return new object[] { "79,228,162,514,264,337,593,543,950,335", NumberStyles.AllowThousands, customFormat3, 79228162514264337593543950335m };

            yield return new object[] { "123.1", NumberStyles.AllowDecimalPoint, nullFormat, 123.1m };
            yield return new object[] { 1000.ToString("N0"), NumberStyles.AllowThousands, nullFormat, 1000m };
            
            yield return new object[] { "123", NumberStyles.Any, defaultFormat, 123m };
            yield return new object[] { "123.567", NumberStyles.Any, defaultFormat, 123.567m };
            yield return new object[] { "123", NumberStyles.Float, defaultFormat, 123m };
            yield return new object[] { "$1000", NumberStyles.Currency, customFormat1, 1000m };
            yield return new object[] { "123.123", NumberStyles.Float, customFormat2, 123.123m };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, customFormat2, -123m };

            // Number buffer limit ran out (string too long)
            yield return new object[] { "1234567890123456789012345.678456", defaultStyle, defaultFormat, 1234567890123456789012345.6785m };
        }

        [Theory, MemberData("Parse_Valid_TestData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, decimal expected)
        {
            decimal d;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.True(decimal.TryParse(value, out d));
                Assert.Equal(expected, d);

                Assert.Equal(expected, decimal.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, decimal.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(decimal.TryParse(value, style, provider ?? new NumberFormatInfo(), out d));
            Assert.Equal(expected, d);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, decimal.Parse(value, style));
            }
            Assert.Equal(expected, decimal.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Float;

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";
            customFormat.NumberDecimalSeparator = ".";

            yield return new object[] { null, defaultStyle, nullFormat, typeof(ArgumentNullException) };
            yield return new object[] { "79228162514264337593543950336", defaultStyle, nullFormat, typeof(OverflowException) };
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
            decimal d;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.False(decimal.TryParse(value, out d));
                Assert.Equal(default(decimal), d);

                Assert.Throws(exceptionType, () => decimal.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => decimal.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(decimal.TryParse(value, style, provider ?? new NumberFormatInfo(), out d));
            Assert.Equal(default(decimal), d);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => decimal.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => decimal.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
        
        public static IEnumerable<object[]> Remainder_Valid_TestData()
        {
            decimal NegativeZero = new decimal(0, 0, 0, true, 0);
            yield return new object[] { 5m, 3m, 2m };
            yield return new object[] { 5m, -3m, 2m };
            yield return new object[] { -5m, 3m, -2m };
            yield return new object[] { -5m, -3m, -2m };
            yield return new object[] { 3m, 5m, 3m };
            yield return new object[] { 3m, -5m, 3m };
            yield return new object[] { -3m, 5m, -3m };
            yield return new object[] { -3m, -5m, -3m };
            yield return new object[] { 10m, -3m, 1m };
            yield return new object[] { -10m, 3m, -1m };
            yield return new object[] { -2.0m, 0.5m, -0.0m };
            yield return new object[] { 2.3m, 0.531m, 0.176m };
            yield return new object[] { 0.00123m, 3242m, 0.00123m };
            yield return new object[] { 3242m, 0.00123m, 0.00044m };
            yield return new object[] { 17.3m, 3m, 2.3m };
            yield return new object[] { 8.55m, 2.25m, 1.80m };
            yield return new object[] { 0.00m, 3m, 0.00m };
            yield return new object[] { NegativeZero, 2.2m, NegativeZero };

            // Max/Min
            yield return new object[] { decimal.MaxValue, decimal.MaxValue, 0m };
            yield return new object[] { decimal.MaxValue, decimal.MinValue, 0m };
            yield return new object[] { decimal.MaxValue, 1, 0m };
            yield return new object[] { decimal.MaxValue, 2394713m, 1494647m };
            yield return new object[] { decimal.MaxValue, -32768m, 32767m };
            yield return new object[] { -0.00m, decimal.MaxValue, -0.00m };
            yield return new object[] { 1.23984m, decimal.MaxValue, 1.23984m };
            yield return new object[] { 2398412.12983m, decimal.MaxValue, 2398412.12983m };
            yield return new object[] { -0.12938m, decimal.MaxValue, -0.12938m };

            yield return new object[] { decimal.MinValue, decimal.MinValue, NegativeZero };
            yield return new object[] { decimal.MinValue, decimal.MaxValue, NegativeZero };
            yield return new object[] { decimal.MinValue, 1, NegativeZero };
            yield return new object[] { decimal.MinValue, 2394713m, -1494647m };
            yield return new object[] { decimal.MinValue, -32768m, -32767m };
            yield return new object[] { 0.0m, decimal.MinValue, 0.0m };
            yield return new object[] { 1.23984m, decimal.MinValue, 1.23984m };
            yield return new object[] { 2398412.12983m, decimal.MinValue, 2398412.12983m };
            yield return new object[] { -0.12938m, decimal.MinValue, -0.12938m };

            yield return new object[] { 57675350989891243676868034225m, 7m, 5m };
            yield return new object[] { -57675350989891243676868034225m, 7m, -5m };
            yield return new object[] { 57675350989891243676868034225m, -7m, 5m };
            yield return new object[] { -57675350989891243676868034225m, -7m, -5m };
            
            yield return new object[] { 792281625142643375935439503.4m, 0.1m, 0.0m };
            yield return new object[] { 79228162514264337593543950.34m, 0.1m, 0.04m };
            yield return new object[] { 7922816251426433759354395.034m, 0.1m, 0.034m };
            yield return new object[] { 792281625142643375935439.5034m, 0.1m, 0.0034m };
            yield return new object[] { 79228162514264337593543950335m, 10m, 5m };
            yield return new object[] { 79228162514264337567774146561m, 10m, 1m };
            yield return new object[] { 79228162514264337567774146560m, 10m, 0m };
            yield return new object[] { 79228162514264337567774146559m, 10m, 9m };
        }

        [Theory, MemberData("Remainder_Valid_TestData")]
        public static void TestRemainder(decimal d1, decimal d2, decimal expected)
        {
            Assert.Equal(expected, d1 % d2);
            Assert.Equal(expected, decimal.Remainder(d1, d2));
        }

        public static IEnumerable<object[]> Remainder_Invalid_TestData()
        {
            yield return new object[] { 5m, 0m, typeof(DivideByZeroException) };
            yield return new object[] { decimal.MaxValue, 0.1m, typeof(OverflowException) };
        }

        [Theory, MemberData("Remainder_Invalid_TestData")]
        public static void TestRemainder_Invalid(decimal d1, decimal d2, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => d1 % d2);
            Assert.Throws(exceptionType, () => decimal.Remainder(d1, d2));
        }

        public static IEnumerable<object[]> Subtract_Valid_TestData()
        {
            yield return new object[] { 1m, 1m, 0m };
            yield return new object[] { 1m, 0m, 1m };
            yield return new object[] { 0m, 1m, -1m };
            yield return new object[] { 1m, 1m, 0m };
            yield return new object[] { -1m, 1m, -2m };
            yield return new object[] { 1m, -1m, 2m };
            yield return new object[] { decimal.MaxValue, decimal.Zero, decimal.MaxValue };
            yield return new object[] { decimal.MinValue, decimal.Zero, decimal.MinValue };
            yield return new object[] { 79228162514264337593543950330m, -5, decimal.MaxValue };
            yield return new object[] { 79228162514264337593543950330m, 5, 79228162514264337593543950325m };
            yield return new object[] { -79228162514264337593543950330m, 5, decimal.MinValue };
            yield return new object[] { -79228162514264337593543950330m, -5, -79228162514264337593543950325m };
            yield return new object[] { 1234.5678m, 0.00009m, 1234.56771m };
            yield return new object[] { -1234.5678m, 0.00009m, -1234.56789m };
            yield return new object[] { 0.1111111111111111111111111111m, 0.1111111111111111111111111111m, 0 };
            yield return new object[] { 0.2222222222222222222222222222m, 0.1111111111111111111111111111m, 0.1111111111111111111111111111m };
            yield return new object[] { 1.1111111111111111111111111110m, 0.5555555555555555555555555555m, 0.5555555555555555555555555555m };
        }

        [Theory, MemberData("Subtract_Valid_TestData")]
        public static void TestSubtract(decimal d1, decimal d2, decimal expected)
        {
            Assert.Equal(expected, d1 - d2);
            Assert.Equal(expected, decimal.Subtract(d1, d2));
        }

        public static IEnumerable<object[]> Subtract_Invalid_TestData()
        {
            yield return new object[] { 79228162514264337593543950330m, -6 };
            yield return new object[] { -79228162514264337593543950330m, 6 };
        }

        [Theory, MemberData("Subtract_Invalid_TestData")]
        public static void TestSubtract_Invalid(decimal d1, decimal d2)
        {
            Assert.Throws<OverflowException>(() => d1 - d2);
            Assert.Throws<OverflowException>(() => decimal.Subtract(d1, d2));
        }

        public static IEnumerable<object[]> TruncateTestData()
        {
            yield return new object[] { 123m, 123m };
            yield return new object[] { 123.123m, 123m };
            yield return new object[] { 123.456m, 123m };
            yield return new object[] { -123.123m, -123m };
            yield return new object[] { -123.456m, -123m };
        }

        [Theory, MemberData("TruncateTestData")]
        public static void TestTruncate(decimal d, decimal expected)
        {
            Assert.Equal(expected, decimal.Truncate(d));
        }

        public static IEnumerable<object[]> GetBitsTestData()
        {
            yield return new object[] { 1M, new int[] { 0x00000001, 0x00000000, 0x00000000, 0x00000000 } };
            yield return new object[] { 100000000000000M, new int[] { 0x107A4000, 0x00005AF3, 0x00000000, 0x00000000 } };
            yield return new object[] { 100000000000000.00000000000000M, new int[] { 0x10000000, 0x3E250261, 0x204FCE5E, 0x000E0000 } };
            yield return new object[] { 1.0000000000000000000000000000M, new int[] { 0x10000000, 0x3E250261, 0x204FCE5E, 0x001C0000 } };
            yield return new object[] { 123456789M, new int[] { 0x075BCD15, 0x00000000, 0x00000000, 0x00000000 } };
            yield return new object[] { 0.123456789M, new int[] { 0x075BCD15, 0x00000000, 0x00000000, 0x00090000 } };
            yield return new object[] { 0.000000000123456789M, new int[] { 0x075BCD15, 0x00000000, 0x00000000, 0x00120000 } };
            yield return new object[] { 0.000000000000000000123456789M, new int[] { 0x075BCD15, 0x00000000, 0x00000000, 0x001B0000 } };
            yield return new object[] { 4294967295M, new int[] { unchecked((int)0xFFFFFFFF), 0x00000000, 0x00000000, 0x00000000 } };
            yield return new object[] { 18446744073709551615M, new int[] { unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), 0x00000000, 0x00000000 } };
            yield return new object[] { decimal.MaxValue, new int[] { unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), 0x00000000 } };
            yield return new object[] { decimal.MinValue, new int[] { unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), unchecked((int)0x80000000) } };
            yield return new object[] { -7.9228162514264337593543950335M, new int[] { unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), unchecked((int)0x801C0000) } };
        }

        [Theory, MemberData("GetBitsTestData")]
        public static void TestGetBits(decimal input, int[] expected)
        {
            int[] bits = decimal.GetBits(input);

            Assert.Equal(expected, bits);

            bool sign = (bits[3] & 0x80000000) != 0;
            byte scale = (byte)((bits[3] >> 16) & 0x7F);
            decimal newValue = new decimal(bits[0], bits[1], bits[2], sign, scale);

            Assert.Equal(input, newValue);
        }

        [Fact]
        public static void TestToSingle()
        {
            float f = 12345.12f;
            Assert.Equal(f, decimal.ToSingle((decimal)f));
            Assert.Equal(-f, decimal.ToSingle((decimal)(-f)));

            f = 1e20f;
            Assert.Equal(f, decimal.ToSingle((decimal)f));
            Assert.Equal(-f, decimal.ToSingle((decimal)(-f)));

            f = 1e27f;
            Assert.Equal(f, decimal.ToSingle((decimal)f));
            Assert.Equal(-f, decimal.ToSingle((decimal)(-f)));
        }

        [Fact]
        public static void TestToDouble()
        {
            double d = decimal.ToDouble(new decimal(0, 0, 1, false, 0));
            
            double dbl = 123456789.123456;
            Assert.Equal(dbl, decimal.ToDouble((decimal)dbl));
            Assert.Equal(-dbl, decimal.ToDouble((decimal)(-dbl)));

            dbl = 1e20;
            Assert.Equal(dbl, decimal.ToDouble((decimal)dbl));
            Assert.Equal(-dbl, decimal.ToDouble((decimal)(-dbl)));

            dbl = 1e27;
            Assert.Equal(dbl, decimal.ToDouble((decimal)dbl));
            Assert.Equal(-dbl, decimal.ToDouble((decimal)(-dbl)));

            dbl = long.MaxValue;
            // Need to pass in the Int64.MaxValue to ToDouble and not dbl because the conversion to double is a little lossy and we want precision
            Assert.Equal(dbl, decimal.ToDouble(long.MaxValue));
            Assert.Equal(-dbl, decimal.ToDouble(-long.MaxValue));
        }

        [Fact]
        public static void TestToSByte()
        {
            Assert.Equal(sbyte.MinValue, decimal.ToSByte(sbyte.MinValue));
            Assert.Equal(-123, decimal.ToSByte(-123));
            Assert.Equal(0, decimal.ToSByte(0));
            Assert.Equal(123, decimal.ToSByte(123));
            Assert.Equal(sbyte.MaxValue, decimal.ToSByte(sbyte.MaxValue));
        }

        [Fact]
        public static void TestToSByte_Invalid()
        {
            Assert.Throws<OverflowException>(() => decimal.ToSByte(sbyte.MinValue - 1)); // Decimal < sbyte.MinValue
            Assert.Throws<OverflowException>(() => decimal.ToSByte(sbyte.MaxValue + 1)); // Decimal > sbyte.MaxValue
        }

        [Fact]
        public static void TestToInt16()
        {
            Assert.Equal(short.MinValue, decimal.ToInt16(short.MinValue));
            Assert.Equal(-123, decimal.ToInt16(-123));
            Assert.Equal(0, decimal.ToInt16(0));
            Assert.Equal(123, decimal.ToInt16(123));
            Assert.Equal(short.MaxValue, decimal.ToInt16(short.MaxValue));
        }

        [Fact]
        public static void TestToInt16_Invalid()
        {
            Assert.Throws<OverflowException>(() => decimal.ToInt16(short.MinValue - 1)); // Decimal < short.MinValue
            Assert.Throws<OverflowException>(() => decimal.ToInt16(short.MaxValue + 1)); // Decimal > short.MaxValue
        }

        [Fact]
        public static void TestToInt32()
        {
            Assert.Equal(int.MinValue, decimal.ToInt32(int.MinValue));
            Assert.Equal(-123, decimal.ToInt32(-123));
            Assert.Equal(0, decimal.ToInt32(0));
            Assert.Equal(123, decimal.ToInt32(123));
            Assert.Equal(int.MaxValue, decimal.ToInt32(int.MaxValue));
        }

        [Fact]
        public static void TestToInt32_Invalid()
        {
            Assert.Throws<OverflowException>(() => decimal.ToInt32((long)int.MinValue - 1)); // Decimal < int.MinValue
            Assert.Throws<OverflowException>(() => decimal.ToInt32((long)int.MaxValue + 1)); // Decimal > int.MaxValue
        }

        [Fact]
        public static void TestToInt64()
        {
            Assert.Equal(long.MinValue, decimal.ToInt64(long.MinValue));
            Assert.Equal(-123, decimal.ToInt64(-123));
            Assert.Equal(0, decimal.ToInt64(0));
            Assert.Equal(123, decimal.ToInt64(123));
            Assert.Equal(long.MaxValue, decimal.ToInt64(long.MaxValue));
        }

        [Fact]
        public static void TestToByte()
        {
            Assert.Equal(byte.MinValue, decimal.ToByte(byte.MinValue));
            Assert.Equal(123, decimal.ToByte(123));
            Assert.Equal(byte.MaxValue, decimal.ToByte(byte.MaxValue));
        }

        [Fact]
        public static void TestToByte_Invalid()
        {
            Assert.Throws<OverflowException>(() => decimal.ToByte(byte.MinValue - 1)); // Decimal < byte.MinValue
            Assert.Throws<OverflowException>(() => decimal.ToByte(byte.MaxValue + 1)); // Decimal > byte.MaxValue
        }

        [Fact]
        public static void TestToUInt16()
        {
            Assert.Equal(ushort.MinValue, decimal.ToUInt16(ushort.MinValue));
            Assert.Equal(123, decimal.ToByte(123));
            Assert.Equal(ushort.MaxValue, decimal.ToUInt16(ushort.MaxValue));
        }

        [Fact]
        public static void TestToUInt16_Invalid()
        {
            Assert.Throws<OverflowException>(() => decimal.ToUInt16(ushort.MinValue - 1)); // Decimal < ushort.MinValue
            Assert.Throws<OverflowException>(() => decimal.ToUInt16(ushort.MaxValue + 1)); // Decimal > ushort.MaxValue
        }
        
        [Fact]
        public static void TestToUInt32()
        {
            Assert.Equal(uint.MinValue, decimal.ToUInt32(uint.MinValue));
            Assert.Equal((uint)123, decimal.ToUInt32(123));
            Assert.Equal(uint.MaxValue, decimal.ToUInt32(uint.MaxValue));
        }

        [Fact]
        public static void TestToUInt32_Invalid()
        {
            Assert.Throws<OverflowException>(() => decimal.ToUInt32((long)uint.MinValue - 1)); // Decimal < uint.MinValue
            Assert.Throws<OverflowException>(() => decimal.ToUInt32((long)uint.MaxValue + 1)); // Decimal > uint.MaxValue
        }

        [Fact]
        public static void TestToUInt64()
        {
            Assert.Equal(ulong.MinValue, decimal.ToUInt64(ulong.MinValue));
            Assert.Equal((ulong)123, decimal.ToUInt64(123));
            Assert.Equal(ulong.MaxValue, decimal.ToUInt64(ulong.MaxValue));
        }

        [Fact]
        public static void TestToUInt64_Invalid()
        {
            Assert.Throws<OverflowException>(() => decimal.ToUInt64((long)ulong.MinValue - 1)); // Decimal < uint.MinValue
        }

        public static IEnumerable<object[]> ToStringTestData()
        {
            yield return new object[] { -8249.000003m, string.Format("{0}", -8249.000003) };
            yield return new object[] { 6310.23m, string.Format("{0}", 6310.23) };

            yield return new object[] { decimal.Zero, "0" };
            yield return new object[] { decimal.One, "1" };
            yield return new object[] { decimal.MinusOne, "-1" };
            yield return new object[] { decimal.MinValue, "-79228162514264337593543950335" };
            yield return new object[] { decimal.MaxValue, "79228162514264337593543950335" };
        }

        [Theory, MemberData("ToStringTestData")]
        public static void TestToString(decimal d, string expected)
        {
            Assert.Equal(expected, d.ToString());
        }
    }
}
