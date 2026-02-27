using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;

public class ProviderCourseModelExternal : ProviderCourseModelBase
{
    public string LarsCode { get; set; }

    public static implicit operator ProviderCourseModelExternal(ProviderCourseModel providerCourseModel)
    {
        if (providerCourseModel == null) return null;

        var model = new ProviderCourseModelExternal
        {
            ProviderCourseId = providerCourseModel.ProviderCourseId,
            LarsCode = providerCourseModel.LarsCode,
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
            ApprenticeshipType = providerCourseModel.ApprenticeshipType
        };

        return model;
    }
}