// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

using Xunit;

namespace System.IO.Tests
{
    public static class PathTooLongExceptionInteropTests
    {
        [Fact]
        public static void TestFrom_HR()
        {
            int hr = HResults.COR_E_PATHTOOLONG;
            PathTooLongException exception = Marshal.GetExceptionForHR(hr) as PathTooLongException;
            Assert.NotNull(exception);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: hr, validateMessage: false);
        }
    }
}
