// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ComponentModel.DataAnnotations
{
    public class CompareAttributeTests
    {
        [Fact]
        public static void Constructor_NullOtherProperty_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("otherProperty", () => new CompareAttribute(null));
        }

        [Theory]
        [InlineData("OtherProperty")]
        [InlineData("")]
        public static void Constructor(string otherProperty)
        {
            CompareAttribute attribute = new CompareAttribute(otherProperty);
            Assert.Equal(otherProperty, attribute.OtherProperty);

            Assert.True(attribute.RequiresValidationContext);
        }

        [Fact]
        public static void Validate_EqualObjects_DoesNotThrow()
        {
            var otherObject = new CompareObject("test");
            var currentObject = new CompareObject("test");
            var testContext = new ValidationContext(otherObject, null, null);

            var attribute = new CompareAttribute("CompareProperty");
            attribute.Validate(currentObject.CompareProperty, testContext);
        }

        [Theory]
        [InlineData("CompareProperty")]
        [InlineData("ComparePropertyWithDisplayName")]
        [InlineData("UnknownPropertyName")]
        public static void Validate_ComparedObjectsNotEqual_ThrowsValidationException(string otherProperty)
        {
            var currentObject = new CompareObject("a");
            var otherObject = new CompareObject("b");

            var testContext = new ValidationContext(otherObject, null, null);
            testContext.DisplayName = "CurrentProperty";

            var attribute = new CompareAttribute(otherProperty);
            Assert.Throws<ValidationException>(() => attribute.Validate(currentObject.CompareProperty, testContext));
        }

        [Fact]
        public static void Validate_CustomDerivedClass_DoesNotThrow()
        {
            var otherObject = new CompareObject("a");
            var currentObject = new CompareObject("b");
            var testContext = new ValidationContext(otherObject, null, null);

            var attribute = new DerivedCompareAttribute("CompareProperty");
            attribute.Validate(currentObject.CompareProperty, testContext);
        }

        private class DerivedCompareAttribute : CompareAttribute
        {
            public DerivedCompareAttribute(string otherProperty) : base(otherProperty) { }

            protected override ValidationResult IsValid(object value, ValidationContext context) => ValidationResult.Success;
        }

        private class CompareObject
        {
            public string CompareProperty { get; set; }

            [Display(Name = "DisplayName")]
            public string ComparePropertyWithDisplayName { get; set; }

            public CompareObject(string otherValue)
            {
                CompareProperty = otherValue;
                ComparePropertyWithDisplayName = otherValue;
            }
        }
    }
}
