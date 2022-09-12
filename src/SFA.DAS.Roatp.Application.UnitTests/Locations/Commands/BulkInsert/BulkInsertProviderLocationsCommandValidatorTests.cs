using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkInsert
{
    [TestFixture]
    public class BulkInsertProviderLocationsCommandValidatorTests
    {
        private readonly string _userId = "userid";
        private const string EmptptySubregionIdsErrorMessage = "SubregionsIds is required";
        private const string SubRegionsAlreadyExistsErrorMessage = "All or some of the sub-regions already exist on the provider locations";
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId
            };

            var sut = new BulkInsertProviderLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId
            };
            var sut = new BulkInsertProviderLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = userId
            };
            var sut = new BulkInsertProviderLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>(), Mock.Of<IProviderLocationsReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [Test]
        public async Task ValidateSelectedSubregionIds_EmptySelectedSubregionIds_ReturnsError()
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId
            };

            Mock<IRegionsReadRepository> regionReadRepositoryMock = new Mock<IRegionsReadRepository>();
            regionReadRepositoryMock.Setup(r => r.GetAllRegions()).ReturnsAsync(new List<Domain.Entities.Region>());

            Mock<IProvidersReadRepository> providerReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providerReadRepositoryMock.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

            Mock<IProviderCoursesReadRepository> providerCourseReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
            providerCourseReadRepositoryMock.Setup(m => m.GetProviderCourse(It.IsAny<int>(), command.LarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());

            Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { Id = 1, RegionId = 1 } });

            var sut = new BulkInsertProviderLocationsCommandValidator(providerReadRepositoryMock.Object, providerCourseReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.SelectedSubregionIds);
            Assert.IsTrue(result.Errors.Exists(a => a.ErrorMessage.Contains(EmptptySubregionIdsErrorMessage)));
        }

        [Test]
        public async Task ValidateSelectedSubregionIds_RegionAlreadyExistsInProvideLocation_ReturnsError()
        {
            var command = new BulkInsertProviderLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId,
                SelectedSubregionIds = new List<int> { 1 }
            };
            Mock<IRegionsReadRepository> regionReadRepositoryMock = new Mock<IRegionsReadRepository>();
            regionReadRepositoryMock.Setup(r => r.GetAllRegions()).ReturnsAsync(new List<Domain.Entities.Region> { new Domain.Entities.Region { Id = 1, RegionName = "Test" } });

            Mock<IProvidersReadRepository> providerReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providerReadRepositoryMock.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

            Mock<IProviderCoursesReadRepository> providerCourseReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
            providerCourseReadRepositoryMock.Setup(m => m.GetProviderCourse(It.IsAny<int>(), command.LarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());

            Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { Id = 1, RegionId = 1} });

            var sut = new BulkInsertProviderLocationsCommandValidator(providerReadRepositoryMock.Object, providerCourseReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.SelectedSubregionIds);
            Assert.IsTrue(result.Errors.Exists(a => a.ErrorMessage.Contains(SubRegionsAlreadyExistsErrorMessage)));
        }
    }
}
