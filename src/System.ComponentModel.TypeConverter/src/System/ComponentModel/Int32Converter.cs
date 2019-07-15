// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert 32-bit signed integer objects to and
    /// from various other representations.
    /// </summary>
    public class Int32Converter : BaseNumberConverter
    {
        /// <summary>
        /// The Type this converter is targeting (e.g. Int16, UInt32, etc.)
        /// </summary>
        private protected override Type TargetType => typeof(int);

        /// <summary>
        /// Convert the given value to a string using the given radix
        /// </summary>
        private protected override object FromString(string value, int radix) => Convert.ToInt32(value, radix);

        /// <summary>
        /// Convert the given value to a string using the given formatInfo
        /// </summary>
        private protected override object FromString(string value, NumberFormatInfo formatInfo)
        {
            return int.Parse(value, NumberStyles.Integer, formatInfo);
        }

        /// <summary>
        /// Convert the given value from a string using the given formatInfo
        /// </summary>
        private protected override string ToString(object value, NumberFormatInfo formatInfo)
        {
            return ((int)value).ToString("G", formatInfo);
        }
    }
}
