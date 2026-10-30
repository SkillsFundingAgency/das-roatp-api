using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourse;

public class GetProviderAvailableCourseQueryHandler(IMediator _mediator, IStandardsReadRepository _standardsReadRepository) : IRequestHandler<GetProviderAvailableCourseQuery, GetProviderAvailableCourseQueryResult>
{
    public async Task<GetProviderAvailableCourseQueryResult> Handle(GetProviderAvailableCourseQuery request, CancellationToken cancellationToken)
    {
        Standard standard = await _standardsReadRepository.GetStandard(request.LarsCode);

        if (standard == null)
            return null;

        GetProviderAvailableCoursesQuery query = new(request.Ukprn, request.CourseType);
        ValidatedResponse<GetProviderAvailableCoursesQueryResult> result = await _mediator.Send(query, cancellationToken);
        if (!result.IsValidResponse)
            return null;

        var course = result.Result.AvailableCourses.FirstOrDefault(c => c.LarsCode == request.LarsCode);

        if (course == null)
            return null;

        return
            new GetProviderAvailableCourseQueryResult()
            {
                LarsCode = course.LarsCode,
                Level = course.Level,
                Title = course.Title,
                Sector = standard.Route,
                IsRegulatedForProvider = standard.IsRegulatedForProvider,
                ApprovalBody = standard.ApprovalBody,
                LastDateStarts = standard.LastDateStarts,
                IfateReferenceNumber = standard.IfateReferenceNumber,
                LearningType = standard.LearningType
            };
    }
}
