﻿using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkDelete
{
    [TestFixture]
    public class BulkDeleteProviderLocationsCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_Records(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCourseLocationsReadRepository> providerCourseLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsBulkRepository> providerLocationsBulkRepositoryMock,
            BulkDeleteProviderLocationsCommand command,
            BulkDeleteProviderLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var providerId = 1;
            var regionalLocationId = 2;
            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation { Id = 1, ProviderId =providerId, LocationType = LocationType.Provider},
                new ProviderLocation { Id = regionalLocationId, ProviderId = providerId, LocationType = LocationType.Regional},
                new ProviderLocation { Id = 3, ProviderId = providerId, LocationType = LocationType.National}
            };
            var providerCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation { Id = 1, ProviderCourseId = 2, ProviderLocationId=1 } };

            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(providerLocations);
            providerCourseLocationsReadRepositoryMock.Setup(r => r.GetProviderCourseLocationsByUkprn(command.Ukprn)).ReturnsAsync(providerCourseLocations);

            var response = await sut.Handle(command, cancellationToken);

            providerLocationsBulkRepositoryMock.Verify(d => d.BulkDelete(It.Is<IEnumerable<int>>(x => x.Contains(regionalLocationId)), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Result, Is.EqualTo(1));
        }
        
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_NoRecords(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCourseLocationsReadRepository> providerCourseLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsBulkRepository> providerLocationsDeleteRepositoryMock,
            BulkDeleteProviderLocationsCommand command,
            BulkDeleteProviderLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var providerId = 1;
            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation { Id = 1, ProviderId = providerId, LocationType = LocationType.Provider},
                new ProviderLocation { Id = 3, ProviderId = providerId, LocationType = LocationType.National}
            };
            var providerCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation { Id = 1, ProviderCourseId = 1, ProviderLocationId = 1 } };
        
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(providerLocations);
            providerCourseLocationsReadRepositoryMock.Setup(r => r.GetProviderCourseLocationsByUkprn(It.IsAny<int>())).ReturnsAsync(providerCourseLocations);
        
            var response = await sut.Handle(command, cancellationToken);
        
            providerLocationsDeleteRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Result, Is.EqualTo(0));
        }
    }

}
