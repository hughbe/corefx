// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class ValueTypeTests
    {
        [Fact]
        public static void TestToString()
        {
            object o = new S();
            string s = o.ToString();
            Assert.NotNull(s);

            string s1 = o.GetType().ToString();
            Assert.Equal(s, s1);
            Assert.Equal("System.Runtime.Tests.ValueTypeTests+S", s);
        }

        public struct S
        {
            public int x;
            public int y;
        }
    }
}
