using MediatR;
using SFA.DAS.Roatp.Application.Common;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse
{
    public class CreateProviderCourseCommand : IRequest<int>, IUkprn, ILarsCode
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public bool HasNationalDeliveryOption { get; set; }
        public List<ProviderCourseLocationCommandModel> ProviderLocations { get; set; } = new List<ProviderCourseLocationCommandModel>();
        public List<int> SubregionIds { get; set; } = new List<int>();

        public static implicit operator Domain.Entities.ProviderCourse(CreateProviderCourseCommand source)
            => new Domain.Entities.ProviderCourse
            {
                LarsCode = source.LarsCode,
                IsApprovedByRegulator = source.IsApprovedByRegulator,
                StandardInfoUrl = source.StandardInfoUrl,
                ContactUsPhoneNumber = source.ContactUsPhoneNumber,
                ContactUsEmail = source.ContactUsEmail,
                ContactUsPageUrl = source.ContactUsPageUrl,
                HasPortableFlexiJobOption = false,
                IsImported = false
            };
    }
}
