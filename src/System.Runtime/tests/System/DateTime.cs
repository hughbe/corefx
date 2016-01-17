// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Xunit;

namespace System.Runtime.Tests
{
    public static class DateTimeTests
    {
        [Fact]
        public static void TestCtor_Long()
        {
            VerifyDateTime(new DateTime(999999999999999999), 3169, 11, 16, 9, 46, 39, 999, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void TestCtor_Long_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(DateTime.MinValue.Ticks - 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(DateTime.MaxValue.Ticks + 1));
        }

        [Fact]
        public static void TestCtor_Long_DateTimeKind()
        {
            VerifyDateTime(new DateTime(999999999999999999, DateTimeKind.Utc), 3169, 11, 16, 9, 46, 39, 999, DateTimeKind.Utc);
        }

        [Fact]
        public static void TestCtor_Long_DateTimeKind_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(DateTime.MinValue.Ticks - 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(DateTime.MaxValue.Ticks + 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentException>(() => new DateTime(0, DateTimeKind.Unspecified - 1));
            Assert.Throws<ArgumentException>(() => new DateTime(0, DateTimeKind.Local + 1));
        }

        [Fact]
        public static void TestCtor_Int_Int_Int()
        {
            var dt = new DateTime(2012, 6, 11);
            VerifyDateTime(dt, 2012, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(0, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(10000, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 13, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 32));
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int()
        {
            var dt = new DateTime(2012, 12, 31, 13, 50, 10);
            VerifyDateTime(dt, 2012, 12, 31, 13, 50, 10, 0, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(0, 1, 1, 1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(10000, 1, 1, 1, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 0, 1, 1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 13, 1, 1, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 0, 1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 32, 1, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, -1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 24, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 60, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 60));
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Int_DateTimeKind()
        {
            var dt = new DateTime(1986, 8, 15, 10, 20, 5, DateTimeKind.Local);
            VerifyDateTime(dt, 1986, 8, 15, 10, 20, 5, 0, DateTimeKind.Local);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_DateTimeKind_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(0, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(10000, 1, 1, 1, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 0, 1, 1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 13, 1, 1, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 0, 1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 32, 1, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, -1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 24, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, -1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 60, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, -1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 60, DateTimeKind.Utc));

            Assert.Throws<ArgumentException>(() => new DateTime(1, 1, 1, 1, 1, 1, DateTimeKind.Unspecified - 1));
            Assert.Throws<ArgumentException>(() => new DateTime(1, 1, 1, 1, 1, 1, DateTimeKind.Local + 1));
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Int()
        {
            var dt = new DateTime(1973, 10, 6, 14, 30, 0, 500);
            VerifyDateTime(dt, 1973, 10, 6, 14, 30, 0, 500, DateTimeKind.Unspecified);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(0, 1, 1, 1, 1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(10000, 1, 1, 1, 1, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 0, 1, 1, 1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 13, 1, 1, 1, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 0, 1, 1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 32, 1, 1, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, -1, 1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 24, 1, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, -1, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 60, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 60, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 1, 1000));
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Int_Int_DateTimeKind()
        {
            var dt = new DateTime(1986, 8, 15, 10, 20, 5, 600, DateTimeKind.Local);
            VerifyDateTime(dt, 1986, 8, 15, 10, 20, 5, 600, DateTimeKind.Local);
        }

        [Fact]
        public static void TestCtor_Int_Int_Int_Int_Int_Int_Int_DateTimeKind_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(0, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(10000, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 0, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 13, 1, 1, 1, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 0, 1, 1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 32, 1, 1, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, -1, 1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 24, 1, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, -1, 1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 60, 1, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, -1, 1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 60, 1, DateTimeKind.Utc));

            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 1, -1, DateTimeKind.Utc));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DateTime(1, 1, 1, 1, 1, 1, 1000, DateTimeKind.Utc));

            Assert.Throws<ArgumentException>(() => new DateTime(1, 1, 1, 1, 1, 1, 1, DateTimeKind.Unspecified - 1));
            Assert.Throws<ArgumentException>(() => new DateTime(1, 1, 1, 1, 1, 1, 1, DateTimeKind.Local + 1));
        }

        [Fact]
        public static void TestDateTimeLimits()
        {
            VerifyDateTime(DateTime.MaxValue, 9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified);
            VerifyDateTime(DateTime.MinValue, 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
        }

        [Theory]
        [InlineData(2004, true)]
        [InlineData(2005, false)]
        public static void TestLeapYears(int year, bool expected)
        {
            Assert.Equal(expected, DateTime.IsLeapYear(year));
        }

        [Fact]
        public static void TestAddition()
        {
            var dt = new DateTime(1986, 8, 15, 10, 20, 5, 70);
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
            var dt = new DateTime(2012, 6, 18);
            Assert.Equal(DayOfWeek.Monday, dt.DayOfWeek);
        }

        [Fact]
        public static void TestTimeOfDay()
        {
            var dt = new DateTime(2012, 6, 18, 10, 5, 1, 0);
            TimeSpan ts = dt.TimeOfDay;

            DateTime newDate = dt.Subtract(ts);
            Assert.Equal(new DateTime(2012, 6, 18, 0, 0, 0, 0).Ticks, newDate.Ticks);
            Assert.Equal(dt.Ticks, newDate.Add(ts).Ticks);
        }

        [Fact]
        public static void TestToday()
        {
            DateTime today = DateTime.Today;
            DateTime now = DateTime.Now;
            VerifyDateTime(today, now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Local);

            today = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
            Assert.Equal(DateTimeKind.Utc, today.Kind);
            Assert.False(today.IsDaylightSavingTime());
        }

        [Fact]
        public static void TestConversion()
        {
            DateTime today = DateTime.Today;
            long dateTimeRaw = today.ToBinary();
            Assert.Equal(today, DateTime.FromBinary(dateTimeRaw));

            dateTimeRaw = today.ToFileTime();
            Assert.Equal(today, DateTime.FromFileTime(dateTimeRaw));

            dateTimeRaw = today.ToFileTimeUtc();
            Assert.Equal(today, DateTime.FromFileTimeUtc(dateTimeRaw).ToLocalTime());
        }
        
        public static IEnumerable<object[]> Subtract_TimeSpan_TestData()
        {
            var dt1 = new DateTime(2012, 6, 18, 10, 5, 1, 0, DateTimeKind.Utc);

            yield return new object[] { dt1, new TimeSpan(10, 5, 1), new DateTime(2012, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc) };
            yield return new object[] { dt1, new TimeSpan(-10, -5, -1), new DateTime(2012, 6, 18, 20, 10, 2, 0, DateTimeKind.Utc) };
        }

        [Theory, MemberData("Subtract_TimeSpan_TestData")]
        public static void TestSubtract_TimeSpan(DateTime dt, TimeSpan ts, DateTime expected)
        {
            Assert.Equal(expected, dt - ts);
            Assert.Equal(expected, dt.Subtract(ts));
        }

        public static IEnumerable<object[]> Subtract_DateTimeOffset_TestData()
        {
            var date1 = new DateTime(1996, 6, 3, 22, 15, 0, DateTimeKind.Utc);
            var date2 = new DateTime(1996, 12, 6, 13, 2, 0, DateTimeKind.Utc);
            var date3 = new DateTime(1996, 10, 12, 8, 42, 0, DateTimeKind.Utc);

            yield return new object[] { date2, date1, new TimeSpan(185, 14, 47, 0) };
            yield return new object[] { date1, date2, new TimeSpan(-185, -14, -47, 0) };
            yield return new object[] { date1, date2, new TimeSpan(-185, -14, -47, 0) };
        }

        [Theory, MemberData("Subtract_DateTimeOffset_TestData")]
        public static void TestSubtract_DateTime(DateTime dt1, DateTime dt2, TimeSpan expected)
        {
            Assert.Equal(expected, dt1 - dt2);
            Assert.Equal(expected, dt1.Subtract(dt2));
        }

        [Fact]
        public static void TestParse_String()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedStr = expected.ToString();

            DateTime result = DateTime.Parse(expectedStr);
            Assert.Equal(expectedStr, result.ToString());
        }

        [Fact]
        public static void TestParse_String_FormatProvider()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedStr = expected.ToString();

            DateTime result = DateTime.Parse(expectedStr, null);
            Assert.Equal(expectedStr, result.ToString());
        }

        [Fact]
        public static void TestParse_String_FormatProvider_DateTimeStyles()
        {
            DateTime expected = DateTime.MaxValue;
            string expectedStr = expected.ToString();

            DateTime result = DateTime.Parse(expectedStr, null, DateTimeStyles.None);
            Assert.Equal(expectedStr, result.ToString());
        }

        [Fact]
        public static void TestParse_Japanese()
        {
            var expected = new DateTime(2012, 12, 21, 10, 8, 6);
            var ci = new CultureInfo("ja-JP");

            string s = string.Format(ci, "{0}", expected);
            Assert.Equal(expected, DateTime.Parse(s, ci));
        }

        [Fact]
        public static void TestTryParse_String()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("u");

            DateTime result;
            Assert.True(DateTime.TryParse(expectedStr, out result));
            Assert.Equal(expectedStr, result.ToString("u"));
        }

        [Fact]
        public static void TestTryParse_String_FormatProvider_DateTimeStyles_U()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("u");

            DateTime result;
            Assert.True(DateTime.TryParse(expectedStr, null, DateTimeStyles.None, out result));
            Assert.Equal(expectedStr, result.ToString("u"));
        }

        [Fact]
        public static void TestTryParse_String_FormatProvider_DateTimeStyles_G()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("g");

            DateTime result;
            Assert.True(DateTime.TryParse(expectedStr, null, DateTimeStyles.AssumeUniversal, out result));
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        [Fact]
        public static void TestTryParse_TimeDesignators()
        {
            DateTime result;
            Assert.True(DateTime.TryParse("4/21 5am", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(4, result.Month);
            Assert.Equal(21, result.Day);
            Assert.Equal(5, result.Hour);

            Assert.True(DateTime.TryParse("4/21 5pm", new CultureInfo("en-US"), DateTimeStyles.None, out result));
            Assert.Equal(4, result.Month);
            Assert.Equal(21, result.Day);
            Assert.Equal(17, result.Hour);
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("G");

            DateTime result = DateTime.ParseExact(expectedStr, "G", null);
            Assert.Equal(expectedStr, result.ToString("G"));
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider_DateTimeStyles_U()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("u");

            DateTime result = DateTime.ParseExact(expectedStr, "u", null, DateTimeStyles.None);
            Assert.Equal(expectedStr, result.ToString("u"));
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider_DateTimeStyles_G()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("g");

            DateTime result = DateTime.ParseExact(expectedStr, "g", null, DateTimeStyles.AssumeUniversal);
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider_DateTimeStyles_O()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("o");

            DateTime result = DateTime.ParseExact(expectedStr, "o", null, DateTimeStyles.None);
            Assert.Equal(expectedStr, result.ToString("o"));
        }

        [Fact]
        public static void TestParseExact_String_String_FormatProvider_DateTimeStyles_CustomFormatProvider()
        {
            var formatter = new MyFormatter();
            string dateBefore = DateTime.Now.ToString();

            DateTime dateAfter = DateTime.ParseExact(dateBefore, "G", formatter, DateTimeStyles.AssumeUniversal);
            Assert.Equal(dateBefore, dateAfter.ToString());
        }

        [Fact]
        public static void TestParseExact_String_StringArray_FormatProvider_DateTimeStyles()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("g");

            var formats = new string[] { "g" };
            DateTime result = DateTime.ParseExact(expectedStr, formats, null, DateTimeStyles.AssumeUniversal);
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        [Fact]
        public static void TestTryParseExact_String_String_FormatProvider_DateTimeStyles_NullFormatProvider()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("g");

            DateTime resulted;
            Assert.True(DateTime.TryParseExact(expectedStr, "g", null, DateTimeStyles.AssumeUniversal, out resulted));
            Assert.Equal(expectedStr, resulted.ToString("g"));
        }

        [Fact]
        public static void TestTryParseExact_String_StringArray_FormatProvider_DateTimeStyles()
        {
            DateTime src = DateTime.MaxValue;
            string expectedStr = src.ToString("g");

            var formats = new string[] { "g" };
            DateTime result;
            Assert.True(DateTime.TryParseExact(expectedStr, formats, null, DateTimeStyles.AssumeUniversal, out result));
            Assert.Equal(expectedStr, result.ToString("g"));
        }

        [Theory]
        [InlineData("fi-FI")]
        [InlineData("nb-NO")]
        [InlineData("nb-SJ")]
        [InlineData("sr-Cyrl-XK")]
        [InlineData("sr-Latn-ME")]
        [InlineData("sr-Latn-RS")]
        [InlineData("sr-Latn-XK")]
        public static void TestParse_SpecialCultures(string cultureName)
        {
            // Test DateTime parsing with cultures which has the date separator and time separator are same

            CultureInfo ci;
            try
            {
                ci = new CultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                // Ignore un-supported culture in current platform
                return;
            }

            var dt = new DateTime(2015, 11, 20, 11, 49, 50);
            string dateString = dt.ToString(ci.DateTimeFormat.ShortDatePattern, ci);

            DateTime parsedDate;
            Assert.True(DateTime.TryParse(dateString, ci, DateTimeStyles.None, out parsedDate));
            if (ci.DateTimeFormat.ShortDatePattern.Contains("yyyy") || HasDifferentDateTimeSeparators(ci.DateTimeFormat))
            {
                Assert.Equal(dt.Date, parsedDate);
            }
            else
            {
                // When the date separator and time separator are the same, DateTime.TryParse cannot 
                // tell the difference between a short date like dd.MM.yy and a short time
                // like HH.mm.ss. So it assumes that if it gets 03.04.11, that must be a time
                // and uses the current date to construct the date time.
                DateTime now = DateTime.Now;
                Assert.Equal(new DateTime(now.Year, now.Month, now.Day, dt.Day, dt.Month, dt.Year % 100), parsedDate);
            }

            dateString = dt.ToString(ci.DateTimeFormat.LongDatePattern, ci);
            Assert.True(DateTime.TryParse(dateString, ci, DateTimeStyles.None, out parsedDate));
            Assert.Equal(dt.Date, parsedDate);

            dateString = dt.ToString(ci.DateTimeFormat.FullDateTimePattern, ci);
            Assert.True(DateTime.TryParse(dateString, ci, DateTimeStyles.None, out parsedDate));
            Assert.Equal(dt, parsedDate);

            dateString = dt.ToString(ci.DateTimeFormat.LongTimePattern, ci);
            Assert.True(DateTime.TryParse(dateString, ci, DateTimeStyles.None, out parsedDate));
            Assert.Equal(dt.TimeOfDay, parsedDate.TimeOfDay);
        }
        
        private static bool HasDifferentDateTimeSeparators(DateTimeFormatInfo dateTimeFormat)
        {
            // Since .NET Core doesn't expose DateTimeFormatInfo DateSeparator and TimeSeparator properties,
            // this method gets the separators using DateTime.ToString by passing in the invariant separators.
            // The invariant separators will then get turned into the culture's separators by ToString,
            // which are then compared.

            var dt = new DateTime(2015, 11, 24, 17, 57, 29);
            string separators = dt.ToString("/@:", dateTimeFormat);

            int delimiterIndex = separators.IndexOf('@');
            string dateSeparator = separators.Substring(0, delimiterIndex);
            string timeSeparator = separators.Substring(delimiterIndex + 1);
            return dateSeparator != timeSeparator;
        }

        [Fact]
        public static void TestGetDateTimeFormats()
        {
            var allStandardFormats = new char[]
            {
                'd', 'D', 'f', 'F', 'g', 'G',
                'm', 'M', 'o', 'O', 'r', 'R',
                's', 't', 'T', 'u', 'U', 'y', 'Y',
            };

            var dt = new DateTime(2009, 7, 28, 5, 23, 15);
            var formats = new List<string>();

            foreach (char format in allStandardFormats)
            {
                string[] dates = dt.GetDateTimeFormats(format);

                Assert.True(dates.Length > 0);

                DateTime parsedDate;
                Assert.True(DateTime.TryParseExact(dates[0], format.ToString(), CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedDate));

                formats.AddRange(dates);
            }

            List<string> actualFormats = dt.GetDateTimeFormats().ToList();
            Assert.Equal(formats.OrderBy(t => t), actualFormats.OrderBy(t => t));

            actualFormats = dt.GetDateTimeFormats(CultureInfo.CurrentCulture).ToList();
            Assert.Equal(formats.OrderBy(t => t), actualFormats.OrderBy(t => t));
        }

        [Fact]
        public static void TestGetDateTimeFormats_FormatSpecifier_InvalidFormat()
        {
            var dt = new DateTime(2009, 7, 28, 5, 23, 15);

            Assert.Throws<FormatException>(() => dt.GetDateTimeFormats('x'));
        }

        private static void VerifyDateTime(DateTime dt, int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
        {
            Assert.Equal(year, dt.Year);
            Assert.Equal(month, dt.Month);
            Assert.Equal(day, dt.Day);
            Assert.Equal(hour, dt.Hour);
            Assert.Equal(minute, dt.Minute);
            Assert.Equal(second, dt.Second);
            Assert.Equal(millisecond, dt.Millisecond);

            Assert.Equal(kind, dt.Kind);
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
