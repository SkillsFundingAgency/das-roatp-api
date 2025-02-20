using FluentValidation;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistCountForUser;

public class GetShortlistsCountForUserQueryValidator : AbstractValidator<GetShortlistsCountForUserQuery>
{
    public GetShortlistsCountForUserQueryValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}
