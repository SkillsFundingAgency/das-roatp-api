using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateApprovedByRegulator;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseEditConfirmRegulatedStandardModel
    {
        public bool? IsApprovedByRegulator { get; set; }
        public static implicit operator UpdateApprovedByRegulatorCommand(ProviderCourseEditConfirmRegulatedStandardModel model) =>
            new UpdateApprovedByRegulatorCommand
            {
                IsApprovedByRegulator = (bool)model.IsApprovedByRegulator,
            };
    }
}
