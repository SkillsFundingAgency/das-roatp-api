using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.Delete
{
    [TestFixture]
    public class DeleteProviderCourseLocationCommandValidatorTests
    {
        private readonly string _userId = "userid";
        [Test]
        public async Task ValidateId_LessThanZero_ReturnsError()
        {
            var command = new DeleteProviderCourseLocationCommand(10012002, 123, 0, _userId);

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>(), Mock.Of<IProviderCourseLocationReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

       
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new DeleteProviderCourseLocationCommand(10012002, 123, 1, _userId);

            var _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationReadRepository>();
            _providerCourseLocationReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.ProviderCourseLocation> { new Domain.Entities.ProviderCourseLocation { Id = 1 } });

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>(), _providerCourseLocationReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new DeleteProviderCourseLocationCommand(10012002, 123, 1, _userId);

            var _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationReadRepository>();
            _providerCourseLocationReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.ProviderCourseLocation> { new Domain.Entities.ProviderCourseLocation { Id = 1 } });

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>(), _providerCourseLocationReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new DeleteProviderCourseLocationCommand(10012002, 123, 1, userId);

            var _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationReadRepository>();
            _providerCourseLocationReadRepositoryMock
                .Setup(x => x.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new System.Collections.Generic.List<Domain.Entities.ProviderCourseLocation> { new Domain.Entities.ProviderCourseLocation { Id = 1 } });

            var sut = new DeleteProviderCourseLocationCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>(), _providerCourseLocationReadRepositoryMock.Object);

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }
    }
}
