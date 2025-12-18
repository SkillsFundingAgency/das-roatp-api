using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse
{
    public class ProviderCourseModelExternal : ProviderCourseModelBase
    {
        public int LarsCode { get; set; }

        public static implicit operator ProviderCourseModelExternal(ProviderCourseModel providerCourseModel)
        {
            if (providerCourseModel == null) return null;

            var model = new ProviderCourseModelExternal
            {
                ProviderCourseId = providerCourseModel.ProviderCourseId,
                LarsCode = int.TryParse(providerCourseModel.LarsCode, out var l) ? l : 0,
                StandardInfoUrl = providerCourseModel.StandardInfoUrl,
                ContactUsPhoneNumber = providerCourseModel.ContactUsPhoneNumber,
                ContactUsEmail = providerCourseModel.ContactUsEmail,
                IsApprovedByRegulator = providerCourseModel.IsApprovedByRegulator,
                IsImported = providerCourseModel.IsImported,
                HasPortableFlexiJobOption = providerCourseModel.HasPortableFlexiJobOption,
                HasLocations = providerCourseModel.HasLocations,
                IsRegulatedForProvider = providerCourseModel.IsRegulatedForProvider,
                IfateReferenceNumber = providerCourseModel.IfateReferenceNumber,
                Level = providerCourseModel.Level,
                CourseName = providerCourseModel.CourseName,
                Version = providerCourseModel.Version,
                ApprovalBody = providerCourseModel.ApprovalBody,
            };

            return model;
        }
    }
}
