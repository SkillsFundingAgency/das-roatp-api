using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProvidersNotAllowed;

public record GetProvidersNotAllowedQuery(string LarsCode) : IRequest<RestrictedCourseDetailsModel>;