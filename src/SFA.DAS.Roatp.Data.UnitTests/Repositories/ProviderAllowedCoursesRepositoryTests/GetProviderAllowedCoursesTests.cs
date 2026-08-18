using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderAllowedCoursesRepositoryTests;

public class GetProviderAllowedCoursesTests
{
    [Test]
    public async Task WhenCourseExistsForUkprnAndCourseType_ThenReturnCourse()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        const int ukprn = 12345678;

        var standard = new Standard
        {
            StandardUId = "STD-001",
            LarsCode = "100",
            IfateReferenceNumber = "IFATE001",
            Level = 3,
            Title = "Test Course",
            CourseType = CourseType.Apprenticeship,
            IsActiveAvailable = true
        };

        var providerCourse = new ProviderCourse
        {
            Ukprn = ukprn,
            LarsCode = standard.LarsCode
        };

        context.ProviderAllowedCourses.Add(
            new ProviderAllowedCourse
            {
                Ukprn = ukprn,
                LarsCode = standard.LarsCode,
                Standard = standard,
                ProviderCourse = providerCourse
            });

        await context.SaveChangesAsync();

        var repository = new ProviderAllowedCoursesRepository(context);

        var result = await repository.GetProviderAllowedCourses(
            ukprn,
            CourseType.Apprenticeship,
            CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(1));

        var returnedCourse = result.Single();

        Assert.That(returnedCourse.Ukprn, Is.EqualTo(ukprn));
        Assert.That(returnedCourse.LarsCode, Is.EqualTo("100"));
        Assert.That(returnedCourse.Standard, Is.Not.Null);
        Assert.That(returnedCourse.Standard.CourseType, Is.EqualTo(CourseType.Apprenticeship));
        Assert.That(returnedCourse.Standard.LarsCode, Is.EqualTo("100"));
    }

    [Test]
    public async Task WhenNoCourseExistsForUkprnAndCourseType_ThenReturnEmptyList()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        const int ukprn = 12345678;

        var standard = new Standard
        {
            StandardUId = "STD-001",
            LarsCode = "100",
            IfateReferenceNumber = "IFATE001",
            Level = 3,
            Title = "Test Course",
            CourseType = CourseType.Apprenticeship,
            IsActiveAvailable = true
        };

        context.ProviderAllowedCourses.Add(
            new ProviderAllowedCourse
            {
                Ukprn = ukprn,
                LarsCode = standard.LarsCode,
                Standard = standard
            });

        await context.SaveChangesAsync();

        var repository = new ProviderAllowedCoursesRepository(context);

        var result = await repository.GetProviderAllowedCourses(
            ukprn,
            CourseType.ShortCourse,
            CancellationToken.None);

        Assert.That(result, Is.Empty);
    }
}
