using System.Diagnostics;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderCourseTypesRepositoryTests;

public class RestrictProviderTests
{
    [Test]
    public async Task WhenProviderIsNotRestricted_AndNoCoursesToAddOrRemove_ThenRestrictsProvider_AndCreatesOneAuditEntry()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string userId = "TestUserID";
        string userDisplayName = "Test User";
        CourseType courseType = CourseType.Apprenticeship;

        AddProvider(context, ukprn);
        AddProviderCourseType(context, ukprn, courseType);

        ProviderCourseTypesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.RestrictProvider(
            ukprn,
            courseType,
            [],
            [],
            userId,
            userDisplayName);

        activity.Stop();

        var providerCourseType = await context.ProviderCoursesTypes
            .SingleAsync(x => x.Ukprn == ukprn && x.CourseType == courseType);

        providerCourseType.IsRestrictedProvider.Should().BeTrue();

        context.Audits.Should().HaveCount(1);

        var audit = await context.Audits.SingleAsync();

        audit.EntityType.Should().Be(nameof(ProviderCourseType));
        audit.EntityId.Should().Be(ukprn.ToString());
        audit.UserId.Should().Be(userId);
        audit.UserDisplayName.Should().Be(userDisplayName);
        audit.UserAction.Should().Be("UpdateProviderCourseType");
    }

    [Test]
    public async Task WhenCoursesToAddAreProvided_ThenAddsCourses_AndCreatesTwoAuditEntries()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string userId = "TestUserID";
        string userDisplayName = "Test User";
        CourseType courseType = CourseType.Apprenticeship;

        AddProvider(context, ukprn);
        AddProviderCourseType(context, ukprn, courseType);

        var coursesToAdd = new List<ProviderAllowedCourse>
        {
            new()
            {
                Ukprn = ukprn,
                LarsCode = "LARS001"
            },
            new()
            {
                Ukprn = ukprn,
                LarsCode = "LARS002"
            }
        };

        ProviderCourseTypesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.RestrictProvider(
            ukprn,
            courseType,
            coursesToAdd,
            [],
            userId,
            userDisplayName);

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderCourseTypes.Should().ContainSingle(x =>
            x.CourseType == courseType &&
            x.IsRestrictedProvider);

        provider.ProviderAllowedCourses.Should().HaveCount(2);
        provider.ProviderAllowedCourses.Should().Contain(x => x.LarsCode == "LARS001");
        provider.ProviderAllowedCourses.Should().Contain(x => x.LarsCode == "LARS002");

        context.Audits.Should().HaveCount(2);

        var audits = await context.Audits.ToListAsync();

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderCourseType) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "UpdateProviderCourseType");

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderAllowedCourse) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "UpdateProviderAllowedCourse");
    }

    [Test]
    public async Task WhenCoursesToRemoveAreProvided_ThenRemovesCourses_AndCreatesTwoAuditEntries()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        int ukprn = 12345678;
        string userId = "TestUserID";
        string userDisplayName = "Test User";
        CourseType courseType = CourseType.Apprenticeship;

        AddProvider(context, ukprn);
        AddProviderCourseType(context, ukprn, courseType);
        AddProviderAllowedCourse(context, ukprn, "LARS001", DateTime.UtcNow);
        AddProviderAllowedCourse(context, ukprn, "LARS002", DateTime.UtcNow);

        context.ChangeTracker.Clear();

        var courseToRemove = await context.ProviderAllowedCourses
            .AsNoTracking()
            .SingleAsync(x => x.Ukprn == ukprn && x.LarsCode == "LARS001");

        var coursesToRemove = new List<ProviderAllowedCourse>
        {
            new()
            {
                Id = courseToRemove.Id,
                Ukprn = courseToRemove.Ukprn,
                LarsCode = courseToRemove.LarsCode,
                LastDateStarts = courseToRemove.LastDateStarts
            }
        };

        context.ChangeTracker.Clear();

        ProviderCourseTypesRepository sut = new(context);

        using var activity = new Activity("test");
        activity.Start();

        await sut.RestrictProvider(
            ukprn,
            courseType,
            [],
            coursesToRemove,
            userId,
            userDisplayName);

        activity.Stop();

        var provider = await context.Providers
            .Include(x => x.ProviderCourseTypes)
            .Include(x => x.ProviderAllowedCourses)
            .SingleAsync(x => x.Ukprn == ukprn);

        provider.ProviderCourseTypes.Should().ContainSingle(x =>
            x.CourseType == courseType &&
            x.IsRestrictedProvider);

        provider.ProviderAllowedCourses.Should().ContainSingle(x =>
            x.LarsCode == "LARS002");

        provider.ProviderAllowedCourses.Should().NotContain(x =>
            x.LarsCode == "LARS001");

        context.Audits.Should().HaveCount(2);

        var audits = await context.Audits.ToListAsync();

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderCourseType) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "UpdateProviderCourseType");

        audits.Should().Contain(x =>
            x.EntityType == nameof(ProviderAllowedCourse) &&
            x.EntityId == ukprn.ToString() &&
            x.UserId == userId &&
            x.UserDisplayName == userDisplayName &&
            x.UserAction == "UpdateProviderAllowedCourse");
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

    private static void AddProviderCourseType(
        RoatpDataContext context,
        int ukprn,
        CourseType courseType,
        bool isRestrictedProvider = false)
    {
        context.ProviderCoursesTypes.Add(new ProviderCourseType
        {
            Ukprn = ukprn,
            CourseType = courseType,
            IsRestrictedProvider = isRestrictedProvider
        });

        context.SaveChanges();
    }

    private static void AddProviderAllowedCourse(
        RoatpDataContext context,
        int ukprn,
        string larsCode,
        DateTime? lastDateStarts)
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