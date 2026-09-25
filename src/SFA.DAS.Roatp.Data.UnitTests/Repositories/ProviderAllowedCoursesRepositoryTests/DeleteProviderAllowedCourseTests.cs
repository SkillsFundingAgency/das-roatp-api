using System.Diagnostics;
using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderAllowedCoursesRepositoryTests;

public class DeleteProviderAllowedCourseTests
{
    [Test, AutoData]
    public async Task WhenProviderAllowedCourseExists_ThenRemovesAllowedCourse_AndCreatesAuditEntry(
        int ukprn,
        string larsCode,
        string userId,
        string userDisplayName,
        string userAction)
    {
        // Arrange
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LarsCode = larsCode,
            Provider = new Provider
            {
                Ukprn = ukprn,
                LegalName = "Test Provider"
            }
        };

        context.ProviderAllowedCourses.Add(providerAllowedCourse);
        await context.SaveChangesAsync();

        var sut = new ProviderAllowedCoursesRepository(context);

        using var activity = new Activity("test");
        activity.Start();

        // Act
        await sut.DeleteProviderAllowedCourse(ukprn, larsCode, userId, userDisplayName, userAction, CancellationToken.None);

        // Assert
        context.ProviderAllowedCourses.Should().NotContain(x =>
            x.LarsCode == larsCode &&
            x.Provider.Ukprn == ukprn);

        context.Audits.Should().ContainSingle();

        context.Audits.Single().Should().BeEquivalentTo(new
        {
            EntityType = nameof(ProviderAllowedCourse),
            EntityId = ukprn.ToString(),
            UserId = userId,
            UserDisplayName = userDisplayName,
            UserAction = userAction
        });
    }

    [Test, AutoData]
    public async Task WhenProviderAllowedCourseDoesNotExist_ThenDoesNotCreateAuditEntry(
        int ukprn,
        string larsCode,
        string userId,
        string userDisplayName,
        string userAction)
    {
        // Arrange
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        var sut = new ProviderAllowedCoursesRepository(context);

        // Act
        await sut.DeleteProviderAllowedCourse(ukprn, larsCode, userId, userDisplayName, userAction, CancellationToken.None);

        // Assert
        context.Audits.Should().BeEmpty();
    }
}
