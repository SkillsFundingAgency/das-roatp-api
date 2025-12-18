using System;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    public class AddProviderCourseLocationCommand : IRequest<ValidatedResponse<int>>, IUkprn, ILarsCodeUkprn, IUserInfo
    {

        public int Ukprn { get; set; }
        public string LarsCode { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public Guid LocationNavigationId { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }

        public AddProviderCourseLocationCommand(int ukprn, string larsCode, string userId, string userDisplayName, Guid locationNavigationId, bool? hasDayReleaseDeliveryOption, bool? hasBlockReleaseDeliveryOption)
        {
            Ukprn = ukprn;
            LarsCode = larsCode;
            UserId = userId;
            UserDisplayName = userDisplayName;
            LocationNavigationId = locationNavigationId;
            HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption;
            HasBlockReleaseDeliveryOption = hasBlockReleaseDeliveryOption;
        }
    }
}
