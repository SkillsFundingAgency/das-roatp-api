using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    [TestFixture]
    public class AddProviderCourseLocationCommandValidatorTests
    {
        private Mock<IProvidersReadRepository> _providerReadRepositoryMock;
        private Mock<IProviderCoursesReadRepository> _providerCourseReadRepositoryMock;
        private Mock<IProviderLocationsReadRepository> _providerLocationsReadRepositoryMock;
        private Mock<IProviderCourseLocationsReadRepository> _providerCourseLocationsReadRepositoryMock;
        private int ukprn = 10012002;
        private int larsCode = 123;
        private string userId = "user";

        private AddProviderCourseLocationCommand _command;

        [SetUp]
        public void Before_Each_Test()
        {
            _providerReadRepositoryMock = new Mock<IProvidersReadRepository>();
            _providerReadRepositoryMock
                .Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new Provider());

            _providerCourseReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
            _providerCourseReadRepositoryMock
                .Setup(x => x.GetProviderCourseByUkprn(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Domain.Entities.ProviderCourse());

            _providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            _providerLocationsReadRepositoryMock
                .Setup(x => x.GetProviderLocation(It.IsAny<int>(), It.IsAny<Guid>()))
                .ReturnsAsync(new ProviderLocation());

            _providerCourseLocationsReadRepositoryMock = new Mock<IProviderCourseLocationsReadRepository>();
            _providerCourseLocationsReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<ProviderCourseLocation>());

            _command = new AddProviderCourseLocationCommand(ukprn, larsCode, userId, Guid.NewGuid(), true, true);
        }

        [Test, RecursiveMoqAutoData]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            _command = new AddProviderCourseLocationCommand(100, larsCode, userId, Guid.NewGuid(), true, true);
            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var locationNavigationId = Guid.NewGuid();
            _command = new AddProviderCourseLocationCommand(ukprn, 0, userId, locationNavigationId, true, true);

            var provider = new Provider();
            _providerReadRepositoryMock
                .Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync(provider);

            var providercourse = new Domain.Entities.ProviderCourse();
            providercourse.ProviderId = provider.Id;
            _providerCourseReadRepositoryMock
            .Setup(x => x.GetProviderCourseByUkprn(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(providercourse);

            var providerlocation = new ProviderLocation();
            providerlocation.NavigationId = locationNavigationId;
            _providerLocationsReadRepositoryMock
                .Setup(x => x.GetProviderLocation(It.IsAny<int>(), It.IsAny<Guid>()))
                .ReturnsAsync(providerlocation);

            var providerCourseLocations = new List<ProviderCourseLocation>();
            _providerCourseLocationsReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(providerCourseLocations);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            _command = new AddProviderCourseLocationCommand(ukprn, larsCode, userId, Guid.NewGuid(), true, true);


            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [Test]
        public async Task ValidateLocationNavigationId_InValid_ReturnsError()
        {
            _command = new AddProviderCourseLocationCommand(ukprn, larsCode, userId, Guid.Empty, true, true);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.LocationNavigationId);
            result.ShouldHaveValidationErrorFor(c => c.LocationNavigationId).WithErrorMessage(AddProviderCourseLocationCommandValidator.TrainingVenueErrorMessage);

        }

        [Test]
        public async Task ValidateLocationNavigationId_Valid_ReturnsNoError()
        {
            _command = new AddProviderCourseLocationCommand(ukprn, larsCode, userId, Guid.NewGuid(), true, true);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldNotHaveValidationErrorFor(c => c.LocationNavigationId);
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ValidateHasDayReleaseDeliveryOption_SelectedDeliveryOption_ReturnsNoError(bool? hasDayReleaseDeliveryOption, bool? hasBlockReleaseDeliveryOption)
        {
            _command = new AddProviderCourseLocationCommand(ukprn, larsCode, userId, Guid.NewGuid(), hasDayReleaseDeliveryOption, hasBlockReleaseDeliveryOption);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldNotHaveValidationErrorFor(c => c.HasDayReleaseDeliveryOption);
        }

        [TestCase(false, false)]
        [TestCase(null, null)]
        [TestCase(false, null)]
        [TestCase(null, false)]
        public async Task ValidateHasDayReleaseDeliveryOption_NotSelected_ReturnsError(bool hasDayReleaseDeliveryOption, bool hasBlockReleaseDeliveryOption)
        {
            _command = new AddProviderCourseLocationCommand(ukprn, larsCode, userId, Guid.NewGuid(), hasDayReleaseDeliveryOption, hasBlockReleaseDeliveryOption);

            var sut = new AddProviderCourseLocationCommandValidator(_providerReadRepositoryMock.Object, _providerCourseReadRepositoryMock.Object, _providerLocationsReadRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(_command);

            result.ShouldHaveValidationErrorFor(c => c.HasDayReleaseDeliveryOption);
        }
    }
}
