namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses
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
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsImported { get; set; } = false;
        public bool? HasNationalDeliveryOption { get; set; }
        public bool? HasHundredPercentEmployerDeliveryOption { get; set; }
        public bool HasPortableFlexiJobOption { get; set; }
        public string Version { get; set; }
        public string ApprovalBody { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocations { get; set; }

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
                IsApprovedByRegulator = providerCourse.IsApprovedByRegulator,
                IsImported = providerCourse.IsImported,
                HasPortableFlexiJobOption = providerCourse.HasPortableFlexiJobOption,
                HasLocations = providerCourse.Locations.Count > 0,
                IsRegulatedForProvider = providerCourse.Standard?.IsRegulatedForProvider ?? false
            };


            return model;
        }
        public void AttachCourseDetails(string ifateRefNum, int level, string title, string version, string approvalBody)
        {
            IfateReferenceNumber = ifateRefNum;
            Level = level;
            CourseName = title;
            Version = version;
            ApprovalBody = approvalBody;
        }
    }
}
