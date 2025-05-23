using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System;

namespace SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails
{
    public class UpdateProviderLocationDetailsCommand : IRequest<ValidatedResponse<bool>>, IUkprn, IUserInfo
    {
        public int Ukprn { get; set; }
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string LocationName { get; set; }
    }
}
