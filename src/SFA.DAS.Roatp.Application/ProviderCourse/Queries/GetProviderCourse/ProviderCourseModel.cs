namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;

public class ProviderCourseModel : ProviderCourseModelBase
{
    public string LarsCode { get; set; }

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

    public void AttachCourseDetails(string ifateRefNum, int level, string title, string version,
            string approvalBody)
    {
        IfateReferenceNumber = ifateRefNum;
        Level = level;
        CourseName = title;
        Version = version;
        ApprovalBody = approvalBody;
    }
}
