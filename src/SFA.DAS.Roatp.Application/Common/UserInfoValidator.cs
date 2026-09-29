using FluentValidation;

namespace SFA.DAS.Roatp.Application.Common;

public class UserInfoValidator : AbstractValidator<IUserInfo>
{
    public const string UserIdEmptyErrorMessage = "User Id can't be empty";
    public const string UserDisplayNameEmptyErrorMessage = "User display name can't be empty";
    public const string MaximumAllowedLengthErrorMessage = "Must be 256 characters or less";

    public UserInfoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(UserIdEmptyErrorMessage)
            .MaximumLength(256)
            .WithMessage(MaximumAllowedLengthErrorMessage)
            .Matches(Constants.RegularExpressions.ValidCharactersRegex)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage);

        RuleFor(x => x.UserDisplayName)
            .NotEmpty()
            .WithMessage(UserDisplayNameEmptyErrorMessage)
            .MaximumLength(256)
            .WithMessage(MaximumAllowedLengthErrorMessage)
            .Matches(Constants.RegularExpressions.ValidCharactersRegex)
            .WithMessage(ValidationMessages.InvalidCharactersErrorMessage);
    }
}
