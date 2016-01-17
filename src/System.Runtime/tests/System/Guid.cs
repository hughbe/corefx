// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

using Xunit;

namespace System.Runtime.Tests
{
    public static class GuidTests
    {
        private static readonly Guid _testGuid = new Guid("a8a110d5-fc49-43c5-bf46-802db8f843ff");

        [Fact]
        public static void TestCtor_ByteArray()
        {
            var guid = new Guid(_testGuid.ToByteArray());
            Assert.Equal(_testGuid, guid);
        }

        [Fact]
        public static void TestCtor_Int32_Int16_Int16_Byte_Byte_Byte_Byte_Byte_Byte_Byte_Byte()
        {
            var guid = new Guid(unchecked((int)0xa8a110d5), unchecked((short)0xfc49), (short)0x43c5, 0xbf, 0x46, 0x80, 0x2d, 0xb8, 0xf8, 0x43, 0xff);
            Assert.Equal(_testGuid, guid);
        }

        [Fact]
        public static void TestCtor_Int32_Int16_Int16_ByteArray()
        {
            var guid = new Guid(unchecked((int)0xa8a110d5), unchecked((short)0xfc49), (short)0x43c5, new byte[] { 0xbf, 0x46, 0x80, 0x2d, 0xb8, 0xf8, 0x43, 0xff });
            Assert.Equal(_testGuid, guid);
        }

        [Fact]
        public static void TestCtor_String()
        {
            var guid = new Guid("a8a110d5-fc49-43c5-bf46-802db8f843ff");
            Assert.Equal(_testGuid, guid);
        }

