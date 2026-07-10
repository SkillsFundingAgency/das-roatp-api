using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.AddProviderToAllowedCourse;

public class AddProviderToAllowedCourseCommandHandler(IStandardsReadRepository _standardsReadRepository, IProviderCourseTypesReadRepository _providerCourseTypesReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<AddProviderToAllowedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(AddProviderToAllowedCourseCommand command, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(command.LarsCode);

        var providerCourseTypes = await _providerCourseTypesReadRepository.GetProviderCourseTypesByUkprn(command.Ukprn);

        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(command.LarsCode, cancellationToken);

        bool hasProviderCourseType = providerCourseTypes.Any(x => x.CourseType == standard.CourseType);

        if (providerAllowedCourses.Any(x => x.Ukprn == command.Ukprn))
    }
}
