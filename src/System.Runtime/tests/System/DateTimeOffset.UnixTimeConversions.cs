// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using Xunit;

namespace System.Runtime.Tests
{
    public static class DateTimeOffsetUnixTimeConversionTests
    {
        private static IEnumerable<object[]> TestTimes() 
        {
            yield return new object[] { TestTime.FromMilliseconds(DateTimeOffset.MinValue, -62135596800000) };
            yield return new object[] { TestTime.FromMilliseconds(DateTimeOffset.MaxValue, 253402300799999) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero), 0) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(2014, 6, 13, 17, 21, 50, TimeSpan.Zero), 1402680110000) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(2830, 12, 15, 1, 23, 45, TimeSpan.Zero), 27169089825000) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(2830, 12, 15, 1, 23, 45, 399, TimeSpan.Zero), 27169089825399) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(9999, 12, 30, 23, 24, 25, TimeSpan.Zero), 253402212265000) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(1907, 7, 7, 7, 7, 7, TimeSpan.Zero), -1971967973000) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(1907, 7, 7, 7, 7, 7, 1, TimeSpan.Zero), -1971967972999) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(1907, 7, 7, 7, 7, 7, 777, TimeSpan.Zero), -1971967972223) };
            yield return new object[] { TestTime.FromMilliseconds(new DateTimeOffset(601636288270011234, TimeSpan.Zero), -1971967972999) };
        }

        [Theory, MemberData("TestTimes")]
        public static void TestToUnixTimeMilliseconds(TestTime test)
        {
            long expectedMilliseconds = test.UnixTimeMilliseconds;
            long actualMilliseconds = test.DateTimeOffset.ToUnixTimeMilliseconds();
            Assert.Equal(expectedMilliseconds, actualMilliseconds);
        }

        [Theory, MemberData("TestTimes")]
        public static void TesToUnixTimeMilliseconds_RountTrip(TestTime test)
        {
            long unixTimeMilliseconds = test.DateTimeOffset.ToUnixTimeMilliseconds();
            TestFromUnixTimeMilliseconds(TestTime.FromMilliseconds(test.DateTimeOffset, unixTimeMilliseconds));
        }

        [Theory, MemberData("TestTimes")]
        public static void TestToUnixTimeSeconds(TestTime test)
        {
            long expectedSeconds = test.UnixTimeSeconds;
            long actualSeconds = test.DateTimeOffset.ToUnixTimeSeconds();
            Assert.Equal(expectedSeconds, actualSeconds);
        }

        [Theory, MemberData("TestTimes")]
        public static void TesToUnixTimeSeconds_RoundTrip(TestTime test)
        {
            long unixTimeSeconds = test.DateTimeOffset.ToUnixTimeSeconds();
            TestFromUnixTimeSeconds(TestTime.FromSeconds(test.DateTimeOffset, unixTimeSeconds));
        }

        [Theory, MemberData("TestTimes")]
        public static void TestFromUnixTimeMilliseconds(TestTime test)
        {
            // Only assert that expected == actual up to millisecond precision for conversion from milliseconds
            long expectedTicks = (test.DateTimeOffset.UtcTicks / TimeSpan.TicksPerMillisecond) * TimeSpan.TicksPerMillisecond;
            long actualTicks = DateTimeOffset.FromUnixTimeMilliseconds(test.UnixTimeMilliseconds).UtcTicks;
            Assert.Equal(expectedTicks, actualTicks);
        }

        [Fact]
        public static void TestFromUnixTimeMilliseconds_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeMilliseconds(long.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeMilliseconds(long.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeMilliseconds(-62135596800001)); // Milliseconds < DateTimeOffset.MinValue
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeMilliseconds(253402300800000)); // Milliseconds > DateTimeOffset.MaxValue
        }

        [Theory, MemberData("TestTimes")]
        public static void TestFromUnixTimeSeconds(TestTime test)
        {
            // Only assert that expected == actual up to second precision for conversion from seconds
            long expectedTicks = (test.DateTimeOffset.UtcTicks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond;
            long actualTicks = DateTimeOffset.FromUnixTimeSeconds(test.UnixTimeSeconds).UtcTicks;
            Assert.Equal(expectedTicks, actualTicks);
        }

        [Fact]
        public static void TestFromUnixTimeSeconds_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeSeconds(long.MinValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeSeconds(long.MaxValue));

            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeSeconds(-62135596801));// Seconds < DateTimeOffset.MinValue
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeOffset.FromUnixTimeSeconds(253402300800)); // Seconds > DateTimeOffset.MaxValue
        }

        [Theory, MemberData("TestTimes")]
        public static void TestFromUnixTimeMilliseconds_RoundTrip(TestTime test)
        {
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds(test.UnixTimeMilliseconds);
            TestToUnixTimeMilliseconds(TestTime.FromMilliseconds(dateTime, test.UnixTimeMilliseconds));
        }

        [Theory, MemberData("TestTimes")]
        public static void TestFromUnixTimeSeconds_RoundTrip(TestTime test)
        {
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(test.UnixTimeSeconds);
            TestToUnixTimeSeconds(TestTime.FromSeconds(dateTime, test.UnixTimeSeconds));
        }

        public class TestTime
        {
            private TestTime(DateTimeOffset dateTimeOffset, long unixTimeMilliseconds, long unixTimeSeconds)
            {
                DateTimeOffset = dateTimeOffset;
                UnixTimeMilliseconds = unixTimeMilliseconds;
                UnixTimeSeconds = unixTimeSeconds;
            }

            public static TestTime FromMilliseconds(DateTimeOffset dateTimeOffset, long unixTimeMilliseconds)
            {
                long unixTimeSeconds = unixTimeMilliseconds / 1000;

                // Always round UnixTimeSeconds down toward 1/1/0001 00:00:00
                // (this happens automatically for unixTimeMilliseconds > 0)
                bool hasSubSecondPrecision = unixTimeMilliseconds % 1000 != 0;
                if (unixTimeMilliseconds < 0 && hasSubSecondPrecision)
                {
                    --unixTimeSeconds;
                }

                return new TestTime(dateTimeOffset, unixTimeMilliseconds, unixTimeSeconds);
            }

            public static TestTime FromSeconds(DateTimeOffset dateTimeOffset, long unixTimeSeconds)
            {
                return new TestTime(dateTimeOffset, unixTimeSeconds * 1000, unixTimeSeconds);
            }

            public DateTimeOffset DateTimeOffset { get; private set; }
            public long UnixTimeMilliseconds { get; private set; }
            public long UnixTimeSeconds { get; private set; }
        }
    }
}
