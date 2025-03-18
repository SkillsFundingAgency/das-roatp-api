using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class GetCourseTrainingProvidersCountQueryHandler : IRequestHandler<GetCourseTrainingProvidersCountQuery, ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>>
{
    private readonly IProvidersCountReadRepository _trainingCoursesReadRepository;

    public GetCourseTrainingProvidersCountQueryHandler(IProvidersCountReadRepository trainingCoursesReadRepository)
    {
        _trainingCoursesReadRepository = trainingCoursesReadRepository;
    }

    public async Task<ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>> Handle(GetCourseTrainingProvidersCountQuery query, CancellationToken cancellationToken)
    {
        var results = await _trainingCoursesReadRepository.GetProviderTrainingCourses(query.LarsCodes, query.Longitude, query.Latitude, query.Distance, cancellationToken);

        return new ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>(
            new GetCourseTrainingProvidersCountQueryResult(
                results.Select(a => (CourseTrainingProviderCountModel)a).ToList()
            )
        );
    }
}
