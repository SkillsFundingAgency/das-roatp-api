using System.Collections.Generic;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseAddModel
    {
        public bool? IsApprovedByRegulator { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public bool HasNationalDeliveryOption { get; set; }
        public List<ProviderCourseLocationCommandModel> ProviderLocations { get; set; }
        public List<int> SubregionIds { get; set; }

        public static implicit operator CreateProviderCourseCommand(ProviderCourseAddModel source)
            => new CreateProviderCourseCommand
            {
                IsApprovedByRegulator = source.IsApprovedByRegulator,
                StandardInfoUrl = source.StandardInfoUrl,
                ContactUsPhoneNumber = source.ContactUsPhoneNumber,
                ContactUsEmail = source.ContactUsEmail,
                ContactUsPageUrl = source.ContactUsPageUrl,
                HasNationalDeliveryOption = source.HasNationalDeliveryOption,
                ProviderLocations = source.ProviderLocations,
                SubregionIds = source.SubregionIds
            };
    }
}