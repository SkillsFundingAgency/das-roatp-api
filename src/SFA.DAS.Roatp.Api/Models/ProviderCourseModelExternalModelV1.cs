using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.Models;

public class ProviderCourseModelExternalModelV1
{
    public int LarsCode { get; set; }
    public int ProviderCourseId { get; set; }
    public string CourseName { get; set; }
    public CourseType? CourseType { get; set; }
    public int Level { get; set; }
    public string IfateReferenceNumber { get; set; }
    public string StandardInfoUrl { get; set; }
    public string ContactUsPhoneNumber { get; set; }
    public string ContactUsEmail { get; set; }
    public bool? IsApprovedByRegulator { get; set; }
    public bool? HasNationalDeliveryOption { get; set; }
    public bool? HasHundredPercentEmployerDeliveryOption { get; set; }
    public bool HasPortableFlexiJobOption { get; set; }
    public bool HasOnlineDeliveryOption { get; set; }
    public string ApprovalBody { get; set; }
    public bool IsRegulatedForProvider { get; set; }
    public bool HasLocations { get; set; }


    public static implicit operator ProviderCourseModelExternalModelV1(ProviderCourseModelExternal providerCourseModel)
    {
        if (providerCourseModel == null) return null;

        var model = new ProviderCourseModelExternalModelV1
        {
            ProviderCourseId = providerCourseModel.ProviderCourseId,
            LarsCode = int.TryParse(providerCourseModel.LarsCode, out var l) ? l : 0,
            StandardInfoUrl = providerCourseModel.StandardInfoUrl,
            ContactUsPhoneNumber = providerCourseModel.ContactUsPhoneNumber,
            ContactUsEmail = providerCourseModel.ContactUsEmail,
            IsApprovedByRegulator = providerCourseModel.IsApprovedByRegulator,
            HasPortableFlexiJobOption = providerCourseModel.HasPortableFlexiJobOption,
            HasOnlineDeliveryOption = providerCourseModel.HasOnlineDeliveryOption,
            HasLocations = providerCourseModel.HasLocations,
            IsRegulatedForProvider = providerCourseModel.IsRegulatedForProvider,
            IfateReferenceNumber = providerCourseModel.IfateReferenceNumber,
            Level = providerCourseModel.Level,
            CourseName = providerCourseModel.CourseName,
            ApprovalBody = providerCourseModel.ApprovalBody,
            CourseType = providerCourseModel.CourseType,
        };

        return model;
    }
}