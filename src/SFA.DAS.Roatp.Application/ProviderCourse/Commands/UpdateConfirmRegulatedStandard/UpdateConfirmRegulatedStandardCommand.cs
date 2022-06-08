using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateConfirmRegulatedStandard
{
    public class UpdateConfirmRegulatedStandardCommand : IRequest
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
    }
}
