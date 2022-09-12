using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse.CreateProviderCourseHandlerTests
{
    public class CreateProviderCourseHandlerWhenEmployerLocationOptionWithRegionsTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_HasSelectedExistingSubregions_UsesExistingProviderLocationId(
        [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
        [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
        [Frozen] Mock<IProviderCoursesWriteRepository> providerCoursesWriteRepositoryMock,
        CreateProviderCourseCommandHandler sut,
        CreateProviderCourseCommand command,
        Provider provider,
        ProviderLocation providerLocation)
        {
            providerLocation.LocationType = LocationType.Regional;
            command.HasNationalDeliveryOption = false;
            command.ProviderLocations = null;
            command.SubregionIds = new List<int> { providerLocation.RegionId.GetValueOrDefault() };
            providersReadRepositoryMock.Setup(p => p.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);
            providerLocationsReadRepositoryMock.Setup(p => p.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(new List<ProviderLocation>() { providerLocation });

            await sut.Handle(command, new CancellationToken());

            providerCoursesWriteRepositoryMock.Verify(p => p.CreateProviderCourse(It.Is<Domain.Entities.ProviderCourse>(c => c.ProviderId == provider.Id && c.Locations.Count == 1 && c.Locations.First().ProviderLocationId == providerLocation.Id && c.Locations.First().Location == null)));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_HasSelectedNonExistingSubregions_CreatesNewProviderLocation(
            [Frozen] Mock<IProvidersReadRepository> providerReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCoursesWriteRepository> providerCourseEditRepositoryMock,
            [Frozen] Mock<IRegionsReadRepository> regionReadRepositoryMock,
            CreateProviderCourseCommandHandler sut,
            CreateProviderCourseCommand command,
            Provider provider,
            ProviderLocation providerLocation,
            Region region)
        {
            providerLocation.LocationType = LocationType.Regional;
            command.HasNationalDeliveryOption = false;
            command.ProviderLocations = null;
            command.SubregionIds = new List<int> { region.Id };
            regionReadRepositoryMock.Setup(r => r.GetAllRegions()).ReturnsAsync(new List<Region> { region });
            providerReadRepositoryMock.Setup(p => p.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);
            providerLocationsReadRepositoryMock.Setup(p => p.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(new List<ProviderLocation>() { providerLocation });

            await sut.Handle(command, new CancellationToken());

            providerCourseEditRepositoryMock.Verify(p => p.CreateProviderCourse(It.Is<Domain.Entities.ProviderCourse>(c => c.ProviderId == provider.Id && c.Locations.Count == 1 && c.Locations.First().Location != null)));
        }
    }
}
