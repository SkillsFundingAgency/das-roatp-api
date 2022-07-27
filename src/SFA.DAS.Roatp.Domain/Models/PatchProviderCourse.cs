namespace SFA.DAS.Roatp.Domain.Models
{
    public class PatchProviderCourse: ProviderCourseBase
    {
        public static implicit operator PatchProviderCourse(Entities.ProviderCourse source)
        {
            return new PatchProviderCourse
            {
                IsApprovedByRegulator = source.IsApprovedByRegulator,
                StandardInfoUrl = source.StandardInfoUrl,
                ContactUsPhoneNumber = source.ContactUsPhoneNumber,
                ContactUsEmail = source.ContactUsEmail,
                ContactUsPageUrl = source.ContactUsPageUrl
            };
        }
    }
}
