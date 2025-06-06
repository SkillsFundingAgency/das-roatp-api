using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetProviderLocationDetailsQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            ProviderLocation location,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepository,
            [Frozen] Mock<IStandardsReadRepository> standardReadRepositoryMock,
            GetProviderLocationDetailsQuery query,
            GetProviderLocationDetailsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            standardReadRepositoryMock.Setup(x => x.GetAllStandards()).ReturnsAsync(new List<Standard>());
            providerLocationsReadRepository.Setup(r => r.GetProviderLocation(query.Ukprn, query.Id)).ReturnsAsync(location);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Should().NotBeNull();
            var result = response.Result as ProviderLocationModel;
            result.Should().BeEquivalentTo(location, c => c
                .Excluding(s => s.Id)
                .Excluding(s => s.ProviderId)
                .Excluding(s => s.Provider)
                .Excluding(s => s.ImportedLocationId)
                .Excluding(s => s.Region)
                .Excluding(s => s.ProviderCourseLocations));

            result.ProviderLocationId.Should().Be(location.Id);

            result.Standards.Should().BeEquivalentTo(new List<Standard>());
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult_With_Standards(
            List<Standard> standards,
            ProviderLocation location,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepository,
            [Frozen] Mock<IStandardsReadRepository> standardReadRepositoryMock,
            GetProviderLocationDetailsQuery query,
            GetProviderLocationDetailsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            var matchedLarsCode = standards[0].LarsCode;
            location.ProviderCourseLocations[0].ProviderCourse.LarsCode = matchedLarsCode;

            standardReadRepositoryMock.Setup(x => x.GetAllStandards()).ReturnsAsync(standards);
            providerLocationsReadRepository.Setup(r => r.GetProviderLocation(query.Ukprn, query.Id)).ReturnsAsync(location);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Should().NotBeNull();
            var result = response.Result as ProviderLocationModel;
            result.Should().BeEquivalentTo(location, c => c
                .Excluding(s => s.Id)
                .Excluding(s => s.ProviderId)
                .Excluding(s => s.Provider)
                .Excluding(s => s.ImportedLocationId)
                .Excluding(s => s.Region)
                .Excluding(s => s.ProviderCourseLocations));

            result.ProviderLocationId.Should().Be(location.Id);

            result.Standards.Should().BeEquivalentTo(standards.Where(x => x.LarsCode == matchedLarsCode),
                c => c
                .Excluding(s => s.StandardUId)
                .Excluding(s => s.IfateReferenceNumber)
                .Excluding(s => s.Version)
                .Excluding(s => s.ApprovalBody)
                .Excluding(s => s.SectorSubjectAreaTier1)
            );

            result.Standards[0].HasOtherVenues.Should().Be(false);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult_OneStandard_HasOtherVenues(
            List<Standard> standards,
            ProviderLocation location,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepository,
            [Frozen] Mock<IStandardsReadRepository> standardReadRepositoryMock,
            GetProviderLocationDetailsQuery query,
            GetProviderLocationDetailsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            var matchedLarsCode = standards[0].LarsCode;
            location.ProviderCourseLocations[0].ProviderCourse.LarsCode = matchedLarsCode;
            location.ProviderCourseLocations[1].ProviderCourse.LarsCode = matchedLarsCode;

            location.Provider.Courses[0].LarsCode = matchedLarsCode;


            standardReadRepositoryMock.Setup(x => x.GetAllStandards()).ReturnsAsync(standards);
            providerLocationsReadRepository.Setup(r => r.GetProviderLocation(query.Ukprn, query.Id)).ReturnsAsync(location);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Should().NotBeNull();
            var result = response.Result as ProviderLocationModel;
            result.Should().BeEquivalentTo(location, c => c
                .Excluding(s => s.Id)
                .Excluding(s => s.ProviderId)
                .Excluding(s => s.Provider)
                .Excluding(s => s.ImportedLocationId)
                .Excluding(s => s.Region)
                .Excluding(s => s.ProviderCourseLocations));

            result.ProviderLocationId.Should().Be(location.Id);

            result.Standards.Should().BeEquivalentTo(standards.Where(x => x.LarsCode == matchedLarsCode),
                c => c
                    .Excluding(s => s.StandardUId)
                    .Excluding(s => s.IfateReferenceNumber)
                    .Excluding(s => s.Version)
                    .Excluding(s => s.ApprovalBody)
                    .Excluding(s => s.SectorSubjectAreaTier1)
            );

            result.Standards[0].HasOtherVenues.Should().Be(true);
        }
    }
}
