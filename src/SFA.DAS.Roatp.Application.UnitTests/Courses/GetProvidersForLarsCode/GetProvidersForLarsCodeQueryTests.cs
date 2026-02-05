using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersForLarsCode;
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
        var request = new GetProvidersForLarsCodeRequest
        {
            Latitude = latitude,
            Longitude = longitude,
            Location = location,
            OrderBy = orderBy,
            Distance = distance,
            Page = page,
            PageSize = pageSize,
            UserId = userId,
            DeliveryModes = new List<DeliveryMode?> { null, DeliveryMode.Provider, DeliveryMode.DayRelease },
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

        sut.DeliveryModes.Should().BeEquivalentTo(new[] { DeliveryMode.Provider, DeliveryMode.DayRelease });
        sut.EmployerProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Good });
        sut.ApprenticeProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Excellent });
        sut.Qar.Should().BeEquivalentTo(new[] { QarRating.Good });
    }

    [Test, RecursiveMoqAutoData]
    public void Constructor_V2Request_EmptyCollections_ResultEmptyLists(
        string larsCode,
        GetProvidersForLarsCodeRequest request)
    {
        request.DeliveryModes = new List<DeliveryMode?>();
        request.EmployerProviderRatings = new List<ProviderRating?>();
        request.ApprenticeProviderRatings = new List<ProviderRating?>();
        request.Qar = new List<QarRating?>();

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
        var request = new GetProvidersForLarsCodeRequest
        {
            Latitude = latitude,
            Longitude = longitude,
            Location = location,
            OrderBy = orderBy,
            Distance = distance,
            Page = page,
            PageSize = pageSize,
            UserId = userId,
            DeliveryModes = new List<DeliveryMode?> { null, DeliveryMode.Provider, DeliveryMode.Workplace },
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
            .Select(x => (DeliveryMode)x!)
            .ToList();

        sut.DeliveryModes.Should().BeEquivalentTo(expectedModes);
        sut.EmployerProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Poor });
        sut.ApprenticeProviderRatings.Should().BeEquivalentTo(new[] { ProviderRating.Good });
        sut.Qar.Should().BeEquivalentTo(new[] { QarRating.Poor });
    }

    [Test, RecursiveMoqAutoData]
    public void Constructor_RequestWithNullCollections_LeavesListsEmpty(
        string larsCode,
        decimal? latitude,
        decimal? longitude,
        string location,
        ProviderOrderBy? orderBy,
        decimal? distance,
        int? page,
        int? pageSize,
        Guid? userId)
    {
        var request = new GetProvidersForLarsCodeRequest
        {
            Latitude = latitude,
            Longitude = longitude,
            Location = location,
            OrderBy = orderBy,
            Distance = distance,
            Page = page,
            PageSize = pageSize,
            UserId = userId,
            DeliveryModes = null,
            EmployerProviderRatings = null,
            ApprenticeProviderRatings = null,
            Qar = null
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

        sut.DeliveryModes.Should().NotBeNull();
        sut.DeliveryModes.Should().BeEmpty();

        sut.EmployerProviderRatings.Should().NotBeNull();
        sut.EmployerProviderRatings.Should().BeEmpty();

        sut.ApprenticeProviderRatings.Should().NotBeNull();
        sut.ApprenticeProviderRatings.Should().BeEmpty();

        sut.Qar.Should().NotBeNull();
        sut.Qar.Should().BeEmpty();
    }
}