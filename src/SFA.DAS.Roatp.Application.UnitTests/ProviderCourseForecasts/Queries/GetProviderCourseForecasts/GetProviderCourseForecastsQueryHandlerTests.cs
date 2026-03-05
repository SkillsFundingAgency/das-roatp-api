using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryHandlerTests
{
    private const int ExpectedQuarter = 4;
    private const string ExpectedTimePeriod = "AY2324";
    private const int ExpectedEstimatedLearners = 10;
    private static readonly GetProviderCourseForecastsQuery Query = new(10012002, "ZSC10001");
    private static DateTime GetDate(int year, int month) => new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified);

    private ProviderCourseForecast _pastForecast;
    private ProviderCourseForecast _currentForecast;

    [SetUp]
    public void Before_Each_Test()
    {
        _pastForecast = new() { LarsCode = Query.LarsCode, Ukprn = Query.Ukprn, EstimatedLearners = 100, Quarter = ExpectedQuarter, TimePeriod = "AY2223", UpdatedDate = GetDate(2023, 4) };
        _currentForecast = new() { LarsCode = Query.LarsCode, Ukprn = Query.Ukprn, EstimatedLearners = ExpectedEstimatedLearners, Quarter = ExpectedQuarter, TimePeriod = ExpectedTimePeriod, UpdatedDate = GetDate(2023, 12) };
    }

    private static List<ForecastQuarter> GetForecastQuarters()
        => [
            new ForecastQuarter { Quarter = 2, TimePeriod = "AY2223", StartDate = GetDate(2023, 5), EndDate = GetDate(2023, 7) },
            new ForecastQuarter { Quarter = 3, TimePeriod = "AY2324", StartDate = GetDate(2023, 8), EndDate = GetDate(2023, 10) },
            new ForecastQuarter { Quarter = ExpectedQuarter, TimePeriod = ExpectedTimePeriod, StartDate = GetDate(2023, 11), EndDate = GetDate(2024, 1) },
            new ForecastQuarter { Quarter = 1, TimePeriod = "AY2324", StartDate = GetDate(2024, 2), EndDate = GetDate(2024, 4) }
        ];

    [Test, MoqAutoData]
    public async Task Handler_ForecastsFound_GetForecastsForProviderCourseWithEstimatedLearners(
        [Frozen] Mock<IForecastQuartersRepository> forecastQuartersRepositoryMock,
        [Frozen] Mock<IProviderCourseForecastRepository> providerCourseForecastRepositoryMock,
        [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepositoryMock,
        GetProviderCourseForecastsQueryHandler sut,
        CancellationToken cancellationToken)
    {
        providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(Query.Ukprn, cancellationToken)).ReturnsAsync(
        [
            new ProviderCourseType { Ukprn = Query.Ukprn, CourseType = CourseType.ShortCourse }
        ]);
        forecastQuartersRepositoryMock.Setup(r => r.GetForecastQuarters(cancellationToken)).ReturnsAsync(GetForecastQuarters());
        providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, cancellationToken)).ReturnsAsync(
        [_currentForecast]);

        var result = await sut.Handle(Query, cancellationToken);

        result.Result.Forecasts.Should().HaveCount(4);
        result.Result.Forecasts.Should().Contain(r => r.Quarter == ExpectedQuarter && r.EstimatedLearners == ExpectedEstimatedLearners);
        result.Result.Forecasts.Should().Contain(r => r.Quarter != ExpectedQuarter && r.EstimatedLearners == null);
    }

    [Test, MoqAutoData]
    public async Task Handler_NoForecastsFound_GetsEmptyForecastsForProviderCourse(
        [Frozen] Mock<IForecastQuartersRepository> forecastQuartersRepositoryMock,
        [Frozen] Mock<IProviderCourseForecastRepository> providerCourseForecastRepositoryMock,
        [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepositoryMock,
        GetProviderCourseForecastsQueryHandler sut,
        int expectedLearners,
        CancellationToken cancellationToken)
    {
        providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(Query.Ukprn, cancellationToken)).ReturnsAsync(
        [
            new ProviderCourseType { Ukprn = Query.Ukprn, CourseType = CourseType.ShortCourse }
        ]);
        forecastQuartersRepositoryMock.Setup(r => r.GetForecastQuarters(cancellationToken)).ReturnsAsync(GetForecastQuarters());
        providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, cancellationToken)).ReturnsAsync([]);

        var result = await sut.Handle(Query, cancellationToken);

        result.Result.Forecasts.Should().HaveCount(4);
        result.Result.Forecasts.Should().Contain(r => r.Quarter == ExpectedQuarter && r.EstimatedLearners == null);
    }


    [Test, MoqAutoData]
    public async Task Handler_OlderForecastsFound_GetsCurrentForecastsForProviderCourse(
        [Frozen] Mock<IForecastQuartersRepository> forecastQuartersRepositoryMock,
        [Frozen] Mock<IProviderCourseForecastRepository> providerCourseForecastRepositoryMock,
        [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepositoryMock,
        GetProviderCourseForecastsQueryHandler sut,
        int expectedLearners,
        CancellationToken cancellationToken)
    {
        providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(Query.Ukprn, cancellationToken)).ReturnsAsync(
        [
            new ProviderCourseType { Ukprn = Query.Ukprn, CourseType = CourseType.ShortCourse }
        ]);
        forecastQuartersRepositoryMock.Setup(r => r.GetForecastQuarters(cancellationToken)).ReturnsAsync(GetForecastQuarters());
        providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, cancellationToken)).ReturnsAsync([_pastForecast, _currentForecast]);

        var result = await sut.Handle(Query, cancellationToken);

        result.Result.Forecasts.Should().HaveCount(4);
        result.Result.Forecasts.Should().Contain(r => r.TimePeriod == ExpectedTimePeriod && r.Quarter == ExpectedQuarter && r.EstimatedLearners == ExpectedEstimatedLearners);
        result.Result.Forecasts.Should().NotContain(r => r.EstimatedLearners == _pastForecast.EstimatedLearners);
    }

    [Test, MoqAutoData]
    public async Task Handler_ShortCourseTypeNotAllowed_RetursNull(
        [Frozen] Mock<IForecastQuartersRepository> forecastQuartersRepositoryMock,
        [Frozen] Mock<IProviderCourseForecastRepository> providerCourseForecastRepositoryMock,
        [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepositoryMock,
        GetProviderCourseForecastsQueryHandler sut,
        int expectedLearners,
        CancellationToken cancellationToken)
    {
        providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(Query.Ukprn, cancellationToken)).ReturnsAsync(
        [
            new ProviderCourseType { Ukprn = Query.Ukprn, CourseType = CourseType.Apprenticeship }
        ]);

        var result = await sut.Handle(Query, cancellationToken);

        result.Result.Should().BeNull();
        result.IsValidResponse.Should().BeTrue();
        forecastQuartersRepositoryMock.Verify(r => r.GetForecastQuarters(It.IsAny<CancellationToken>()), Times.Never);
        providerCourseForecastRepositoryMock.Verify(r => r.GetProviderCourseForecasts(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
