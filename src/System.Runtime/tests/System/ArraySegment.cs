// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using Xunit;

namespace System.Runtime.Tests
{
    public static class ArraySegmentTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var seg = new ArraySegment<int>();
            Assert.Null(seg.Array);
            Assert.Equal(0, seg.Offset);
            Assert.Equal(0, seg.Count);
        }

        [Fact]
        public static void TestCtor_Array()
        {
            var intArray = new int[] { 7, 8, 9, 10, 11 };
            var seg = new ArraySegment<int>(intArray);

            Assert.Same(intArray, seg.Array);
            Assert.Equal(0, seg.Offset);
            Assert.Equal(5, seg.Count);
        }

        [Fact]
        public static void TestCtor_Array_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => new ArraySegment<int>(null, 0, 0)); // Array is null
        }

        [Fact]
        public static void TestCtor_Array_Int_Int()
        {
            var intArray = new int[] { 7, 8, 9, 10, 11 };
            var seg = new ArraySegment<int>(intArray, 2, 3);

            Assert.Same(intArray, seg.Array);
            Assert.Equal(2, seg.Offset);
            Assert.Equal(3, seg.Count);
        }

        [Fact]
        public static void TestCtor_Array_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => new ArraySegment<int>(null, 0, 0)); // Array is null

            Assert.Throws<ArgumentOutOfRangeException>(() => new ArraySegment<int>(new int[10], -1, 0)); // Offset < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => new ArraySegment<int>(new int[10], 0, -1)); // Count < 0
            
            Assert.Throws<ArgumentException>(() => new ArraySegment<int>(new int[10], 10, 1)); // Offset + count > array.Length
            Assert.Throws<ArgumentException>(() => new ArraySegment<int>(new int[10], 9, 2)); // Offset + count > array.Length
        }

        [Fact]
        public static void TestEquals()
        {
            var intArray = new int[] { 7, 8, 9, 10, 11, 12 };
            var seg1 = new ArraySegment<int>(intArray, 2, 3);
            var seg2 = new ArraySegment<int>(intArray, 2, 3);
            var seg3 = new ArraySegment<int>(intArray, 3, 3);
            var seg4 = new ArraySegment<int>(intArray, 2, 4);

            Assert.True(seg1.Equals(seg1));
            Assert.Equal(seg1.GetHashCode(), seg1.GetHashCode());

            Assert.True(seg1.Equals(seg2));
            Assert.Equal(seg1.GetHashCode(), seg2.GetHashCode());

            Assert.False(seg1.Equals(seg3));
            Assert.NotEqual(seg1.GetHashCode(), seg3.GetHashCode());

            Assert.False(seg1.Equals(seg4));
            Assert.NotEqual(seg1.GetHashCode(), seg4.GetHashCode());

            Assert.False(seg1.Equals(null));
        }

        [Fact]
        public static void TestIList()
        {
            int[] intArray = { 7, 8, 9, 10, 11, 12, 13 };
            ArraySegment<int> seg = new ArraySegment<int>(intArray, 2, 3);
            IList<int> iList = seg;

            Assert.Equal(3, iList.Count);

            Assert.True(iList.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => iList.Add(2));
            Assert.Throws<NotSupportedException>(() => iList.Clear());
            Assert.Throws<NotSupportedException>(() => iList.Remove(2));
            Assert.Throws<NotSupportedException>(() => iList.RemoveAt(2));

            Assert.Equal(10, iList[1]);
            iList[1] = 99;
            Assert.Equal(99, iList[1]);

            Assert.True(iList.Contains(11));
            Assert.Equal(2, iList.IndexOf(11));

            Assert.False(iList.Contains(8788));
            Assert.Equal(-1, iList.IndexOf(8788));

            var dst = new int[10];
            iList.CopyTo(dst, 5);
            Assert.Equal(new int[] { 0, 0, 0, 0, 0, 9, 99, 11, 0, 0 }, dst);

            IEnumerator<int> enumerator = iList.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (enumerator.MoveNext())
                {
                    Assert.Equal(intArray[counter + 2], iList[counter]);
                    counter++;
                }
                Assert.Equal(iList.Count, counter);

                enumerator.Reset();
            }
        }

        [Fact]
        public static void TestCopyTo()
        {
            string[] stringArray = new string[] { "0", "1", "2", "3", "4" }; ;
            IList<string> stringSeg = new ArraySegment<string>(stringArray, 1, 3);
            stringSeg.CopyTo(stringArray, 2);
            Assert.Equal(new string[] { "0", "1", "1", "2", "3" }, stringArray);

            stringArray = new string[] { "0", "1", "2", "3", "4" };
            stringSeg = new ArraySegment<string>(stringArray, 1, 3);
            stringSeg.CopyTo(stringArray, 0);
            Assert.Equal(stringArray, new string[] { "1", "2", "3", "3", "4" });

            var intArray = new int[] { 0, 1, 2, 3, 4 };
            IList<int> intSeg = new ArraySegment<int>(intArray, 1, 3);
            intSeg.CopyTo(intArray, 2);
            Assert.Equal(intArray, new int[] { 0, 1, 1, 2, 3 });

            intArray = new int[] { 0, 1, 2, 3, 4 };
            intSeg = new ArraySegment<int>(intArray, 1, 3);
            intSeg.CopyTo(intArray, 0);
            Assert.Equal(intArray, new int[] { 1, 2, 3, 3, 4 });
        }
    }
}
