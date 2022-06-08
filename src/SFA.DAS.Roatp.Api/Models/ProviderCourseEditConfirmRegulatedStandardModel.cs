using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateConfirmRegulatedStandard;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseEditConfirmRegulatedStandardModel
    {
        public bool? IsApprovedByRegulator { get; set; }
        public static implicit operator UpdateConfirmRegulatedStandardCommand(ProviderCourseEditConfirmRegulatedStandardModel model) =>
            new UpdateConfirmRegulatedStandardCommand
            {
                IsApprovedByRegulator = model.IsApprovedByRegulator,
            };
    }
}
