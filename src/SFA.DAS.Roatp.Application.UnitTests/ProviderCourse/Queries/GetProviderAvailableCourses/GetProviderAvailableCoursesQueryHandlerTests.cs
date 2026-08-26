using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries.GetProviderAvailableCourses;

public class GetProviderAvailableCoursesQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsNotAllowed_ReturnEmptyResult(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([]);

        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses, Is.Empty);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsRestricted_ThenReturnsListBasedOnAllowedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = true }]);

        string[] allowedLarsCodes = ["1", "2", "3"];
        var allowedCourses = fixture
            .Build<ProviderAllowedCourse>()
            .Without(a => a.LastDateStarts)
            .CreateMany(a => a.LarsCode, allowedLarsCodes)
            .ToList();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync(allowedCourses);

        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync([]);

        var courses = fixture
            .Build<Standard>()
            .CreateMany(a => a.LarsCode, allowedLarsCodes)
            .ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(courses);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Select(c => c.LarsCode), Is.EqualTo(allowedLarsCodes));
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsRestricted_ThenExcludesCoursesAlreadyAdded(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = true }]);

        string[] allowedLarsCodes = ["1", "2", "3"];
        var allowedCourses = fixture
            .Build<ProviderAllowedCourse>()
            .Without(a => a.LastDateStarts)
            .CreateMany(a => a.LarsCode, allowedLarsCodes)
            .ToList();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync(allowedCourses);

        string[] includedLarsCodes = ["1", "2"];
        var includedCourses = fixture
            .Build<Domain.Entities.ProviderCourse>()
            .CreateMany(a => a.LarsCode, includedLarsCodes)
            .ToList();
        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(includedCourses);

        var courses = fixture
            .Build<Standard>()
            .CreateMany(a => a.LarsCode, allowedLarsCodes)
            .ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(courses);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Single().LarsCode, Is.EqualTo("3"));
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsRestricted_ThenExcludesCeasedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = true }]);

        string[] allowedLarsCodes = ["1", "2", "3"];
        DateTime?[] dates = [null, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1)];
        var allowedCourses = fixture
            .Build<ProviderAllowedCourse>()
            .Without(a => a.LastDateStarts)
            .WithValues(a => a.LastDateStarts, dates)
            .CreateMany(a => a.LarsCode, allowedLarsCodes)
            .ToList();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync(allowedCourses);

        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync([]);

        var courses = fixture
            .Build<Standard>()
            .WithValues(a => a.LarsCode, allowedLarsCodes)
            .CreateMany(allowedLarsCodes.Length)
            .ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(courses);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Select(c => c.LarsCode), Is.EqualTo(["1", "2"]));
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsNotRestricted_ThenIncludesUnrestrictedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = false }]);

        string[] unrestrictedLarsCodes = ["1", "2", "3"];
        var courses = fixture
            .Build<Standard>()
            .Without(s => s.RestrictedCourseView)
            .CreateMany(a => a.LarsCode, unrestrictedLarsCodes)
            .ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(courses);

        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync([]);

        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync([]);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Select(c => c.LarsCode), Is.EqualTo(unrestrictedLarsCodes));
    }


    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsNotRestricted_ThenExcludesCeasedUnrestrictedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = false }]);

        string[] unrestrictedLarsCodes = ["1", "2", "3"];
        var courses = fixture
            .Build<Standard>()
            .Without(s => s.RestrictedCourseView)
            .CreateMany(a => a.LarsCode, unrestrictedLarsCodes)
            .ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(courses);

        DateTime?[] dates = [null, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1)];
        var allowedCourses = fixture
            .Build<ProviderAllowedCourse>()
            .WithValues(a => a.LastDateStarts, dates)
            .CreateMany(a => a.LarsCode, unrestrictedLarsCodes)
            .ToList();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync(allowedCourses);

        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync([]);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Select(c => c.LarsCode), Is.EqualTo(["1", "2"]));
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsNotRestricted_ThenExcludesRestrictedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = false }]);

        string[] unrestrictedLarsCodes = ["1", "2", "3"];
        var UnrestrictedCourses = fixture
            .Build<Standard>()
            .Without(s => s.RestrictedCourseView)
            .CreateMany(a => a.LarsCode, unrestrictedLarsCodes);
        string[] restrictedLarsCodes = ["4"];
        var restrictedCourses = fixture
            .Build<Standard>()
            .With(s => s.RestrictedCourseView, new RestrictedCourseView())
            .CreateMany(a => a.LarsCode, restrictedLarsCodes);
        var courses = UnrestrictedCourses.Concat(restrictedCourses).ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(courses);

        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync([]);

        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync([]);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Select(c => c.LarsCode), Is.EqualTo(unrestrictedLarsCodes));
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsNotRestricted_ThenIncludesAllowedUnceasedRestrictedCourses(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = false }]);

        string[] restrictedLarsCodes = ["1", "2", "3"];
        var restrictedCourses = fixture
            .Build<Standard>()
            .With(s => s.RestrictedCourseView, new RestrictedCourseView())
            .CreateMany(a => a.LarsCode, restrictedLarsCodes).ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(restrictedCourses);

        DateTime?[] dates = [null, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1)];
        var allowedCourses = fixture
            .Build<ProviderAllowedCourse>()
            .WithValues(a => a.LastDateStarts, dates)
            .CreateMany(a => a.LarsCode, restrictedLarsCodes)
            .ToList();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync(allowedCourses);

        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync([]);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Select(c => c.LarsCode), Is.EqualTo(["1", "2"]));
    }

    [Test, MoqAutoData]
    public async Task WhenCourseTypeIsAllowed_AndProviderIsNotRestricted_ThenExcludesCoursesAlreadyAdded(
        [Frozen] Mock<IProviderCourseTypesRepository> providerCourseTypesRepoMock,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepoMock,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepoMock,
        [Frozen] Mock<IStandardsReadRepository> standardsRepoMock,
        GetProviderAvailableCoursesQuery query,
        GetProviderAvailableCoursesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        Fixture fixture = GetRecursiveResilientFixture();

        providerCourseTypesRepoMock.Setup(x => x.GetProviderCourseTypesByUkprn(query.Ukprn, cancellationToken)).ReturnsAsync([new ProviderCourseType { CourseType = query.CourseType, IsRestrictedProvider = false }]);

        string[] restrictedLarsCodes = ["1", "2"];
        var restrictedCourses = fixture
            .Build<Standard>()
            .With(s => s.CourseType, query.CourseType)
            .With(s => s.RestrictedCourseView, new RestrictedCourseView())
            .CreateMany(a => a.LarsCode, restrictedLarsCodes)
            .ToList();
        string[] unrestrictedLarsCodes = ["3", "4"];
        var unRestrictedCourses = fixture
            .Build<Standard>()
            .With(s => s.CourseType, query.CourseType)
            .With(s => s.RestrictedCourseView, () => null)
            .CreateMany(a => a.LarsCode, unrestrictedLarsCodes)
            .ToList();
        var allCourses = restrictedCourses.Concat(unRestrictedCourses).ToList();
        standardsRepoMock.Setup(x => x.GetCoursesByCourseType(query.CourseType, cancellationToken)).ReturnsAsync(allCourses);

        var allowedCourses = fixture
            .Build<ProviderAllowedCourse>()
            .With(a => a.LastDateStarts, () => null)
            .CreateMany(a => a.LarsCode, restrictedLarsCodes)
            .ToList();
        providerAllowedCoursesRepoMock.Setup(x => x.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync(allowedCourses);

        var includedCourses = fixture
            .Build<Domain.Entities.ProviderCourse>()
            .CreateMany(pc => pc.LarsCode, "1", "3")
            .ToList();
        providerCoursesReadRepoMock.Setup(x => x.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(includedCourses);

        // Action
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> validationResult = await sut.Handle(query, cancellationToken);

        Assert.That(validationResult.Result.AvailableCourses.Select(c => c.LarsCode), Is.EqualTo(["2", "4"]));
    }

    private static Fixture GetRecursiveResilientFixture()
    {
        Fixture fixture = new();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }
}
