// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;

using Xunit;

namespace System.Runtime.Tests
{
    public static class DateTimeOffsetTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            VerifyDateTimeOffset(new DateTimeOffset(), 1, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
            VerifyDateTimeOffset(default(DateTimeOffset), 1, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        }
        
        [Fact]
        public static void TestCtor_DateTime()
        {
            var dt = new DateTimeOffset(new DateTime(2012, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc));
            VerifyDateTimeOffset(dt, 2012, 6, 11, 0, 0, 0, 0, TimeSpan.Zero);

            dt = new DateTimeOffset(new DateTime(1986, 8, 15, 10, 20, 5, 4, DateTimeKind.Local));
            VerifyDateTimeOffset(dt, 1986, 8, 15, 10, 20, 5, 4, null);

            DateTimeOffset today = new DateTimeOffset(DateTime.Today);
            DateTimeOffset now = DateTimeOffset.Now;
            VerifyDateTimeOffset(today, now.Year, now.Month, now.Day, 0, 0, 0, 0, TimeSpan.Zero);

            today = new DateTimeOffset(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc));
            Assert.Equal(TimeSpan.Zero, today.Offset);
            Assert.False(today.UtcDateTime.IsDaylightSavingTime());
        }

        [Fact]
        public static void TestCtor_DateTime_Invalid()
        {
            // DateTime < DateTimeOffset.MinValue
            DateTimeOffset min = DateTimeOffset.MinValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond - 1, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour, min.Minute, min.Second - 1, min.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour, min.Minute - 1, min.Second, min.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour - 1, min.Minute , min.Second, min.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day - 1, min.Hour, min.Minute, min.Second, min.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month - 1, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year - 1, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond, DateTimeKind.Utc)));

            // DateTime > DateTimeOffset.MaxValue
            DateTimeOffset max = DateTimeOffset.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond + 1, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second + 1, max.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour, max.Minute + 1, max.Second, max.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour + 1, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day + 1, max.Hour, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month + 1, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year + 1, max.Month, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc)));
        }

        [Fact]
        public static void TestCtor_DateTime_TimeSpan()
        {
            var dt = new DateTimeOffset(DateTime.MinValue, TimeSpan.FromHours(-14));
            VerifyDateTimeOffset(dt, 1, 1, 1, 0, 0, 0, 0, TimeSpan.FromHours(-14));

            dt = new DateTimeOffset(DateTime.MaxValue, TimeSpan.FromHours(14));
            VerifyDateTimeOffset(dt, 9999, 12, 31, 23, 59, 59, 999, TimeSpan.FromHours(14));

            dt = new DateTimeOffset(new DateTime(2012, 12, 31, 13, 50, 10), TimeSpan.Zero);
            VerifyDateTimeOffset(dt, 2012, 12, 31, 13, 50, 10, 0, TimeSpan.Zero);
        }

        [Fact]
        public static void TestCtor_DateTime_TimeSpan_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(DateTime.Now, TimeSpan.FromHours(15))); // Local time and non timezone timespan
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(DateTime.Now, TimeSpan.FromHours(-15))); // Local time and non timezone timespan

            Assert.Throws<ArgumentException>(() => new DateTimeOffset(DateTime.UtcNow, TimeSpan.FromHours(1))); // Local time and non zero timespan
            
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(DateTime.UtcNow, new TimeSpan(0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(DateTime.UtcNow, new TimeSpan(0, 0, -3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(DateTime.UtcNow, new TimeSpan(0, 0, 0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(DateTime.UtcNow, new TimeSpan(0, 0, 0, 0, -3))); // TimeSpan is not whole minutes

            // DateTime < DateTimeOffset.MinValue
            DateTimeOffset min = DateTimeOffset.MinValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond - 1, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour, min.Minute, min.Second - 1, min.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour, min.Minute - 1, min.Second, min.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day, min.Hour - 1, min.Minute, min.Second, min.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month, min.Day - 1, min.Hour, min.Minute, min.Second, min.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year, min.Month - 1, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(min.Year - 1, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));

            // DateTime > DateTimeOffset.MaxValue
            DateTimeOffset max = DateTimeOffset.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond + 1, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second + 1, max.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour, max.Minute + 1, max.Second, max.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day, max.Hour + 1, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month, max.Day + 1, max.Hour, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year, max.Month + 1, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(new DateTime(max.Year + 1, max.Month, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond, DateTimeKind.Utc), TimeSpan.Zero));
        }

        [Fact]
        public static void TestCtor_Long_TimeSpan()
        {
            var expected = new DateTime(1, 2, 3, 4, 5, 6, 7);
            var dt = new DateTimeOffset(expected.Ticks, TimeSpan.Zero);
            VerifyDateTimeOffset(dt, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, TimeSpan.Zero);
        }

        [Fact]
        public static void TestCtor_Long_TimeSpan_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(0, new TimeSpan(0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(0, new TimeSpan(0, 0, -3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(0, new TimeSpan(0, 0, 0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(0, new TimeSpan(0, 0, 0, 0, -3))); // TimeSpan is not whole minutes
            
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(0, TimeSpan.FromHours(-15))); // TimeZone.Offset > 14
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(0, TimeSpan.FromHours(15))); // TimeZone.Offset < -14

            // Invalid DateTime
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(DateTimeOffset.MinValue.Ticks - 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(DateTimeOffset.MaxValue.Ticks + 1, TimeSpan.Zero));
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Int_TimeSpan()
        {
            var dt = new DateTimeOffset(1973, 10, 6, 14, 30, 0, 500, TimeSpan.Zero);
            VerifyDateTimeOffset(dt, 1973, 10, 6, 14, 30, 0, 500, TimeSpan.Zero);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Int_TimeSpan_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, -3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0, 0, -3))); // TimeSpan is not whole minutes

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, 1, TimeSpan.FromHours(-15))); // TimeZone.Offset > 14
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, 1, TimeSpan.FromHours(15))); // TimeZone.Offset < -14

            // Not a valid DateTime
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(0, 1, 1, 1, 1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(10000, 1, 1, 1, 1, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 0, 1, 1, 1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 13, 1, 1, 1, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 0, 1, 1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 32, 1, 1, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, -1, 1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 24, 1, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, -1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 60, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, -1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 60, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, -1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, 1000, TimeSpan.Zero));

            // DateTime < DateTimeOffset.MinValue
            DateTimeOffset min = DateTimeOffset.MinValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond - 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day, min.Hour, min.Minute, min.Second - 1, min.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day, min.Hour, min.Minute - 1, min.Second, min.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day, min.Hour - 1, min.Minute, min.Second, min.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day - 1, min.Hour, min.Minute, min.Second, min.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month - 1, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year - 1, min.Month, min.Day, min.Hour, min.Minute, min.Second, min.Millisecond, TimeSpan.Zero));

            // DateTime > DateTimeOffset.MaxValue
            DateTimeOffset max = DateTimeOffset.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond + 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second + 1, max.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day, max.Hour, max.Minute + 1, max.Second, max.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day, max.Hour + 1, max.Minute, max.Second, max.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day + 1, max.Hour, max.Minute, max.Second, max.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month + 1, max.Day + 1, max.Hour, max.Minute, max.Second, max.Millisecond, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year + 1, max.Month, max.Day, max.Hour, max.Minute, max.Second, max.Millisecond, TimeSpan.Zero));

        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_TimeSpan()
        {
            var dt = new DateTimeOffset(1973, 10, 6, 14, 30, 0, TimeSpan.Zero);
            VerifyDateTimeOffset(dt, 1973, 10, 6, 14, 30, 0, 0, TimeSpan.Zero);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_TimeSpan_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, -3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0, 0, 3))); // TimeSpan is not whole minutes
            Assert.Throws<ArgumentException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, new TimeSpan(0, 0, 0, 0, -3))); // TimeSpan is not whole minutes

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, TimeSpan.FromHours(-15))); // TimeZone.Offset > 14
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 1, TimeSpan.FromHours(15))); // TimeZone.Offset < -14

            // Not a valid DateTime
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(0, 1, 1, 1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(10000, 1, 1, 1, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 0, 1, 1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 13, 1, 1, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 0, 1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 32, 1, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, -1, 1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 24, 1, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, -1, 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 60, 1, TimeSpan.Zero));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, -1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(1, 1, 1, 1, 1, 60, TimeSpan.Zero));

            // DateTime < DateTimeOffset.MinValue
            DateTimeOffset min = DateTimeOffset.MinValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day, min.Hour, min.Minute, min.Second - 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day, min.Hour, min.Minute - 1, min.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day, min.Hour - 1, min.Minute, min.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month, min.Day - 1, min.Hour, min.Minute, min.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year, min.Month - 1, min.Day, min.Hour, min.Minute, min.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(min.Year - 1, min.Month, min.Day, min.Hour, min.Minute, min.Second, TimeSpan.Zero));

            // DateTime > DateTimeOffset.MaxValue
            DateTimeOffset max = DateTimeOffset.MaxValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day, max.Hour, max.Minute, max.Second + 1, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day, max.Hour, max.Minute + 1, max.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day, max.Hour + 1, max.Minute, max.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month, max.Day + 1, max.Hour, max.Minute, max.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year, max.Month + 1, max.Day + 1, max.Hour, max.Minute, max.Second, TimeSpan.Zero));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeOffset(max.Year + 1, max.Month, max.Day, max.Hour, max.Minute, max.Second, TimeSpan.Zero));
        }

        [Fact]
        public static void TestDateTimeLimits()
        {
            VerifyDateTimeOffset(DateTimeOffset.MaxValue, 9999, 12, 31, 23, 59, 59, 999, TimeSpan.Zero);
            VerifyDateTimeOffset(DateTimeOffset.MinValue, 1, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        }

        [Fact]
        public static void TestAddSubtract_TimeSpan()
        {
            var dt = new DateTimeOffset(new DateTime(2012, 6, 18, 10, 5, 1, 0));
            TimeSpan ts = dt.TimeOfDay;

            DateTimeOffset newDate = dt.Subtract(ts);
            Assert.Equal(new DateTimeOffset(new DateTime(2012, 6, 18, 0, 0, 0, 0)).Ticks, newDate.Ticks);
            Assert.Equal(dt.Ticks, newDate.Add(ts).Ticks);
        }

        public static IEnumerable<object[]> Subtract_TimeSpan_TestData()
        {
            var dt1 = new DateTimeOffset(new DateTime(2012, 6, 18, 10, 5, 1, 0, DateTimeKind.Utc));

            yield return new object[] { dt1, new TimeSpan(10, 5, 1), new DateTimeOffset(new DateTime(2012, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc)) };
            yield return new object[] { dt1, new TimeSpan(-10, -5, -1), new DateTimeOffset(new DateTime(2012, 6, 18, 20, 10, 2, 0, DateTimeKind.Utc)) };
        }

        [Theory, MemberData("Subtract_TimeSpan_TestData")]
        public static void TestSubtract_TimeSpan(DateTimeOffset dt, TimeSpan ts, DateTimeOffset expected)
        {
            Assert.Equal(expected, dt - ts);
            Assert.Equal(expected, dt.Subtract(ts));
        }

        public static IEnumerable<object[]> Subtract_DateTimeOffset_TestData()
        {
            var date1 = new DateTimeOffset(new DateTime(1996, 6, 3, 22, 15, 0, DateTimeKind.Utc));
            var date2 = new DateTimeOffset(new DateTime(1996, 12, 6, 13, 2, 0, DateTimeKind.Utc));
            var date3 = new DateTimeOffset(new DateTime(1996, 10, 12, 8, 42, 0, DateTimeKind.Utc));

            yield return new object[] { date2, date1, new TimeSpan(185, 14, 47, 0) };
            yield return new object[] { date1, date2, new TimeSpan(-185, -14, -47, 0) };
            yield return new object[] { date1, date2, new TimeSpan(-185, -14, -47, 0) };
        }

        [Theory, MemberData("Subtract_DateTimeOffset_TestData")]
        public static void TestSubtract_DateTimeOffset(DateTimeOffset dt1, DateTimeOffset dt2, TimeSpan expected)
        {
            Assert.Equal(expected, dt1 - dt2);
            Assert.Equal(expected, dt1.Subtract(dt2));
        }

        [Fact]
        public static void TestAddition()
        {
            var dt = new DateTimeOffset(new DateTime(1986, 8, 15, 10, 20, 5, 70));
            Assert.Equal(17, dt.AddDays(2).Day);
            Assert.Equal(13, dt.AddDays(-2).Day);

            Assert.Equal(10, dt.AddMonths(2).Month);
            Assert.Equal(6, dt.AddMonths(-2).Month);

            Assert.Equal(1996, dt.AddYears(10).Year);
            Assert.Equal(1976, dt.AddYears(-10).Year);

            Assert.Equal(13, dt.AddHours(3).Hour);
            Assert.Equal(7, dt.AddHours(-3).Hour);

            Assert.Equal(25, dt.AddMinutes(5).Minute);
            Assert.Equal(15, dt.AddMinutes(-5).Minute);

            Assert.Equal(35, dt.AddSeconds(30).Second);
            Assert.Equal(2, dt.AddSeconds(-3).Second);

            Assert.Equal(80, dt.AddMilliseconds(10).Millisecond);
            Assert.Equal(60, dt.AddMilliseconds(-10).Millisecond);
        }

        [Fact]
        public static void TestDayOfWeek()
        {
            var dt = new DateTimeOffset(new DateTime(2012, 6, 18));
            Assert.Equal(DayOfWeek.Monday, dt.DayOfWeek);
        }
        
        [Fact]
        public static void TestToFromFileTime()
        {
            var today = new DateTimeOffset(DateTime.Today);

            long dateTimeRaw = today.ToFileTime();
            Assert.Equal(today, DateTimeOffset.FromFileTime(dateTimeRaw));
        }

        [Fact]
        public static void TestUtcDateTime()
        {
            DateTime now = DateTime.Now;
            var dto = new DateTimeOffset(now);
            Assert.Equal(DateTime.Today, dto.Date);
            Assert.Equal(now, dto.DateTime);
            Assert.Equal(now.ToUniversalTime(), dto.UtcDateTime);
        }
        
        [Fact]
        public static void TestParse_String()
        {
            DateTimeOffset expected = DateTimeOffset.MaxValue;
            string expectedStr = expected.ToString();

            DateTimeOffset result = DateTimeOffset.Parse(expectedStr);
            Assert.Equal(expectedStr, result.ToString());
        }

        [Fact]
        public static void TestParse_String_FormatProvider()
        {
            DateTimeOffset expected = DateTimeOffset.MaxValue;
            string expectedStr = expected.ToString();

            DateTimeOffset result = DateTimeOffset.Parse(expectedStr, null);
            Assert.Equal(expectedStr, result.ToString());
        }

        [Fact]
        public static void TestParse_String_FormatProvider_DateTimeStyles()
        {
            DateTimeOffset expected = DateTimeOffset.MaxValue;
            string expectedStr = expected.ToString();

            DateTimeOffset result = DateTimeOffset.Parse(expectedStr, null, DateTimeStyles.None);
            Assert.Equal(expectedStr, result.ToString());
        }

        [Fact]
        public static void TestParse_Japanese()
        {
            var expected = new DateTimeOffset(new DateTime(2012, 12, 21, 10, 8, 6));
            var ci = new CultureInfo("ja-JP");

            string s = string.Format(ci, "{0}", expected);
            Assert.Equal(expected, DateTimeOffset.Parse(s, ci));
        }
        
        [Fact]
        public static void TestTryParse_String()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("u");

            DateTimeOffset result;
            Assert.True(DateTimeOffset.TryParse(expectedStr, out result));
            Assert.Equal(expectedStr, result.ToString("u"));
        }

        [Fact]
        public static void TestTryParse_String_FormatProvider_DateTimeStyles_U()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("u");

            DateTimeOffset result;
            Assert.True(DateTimeOffset.TryParse(expectedStr, null, DateTimeStyles.None, out result));
            Assert.Equal(expectedStr, result.ToString("u"));
        }

        [Fact]
        public static void TestTryParse_String_FormatProvider_DateTimeStyles_G()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("g");

            DateTimeOffset result;
            Assert.True(DateTimeOffset.TryParse(expectedStr, null, DateTimeStyles.AssumeUniversal, out result));
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        [Fact]
        public static void TestTryParse_TimeDesignators()
        {
            DateTimeOffset result;
            Assert.True(DateTimeOffset.TryParse("4/21 5am", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(4, result.Month);
            Assert.Equal(21, result.Day);
            Assert.Equal(5, result.Hour);

            Assert.True(DateTimeOffset.TryParse("4/21 5pm", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(4, result.Month);
            Assert.Equal(21, result.Day);
            Assert.Equal(17, result.Hour);
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("G");

            DateTimeOffset result = DateTimeOffset.ParseExact(expectedStr, "G", null);
            Assert.Equal(expectedStr, result.ToString("G"));
        }

        [Fact]
        public static void  TestParseExact_String_String_FormatProvider_DateTimeStyles_U()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("u");

            DateTimeOffset result = DateTimeOffset.ParseExact(expectedStr, "u", null, DateTimeStyles.None);
            Assert.Equal(expectedStr, result.ToString("u"));
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider_DateTimeStyles_G()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("g");

            DateTimeOffset result = DateTimeOffset.ParseExact(expectedStr, "g", null, DateTimeStyles.AssumeUniversal);
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider_DateTimeStyles_O()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("o");

            DateTimeOffset result = DateTimeOffset.ParseExact(expectedStr, "o", null, DateTimeStyles.None);
            Assert.Equal(expectedStr, result.ToString("o"));
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider_DateTimeStyles_CustomFormatProvider()
        {
            var formatter = new MyFormatter();
            string dateBefore = DateTime.Now.ToString();

            DateTimeOffset dateAfter = DateTimeOffset.ParseExact(dateBefore, "G", formatter, DateTimeStyles.AssumeUniversal);
            Assert.Equal(dateBefore, dateAfter.DateTime.ToString());
        }

        [Fact]
        public static void TestParseExact_String_StringArray_FormatProvider_DateTimeStyles()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("g");

            var formats = new string[] { "g" };
            DateTimeOffset result = DateTimeOffset.ParseExact(expectedStr, formats, null, DateTimeStyles.AssumeUniversal);
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        [Fact]
        public static void TestTryParseExact_String_String_FormatProvider_DateTimeStyles_NullFormatProvider()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("g");

            DateTimeOffset resulted;
            Assert.True(DateTimeOffset.TryParseExact(expectedStr, "g", null, DateTimeStyles.AssumeUniversal, out resulted));
            Assert.Equal(expectedStr, resulted.ToString("g"));
        }
        
        [Fact]
        public static void TestTryParseExact_String_StringArray_FormatProvider_DateTimeStyles()
        {
            DateTimeOffset src = DateTimeOffset.MaxValue;
            string expectedStr = src.ToString("g");

            var formats = new string[] { "g" };
            DateTimeOffset result;
            Assert.True(DateTimeOffset.TryParseExact(expectedStr, formats, null, DateTimeStyles.AssumeUniversal, out result));
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        private static void VerifyDateTimeOffset(DateTimeOffset dt, int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan? ts)
        {
            Assert.Equal(year, dt.Year);
            Assert.Equal(month, dt.Month);
            Assert.Equal(day, dt.Day);
            Assert.Equal(hour, dt.Hour);
            Assert.Equal(minute, dt.Minute);
            Assert.Equal(second, dt.Second);
            Assert.Equal(millisecond, dt.Millisecond);

            if (ts.HasValue)
            {
                Assert.Equal(ts.Value, dt.Offset);
            }
        }

        private class MyFormatter : IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                return typeof(IFormatProvider) == formatType ? this : null;
            }
        }
    }
}
