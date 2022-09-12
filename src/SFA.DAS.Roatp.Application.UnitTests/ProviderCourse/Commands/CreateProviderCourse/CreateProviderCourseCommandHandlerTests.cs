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

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Commands.CreateProviderCourse
{
    [TestFixture]
    public class CreateProviderCourseCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_HasNationalDeliveryOptionOnly_AddsNewNationalLocation(
            [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCoursesWriteRepository> providerCoursesWriteRepositoryMock,
            CreateProviderCourseCommandHandler sut,
            CreateProviderCourseCommand command,
            Provider provider)
        {
            command.HasNationalDeliveryOption = true;
            command.ProviderLocations = null;
            command.SubregionIds = null;
            providersReadRepositoryMock.Setup(p => p.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);
            providerLocationsReadRepositoryMock.Setup(p => p.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(new List<ProviderLocation>());

            await sut.Handle(command, new CancellationToken());

            providerCoursesWriteRepositoryMock.Verify(p => p.CreateProviderCourse(It.Is<Domain.Entities.ProviderCourse>(c => c.ProviderId == provider.Id && c.Locations.Count == 1 && c.Locations.First().Location.LocationType == LocationType.National && c.Locations.First().ProviderLocationId == 0)));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_HasNationalDeliveryOptionOnly_AssociatesExistingNationalLocation(
            [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCoursesWriteRepository> providerCoursesWriteRepositoryMock,
            CreateProviderCourseCommandHandler sut,
            CreateProviderCourseCommand command,
            Provider provider,
            ProviderLocation providerLocation)
        {
            providerLocation.LocationType = LocationType.National;
            command.HasNationalDeliveryOption = true;
            command.ProviderLocations = null;
            command.SubregionIds = null;
            providersReadRepositoryMock.Setup(p => p.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);
            providerLocationsReadRepositoryMock.Setup(p => p.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(new List<ProviderLocation>() { providerLocation});

            await sut.Handle(command, new CancellationToken());

            providerCoursesWriteRepositoryMock.Verify(p => p.CreateProviderCourse(It.Is<Domain.Entities.ProviderCourse>(c => c.ProviderId == provider.Id && c.Locations.Count == 1 && c.Locations.First().ProviderLocationId == providerLocation.Id)));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_HasProviderLocationsOnly_AddProviderLocations(
            [Frozen] Mock<IProvidersReadRepository> providerReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCoursesWriteRepository> providerCourseEditRepositoryMock,
            CreateProviderCourseCommandHandler sut,
            CreateProviderCourseCommand command,
            Provider provider)
        {
            command.HasNationalDeliveryOption = false;
            command.SubregionIds = null;
            providerReadRepositoryMock.Setup(p => p.GetByUkprn(command.Ukprn)).ReturnsAsync(provider);
            providerLocationsReadRepositoryMock.Setup(p => p.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(provider.Locations);

            command.ProviderLocations = new List<ProviderCourseLocationCommandModel>();
            foreach (var location in provider.Locations)
            {
                command.ProviderLocations.Add(new ProviderCourseLocationCommandModel { ProviderLocationId = location.NavigationId });
            }

            var numberOfLocations = provider.Locations.Count;

            await sut.Handle(command, new CancellationToken());

            providerCourseEditRepositoryMock.Verify(p => p.CreateProviderCourse(It.Is<Domain.Entities.ProviderCourse>(c => c.ProviderId == provider.Id && c.Locations.Count == numberOfLocations && c.Locations.First().ProviderLocationId == provider.Locations.First().Id)));
        }
    }
}
