using FluentValidation;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Common;

public class LarsCodeValidator : AbstractValidator<ILarsCode>
{
    public const string InvalidMessage = "Larscode must be provided";
    public const string NotFoundMessage = "Larscode not found";

    //MFCMFC not sure if this change is enough - - should I check for larscode bigger than 10 chars?
    public LarsCodeValidator(IStandardsReadRepository standardsReadRepository)
    {
        RuleFor(x => x.LarsCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(InvalidMessage)
            .MustAsync(async (larsCode, cancellation) =>
            {
                var standard = await standardsReadRepository.GetStandard(larsCode);
                return standard != null;
            })
            .WithMessage(NotFoundMessage);
    }
}