namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse
{
    public class ProviderCourseModelExternal : ProviderCourseModelBase
    {
        public int LarsCode { get; set; }

        public static implicit operator ProviderCourseModelExternal(Domain.Entities.ProviderCourse providerCourse)
        {
            if (providerCourse == null) return null;

            var model = new ProviderCourseModelExternal
            {
                ProviderCourseId = providerCourse.Id,
                LarsCode = int.TryParse(providerCourse.LarsCode, out var l) ? l : 0,
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
