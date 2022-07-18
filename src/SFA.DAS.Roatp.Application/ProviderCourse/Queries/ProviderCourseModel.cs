using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries
{
    public class ProviderCourseModel
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsImported { get; set; } = false;
        public bool? IsConfirmed { get; set; } //required if imported
        public bool? HasNationalDeliveryOption { get; set; }
        public bool? HasHundredPercentEmployerDeliveryOption { get; set; }
       public bool HasPortableFlexiJobOption { get; set; }
        public string Version { get; set; }
        public string ApprovalBody { get; set; }

        public static implicit operator ProviderCourseModel(Domain.Entities.ProviderCourse providerCourse)
        {
            if (providerCourse == null) return null;

            var model = new ProviderCourseModel
            {
                ProviderCourseId = providerCourse.Id,
                LarsCode = providerCourse.LarsCode,
                StandardInfoUrl = providerCourse.StandardInfoUrl,
                ContactUsPhoneNumber = providerCourse.ContactUsPhoneNumber,
                ContactUsEmail = providerCourse.ContactUsEmail,
                ContactUsPageUrl = providerCourse.ContactUsPageUrl,
                IsApprovedByRegulator = providerCourse.IsApprovedByRegulator,
                IsImported = providerCourse.IsImported,
                HasPortableFlexiJobOption = providerCourse.HasPortableFlexiJobOption
                
            };

           
            return model;
        }
        public void UpdateCourseDetails(string ifateRefNum, int level, string title, string version, string approvalBody)
        {
            IfateReferenceNumber = ifateRefNum;
            Level = level;
            CourseName = title;
            Version = version;
            ApprovalBody = approvalBody;
        }
    }
}
