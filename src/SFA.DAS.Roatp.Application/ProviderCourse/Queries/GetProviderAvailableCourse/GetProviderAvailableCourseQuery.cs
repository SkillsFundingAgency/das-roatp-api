using MediatR;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourse;

public record class GetProviderAvailableCourseQuery(int Ukprn, CourseType CourseType, string LarsCode) : IRequest<GetProviderAvailableCourseQueryResult>;
