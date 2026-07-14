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
    public async Task WhenProviderDoesNotHaveCourseType_AndAllowedCourseDoesNotExist_ThenAddsBothCourseTypeAndAllowedCourse_AndCreatesTwoAuditEntries()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        var lastDateStarts = DateTime.Today;

        AddProvider(context, ukprn);

        ProviderAllowedCoursesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.UpsertProviderAllowedCourse(ukprn, larsCode, CourseType.Apprenticeship, lastDateStarts, "TestUserID", "Test User");

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
    }

    [Test]
    public async Task WhenProviderHasCourseType_AndAllowedCourseDoesNotExist_ThenAddsAllowedCourse_AndCreatesOneAuditEntry()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        var lastDateStarts = DateTime.Today;

        AddProvider(context, ukprn);
        AddProviderCourseType(context, ukprn, CourseType.Apprenticeship);

        ProviderAllowedCoursesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.UpsertProviderAllowedCourse(ukprn, larsCode, CourseType.Apprenticeship, lastDateStarts, "TestUserID", "Test User");

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderAllowedCourses.Should().ContainSingle(x =>
            x.LarsCode == larsCode &&
            x.LastDateStarts == lastDateStarts);

        context.Audits.Should().HaveCount(1);
    }

    [Test]
    public async Task WhenProviderHasCourseType_AndAllowedCourseExists_ThenUpdatesLastDateStarts_AndCreatesOneAuditEntry()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        var existingLastDateStarts = DateTime.UtcNow.AddMonths(-1);
        var updatedLastDateStarts = DateTime.UtcNow;

        AddProvider(context, ukprn);
        AddProviderCourseType(context, ukprn, CourseType.Apprenticeship);
        AddProviderAllowedCourse(context, ukprn, larsCode, existingLastDateStarts);

        ProviderAllowedCoursesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.UpsertProviderAllowedCourse(ukprn, larsCode, CourseType.Apprenticeship, updatedLastDateStarts, "TestUserID", "Test User");

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderAllowedCourses.Should().ContainSingle(x =>
            x.LarsCode == larsCode &&
            x.LastDateStarts == updatedLastDateStarts);

        context.Audits.Should().HaveCount(1);
    }

    [Test]
    public async Task WhenProviderDoesNotHaveCourseType_AndAllowedCourseExists_ThenAddsCourseTypeUpdatesAllowedCourse_AndCreatesTwoAuditEntries()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        var existingLastDateStarts = DateTime.UtcNow.AddMonths(-1);
        var updatedLastDateStarts = DateTime.UtcNow;

        AddProvider(context, ukprn);
        AddProviderAllowedCourse(context, ukprn, larsCode, existingLastDateStarts);

        ProviderAllowedCoursesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.UpsertProviderAllowedCourse(ukprn, larsCode, CourseType.Apprenticeship, updatedLastDateStarts, "TestUserID", "Test User");

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderCourseTypes.Should().ContainSingle(x => x.CourseType == CourseType.Apprenticeship);
        provider.ProviderAllowedCourses.Should().ContainSingle(x =>
            x.LarsCode == larsCode &&
            x.LastDateStarts == updatedLastDateStarts);

        context.Audits.Should().HaveCount(2);
    }

    [Test]
    public async Task WhenAllowedCourseExistsAndLastDateStartsIsNull_ThenUpdatesLastDateStartsToNull_AndCreatesOneAuditEntry()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string larsCode = "LARS001";
        var existingLastDateStarts = DateTime.UtcNow.AddMonths(-1);

        AddProvider(context, ukprn);
        AddProviderCourseType(context, ukprn, CourseType.Apprenticeship);
        AddProviderAllowedCourse(context, ukprn, larsCode, existingLastDateStarts);

        ProviderAllowedCoursesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.UpsertProviderAllowedCourse(ukprn, larsCode, CourseType.Apprenticeship, null, "TestUserID", "Test User");

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderAllowedCourses.Should().ContainSingle(x =>
            x.LarsCode == larsCode &&
            x.LastDateStarts == null);

        context.Audits.Should().HaveCount(1);
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
