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
        private const string ProviderDataNotFoundErrorMessage = "Relevant provider data not found to insert provider course locations";
        Mock<IProviderReadRepository> providerReadRepositoryMock;
        Mock<IProviderCourseReadRepository> providerCourseReadRepositoryMock;
        Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock;

        [SetUp]
        public void Setup()
        {
            providerReadRepositoryMock = new Mock<IProviderReadRepository>();
            providerReadRepositoryMock.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

            providerCourseReadRepositoryMock = new Mock<IProviderCourseReadRepository>();
            providerCourseReadRepositoryMock.Setup(m => m.GetAllProviderCourses(It.IsAny<int>())).ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

            providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { Id = 1, RegionId = 1 } });
        }

        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId,
                SelectedSubregionIds = new List<int> { 1 }
            };

            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providerReadRepositoryMock.Object, providerCourseReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId,
                SelectedSubregionIds = new List<int> { 1 }
            };
            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providerReadRepositoryMock.Object, providerCourseReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

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
                SelectedSubregionIds = new List<int> { 1 }
            };
            

            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providerReadRepositoryMock.Object, providerCourseReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [Test]
        public async Task ValidateUkprn_RegionAlreadyExistsInProvideLocation_ReturnsError()
        {
            var command = new BulkInsertProviderCourseLocationsCommand
            {
                Ukprn = 10012002,
                LarsCode = 123,
                UserId = _userId,
                SelectedSubregionIds = new List<int> { 1 }
            };
            var sut = new BulkInsertProviderCourseLocationsCommandValidator(providerReadRepositoryMock.Object, providerCourseReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
            Assert.IsTrue(result.Errors.Exists(a => a.ErrorMessage.Contains(ProviderDataNotFoundErrorMessage)));
        }
    }
}
