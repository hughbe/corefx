// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class UIntPtrTests
    {
        private static unsafe bool s_runTests = sizeof(void*) != 4; // Skip UIntPtr tests on 32-bit platforms

        [Fact]
        public static unsafe void TestSize()
        {
            if (!s_runTests)
                return;

            Assert.Equal(sizeof(void*), UIntPtr.Size);
        }

        [Fact]
        public static void TestZero()
        {
            if (!s_runTests)
                return;

            TestPointer(UIntPtr.Zero, 0);
        }

        [Fact]
        public static void TestCtor_UInt()
        {
            if (!s_runTests)
                return;

            uint i = 42;
            TestPointer(new UIntPtr(i), i);
            TestPointer((UIntPtr)i, i);
        }

        [Fact]
        public static void TestCtor_ULong()
        {
            if (!s_runTests)
                return;

            ulong l = 0x0fffffffffffffff;
            TestPointer(new UIntPtr(l), l);
            TestPointer((UIntPtr)l, l);
        }

        [Fact]
        public static unsafe void TestCtor_VoidPointer_ToPointer()
        {
            if (!s_runTests)
                return;

            void* pv = new UIntPtr(42).ToPointer();
            TestPointer(new UIntPtr(pv), 42);
            TestPointer((UIntPtr)pv, 42);
        }

        [Fact]
        public static void TestAdd()
        {
            if (!s_runTests)
                return;

            UIntPtr p = UIntPtr.Add(new UIntPtr(42), 5);
            TestPointer(p, 42 + 5);

            p = new UIntPtr(40) + 2;
            Assert.Equal(new UIntPtr(42), p);

            // Add is spected NOT to generate an OverflowException
            p = UIntPtr.Add(new UIntPtr(0xffffffffffffffff), 5);
            unchecked
            {
                TestPointer(p, 0x0000000000000004);
            }
        }

        [Fact]
        public static void TestSubtract()
        {
            if (!s_runTests)
                return;

            UIntPtr p = UIntPtr.Subtract(new UIntPtr(42), 5);
            TestPointer(p, 42 - 5);

            p = new UIntPtr(44) - 2;
            Assert.Equal(new UIntPtr(42), p);
        }

        [Fact]
        public static void TestEquals()
        {
            if (!s_runTests)
                return;

            var p1 = new UIntPtr(42);
            var p2 = new UIntPtr(42);

            Assert.True(p1.Equals(p1));
            Assert.Equal(p1.GetHashCode(), p1.GetHashCode());

            Assert.True(p1.Equals(p2));
            Assert.Equal(p1.GetHashCode(), p2.GetHashCode());

            Assert.False(p1.Equals(42));
            Assert.False(p1.Equals(null));

            int h = p1.GetHashCode();
            int h2 = p1.GetHashCode();
            Assert.Equal(h, h2);
        }

        [Fact]
        public static unsafe void TestImplicitCast()
        {
            if (!s_runTests)
                return;

            var p = new UIntPtr(42);
            
            uint i = (uint)p;
            ulong l = (ulong)p;
            void* v = p.ToPointer();

            Assert.Equal(42u, i);
            Assert.Equal(42u, l);

            Assert.Equal(p, (UIntPtr)i);
            Assert.Equal(p, (UIntPtr)l);
            Assert.Equal(p, (UIntPtr)v);

            p = new UIntPtr(0x7fffffffffffffff);
            Assert.Throws<OverflowException>(() => (uint)p);
        }

        private static void TestPointer(UIntPtr p, ulong expected)
        {
            Assert.Equal(expected, p.ToUInt64());

            uint expected32 = (uint)expected;
            if (expected32 != expected)
            {
                Assert.Throws<OverflowException>(() => p.ToUInt32());
                return;
            }

            Assert.Equal(expected32, p.ToUInt32());
            
            Assert.Equal(expected.ToString(), p.ToString());

            Assert.Equal(p, new UIntPtr(expected));
            Assert.True(p == new UIntPtr(expected));
            Assert.False(p != new UIntPtr(expected));

            Assert.NotEqual(p, new UIntPtr(expected + 1));
            Assert.False(p == new UIntPtr(expected + 1));
            Assert.True(p != new UIntPtr(expected + 1));
        }
    }
}
