using System.Diagnostics;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderAllowedCoursesRepositoryTests;

public class UpdateLastDateStartsTests
{
    [Test]
    public async Task WhenLastDateStartsPassedHasDate_ThenUpdatesLastDateStartsAndCreatesAudit()
    {
        // Arrange
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        DateTime? lastDateStarts = DateTime.Today.AddDays(-1);
        DateTime? existingLastDateStarts = DateTime.Today;
        string userId = "TestUserID";
        string userDisplayName = "Test User";

        AddProviderAllowedCourse(context, ukprn, larsCode, existingLastDateStarts);

        ProviderAllowedCoursesRepository sut = new(context);

        // Act
        using var activity = new Activity("test");
        activity.Start();

        await sut.UpdateLastDateStarts(ukprn, larsCode, lastDateStarts, userId, userDisplayName);

        activity.Stop();

        // Assert
        var providerAllowedCourse = await context.ProviderAllowedCourses
            .SingleAsync(x => x.Ukprn == ukprn && x.LarsCode == larsCode);

        providerAllowedCourse.Should().NotBeNull();
        providerAllowedCourse.LastDateStarts.Should().Be(lastDateStarts);

        context.Audits.Should().HaveCount(1);

        var audits = await context.Audits.ToListAsync();

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderAllowedCourse) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "UpdateProviderAllowedCourse");
    }

    [Test]
    public async Task WhenLastDateStartsPassedIsNull_ThenUpdatesLastDateStartsToNullAndCreatesAudit()
    {
        // Arrange
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        DateTime? lastDateStarts = null;
        DateTime? existingLastDateStarts = DateTime.Today;
        string userId = "TestUserID";
        string userDisplayName = "Test User";

        AddProviderAllowedCourse(context, ukprn, larsCode, existingLastDateStarts);

        ProviderAllowedCoursesRepository sut = new(context);

        // Act
        using var activity = new Activity("test");
        activity.Start();

        await sut.UpdateLastDateStarts(ukprn, larsCode, lastDateStarts, userId, userDisplayName);

        activity.Stop();

        // Assert
        var providerAllowedCourse = await context.ProviderAllowedCourses
            .SingleAsync(x => x.Ukprn == ukprn && x.LarsCode == larsCode);

        providerAllowedCourse.LastDateStarts.Should().BeNull();

        context.Audits.Should().HaveCount(1);

        var audits = await context.Audits.ToListAsync();

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderAllowedCourse) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "UpdateProviderAllowedCourse");
    }

    private static void AddProviderAllowedCourse(RoatpDataContext context, int ukprn, string larsCode, DateTime? lastDateStarts)
    {
        context.ProviderAllowedCourses.Add(new ProviderAllowedCourse
        {
            Ukprn = ukprn,
            LarsCode = larsCode,
            LastDateStarts = lastDateStarts
        });

        context.SaveChanges();
    }
}
