using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.AddNationalLocation
{
    [TestFixture]
    public class AddNationalLocationToProviderCourseLocationsCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_NationalLocationMissing_CreatesNationalLocation(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsWriteRepository> providerLocationWriteRepositoryMock,
            [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
            AddNationalLocationToProviderCourseLocationsCommandHandler sut,
            Provider provider,
            List<ProviderLocation> providerLocations,
            AddNationalLocationToProviderCourseLocationsCommand command
            )
        {
            providerLocations.ForEach(p => p.LocationType = LocationType.Provider);
            providersReadRepositoryMock.Setup(m => m.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);
            providerLocationsReadRepositoryMock.Setup(m => m.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(providerLocations);

            await sut.Handle(command, new CancellationToken());

            providerLocationWriteRepositoryMock.Verify(p => p.Create(It.Is<ProviderLocation>(l => l.LocationType == LocationType.National)));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_NationalLocationEsits_CreatesNationalLocation(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsWriteRepository> providerLocationWriteRepositoryMock,
            [Frozen] Mock<IProviderCoursesReadRepository> providerCourseReadRepositoryMock,
            [Frozen] Mock<IProvidersReadRepository> providerReadRepositoryMock,
            [Frozen] Mock<IProviderCourseLocationsWriteRepository> providerCourseLocationsWriteRepositoryMock,
            AddNationalLocationToProviderCourseLocationsCommandHandler sut,
            Provider provider,
            AddNationalLocationToProviderCourseLocationsCommand command,
            Domain.Entities.ProviderCourse providerCourse,
            ProviderLocation providerLocation
            )
        {
            providerLocation.LocationType = LocationType.National;
            var providerLocations = new List<ProviderLocation> { providerLocation };

            providerReadRepositoryMock.Setup(m => m.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);
            providerLocationsReadRepositoryMock.Setup(m => m.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(providerLocations);
            providerCourseReadRepositoryMock.Setup(m => m.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourse);

            await sut.Handle(command, new CancellationToken());

            providerLocationWriteRepositoryMock.Verify(p => p.Create(It.IsAny<ProviderLocation>()), Times.Never);
            providerCourseLocationsWriteRepositoryMock.Verify(m => m.Create(It.Is<ProviderCourseLocation>(l => l.ProviderLocationId == providerLocation.Id && !l.IsImported && l.ProviderCourseId == providerCourse.Id)));
        }
    }
}
