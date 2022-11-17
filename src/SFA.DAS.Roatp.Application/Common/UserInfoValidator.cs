using FluentValidation;

namespace SFA.DAS.Roatp.Application.Common
{
    public class UserInfoValidator : AbstractValidator<IUserInfo>
    {
        public const string UserIdEmptyErrorMessage = "User Id can't be empty";
        public const string UserDisplayNameEmptyErrorMessage = "User display name can't be empty";

        public UserInfoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage(UserIdEmptyErrorMessage);

            RuleFor(x => x.UserDisplayName)
               .NotEmpty()
               .WithMessage(UserDisplayNameEmptyErrorMessage);
        }
    }
}
