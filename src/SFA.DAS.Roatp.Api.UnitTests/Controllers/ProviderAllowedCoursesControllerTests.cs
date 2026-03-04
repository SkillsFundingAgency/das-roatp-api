using System.Threading;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;

public class ProviderAllowedCoursesControllerTests
{
    [Test, MoqAutoData]
    public void GetAllowedCourses_ReturnsOkResult(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        int ukprn,
        CourseType courseType,
        GetProviderAllowedCoursesQueryResult expected,
        CancellationToken cancellationToken)
    {
        _mediatorMock.Setup(m => m.Send(It.Is<GetProviderAllowedCoursesQuery>(q => q.Ukprn == ukprn && q.CourseType == courseType), cancellationToken))
            .ReturnsAsync(expected);

        var result = sut.GetAllowedCourses(ukprn, courseType, cancellationToken).GetAwaiter().GetResult();

        result.As<OkObjectResult>().Value.Should().Be(expected);
    }
}
