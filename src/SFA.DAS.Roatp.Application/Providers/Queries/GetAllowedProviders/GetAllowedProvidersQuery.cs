using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetAllowedProviders;

public record GetAllowedProvidersQuery(string LarsCode) : IRequest<RestrictedCourseDetailsModel>;
