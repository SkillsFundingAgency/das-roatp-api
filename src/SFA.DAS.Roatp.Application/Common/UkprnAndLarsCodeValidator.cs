using FluentValidation;

namespace SFA.DAS.Roatp.Application.Common;

public class UkprnAndLarsCodeValidator : AbstractValidator<IUkprnAndLarsCodeValidator>
{
    public UkprnAndLarsCodeValidator(
        IValidator<IUkprn> ukprnValidator,
        IValidator<ILarsCode> larsCodeValidator)
    {
        Include(ukprnValidator);
        Include(larsCodeValidator);
    }
}
