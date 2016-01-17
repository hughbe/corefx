// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace System.Runtime.Tests
{
    public static class ArrayTests
    {
        [Fact]
        public static void TestConstruction()
        {
            // Check a number of the simple APIs on Array for dimensions up to 4.
            Array a = new int[] { 1, 2, 3 };
            VerifyArray(a, 1, new int[] { 3 }, new int[] { 0 }, new int[] { 2 }, true);

            a = new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            VerifyArray(a, 2, new int[] { 2, 3 }, new int[] { 0, 0 }, new int[] { 1, 2 }, true);

            a = new int[2, 3, 4];
            VerifyArray(a, 3, new int[] { 2, 3, 4 }, new int[] { 0, 0, 0 }, new int[] { 1, 2, 3 }, true);

            a = new int[2, 3, 4, 5];
            VerifyArray(a, 4, new int[] { 2, 3, 4, 5 }, new int[] { 0, 0, 0, 0 }, new int[] { 1, 2, 3, 4 }, true);
        }

        [Fact]
        public static void TestConstructionMultidimArrays()
        {
            // This C# initialization syntax generates some peculiar looking IL.
            // Initializations of this form are handled specially on Desktop and
            // in .NET Native by UTC.
            int[,,,] array = new int[,,,] { { { { 1, 2, 3 }, { 1, 2, 3 } }, { { 1, 2, 3 }, { 1, 2, 3 } } },
                                        { { { 1, 2, 3 }, { 1, 2, 3 } }, { { 1, 2, 3 }, { 1, 2, 3 } } } };
            Assert.NotNull(array);
            Assert.Equal(array.GetValue(0, 0, 0, 0), 1);
            Assert.Equal(array.GetValue(0, 0, 0, 1), 2);
            Assert.Equal(array.GetValue(0, 0, 0, 2), 3);
        }

        [Fact]
        public static void TestCastAsIListOfT()
        {
            var sa = new string[] { "Hello", "There" };
            Assert.True(sa is IList<string>);

            IList<string> ils = sa;
            Assert.Equal(2, ils.Count);
                        
            Assert.True(ils.Contains("There"));
            Assert.False(ils.Contains(null));
            Assert.Equal(1, ils.IndexOf("There"));
            Assert.Equal(-1, ils.IndexOf(null));

            var sa2 = new string[2];
            ils.CopyTo(sa2, 0);
            Assert.Equal(sa, sa2);
            
            var ia1 = new int[] { 1, 2, 3, 4 };
            var dst = new int[4];
            ((IList<int>)ia1).CopyTo(dst, 0);
            Assert.Equal(dst, ia1);

            ia1 = new int[] { 1, 2, 3, 4 };
            dst = new int[6];
            ((IList<int>)ia1).CopyTo(dst, 1);
            Assert.Equal(new int[] { 0, 1, 2, 3, 4, 0 }, dst);
            
            Assert.True(ils.IsReadOnly);

            Assert.Throws<NotSupportedException>(() => ils.Add("Hi"));
            Assert.Throws<NotSupportedException>(() => ils.Clear());
            Assert.Throws<NotSupportedException>(() => ils.Remove("There"));
            Assert.Throws<NotSupportedException>(() => ils.RemoveAt(1));
            Assert.Throws<NotSupportedException>(() => ils.Insert(0, "x"));

            IEnumerator<string> e = ils.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (e.MoveNext())
                {
                    Assert.Equal(sa[counter], e.Current);
                    counter++;
                }
                Assert.Equal(sa.Length, counter);

                e.Reset();
            }

            ils[0] = "50";
            Assert.Equal("50", sa[0]);
            Assert.Equal(sa[1], ils[1]);
            Assert.Equal("There", sa[1]);
        }

        public static IEnumerable<object[]> BinarySearchTestData
        {
            get
            {
                int[] intArray = { 1, 3, 6, 6, 8, 10, 12, 16 };
                IComparer intComparer = new IntegerComparer();
                IComparer<int> intGenericComparer = new IntegerComparer();

                string[] strArray = { null, "aa", "bb", "bb", "cc", "dd", "ee" };
                IComparer strComparer = new StringComparer();
                IComparer<string> strGenericComparer = new StringComparer();

                return new[]
                {
                   new object[] { intArray, 8, intComparer, intGenericComparer, new Func<int, bool>(i => i == 4) },
                   new object[] { intArray, 99, intComparer, intGenericComparer, new Func<int, bool>(i => i == ~(intArray.Length))  },
                   new object[] { intArray, 6, intComparer, intGenericComparer, new Func<int, bool>(i => i == 2 || i == 3)  },
                   new object[] { strArray, "bb", strComparer, strGenericComparer, new Func<int, bool>(i => i == 2 || i == 3)  },
                   new object[] { strArray, null, strComparer, null, new Func<int, bool>(i => i == 0)  },
               };
            }
        }

        [Theory, MemberData("BinarySearchTestData")]
        public static void TestBinarySearch<T>(T[] array, T value, IComparer comparer, IComparer<T> genericComparer, Func<int, bool> verifier)
        {
            int idx = Array.BinarySearch(array, value, comparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, value, genericComparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, value);
            Assert.True(verifier(idx));
        }

        public static IEnumerable<object[]> BinarySearchTestDataInRange
        {
            get
            {
                int[] intArray = { 1, 3, 6, 6, 8, 10, 12, 16 };
                IComparer intComparer = new IntegerComparer();
                IComparer<int> intGenericComparer = new IntegerComparer();

                string[] strArray = { null, "aa", "bb", "bb", "cc", "dd", "ee" };
                IComparer strComparer = new StringComparer();
                IComparer<string> strGenericComparer = new StringComparer();

                return new[]
                {
                   new object[] { intArray, 0, 8, 99, intComparer, intGenericComparer, new Func<int, bool>(i => i == ~(intArray.Length))  },
                   new object[] { intArray, 0, 8, 6, intComparer, intGenericComparer, new Func<int, bool>(i => i == 2 || i == 3)  },
                   new object[] { intArray, 1, 5, 16, intComparer, intGenericComparer, new Func<int, bool>(i => i == -7)  },
                   new object[] { strArray, 0, strArray.Length, "bb", strComparer, strGenericComparer, new Func<int, bool>(i => i == 2 || i == 3)  },
                   new object[] { strArray, 3, 4, "bb", strComparer, strGenericComparer, new Func<int, bool>(i => i == 3)  },
                   new object[] { strArray, 4, 3, "bb", strComparer, strGenericComparer, new Func<int, bool>(i => i == -5)  },
                   new object[] { strArray, 4, 0, "bb", strComparer, strGenericComparer, new Func<int, bool>(i => i == -5)  },
                   new object[] { strArray, 0, 7, null, strComparer, null, new Func<int, bool>(i => i == 0)  },
               };
            }
        }

        [Theory, MemberData("BinarySearchTestDataInRange")]
        public static void TestBinarySearchInRange<T>(T[] array, int index, int length, T value, IComparer comparer, IComparer<T> genericComparer, Func<int, bool> verifier)
        {
            int idx = Array.BinarySearch(array, index, length, value, comparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, index, length, value, genericComparer);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch((Array)array, index, length, value);
            Assert.True(verifier(idx));

            idx = Array.BinarySearch(array, index, length, value);
            Assert.True(verifier(idx));
        }

        [Fact]
        public static void TestGetAndSetValue()
        {
            var intArray = new int[3] { 7, 8, 9 };
            Array array = intArray;
            
            Assert.Equal(7, array.GetValue(0));
            array.SetValue(41, 0);
            Assert.Equal(41, intArray[0]);
            
            Assert.Equal(8, array.GetValue(1));
            array.SetValue(42, 1);
            Assert.Equal(42, intArray[1]);
            
            Assert.Equal(9, array.GetValue(2));
            array.SetValue(43, 2);
            Assert.Equal(43, intArray[2]);
            
            array = new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            Assert.Equal(1, array.GetValue(0, 0));
            Assert.Equal(6, array.GetValue(1, 2));
            array.SetValue(42, 1, 2);
            Assert.Equal(42, array.GetValue(1, 2));
        }

        [Fact]
        public static void TestGetValue_Invalid()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new int[10].GetValue(-1)); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[10].GetValue(10)); // Index >= array.Length

            Assert.Throws<ArgumentNullException>(() => new int[10].GetValue(null)); // Indices is null
            Assert.Throws<ArgumentException>(() => new int[10, 10].GetValue(new int[] { 1, 2, 3 })); // Too many indicies
        }

        [Fact]
        public static void TestSetValue_Invalid()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new int[10].SetValue(1, -1)); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => new int[10].SetValue(1, 10)); // Index >= array.Length

            Assert.Throws<ArgumentNullException>(() => new int[10].SetValue(1, null)); // Indices is null
            Assert.Throws<ArgumentException>(() => new int[10, 10].SetValue(1, new int[] { 1, 2, 3 })); // Indices is null
        }

        [Fact]
        public static void TestClear_PrimitiveValuesWithoutGCRefs()
        {
            var array = new int[] { 7, 8, 9 };
            Array.Clear(array, 0, 3);
            Assert.Equal(new int[] { 0, 0, 0 }, array);

            array = new int[] { 7, 8, 9 };
            ((IList)array).Clear();
            Assert.Equal(new int[] { 0, 0, 0 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 2, 3);
            Assert.Equal(new int[] { 0x1234567, 0x789abcde, 0, 0, 0, 0x22446688 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 0, 6);
            Assert.Equal(new int[] { 0, 0, 0, 0, 0, 0 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 6, 0);
            Assert.Equal(new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 }, array);

            array = new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 };
            Array.Clear(array, 0, 0);
            Assert.Equal(new int[] { 0x1234567, 0x789abcde, 0x22334455, 0x66778899, 0x11335577, 0x22446688 }, array);
        }

        [Fact]
        public static void TestClear_ValuesWithGCRefs()
        {
            var array = new string[] { "7", "8", "9" };
            Array.Clear(array, 0, 3);
            Assert.Equal(new string[] { null, null, null }, array);

            array = new string[] { "7", "8", "9" };
            ((IList)array).Clear();
            Assert.Equal(new string[] { null, null, null }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 2, 3);
            Assert.Equal(new string[] { "0x1234567", "0x789abcde", null, null, null, "0x22446688" }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 0, 6);
            Assert.Equal(new string[] { null, null, null, null, null, null }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 6, 0);
            Assert.Equal(new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" }, array);

            array = new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" };
            Array.Clear(array, 0, 0);
            Assert.Equal(new string[] { "0x1234567", "0x789abcde", "0x22334455", "0x66778899", "0x11335577", "0x22446688" }, array);
        }

        [Fact]
        public static void TestClear_ValuesWithEmbeddedGCRefs()
        {
            var array = new G[]
            {
                new G { x = 1, s = "Hello", z = 2 },
                new G { x = 2, s = "Hello", z = 3 },
                new G { x = 3, s = "Hello", z = 4 },
                new G { x = 4, s = "Hello", z = 5 },
                new G { x = 5, s = "Hello", z = 6 }
            };

            Array.Clear(array, 0, 5);
            for (int i = 0; i < array.Length; i++)
            {
                Assert.Equal(0, array[i].x);
                Assert.Null(array[i].s);
                Assert.Equal(0, array[i].z);
            }

            array = new G[]
            {
                new G { x = 1, s = "Hello", z = 2 },
                new G { x = 2, s = "Hello", z = 3 },
                new G { x = 3, s = "Hello", z = 4 },
                new G { x = 4, s = "Hello", z = 5 },
                new G { x = 5, s = "Hello", z = 6 }
            };

            Array.Clear(array, 2, 3);

            Assert.Equal(1, array[0].x);
            Assert.Equal("Hello", array[0].s);
            Assert.Equal(2, array[0].z);

            Assert.Equal(2, array[1].x);
            Assert.Equal("Hello", array[1].s);
            Assert.Equal(3, array[1].z);

            for (int i = 2; i < 2 + 3; i++)
            {
                Assert.Equal(0, array[i].x);
                Assert.Null(array[i].s);
                Assert.Equal(0, array[i].z);
            }
        }

        [Fact]
        public static void TestClear_Invalid()
        {
            var intArray = new int[10];

            Assert.Throws<ArgumentNullException>(() => Array.Clear(null, 0, 0)); // Array is null

            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, -1, 0)); // Index < 0
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 0, -1)); // Length < 0 

            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 0, 11)); // Length > array.Length
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 10, 1)); // Index + length > array.Length
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 9, 2)); // Index + length > array.Length
            Assert.Throws<IndexOutOfRangeException>(() => Array.Clear(intArray, 6, 0x7fffffff)); // Index + length > array.Length
        }

        [Fact]
        public static void TestCopy_GCReferenceArray()
        {
            var src = new string[] { "Red", "Green", null, "Blue" };
            var dst = new string[] { "X", "X", "X", "X" };
            Array.Copy(src, 0, dst, 0, 4);
            Assert.Equal(new string[] { "Red", "Green", null, "Blue" }, dst);

            // With reverse overlap
            src = new string[] { "Red", "Green", null, "Blue" };
            Array.Copy(src, 1, src, 2, 2);
            Assert.Equal(new string[] { "Red", "Green", "Green", null }, src);
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_ToReferenceTypeArray()
        {
            var src1 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            object[] dst1 = new object[10];
            Array.Copy(src1, 2, dst1, 5, 3);
            Assert.Equal(new object[] { null, null, null, null, null, 2, 3, 4, null, null }, dst1);

            var src2 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            object[] dst2 = new IEquatable<int>[10];
            Array.Copy(src2, 2, dst2, 5, 3);
            Assert.Equal(new IEquatable<int>[] { null, null, null, null, null, 2, 3, 4, null, null }, dst2);

            var src3 = new int?[] { 0, 1, 2, default(int?), 4, 5, 6, 7, 8, 9 };
            object[] dst3 = new object[10];
            Array.Copy(src3, 2, dst3, 5, 3);
            Assert.Equal(new object[] { null, null, null, null, null, 2, null, 4, null, null }, dst3);
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_ToReferenceTypeArray_Invalid()
        {
            var src = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            object[] dst = new IEnumerable<int>[10];
            Assert.Throws<ArrayTypeMismatchException>(() => Array.Copy(src, 0, dst, 0, 10));
        }

        [Fact]
        public static void TestCopy_ReferenceTypeArray_ToValueTypeArray()
        {
            const int cc = unchecked((int)0xcccccccc);

            // Test 1: simple compatible array types
            var src = new object[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;

            var dst = new int[10];
            for (int i = 0; i < dst.Length; i++)
                dst[i] = cc;

            Array.Copy(src, 2, dst, 5, 3);
            Assert.Equal(new int[] { cc, cc, cc, cc, cc, 2, 3, 4, cc, cc }, dst);

            // Test 2: more complex compatible array types
            src = new IEquatable<int>[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;

            dst = new int[10];
            for (int i = 0; i < dst.Length; i++)
                dst[i] = cc;

            Array.Copy(src, 2, dst, 5, 3);
            Assert.Equal(new int[] { cc, cc, cc, cc, cc, 2, 3, 4, cc, cc }, dst);

            // Test 3: array contains incompatible types that are ignored by the method (not in index range)
            src = new IEquatable<int>[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;
            src[1] = new NotInt32();
            src[5] = new NotInt32();

            dst = new int[10];
            for (int i = 0; i < dst.Length; i++)
                dst[i] = cc;

            Array.Copy(src, 2, dst, 5, 3);
            Assert.Equal(new int[] { cc, cc, cc, cc, cc, 2, 3, 4, cc, cc }, dst);

            // Test 4: compatible array types (nullables)
            src = new object[10];
            for (int i = 0; i < src.Length; i++)
                src[i] = i;
            src[4] = null;

            var dNullable = new int?[10];
            for (int i = 0; i < dNullable.Length; i++)
                dNullable[i] = cc;

            Array.Copy(src, 2, dNullable, 5, 3);
            Assert.True(dNullable[0].HasValue && dNullable[0].Value == cc);
            Assert.True(dNullable[1].HasValue && dNullable[1].Value == cc);
            Assert.True(dNullable[2].HasValue && dNullable[2].Value == cc);
            Assert.True(dNullable[3].HasValue && dNullable[3].Value == cc);
            Assert.True(dNullable[4].HasValue && dNullable[4].Value == cc);
            Assert.True(dNullable[5].HasValue && dNullable[5].Value == 2);
            Assert.True(dNullable[6].HasValue && dNullable[6].Value == 3);
            Assert.False(dNullable[7].HasValue);
            Assert.True(dNullable[8].HasValue && dNullable[8].Value == cc);
            Assert.True(dNullable[9].HasValue && dNullable[9].Value == cc);
        }

        [Fact]
        public static void TestCopy_ReferenceTypeArray_ToValueTypeArray_Invalid()
        {
            object[] src = new string[10];
            int[] dst = new int[10];
            Assert.Throws<ArrayTypeMismatchException>(() => Array.Copy(src, 0, dst, 0, 10)); // Different array types

            src = new IEquatable<int>[10];
            src[4] = new NotInt32();
            dst = new int[10];
            // Legacy: Note that the cast checks are done during copying, so some elements in the destination
            // array may already have been overwritten.
            Assert.Throws<InvalidCastException>(() => Array.Copy(src, 2, dst, 5, 3));

            src = new IEquatable<int>[10];
            src[4] = null;
            dst = new int[10];
            // Legacy: Note that the cast checks are done during copying, so some elements in the destination
            // array may already have been overwritten.
            Assert.Throws<InvalidCastException>(() => Array.Copy(src, 2, dst, 5, 3));
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_ToObjectArray()
        {
            var src = new G[]
            {
                new G { x = 1, s = "Hello1", z = 2 },
                new G { x = 2, s = "Hello2", z = 3 },
                new G { x = 3, s = "Hello3", z = 4 },
                new G { x = 4, s = "Hello4", z = 5 },
                new G { x = 5, s = "Hello5", z = 6 }
            };

            var dst = new object[5];
            Array.Copy(src, 0, dst, 0, 5);
            for (int i = 0; i < dst.Length; i++)
            {
                Assert.True(dst[i] is G);
                G g = (G)dst[i];
                Assert.Equal(src[i].x, g.x);
                Assert.Equal(src[i].s, g.s);
                Assert.Equal(src[i].z, g.z);
            }
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_WithGCReferences()
        {
            var src = new G[]
            {
                new G { x = 1, s = "Hello1", z = 2 },
                new G { x = 2, s = "Hello2", z = 3 },
                new G { x = 3, s = "Hello3", z = 4 },
                new G { x = 4, s = "Hello4", z = 5 },
                new G { x = 5, s = "Hello5", z = 6 }
            };
            
            var dst = new G[5];
            Array.Copy(src, 0, dst, 0, 5);
            for (int i = 0; i < dst.Length; i++)
            {
                Assert.Equal(src[i].x, dst[i].x);
                Assert.Equal(src[i].s, dst[i].s);
                Assert.Equal(src[i].z, dst[i].z);
            }

            // With overlap
            Array.Copy(src, 1, src, 2, 3);
            Assert.Equal(1, src[0].x);
            Assert.Equal("Hello1", src[0].s);
            Assert.Equal(2, src[0].z);

            Assert.Equal(2, src[1].x);
            Assert.Equal("Hello2", src[1].s);
            Assert.Equal(3, src[1].z);

            Assert.Equal(2, src[2].x);
            Assert.Equal("Hello2", src[2].s);
            Assert.Equal(3, src[2].z);

            Assert.Equal(3, src[3].x);
            Assert.Equal("Hello3", src[3].s);
            Assert.Equal(4, src[3].z);

            Assert.Equal(4, src[4].x);
            Assert.Equal("Hello4", src[4].s);
            Assert.Equal(5, src[4].z);
        }

        [Fact]
        public static void TestCopy_ValueTypeArray_WithNoGCReferences()
        {
            // Value-type to value-type array copy.
            var src = new int[] { 0x12345678, 0x22334455, 0x778899aa };
            var dst = new int[3];
            Array.Copy(src, 0, dst, 0, 3);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa }, dst);

            // Value-type to value-type array copy (in place, with overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.Copy(src, 3, src, 2, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x55443322, 0x33445566, 0x33445566 }, src);

            // Value-type to value-type array copy (in place, with reverse overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.Copy(src, 2, src, 3, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x778899aa, 0x55443322 }, src);
        }

        [Fact]
        public static void TestConstrainedCopy_GCReferenceArray()
        {
            var s = new string[] { "Red", "Green", null, "Blue" };
            var d = new string[] { "X", "X", "X", "X" };
            Array.ConstrainedCopy(s, 0, d, 0, 4);
            Assert.Equal(new string[] { "Red", "Green", null, "Blue" }, d);

            // With reverse overlap
            s = new string[] { "Red", "Green", null, "Blue" };
            Array.ConstrainedCopy(s, 1, s, 2, 2);
            Assert.Equal(new string[] { "Red", "Green", "Green", null}, s);
        }

        [Fact]
        public static void TestConstrainedCopy_ValueTypeArray_WithGCReferences()
        {
            var src = new G[]
            {
                new G { x = 1, s = "Hello1", z = 2 },
                new G { x = 2, s = "Hello2", z = 3 },
                new G { x = 3, s = "Hello3", z = 4 },
                new G { x = 4, s = "Hello4", z = 5 },
                new G { x = 5, s = "Hello5", z = 6 }
            };

            var dst = new G[5];
            Array.ConstrainedCopy(src, 0, dst, 0, 5);
            for (int i = 0; i < dst.Length; i++)
            {
                Assert.Equal(src[i].x, dst[i].x);
                Assert.Equal(src[i].s, dst[i].s);
                Assert.Equal(src[i].z, dst[i].z);
            }

            // With overlap
            Array.ConstrainedCopy(src, 1, src, 2, 3);
            Assert.Equal(1, src[0].x);
            Assert.Equal("Hello1", src[0].s);
            Assert.Equal(2, src[0].z);

            Assert.Equal(2, src[1].x);
            Assert.Equal("Hello2", src[1].s);
            Assert.Equal(3, src[1].z);

            Assert.Equal(2, src[2].x);
            Assert.Equal("Hello2", src[2].s);
            Assert.Equal(3, src[2].z);

            Assert.Equal(3, src[3].x);
            Assert.Equal("Hello3", src[3].s);
            Assert.Equal(4, src[3].z);

            Assert.Equal(4, src[4].x);
            Assert.Equal("Hello4", src[4].s);
            Assert.Equal(5, src[4].z);
        }

        [Fact]
        public static void TestConstrainedCopy_ValueTypeArray_WithNoCGReferences()
        {
            var src = new int[] { 0x12345678, 0x22334455, 0x778899aa };
            var dst = new int[3];

            // Value-type to value-type array ConstrainedCopy.
            Array.ConstrainedCopy(src, 0, dst, 0, 3);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa }, dst);

            // Value-type to value-type array ConstrainedCopy (in place, with overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.ConstrainedCopy(src, 3, src, 2, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x55443322, 0x33445566, 0x33445566 }, src);

            // Value-type to value-type array ConstrainedCopy (in place, with reverse overlap)
            src = new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x55443322, 0x33445566 };
            Array.ConstrainedCopy(src, 2, src, 3, 2);
            Assert.Equal(new int[] { 0x12345678, 0x22334455, 0x778899aa, 0x778899aa, 0x55443322 }, src);
        }
        
        [Fact]
        public static void TestFind()
        {
            var intArray = new int[] { 7, 8, 9 };

            // Exists included here since it's a trivial wrapper around FindIndex
            Assert.True(Array.Exists(intArray, i => i == 8));
            Assert.False(Array.Exists(intArray, i => i == -1));
            
            int[] results = Array.FindAll(intArray, i => (i % 2) != 0);
            Assert.Equal(results.Length, 2);
            Assert.True(Array.Exists(results, i => i == 7));
            Assert.True(Array.Exists(results, i => i == 9));

            var stringArray = new string[] { "7", "8", "88", "888", "9" };
            Assert.Equal("8", Array.Find(stringArray, s => s.StartsWith("8")));
            Assert.Null(Array.Find(stringArray, s => s == "X"));

            intArray = new int[] { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
            Assert.Equal(3, Array.FindIndex(intArray, i => i >= 43));                        
            Assert.Equal(-1, Array.FindIndex(intArray, i => i == 99));

            Assert.Equal(3, Array.FindIndex(intArray, 3, i => i == 43));
            Assert.Equal(-1, Array.FindIndex(intArray, 4, i => i == 43));

            Assert.Equal(3, Array.FindIndex(intArray, 1, 3, i => i == 43));
            Assert.Equal(-1, Array.FindIndex(intArray, 1, 2, i => i == 43));

            stringArray = new string[] { "7", "8", "88", "888", "9" };
            Assert.Equal("888", Array.FindLast(stringArray, s => s.StartsWith("8")));            
            Assert.Null(Array.FindLast(stringArray, s => s == "X"));

            intArray = new int[] { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
            Assert.Equal(9, Array.FindLastIndex(intArray, i => i >= 43));
            Assert.Equal(-1, Array.FindLastIndex(intArray, i => i == 99));

            Assert.Equal(3, Array.FindLastIndex(intArray, 3, i => i == 43));
            Assert.Equal(-1, Array.FindLastIndex(intArray, 2, i => i == 43));

            Assert.Equal(3, Array.FindLastIndex(intArray, 5, 3, i => i == 43));
            Assert.Equal(-1, Array.FindLastIndex(intArray, 5, 2, i => i == 43));
        }

        [Fact]
        public static void TestGetEnumerator()
        {
            int[] arr = { 7, 8, 9 };
            IEnumerator ie = arr.GetEnumerator();

            for (int i = 0; i < 2; i++)
            {
                int counter = 0;
                while (ie.MoveNext())
                {
                    Assert.Equal(arr[counter], ie.Current);
                    counter++;
                }

                Assert.Equal(arr.Length, counter);

                ie.Reset();
            }
        }

        public static IEnumerable<object[]> IndexOfTestData
        {
            get
            {
                var intArray = new int[] { 7, 7, 8, 8, 9, 9 };
                var stringArray = new string[] { null, null, "Hello", "Hello", "Goodbye", "Goodbye", null, null };

                return new[]
                {
                    new object[] { intArray, 8, 0, 0, 2 },
                    new object[] { intArray, 8, 3, 0, 3 },
                    new object[] { intArray, 8, 4, 0, -1 },
                    new object[] { intArray, 9, 2, 2, -1 },
                    new object[] { intArray, 9, 2, 3, 4 },
                    new object[] { intArray, 10, 0, 0, -1 },
                    new object[] { stringArray, null, 0, 0, 0 },
                    new object[] { stringArray, "Hello", 0, 0, 2 },
                    new object[] { stringArray, "Goodbye", 0, 0, 4 },
                    new object[] { stringArray, "Nowhere", 0, 0, -1 },
                    new object[] { stringArray, "Hello", 3, 0, 3 },
                    new object[] { stringArray, "Hello", 4, 0, -1 },
                    new object[] { stringArray, "Goodbye", 2, 3, 4 },
                    new object[] { stringArray, "Goodbye", 2, 2, -1 }
               };
            }
        }

        [Theory, MemberData("IndexOfTestData")]
        public static void TestIndexOf(Array array, object value, int startIndex, int count, int expected)
        {
            if (startIndex == 0)
            {
                Assert.Equal(expected, Array.IndexOf(array, value));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.IndexOf((int[])array, (int)value)); // Make IndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.IndexOf((string[])array, (string)value)); // Make IndexOf generic for string.
                }
            }
            if (count == 0)
            {
                Assert.Equal(expected, Array.IndexOf(array, value, startIndex));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.IndexOf((int[])array, (int)value, startIndex)); // Make IndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.IndexOf((string[])array, (string)value, startIndex)); // Make IndexOf generic for string.
                }
                count = array.Length - startIndex;
            }
            
            Assert.Equal(expected, Array.IndexOf(array, value, startIndex, count));
            if (array is int[])
            {
                Assert.Equal(expected, Array.IndexOf((int[])array, (int)value, startIndex, count)); // Make IndexOf generic for int.
            }
            else if (array is string[])
            {
                Assert.Equal(expected, Array.IndexOf((string[])array, (string)value, startIndex, count)); // Make IndexOf generic for string.
            }
        }

        public static IEnumerable<object[]> LastIndexOfTestData
        {
            get
            {
                var intArray = new int[] { 7, 7, 8, 8, 9, 9 };
                var stringArray = new string[] { null, null, "Hello", "Hello", "Goodbye", "Goodbye", null, null };

                return new[]
                {
                    new object[] { intArray, 8, 0, 0, 3 },
                    new object[] { intArray, 8, 1, 0, -1 },
                    new object[] { intArray, 8, 3, 0, 3 },
                    new object[] { intArray, 7, 3, 2, -1 },
                    new object[] { intArray, 7, 3, 3, 1 },
                    new object[] { stringArray, null, 0, 0, 7 },
                    new object[] { stringArray, "Hello", 0, 0, 3 },
                    new object[] { stringArray, "Goodbye", 0, 0, 5 },
                    new object[] { stringArray, "Nowhere", 0, 0, -1 },
                    new object[] { stringArray, "Hello", 2, 0, 2 },
                    new object[] { stringArray, "Hello", 3, 0, 3 },
                    new object[] { stringArray, "Goodbye", 7, 2, -1 },
                    new object[] { stringArray, "Goodbye", 7, 3, 5 },
               };
            }
        }

        [Theory, MemberData("LastIndexOfTestData")]
        public static void TestLastIndexOf(Array array, object value, int startIndex, int count, int expected)
        {
            if (startIndex == 0)
            {
                startIndex = array.Length - 1;
                Assert.Equal(expected, Array.LastIndexOf(array, value));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((int[])array, (int)value)); // Make LastIndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((string[])array, (string)value)); // Make LastIndexOf generic for string.
                }
            }
            if (count == 0)
            {
                count = startIndex;
                Assert.Equal(expected, Array.LastIndexOf(array, value, startIndex));
                if (array is int[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((int[])array, (int)value, startIndex)); // Make LastIndexOf generic for int.
                }
                else if (array is string[])
                {
                    Assert.Equal(expected, Array.LastIndexOf((string[])array, (string)value, startIndex)); // Make LastIndexOf generic for string.
                }
            }
            
            Assert.Equal(expected, Array.LastIndexOf(array, value, startIndex, count));
            if (array is int[])
            {
                Assert.Equal(expected, Array.LastIndexOf((int[])array, (int)value, startIndex, count)); // Make LastIndexOf generic for int.
            }
            else if (array is string[])
            {
                Assert.Equal(expected, Array.LastIndexOf((string[])array, (string)value, startIndex, count)); // Make LastIndexOf generic for string.
            }
        }

        [Fact]
        public static void TestIStructural()
        {
            var array = new int[] { 2, 3, 4, 5 };

            IStructuralComparable isc = array;

            Assert.Equal(0, isc.CompareTo(new int[] { 2, 3, 4, 5 }, new IntegerComparer()));
            Assert.Equal(1, isc.CompareTo(new int[] { 1, 3, 4, 5 }, new IntegerComparer()));
            Assert.Equal(-1, isc.CompareTo(new int[] { 2, 3, 4, 6 }, new IntegerComparer()));
            
            IStructuralEquatable ise = array;
            Assert.True(ise.Equals(new int[] { 2, 3, 4, 5 }, new IntegerComparer()));
            Assert.False(ise.Equals(new int[] { 2 }, new IntegerComparer()));

            int hash1 = ise.GetHashCode(new IntegerComparer());
            int hash2 = ((IStructuralEquatable)(array.Clone())).GetHashCode(new IntegerComparer());
            Assert.Equal(hash1, hash2);
        }

        public static IEnumerable<object[]> ResizeTestData
        {
            get
            {
                return new[]
                {
                    new object[] { new int[] { 1, 2, 3, 4, 5 }, 7, new int[] { 1, 2, 3, 4, 5, default(int), default(int) } },
                    new object[] { new int[] { 1, 2, 3, 4, 5 }, 3, new int[] { 1, 2, 3 } },
                    new object[] { null, 3, new int[] { default(int), default(int), default(int) } }
               };
            }
        }

        [Theory, MemberData("ResizeTestData")]
        public static void TestResize(int[] array, int newSize, int[] expected)
        {
            int[] testArray = array;
            Array.Resize(ref testArray, newSize);
            Assert.Equal(newSize, testArray.Length);
            Assert.Equal(expected, testArray);
        }

        [Fact]
        public static void TestResize_Invalid()
        {
            var array = new int[0];
            Assert.Throws<ArgumentOutOfRangeException>(() => Array.Resize(ref array, -1)); // New size < 0
            Assert.Equal(new int[0], array);
        }

        public static IEnumerable<object[]> ReverseTestData
        {
            get
            {
                return new[]
                {
                    new object[] { new int[] { 1, 2, 3, 4, 5 }, 0, 5, new int[] { 5, 4, 3, 2, 1 } },
                    new object[] { new int[] { 1, 2, 3, 4, 5 }, 2, 3, new int[] { 1, 2, 5, 4, 3 } },
                    new object[] { new string[] { "1", "2", "3", "4", "5" }, 0, 5, new string[] { "5", "4", "3", "2", "1" } },
                    new object[] { new string[] { "1", "2", "3", "4", "5" }, 2, 3, new string[] { "1", "2", "5", "4", "3" } }
               };
            }
        }

        [Theory, MemberData("ReverseTestData")]
        public static void TestReverse(Array array, int index, int count, Array expected)
        {
            if (index == 0 && count == array.Length)
            {
                Array testArray = (Array)array.Clone();
                Array.Reverse(testArray);
                Assert.Equal(expected, testArray);
            }
            Array.Reverse(array, index, count);
            Assert.Equal(expected, expected);
        }

        [Fact]
        public static void TestReverse_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Array.Reverse(null)); // Array is null
            Assert.Throws<ArgumentNullException>(() => Array.Reverse(null, 0, 0)); // Array is null

            Assert.Throws<RankException>(() => Array.Reverse(new int[10, 10])); // Array is multidimensional
            Assert.Throws<RankException>(() => Array.Reverse(new int[10, 10], 0, 0)); // Array is multidimensional

            Assert.Throws<ArgumentOutOfRangeException>(() => Array.Reverse(new int[10], -1, 10)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => Array.Reverse(new int[10], 0, -1)); // Count < 0

            Assert.Throws<ArgumentException>(() => Array.Reverse(new int[10], 10, 1)); // Index + count > array.Length
            Assert.Throws<ArgumentException>(() => Array.Reverse(new int[10], 9, 2)); // Index + count > array.Length
        }

        [Fact]
        public static void TestSort()
        {
            IComparer<int> icomparer = new IntegerComparer();
            TestSortHelper(new int[] { }, 0, 0, icomparer);
            TestSortHelper(new int[] { 5 }, 0, 1, icomparer);
            TestSortHelper(new int[] { 5, 2 }, 0, 2, icomparer);

            TestSortHelper(new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 0, 9, icomparer);
            TestSortHelper(new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 3, 4, icomparer);
            TestSortHelper(new int[] { 5, 2, 9, 8, 4, 3, 2, 4, 6 }, 3, 6, icomparer);

            IComparer<string> scomparer = new StringComparer();
            TestSortHelper(new string[] { }, 0, 0, scomparer);
            TestSortHelper(new string[] { "5" }, 0, 1, scomparer);
            TestSortHelper(new string[] { "5", "2" }, 0, 2, scomparer);

            TestSortHelper(new string[] { "5", "2", null, "8", "4", "3", "2", "4", "6" }, 0, 9, scomparer);
            TestSortHelper(new string[] { "5", "2", null, "8", "4", "3", "2", "4", "6" }, 3, 4, scomparer);
            TestSortHelper(new string[] { "5", "2", null, "8", "4", "3", "2", "4", "6" }, 3, 6, scomparer);
        }

        [Fact]
        public static void TestSort_Array_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Array.Sort(null)); // Array is null
            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10])); // Array is multidimensional
        }

        [Fact]
        public static void TestSort_Array_Array_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Array.Sort(null, new int[10])); // Keys is null

            Assert.Throws<ArgumentException>(() => Array.Sort(new int[10], new int[9])); // Keys.Length > items.Length

            Assert.Throws<RankException>(() => Array.Sort(new int[10, 10], new int[10])); // Keys is multidimensional
            Assert.Throws<RankException>(() => Array.Sort(new int[10], new int[10, 10])); // Items is multidimensional
        }

        [Fact]
        public static void TestSort_Array_Int_Int_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Array.Sort(null, 0, 0)); // Array is null

            Assert.Throws<ArgumentOutOfRangeException>(() => Array.Sort(new int[10], -1, 0)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => Array.Sort(new int[10], 0, -1)); // Length < 0

            Assert.Throws<ArgumentException>(() => Array.Sort(new int[10], 10, 1)); // Index + length > list.Count
            Assert.Throws<ArgumentException>(() => Array.Sort(new int[10], 9, 2)); // Index + length > list.Count
        }

        [Fact]
        public static void TestTrueForAll()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            
            Assert.True(Array.TrueForAll(array, i => i > 0));
            Assert.False(Array.TrueForAll(array, i => i == 3));
            Assert.True(Array.TrueForAll(new int[0], i => false));
        }

        [Fact]
        public static void TestCreateInstance_Type_IntArray()
        {
            string[] stringArray = (string[])Array.CreateInstance(typeof(string), new int[] { 10 });
            Assert.Equal(stringArray, new string[10]);

            stringArray = (string[])Array.CreateInstance(typeof(string), new int[] { 0 });
            Assert.Equal(stringArray, new string[0]);

            int[] intArray1 = (int[])Array.CreateInstance(typeof(int), new int[] { 1 });
            VerifyArray(intArray1, 1, new int[] { 1 }, new int[] { 0 }, new int[] { 0 }, false);
            Assert.Equal(intArray1, new int[1]);

            int[,] intArray2 = (int[,])Array.CreateInstance(typeof(int), new int[] { 1, 2 });
            VerifyArray(intArray2, 2, new int[] { 1, 2 }, new int[] { 0, 0 }, new int[] { 0, 1 }, false);
            intArray2[0, 1] = 42;
            Assert.Equal(42, intArray2[0, 1]);

            int[,,] intArray3 = (int[,,])Array.CreateInstance(typeof(int), new int[] { 1, 2, 3 });
            VerifyArray(intArray3, 3, new int[] { 1, 2, 3 }, new int[] { 0, 0, 0 }, new int[] { 0, 1, 2 }, false);
            intArray3[0, 1, 2] = 42;
            Assert.Equal(42, intArray3[0, 1, 2]);

            int[,,,] intArray4 = (int[,,,])Array.CreateInstance(typeof(int), new int[] { 1, 2, 3, 4 });
            VerifyArray(intArray4, 4, new int[] { 1, 2, 3, 4 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 1, 2 }, false);
            intArray4[0, 1, 2, 3] = 42;
            Assert.Equal(42, intArray4[0, 1, 2, 3]);
        }

        [Fact]
        public static void TestCreateInstance_Type_IntArray_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Array.CreateInstance(null, new int[] { 10 })); // ElementType is null
            Assert.Throws<ArgumentNullException>(() => Array.CreateInstance(typeof(int), null)); // Lengths is null

            Assert.Throws<ArgumentException>(() => Array.CreateInstance(typeof(int), new int[0])); // Array is empty
            Assert.Throws<ArgumentOutOfRangeException>(() => Array.CreateInstance(typeof(int), new int[] { -1 })); // Array contains negative integers
        }

        [Fact]
        public static void TestCreateInstance_Type_IntArray_IntArray()
        {
            int[] intArray1 = (int[])Array.CreateInstance(typeof(int), new int[] { 5 }, new int[] { 0 });
            Assert.Equal(intArray1, new int[5]);
            VerifyArray(intArray1, 1, new int[] { 5 }, new int[] { 0 }, new int[] { 4 }, false);

            int[,,] intArray2 = (int[,,])Array.CreateInstance(typeof(int), new int[] { 7, 8, 9 }, new int[] { 1, 2, 3 });
            Assert.Equal(intArray2, new int[7, 8, 9]);
            VerifyArray(intArray2, 3, new int[] { 7, 8, 9 }, new int[] { 1, 2, 3 }, new int[] { 7, 9, 11 }, false);
        }

        [Fact]
        public static void TestCreateInstance_Type_IntArray_IntArray_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Array.CreateInstance(null, new int[] { 1 }, new int[] { 1 })); // ElementType is null
            Assert.Throws<ArgumentNullException>(() => Array.CreateInstance(typeof(int), null, new int[] { 1 })); // Lengths is null
            Assert.Throws<ArgumentNullException>(() => Array.CreateInstance(typeof(int), new int[] { 1 }, null)); // Lower bounds is null

            Assert.Throws<ArgumentException>(() => Array.CreateInstance(typeof(int), new int[0], new int[] { 1 })); // Lengths is empty
            Assert.Throws<ArgumentOutOfRangeException>(() => Array.CreateInstance(typeof(int), new int[] { -1 }, new int[] { 1 })); // Lengths contains negative integer
            Assert.Throws<ArgumentException>(() => Array.CreateInstance(typeof(int), new int[] { 1 }, new int[] { 1, 2 })); // Lengths and lower bounds have different lengths
        }

        [Fact]
        public static void TestSetValueCasting()
        {
            // null -> default(null)
            var arr1 = new S[3];
            arr1[1].X = 0x22222222;
            arr1.SetValue(null, new int[] { 1 });
            Assert.Equal(0, arr1[1].X);

            // T -> Nullable<T>
            var arr2 = new int?[3];
            arr2.SetValue(42, new int[] { 1 });
            int? nullable1 = arr2[1];
            Assert.True(nullable1.HasValue);
            Assert.Equal(42, nullable1.Value);

            // null -> Nullable<T>
            var arr3 = new int?[3];
            arr3[1] = 42;
            arr3.SetValue(null, new int[] { 1 });
            int? nullable2 = arr3[1];
            Assert.False(nullable2.HasValue);

            // Primitive widening
            var arr4 = new int[3];
            arr4.SetValue((short)42, new int[] { 1 });
            Assert.Equal(42, arr4[1]);

            // Widening from enum to primitive
            var arr5 = new int[3];
            arr5.SetValue(E1.MinusTwo, new int[] { 1 });
            Assert.Equal(-2, arr5[1]);
        }

        [Fact]
        public static void TestSetValueCasting_Invalid()
        {
            // Unlike most of the other reflection apis, converting or widening a primitive to an enum is NOT allowed.
            var arr1 = new E1[3];
            Assert.Throws<InvalidCastException>(() => arr1.SetValue((sbyte)1, new int[] { 1 }));

            // Primitive widening must be value-preserving
            var arr2 = new int[3];
            Assert.Throws<ArgumentException>(() => arr2.SetValue((uint)42, new int[] { 1 }));

            // T -> Nullable<T>  T must be exact
            var arr3 = new int?[3];
            Assert.Throws<InvalidCastException>(() => arr3.SetValue((short)42, new int[] { 1 }));
        }
        
        [Fact]
        public static void TestCopyTo_ArrayTypeMismatchVsInvalidCast()
        {
            Array s = new B1[10];
            Array d = new D1[10];
            s.CopyTo(d, 0);

            s = new D1[10];
            d = new B1[10];
            s.CopyTo(d, 0);

            s = new I1[10];
            d = new B1[10];
            s.CopyTo(d, 0);

            s = new B1[10];
            d = new I1[10];
            s.CopyTo(d, 0);

            s = new B1[10];
            d = new B2[10];
            Assert.Throws<ArrayTypeMismatchException>(() => s.CopyTo(d, 0));

            s = new B1[] { new B1() };
            d = new I1[1];
            Assert.Throws<InvalidCastException>(() => s.CopyTo(d, 0));
        }
        
        private static void VerifyArrayAsIList(Array array)
        {
            IList ils = array;
            Assert.Equal(array.Length, ils.Count);

            Assert.Equal(array, ils.SyncRoot);

            Assert.False(ils.IsSynchronized);
            Assert.True(ils.IsFixedSize);
            Assert.False(ils.IsReadOnly);

            Assert.Throws<NotSupportedException>(() => ils.Add(2));
            Assert.Throws<NotSupportedException>(() => ils.Insert(0, 2));
            Assert.Throws<NotSupportedException>(() => ils.Remove(0));
            Assert.Throws<NotSupportedException>(() => ils.RemoveAt(0));

            if (array.Rank == 1)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    object obj = ils[i];
                    Assert.Equal(array.GetValue(i), obj);
                    Assert.True(ils.Contains(obj));
                    Assert.Equal(i, ils.IndexOf(obj));
                }

                Assert.False(ils.Contains(null));
                Assert.False(ils.Contains(999));
                Assert.Equal(-1, ils.IndexOf(null));
                Assert.Equal(-1, ils.IndexOf(999));

                ils[1] = 10;
                Assert.Equal(10, ils[1]);
            }
            else
            {
                Assert.Throws<RankException>(() => ils.Contains(null));
                Assert.Throws<RankException>(() => ils.IndexOf(null));
            }
        }

        private static void VerifyArray(Array array, int rank, int[] lengths, int[] lowerBounds, int[] upperBounds, bool checkIList)
        {
            Assert.Equal(rank, array.Rank);

            for (int i = 0; i < lengths.Length; i++)
                Assert.Equal(lengths[i], array.GetLength(i));

            for (int i = 0; i < lowerBounds.Length; i++)
                Assert.Equal(lowerBounds[i], array.GetLowerBound(i));

            for (int i = 0; i < upperBounds.Length; i++)
                Assert.Equal(upperBounds[i], array.GetUpperBound(i));


            Assert.Throws<IndexOutOfRangeException>(() => array.GetLength(-1)); // Dimension < 0
            Assert.Throws<IndexOutOfRangeException>(() => array.GetLength(array.Rank)); // Dimension >= array.Rank

            Assert.Throws<IndexOutOfRangeException>(() => array.GetLowerBound(-1)); // Dimension < 0
            Assert.Throws<IndexOutOfRangeException>(() => array.GetLowerBound(array.Rank)); // Dimension >= array.Rank

            Assert.Throws<IndexOutOfRangeException>(() => array.GetUpperBound(-1)); // Dimension < 0
            Assert.Throws<IndexOutOfRangeException>(() => array.GetUpperBound(array.Rank)); // Dimension >= array.Rank

            if (checkIList)
            {
                VerifyArrayAsIList(array);
            }
        }

        private static void TestSortHelper<T>(T[] array, int index, int length, IComparer<T> comparer)
        {
            T[] control = SimpleSort(array, index, length, comparer);

            Array spawn1 = (Array)(array.Clone());
            Array.Sort(spawn1, index, length, (IComparer)comparer);
            Assert.True(ArraysAreEqual((T[])spawn1, control, comparer));

            T[] spawn2 = (T[])(array.Clone());
            Array.Sort(spawn2, index, length, comparer);
            Assert.True(ArraysAreEqual(spawn2, control, comparer));
        }

        private static T[] SimpleSort<T>(T[] a, int index, int length, IComparer<T> comparer)
        {
            T[] result = (T[])(a.Clone());
            if (length < 2)
                return result;

            for (int i = index; i < index + length - 1; i++)
            {
                T tmp = result[i];
                for (int j = i + 1; j < index + length; j++)
                {
                    if (comparer.Compare(tmp, result[j]) > 0)
                    {
                        result[i] = result[j];
                        result[j] = tmp;
                        tmp = result[i];
                    }
                }
            }
            return result;
        }

        private static bool ArraysAreEqual<T>(T[] a, T[] b, IComparer<T> comparer)
        {
            // If the same instances were passed, this is unlikely what the test intended.
            Assert.False(ReferenceEquals(a, b));

            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (0 != comparer.Compare(a[i], b[i]))
                    return false;
            }
            return true;
        }

        private struct G
        {
            public int x;
            public string s;
            public int z;
        }

        private class IntegerComparer : IComparer, IComparer<int>, IEqualityComparer
        {
            public int Compare(object x, object y)
            {
                return Compare((int)x, (int)y);
            }

            public int Compare(int x, int y)
            {
                return x - y;
            }

            bool IEqualityComparer.Equals(object x, object y)
            {
                return ((int)x) == ((int)y);
            }

            public int GetHashCode(object obj)
            {
                return ((int)obj) >> 2;
            }
        }

        private class StringComparer : IComparer, IComparer<string>
        {
            public int Compare(object x, object y)
            {
                return Compare((string)x, (string)y);
            }

            public int Compare(string x, string y)
            {
                if (x == y)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;
                return x.CompareTo(y);
            }
        }

        private enum E1 : sbyte
        {
            MinusTwo = -2
        }

        private struct S
        {
            public int X;
        }

        private class NotInt32 : IEquatable<int>
        {
            public bool Equals(int other)
            {
                throw new NotImplementedException();
            }
        }

        private class B1 { }
        private class D1 : B1 { }
        private class B2 { }
        private class D2 : B2 { }
        private interface I1 { }
        private interface I2 { }
    }
}
