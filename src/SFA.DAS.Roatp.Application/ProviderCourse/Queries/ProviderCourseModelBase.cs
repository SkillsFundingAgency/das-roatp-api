namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries;

public class ProviderCourseModelBase
{
    public int ProviderCourseId { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string StandardInfoUrl { get; set; }
    public string ContactUsPhoneNumber { get; set; }
    public string ContactUsEmail { get; set; }
    public bool? IsApprovedByRegulator { get; set; }
    public bool IsImported { get; set; } = false;
    public bool? HasNationalDeliveryOption { get; set; }
    public bool? HasHundredPercentEmployerDeliveryOption { get; set; }
    public bool HasPortableFlexiJobOption { get; set; }
    public bool HasOnlineDeliveryOption { get; set; }
    public string ApprovalBody { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public bool HasLocations { get; set; }
}
