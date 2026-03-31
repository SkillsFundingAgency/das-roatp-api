using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderCoursesReadRepositoryTests;

public class GetAllShortCoursesTests
{
    private Fixture _fixture = null!;

    [SetUp]
    public void Before_Each_Test()
    {
        _fixture = new();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Test]
    public async Task GetAllShortCourses_ReturnsOnlyShortCourses()
    {
        // seed data
        using var context = RoatpDataContextFactory.CreateInMemoryContext();

        var provider1 = CreateProviderWithUkprn(1001);
        var provider2 = CreateProviderWithUkprn(2001);


        Standard shortCourse1 = CreateStandardWithCourseType(CourseType.ShortCourse);
        Standard shortCourse2 = CreateStandardWithCourseType(CourseType.ShortCourse);
        Standard apprenticeship = CreateStandardWithCourseType(CourseType.Apprenticeship);

        context.ProviderCourses.AddRange(
            CreateProviderCourse(provider1, shortCourse1),
            CreateProviderCourse(provider1, shortCourse2),
            CreateProviderCourse(provider2, apprenticeship)
        );

        await context.SaveChangesAsync();

        // act

        var repo = new ProviderCoursesReadRepository(context);
        var result = await repo.GetAllShortCourses(CancellationToken.None);

        // assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        CollectionAssert.AreEquivalent(
        new[]
        {
                (Ukprn: 1001, Lars: shortCourse1.LarsCode),
                (Ukprn: 1001, Lars: shortCourse2.LarsCode)
        },
        result.Select(r => (r.Ukprn, r.LarsCode)).ToArray());
    }

    [Test]
    public async Task GetAllShortCourses_ReturnsEmpty_WhenNoShortCourses()
    {
        using var context = RoatpDataContextFactory.CreateInMemoryContext();
        var provider = CreateProviderWithUkprn(3001);
        var longStandard = CreateStandardWithCourseType(CourseType.Apprenticeship);

        context.ProviderCourses.Add(CreateProviderCourse(provider, longStandard));

        await context.SaveChangesAsync();

        var repo = new ProviderCoursesReadRepository(context);
        var result = await repo.GetAllShortCourses(CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    private Provider CreateProviderWithUkprn(int ukprn)
        => _fixture
        .Build<Provider>()
        .With(p => p.Ukprn, ukprn)
        .Without(p => p.Locations)
        .Without(p => p.Courses)
        .Without(p => p.ProviderContacts)
        .Without(p => p.ProviderAddress)
        .Without(p => p.ProviderRegistrationDetail)
        .Without(p => p.ProviderCoursesTimelines)
        .Without(p => p.ProviderCourseTypes)
        .Create();

    private Standard CreateStandardWithCourseType(CourseType courseType)
        => _fixture
        .Build<Standard>()
        .With(s => s.CourseType, courseType)
        .Without(s => s.ProviderCourses)
        .Without(s => s.ProviderCoursesTimelines)
        .Without(s => s.ProviderAllowedCourses)
        .Create();

    private ProviderCourse CreateProviderCourse(Provider provider, Standard course)
        => _fixture
        .Build<ProviderCourse>()
        .With(pc => pc.Provider, provider)
        .With(pc => pc.Standard, course)
        .Without(pc => pc.Locations)
        .Without(pc => pc.Versions)
        .Create();
}
