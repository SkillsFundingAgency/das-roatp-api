using System.Collections.Generic;
using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse
{
    public class CreateProviderCourseCommand : IRequest<ValidatedResponse<int>>, IUkprn, ILarsCodeUkprn, ILarsCode, IUserInfo, ICourseType
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int Ukprn { get; set; }
        public string LarsCode { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public bool HasNationalDeliveryOption { get; set; }
        public bool HasOnlineDeliveryOption { get; set; }
        public CourseType CourseType { get; set; }
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
                HasPortableFlexiJobOption = false,
                IsImported = false,
                HasOnlineDeliveryOption = source.HasOnlineDeliveryOption,
            };
    }
}
