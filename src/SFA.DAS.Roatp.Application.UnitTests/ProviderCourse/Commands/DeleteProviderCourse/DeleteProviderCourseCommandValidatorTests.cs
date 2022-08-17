using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.DeleteProviderCourse
{
    [TestFixture]
    public class DeleteProviderCourseCommandValidatorTests
    {
        private readonly string _userId = "userid";
        private readonly int _ukprn = 10012002;
        private readonly int _larsCode = 123;
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new DeleteProviderCourseCommand(10012002, _larsCode, _userId);

            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new DeleteProviderCourseCommand(_ukprn, _larsCode, _userId);
            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new DeleteProviderCourseCommand(_ukprn, _larsCode, userId);
            var sut = new DeleteProviderCourseCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }
    }
}
