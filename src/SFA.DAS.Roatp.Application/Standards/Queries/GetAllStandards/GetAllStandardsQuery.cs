using MediatR;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards;

public class GetAllStandardsQuery : IRequest<GetAllStandardsQueryResult>
{
    public CourseType? CourseType { get; }
    public GetAllStandardsQuery(CourseType? courseType)
    {
        CourseType = courseType;
    }
}