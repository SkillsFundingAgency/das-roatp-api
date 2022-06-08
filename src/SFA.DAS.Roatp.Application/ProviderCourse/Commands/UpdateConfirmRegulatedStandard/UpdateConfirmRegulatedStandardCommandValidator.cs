using FluentValidation;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateConfirmRegulatedStandard
{
    public class UpdateConfirmRegulatedStandardCommandValidator : AbstractValidator<UpdateConfirmRegulatedStandardCommand>
    {
        public UpdateConfirmRegulatedStandardCommandValidator()
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
