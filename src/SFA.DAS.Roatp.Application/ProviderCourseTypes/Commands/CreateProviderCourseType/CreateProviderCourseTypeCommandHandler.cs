using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;

public class CreateProviderCourseTypeCommandHandler(IProviderCourseTypesRepository _providerCourseTypesRepository) : IRequestHandler<CreateProviderCourseTypeCommand, ValidatedResponse<Unit>>
{
    public async Task<ValidatedResponse<Unit>> Handle(CreateProviderCourseTypeCommand command, CancellationToken cancellationToken)
    {
        var providerCourseTypes = new List<ProviderCourseType>();

        foreach (var courseType in command.CourseTypes)
        {
            var providerCourseType = new ProviderCourseType()
            {
                Ukprn = command.Ukprn,
                CourseType = courseType
            };

            providerCourseTypes.Add(providerCourseType);
        }

        await _providerCourseTypesRepository.CreateProviderCourseType(providerCourseTypes, command.UserId, command.UserDisplayName, command.Ukprn, AuditEventTypes.CreateProviderCourseType, cancellationToken);

        return new ValidatedResponse<Unit>(Unit.Value);
    }
}
