using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.BulkDelete
{
    [TestFixture]
    public class BulkDeleteProviderCourseLocationsCommandValidatorTests
    {
        private readonly string _userId = "userid";
        [Test]
        public async Task ValidateDeleteOption_NoneSelected_ReturnsError()
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, DeleteProviderCourseLocationOption.None, _userId);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.DeleteProviderCourseLocationOptions);
        }

        [TestCase(DeleteProviderCourseLocationOption.DeleteProviderLocations)]
        [TestCase(DeleteProviderCourseLocationOption.DeleteEmployerLocations)]
        public async Task ValidateDeleteOption_Selected_ReturnsValid(DeleteProviderCourseLocationOption options)
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, options, _userId);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(c => c.DeleteProviderCourseLocationOptions);
        }

        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, DeleteProviderCourseLocationOption.DeleteProviderLocations, _userId);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, DeleteProviderCourseLocationOption.DeleteProviderLocations, _userId);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new BulkDeleteProviderCourseLocationsCommand(10012002, 123, DeleteProviderCourseLocationOption.DeleteProviderLocations, userId);

            var sut = new BulkDeleteProviderCourseLocationsCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }
    }
}
