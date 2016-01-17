// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class EmptyTests
    {
        [Fact]
        public static void TestEmpty()
        {
            Assert.True(Array.Empty<int>() != null);
            Assert.Equal(0, Array.Empty<int>().Length);
            Assert.Equal(1, Array.Empty<int>().Rank);
            Assert.Equal(Array.Empty<int>(), Array.Empty<int>());

            Assert.True(Array.Empty<object>() != null);
            Assert.Equal(0, Array.Empty<object>().Length);
            Assert.Equal(1, Array.Empty<object>().Rank);
            Assert.Equal(Array.Empty<object>(), Array.Empty<object>());
        }

        [Fact]
        public static void TestEmptyTypes()
        {
            Assert.NotNull(Type.EmptyTypes);
            Assert.Same(Type.EmptyTypes, Type.EmptyTypes);
        }

        [Fact]
        public static void TestMissing()
        {
            Assert.NotNull(Type.Missing);
            Assert.Same(Type.Missing, Type.Missing);
        }
    }
}
