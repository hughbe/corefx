// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class VersionTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 3)]
        [InlineData(int.MaxValue, int.MaxValue)]
        public static void TestCtor_Int_Int(int major, int minor)
        {
            VerifyConstructor(major, minor);
        }

        [Fact]
        public static void TestCtor_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(-1, 0)); // Major < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(0, -1)); // Minor < 0
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(2, 3, 4)]
        [InlineData(int.MaxValue, int.MaxValue, int.MaxValue)]
        public static void TestCtor_Int_Int_Int(int major, int minor, int build)
        {
            VerifyConstructor(major, minor, build);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(-1, 0, 0)); // Major < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(0, -1, 0)); // Minor < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(0, 0, -1)); // Build < 0
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(2, 3, 4, 7)]
        [InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue)]
        public static void TestMajorMinorBuildRevisionConstructor(int major, int minor, int build, int revision)
        {
            VerifyConstructor(major, minor, build, revision);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(-1, 0, 0, 0)); // Major < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(0, -1, 0, 0)); // Minor < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(0, 0, -1, 0)); // Build < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new Version(0, 0, 0, -1)); // Revision < 0
        }

        private static void VerifyConstructor(int major, int minor)
        {
            var version = new Version(major, minor);
            VerifyVersion(version, major, minor, -1, -1);
        }

        private static void VerifyConstructor(int major, int minor, int build)
        {
            var version = new Version(major, minor, build);
            VerifyVersion(version, major, minor, build, -1);
        }

        private static void VerifyConstructor(int major, int minor, int build, int revision)
        {
            var version = new Version(major, minor, build, revision);
            VerifyVersion(version, major, minor, build, revision);
        }

        private static void VerifyVersion(Version version, int major, int minor, int build, int revision)
        {
            Assert.Equal(major, version.Major);
            Assert.Equal(minor, version.Minor);
            Assert.Equal(build, version.Build);
            Assert.Equal(revision, version.Revision);
            Assert.Equal((short)(revision >> 16), version.MajorRevision);
            Assert.Equal((short)(revision & 0xFFFF), version.MinorRevision);
        }

        public static IEnumerable<object[]> CompareToData()
        {
            yield return new object [] { null, 1 };
            yield return new object[] { new Version(1, 2), 0 };
            yield return new object[] { new Version(2, 0), -1 };
            yield return new object[] { new Version(1, 2, 1), -1 };
            yield return new object[] { new Version(1, 2, 0, 1), -1 };
            yield return new object[] { new Version(1, 0), 1 };
            yield return new object[] { new Version(1, 0, 1), 1 };
            yield return new object[] { new Version(1, 0, 0, 1), 1 };
        }
        
        [Theory]
        [MemberData("CompareToData")]
        public static void TestCompareTo(Version value, int expected)
        {
            var version = new Version(1, 2);
            Assert.Equal(expected, CompareHelper.NormalizeCompare(version.CompareTo(value)));

            IComparable iComparable = version;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(iComparable.CompareTo(value)));
        }

        private static IEnumerable<object[]> EqualsData()
        {
            yield return new object[] { new Version(2, 3), new Version(2, 3), true };
            yield return new object[] { new Version(2, 3, 4), new Version(2, 3, 4), true };
            yield return new object[] { new Version(2, 3, 4, 5), new Version(2, 3, 4, 5), true };

            yield return new object[] { new Version(5, 5), new Version(5, 4), false };
            yield return new object[] { new Version(10, 10, 10), new Version(10, 10, 2), false };
            yield return new object[] { new Version(10, 10, 10, 10), new Version(10, 10, 10, 3), false };
            yield return new object[] { new Version(10, 10, 10, 10), new Version(10, 10), false };
        }

        [Fact]
        public static void TestEquals_Object()
        {
            var x = new Version(2, 3);
            Assert.Equal(x, (object)x);

            var y = (object)x;
            Assert.Equal(x, y);

            y = new Version(2, 4);
            Assert.NotEqual(x, y);
        }
        
        [Theory]
        [MemberData("EqualsData")]
        public static void TestEquals(Version version1, Version version2, bool equal)
        {
            Assert.Equal(version1, version1);

            Assert.Equal(equal, version1.Equals(version2));
            Assert.Equal(equal, version2.Equals(version1));
        }

        [Theory]
        [MemberData("EqualsData")]
        public static void TestGetHashCode(Version version1, Version version2, bool equal)
        {
            Assert.Equal(equal, version1.GetHashCode().Equals(version2.GetHashCode()));
        }

        [Fact]
        public static void TestToString()
        {
            Assert.Equal("1.2", new Version(1, 2).ToString());
            Assert.Equal("1.2.3", new Version(1, 2, 3).ToString());
            Assert.Equal("1.2.3.4", new Version(1, 2, 3, 4).ToString());
        }

        [Fact]
        public static void TestToStringFieldCount()
        {
            var version = new Version(5, 3);
            Assert.Equal(string.Empty, version.ToString(0));
            Assert.Equal("5", version.ToString(1));
            Assert.Equal("5.3", version.ToString(2));
            Assert.Throws<ArgumentException>(() => version.ToString(3));
            Assert.Throws<ArgumentException>(() => version.ToString(4));
            Assert.Throws<ArgumentException>(() => version.ToString(5));
            Assert.Throws<ArgumentException>(() => version.ToString(-1));

            version = new Version(10, 11, 12);
            Assert.Equal(string.Empty, version.ToString(0));
            Assert.Equal("10", version.ToString(1));
            Assert.Equal("10.11", version.ToString(2));
            Assert.Equal("10.11.12", version.ToString(3));
            Assert.Throws<ArgumentException>(() => version.ToString(4));
            Assert.Throws<ArgumentException>(() => version.ToString(5));
            Assert.Throws<ArgumentException>(() => version.ToString(-1));

            version = new Version(1, 2, 3, 4);
            Assert.Equal(string.Empty, version.ToString(0));
            Assert.Equal("1", version.ToString(1));
            Assert.Equal("1.2", version.ToString(2));
            Assert.Equal("1.2.3", version.ToString(3));
            Assert.Equal("1.2.3.4", version.ToString(4));
            Assert.Throws<ArgumentException>(() => version.ToString(5));
            Assert.Throws<ArgumentException>(() => version.ToString(-1));
        }

        private static IEnumerable<object[]> ParseData()
        {
            yield return new object[] { "1.2", new Version(1, 2) };
            yield return new object[] { "1.2.3", new Version(1, 2, 3) };
            yield return new object[] { "1.2.3.4", new Version(1, 2, 3, 4) };
            yield return new object[] { "2  .3.    4.  \t\r\n15  ", new Version(2, 3, 4, 15) };
        }

        [Theory]
        [MemberData("ParseData")]
        public static void TestParse(string input, Version expected)
        {
            Assert.Equal(expected, Version.Parse(input));

            Version version;
            Assert.True(Version.TryParse(input, out version));
            Assert.Equal(expected, version);
        }

        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData("1,2,3,4", typeof(ArgumentException))]
        [InlineData("1", typeof(ArgumentException))]
        [InlineData("1.b.3.4", typeof(FormatException))]
        [InlineData("1.-1", typeof(ArgumentOutOfRangeException))]
        [InlineData("1.5000000000000", typeof(OverflowException))]
        public static void TestParse_Invalid(string input, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => Version.Parse(input));
            Version version;
            Assert.False(Version.TryParse(input, out version));
        }
    }
}
