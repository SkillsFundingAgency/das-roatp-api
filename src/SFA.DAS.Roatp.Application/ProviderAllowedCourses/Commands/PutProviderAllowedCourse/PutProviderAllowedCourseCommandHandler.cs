using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PutProviderAllowedCourse;

public class PutProviderAllowedCourseCommandHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<PutProviderAllowedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(PutProviderAllowedCourseCommand request, CancellationToken cancellationToken)
    {
        await _providerAllowedCoursesRepository.PutProviderAllowedCourse(request.Ukprn, request.LarsCode, request.LastDateStarts?.Date, request.UserId, request.UserDisplayName);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
