using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForLarsCode;

[TestFixture]
public class GetProvidersForLarsCodeQueryTests
{
    [Test, RecursiveMoqAutoData]
    public void Constructor_V2Request_MapsScalarsAndFiltersNulls(
        string larsCode,
        decimal? latitude,
        decimal? longitude,
        string location,
        ProviderOrderBy orderBy,
        decimal? distance,
        int? page,
        int? pageSize,
        Guid? userId)
    {
        var request = new GetProvidersFromLarsCodeRequestV2
        {
            Latitude = latitude,
            Longitude = longitude,
            Location = location,
            OrderBy = orderBy,
            Distance = distance,
            Page = page,
            PageSize = pageSize,
            UserId = userId,
            DeliveryModes = new List<DeliveryModeV2?> { null, DeliveryModeV2.Provider, DeliveryModeV2.DayRelease },
            EmployerProviderRatings = new List<ProviderRating?> { null, ProviderRating.Good },
            ApprenticeProviderRatings = new List<ProviderRating?> { null, ProviderRating.Excellent },
            Qar = new List<QarRating?> { null, QarRating.Good }
        };

        var sut = new GetProvidersForLarsCodeQuery(larsCode, request);

        sut.LarsCode.Should().Be(larsCode);
        sut.Latitude.Should().Be(latitude);
        sut.Longitude.Should().Be(longitude);
        sut.Location.Should().Be(location);
        sut.OrderBy.Should().Be(orderBy);
        sut.Distance.Should().Be(distance);
        sut.Page.Should().Be(page);
        sut.PageSize.Should().Be(pageSize);
        sut.UserId.Should().Be(userId);

        sut.DeliveryModes.Should().BeEquivalentTo(new[] { DeliveryModeV2.Provider, DeliveryModeV2.DayRelease });
        sut.EmployerProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Good });
        sut.ApprenticeProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Excellent });
        sut.Qar.Should().BeEquivalentTo(new[] { QarRating.Good });
    }

    [Test, RecursiveMoqAutoData]
    public void Constructor_V2Request_NullCollections_ResultEmptyLists(
        string larsCode,
        GetProvidersFromLarsCodeRequestV2 request)
    {
        request.DeliveryModes = null;
        request.EmployerProviderRatings = null;
        request.ApprenticeProviderRatings = null;
        request.Qar = null;

        var sut = new GetProvidersForLarsCodeQuery(larsCode, request);

        sut.DeliveryModes.Should().NotBeNull();
        sut.DeliveryModes.Should().BeEmpty();

        sut.EmployerProviderRatings.Should().NotBeNull();
        sut.EmployerProviderRatings.Should().BeEmpty();

        sut.ApprenticeProviderRatings.Should().NotBeNull();
        sut.ApprenticeProviderRatings.Should().BeEmpty();

        sut.Qar.Should().NotBeNull();
        sut.Qar.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public void Constructor_V1Request_MapsScalarsAndCastsDeliveryModes(
        string larsCode,
        decimal? latitude,
        decimal? longitude,
        string location,
        ProviderOrderBy orderBy,
        decimal? distance,
        int? page,
        int? pageSize,
        Guid? userId)
    {
        var request = new GetProvidersFromLarsCodeRequest
        {
            Latitude = latitude,
            Longitude = longitude,
            Location = location,
            OrderBy = orderBy,
            Distance = distance,
            Page = page,
            PageSize = pageSize,
            UserId = userId,
            DeliveryModes = new List<DeliveryModeV1?> { null, DeliveryModeV1.Provider, DeliveryModeV1.Workplace },
            EmployerProviderRatings = new List<ProviderRating?> { null, ProviderRating.Poor },
            ApprenticeProviderRatings = new List<ProviderRating?> { null, ProviderRating.Good },
            Qar = new List<QarRating?> { null, QarRating.Poor }
        };

        var sut = new GetProvidersForLarsCodeQuery(larsCode, request);

        sut.LarsCode.Should().Be(larsCode);
        sut.Latitude.Should().Be(latitude);
        sut.Longitude.Should().Be(longitude);
        sut.Location.Should().Be(location);
        sut.OrderBy.Should().Be(orderBy);
        sut.Distance.Should().Be(distance);
        sut.Page.Should().Be(page);
        sut.PageSize.Should().Be(pageSize);
        sut.UserId.Should().Be(userId);

        var expectedModes = request.DeliveryModes!
            .Where(x => x != null)
            .Select(x => (DeliveryModeV2)x!)
            .ToList();

        sut.DeliveryModes.Should().BeEquivalentTo(expectedModes);
        sut.EmployerProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Poor });
        sut.ApprenticeProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Good });
        sut.Qar.Should().BeEquivalentTo(new[] { QarRating.Poor });
    }

    [Test, RecursiveMoqAutoData]
    public void Constructor_V1Request_NullCollections_ResultEmptyLists(
        string larsCode,
        GetProvidersFromLarsCodeRequest request)
    {
        request.DeliveryModes = null;
        request.EmployerProviderRatings = null;
        request.ApprenticeProviderRatings = null;
        request.Qar = null;

        var sut = new GetProvidersForLarsCodeQuery(larsCode, request);

        sut.DeliveryModes.Should().NotBeNull();
        sut.DeliveryModes.Should().BeEmpty();

        sut.EmployerProviderRatings.Should().NotBeNull();
        sut.EmployerProviderRatings.Should().BeEmpty();

        sut.ApprenticeProviderRatings.Should().NotBeNull();
        sut.ApprenticeProviderRatings.Should().BeEmpty();

        sut.Qar.Should().NotBeNull();
        sut.Qar.Should().BeEmpty();
    }

    [Test]
    public void Properties_PrivateSetters_ReadOnlyExternally()
    {
        var request = new GetProvidersFromLarsCodeRequestV2
        {
            Latitude = 1.23m,
            Longitude = 4.56m,
            Location = "Somewhere",
            OrderBy = ProviderOrderBy.Distance,
            Distance = 10m,
            Page = 2,
            PageSize = 25,
            UserId = Guid.NewGuid()
        };

        var sut = new GetProvidersForLarsCodeQuery("123", request);

        sut.Latitude.Should().Be(1.23m);
        sut.Longitude.Should().Be(4.56m);
        sut.Location.Should().Be("Somewhere");
        sut.OrderBy.Should().Be(ProviderOrderBy.Distance);
        sut.Distance.Should().Be(10m);
        sut.Page.Should().Be(2);
        sut.PageSize.Should().Be(25);
        sut.UserId.Should().NotBeNull();
    }
}