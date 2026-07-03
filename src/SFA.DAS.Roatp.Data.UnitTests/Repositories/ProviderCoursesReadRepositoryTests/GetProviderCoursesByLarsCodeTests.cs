using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderCoursesReadRepositoryTests;

public class GetProviderCoursesByLarsCodeTests
{
    [Test]
    public async Task WhenCoursesExistForLarsCode_ThenReturnMatchingCourses()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        const string matchingLarsCode = "100";
        const string otherLarsCode = "200";

        var provider1 = new Provider { Ukprn = 1001, LegalName = "Test1" };
        var provider2 = new Provider { Ukprn = 2001, LegalName = "Test2" };
        var provider3 = new Provider { Ukprn = 3001, LegalName = "Test3" };

        context.ProviderCourses.AddRange(
            new ProviderCourse { LarsCode = matchingLarsCode, Provider = provider1 },
            new ProviderCourse { LarsCode = matchingLarsCode, Provider = provider2 },
            new ProviderCourse { LarsCode = otherLarsCode, Provider = provider3 });

        await context.SaveChangesAsync();

        var repository = new ProviderCoursesReadRepository(context);

        var result = await repository.GetProviderCoursesByLarsCode(matchingLarsCode);

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task WhenNoCoursesExistForLarsCode_ThenReturnEmptyList()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        context.ProviderCourses.AddRange(
            new ProviderCourse
            {
                LarsCode = "100",
                Provider = new Provider { Ukprn = 1001, LegalName = "Test1" }
            },
            new ProviderCourse
            {
                LarsCode = "200",
                Provider = new Provider { Ukprn = 2001, LegalName = "Test2" }
            });

        await context.SaveChangesAsync();

        var repository = new ProviderCoursesReadRepository(context);

        var result = await repository.GetProviderCoursesByLarsCode("300");

        Assert.That(result, Is.Empty);
    }
}
