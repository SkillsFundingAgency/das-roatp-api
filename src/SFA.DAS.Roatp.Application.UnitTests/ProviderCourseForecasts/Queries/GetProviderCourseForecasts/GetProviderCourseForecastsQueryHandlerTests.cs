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
    private const string LarsCode = "ZSC10001";
    private const int Ukprn = 10012002;
    private static readonly GetProviderCourseForecastsQuery Query = new(Ukprn, LarsCode);
    private static DateTime GetDate(int year, int month) => new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified);

    private ProviderCourseForecast _pastForecast;
    private ProviderCourseForecast _currentForecast;

    private static List<ForecastQuarter> GetForecastQuarters()
    => [
            new ForecastQuarter { Quarter = 2, TimePeriod = "AY2223", StartDate = GetDate(2023, 5), EndDate = GetDate(2023, 7) },
            new ForecastQuarter { Quarter = 3, TimePeriod = "AY2324", StartDate = GetDate(2023, 8), EndDate = GetDate(2023, 10) },
            new ForecastQuarter { Quarter = ExpectedQuarter, TimePeriod = ExpectedTimePeriod, StartDate = GetDate(2023, 11), EndDate = GetDate(2024, 1) },
            new ForecastQuarter { Quarter = 1, TimePeriod = "AY2324", StartDate = GetDate(2024, 2), EndDate = GetDate(2024, 4) }
    ];

    private Mock<IStandardsReadRepository> _standardsReadRepositoryMock;
    private Mock<IProviderCourseForecastRepository> _providerCourseForecastRepositoryMock;
    private Mock<IProviderCourseTypesReadRepository> _providerCourseTypesReadRepositoryMock;
    private Mock<IForecastQuartersRepository> _forecastQuartersRepositoryMock;
    GetProviderCourseForecastsQueryHandler _sut;

    [SetUp]
    public void Before_Each_Test()
    {
        _pastForecast = new() { LarsCode = Query.LarsCode, Ukprn = Query.Ukprn, EstimatedLearners = 100, Quarter = ExpectedQuarter, TimePeriod = "AY2223", UpdatedDate = GetDate(2023, 4) };
        _currentForecast = new() { LarsCode = Query.LarsCode, Ukprn = Query.Ukprn, EstimatedLearners = ExpectedEstimatedLearners, Quarter = ExpectedQuarter, TimePeriod = ExpectedTimePeriod, UpdatedDate = GetDate(2023, 12), CreatedDate = GetDate(2023, 10) };

        _standardsReadRepositoryMock = new();
        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.IsAny<string>())).ReturnsAsync(new Standard { LarsCode = LarsCode, Title = "Test Course", Level = 2, CourseType = CourseType.ShortCourse });

        _providerCourseTypesReadRepositoryMock = new();
        _providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(Query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new ProviderCourseType { Ukprn = Query.Ukprn, CourseType = CourseType.ShortCourse }
        ]);

        _forecastQuartersRepositoryMock = new();
        _forecastQuartersRepositoryMock.Setup(r => r.GetForecastQuarters(It.IsAny<CancellationToken>())).ReturnsAsync(GetForecastQuarters());

        _providerCourseForecastRepositoryMock = new();

        _sut = new GetProviderCourseForecastsQueryHandler(_providerCourseTypesReadRepositoryMock.Object, _providerCourseForecastRepositoryMock.Object, _forecastQuartersRepositoryMock.Object, _standardsReadRepositoryMock.Object);
    }

    [Test]
    public async Task Handler_ForecastsFound_GetForecastsForProviderCourseWithEstimatedLearners()
    {
        _providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, It.IsAny<CancellationToken>())).ReturnsAsync([_currentForecast]);

        var result = await _sut.Handle(Query, default);

        result.Result.Forecasts.Should().HaveCount(4);
        result.Result.Forecasts.Should().Contain(r => r.Quarter == ExpectedQuarter && r.EstimatedLearners == ExpectedEstimatedLearners && r.UpdatedDate == _currentForecast.UpdatedDate);
        result.Result.Forecasts.Should().Contain(r => r.Quarter != ExpectedQuarter && r.EstimatedLearners == null);
    }

    [Test]
    public async Task Handler_ForecastsFound_UsesCreatedDateForUpdateDate()
    {
        _currentForecast.UpdatedDate = null;
        _providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, It.IsAny<CancellationToken>())).ReturnsAsync([_currentForecast]);

        var result = await _sut.Handle(Query, default);

        result.Result.Forecasts.Should().HaveCount(4);
        result.Result.Forecasts.Should().Contain(r => r.Quarter == ExpectedQuarter && r.EstimatedLearners == ExpectedEstimatedLearners && r.UpdatedDate == _currentForecast.CreatedDate);
        result.Result.Forecasts.Should().Contain(r => r.Quarter != ExpectedQuarter && r.EstimatedLearners == null);
    }

    [Test, AutoData]
    public async Task Handler_NoForecastsFound_GetsEmptyForecastsForProviderCourse(int expectedLearners, CancellationToken cancellationToken)
    {
        _providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, cancellationToken)).ReturnsAsync([]);

        var result = await _sut.Handle(Query, cancellationToken);

        result.Result.Forecasts.Should().HaveCount(4);
        result.Result.Forecasts.Should().Contain(r => r.Quarter == ExpectedQuarter && r.EstimatedLearners == null && r.UpdatedDate == null);
    }


    [Test, AutoData]
    public async Task Handler_OlderForecastsFound_GetsCurrentForecastsForProviderCourse(
        int expectedLearners,
        CancellationToken cancellationToken)
    {
        _providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, cancellationToken)).ReturnsAsync([_pastForecast, _currentForecast]);

        var result = await _sut.Handle(Query, cancellationToken);

        result.Result.Forecasts.Should().HaveCount(4);
        result.Result.Forecasts.Should().Contain(r => r.TimePeriod == ExpectedTimePeriod && r.Quarter == ExpectedQuarter && r.EstimatedLearners == ExpectedEstimatedLearners);
        result.Result.Forecasts.Should().NotContain(r => r.EstimatedLearners == _pastForecast.EstimatedLearners);
    }

    [Test, AutoData]
    public async Task Handler_ShortCourseTypeNotAllowed_RetursNull(
        int expectedLearners,
        CancellationToken cancellationToken)
    {
        _providerCourseTypesReadRepositoryMock.Setup(r => r.GetProviderCourseTypesByUkprn(Query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new ProviderCourseType { Ukprn = Query.Ukprn, CourseType = CourseType.Apprenticeship }
        ]);

        var result = await _sut.Handle(Query, cancellationToken);

        result.Result.Should().BeNull();
        result.IsValidResponse.Should().BeTrue();
        _forecastQuartersRepositoryMock.Verify(r => r.GetForecastQuarters(It.IsAny<CancellationToken>()), Times.Never);
        _providerCourseForecastRepositoryMock.Verify(r => r.GetProviderCourseForecasts(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handler_ReturnsStandardDetails(
        int expectedLearners,
        Standard standard,
        CancellationToken cancellationToken)
    {
        _providerCourseForecastRepositoryMock.Setup(r => r.GetProviderCourseForecasts(Query.Ukprn, Query.LarsCode, It.IsAny<CancellationToken>())).ReturnsAsync([_currentForecast]);

        _standardsReadRepositoryMock.Setup(r => r.GetStandard(It.IsAny<string>())).ReturnsAsync(standard);

        var result = await _sut.Handle(Query, cancellationToken);

        result.Result.LarsCode.Should().Be(standard.LarsCode);
        result.Result.CourseName.Should().Be(standard.Title);
        result.Result.CourseLevel.Should().Be(standard.Level);
    }
}
