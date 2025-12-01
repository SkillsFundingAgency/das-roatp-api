using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class GetCourseTrainingProvidersCountQueryHandler : IRequestHandler<GetCourseTrainingProvidersCountQuery, ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>>
{
    private readonly IProvidersCountReadRepository _trainingCoursesReadRepository;

    public GetCourseTrainingProvidersCountQueryHandler(IProvidersCountReadRepository trainingCoursesReadRepository)
    {
        _trainingCoursesReadRepository = trainingCoursesReadRepository;
    }

    public async Task<ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>> Handle(
        GetCourseTrainingProvidersCountQuery query, CancellationToken cancellationToken)
    {
        var results = await _trainingCoursesReadRepository.GetProviderTrainingCourses(query.LarsCodes, query.Longitude,
            query.Latitude, query.Distance, cancellationToken);

        if (results.Any())
        {
            return new ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>(
                new GetCourseTrainingProvidersCountQueryResult(
                    results.Select(a => (CourseTrainingProviderCountModel)a).ToList()
                )
            );
        }
        return new ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>((GetCourseTrainingProvidersCountQueryResult)null);
    }
}
