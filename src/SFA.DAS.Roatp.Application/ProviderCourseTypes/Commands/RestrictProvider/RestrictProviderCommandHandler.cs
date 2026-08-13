using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.RestrictProvider;

public class RestrictProviderCommandHandler(IProviderCoursesReadRepository providerCoursesReadRepository, IProviderAllowedCoursesRepository providerAllowedCoursesRepository, IProviderCourseTypesRepository providerCourseTypesRepository) : IRequestHandler<RestrictProviderCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(RestrictProviderCommand command, CancellationToken cancellationToken)
    {
        var providerCourses = await providerCoursesReadRepository.GetAllProviderCourses(command.Ukprn);

        var coursesToAdd = providerCourses
            .Where(pc =>
                pc.Standard.CourseType == command.CourseType &&
                pc.ProviderAllowedCourse == null)
            .Select(pc => new ProviderAllowedCourse
            {
                Ukprn = pc.Ukprn ?? command.Ukprn,
                LarsCode = pc.LarsCode
            })
            .ToList();

        var providerAllowedCourses = await providerAllowedCoursesRepository.GetProviderAllowedCourses(command.Ukprn, command.CourseType, cancellationToken);

        var coursesToRemove = providerAllowedCourses
            .Where(pac => !providerCourses.Any(pc => pc.Ukprn == pac.Ukprn && pc.LarsCode == pac.LarsCode)).ToList();

        await providerCourseTypesRepository.RestrictProvider(command.Ukprn, command.CourseType, coursesToAdd, coursesToRemove, command.UserId, command.UserDisplayName, cancellationToken);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
