using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;

namespace SFA.DAS.Roatp.Application.Course.GetProvidersNotAllowed.Queries;

public record GetProvidersNotAllowedQuery(string LarsCode) : IRequest<CourseAllowedProvidersModel>;