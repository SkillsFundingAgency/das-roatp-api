using MediatR;
using SFA.DAS.Roatp.Application.Common.Models;

namespace SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;

public record GetAllowedProvidersQuery(string LarsCode) : IRequest<CourseAllowedProvidersModel>;
