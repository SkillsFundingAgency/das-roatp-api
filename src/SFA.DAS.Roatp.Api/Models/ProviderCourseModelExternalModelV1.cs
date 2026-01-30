using SFA.DAS.Roatp.Application.ProviderCourse.Queries;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;

namespace SFA.DAS.Roatp.Api.Models;

public class ProviderCourseModelExternalModelV1 : ProviderCourseModelBase
{
    public int LarsCode { get; set; }
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