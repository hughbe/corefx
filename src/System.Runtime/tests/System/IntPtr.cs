// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Runtime.Tests
{
    public static class IntPtrTests
    {

        private static unsafe bool s_runTests = sizeof(void*) != 4; // Skip IntPtr tests on 32-bit platforms

        [Fact]
        public static unsafe void TestSize()
        {
            if (!s_runTests)
                return;

            Assert.Equal(sizeof(void*), IntPtr.Size);
        }

        [Fact]
        public static void TestZero()
        {
            if (!s_runTests)
                return;

            TestPointer(IntPtr.Zero, 0);
        }

        [Fact]
        public static void TestCtor_Int()
        {
            if (!s_runTests)
                return;

            int i = 42;
            TestPointer(new IntPtr(i), i);
            TestPointer((IntPtr)i, i);

            i = -1;
            TestPointer(new IntPtr(i), i);
            TestPointer((IntPtr)i, i);
        }

        [Fact]
        public static void TestCtor_Long()
        {
            if (!s_runTests)
                return;

            long l = 0x0fffffffffffffff;
            TestPointer(new IntPtr(l), l);
            TestPointer((IntPtr)l, l);
        }

        [Fact]
        public static unsafe void TestCtor_VoidPointer_ToPointer()
        {
            if (!s_runTests)
                return;

            void* pv = new IntPtr(42).ToPointer();
            TestPointer(new IntPtr(pv), 42);
            TestPointer((IntPtr)pv, 42);
        }

        [Fact]
        public static void TestAdd()
        {
            if (!s_runTests)
                return;

            IntPtr p = IntPtr.Add(new IntPtr(42), 5);
            TestPointer(p, 42 + 5);

            p = new IntPtr(40) + 2;
            Assert.Equal(new IntPtr(42), p);

            // Add is spected NOT to generate an OverflowException
            p = IntPtr.Add(new IntPtr(0x7fffffffffffffff), 5);
            unchecked
            {
                TestPointer(p, (long)0x8000000000000004);
            }
        }

        [Fact]
        public static void TestSubtract()
        {
            if (!s_runTests)
                return;

            IntPtr p = IntPtr.Subtract(new IntPtr(42), 5);
            TestPointer(p, 42 - 5);

            p = new IntPtr(44) - 2;
            Assert.Equal(new IntPtr(42), p);
        }

        [Fact]
        public static void TestEquals()
        {
            if (!s_runTests)
                return;

            var p1 = new IntPtr(42);
            var p2 = new IntPtr(42);

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

            var p = new IntPtr(42);

            uint i = (uint)p;
            ulong l = (ulong)p;
            void* v = p.ToPointer();

            Assert.Equal(42u, i);
            Assert.Equal(42u, l);

            Assert.Equal(p, (IntPtr)i);
            Assert.Equal(p, (IntPtr)l);
            Assert.Equal(p, (IntPtr)v);

            p = new IntPtr(0x7fffffffffffffff);
            Assert.Throws<OverflowException>(() => (int)p);
        }
        
        private static void TestPointer(IntPtr p, long expected)
        {
            Assert.Equal(expected, p.ToInt64());

            int expected32 = (int)expected;
            if (expected32 != expected)
            {
                Assert.Throws<OverflowException>(() => p.ToInt32());
                return;
            }
            
            int i = p.ToInt32();
            Assert.Equal(expected32, p.ToInt32());
            
            Assert.Equal(expected.ToString(), p.ToString());
            Assert.Equal(expected.ToString("x"), p.ToString("x"));
            
            Assert.Equal(p, new IntPtr(expected));
            Assert.True(p == new IntPtr(expected));
            Assert.False(p != new IntPtr(expected));

            Assert.NotEqual(p, new IntPtr(expected + 1));
            Assert.False(p == new IntPtr(expected + 1));
            Assert.True(p != new IntPtr(expected + 1));
        }
    }
}
