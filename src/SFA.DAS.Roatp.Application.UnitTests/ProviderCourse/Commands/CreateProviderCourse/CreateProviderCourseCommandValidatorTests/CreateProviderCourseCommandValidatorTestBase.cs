using Moq;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    public abstract class CreateProviderCourseCommandValidatorTestBase
    {
        protected Mock<IProvidersReadRepository> ProviderReadRepositoryMock;
        protected Mock<IProviderCourseReadRepository> ProviderCourseReadRepositoryMock;
        protected Mock<IStandardsReadRepository> StandardReadRepositoryMock;
        protected Mock<IProviderLocationsReadRepository> ProviderLocationsReadRepositoryMock;
        protected Mock<IRegionsReadRepository> RegionReadRepositoryMock;
        public const int ValidUkprn = 10012002;
        protected const int ValidComboLarsCode = 321;
        protected const int RegulatedLarsCode = 123;
        protected const int NonRegulatedLarsCode = 111;
        public static Guid NavigationId = new Guid("f26bac30-23a8-11ed-861d-0242ac120002");
        protected const int ValidRegionId = 9;

        protected CreateProviderCourseCommandValidator GetSut()
        {
            ProviderReadRepositoryMock = new Mock<IProvidersReadRepository>();
            ProviderReadRepositoryMock.Setup(p => p.GetByUkprn(ValidUkprn)).ReturnsAsync(new Provider { Ukprn = ValidUkprn });

            StandardReadRepositoryMock = new Mock<IStandardsReadRepository>();
            StandardReadRepositoryMock
                .Setup(r => r.GetStandard(It.Is<int>(i => i == ValidComboLarsCode || i == NonRegulatedLarsCode)))
                .ReturnsAsync(new Standard());
            StandardReadRepositoryMock
                .Setup(r => r.GetStandard(It.Is<int>(i => i == RegulatedLarsCode)))
                .ReturnsAsync(new Standard { ApprovalBody = Guid.NewGuid().ToString() });

            ProviderCourseReadRepositoryMock = new Mock<IProviderCourseReadRepository>();
            ProviderCourseReadRepositoryMock.Setup(r => r.GetProviderCourseByUkprn(ValidUkprn, ValidComboLarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());

            ProviderLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            ProviderLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(ValidUkprn)).ReturnsAsync(new List<ProviderLocation> {new ProviderLocation {NavigationId = NavigationId}});
            RegionReadRepositoryMock = new Mock<IRegionsReadRepository>();
            RegionReadRepositoryMock.Setup(r => r.GetAllRegions()).ReturnsAsync(new List<Region> { new Region { Id = ValidRegionId } });

            return new CreateProviderCourseCommandValidator(ProviderReadRepositoryMock.Object, StandardReadRepositoryMock.Object, ProviderCourseReadRepositoryMock.Object,ProviderLocationsReadRepositoryMock.Object, RegionReadRepositoryMock.Object);
        }
    }
}
