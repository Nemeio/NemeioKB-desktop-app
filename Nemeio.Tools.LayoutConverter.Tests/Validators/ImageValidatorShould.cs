using System;
using System.Collections.Generic;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Models.Requirements;
using Nemeio.Tools.LayoutConverter.Validators;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Validators
{
    public class ImageValidatorShould
    {
        private const string FakeImageFilePath = @"C:\this\is\a\fake\image\file\path.png";

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ImageValidator_Validate_WithInvalidParameter_ThrowArgumentNullException(string imageFilePath)
        {
            var imageValidator = new ImageValidator();

            Assert.Throws<ArgumentNullException>(() => 
            {
                imageValidator.Validate(imageFilePath);
            });
        }

        [Test]
        public void ImageValidator_Validate_WithoutAnyRequirement_ThrowInvalidOperationException()
        {
            var imageValidator = new ImageValidator();

            Assert.Throws<InvalidOperationException>(() => 
            {
                imageValidator.Validate(FakeImageFilePath);
            });
        }

        [Test]
        public void ImageValidator_Validate_WithoutError_NotThrowException()
        {
            var imageValidator = new ImageValidator();
            imageValidator.AddRequirement(new UselessRequirement());

            Assert.DoesNotThrow(() =>
            {
                imageValidator.Validate(FakeImageFilePath);
            });
        }

        [Test]
        public void ImageValidator_Validate_WithError_ThrowsMissingRequirementException()
        {
            var imageValidator = new ImageValidator();
            imageValidator.AddRequirement(new ThereAlwaysAnErrorRequirement());

            Assert.Throws<MissingRequirementException>(() =>
            {
                imageValidator.Validate(FakeImageFilePath);
            });
        }

        [Test]
        public void ImageValidator_AddRequirement_WhichIsUnknow_Ok()
        {
            var imageValidator = new ImageValidator();
            var added = imageValidator.AddRequirement(new UselessRequirement());

            added.Should().BeTrue();
        }

        [Test]
        public void ImageValidator_AddRequirement_WhichAlreadyAdd_ReturnFalse()
        {
            var requirement = new UselessRequirement();
            var imageValidator = new ImageValidator();
            imageValidator.AddRequirement(requirement);

            var added = imageValidator.AddRequirement(requirement);

            added.Should().BeFalse();
        }

        [Test]
        public void ImageValidator_AddRequirement_WithNullParameter_ThrowNullArgumentException()
        {
            var imageValidator = new ImageValidator();

            Assert.Throws<ArgumentNullException>(() => 
            {
                imageValidator.AddRequirement(null);
            });
        }

        private class UselessRequirement : Requirement
        {
            internal override IEnumerable<RequirementError> Check(string filePath)
            {
                //  Nothing do to here
                return null;
            }
        }

        private class ThereAlwaysAnErrorRequirement : Requirement
        {
            internal override IEnumerable<RequirementError> Check(string filePath)
            {
                return new List<RequirementError>()
                {
                    new RequirementError(
                        filePath,
                        "This is an error"
                    )
                };
            }
        }
    }
}
