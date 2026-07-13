using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetProviderCoursesTimelines;

public class GetProviderCoursesTimelinesQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ReturnsCorrectResult(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        GetProviderCoursesTimelinesQuery query,
        GetProviderCoursesTimelinesQueryHandler sut,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetProviderCoursesTimelines(query.Ukprn, cancellationToken)).ReturnsAsync(TestDataHelper.GetProviderRegistrationDetails());

        ProviderCoursesTimelineModel expected = TestDataHelper.GetProviderRegistrationDetails();

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.Should().BeEquivalentTo(expected);
    }

    [Test, MoqAutoData]
    public async Task Handle_CourseIsNotAllowed_ReturnsLastDateStartsAsNull(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        [Greedy] GetProviderCoursesTimelinesQueryHandler sut,
        GetProviderCoursesTimelinesQuery query,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetProviderCoursesTimelines(query.Ukprn, cancellationToken)).ReturnsAsync(TestDataHelper.GetProviderRegistrationDetailsCourseIsNotAllowed());

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.CourseTypes.FirstOrDefault().Courses.FirstOrDefault().LastDateStarts.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task Handle_ProviderDoesNotProvideCourse_ReturnsLastDateStartsAsNull(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        [Greedy] GetProviderCoursesTimelinesQueryHandler sut,
        GetProviderCoursesTimelinesQuery query,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetProviderCoursesTimelines(query.Ukprn, cancellationToken)).ReturnsAsync(TestDataHelper.GetProviderRegistrationDetailsProviderDoesNotProvideCourse());

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.CourseTypes.FirstOrDefault().Courses.FirstOrDefault().LastDateStarts.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task Handle_ReturnsNullResult(
        [Frozen] Mock<IProviderCoursesTimelineRepository> repoMock,
        [Greedy] GetProviderCoursesTimelinesQueryHandler sut,
        GetProviderCoursesTimelinesQuery query,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(r => r.GetProviderCoursesTimelines(query.Ukprn, cancellationToken)).ReturnsAsync(() => null);

        ProviderCoursesTimelineModel actual = await sut.Handle(query, cancellationToken);

        actual.Should().BeNull();
    }
}
