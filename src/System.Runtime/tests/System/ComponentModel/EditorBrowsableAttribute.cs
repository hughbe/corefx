// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.ComponentModel.Tests
{
    public static class EditorBrowsableAttributeTests
    {
        [Theory]
        [InlineData(EditorBrowsableState.Advanced)]
        [InlineData(EditorBrowsableState.Always)]
        [InlineData(EditorBrowsableState.Never)]
        [InlineData((EditorBrowsableState)12345)]
        public static void TestCtor_EditableBrowserState(EditorBrowsableState state)
        {
            Assert.Equal(state, new EditorBrowsableAttribute(state).State);
        }

        [Theory]
        [InlineData(EditorBrowsableState.Advanced, EditorBrowsableState.Advanced, true)]
        [InlineData(EditorBrowsableState.Always, EditorBrowsableState.Always, true)]
        [InlineData(EditorBrowsableState.Never, EditorBrowsableState.Never, true)]
        [InlineData((EditorBrowsableState)12345, (EditorBrowsableState)12345, true)]
        [InlineData(EditorBrowsableState.Advanced, EditorBrowsableState.Always, false)]
        [InlineData(EditorBrowsableState.Advanced, EditorBrowsableState.Never, false)]
        public static void TestEqual(EditorBrowsableState state1, EditorBrowsableState state2, bool equal)
        {
            var attr1 = new EditorBrowsableAttribute(state1);
            var attr2 = new EditorBrowsableAttribute(state2);

            Assert.Equal(attr1, attr1);
            Assert.Equal(equal, attr1.Equals(attr2));
            Assert.Equal(equal, attr2.Equals(attr1));
            Assert.Equal(equal, attr1.GetHashCode().Equals(attr2.GetHashCode()));
        }

        [Fact]
        public static void TestEqualNull()
        {
            Assert.False(new EditorBrowsableAttribute(EditorBrowsableState.Advanced).Equals(null));
        }

        [Fact]
        public static void TestEnumValues()
        {
            Assert.Equal(0, (int)EditorBrowsableState.Always);
            Assert.Equal(1, (int)EditorBrowsableState.Never);
            Assert.Equal(2, (int)EditorBrowsableState.Advanced);
        }
    }
}
