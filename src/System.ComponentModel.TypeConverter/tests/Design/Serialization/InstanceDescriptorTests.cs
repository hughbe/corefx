// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information. 

//
// InstanceDescriptorTest.cs - Unit tests for 
//	System.ComponentModel.Design.Serialization.InstanceDescriptor
//
// Author:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.ComponentModel.Design.Serialization.Tests
{
    public class InstanceDescriptorTests
    {
        private const string Url = "http://www.mono-project.com/";

        public static IEnumerable<object[]> Ctor_MemberInfo_ICollection_TestData()
        {
            // ConstructorInfo.
            yield return new object[] { typeof(EditorAttribute).GetConstructor(new Type[0]), null };
            yield return new object[] { typeof(EditorAttribute).GetConstructor(new Type[0]), new object[0] };
            yield return new object[] { typeof(Uri).GetConstructor(new Type[] { typeof(string) }), new object[] { Url } };
            yield return new object[] { typeof(Uri).GetConstructor(new Type[] { typeof(string) }), new object[] { 1 } };

            // FieldInfo.
            yield return new object[] { typeof(DataClass).GetField(nameof(DataClass.Field)), null };
            yield return new object[] { typeof(DataClass).GetField(nameof(DataClass.Field)), new object[0] };

            // PropertyInfo.
            yield return new object[] { typeof(DataClass).GetProperty(nameof(DataClass.Property)), null };
            yield return new object[] { typeof(DataClass).GetProperty(nameof(DataClass.Property)), new object[0] };
            yield return new object[] { typeof(DataClass).GetProperty(nameof(DataClass.Property)), new object[] { new object() } };

            // MethodInfo.
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.ParameterlessMethod)), null };
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.ParameterlessMethod)), new object[0] };
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { 1 } };
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { new object() } };

            MethodInfo argumentMethod = typeof(DataClass).GetMethod(nameof(DataClass.IntReturningMethod));
            var argumentInstanceDescriptor = new InstanceDescriptor(argumentMethod, null);
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { argumentInstanceDescriptor } };

            // EventInfo.
            yield return new object[] { typeof(DataClass).GetEvent(nameof(DataClass.Event)), null };
            yield return new object[] { typeof(DataClass).GetEvent(nameof(DataClass.Event)), new object[0] };
            yield return new object[] { typeof(DataClass).GetEvent(nameof(DataClass.Event)), new object[] { new object() } };

            // MemberInfo.
            yield return new object[] { null, null };
            yield return new object[] { null, new object[0] };
            yield return new object[] { null, new object[] { new object() } };
        }

        [Theory]
        [MemberData(nameof(Ctor_MemberInfo_ICollection_TestData))]
        public void Ctor_ConstructorInfo_ICollection(MemberInfo member, object[] arguments)
        {
            InstanceDescriptor instanceDescriptor = new InstanceDescriptor(member, arguments);
            Assert.NotSame(arguments, instanceDescriptor.Arguments);
            Assert.Equal(arguments ?? Array.Empty<object[]>(), instanceDescriptor.Arguments);
            Assert.True(instanceDescriptor.IsComplete);
            Assert.Same(member, instanceDescriptor.MemberInfo);
        }

        public static IEnumerable<object[]> Ctor_MemberInfo_ICollection_Bool_TestData()
        {
            foreach (bool isComplete in new bool[] { true, false })
            {
                // ConstructorInfo.
                yield return new object[] { typeof(int).GetConstructor(new Type[0]), null, isComplete };
                yield return new object[] { typeof(int).GetConstructor(new Type[0]), new object[0], isComplete };
                yield return new object[] { typeof(Uri).GetConstructor(new Type[] { typeof(string) }), new object[] { Url }, isComplete };
                yield return new object[] { typeof(Uri).GetConstructor(new Type[] { typeof(string) }), new object[] { 1 }, isComplete };

                // FieldInfo.
                yield return new object[] { typeof(DataClass).GetField(nameof(DataClass.Field)), null, isComplete };
                yield return new object[] { typeof(DataClass).GetField(nameof(DataClass.Field)), new object[0], isComplete };

                // PropertyInfo.
                yield return new object[] { typeof(DataClass).GetProperty(nameof(DataClass.Property)), null, isComplete };
                yield return new object[] { typeof(DataClass).GetProperty(nameof(DataClass.Property)), new object[0], isComplete };
                yield return new object[] { typeof(DataClass).GetProperty(nameof(DataClass.Property)), new object[] { new object() }, isComplete };

                // MethodInfo.
                yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.ParameterlessMethod)), null, isComplete };
                yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.ParameterlessMethod)), new object[0], isComplete };
                yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { 1 }, isComplete };
                yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { new object() }, isComplete };

                MethodInfo argumentMethod = typeof(DataClass).GetMethod(nameof(DataClass.IntReturningMethod));
                var argumentInstanceDescriptor = new InstanceDescriptor(argumentMethod, null);
                yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { argumentInstanceDescriptor }, isComplete };

                // EventInfo.
                yield return new object[] { typeof(DataClass).GetEvent(nameof(DataClass.Event)), null, isComplete };
                yield return new object[] { typeof(DataClass).GetEvent(nameof(DataClass.Event)), new object[0], isComplete };
                yield return new object[] { typeof(DataClass).GetEvent(nameof(DataClass.Event)), new object[] { new object() }, isComplete };

                // MemberInfo.
                yield return new object[] { null, null, isComplete };
                yield return new object[] { null, new object[0], isComplete };
                yield return new object[] { null, new object[] { new object() }, isComplete };
            }
        }

        [Theory]
        [MemberData(nameof(Ctor_MemberInfo_ICollection_Bool_TestData))]
        public void Ctor_MemberInfo_ICollection_Boolean(MemberInfo member, object[] arguments, bool isComplete)
        {
            InstanceDescriptor instanceDescriptor = new InstanceDescriptor(member, arguments, isComplete);
            Assert.NotSame(arguments, instanceDescriptor.Arguments);
            Assert.Equal(arguments ?? Array.Empty<object>(), instanceDescriptor.Arguments);
            Assert.Equal(isComplete, instanceDescriptor.IsComplete);
            Assert.Same(member, instanceDescriptor.MemberInfo);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new object[] { new object[] { } })]
        [InlineData(new object[] { new object[] { "s", 1 } })]
        public void Ctor_ConstructorInfoArgumentMismatch_ThrowsArgumentException(object[] arguments)
        {
            ConstructorInfo member = typeof(Uri).GetConstructor(new Type[] { typeof(string) });
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(member, arguments));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(member, arguments, isComplete: true));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(member, arguments, isComplete: false));
        }

        [Fact]
        public void Ctor_ConstructorInfoStatic_ThrowsArgumentException()
        {
            ConstructorInfo constructor = typeof(DataClass).GetConstructors(BindingFlags.Static | BindingFlags.NonPublic).Single();
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(constructor, null));
        }

        [Fact]
        public void Ctor_FieldInfoArgumentMismatch_ThrowsArgumentException()
        {
            FieldInfo member = typeof(DataClass).GetField(nameof(DataClass.Field));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(member, new object[] { Url }));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(member, new object[] { Url }, isComplete: true));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(member, new object[] { Url }, isComplete: false));
        }

        [Fact]
        public void Ctor_FieldInfoNonStatic_ThrowsArgumentException()
        {
            FieldInfo member = typeof(DataClass).GetField(nameof(DataClass.NonStaticField));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(member, null));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(member, null, isComplete: true));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(member, null, isComplete: false));
        }

        [Fact]
        public void Ctor_PropertyInfoNonStatic_ThrowsArgumentException()
        {
            PropertyInfo pi = typeof(DataClass).GetProperty(nameof(DataClass.NonStaticProperty));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(pi, null));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(pi, null, isComplete: true));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(pi, null, isComplete: false));
        }

        [Fact]
        public void Ctor_PropertyInfoWriteOnly_ThrowsArgumentException()
        {
            PropertyInfo pi = typeof(DataClass).GetProperty(nameof(DataClass.WriteOnlyProperty));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(pi, null));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(pi, null, isComplete: true));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(pi, null, isComplete: false));
        }

        [Fact]
        public void Ctor_MethodInfoNonStatic_ThrowsArgumentException()
        {
            MethodInfo method = typeof(DataClass).GetMethod(nameof(DataClass.NonStaticMethod));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(method, new object[] { 1 }));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(method, new object[] { 1 }, isComplete: true));
            AssertExtensions.Throws<ArgumentException>("member", () => new InstanceDescriptor(method, new object[] { 1 }, isComplete: false));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new object[] { new object[] { } })]
        [InlineData(new object[] { new object[] { "s", 1 } })]
        public void Ctor_MethodInfoArgumentMismatch_ThrowsArgumentException(object[] arguments)
        {
            MethodInfo method = typeof(DataClass).GetMethod(nameof(DataClass.IntMethod));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(method, arguments));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(method, arguments, isComplete: true));
            AssertExtensions.Throws<ArgumentException>(null, () => new InstanceDescriptor(method, arguments, isComplete: false));
        }

        public static IEnumerable<object[]> Invoke_TestData()
        {
            // ConstructorInfo.
            yield return new object[] { typeof(EditorAttribute).GetConstructor(new Type[0]), new object[0], new EditorAttribute() };
            yield return new object[] { typeof(Uri).GetConstructor(new Type[] { typeof(string) }), new object[] { Url }, new Uri(Url) };

            // FieldInfo.
            yield return new object[] { typeof(DataClass).GetField(nameof(DataClass.Field)), new object[0], "Field" };

            // PropertyInfo.
            yield return new object[] { typeof(DataClass).GetProperty(nameof(DataClass.Property)), new object[0], "Property" };

            // MethodInfo.
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.ParameterlessMethod)), new object[0], "ParameterlessMethod" };
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { 1 }, "1" };

            MethodInfo argumentMethod = typeof(DataClass).GetMethod(nameof(DataClass.IntReturningMethod));
            var argumentInstanceDescriptor = new InstanceDescriptor(argumentMethod, null);
            yield return new object[] { typeof(DataClass).GetMethod(nameof(DataClass.IntMethod)), new object[] { argumentInstanceDescriptor }, "1" };

            // EventInfo.
            yield return new object[] { typeof(DataClass).GetEvent(nameof(DataClass.Event)), new object[0], null };

            // Null MemberInfo.
            yield return new object[] { null, new object[0], null };
            yield return new object[] { null, new object[] { new object() }, null };
        }

        [Theory]
        [MemberData(nameof(Invoke_TestData))]
        public void Invoke_Invoke_ReturnsExpected(MemberInfo member, object[] arguments, object expected)
        {
            var instanceDescriptor = new InstanceDescriptor(member, arguments);
            Assert.Equal(expected, instanceDescriptor.Invoke());
        }

        [Fact]
        public void Invoke_PropertyInfoArgumentMismatch_ThrowsTargetParameterCountException()
        {
            PropertyInfo member = typeof(DataClass).GetProperty(nameof(DataClass.Property));
            InstanceDescriptor instanceDescriptor = new InstanceDescriptor(member, new object[] { Url });
            Assert.Throws<TargetParameterCountException>(() => instanceDescriptor.Invoke());
        }

        private class DataClass
        {
            static DataClass() { }

            public static string Field = nameof(Field);

            public string NonStaticField = nameof(NonStaticField);

            public static string Property { get; set; } = nameof(Property);

            public string NonStaticProperty { get; set; }

            public static string WriteOnlyProperty
            {
                set { }
            }

            public void NonStaticMethod(int i) { }

            public static int IntReturningMethod() => 1;

            public static string ParameterlessMethod() => nameof(ParameterlessMethod);

            public static string IntMethod(int i) => i.ToString();

            public event EventHandler Event { add { } remove { } }
        }
    }
}
