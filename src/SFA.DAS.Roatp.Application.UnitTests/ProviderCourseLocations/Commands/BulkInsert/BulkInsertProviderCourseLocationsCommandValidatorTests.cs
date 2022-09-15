using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.BulkInsert
{
    [TestFixture]
    public class BulkInsertProviderCourseLocationsCommandValidatorTests
    {
        private readonly string _userId = "userid";
        private const string EmptptySubregionIdsErrorMessage = "SubregionsIds is required";
        private const string SelectedSubregionIdsNotExistsinProviderLocationsErrorMessage = "Provider locations does not have any or some of the sub-regions being added on the course. It is required to add sub regions to the provider locations before associating them with a course";
        private const string SelectedSubregionIdsAlreadyExistsinProviderCourseLocationsErrorMessage = "All or some of the sub-regions are associated to the provider course. It is required that there are no national or regional locations associated to the course";
        Mock<IProvidersReadRepository> providersReadRepositoryMock;
        Mock<IProviderCoursesReadRepository> providerCoursesReadRepositoryMock;
        Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock;
        Mock<IProviderCourseLocationsReadRepository> providerCourseLocationsReadRepositoryMock;

        [SetUp]
        public void Setup()
        {
            providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providersReadRepositoryMock.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

            providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
            providerCoursesReadRepositoryMock.Setup(m => m.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Domain.Entities.ProviderCourse());

            providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { Id = 1, RegionId = 1 } });

            providerCourseLocationsReadRepositoryMock = new Mock<IProviderCourseLocationsReadRepository>();
            providerCourseLocationsReadRepositoryMock.Setup(l => l.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<ProviderCourseLocation> { new ProviderCourseLocation { Location = new ProviderLocation { LocationType = LocationType.Regional } } });
        }

        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 100,
                LarsCode = 123,
                UserId = _userId,
            };

            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providersReadRepositoryMock.Object, providerCoursesReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object, providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 0,
                UserId = _userId,
            };
            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providersReadRepositoryMock.Object, providerCoursesReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object, providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = userId,
            };


            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providersReadRepositoryMock.Object, providerCoursesReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object, providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [Test]
        public async Task ValidateSelectedSubregionIds_EmptySelectedSubregionIds_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId,
            };
            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providersReadRepositoryMock.Object, providerCoursesReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object, providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.SelectedSubregionIds);
            Assert.IsTrue(result.Errors.Exists(a => a.ErrorMessage.Contains(EmptptySubregionIdsErrorMessage)));
        }

        [Test]
        public async Task ValidateSelectedSubregionIds_ProviderDataNotFound_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId,
                SelectedSubregionIds = new List<int> { 10 }
            };
            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providersReadRepositoryMock.Object, providerCoursesReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object, providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.SelectedSubregionIds);
            Assert.IsTrue(result.Errors.Exists(a => a.ErrorMessage.Contains(SelectedSubregionIdsNotExistsinProviderLocationsErrorMessage)));
        }

        [Test]
        public async Task ValidateSelectedSubregionIds_SubregionIdsAlreadyExistsinProviderCourseLocations_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId,
                SelectedSubregionIds = new List<int> { 1 }
            };
            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providersReadRepositoryMock.Object, providerCoursesReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object, providerCourseLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.SelectedSubregionIds);
            Assert.IsTrue(result.Errors.Exists(a => a.ErrorMessage.Contains(SelectedSubregionIdsAlreadyExistsinProviderCourseLocationsErrorMessage)));
        }
    }
}
