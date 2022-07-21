using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Models
{
    public class PatchProviderCourse: ProviderCourseBase
    {
  
        public static implicit operator PatchProviderCourse(ProviderCourse source)
        {
            return new PatchProviderCourse
            {
                IsApprovedByRegulator = source.IsApprovedByRegulator, 
                // StandardInfoUrl  = source.StandardInfoUrl,
                // ContactUsPhoneNumber = source.ContactUsPhoneNumber,
                // ContactUsEmail = source.ContactUsEmail,
                // ContactUsPageUrl = source.ContactUsPageUrl
            };
        }
    }

    public class PatchProviderCourseRequest
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        
        public List<PatchOperation> Data { get; set; }
    }
}
