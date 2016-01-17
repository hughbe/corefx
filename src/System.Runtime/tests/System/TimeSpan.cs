// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;

using Xunit;

namespace System.Runtime.Tests
{
    public static class TimeSpanTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            VerifyTimeSpan(new TimeSpan(), 0, 0, 0, 0, 0);
            VerifyTimeSpan(default(TimeSpan), 0, 0, 0, 0, 0);
        }

        [Fact]
        public static void TestCtor_Long()
        {
            VerifyTimeSpan(new TimeSpan(999999999999999999), 1157407, 9, 46, 39, 999);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int()
        {
            var ts = new TimeSpan(10, 9, 8);
            VerifyTimeSpan(ts, 0, 10, 9, 8, 0);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int()
        {
            var ts = new TimeSpan(10, 9, 8, 7, 6);
            VerifyTimeSpan(ts, 10, 9, 8, 7, 6);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Invalid()
        {
            // TimeSpan > TimeSpan.MinValue
            TimeSpan min = TimeSpan.MinValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(min.Days - 1, min.Hours, min.Minutes, min.Seconds, min.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(min.Days, min.Hours - 1, min.Minutes, min.Seconds, min.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(min.Days, min.Hours, min.Minutes - 1, min.Seconds, min.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(min.Days, min.Hours, min.Minutes, min.Seconds - 1, min.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(min.Days, min.Hours, min.Minutes, min.Seconds, min.Milliseconds - 1));

            // TimeSpan > TimeSpan.MaxValue
            TimeSpan max = TimeSpan.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(max.Days + 1, max.Hours, max.Minutes, max.Seconds, max.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(max.Days, max.Hours + 1, max.Minutes, max.Seconds, max.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(max.Days, max.Hours, max.Minutes + 1, max.Seconds, max.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(max.Days, max.Hours, max.Minutes, max.Seconds + 1, max.Milliseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimeSpan(max.Days, max.Hours, max.Minutes, max.Seconds, max.Milliseconds + 1));
        }
        
        [Fact]
        public static void TestLimits()
        {
            VerifyTimeSpan(TimeSpan.MaxValue, 10675199, 2, 48, 5, 477);
            VerifyTimeSpan(TimeSpan.MinValue, -10675199, -2, -48, -5, -477);
            VerifyTimeSpan(TimeSpan.Zero, 0, 0, 0, 0, 0);
        }

        [Fact]
        public static void TestGetandSetParts()
        {
            var ts = new TimeSpan(10, 9, 8, 7, 6);
            
            Assert.Equal(new TimeSpan(ts.Days, 0, 0, 0, 0), TimeSpan.FromDays(ts.Days));            
            Assert.Equal(new TimeSpan(0, ts.Hours, 0, 0, 0), TimeSpan.FromHours(ts.Hours));            
            Assert.Equal(new TimeSpan(0, 0, ts.Minutes, 0, 0), TimeSpan.FromMinutes(ts.Minutes));            
            Assert.Equal(new TimeSpan(0, 0, 0, ts.Seconds, 0), TimeSpan.FromSeconds(ts.Seconds));            
            Assert.Equal(new TimeSpan(0, 0, 0, 0, ts.Milliseconds), TimeSpan.FromMilliseconds(ts.Milliseconds));
        }

        public static IEnumerable<object[]> ParseExact_Valid_TestData()
        {
            // Standard timespan formats 'c', 'g', 'G'
            yield return new object[] { "12:24:02", "c", new TimeSpan(0, 12, 24, 2) };
            yield return new object[] { "1.12:24:02", "c", new TimeSpan(1, 12, 24, 2) };
            yield return new object[] { "-01.07:45:16.999", "c", new TimeSpan(1, 7, 45, 16, 999).Negate() };
            yield return new object[] { "12:24:02", "g", new TimeSpan(0, 12, 24, 2) };
            yield return new object[] { "1:12:24:02", "g", new TimeSpan(1, 12, 24, 2) };
            yield return new object[] { "-01:07:45:16.999", "g", new TimeSpan(1, 7, 45, 16, 999).Negate() };
            yield return new object[] { "1:12:24:02.243", "G", new TimeSpan(1, 12, 24, 2, 243) };
            yield return new object[] { "-01:07:45:16.999", "G", new TimeSpan(1, 7, 45, 16, 999).Negate() };

            // Custom timespan formats
            yield return new object[] { "12.23:32:43", @"dd\.h\:m\:s", new TimeSpan(12, 23, 32, 43) };
            yield return new object[] { "012.23:32:43.893", @"ddd\.h\:m\:s\.fff", new TimeSpan(12, 23, 32, 43, 893) };
            yield return new object[] { "12.05:02:03", @"d\.hh\:mm\:ss", new TimeSpan(12, 5, 2, 3) };
            yield return new object[] { "12:34 minutes", @"mm\:ss\ \m\i\n\u\t\e\s", new TimeSpan(0, 12, 34) };
        }

        [Theory, MemberData("ParseExact_Valid_TestData")]
        public static void ParseExactTest(string inputTimeSpan, string format, TimeSpan expectedTimeSpan)
        {
            TimeSpan actualTimeSpan;
            Assert.Equal(expectedTimeSpan, TimeSpan.ParseExact(inputTimeSpan, format, new CultureInfo("en-US")));
            
            Assert.True(TimeSpan.TryParseExact(inputTimeSpan, format, new CultureInfo("en-US"), out actualTimeSpan));
            Assert.Equal(expectedTimeSpan, actualTimeSpan);

            // TimeSpanStyles is interpreted only for custom formats
            if (format != "c" && format != "g" && format != "G")
            {
                Assert.Equal(expectedTimeSpan.Negate(), TimeSpan.ParseExact(inputTimeSpan, format, new CultureInfo("en-US"), TimeSpanStyles.AssumeNegative));
                
                Assert.True(TimeSpan.TryParseExact(inputTimeSpan, format, new CultureInfo("en-US"), TimeSpanStyles.AssumeNegative, out actualTimeSpan));
                Assert.Equal(expectedTimeSpan.Negate(), actualTimeSpan);
            }
        }

        public static IEnumerable<object[]> ParseExact_Invalid_TestData()
        {
            // Standard timespan formats 'c', 'g', 'G'
            yield return new object[] { "24:24:02", "c", typeof(OverflowException) };
            yield return new object[] { "1:12:24:02", "c", typeof(FormatException) };
            yield return new object[] { "12:61:02", "g", typeof(OverflowException) };
            yield return new object[] { "1.12:24:02", "g", typeof(FormatException) };
            yield return new object[] { "1:07:45:16.99999999", "G", typeof(OverflowException) };
            yield return new object[] { "1:12:24:02", "G", typeof(FormatException) };

            // Custom timespan formats
            yield return new object[] { "12.35:32:43", @"dd\.h\:m\:s", typeof(OverflowException) };
            yield return new object[] { "12.5:2:3", @"d\.hh\:mm\:ss", typeof(FormatException) };
            yield return new object[] { "12.5:2", @"d\.hh\:mm\:ss", typeof(FormatException) };
        }

        [Theory, MemberData("ParseExact_Invalid_TestData")]
        public static void ParseExactTest_Exception(string inputTimeSpan, string format, Type expectedException)
        {
            Assert.Throws(expectedException, () => TimeSpan.ParseExact(inputTimeSpan, format, new CultureInfo("en-US")));

            TimeSpan actualTimeSpan;
            Assert.False(TimeSpan.TryParseExact(inputTimeSpan, format, new CultureInfo("en-US"), out actualTimeSpan));
            Assert.Equal(TimeSpan.Zero, actualTimeSpan);
        }

        private static void VerifyTimeSpan(TimeSpan ts, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Assert.Equal(days, ts.Days);
            Assert.Equal(hours, ts.Hours);
            Assert.Equal(minutes, ts.Minutes);
            Assert.Equal(seconds, ts.Seconds);
            Assert.Equal(milliseconds, ts.Milliseconds);
        }
    }
}
