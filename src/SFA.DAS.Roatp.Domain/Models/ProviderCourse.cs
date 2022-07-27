namespace SFA.DAS.Roatp.Domain.Models
{
    public class ProviderCourse: ProviderCourseBase
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int LarsCode { get; set; }
     
       
        public bool IsImported { get; set; } = false;
        public bool HasPortableFlexiJobOption { get; set; }

        public static implicit operator ProviderCourse(Entities.ProviderCourse source)
        {
            if (source == null)
                return null;

            return new ProviderCourse
            {
                Id = source.Id,
                IsApprovedByRegulator = source.IsApprovedByRegulator,
                ProviderId = source.ProviderId,
                LarsCode = source.LarsCode,
                StandardInfoUrl = source.StandardInfoUrl,
                ContactUsEmail = source.ContactUsEmail,
                ContactUsPageUrl = source.ContactUsPageUrl,
                ContactUsPhoneNumber = source.ContactUsPhoneNumber,
                IsImported = source.IsImported,
                HasPortableFlexiJobOption = source.HasPortableFlexiJobOption
            };
        }
    }
}
