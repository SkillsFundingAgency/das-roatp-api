using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries.GetProvidersCountForCourse
{
    [TestFixture]
    public class GetProvidersCountForCourseQueryValidatorTests
    {
        [Test]
        public async Task ValidateLarsCode_InvalidValue_HasValidationError()
        {
            var query = new GetProvidersCountForCourseQuery(0);
            var repositoryMock = new Mock<IStandardsReadRepository>();
            var sut = new GetProvidersCountForCourseQueryValidator(repositoryMock.Object);

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(x => x.LarsCode);
            repositoryMock.Verify(s => s.GetStandard(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task ValidateLarsCode_InvalidStandard_HasValidationError()
        {
            var larsCode = 1;
            var query = new GetProvidersCountForCourseQuery(larsCode);
            var repositoryMock = new Mock<IStandardsReadRepository>();
            repositoryMock.Setup(r => r.GetStandard(larsCode)).ReturnsAsync((Standard)null);
            var sut = new GetProvidersCountForCourseQueryValidator(repositoryMock.Object);

            var result = await sut.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(x => x.LarsCode);
            repositoryMock.Verify(s => s.GetStandard(larsCode));
        }

        [Test]
        public async Task ValidateLarsCode_ValidStandard_IsValid()
        {
            var larsCode = 1;
            var query = new GetProvidersCountForCourseQuery(larsCode);
            var repositoryMock = new Mock<IStandardsReadRepository>();
            repositoryMock.Setup(r => r.GetStandard(larsCode)).ReturnsAsync(new Standard());
            var sut = new GetProvidersCountForCourseQueryValidator(repositoryMock.Object);

            var result = await sut.TestValidateAsync(query);

            result.ShouldNotHaveValidationErrorFor(x => x.LarsCode);
            repositoryMock.Verify(s => s.GetStandard(larsCode));
        }
    }
}
