using FluentValidation;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

public class GetShortlistsForUserQueryValidator : AbstractValidator<GetShortlistsForUserQuery>
{
    public GetShortlistsForUserQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
