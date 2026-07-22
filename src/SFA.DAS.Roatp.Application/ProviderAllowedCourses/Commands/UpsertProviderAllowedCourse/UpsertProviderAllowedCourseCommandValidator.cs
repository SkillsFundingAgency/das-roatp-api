using FluentValidation;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;

public class UpsertProviderAllowedCourseCommandValidator : AbstractValidator<UpsertProviderAllowedCourseCommand>
{
    public const string InvalidLastDateStarts = "LastDateStarts cannot be after LastDateStarts of the course";
    public UpsertProviderAllowedCourseCommandValidator(IStandardsReadRepository standardsReadRepository)
    {
        Include(new UserInfoValidator());
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                if (!command.LastDateStarts.HasValue)
                {
                    return true;
                }

                var standard = await standardsReadRepository.GetStandard(command.LarsCode);

                return !command.LastDateStarts.HasValue
                       || standard?.LastDateStarts == null
                       || command.LastDateStarts.Value <= standard.LastDateStarts.Value;
            })
            .WithMessage(InvalidLastDateStarts);
    }
}
