using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;

public class UpsertProviderAllowedCourseCommandHandler(IStandardsReadRepository _standardsReadRepository, IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<UpsertProviderAllowedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(UpsertProviderAllowedCourseCommand command, CancellationToken cancellationToken)
    {
        var standard = await _standardsReadRepository.GetStandard(command.LarsCode);

        if (command.IsStartRestricted)
        {
            command.LastDateStarts = DateConstants.StartRestrictedDate;
        }

        await _providerAllowedCoursesRepository.UpsertProviderAllowedCourse(command.Ukprn, command.LarsCode, standard.CourseType, command.LastDateStarts, command.UserId, command.UserDisplayName);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
