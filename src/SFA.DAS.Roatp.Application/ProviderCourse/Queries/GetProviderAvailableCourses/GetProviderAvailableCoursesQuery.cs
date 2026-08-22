using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;

public record class GetProviderAvailableCoursesQuery(int Ukprn, CourseType CourseType) : IRequest<ValidatedResponse<GetProviderAvailableCoursesQueryResult>>, IUkprn;
