using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories;

public class ProviderCourseForecastRepositoryTests
{
    private readonly ProviderCourseForecast ExistingForecast = new()
    {
        Ukprn = 12345678,
        LarsCode = "ZSC00001",
        TimePeriod = "2425",
        Quarter = 4,
        EstimatedLearners = 20,
        CreatedDate = DateTime.UtcNow.AddDays(-1),
        UpdatedDate = DateTime.UtcNow.AddDays(-1)
    };

    [Test]
    public async Task UpsertProviderCourseForecasts_InsertsNewForecasts()
    {
        var dataContext = RoatpDataContextFactory.CreateInMemoryContext();
        AddForecastsData(dataContext);
        ProviderCourseForecastRepository sut = new(dataContext);

        ProviderCourseForecast forecast = new()
        {
            Ukprn = ExistingForecast.Ukprn,
            LarsCode = ExistingForecast.LarsCode,
            TimePeriod = "2526",
            Quarter = 1,
            EstimatedLearners = 10,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        await sut.UpsertProviderCourseForecasts([forecast], CancellationToken.None);

        dataContext.ProviderCourseForecasts.Count().Should().Be(2);
        dataContext.ProviderCourseForecasts.Should().Contain(f => f.Quarter == forecast.Quarter);
    }

    [Test]
    public async Task UpsertProviderCourseForecasts_UpdatesExistingForecast()
    {
        var dataContext = RoatpDataContextFactory.CreateInMemoryContext();
        AddForecastsData(dataContext);
        ProviderCourseForecastRepository sut = new(dataContext);

        ProviderCourseForecast forecast = new()
        {
            Ukprn = ExistingForecast.Ukprn,
            LarsCode = ExistingForecast.LarsCode,
            TimePeriod = ExistingForecast.TimePeriod,
            Quarter = ExistingForecast.Quarter,
            EstimatedLearners = 50,
        };

        await sut.UpsertProviderCourseForecasts([forecast], CancellationToken.None);

        dataContext.ProviderCourseForecasts.Count().Should().Be(1);
        dataContext.ProviderCourseForecasts.Should().Contain(f => f.Quarter == forecast.Quarter && f.EstimatedLearners == forecast.EstimatedLearners && f.UpdatedDate.GetValueOrDefault().Date == DateTime.UtcNow.Date);
    }

    private void AddForecastsData(RoatpDataContext dataContext)
    {
        dataContext.AddRange(ExistingForecast);
        dataContext.SaveChanges();
    }
}
