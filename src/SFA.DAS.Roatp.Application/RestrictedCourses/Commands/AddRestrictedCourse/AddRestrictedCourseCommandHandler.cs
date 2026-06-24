using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandHandler(IRestrictedCourseWriteRepository _restrictedCourseWriteRepository, IProviderCoursesReadRepository _providerCoursesReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<AddRestrictedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(AddRestrictedCourseCommand command, CancellationToken cancellationToken)
    {
        var providerCourses = await _providerCoursesReadRepository.GetProviderCoursesByLarsCode(command.LarsCode);

        var providerAllowedCourses = await _providerAllowedCoursesRepository.GetProviderAllowedCoursesByLarsCode(command.LarsCode, cancellationToken);

        var restrictedCourse = new RestrictedCourse()
        {
            LarsCode = command.LarsCode,
            ProviderAllowedCourses = providerCourses
                .Where(pc => !providerAllowedCourses.Any(pac =>
                    pac.Ukprn == pc.Provider.Ukprn &&
                    pac.LarsCode == command.LarsCode))
                .Select(pc => new ProviderAllowedCourse
                {
                    Ukprn = pc.Provider.Ukprn,
                    LarsCode = command.LarsCode
                })
                .ToList()
        };

        await _restrictedCourseWriteRepository.CreateRestrictedCourse(command.LarsCode, restrictedCourse, command.UserId, command.UserDisplayName, AuditEventTypes.CreateRestrictedCourse);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
