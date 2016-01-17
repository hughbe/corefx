// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.IO.Tests
{
    public static class DirectoryNotFoundExceptionTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var excpetion = new DirectoryNotFoundException();
            ExceptionUtility.ValidateExceptionProperties(excpetion, hResult: HResults.COR_E_DIRECTORYNOTFOUND, validateMessage: false);
        }

        [Fact]
        public static void TestCtor_Message()
        {
            string message = "That page was missing from the directory.";
            var exception = new DirectoryNotFoundException(message);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: HResults.COR_E_DIRECTORYNOTFOUND, message: message);
        }

        [Fact]
        public static void TestCtor_Message_InnerException()
        {
            string message = "That page was missing from the directory.";
            var innerException = new Exception("Inner exception");
            var exception = new DirectoryNotFoundException(message, innerException);
            ExceptionUtility.ValidateExceptionProperties(exception, hResult: HResults.COR_E_DIRECTORYNOTFOUND, innerException: innerException, message: message);
        }
    }
}
