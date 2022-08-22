using Moq;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    public abstract class CreateProviderCourseCommandValidatorTestBase
    {
        protected Mock<IProviderReadRepository> ProviderReadRepositoryMock;
        protected Mock<IProviderCourseReadRepository> ProviderCourseReadRepositoryMock;
        protected Mock<IStandardReadRepository> StandardReadRepositoryMock;
        protected const int ValidUkprn = 10012002;
        protected const int ValidComboLarsCode = 321;
        protected const int RegulatedLarsCode = 123;
        protected const int NonRegulatedLarsCode = 111;

        protected CreateProviderCourseCommandValidator GetSut()
        {
            ProviderReadRepositoryMock = new Mock<IProviderReadRepository>();
            ProviderReadRepositoryMock.Setup(p => p.GetByUkprn(ValidUkprn)).ReturnsAsync(new Provider { Ukprn = ValidUkprn });

            StandardReadRepositoryMock = new Mock<IStandardReadRepository>();
            StandardReadRepositoryMock
                .Setup(r => r.GetStandard(It.Is<int>(i => i == ValidComboLarsCode || i == NonRegulatedLarsCode)))
                .ReturnsAsync(new Standard());
            StandardReadRepositoryMock
                .Setup(r => r.GetStandard(It.Is<int>(i => i == RegulatedLarsCode)))
                .ReturnsAsync(new Standard { ApprovalBody = Guid.NewGuid().ToString() });

            ProviderCourseReadRepositoryMock = new Mock<IProviderCourseReadRepository>();
            ProviderCourseReadRepositoryMock.Setup(r => r.GetProviderCourseByUkprn(ValidUkprn, ValidComboLarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());

            return new CreateProviderCourseCommandValidator(ProviderReadRepositoryMock.Object, StandardReadRepositoryMock.Object, ProviderCourseReadRepositoryMock.Object);
        }
    }
}
