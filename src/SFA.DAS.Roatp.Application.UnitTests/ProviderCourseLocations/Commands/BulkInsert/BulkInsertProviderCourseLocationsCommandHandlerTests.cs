using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.BulkInsert
{
    [TestFixture]
    public class BulkInsertProviderCourseLocationsCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Inserts_Records(
            [Frozen] Mock<IProviderReadRepository> providerReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCourseReadRepository> providerCourseReadRepositoryMock,
            BulkInsertProviderCourseLocationsCommand command,
            BulkInsertProviderCourseLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var provider = new Provider { Id = 1, Ukprn = command.Ukprn };
            var providerLocations = new List<ProviderLocation> { new ProviderLocation { Id = 1, ProviderId = 1, RegionId = command.SelectedSubregionIds.First() } };
            var providerCourse = new List<Domain.Entities.ProviderCourse>() { new Domain.Entities.ProviderCourse { ProviderId = 1, Id = 1, LarsCode = command.LarsCode } };
            providerReadRepositoryMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);

            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(providerLocations);

            providerCourseReadRepositoryMock.Setup(r => r.GetAllProviderCourses(It.IsAny<int>())).ReturnsAsync(providerCourse);

            command.SelectedSubregionIds = new List<int> { providerLocations.FirstOrDefault(a => a.RegionId.HasValue).RegionId.Value };

            var result = await sut.Handle(command, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(1));
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Inserts_NoRecords(
            [Frozen] Mock<IProviderReadRepository> providerReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCourseReadRepository> providerCourseReadRepositoryMock,
            Provider provider,
            List<ProviderLocation> providerLocations,
            Domain.Entities.ProviderCourse providerCourse,
            BulkInsertProviderCourseLocationsCommand command,
            BulkInsertProviderCourseLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            providerReadRepositoryMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);

            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(providerLocations);

            providerCourseReadRepositoryMock.Setup(r => r.GetProviderCourseByUkprn(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourse);

            command.SelectedSubregionIds = new List<int>();

            var result = await sut.Handle(command, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(command.SelectedSubregionIds.Count));
        }
    }

}
