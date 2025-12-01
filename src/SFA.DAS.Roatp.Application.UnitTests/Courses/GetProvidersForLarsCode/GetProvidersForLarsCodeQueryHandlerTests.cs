using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForLarsCode;

[TestFixture]
public class GetProvidersForLarsCodeQueryHandlerTests
{
    [Test, RecursiveMoqAutoData()]
    public async Task Handle_ReturnsResult(
        [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
        [Frozen] Mock<ILogger<GetProvidersForLarsCodeQueryHandler>> loggerMock,
        GetProvidersForLarsCodeQueryHandler sut,
        List<ProviderSearchModel> pagedProviderDetails,
        GetProvidersFromLarsCodeRequest request,
        string larsCode,
        CancellationToken cancellationToken)
    {
        request.DeliveryModes = new List<DeliveryMode?>
        {
            DeliveryMode.Provider,
            DeliveryMode.BlockRelease,
            DeliveryMode.DayRelease,
            DeliveryMode.Workplace
        };

        var isProvider = true;
        var isWorkplace = true;
        var isBlockRelease = true;
        var isDayRelease = true;

        GetProvidersForLarsCodeQuery query = new GetProvidersForLarsCodeQuery(larsCode, request);

        foreach (var detail in pagedProviderDetails)
        {
            detail.LocationsCount = 0;
            detail.LocationTypes = "0";
            detail.CourseDistances = "0";
            detail.AtEmployers = "0";
            detail.DayReleases = "0";
            detail.BlockReleases = "0";
        }

        var qar = string.Empty;
        if (request.Qar is { Count: > 0 })
        {
            qar = string.Join(',', request.Qar);
        }

        var employerProviderRatings = string.Empty;
        if (request.EmployerProviderRatings is { Count: > 0 })
        {
            employerProviderRatings = string.Join(',', request.EmployerProviderRatings);
        }

        var apprenticeProviderRatings = string.Empty;
        if (request.ApprenticeProviderRatings is { Count: > 0 })
        {
            apprenticeProviderRatings = string.Join(',', request.ApprenticeProviderRatings);
        }

        var parametersUsed = new GetProvidersFromLarsCodeOptionalParameters
        {
            Page = request.Page,
            PageSize = request.PageSize,
            IsWorkplace = isWorkplace,
            IsProvider = isProvider,
            IsBlockRelease = isBlockRelease,
            IsDayRelease = isDayRelease,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Location = request.Location,
            Distance = request.Distance,
            QarRange = qar,
            EmployerProviderRatings = employerProviderRatings,
            ApprenticeProviderRatings = apprenticeProviderRatings,
            UserId = request.UserId
        };

        providersReadRepositoryMock.Setup(x => x.GetProvidersByLarsCode(
            larsCode,
            (ProviderOrderBy)query.OrderBy!,
            It.Is<GetProvidersFromLarsCodeOptionalParameters>(
                p =>
                    p.Page == parametersUsed.Page
                    && p.PageSize == parametersUsed.PageSize
                    && p.IsWorkplace == parametersUsed.IsWorkplace
                    && p.IsProvider == parametersUsed.IsProvider
                    && p.IsBlockRelease == parametersUsed.IsBlockRelease
                    && p.IsDayRelease == parametersUsed.IsDayRelease
                    && p.Latitude == parametersUsed.Latitude
                    && p.Longitude == parametersUsed.Longitude
                    && p.Distance == parametersUsed.Distance
                    && p.QarRange == parametersUsed.QarRange
                    && p.EmployerProviderRatings == parametersUsed.EmployerProviderRatings
                    && p.ApprenticeProviderRatings == parametersUsed.ApprenticeProviderRatings
                    && p.Location == parametersUsed.Location
                    && p.UserId == parametersUsed.UserId
            ),
            cancellationToken
            )).ReturnsAsync(pagedProviderDetails);

        var response = await sut.Handle(query, cancellationToken);
        var result = response.Result;
        result.Should().NotBeNull();
        providersReadRepositoryMock.Verify(x => x.GetProvidersByLarsCode(
                larsCode, (ProviderOrderBy)query.OrderBy!,
                It.Is<GetProvidersFromLarsCodeOptionalParameters>(
                    p =>
                        p.Page == parametersUsed.Page

                        && p.PageSize == parametersUsed.PageSize
                        && p.IsWorkplace == parametersUsed.IsWorkplace
                        && p.IsProvider == parametersUsed.IsProvider
                        && p.IsBlockRelease == parametersUsed.IsBlockRelease
                        && p.IsDayRelease == parametersUsed.IsDayRelease
                        && p.Latitude == parametersUsed.Latitude
                        && p.Longitude == parametersUsed.Longitude
                        && p.Distance == parametersUsed.Distance
                        && p.QarRange == parametersUsed.QarRange
                        && p.EmployerProviderRatings == parametersUsed.EmployerProviderRatings
                        && p.ApprenticeProviderRatings == parametersUsed.ApprenticeProviderRatings
                        && p.Location == parametersUsed.Location
                        && p.UserId == parametersUsed.UserId
                 ), cancellationToken),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Test, RecursiveMoqAutoData()]
    public async Task Handle_NoData_ReturnsExpectedResult(
    [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
    GetProvidersForLarsCodeQueryHandler sut,
    GetProvidersFromLarsCodeRequest request,
    CancellationToken cancellationToken)
    {
        string larsCode = "2";
        int larsCodeValue = 2;
        var query = new GetProvidersForLarsCodeQuery(larsCode, request);

        List<ProviderSearchModel> pagedProviderDetails = new()
        {
            new()
            {
                Page = request.Page ?? Pagination.DefaultPage,
                PageSize = request.PageSize ?? Pagination.DefaultPageSize,
                TotalPages = 0,
                TotalCount = 0,
                LarsCode = larsCode
            }
        };

        providersReadRepositoryMock.Setup(x => x.GetProvidersByLarsCode(
            larsCode.ToString(),
            (ProviderOrderBy)query.OrderBy!,
            It.IsAny<GetProvidersFromLarsCodeOptionalParameters>(),
            cancellationToken
        )).ReturnsAsync(pagedProviderDetails);

        var expectedResult = new GetProvidersForLarsCodeQueryResult
        {
            Page = request.Page ?? Pagination.DefaultPage,
            PageSize = request.PageSize ?? Pagination.DefaultPageSize,
            TotalPages = 0,
            TotalCount = 0,
            LarsCode = larsCodeValue,
            Providers = []
        };

        var expectedValidatedResult = new ValidatedResponse<GetProvidersForLarsCodeQueryResult>(expectedResult);

        var response = await sut.Handle(query, cancellationToken);
        var result = response.Result;

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedValidatedResult.Result);
        providersReadRepositoryMock.Verify(x => x.GetProvidersByLarsCode(larsCode.ToString(),
                (ProviderOrderBy)query.OrderBy!,
                It.IsAny<GetProvidersFromLarsCodeOptionalParameters>(),
                cancellationToken),
            Times.Once);
    }


    [Test, RecursiveMoqAutoData()]
    public async Task Handle_ReturnsResult_WithMultipleLocations(
        [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
        GetProvidersForLarsCodeQueryHandler sut,
        List<ProviderSearchModel> pagedProviderDetails,
        GetProvidersFromLarsCodeRequest request,
        string larsCode,
        CancellationToken cancellationToken)
    {
        request.DeliveryModes = new List<DeliveryMode?>
        {
            DeliveryMode.Provider,
            DeliveryMode.BlockRelease,
            DeliveryMode.DayRelease,
            DeliveryMode.Workplace
        };

        var isProvider = true;
        var isWorkplace = true;
        var isBlockRelease = true;
        var isDayRelease = true;

        var locationsCount = 3;
        var locationTypesValues = "0,1,2";
        var courseDistancesValues = "31.3,0,21";
        var atEmployersValues = "0,1,0";
        var dayReleasesValues = "1,1,0";
        var blockReleasesValues = "1,0,1";

        GetProvidersForLarsCodeQuery query = new GetProvidersForLarsCodeQuery(larsCode, request);

        foreach (var detail in pagedProviderDetails)
        {
            detail.LocationsCount = locationsCount;
            detail.LocationTypes = locationTypesValues;
            detail.CourseDistances = courseDistancesValues;
            detail.AtEmployers = atEmployersValues;
            detail.DayReleases = dayReleasesValues;
            detail.BlockReleases = blockReleasesValues;
        }

        var locationTypesExpected = locationTypesValues.Split(',').Select(x => (LocationType)int.Parse(x.Trim())).ToList();
        var courseDistancesExpected = courseDistancesValues.Split(',').Select(x => decimal.Parse(x.Trim())).ToList();
        var atEmployersExpected = atEmployersValues.Split(',').Select(x => int.Parse(x.Trim()) != 0).ToList();
        var dayReleasesExpected = dayReleasesValues.Split(',').Select(x => int.Parse(x.Trim()) != 0).ToList();
        var blockReleasesExpected = blockReleasesValues.Split(',').Select(x => int.Parse(x.Trim()) != 0).ToList();

        var expectedLocations = new List<ProviderLocationModel>();

        for (var i = 0; i < locationsCount; i++)
        {
            var expectedLocation = new ProviderLocationModel
            {
                Ordering = i + 1,
                LocationType = locationTypesExpected[i],
                CourseDistance = courseDistancesExpected[i],
                AtEmployer = atEmployersExpected[i],
                DayRelease = dayReleasesExpected[i],
                BlockRelease = blockReleasesExpected[i]
            };

            expectedLocations.Add(expectedLocation);
        }

        var qar = string.Empty;
        if (request.Qar is { Count: > 0 })
        {
            qar = string.Join(',', request.Qar);
        }

        var employerProviderRatings = string.Empty;
        if (request.EmployerProviderRatings is { Count: > 0 })
        {
            employerProviderRatings = string.Join(',', request.EmployerProviderRatings);
        }

        var apprenticeProviderRatings = string.Empty;
        if (request.ApprenticeProviderRatings is { Count: > 0 })
        {
            apprenticeProviderRatings = string.Join(',', request.ApprenticeProviderRatings);
        }

        var parametersUsed = new GetProvidersFromLarsCodeOptionalParameters
        {
            Page = request.Page,
            PageSize = request.PageSize,
            IsWorkplace = isWorkplace,
            IsProvider = isProvider,
            IsBlockRelease = isBlockRelease,
            IsDayRelease = isDayRelease,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Location = request.Location,
            Distance = request.Distance,
            QarRange = qar,
            EmployerProviderRatings = employerProviderRatings,
            ApprenticeProviderRatings = apprenticeProviderRatings,
            UserId = request.UserId
        };

        providersReadRepositoryMock.Setup(x => x.GetProvidersByLarsCode(
            larsCode,
            (ProviderOrderBy)query.OrderBy!,
            It.Is<GetProvidersFromLarsCodeOptionalParameters>(
                p =>
                    p.Page == parametersUsed.Page
                    && p.PageSize == parametersUsed.PageSize
                    && p.IsWorkplace == parametersUsed.IsWorkplace
                    && p.IsProvider == parametersUsed.IsProvider
                    && p.IsBlockRelease == parametersUsed.IsBlockRelease
                    && p.IsDayRelease == parametersUsed.IsDayRelease
                    && p.Latitude == parametersUsed.Latitude
                    && p.Longitude == parametersUsed.Longitude
                    && p.Location == parametersUsed.Location
                    && p.Distance == parametersUsed.Distance
                    && p.QarRange == parametersUsed.QarRange
                    && p.EmployerProviderRatings == parametersUsed.EmployerProviderRatings
                    && p.ApprenticeProviderRatings == parametersUsed.ApprenticeProviderRatings
                    && p.UserId == parametersUsed.UserId
            ),
            cancellationToken
            )).ReturnsAsync(pagedProviderDetails);

        var response = await sut.Handle(query, cancellationToken);
        var result = response.Result;
        result.Should().NotBeNull();
        var firstProvider = result.Providers[0];

        firstProvider.Locations.Should().BeEquivalentTo(expectedLocations);
    }
}
