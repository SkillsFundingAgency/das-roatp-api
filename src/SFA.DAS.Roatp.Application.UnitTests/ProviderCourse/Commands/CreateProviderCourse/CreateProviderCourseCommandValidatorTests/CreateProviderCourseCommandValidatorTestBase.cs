using System;
using System.Collections.Generic;
using Moq;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseCommandValidatorTests
{
    public abstract class CreateProviderCourseCommandValidatorTestBase
    {
        protected Mock<IProvidersReadRepository> providersReadRepositoryMock;
        protected Mock<IProviderCoursesReadRepository> providerCoursesReadRepositoryMock;
        protected Mock<IStandardsReadRepository> standardsReadRepositoryMock;
        protected Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock;
        protected Mock<IRegionsReadRepository> regionsReadRepositoryMock;
        public const int ValidUkprn = 10012002;
        protected const int ValidComboLarsCode = 321;
        protected const int RegulatedLarsCode = 123;
        protected const int NonRegulatedLarsCode = 111;
        public static Guid NavigationId = new Guid("f26bac30-23a8-11ed-861d-0242ac120002");
        protected const int ValidRegionId = 9;

        protected CreateProviderCourseCommandValidator GetSut()
        {
            providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            providersReadRepositoryMock.Setup(p => p.GetByUkprn(ValidUkprn)).ReturnsAsync(new Provider { Ukprn = ValidUkprn });

            standardsReadRepositoryMock = new Mock<IStandardsReadRepository>();
            standardsReadRepositoryMock
                .Setup(r => r.GetStandard(It.IsAny<int>()))
                .ReturnsAsync(new Standard { IsRegulatedForProvider = false });
            standardsReadRepositoryMock
                .Setup(r => r.GetStandard(It.Is<int>(i => i == ValidComboLarsCode || i == NonRegulatedLarsCode)))
                .ReturnsAsync(new Standard { IsRegulatedForProvider = false });
            standardsReadRepositoryMock
                .Setup(r => r.GetStandard(It.Is<int>(i => i == RegulatedLarsCode)))
                .ReturnsAsync(new Standard { IsRegulatedForProvider = true });

            providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
            providerCoursesReadRepositoryMock.Setup(r => r.GetProviderCourseByUkprn(ValidUkprn, ValidComboLarsCode)).ReturnsAsync(new Domain.Entities.ProviderCourse());

            providerLocationsReadRepositoryMock = new Mock<IProviderLocationsReadRepository>();
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(ValidUkprn)).ReturnsAsync(new List<ProviderLocation> { new ProviderLocation { NavigationId = NavigationId } });
            regionsReadRepositoryMock = new Mock<IRegionsReadRepository>();
            regionsReadRepositoryMock.Setup(r => r.GetAllRegions()).ReturnsAsync(new List<Region> { new Region { Id = ValidRegionId } });

            return new CreateProviderCourseCommandValidator(providersReadRepositoryMock.Object, standardsReadRepositoryMock.Object, providerCoursesReadRepositoryMock.Object, providerLocationsReadRepositoryMock.Object, regionsReadRepositoryMock.Object);
        }
    }
}
