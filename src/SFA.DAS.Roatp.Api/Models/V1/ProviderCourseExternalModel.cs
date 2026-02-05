using SFA.DAS.Roatp.Application.ProviderCourse.Queries;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;

namespace SFA.DAS.Roatp.Api.Models.V1;

public class ProviderCourseExternalModel : ProviderCourseModelBase
{
    public int LarsCode { get; set; }
    public static implicit operator ProviderCourseExternalModel(ProviderCourseModelExternal providerCourseModel)
    {
        if (providerCourseModel == null) return null;

        var model = new ProviderCourseExternalModel
        {
            ProviderCourseId = providerCourseModel.ProviderCourseId,
            LarsCode = int.Parse(providerCourseModel.LarsCode),
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