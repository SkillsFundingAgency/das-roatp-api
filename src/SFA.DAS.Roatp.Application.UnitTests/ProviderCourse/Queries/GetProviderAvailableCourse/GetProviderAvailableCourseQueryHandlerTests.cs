using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common.Models;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourse;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries.GetProviderAvailableCourse;

public class GetProviderAvailableCourseQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenCourseDoesNotExist_ThenReturnsNull(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] GetProviderAvailableCourseQueryHandler sut,
        GetProviderAvailableCourseQuery request)
    {
        standardsReadRepository
            .Setup(x => x.GetStandard(request.LarsCode))
            .ReturnsAsync((Standard)null);

        var result = await sut.Handle(request, CancellationToken.None);

        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenAvailableCoursesResponseIsInvalid_ThenReturnsNull(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] GetProviderAvailableCourseQueryHandler sut,
        GetProviderAvailableCourseQuery request,
        CancellationToken cancellationToken)
    {
        standardsReadRepository
            .Setup(x => x.GetStandard(request.LarsCode))
            .ReturnsAsync(new Standard());

        mediator
            .Setup(x => x.Send(
                It.IsAny<GetProviderAvailableCoursesQuery>(),
                cancellationToken))
            .ReturnsAsync(new ValidatedResponse<GetProviderAvailableCoursesQueryResult>(
                [new ValidationFailure("Property", "Error message")]));

        var result = await sut.Handle(request, cancellationToken);

        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenAvailableCoursesIsEmpty_ThenReturnsNull(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] GetProviderAvailableCourseQueryHandler sut,
        GetProviderAvailableCourseQuery request,
        CancellationToken cancellationToken)
    {
        standardsReadRepository
            .Setup(x => x.GetStandard(request.LarsCode))
            .ReturnsAsync(new Standard());

        mediator
            .Setup(x => x.Send(
                It.IsAny<GetProviderAvailableCoursesQuery>(),
                cancellationToken))
            .ReturnsAsync(new ValidatedResponse<GetProviderAvailableCoursesQueryResult>(
                new GetProviderAvailableCoursesQueryResult
                {
                    AvailableCourses = new List<CourseBasicModel>()
                }));

        var result = await sut.Handle(request, cancellationToken);

        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenLarsCodeDoesNotExistInAvailableCourses_ThenReturnsNull(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] GetProviderAvailableCourseQueryHandler sut,
        CancellationToken cancellationToken)
    {
        var request = new GetProviderAvailableCourseQuery(1234, Domain.Models.CourseType.Apprenticeship, "LARS1");

        standardsReadRepository
            .Setup(x => x.GetStandard(request.LarsCode))
            .ReturnsAsync(new Standard());

        mediator
            .Setup(x => x.Send(
                It.IsAny<GetProviderAvailableCoursesQuery>(),
                cancellationToken))
            .ReturnsAsync(new ValidatedResponse<GetProviderAvailableCoursesQueryResult>(
                new GetProviderAvailableCoursesQueryResult
                {
                    AvailableCourses = new List<CourseBasicModel>
                    {
                        new("LARS2", 4, "Different course")
                    }
                }));

        var result = await sut.Handle(request, cancellationToken);

        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenLarsCodeDoesExistInAvailableCourses_ThenCourseDetailsIsReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] GetProviderAvailableCourseQueryHandler sut,
        GetProviderAvailableCourseQuery request,
        string title,
        string route,
        string approvalBody)
    {
        var lastDateStarts = System.DateTime.UtcNow.Date.AddDays(1);

        standardsReadRepository
            .Setup(x => x.GetStandard(request.LarsCode))
            .ReturnsAsync(new Standard
            {
                Route = route,
                IsRegulatedForProvider = true,
                ApprovalBody = approvalBody,
                LastDateStarts = lastDateStarts,
                IfateReferenceNumber = "Test1",
                LearningType = LearningType.Apprenticeship
            });

        mediator
            .Setup(x => x.Send(
                It.IsAny<GetProviderAvailableCoursesQuery>(),
                CancellationToken.None))
            .ReturnsAsync(new ValidatedResponse<GetProviderAvailableCoursesQueryResult>(
                new GetProviderAvailableCoursesQueryResult
                {
                    AvailableCourses = new List<CourseBasicModel>
                    {
                        new(request.LarsCode, 4, title)
                    }
                }));

        var result = await sut.Handle(request, CancellationToken.None);

        result.Should().BeEquivalentTo(new GetProviderAvailableCourseQueryResult
        {
            LarsCode = request.LarsCode,
            Level = 4,
            Title = title,
            Sector = route,
            IsRegulatedForProvider = true,
            ApprovalBody = approvalBody,
            LastDateStarts = lastDateStarts,
            IfateReferenceNumber = "Test1",
            LearningType = LearningType.Apprenticeship
        });
    }
}
