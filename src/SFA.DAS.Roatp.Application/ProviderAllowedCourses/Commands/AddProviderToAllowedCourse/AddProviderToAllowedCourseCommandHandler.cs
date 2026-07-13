using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.AddProviderToAllowedCourse;

public class AddProviderToAllowedCourseCommandHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<AddProviderToAllowedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(AddProviderToAllowedCourseCommand command, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(command.LarsCode);

        await _providerAllowedCoursesRepository.AddProviderAllowedCourse(command.Ukprn, command.LarsCode, standard.CourseType, command.LastDateStarts, command.UserId, command.UserDisplayName);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
