using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourses;

public sealed class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, ValidatedResponse<GetCoursesQueryResult>>
{
    private readonly ITrainingCoursesReadRepository _trainingCoursesReadRepository;

    public GetCoursesQueryHandler(ITrainingCoursesReadRepository trainingCoursesReadRepository)
    {
        _trainingCoursesReadRepository = trainingCoursesReadRepository;
    }

    public async Task<ValidatedResponse<GetCoursesQueryResult>> Handle(GetCoursesQuery query, CancellationToken cancellationToken)
    {
        var results = await _trainingCoursesReadRepository.GetProviderTrainingCourses(query.LarsCodes, query.Longitude, query.Latitude, query.Distance, cancellationToken);

        return new ValidatedResponse<GetCoursesQueryResult>(
            new GetCoursesQueryResult(
                results.Select(a => (CourseModel)a).ToList()
            )
        );
    }
}
