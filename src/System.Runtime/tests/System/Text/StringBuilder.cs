// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

using Xunit;

namespace System.Runtime.Tests
{
    public static class StringBuilderTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var sb = new StringBuilder();
            Assert.Equal("", sb.ToString());
            Assert.Equal(0, sb.Length);
        }

        [Fact]
        public static void TestCtor_Int()
        {
            var sb = new StringBuilder(42);
            Assert.Equal("", sb.ToString());
            Assert.Equal(0, sb.Length);

            Assert.True(sb.Capacity >= 42);
        }

        [Fact]
        public static void TestCtor_Int_Int()
        {
            // The second int parameter is MaxCapacity but in CLR4.0 and later, StringBuilder isn't required to honor it.
            var sb = new StringBuilder(42, 50);
            Assert.Equal("", sb.ToString());
            Assert.Equal(0, sb.Length);

            Assert.True(sb.Capacity >= 42);
            Assert.True(sb.Capacity < sb.MaxCapacity);
            Assert.Equal(50, sb.MaxCapacity);
        }

        [Fact]
        public static void TestCtor_String()
        {
            var sb = new StringBuilder("Hello");
            Assert.Equal("Hello", sb.ToString());
            Assert.Equal(5, sb.Length);
        }

        [Fact]
        public static void TestCtor_String_Int()
        {
            var sb = new StringBuilder("Hello", 42);
            Assert.Equal("Hello", sb.ToString());
            Assert.Equal(5, sb.Length);

            Assert.True(sb.Capacity >= 42);
        }

        [Fact]
        public static void TestCtor_String_Int_Int_Int()
        {
            var sb = new StringBuilder("Hello", 2, 3, 42);
            Assert.Equal("llo", sb.ToString());
            Assert.Equal(3, sb.Length);

            Assert.True(sb.Capacity >= 42);
        }

        [Fact]
        public static void TestToString()
        {
            StringBuilder sb = new StringBuilder("Finally");
            Assert.Equal("Finally", sb.ToString());
            Assert.Equal("nal", sb.ToString(2, 3));
        }

        [Fact]
        public static void TestReplace()
        {
            var sb = new StringBuilder("aaaabbbbccccdddd");
            sb.Replace('a', '!', 2, 3);
            Assert.Equal("aa!!bbbbccccdddd", sb.ToString());

            sb = new StringBuilder("aaaabbbbccccdddd");
            sb.Replace("a", "$!", 2, 3);
            Assert.Equal("aa$!$!bbbbccccdddd", sb.ToString());
        }

        [Fact]
        public static void TestRemove()
        {
            var sb = new StringBuilder("Almost");
            sb.Remove(1, 3);
            Assert.Equal("Ast", sb.ToString());
        }

        [Fact]
        public static void TestInsert()
        {
            //@todo: Not testing all the Insert() overloads that just call ToString() on the input and forward to Insert(int, String).
            var sb = new StringBuilder("Hello");
            sb.Insert(2, "!!");
            Assert.Equal("He!!llo", sb.ToString());
        }

        [Fact]
        public static void TestEquals()
        {
            var sb1 = new StringBuilder("Hello");
            var sb2 = new StringBuilder("Hello");
            var sb3 = new StringBuilder("HelloX");

            Assert.True(sb1.Equals(sb1));
            Assert.True(sb1.Equals(sb2));
            Assert.False(sb1.Equals(sb3));
        }

        [Fact]
        public static void TestCopyTo()
        {
            var sb = new StringBuilder("Hello");
            char[] ca = new char[10];
            sb.CopyTo(1, ca, 5, 4);

            Assert.Equal(ca[0], 0);
            Assert.Equal(ca[1], 0);
            Assert.Equal(ca[2], 0);
            Assert.Equal(ca[3], 0);
            Assert.Equal(ca[4], 0);
            Assert.Equal(ca[5], 'e');
            Assert.Equal(ca[6], 'l');
            Assert.Equal(ca[7], 'l');
            Assert.Equal(ca[8], 'o');
            Assert.Equal(ca[9], 0);
        }

        [Fact]
        public static void TestClear()
        {
            var sb = new StringBuilder("Hello");
            sb.Clear();
            Assert.Equal("", sb.ToString());
        }

        [Fact]
        public static void TestLength()
        {
            var sb = new StringBuilder("Hello");
            Assert.Equal(5, 5);

            sb.Length = 2;
            Assert.Equal(2, sb.Length);
            Assert.Equal("He", sb.ToString());
            
            sb.Length = 10;
            Assert.Equal(10, sb.Length);
            Assert.Equal("He" + new string((char)0, 8), sb.ToString());
        }
        
        [Fact]
        public static void TestAppend()
        {
            //@todo: Skipped all the Append overloads that just call ToString() on the argument and delegate to Append(String)
            var sb = new StringBuilder();
            string s = "";
            for (int i = 0; i < 500; i++)
            {
                char c = (char)(0x41 + (i % 10));
                sb.Append(c);
                s += c;
                Assert.Equal(s, sb.ToString());
            }

            sb = new StringBuilder();
            s = "";
            for (int i = 0; i < 500; i++)
            {
                char c = (char)(0x41 + (i % 10));
                int repeat = i % 8;
                sb.Append(c, repeat);
                s += new string(c, repeat);
                Assert.Equal(s, sb.ToString());
            }

            sb = new StringBuilder();
            s = "";
            for (int i = 0; i < 500; i++)
            {
                char c = (char)(0x41 + (i % 10));
                int repeat = i % 8;
                char[] ca = new char[repeat];
                for (int j = 0; j < ca.Length; j++)
                    ca[j] = c;
                sb.Append(ca);
                s += new string(ca);
                Assert.Equal(s, sb.ToString());
            }

            sb = new StringBuilder();
            s = "";
            for (int i = 0; i < 500; i++)
            {
                sb.Append("ab");
                s += "ab";
                Assert.Equal(s, sb.ToString());
            }

            sb = new StringBuilder();
            s = "Hello";
            sb.Append(s, 2, 3);
            Assert.Equal(sb.ToString(), "llo");
        }
        
        [Fact]
        public unsafe static void TestAppendUsingNativePointers()
        {
            StringBuilder sb = new StringBuilder();
            string s = "abc ";
            fixed (char* value = s)
            {
                sb.Append(value, s.Length);
                sb.Append(value, s.Length);
            }
            Assert.Equal("abc abc ", sb.ToString());
        }

        [Fact]
        public unsafe static void TestAppendUsingNativePointerExceptions()
        {
            StringBuilder sb = new StringBuilder();
            string s = "abc ";
            fixed (char* value = s)
            {
                Assert.Throws<NullReferenceException>(() => sb.Append(null, s.Length));

                char* valuePointer = value; // Cannot use char* directly inside an anonymous method 
                Assert.Throws<ArgumentOutOfRangeException>(() => sb.Append(valuePointer, -1));
            }
        }

        [Fact]
        public static void TestAppendFormat()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Foo {0} Bar {1}", "Red", "Green");
            Assert.Equal("Foo Red Bar Green", sb.ToString());
        }

        [Fact]
        public static void TestChars()
        {
            var sb = new StringBuilder("Hello");
            Assert.Equal('e', sb[1]);

            sb[1] = 'X';
            Assert.Equal('X', sb[1]);
            Assert.Equal("HXllo", sb.ToString());
        }
    }
}
