using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;

public class AddRestrictedCourseCommandHandler(IRestrictedCourseWriteRepository _restrictedCourseWriteRepository, IProviderCoursesReadRepository _providerCoursesReadRepository) : IRequestHandler<AddRestrictedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(AddRestrictedCourseCommand command, CancellationToken cancellationToken)
    {
        var providerCourses = await _providerCoursesReadRepository.GetProviderCoursesByLarsCode(command.LarsCode);

        var restrictedCourse = new RestrictedCourse()
        {
            LarsCode = command.LarsCode,
            ProviderAllowedCourses = providerCourses
                .Where(pc => pc.Provider.ProviderAllowedCourse == null)
                .Select(pc => new ProviderAllowedCourse
                {
                    Ukprn = pc.Provider.Ukprn,
                    LarsCode = command.LarsCode
                })
                .DistinctBy(x => x.Ukprn)
                .ToList()
        };

        await _restrictedCourseWriteRepository.CreateRestrictedCourse(command.LarsCode, restrictedCourse, command.UserId, command.UserDisplayName, AuditEventTypes.CreateRestrictedCourse);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
