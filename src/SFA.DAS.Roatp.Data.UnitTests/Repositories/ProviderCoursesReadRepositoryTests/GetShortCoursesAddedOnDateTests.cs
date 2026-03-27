using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Data.UnitTests.Setup;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.UnitTests.Repositories.ProviderCoursesReadRepositoryTests;

public class GetShortCoursesAddedOnDateTests
{
    [Test]
    public async Task GetShortCoursesAddedAfterDate_ReturnsExpectedResult()
    {
        var forDate = DateTime.Today.AddDays(-1);
        using var dataContext = RoatpDataContextFactory.CreateInMemoryContext();
        AddProviderCourses(dataContext, forDate);

        ProviderCoursesReadRepository sut = new(dataContext);

        var actual = await sut.GetShortCoursesAddedOnDate(forDate, default);

        actual.Should().HaveCount(1);
        actual[0].CreatedDate.Date.Should().Be(DateTime.Today.AddDays(-1).Date);
    }

    [Test]
    public async Task GetShortCoursesAddedAfterDate_ReturnsShortCoursesOnly()
    {
        var forDate = DateTime.Today.AddDays(-1);
        using var dataContext = RoatpDataContextFactory.CreateInMemoryContext();
        AddProviderCourses(dataContext, forDate);

        ProviderCoursesReadRepository sut = new(dataContext);

        var actual = await sut.GetShortCoursesAddedOnDate(forDate, default);

        actual.Should().Contain(c => c.Standard.CourseType == CourseType.ShortCourse);
    }

    private static void AddProviderCourses(RoatpDataContext dataContext, DateTime forDate)
    {
        Fixture fixture = new();
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        List<ProviderCourse> providerCourses = fixture.Build<ProviderCourse>()
            .With(c => c.CreatedDate, DateTime.Today)
            .With(c => c.Standard, fixture.Build<Standard>().With(s => s.CourseType, CourseType.ShortCourse).Create())
            .CreateMany()
            .ToList();

        providerCourses.Add(
            fixture
                .Build<ProviderCourse>()
                .With(c => c.CreatedDate, forDate)
                .With(c => c.Standard, fixture.Build<Standard>().With(s => s.CourseType, CourseType.ShortCourse).Create())
                .Create());

        providerCourses.Add(
            fixture
                .Build<ProviderCourse>()
                .With(c => c.CreatedDate, forDate)
                .With(c => c.Standard, fixture.Build<Standard>().With(s => s.CourseType, CourseType.Apprenticeship).Create())
                .Create());

        dataContext.AddRange(providerCourses);
        dataContext.SaveChanges();
    }
}
