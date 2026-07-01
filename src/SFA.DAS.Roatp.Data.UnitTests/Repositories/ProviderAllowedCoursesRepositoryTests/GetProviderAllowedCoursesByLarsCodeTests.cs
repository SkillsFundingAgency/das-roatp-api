using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderAllowedCoursesRepositoryTests;

public class GetProviderAllowedCoursesByLarsCodeTests
{
    [Test]
    public async Task WhenCoursesExistForLarsCode_ThenReturnMatchingCourses()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        const string matchingLarsCode = "100";
        const string otherLarsCode = "200";

        context.ProviderAllowedCourses.AddRange(
            new ProviderAllowedCourse { LarsCode = matchingLarsCode },
            new ProviderAllowedCourse { LarsCode = matchingLarsCode },
            new ProviderAllowedCourse { LarsCode = otherLarsCode });

        await context.SaveChangesAsync();

        var repository = new ProviderAllowedCoursesRepository(context);

        var result = await repository.GetProviderAllowedCoursesByLarsCode(matchingLarsCode, CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task WhenNoCoursesExistForLarsCode_ThenReturnEmptyList()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        context.ProviderAllowedCourses.AddRange(
            new ProviderAllowedCourse { LarsCode = "100" },
            new ProviderAllowedCourse { LarsCode = "200" });

        await context.SaveChangesAsync();

        var repository = new ProviderAllowedCoursesRepository(context);

        var result = await repository.GetProviderAllowedCoursesByLarsCode("300", CancellationToken.None);

        Assert.That(result, Is.Empty);
    }
}