        [Fact]
        public static void TestCtor_String_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => new Guid((string)null));
        }

        [Fact]
        public static void TestEquals()
        {
            Assert.True(_testGuid.Equals(_testGuid));
            Assert.True(_testGuid == _testGuid);
            Assert.False(_testGuid != _testGuid);

            Guid guid = new Guid("a8a110d5-fc49-43c5-bf46-802db8f843ff");
            Assert.True(_testGuid.Equals(guid));
            Assert.True(guid.Equals(_testGuid));
            Assert.True(_testGuid == guid);
            Assert.False(_testGuid != guid);

            Assert.False(_testGuid.Equals(Guid.Empty));
            Assert.False(Guid.Empty.Equals(_testGuid));
            Assert.False(_testGuid == Guid.Empty);
            Assert.True(_testGuid != Guid.Empty);

            Assert.False(_testGuid.Equals(null));
            Assert.False(_testGuid.Equals("a8a110d5-fc49-43c5-bf46-802db8f843ff"));            
        }

        [Fact]
        public static void TestCompareTo()
        {
            // Int32 Guid.CompareTo(Guid)
            Assert.True(_testGuid.CompareTo(new Guid("98a110d5-fc49-43c5-bf46-802db8f843ff")) > 0);
            Assert.True(_testGuid.CompareTo(new Guid("a8a110d5-fc49-43c5-bf46-802db8f843ff")) == 0);
            Assert.True(_testGuid.CompareTo(new Guid("e8a110d5-fc49-43c5-bf46-802db8f843ff")) < 0);

            // Int32 Guid.System.IComparable.CompareTo(Object)
            IComparable icomp = _testGuid;

            Assert.True(icomp.CompareTo(new Guid("98a110d5-fc49-43c5-bf46-802db8f843ff")) > 0);
            Assert.True(icomp.CompareTo(new Guid("a8a110d5-fc49-43c5-bf46-802db8f843ff")) == 0);
            Assert.True(icomp.CompareTo(new Guid("e8a110d5-fc49-43c5-bf46-802db8f843ff")) < 0);

            Assert.True(icomp.CompareTo(null) > 0);

            Assert.Throws<ArgumentException>(() => icomp.CompareTo("a8a110d5-fc49-43c5-bf46-802db8f843ff"));
        }

        [Fact]
        public static void TestToByteArray()
        {
            var bytes1 = new byte[] { 0xd5, 0x10, 0xa1, 0xa8, 0x49, 0xfc, 0xc5, 0x43, 0xbf, 0x46, 0x80, 0x2d, 0xb8, 0xf8, 0x43, 0xff };
            var bytes2 = _testGuid.ToByteArray();

            Assert.Equal(bytes1, bytes2);
        }

        [Fact]
        public static void TestEmpty()
        {
            Assert.Equal(Guid.Empty, new Guid(0, 0, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }));
        }

        [Fact]
        public static void TestNewGuid()
        {
            var g = Guid.NewGuid();
            Assert.NotEqual(Guid.Empty, g);

            var g2 = Guid.NewGuid();
            Assert.NotEqual(g, g2);
        }

        [Fact]
        public static void TestParse()
        {
            Assert.Equal(_testGuid, Guid.Parse("a8a110d5-fc49-43c5-bf46-802db8f843ff"));
            Assert.Equal(_testGuid, Guid.Parse("a8a110d5fc4943c5bf46802db8f843ff"));
            Assert.Equal(_testGuid, Guid.Parse("a8a110d5-fc49-43c5-bf46-802db8f843ff"));
            Assert.Equal(_testGuid, Guid.Parse("{a8a110d5-fc49-43c5-bf46-802db8f843ff}"));
            Assert.Equal(_testGuid, Guid.Parse("(a8a110d5-fc49-43c5-bf46-802db8f843ff)"));
            Assert.Equal(_testGuid, Guid.Parse("{0xa8a110d5,0xfc49,0x43c5,{0xbf,0x46,0x80,0x2d,0xb8,0xf8,0x43,0xff}}"));

            Assert.Equal(_testGuid, Guid.ParseExact("a8a110d5fc4943c5bf46802db8f843ff", "N"));
            Assert.Equal(_testGuid, Guid.ParseExact("a8a110d5-fc49-43c5-bf46-802db8f843ff", "D"));
            Assert.Equal(_testGuid, Guid.ParseExact("{a8a110d5-fc49-43c5-bf46-802db8f843ff}", "B"));
            Assert.Equal(_testGuid, Guid.ParseExact("(a8a110d5-fc49-43c5-bf46-802db8f843ff)", "P"));
            Assert.Equal(_testGuid, Guid.ParseExact("{0xa8a110d5,0xfc49,0x43c5,{0xbf,0x46,0x80,0x2d,0xb8,0xf8,0x43,0xff}}", "X"));
        }

        [Fact]
        public static void TestTryParse()
        {
            Guid g;
            Assert.True(Guid.TryParse("a8a110d5-fc49-43c5-bf46-802db8f843ff", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParse("a8a110d5fc4943c5bf46802db8f843ff", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParse("a8a110d5-fc49-43c5-bf46-802db8f843ff", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParse("{a8a110d5-fc49-43c5-bf46-802db8f843ff}", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParse("(a8a110d5-fc49-43c5-bf46-802db8f843ff)", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParse("{0xa8a110d5,0xfc49,0x43c5,{0xbf,0x46,0x80,0x2d,0xb8,0xf8,0x43,0xff}}", out g));
            Assert.Equal(_testGuid, g);

            Assert.True(Guid.TryParseExact("a8a110d5fc4943c5bf46802db8f843ff", "N", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParseExact("a8a110d5-fc49-43c5-bf46-802db8f843ff", "D", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParseExact("{a8a110d5-fc49-43c5-bf46-802db8f843ff}", "B", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParseExact("(a8a110d5-fc49-43c5-bf46-802db8f843ff)", "P", out g));
            Assert.Equal(_testGuid, g);
            Assert.True(Guid.TryParseExact("{0xa8a110d5,0xfc49,0x43c5,{0xbf,0x46,0x80,0x2d,0xb8,0xf8,0x43,0xff}}", "X", out g));
            Assert.Equal(_testGuid, g);

            Assert.False(Guid.TryParse("a8a110d5fc4943c5bf46802db8f843f", out g)); // One two few digits
            Assert.False(Guid.TryParseExact("a8a110d5-fc49-43c5-bf46-802db8f843ff", "N", out g)); // Contains '-' when "N" doesn't support those
        }

        [Fact]
        public static void TestGetHashCode()
        {
            Assert.NotEqual(_testGuid.GetHashCode(), Guid.Empty.GetHashCode());
        }

        [Fact]
        public static void TestToString()
        {
            Assert.Equal(_testGuid.ToString(), "a8a110d5-fc49-43c5-bf46-802db8f843ff");
            Assert.Equal(_testGuid.ToString("N"), "a8a110d5fc4943c5bf46802db8f843ff");
            Assert.Equal(_testGuid.ToString("D"), "a8a110d5-fc49-43c5-bf46-802db8f843ff");
            Assert.Equal(_testGuid.ToString("B"), "{a8a110d5-fc49-43c5-bf46-802db8f843ff}");
            Assert.Equal(_testGuid.ToString("P"), "(a8a110d5-fc49-43c5-bf46-802db8f843ff)");
            Assert.Equal(_testGuid.ToString("X"), "{0xa8a110d5,0xfc49,0x43c5,{0xbf,0x46,0x80,0x2d,0xb8,0xf8,0x43,0xff}}");
        }

        [Fact]
        public static void TestRandomness()
        {
            const int Iterations = 100;
            const int GuidSize = 16;
            byte[] random = new byte[GuidSize * Iterations];

            for (int i = 0; i < Iterations; i++)
            {
                // Get a new Guid
                Guid g = Guid.NewGuid();
                byte[] bytes = g.ToByteArray();

                // Make sure it's different from all of the previously created ones
                for (int j = 0; j < i; j++)
                {
                    Assert.False(bytes.SequenceEqual(new ArraySegment<byte>(random, j * GuidSize, GuidSize)));
                }

                // Copy it to our randomness array
                Array.Copy(bytes, 0, random, i * GuidSize, GuidSize);
            }

            // Verify the randomness of the data in the array. Guid has some small bias in it 
            // due to several bits fixed based on the format, but that bias is small enough and
            // the variability allowed by VerifyRandomDistribution large enough that we don't do 
            // anything special for it.
            RandomDataGenerator.VerifyRandomDistribution(random);
        }
    }
}
