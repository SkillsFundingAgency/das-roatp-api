using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    [TestFixture]
    public class AddProviderCourseLocationCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_CreatesProviderCourseLocation(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCoursesReadRepository> providerCourseReadRepositoryMock,
            [Frozen] Mock<IProviderCourseLocationsWriteRepository> providerCourseLocationsWriteRepositoryMock,
            AddProviderCourseLocationCommandHandler sut,
            ProviderLocation providerLocation,
            Domain.Entities.ProviderCourse providerCourse,
            AddProviderCourseLocationCommand command
            )
        {
            providerLocation.LocationType = LocationType.Provider;
            providerLocationsReadRepositoryMock.Setup(m => m.GetProviderLocation(It.IsAny<int>(), It.IsAny<Guid>())).ReturnsAsync(providerLocation);
            providerCourseReadRepositoryMock.Setup(m => m.GetProviderCourseByUkprn(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourse);


            await sut.Handle(command, new CancellationToken());

            providerCourseLocationsWriteRepositoryMock.Verify(p => p.Create(It.IsAny<ProviderCourseLocation>()), Times.Once);
            providerCourseLocationsWriteRepositoryMock.Verify(m => m.Create(It.Is<ProviderCourseLocation>(l => l.ProviderLocationId == providerLocation.Id && l.ProviderCourseId == providerCourse.Id && l.HasDayReleaseDeliveryOption == command.HasDayReleaseDeliveryOption && l.HasBlockReleaseDeliveryOption == command.HasBlockReleaseDeliveryOption)));
        }
    }
}
