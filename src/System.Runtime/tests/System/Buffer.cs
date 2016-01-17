// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;

using Xunit;

namespace System.Runtime.Tests
{
    public static class BufferTests
    {
        [Fact]
        public static void TestBlockCopy()
        {
            byte[] b1 = { 0x1a, 0x2b, 0x3c, 0x4d };
            byte[] b2 = new byte[6];
            for (int i = 0; i < b2.Length; i++)
                b2[i] = 0x6f;

            Buffer.BlockCopy(b1, 0, b2, 1, 4);
            Assert.Equal(b2[0], 0x6f);
            Assert.Equal(b2[1], 0x1a);
            Assert.Equal(b2[2], 0x2b);
            Assert.Equal(b2[3], 0x3c);
            Assert.Equal(b2[4], 0x4d);
            Assert.Equal(b2[5], 0x6f);

            b2 = new byte[] { 0x1a, 0x2b, 0x3c, 0x4d, 0x5e };
            Buffer.BlockCopy(b2, 1, b2, 2, 2);
            Assert.Equal(b2[0], 0x1a);
            Assert.Equal(b2[1], 0x2b);
            Assert.Equal(b2[2], 0x2b);
            Assert.Equal(b2[3], 0x3c);
            Assert.Equal(b2[4], 0x5e);
        }

        [Fact]
        public static void TestBlockCopy_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Buffer.BlockCopy(null, 0, new int[3], 0, 0)); // Src is null
            Assert.Throws<ArgumentNullException>(() => Buffer.BlockCopy(new string[3], 0, null, 0, 0)); // Dst is null
            
            Assert.Throws<ArgumentOutOfRangeException>(() => Buffer.BlockCopy(new byte[3], -1, new byte[3], 0, 0)); // SrcOffset < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => Buffer.BlockCopy(new byte[3], 0, new byte[3], -1, 0)); // DstOffset < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => Buffer.BlockCopy(new byte[3], 0, new byte[3], 0, -1)); // Count < 0

            Assert.Throws<ArgumentException>(() => Buffer.BlockCopy(new string[3], 0, new byte[3], 0, 0)); // Src is not a byte array
            Assert.Throws<ArgumentException>(() => Buffer.BlockCopy(new byte[3], 0, new string[3], 0, 0)); // Dst is not a byte array

            Assert.Throws<ArgumentException>(() => Buffer.BlockCopy(new byte[3], 3, new byte[3], 0, 1)); // SrcOffset + count >= src.length
            Assert.Throws<ArgumentException>(() => Buffer.BlockCopy(new byte[3], 4, new byte[3], 0, 0)); // SrcOffset >= src.Length

            Assert.Throws<ArgumentException>(() => Buffer.BlockCopy(new byte[3], 0, new byte[3], 3, 1)); // DstOffset + count >= dst.Length
            Assert.Throws<ArgumentException>(() => Buffer.BlockCopy(new byte[3], 0, new byte[3], 4, 0)); // DstOffset >= dst.Length
        }

        public static unsafe IEnumerable<object[]> ByteLengthTestData
        {
            get
            {
                return new[]
                {
                   new object[] {typeof(byte), sizeof(byte) },
                   new object[] {typeof(sbyte), sizeof(sbyte) },
                   new object[] {typeof(short), sizeof(short) },
                   new object[] {typeof(ushort), sizeof(ushort) },
                   new object[] {typeof(int), sizeof(int) },
                   new object[] {typeof(uint), sizeof(uint) },
                   new object[] {typeof(long), sizeof(long) },
                   new object[] {typeof(ulong), sizeof(ulong) },
                   new object[] {typeof(IntPtr), sizeof(IntPtr) },
                   new object[] {typeof(UIntPtr), sizeof(UIntPtr) },
                   new object[] {typeof(double), sizeof(double) },
                   new object[] {typeof(float), sizeof(float) },
                   new object[] {typeof(bool), sizeof(bool) },
                   new object[] {typeof(char), sizeof(char) },
                   new object[] {typeof(decimal), sizeof(decimal) },
                   new object[] {typeof(DateTime), sizeof(DateTime) },
                   new object[] {typeof(string), -1 },
               };
            }
        }

        [Theory, MemberData("ByteLengthTestData")]
        public static void TestByteLength(Type type, int size)
        {
            const int length = 25;
            Array array = Array.CreateInstance(type, length);
            if (type.GetTypeInfo().IsPrimitive)
            {
                Assert.Equal(length * size, Buffer.ByteLength(array));
            }
            else
            {
                Assert.Throws<ArgumentException>(() => Buffer.ByteLength(array));
            }
        }

        [Fact]
        public static void TestByteLength_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Buffer.ByteLength(null)); // Array is null
        }

        [Fact]
        public static void TestGetByte()
        {
            var array = new uint[] { 0x01234567, 0x89abcdef };

            Assert.Equal(0x67, Buffer.GetByte(array, 0));
            Assert.Equal(0x89, Buffer.GetByte(array, 7));
        }

        [Fact]
        public static void TestGetByte_Invalid()
        {
            var array = new uint[] { 0x01234567, 0x89abcdef };

            Assert.Throws<ArgumentNullException>(() => Buffer.GetByte(null, 0)); // Array is null
            Assert.Throws<ArgumentException>(() => Buffer.GetByte(new object[10], 0)); // Array is not a primitive array

            Assert.Throws<ArgumentOutOfRangeException>(() => Buffer.GetByte(array, -1)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => Buffer.GetByte(array, 8)); // Index >= array.Length
        }

        [Fact]
        public static void TestSetByte()
        {
            uint[] array = { 0x01234567, 0x89abcdef };
                        
            Buffer.SetByte(array, 0, 0x42);
            Assert.Equal(array[0], (uint)0x01234542);
            Assert.Equal(array[1], 0x89abcdef);

            Buffer.SetByte(array, 7, 0xa2);
            Assert.Equal(array[0], (uint)0x01234542);
            Assert.Equal(array[1], 0xa2abcdef);
        }

        [Fact]
        public static void TestSetByte_Invalid()
        {
            var array = new uint[] { 0x01234567, 0x89abcdef };

            Assert.Throws<ArgumentNullException>(() => Buffer.SetByte(null, 0, 0xff)); // Array is null
            Assert.Throws<ArgumentException>(() => Buffer.SetByte(new object[10], 0, 0xff)); // Array is not a primitive array

            Assert.Throws<ArgumentOutOfRangeException>(() => Buffer.SetByte(array, -1, 0xff)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => Buffer.SetByte(array, 8, 0xff)); // Index > array.Length
        }
    }
}
