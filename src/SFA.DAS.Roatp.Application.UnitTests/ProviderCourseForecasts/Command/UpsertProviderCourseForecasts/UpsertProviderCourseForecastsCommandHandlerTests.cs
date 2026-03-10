using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseForecasts.Command.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastsCommandHandlerTests
{
    private const int ExpectedQuarter = 1;
    private const string ExpectedTimePeriod = "2526";
    private const int ExpectedLearners = 10;

    [Test, MoqAutoData]
    public async Task Handler_ForwardsCurrentForecastsOnly(
        [Frozen] Mock<IProviderCourseForecastRepository> providerCourseForecastRepositoryMock,
        [Frozen] Mock<IForecastQuartersRepository> forecastQuartersRepositoryMock,
        UpsertProviderCourseForecastsCommandHandler sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var forecastQuarters = new List<ForecastQuarter>
        {
            new ForecastQuarter { Quarter = 4, TimePeriod = "2425" },
            new ForecastQuarter { Quarter = ExpectedQuarter, TimePeriod = ExpectedTimePeriod },
            new ForecastQuarter { Quarter = 2, TimePeriod = ExpectedTimePeriod },
            new ForecastQuarter { Quarter = 3, TimePeriod = ExpectedTimePeriod }
        };
        forecastQuartersRepositoryMock.Setup(x => x.GetForecastQuarters(cancellationToken)).ReturnsAsync(forecastQuarters);

        List<UpsertProviderCourseForecastModel> forecasts =
        [
            new UpsertProviderCourseForecastModel("2425", 1, 1),
            new UpsertProviderCourseForecastModel("2526", 4, 2),
            new UpsertProviderCourseForecastModel(ExpectedTimePeriod, ExpectedQuarter, ExpectedLearners)
        ];
        UpsertProviderCourseForecastsCommand command = new(10012002, "ZSC00001", forecasts);

        // Act
        await sut.Handle(command, cancellationToken);

        // Assert
        providerCourseForecastRepositoryMock.Verify(x => x.UpsertProviderCourseForecasts(It.Is<IEnumerable<ProviderCourseForecast>>(a => a.Count() == 1 && HasExpectedValues(a.First(), command)), cancellationToken), Times.Once);
    }

    private static bool HasExpectedValues(ProviderCourseForecast actual, UpsertProviderCourseForecastsCommand command)
        => actual.Ukprn == command.Ukprn
        && actual.LarsCode == command.LarsCode
        && actual.Quarter == ExpectedQuarter
        && actual.TimePeriod == ExpectedTimePeriod
        && actual.EstimatedLearners == ExpectedLearners;
}
