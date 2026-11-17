using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.PutProviderCourseTypes;

public class PutProviderCourseTypesCommandHandler(IProviderCourseTypesRepository _providerCourseTypesRepository)
    : IRequestHandler<PutProviderCourseTypesCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(PutProviderCourseTypesCommand command, CancellationToken cancellationToken)
    {
        await _providerCourseTypesRepository.SyncProviderCourseTypes(
            command.Ukprn,
            command.CourseTypes,
            command.UserId,
            command.UserDisplayName,
            AuditEventTypes.UpdateProviderCourseType,
            cancellationToken);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
