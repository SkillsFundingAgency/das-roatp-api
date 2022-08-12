using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    [TestFixture]
    public class AddProviderCourseLocationCommandValidatorTests
    {
        private Mock<IProviderReadRepository> _providerReadRepositoryMock;
        private Mock<IProviderCourseReadRepository> _providerCourseReadRepositoryMock;
        private Mock<IProviderLocationsReadRepository> _providerLocationsReadRepositoryMock;
         
        private AddProviderCourseLocationCommand _command;

        [SetUp]
        public void Before_Each_Test()
        {
            _providerReadRepositoryMock = new Mock<IProviderReadRepository>();
            _providerReadRepositoryMock
                .Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new Provider());

            _providerCourseReadRepositoryMock = new Mock<IProviderCourseReadRepository>();
            _providerCourseReadRepositoryMock
                .Setup(x => x.GetProviderCourseByUkprn(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.Entities.ProviderCourse());

            _providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            _providerLocationsReadRepositoryMock
                .Setup(x => x.GetProviderLocation(It.IsAny<int>(), It.IsAny<Guid>()))
                .ReturnsAsync(new ProviderLocation());

            _command = new AddProviderCourseLocationCommand(10012002, 123, "user", Guid.NewGuid(), true, true);
        }

        [Test, RecursiveMoqAutoData]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            _command = new AddProviderCourseLocationCommand(100, 123, "user", Guid.NewGuid(), true, true);
            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            _command = new AddProviderCourseLocationCommand(10012002, 0, "user", Guid.NewGuid(), true, true);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            _command = new AddProviderCourseLocationCommand(10012002, 123, userId, Guid.NewGuid(), true, true);


            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [Test]
        public async Task ValidateLocationNavigationId_InValid_ReturnsError()
        {
            _command = new AddProviderCourseLocationCommand(10012002, 123, "user", Guid.Empty, true, true);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.LocationNavigationId);
        }

        [Test]
        public async Task ValidateLocationNavigationId_Valid_ReturnsNoError()
        {
            _command = new AddProviderCourseLocationCommand(10012002, 123, "user", Guid.NewGuid(), true, true);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldNotHaveValidationErrorFor(c => c.LocationNavigationId);
        }

        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public async Task ValidateHasDayReleaseDeliveryOption_BothNotSelected_ReturnsError(bool? hasDayReleaseDeliveryOption, bool? hasBlockReleaseDeliveryOption, bool isvalid)
        {
            _command = new AddProviderCourseLocationCommand(10012002, 123, "user", Guid.NewGuid(), hasDayReleaseDeliveryOption, hasBlockReleaseDeliveryOption);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            if (isvalid)
                result.ShouldNotHaveValidationErrorFor(c => c.HasDayReleaseDeliveryOption);
            else
                result.ShouldHaveValidationErrorFor(c => c.HasDayReleaseDeliveryOption);
        }
    }
}
