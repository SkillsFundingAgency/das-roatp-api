using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.Delete
{
    [TestFixture]
    public class DeleteProviderCourseLocationCommandValidatorTests
    {
        private readonly string _userId = "userid";
        private readonly string _userDisplayName = "userDisplayName";
        [Test]
        public async Task ValidateId_LessThanZero_ReturnsError()
        {
            var command = new DeleteProviderCourseLocationCommand(10012002, "123", Guid.Empty, _userId, _userDisplayName);

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), Mock.Of<IProviderCourseLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LocationId);
        }


        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var id = Guid.NewGuid();
            var command = new DeleteProviderCourseLocationCommand(10012002, "123", id, _userId, _userDisplayName);

            var _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationsReadRepository>();
            _providerCourseLocationReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.ProviderCourseLocation> { new Domain.Entities.ProviderCourseLocation { NavigationId = id } });

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), _providerCourseLocationReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var id = Guid.NewGuid();
            var command = new DeleteProviderCourseLocationCommand(10012002, "123", id, _userId, _userDisplayName);

            var _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationsReadRepository>();
            _providerCourseLocationReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.ProviderCourseLocation> { new Domain.Entities.ProviderCourseLocation { NavigationId = id } });

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), _providerCourseLocationReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var id = Guid.NewGuid();
            var command = new DeleteProviderCourseLocationCommand(10012002, "123", id, userId, _userDisplayName);

            var _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationsReadRepository>();
            _providerCourseLocationReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.ProviderCourseLocation> { new Domain.Entities.ProviderCourseLocation { NavigationId = id } });

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), _providerCourseLocationReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserDisplayName_Empty_ReturnsError(string userDisplayName)
        {
            var id = Guid.NewGuid();
            var command = new DeleteProviderCourseLocationCommand(10012002, "123", id, _userId, userDisplayName);

            var _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationsReadRepository>();
            _providerCourseLocationReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.ProviderCourseLocation> { new Domain.Entities.ProviderCourseLocation { NavigationId = id } });

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), _providerCourseLocationReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserDisplayName);
        }
    }
}
