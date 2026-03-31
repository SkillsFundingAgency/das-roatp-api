using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderCourseForecastRepositoryTests;

public class GetProviderCoursesWithRecentForecastsTests
{
    [Test]
    public async Task GetProviderCoursesWithRecentForecasts_ReturnsGroupsWithLastUpdatedAfterCutOff()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();
        var cutOff = DateTime.UtcNow.Date.AddDays(-14);

        // seed data
        context.ProviderCourseForecasts.AddRange(
            // ukprn 1001, L1: one older and one newer -> expect group with newer date
            new ProviderCourseForecast { Ukprn = 1001, LarsCode = "L1", UpdatedDate = cutOff.AddDays(-1) },
            new ProviderCourseForecast { Ukprn = 1001, LarsCode = "L1", UpdatedDate = cutOff.AddDays(1) },

            // ukprn 1002, L2: only older -> should not be returned
            new ProviderCourseForecast { Ukprn = 1002, LarsCode = "L2", UpdatedDate = cutOff.AddDays(-20) },

            // ukprn 1003, L3: recent -> should be returned
            new ProviderCourseForecast { Ukprn = 1003, LarsCode = "L3", UpdatedDate = cutOff.AddDays(2) }
        );

        await context.SaveChangesAsync();

        ProviderCourseForecastRepository sut = new(context);

        // act
        var result = await sut.GetProviderCoursesWithRecentForecasts(cutOff, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        result.Should().Contain(r => r.Ukprn == 1001 && r.LarsCode == "L1");
        result.Should().Contain(r => r.Ukprn == 1003 && r.LarsCode == "L3");
    }

    [Test]
    public async Task GetProviderCoursesWithRecentForecasts_ReturnsEmpty_WhenNoRecentForecasts()
    {
        var cutOff = DateTime.UtcNow.Date.AddDays(-14);

        using var context = RoatpDataContextFactory.CreateInMemoryContext();
        context.ProviderCourseForecasts.AddRange(
            new ProviderCourseForecast { Ukprn = 2001, LarsCode = "L10", UpdatedDate = cutOff.AddDays(-30) },
            new ProviderCourseForecast { Ukprn = 2002, LarsCode = "L11", UpdatedDate = cutOff.AddDays(-16) }
        );

        await context.SaveChangesAsync();

        var repo = new ProviderCourseForecastRepository(context);
        var result = await repo.GetProviderCoursesWithRecentForecasts(cutOff, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
    }
}
