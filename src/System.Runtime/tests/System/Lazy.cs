// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;

using Xunit;

namespace System.Runtime.Tests
{
    public static class LazyTests
    {
        [Fact]
        public static void Test_Ctor()
        {
            // Make sure the default constructor does not throw
            new Lazy<object>();
        }

        [Fact]
        public static void TestCtor_Invalid()
            {
                Assert.Throws<ArgumentNullException>(() => new Lazy<object>(null));
            }

        [Fact]
        public static void TestToString_DoesntForceAllocation()
        {
            var lazy = new Lazy<object>(() => (object)1);
            Assert.NotEqual(1.ToString(), lazy.ToString());
            Assert.False(lazy.IsValueCreated);

            object tmp = lazy.Value;
            Assert.Equal(1.ToString(), lazy.ToString());
        }

        [Fact]
        public static void TestIsValueCreated()
        {
            Lazy<string> lazy = new Lazy<string>(() => "Test");
            Assert.False(lazy.IsValueCreated);

            string temp = lazy.Value;
            Assert.True(lazy.IsValueCreated);
        }

        [Fact]
        public static void TestValue()
        {
            string value = "Test";
            Lazy<string> lazy = new Lazy<string>(() => value);
            Assert.Equal(value, lazy.Value);

            int valueInt = 99;
            var LazyInt = new Lazy<int>(() => valueInt);
            Assert.Equal(valueInt, LazyInt.Value);

            lazy = new Lazy<string>(() => value, true);
            Assert.Equal(value, lazy.Value);

            lazy = new Lazy<string>(() => null, false);
            Assert.Null(lazy.Value);
        }

        [Fact]
        public static void TestValue_Invalid()
        {
            string value = "Test";
            var lazy = new Lazy<string>(() => value);
            string lazilyAllocatedValue;

            Assert.Throws<InvalidOperationException>(() =>
            {
                int x = 0;

                lazy = new Lazy<string>(delegate
                {
                    if (x++ < 5)
                        return lazy.Value;
                    else
                        return "Test";
                }, true);

                lazilyAllocatedValue = lazy.Value;
            });

            Assert.Throws<MissingMemberException>(() =>
            {
                lazy = new Lazy<string>();
                lazilyAllocatedValue = lazy.Value;
            });
        }

        [Fact]
        public static void TestValue_ThrownException_DoesntCreateValue()
        {
            var lazy = new Lazy<string>(() =>
            {
                int zero = 0;
                int x = 1 / zero;
                return "";
            }, true);

            Assert.Throws<DivideByZeroException>(() => lazy.Value);
            Assert.Throws<DivideByZeroException>(() => lazy.Value);

            Assert.False(lazy.IsValueCreated);
        }

        [Fact]
        public static void TestEnsureInitalized_SimpleRefTypes()
        {
            HasDefaultCtor hdcTemplate = new HasDefaultCtor();
            string strTemplate = "foo";

            // Activator.CreateInstance (uninitialized).
            HasDefaultCtor a = null;
            Assert.NotNull(LazyInitializer.EnsureInitialized(ref a));
            Assert.NotNull(a);

            // Activator.CreateInstance (already initialized).
            HasDefaultCtor b = hdcTemplate;
            Assert.Equal(hdcTemplate, LazyInitializer.EnsureInitialized(ref b));
            Assert.Equal(hdcTemplate, b);

            // Func based initialization (uninitialized).
            string c = null;
            Assert.Equal(strTemplate, LazyInitializer.EnsureInitialized(ref c, () => strTemplate));
            Assert.Equal(strTemplate, c);

            // Func based initialization (already initialized).
            string d = strTemplate;
            Assert.Equal(strTemplate, LazyInitializer.EnsureInitialized(ref d, () => strTemplate + "bar"));
            Assert.Equal(strTemplate, d);
        }

        [Fact]
        public static void TestEnsureInitalized_SimpleRefTypes_Invalid()
        {
            // Func based initialization (nulls not permitted).
            string e = null;
            Assert.Throws<InvalidOperationException>(() => LazyInitializer.EnsureInitialized(ref e, () => null));

            // Activator.CreateInstance (for a type without a default ctor).
            NoDefaultCtor ndc = null;
            Assert.Throws<MissingMemberException>(() => LazyInitializer.EnsureInitialized(ref ndc));
        }

