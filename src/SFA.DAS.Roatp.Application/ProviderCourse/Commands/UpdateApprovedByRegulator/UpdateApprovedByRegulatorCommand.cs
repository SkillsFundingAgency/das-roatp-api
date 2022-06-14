using MediatR;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateApprovedByRegulator
{
    public class UpdateApprovedByRegulatorCommand : IRequest
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public bool IsApprovedByRegulator { get; set; }
    }
}
