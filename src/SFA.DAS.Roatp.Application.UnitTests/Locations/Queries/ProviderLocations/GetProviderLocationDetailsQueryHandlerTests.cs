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
            GetProviderLocationDetailsQuery query,
            GetProviderLocationDetailsQueryHandler sut,
            CancellationToken cancellationToken)
        {
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

            result.Standards[0].Title.Should().Be(location.ProviderCourseLocations[0].ProviderCourse.Standard.Title);
            result.Standards[0].Level.Should().Be(location.ProviderCourseLocations[0].ProviderCourse.Standard.Level);
            result.Standards[0].LarsCode.Should().Be(location.ProviderCourseLocations[0].ProviderCourse.Standard.LarsCode);
            result.Standards[0].HasOtherVenues.Should().Be(false);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult_OneStandard_HasOtherVenues(
            List<Standard> standards,
            ProviderLocation location,
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepository,
            GetProviderLocationDetailsQuery query,
            GetProviderLocationDetailsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            var matchedLarsCode = standards[0].LarsCode;
            location.ProviderCourseLocations[0].ProviderCourse.LarsCode = matchedLarsCode;
            location.ProviderCourseLocations[0].ProviderCourse.Standard = standards[0];
            location.ProviderCourseLocations[1].ProviderCourse.LarsCode = matchedLarsCode;
            location.ProviderCourseLocations[1].ProviderCourse.Standard = standards[0];

            location.Provider.Courses[0].LarsCode = matchedLarsCode;
            location.Provider.Courses[0].Standard = standards[0];
            location.Provider.Courses[1].LarsCode = matchedLarsCode;
            location.Provider.Courses[1].Standard = standards[0];

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

            result.Standards[0].HasOtherVenues.Should().Be(true);
        }
    }
}
