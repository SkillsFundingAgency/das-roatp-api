using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;

public class PatchProviderAllowedCourseCommandHandler(IProviderAllowedCoursesRepository _providerAllowedCoursesRepository) : IRequestHandler<PatchProviderAllowedCourseCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(PatchProviderAllowedCourseCommand request, CancellationToken cancellationToken)
    {
        var model = new PatchProviderAllowedCourseModel();

        request.PatchDoc.ApplyTo(model);

        await _providerAllowedCoursesRepository.PatchProviderAllowedCourse(request.Ukprn, request.LarsCode, model.LastDateStarts, request.UserId, request.UserDisplayName);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
