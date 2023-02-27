using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails
{
    public class UpdateProviderLocationDetailsCommand : IRequest<ValidatedResponse<bool>>, IUkprn, IUserInfo
    {
        public int Ukprn { get; set; }
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string LocationName { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
