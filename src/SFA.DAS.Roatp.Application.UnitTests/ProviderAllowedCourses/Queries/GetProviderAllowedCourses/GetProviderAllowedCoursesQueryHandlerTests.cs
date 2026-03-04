using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class GetProviderAllowedCoursesQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsExpectedResult(
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepositoryMock,
        GetProviderAllowedCoursesQueryHandler sut,
        GetProviderAllowedCoursesQuery query,
        List<ProviderAllowedCourse> expected,
        CancellationToken cancellationToken)
    {
        providerAllowedCoursesRepositoryMock.Setup(r => r.GetProviderAllowedCourses(query.Ukprn, query.CourseType, cancellationToken)).ReturnsAsync(expected);

        var result = await sut.Handle(query, cancellationToken);

        result.AllowedCourses.Should().BeEquivalentTo(expected, config => config.ExcludingMissingMembers());
    }
}
