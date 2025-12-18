using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.DeleteProviderCourse
{
    [TestFixture]
    public class DeleteProviderCourseCommandValidatorTests
    {
        private readonly string _userId = "userid";
        private readonly string _userDisplayName = "userDisplayName";
        private readonly int _ukprn = 10012002;
        private readonly string _larsCode = "123";

        [Test]
        public async Task Validates_Ukprn_ReturnsError()
        {
            var command = new DeleteProviderCourseCommand(10012002, _larsCode, _userId, _userDisplayName);

            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task Validates_LarsCode_ReturnsError()
        {
            var command = new DeleteProviderCourseCommand(_ukprn, _larsCode, _userId, _userDisplayName);
            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new DeleteProviderCourseCommand(_ukprn, _larsCode, userId, _userDisplayName);
            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }

        [TestCase("Test")]
        public async Task ValidateUserId_NotEmpty_ReturnsNoErrors(string userId)
        {
            var command = new DeleteProviderCourseCommand(_ukprn, _larsCode, userId, _userDisplayName);
            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(c => c.UserId);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserDisplayName_Empty_ReturnsError(string userDisplayName)
        {
            var command = new DeleteProviderCourseCommand(_ukprn, _larsCode, _userId, userDisplayName);
            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserDisplayName);
        }

        [TestCase("Test")]
        public async Task ValidateUserDisplayName_NotEmpty_ReturnsNoErrors(string userDisplayName)
        {
            var command = new DeleteProviderCourseCommand(_ukprn, _larsCode, _userId, userDisplayName);
            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProvidersReadRepository>(), Mock.Of<IProviderCoursesReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(c => c.UserDisplayName);
        }
    }
}
