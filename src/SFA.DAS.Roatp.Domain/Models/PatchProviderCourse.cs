namespace SFA.DAS.Roatp.Domain.Models
{
    public class PatchProviderCourse
    {
        public bool? IsApprovedByRegulator { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }

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
