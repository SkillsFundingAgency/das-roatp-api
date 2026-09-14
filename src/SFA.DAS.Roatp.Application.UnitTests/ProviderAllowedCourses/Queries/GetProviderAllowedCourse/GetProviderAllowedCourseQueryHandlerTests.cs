using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;

public class GetProviderAllowedCourseQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenProviderAllowedCourseDoesNotExists_ThenReturnsNull(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCourseRepositoryMock,
        GetProviderAllowedCourseQuery query,
        GetProviderAllowedCourseQueryHandler sut)
    {
        providerAllowedCourseRepositoryMock
            .Setup(x => x.GetProviderAllowedCourse(query.Ukprn, query.LarsCode, CancellationToken.None))
            .ReturnsAsync(() => null);

        GetProviderAllowedCourseQueryResult actual = await sut.Handle(query, CancellationToken.None);

        Assert.That(actual, Is.Null);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenProviderAllowedCourseExists_ThenSetsLastDateStarts(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCourseRepositoryMock,
        GetProviderAllowedCourseQuery query,
        GetProviderAllowedCourseQueryHandler sut,
        ProviderAllowedCourse providerAllowedCourse)
    {
        providerAllowedCourseRepositoryMock
            .Setup(x => x.GetProviderAllowedCourse(query.Ukprn, query.LarsCode, CancellationToken.None))
            .ReturnsAsync(providerAllowedCourse);

        GetProviderAllowedCourseQueryResult actual = await sut.Handle(query, CancellationToken.None);

        Assert.That(actual.LastDateStarts, Is.EqualTo(providerAllowedCourse.LastDateStarts));
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenProviderAllowedCourseExists_AndLastDateStartsIsMinimumDate_ThenSetLastDateStartsToNull(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCourseRepositoryMock,
        GetProviderAllowedCourseQuery query,
        GetProviderAllowedCourseQueryHandler sut,
        ProviderAllowedCourse providerAllowedCourse)
    {
        providerAllowedCourse.LastDateStarts = DateConstants.StartRestrictedDate;
        providerAllowedCourseRepositoryMock
            .Setup(x => x.GetProviderAllowedCourse(query.Ukprn, query.LarsCode, CancellationToken.None))
            .ReturnsAsync(providerAllowedCourse);

        GetProviderAllowedCourseQueryResult actual = await sut.Handle(query, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual.LastDateStarts, Is.Null);
            Assert.That(actual.IsClosedToNewStarts, Is.True);
        });
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenProviderAllowedCourseExists_AndProviderCourseExists_ThenSetsIsActiveToTrue(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCourseRepositoryMock,
        GetProviderAllowedCourseQuery query,
        GetProviderAllowedCourseQueryHandler sut,
        ProviderAllowedCourse providerAllowedCourse,
        Domain.Entities.ProviderCourse providerCourse)
    {
        providerAllowedCourse.ProviderCourse = providerCourse;
        providerAllowedCourseRepositoryMock
            .Setup(x => x.GetProviderAllowedCourse(query.Ukprn, query.LarsCode, CancellationToken.None))
            .ReturnsAsync(providerAllowedCourse);

        GetProviderAllowedCourseQueryResult actual = await sut.Handle(query, CancellationToken.None);

        Assert.That(actual.IsActive, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenProviderAllowedCourseExists_AndProviderCourseDoesNotExists_ThenSetsIsActiveToFalse(
    [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCourseRepositoryMock,
    GetProviderAllowedCourseQuery query,
    GetProviderAllowedCourseQueryHandler sut,
    ProviderAllowedCourse providerAllowedCourse)
    {
        providerAllowedCourse.ProviderCourse = null;
        providerAllowedCourseRepositoryMock
            .Setup(x => x.GetProviderAllowedCourse(query.Ukprn, query.LarsCode, CancellationToken.None))
            .ReturnsAsync(providerAllowedCourse);

        GetProviderAllowedCourseQueryResult actual = await sut.Handle(query, CancellationToken.None);

        Assert.That(actual.IsActive, Is.False);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenProviderAllowedCourseExists_AndCourseIsRestricted_ThenSetsIsCourseRestrictedToTrue(
    [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCourseRepositoryMock,
    GetProviderAllowedCourseQuery query,
    GetProviderAllowedCourseQueryHandler sut,
    Standard standard,
    RestrictedCourseView restrictedCourseView,
    ProviderAllowedCourse providerAllowedCourse)
    {
        standard.RestrictedCourseView = restrictedCourseView;
        providerAllowedCourse.Standard = standard;
        providerAllowedCourseRepositoryMock
            .Setup(x => x.GetProviderAllowedCourse(query.Ukprn, query.LarsCode, CancellationToken.None))
            .ReturnsAsync(providerAllowedCourse);

        GetProviderAllowedCourseQueryResult actual = await sut.Handle(query, CancellationToken.None);

        Assert.That(actual.IsCourseRestricted, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenProviderAllowedCourseExists_AndCourseIsNotRestricted_ThenSetsIsCourseRestrictedToFalse(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCourseRepositoryMock,
        GetProviderAllowedCourseQuery query,
        GetProviderAllowedCourseQueryHandler sut,
        Standard standard,
        ProviderAllowedCourse providerAllowedCourse)
    {
        standard.RestrictedCourseView = null;
        providerAllowedCourse.Standard = standard;
        providerAllowedCourseRepositoryMock
            .Setup(x => x.GetProviderAllowedCourse(query.Ukprn, query.LarsCode, CancellationToken.None))
            .ReturnsAsync(providerAllowedCourse);
        GetProviderAllowedCourseQueryResult actual = await sut.Handle(query, CancellationToken.None);
        Assert.That(actual.IsCourseRestricted, Is.False);
    }
}
