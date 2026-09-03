using System.Diagnostics;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderAllowedCoursesRepositoryTests;

public class UpsertProviderAllowedCourseTests
{
    [Test]
    public async Task WhenProviderDoesNotHaveCourseType_ThenAddsBothCourseTypeAndAllowedCourse_AndCreatesTwoAuditEntries()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        var lastDateStarts = DateTime.Today;
        string userId = "TestUserID";
        string userDisplayName = "Test User";
        CourseType courseType = CourseType.Apprenticeship;

        AddProvider(context, ukprn);

        ProviderAllowedCoursesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.CreateProviderAllowedCourse(ukprn, larsCode, courseType, lastDateStarts, userId, userDisplayName);

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderCourseTypes.Should().ContainSingle(x => x.CourseType == CourseType.Apprenticeship);
        provider.ProviderAllowedCourses.Should().ContainSingle(x =>
            x.LarsCode == larsCode &&
            x.LastDateStarts == lastDateStarts);

        context.Audits.Should().HaveCount(2);

        var audits = await context.Audits.ToListAsync();

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderCourseType) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "CreateProviderCourseType");

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderAllowedCourse) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "CreateProviderAllowedCourse");
    }

    [Test]
    public async Task WhenProviderHasCourseType_ThenAddsAllowedCourse_AndCreatesOneAuditEntry()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        var lastDateStarts = DateTime.Today;
        string userId = "TestUserID";
        string userDisplayName = "Test User";
        CourseType courseType = CourseType.Apprenticeship;

        AddProvider(context, ukprn);
        AddProviderCourseType(context, ukprn, CourseType.Apprenticeship);

        ProviderAllowedCoursesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.CreateProviderAllowedCourse(ukprn, larsCode, courseType, lastDateStarts, userId, userDisplayName);

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderAllowedCourses.Should().ContainSingle(x =>
            x.LarsCode == larsCode &&
            x.LastDateStarts == lastDateStarts);

        context.Audits.Should().HaveCount(1);

        var audits = await context.Audits.ToListAsync();

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderAllowedCourse) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "CreateProviderAllowedCourse");
    }

    private static void AddProvider(RoatpDataContext context, int ukprn)
    {
        context.Providers.Add(new Provider
        {
            Ukprn = ukprn,
            LegalName = "Test Provider",
            ProviderCourseTypes = [],
            ProviderAllowedCourses = []
        });

        context.SaveChanges();
    }

    private static void AddProviderCourseType(RoatpDataContext context, int ukprn, CourseType courseType)
    {
        context.ProviderCoursesTypes.Add(new ProviderCourseType
        {
            Ukprn = ukprn,
            CourseType = courseType
        });

        context.SaveChanges();
    }
}
