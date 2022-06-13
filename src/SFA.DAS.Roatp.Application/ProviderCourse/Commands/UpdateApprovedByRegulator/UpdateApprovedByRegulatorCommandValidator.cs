using FluentValidation;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateApprovedByRegulator
{
    public class UpdateApprovedByRegulatorCommandValidator : AbstractValidator<UpdateApprovedByRegulatorCommand>
    {
        public UpdateApprovedByRegulatorCommandValidator()
        {
            RuleFor(c => c.Ukprn)
                .GreaterThan(10000000)
                .LessThan(99999999);
            RuleFor(c => c.LarsCode)
                .GreaterThan(0);
            RuleFor(c => c.IsApprovedByRegulator)
                .NotEmpty();
        }
    }
}