        [Fact]
        public static void TestEnsureInitialized_ComplexRefTypes()
        {
            string strTemplate = "foo";
            HasDefaultCtor hdcTemplate = new HasDefaultCtor();

            // Activator.CreateInstance (uninitialized).
            HasDefaultCtor a = null;
            bool aInit = false;
            object aLock = null;
            Assert.NotNull(LazyInitializer.EnsureInitialized(ref a, ref aInit, ref aLock));
            Assert.NotNull(a);

            // Activator.CreateInstance (already initialized).
            HasDefaultCtor b = hdcTemplate;
            bool bInit = true;
            object bLock = null;
            Assert.Equal(hdcTemplate, LazyInitializer.EnsureInitialized(ref b, ref bInit, ref bLock));
            Assert.Equal(hdcTemplate, b);

            // Func based initialization (uninitialized).
            string c = null;
            bool cInit = false;
            object cLock = null;
            Assert.Equal(strTemplate, LazyInitializer.EnsureInitialized(ref c, ref cInit, ref cLock, () => strTemplate));
            Assert.Equal(strTemplate, c);

            // Func based initialization (already initialized).
            string d = strTemplate;
            bool dInit = true;
            object dLock = null;
            Assert.Equal(strTemplate, LazyInitializer.EnsureInitialized(ref d, ref dInit, ref dLock, () => strTemplate + "bar"));
            Assert.Equal(strTemplate, d);

            // Func based initialization (nulls *ARE* permitted).
            string e = null;
            bool einit = false;
            object elock = null;
            int initCount = 0;

            Assert.Null(LazyInitializer.EnsureInitialized(ref e, ref einit, ref elock, () => { initCount++; return null; }));
            Assert.Null(e);
            Assert.Equal(1, initCount);
            Assert.Null(LazyInitializer.EnsureInitialized(ref e, ref einit, ref elock, () => { initCount++; return null; }));
        }

        [Fact]
        public static void TestEnsureInitalized_ComplexRefTypes_Invalid()
        {
            // Activator.CreateInstance (for a type without a default ctor).
            NoDefaultCtor ndc = null;
            bool ndcInit = false;
            object ndcLock = null;
            Assert.Throws<MissingMemberException>(() => LazyInitializer.EnsureInitialized(ref ndc, ref ndcInit, ref ndcLock));
        }

        [Fact]
        public static void TestLazyInitializerComplexValueTypes()
        {
            var empty = new LIX();
            var template = new LIX(33);

            // Activator.CreateInstance (uninitialized).
            LIX a = default(LIX);
            bool aInit = false;
            object aLock = null;
            LIX ensuredValA = LazyInitializer.EnsureInitialized(ref a, ref aInit, ref aLock);
            Assert.Equal(empty, ensuredValA);
            Assert.Equal(empty, a);

            // Activator.CreateInstance (already initialized).
            LIX b = template;
            bool bInit = true;
            object bLock = null;
            LIX ensuredValB = LazyInitializer.EnsureInitialized(ref b, ref bInit, ref bLock);
            Assert.Equal(template, ensuredValB);
            Assert.Equal(template, b);

            // Func based initialization (uninitialized).
            LIX c = default(LIX);
            bool cInit = false;
            object cLock = null;
            LIX ensuredValC = LazyInitializer.EnsureInitialized(ref c, ref cInit, ref cLock, () => template);
            Assert.Equal(template, c);
            Assert.Equal(template, ensuredValC);

            // Func based initialization (already initialized).
            LIX d = template;
            bool dInit = true;
            object dLock = null;
            LIX template2 = new LIX(template.f * 2);
            LIX ensuredValD = LazyInitializer.EnsureInitialized(ref d, ref dInit, ref dLock, () => template2);
            Assert.Equal(template, ensuredValD);
            Assert.Equal(template, d);
        }
        
        private class HasDefaultCtor { }

        private class NoDefaultCtor
        {
            public NoDefaultCtor(int x) { }
        }

        private struct LIX
        {
            public LIX(int f) { this.f = f; }

            public int f;
            public override bool Equals(object other) { return other is LIX && ((LIX)other).f == f; }
            public override int GetHashCode() { return f.GetHashCode(); }
            public override string ToString() { return "LIX<" + f + ">"; }
        }
    }
}
