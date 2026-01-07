using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Common
{
    [TestFixture]
    public class CourseTypeUkprnValidatorTests
    {
        private Mock<IProviderCourseTypesReadRepository> _repoMock = null!;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IProviderCourseTypesReadRepository>();
        }

        private class TestValidator : AbstractValidator<CourseTypeUkprnValidationObject>
        {
            public TestValidator(IProviderCourseTypesReadRepository repo)
            {
                RuleFor(x => x).ValidateCourseTypeForUkprn(repo);
            }
        }

        [Test]
        public async Task Validate_RepositoryContainsMatchingCourseType_ReturnsIsValidTrue()
        {
            // Arrange
            var ukprn = 10000001;
            var validationObject = new CourseTypeUkprnValidationObject { Ukprn = ukprn, CourseType = CourseType.Apprenticeship };
            _repoMock.Setup(r => r.GetProviderCourseTypesByUkprn(ukprn))
                     .ReturnsAsync(new List<ProviderCourseType>
                     {
                         new ProviderCourseType { Ukprn = ukprn, CourseType = CourseType.Apprenticeship }
                     });

            var sut = new TestValidator(_repoMock.Object);

            // Act
            var result = await sut.ValidateAsync(validationObject);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Validate_RepositoryDoesNotContainMatchingCourseType_ReturnsIsInvalid()
        {
            // Arrange
            var ukprn = 10000001;
            var validationObject = new CourseTypeUkprnValidationObject { Ukprn = ukprn, CourseType = CourseType.ApprenticeshipUnit };
            _repoMock.Setup(r => r.GetProviderCourseTypesByUkprn(ukprn))
                     .ReturnsAsync(new List<ProviderCourseType>
                     {
                         new ProviderCourseType { Ukprn = ukprn, CourseType = CourseType.Apprenticeship }
                     });

            var sut = new TestValidator(_repoMock.Object);

            // Act
            var result = await sut.ValidateAsync(validationObject);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be(CourseTypeUkprnValidator.ProviderCourseTypeNotFoundErrorMessage);
        }

        [Test]
        public async Task Validate_WhenNull_RetunrsInvalid()
        {
            // Arrange
            var ukprn = 10000001;
            var validationObject = new CourseTypeUkprnValidationObject { Ukprn = ukprn, CourseType = CourseType.Apprenticeship };
            _repoMock.Setup(r => r.GetProviderCourseTypesByUkprn(ukprn))
                     .ReturnsAsync((List<ProviderCourseType>?)null);

            var sut = new TestValidator(_repoMock.Object);

            // Act
            var result = await sut.ValidateAsync(validationObject);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be(CourseTypeUkprnValidator.ProviderCourseTypeNotFoundErrorMessage);
        }
    }
}